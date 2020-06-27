using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PatreonDownloader.LinkScraping {
	public class DropboxDownloader : LinkDownloader {
		private string m_Nonce;

		public override string Name => "Dropbox";

		public override bool CanDownloadLink(string url) {
			string authority = new Uri(url).Authority;
			return authority == "www.dropbox.com" || authority == "dropbox.com";
		}

		public override void DownloadFiles(HttpClient client, CookieContainer cookies, string url, string folderPath) {
			if (m_Nonce == null) {
				using HttpResponseMessage dropboxPage = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
				string nonceCookie = dropboxPage.Headers.First(cookie => cookie.Key.ToLower() == "set-cookie").Value.First(value => value.StartsWith("t="));
				m_Nonce = nonceCookie[2..nonceCookie.IndexOf(';')];
			}

			using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
				{ "is_xhr", "true" },
				{ "t", m_Nonce },
				{ "url", url }
			});
			using HttpResponseMessage downloadUrlFetchResult = client.PostAsync("https://www.dropbox.com/sharing/fetch_user_content_link", formContent).Result;

			if (downloadUrlFetchResult.IsSuccessStatusCode) {
				string downloadUrl = downloadUrlFetchResult.Content.ReadAsStringAsync().Result;

				using HttpResponseMessage downloadInfo = client.GetAsync(downloadUrl).Result;
				using FileStream fileStream = File.Create(Path.Combine(folderPath, Util.SanitizeFilename(downloadInfo.Content.Headers.ContentDisposition.FileName[1..^1])));
				using Stream downloadStream = downloadInfo.Content.ReadAsStreamAsync().Result;
				downloadStream.CopyTo(fileStream);
			} else {
				throw new LinkDownloaderException($"Dropbox URL returned a HTTP {downloadUrlFetchResult.StatusCode}: {url}");
			}
		}
	}
}
