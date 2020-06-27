using System;
using System.Collections.Generic;
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
		private static string s_DataFolder;

		private static void Main(string[] args) {
			string sessionToken;

			CookieExtractor[] cookieExtractors = new CookieExtractor[] {
				new ChromeCookieExtractor(),
				new FirefoxCookieExtractor()
			};

			string[] options = new string[cookieExtractors.Length + 1];
			options[0] = "Manually enter/paste session token";
			for (int i = 0; i < cookieExtractors.Length; i++) {
				options[i + 1] = $"Acquire session token from {cookieExtractors[i].Name}";
			}
			int choice = Util.ConsoleChoiceMenu("Enter an option:", options);
			
			if (choice == 0) {
				Console.Write("Enter/Paste session token: ");
				sessionToken = Console.ReadLine();
			} else {
				sessionToken = cookieExtractors[choice - 1].GetPatreonSessionToken();
			}

			s_DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PatreonDownloader");

			string backupFile = Path.Combine(s_DataFolder, "posts.json"); // Unfortunately, no SpecialFolder.Downloads.

			var cookieContainer = new CookieContainer();
			cookieContainer.Add(new Uri("https://www.patreon.com"), new Cookie("session_id", sessionToken));
			using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			using var client = new HttpClient(handler);
			client.DefaultRequestHeaders.Add("User-Agent", "PatreonDownloader 0.3");

			Directory.CreateDirectory(s_DataFolder);

			if (File.Exists(backupFile)) {
				List<PostPage> list = JsonConvert.DeserializeObject<List<PostPage>>(File.ReadAllText(backupFile));

				string question = "A backup file exists.";

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

				choice = Util.ConsoleChoiceMenu(question, options);

				switch (choice) {
					case 0:
						Dictionary<string, List<PostPageIncluded>> ppis = new Dictionary<string, List<PostPageIncluded>>();
						foreach (PostPageIncluded item in list.SelectMany(page => page.Included)) {
							if (ppis.TryGetValue(item.Id, out List<PostPageIncluded> ppiList)) {
								ppiList.Add(item);
							} else {
								ppis[item.Id] = new List<PostPageIncluded>() { item };
							}
						}

						DownloadMedia(client, cookieContainer, list.SelectMany(page => page.Data), ppis);
						break;
					case 1:
						File.Delete(backupFile);
						DownloadAllPosts(client, 0, null, backupFile);
						break;
					case 2:
						DownloadAllPosts(client, list.Count, list[^1].Links.Next, backupFile);
						break;
				}
			} else {
				Console.WriteLine("No local backup file exists. Downloading all post data to a local json file.");
				DownloadAllPosts(client, 1, null, backupFile);
			}
		}

		private static void DownloadMedia(HttpClient client, CookieContainer cookies, IEnumerable<PostPageData> posts, IDictionary<string, List<PostPageIncluded>> inclusions) {
			LinkDownloader[] downloaders = new[] {
				new DropboxDownloader()
				// Add more downloaders here
			};

			int postI = 1;
			foreach (PostPageData post in posts.Where(post => post.Attributes.PostType == "image_file")) {
				DirectoryInfo directory = null;

				#region Media downloading
				IEnumerable<PostPageIncludedMedia> media = post.Relationships.Media.Data.SelectMany(media => inclusions[media.Id]).Select(media => media.Attributes).OfType<PostPageIncludedMedia>();

				if (media.Any()) {
					Console.WriteLine($"Downloading media of post {postI}: {post.Attributes.Title} ({post.Attributes.PublishedAt.ToShortDateString()})");

					directory = Directory.CreateDirectory(Path.Combine(s_DataFolder, post.Attributes.PublishedAt.ToString("yyyy-MM-dd") + " " + Util.SanitizeFilename(post.Attributes.Title)));

					foreach (PostPageIncludedMedia item in media) {
						var response = client.GetAsync(item.ImageUrls.Original).Result;
						// Using content disposition filename to work around some stupid bug that causes item.Filename to be null, and I can't figure out why.
						using FileStream fileStream = File.Create(Path.Combine(directory.FullName, Util.SanitizeFilename(response.Content.Headers.ContentDisposition.FileName[1..^1])));
						using Stream downloadStream = response.Content.ReadAsStreamAsync().Result;
						downloadStream.CopyTo(fileStream);
					}
				}
				#endregion

				#region Link scraping & downloading
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
					Console.WriteLine($"Downloading link {linkI++} ({downloader.Name}) extracted from post {postI}: {post.Attributes.Title} ({post.Attributes.PublishedAt.ToShortDateString()})");
					directory ??= Directory.CreateDirectory(Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
						"Posts",
						post.Attributes.PublishedAt.ToString("yyyy-MM-dd") + " " + Util.SanitizeFilename(post.Attributes.Title)
					));
					downloader.DownloadFiles(client, cookies, url, directory.FullName);
				}
				#endregion

				postI++;
			}

			foreach (IDisposable downloader in downloaders.OfType<IDisposable>()) {
				downloader.Dispose();
			}

			Console.WriteLine("Done.");
			Console.ReadKey();
		}

		private static string DownloadAllPosts(HttpClient client, int initialCount, string nextUrl, string backupFile) {
			if (nextUrl == null) {
				Console.Write("Paste URL of posts call: ");
				nextUrl = Console.ReadLine();
			}

			List<PostPage> pages = new List<PostPage>();

			int pageNumber = initialCount;
			do {
				Console.WriteLine($"Retrieving page {pageNumber++}.");

				string result = client.GetStringAsync(nextUrl).Result;
				PostPage page = JsonConvert.DeserializeObject<PostPage>(result);
				pages.Add(page);

				File.WriteAllText(backupFile, JsonConvert.SerializeObject(pages));

				nextUrl = page.Links?.Next;

				Thread.Sleep(TimeSpan.FromSeconds(10));
			} while (nextUrl != null);

			Console.WriteLine("Done, all post data has been saved locally. Run the program again to download the media.");
			Console.ReadKey();
			return nextUrl;
		}
	}
}
