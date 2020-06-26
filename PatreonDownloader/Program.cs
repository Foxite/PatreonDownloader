using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace PatreonDownloader {
	public class Program {
		private static void Main(string[] args) {
			Console.Write("Paste URL of posts call: ");
			string nextUrl = Console.ReadLine();

			Console.Write("Paste session token: ");
			string sessionToken = Console.ReadLine();
			string backupFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "posts.json"); // Unfortunately, no SpecialFolder.Downloads.

			var cookieContainer = new CookieContainer();
			cookieContainer.Add(new Uri("https://www.patreon.com"), new Cookie("session_id", sessionToken));
			using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			using var client = new HttpClient(handler);
			client.DefaultRequestHeaders.Add("User-Agent", "PatreonDownloader 0.1");

			if (File.Exists(backupFile)) {
				List<PostPage> list = JsonConvert.DeserializeObject<List<PostPage>>(File.ReadAllText(backupFile));

				Console.WriteLine("A backup file exists. Do you want to:");
				Console.WriteLine("[1] Download all images in the backup");
				Console.WriteLine("[2] Delete the backup file and re-download all post data");

				if (list[^1].Links != null) {
					Console.WriteLine("[3] Continue downloading post data where left off");
				} else {
					Console.WriteLine("The local backup indicates that there are no more pages to download.");
				}
				Console.WriteLine("Re-downloading may be necessary if the backup is too old. Media URLs expire after a certain amount of time.");

				int choice;
				while (!(int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= (list[^1].Links != null ? 3 : 2))) {
					Console.WriteLine("Enter the number of the option you want.");
				}

				switch (choice) {
					case 1:
						DownloadMedia(client, list.SelectMany(page => page.Data), list.SelectMany(page => page.Included).ToDictionary(ppi => ppi.Id));
						break;
					case 2:
						File.Delete(backupFile);
						DownloadAllPosts(client, 0, nextUrl, backupFile);
						break;
					case 3:
						DownloadAllPosts(client, list.Count, list[^1].Links.Next, backupFile);
						break;
				}
			} else {
				Console.WriteLine("No local backup file exists. Downloading all post data to a local json file.");
				DownloadAllPosts(client, 1, nextUrl, backupFile);
			}
		}

		private static void DownloadMedia(HttpClient client, IEnumerable<PostPageData> posts, IDictionary<int, PostPageIncluded> inclusions) {
			foreach (PostPageData post in posts.Where(post => post.Attributes.PostType == "image_file")) {
				// TODO extract links from post.Attributes.Content
				IEnumerable <PostPageIncludedMedia> media = post.Relationships.Media.Data.Select(media => inclusions[media.Id].Attributes as PostPageIncludedMedia);

				if (media.Any()) {
					DirectoryInfo directory = Directory.CreateDirectory(Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
						"Posts",
						post.Attributes.PublishedAt.ToString("yyyy-MM-dd") + " " + post.Attributes.Title
					));
					foreach (PostPageIncludedMedia item in media) {
						using FileStream fileStream = File.Create(Path.Combine(directory.FullName, item.Filename));
						using Stream downloadStream = client.GetStreamAsync(item.ImageUrls.Original).Result;
						downloadStream.CopyTo(fileStream);
					}
				}
			}
		}

		private static string DownloadAllPosts(HttpClient client, int initialCount, string nextUrl, string backupFile) {
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

			Console.WriteLine("Done, all post data has been saved locally.");
			Console.ReadKey();
			return nextUrl;
		}
	}
}
