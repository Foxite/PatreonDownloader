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
						DownloadMedia(client, JsonConvert.DeserializeObject<List<PostPage>>(File.ReadAllText(backupFile)).SelectMany(page => page.Data));
						break;
					case 2:
						DownloadAllPosts(client, 0, nextUrl, sessionToken, backupFile);
						break;
					case 3:
						DownloadAllPosts(client, list.Count, list[^1].Links.Next, sessionToken, backupFile);
						break;
				}
			} else {
				Console.WriteLine("No local backup file exists. Downloading all post data to a local json file.");
				DownloadAllPosts(client, 1, nextUrl, sessionToken, backupFile);
			}
		}

		private static void DownloadMedia(HttpClient client, IEnumerable<PostPageData> lists) {

		}

		private static string DownloadAllPosts(HttpClient client, int initialCount, string nextUrl, string sessionToken, string backupFile) {
			List<PostPage> pages = new List<PostPage>();

			int pageNumber = initialCount;
			do {
				Console.WriteLine($"Retrieving page {pageNumber++}.");

				PostPage page = JsonConvert.DeserializeObject<PostPage>(client.GetStringAsync(nextUrl).Result);
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
