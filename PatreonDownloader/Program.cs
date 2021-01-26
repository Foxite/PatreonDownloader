using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PatreonDownloader.CookieExtraction;
using PatreonDownloader.LinkScraping;

namespace PatreonDownloader {
	public class Program {
		private static string DataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PatreonDownloader");

		private static void Main(string[] args) {
			try {
				Directory.CreateDirectory(DataFolder);

				string sessionToken = null;

				CookieExtractor[] cookieExtractors = new CookieExtractor[] {
					new ChromeCookieExtractor(),
					new FirefoxCookieExtractor()
				};

				while (sessionToken == null) {
					string[] options = new string[cookieExtractors.Length + 1];
					options[0] = "Manually paste session token";
					for (int i = 0; i < cookieExtractors.Length; i++) {
						options[i + 1] = $"Acquire session token from {cookieExtractors[i].Name}";
					}
					int choice = Util.ConsoleChoiceMenu("Enter an option:", options);

					if (choice == 0) {
						Console.Write("Paste session token: ");
						sessionToken = Console.ReadLine();
					} else {
						try {
							sessionToken = cookieExtractors[choice - 1].GetPatreonSessionToken();
						} catch (CookieExtractorException e) {
							Console.WriteLine("Could not extract a cookie. Please try another way.");
							Console.WriteLine(e.Message);
						}
					}
				}

				string backupFile = Path.Combine(DataFolder, "posts.json"); // Unfortunately, no SpecialFolder.Downloads.

				var cookieContainer = new CookieContainer();
				cookieContainer.Add(new Uri("https://www.patreon.com"), new Cookie("session_id", sessionToken));
				using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				using var client = new HttpClient(handler);
				client.DefaultRequestHeaders.Add("User-Agent", "PatreonDownloader/1.0");

				Directory.CreateDirectory(DataFolder);

				if (File.Exists(backupFile)) {
					List<PostPage> list = JsonConvert.DeserializeObject<List<PostPage>>(File.ReadAllText(backupFile));

					string question = "A backup file exists.";

					string[] options;
					if (list[^1].Links != null) {
						options = new string[3];
						options[2] = "Continue downloading post data where left off";
					} else {
						options = new string[2];
						question += " The local backup indicates that there are no more pages to download.";
					}

					question += "\nRe-downloading may be necessary if the backup is too old. Media URLs expire after a certain amount of time.";
					question += "\nDo you want to:";

					options[0] = "Download all images in the backup";
					options[1] = "Delete the backup file and re-download all post data";

					int choice = Util.ConsoleChoiceMenu(question, options);

					switch (choice) {
						case 0:
							DownloadMedia(client, cookieContainer, list);
							break;
						case 1:
							File.Delete(backupFile);
							DownloadAllPosts(client, null, backupFile);
							break;
						case 2:
							DownloadAllPosts(client, list, backupFile);
							break;
					}
				} else {
					Console.WriteLine("No local backup file exists. Downloading all post data to a local json file.");
					DownloadAllPosts(client, null, backupFile);
				}
			}
#if !DEBUG
			catch (Exception e) {
				File.WriteAllText(Path.Combine(DataFolder, "error.log"), e.ToStringDemystified());
				LogError("An unknown error has occured. Details have been saved in Documents/PatreonDownloader/error.log. Please forward it to the developer.");
			}
#endif
			finally {
				Console.ReadKey();
			}
		}

		private static void DownloadMedia(HttpClient client, CookieContainer cookies, List<PostPage> posts) {
			Dictionary<string, List<PostPageIncluded>> inclusions = new Dictionary<string, List<PostPageIncluded>>();
			foreach (PostPageIncluded item in posts.SelectMany(page => page.Included)) {
				if (inclusions.TryGetValue(item.Id, out List<PostPageIncluded> ppiList)) {
					ppiList.Add(item);
				} else {
					inclusions[item.Id] = new List<PostPageIncluded>() { item };
				}
			}

			LinkDownloader[] downloaders = new LinkDownloader[] {
				new DropboxDownloader()
			};

			Console.WriteLine("If you want to download ALL saved media, press enter. Otherwise, enter the YYYY-MM-DD of the date that you want to resume downloading from. (Media is downloaded in reverse order of submitting.)");
			DateTime? skipAfter = null;
			bool @continue = false;
			while (!@continue) {
				string ymd = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(ymd)) {
					@continue = true;
				} else if (DateTime.TryParseExact(ymd, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime result)) {
					skipAfter = result;
					@continue = true;
				} else {
					Console.WriteLine("Please press enter, or enter the date in YYYY-MM-DD.");
				}
			}

			int postI = 1;
			foreach (PostPageData post in posts.SelectMany(page => page.Data).Where(post => post.Attributes.PostType == "image_file")) {
				if (skipAfter.HasValue && post.Attributes.PublishedAt > skipAfter) {
					postI++;
					continue;
				}
				DirectoryInfo directory = null;

				#region Media downloading
				IEnumerable<PostPageIncludedMedia> media = post.Relationships.Media.Data.SelectMany(media => inclusions[media.Id]).Select(media => media.Attributes).OfType<PostPageIncludedMedia>();

				if (media.Any()) {
					LogInfo($"Downloading media of post {postI}: {post.Attributes.Title} ({post.Attributes.PublishedAt.ToShortDateString()})");

					directory = Directory.CreateDirectory(Path.Combine(DataFolder, post.Attributes.PublishedAt.ToString("yyyy-MM-dd") + " " + Util.SanitizeFilename(post.Attributes.Title)));

					foreach (PostPageIncludedMedia item in media) {
						var response = client.GetAsync(item.ImageUrls.Original).Result;
						using FileStream fileStream = File.Create(Path.Combine(directory.FullName, Util.SanitizeFilename(item.Filename ?? Path.GetFileName(new Uri(item.DownloadUrl).GetLeftPart(UriPartial.Path)))));
						using Stream downloadStream = response.Content.ReadAsStreamAsync().Result;
						downloadStream.CopyTo(fileStream);
					}
				}
				#endregion

				#region Link scraping & downloading

				if (post.Attributes.Content == null) {
					LogInfo($"Post {postI}: {post.Attributes.Title} ({post.Attributes.PublishedAt.ToShortDateString()}) does not have any content. No links can be extracted from it.");
					LogInfo($"Here's the link if you want to have a look {post.Attributes.PostUrl}");
					LogInfo("Keep in mind that this link only works if you have access to it. As such, if this turns out to be incorrect, the developer will not be able to fix this.");
				} else {
					HtmlDocument contentHtml = new HtmlDocument();
					contentHtml.LoadHtml(post.Attributes.Content);

					int linkI = 1;
					string title = post.Attributes.Title;
					foreach (var (url, downloader) in
						from node in contentHtml.DocumentNode.Descendants()
						where node.Name == "a"
						let href = node.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value
						where href != null
						let downloader = downloaders.FirstOrDefault(dl => dl.CanDownloadLink(href))
						where downloader != null
						select (href, downloader)
					) {
						LogInfo($"Downloading link {linkI++} ({downloader.Name}) extracted from post {postI}: {post.Attributes.Title} ({post.Attributes.PublishedAt.ToShortDateString()})");
						directory ??= Directory.CreateDirectory(Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
							"Posts",
							post.Attributes.PublishedAt.ToString("yyyy-MM-dd") + " " + Util.SanitizeFilename(post.Attributes.Title)
						));
						try {
							downloader.DownloadFiles(client, cookies, url, directory.FullName);
						} catch (LinkDownloaderException e) {
							LogError(e.Message);
						}
					}
				}
				#endregion

				postI++;
			}

			foreach (IDisposable downloader in downloaders.OfType<IDisposable>()) {
				downloader.Dispose();
			}

			LogInfo("Done.");
		}

		private static void DownloadAllPosts(HttpClient client, List<PostPage> pages, string backupFile) {
			string nextUrl;
			if (pages == null) {
				Console.Write("Paste URL of posts call: ");
				nextUrl = Console.ReadLine();
				pages = new List<PostPage>();
			} else {
				nextUrl = pages[^1].Links.Next;
			}

			int pageNumber = pages.Count + 1;

			do {
				LogInfo($"Retrieving page {pageNumber++}.");

				string result = client.GetStringAsync(nextUrl).Result;
				PostPage page = JsonConvert.DeserializeObject<PostPage>(result);

				pages.Add(page);

				File.WriteAllText(backupFile, JsonConvert.SerializeObject(pages));

				nextUrl = page.Links?.Next;

				Thread.Sleep(TimeSpan.FromSeconds(5));
			} while (nextUrl != null);

			LogInfo("Done, all post data has been saved locally. Run the program again to download the media.");
		}

		public enum LogLevel {
			Debug, Info, Notice, Warning, Error
		}

		public static void LogDebug  (string message) => Log(LogLevel.Debug,   message);
		public static void LogInfo   (string message) => Log(LogLevel.Info,    message);
		public static void LogNotice (string message) => Log(LogLevel.Notice,  message);
		public static void LogWarning(string message) => Log(LogLevel.Warning, message);
		public static void LogError  (string message) => Log(LogLevel.Error,   message);

		public static void Log(LogLevel level, string message) {
			const int LongestLevel = 7; // Warning
			string logString = $"{DateTime.Now:u} {level,LongestLevel}: {message}";
			Console.WriteLine(logString);
			File.AppendAllText(Path.Combine(DataFolder, "output.log"), logString + Environment.NewLine);
		}
	}
}
