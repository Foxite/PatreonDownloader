﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace PatreonDownloader.LinkScraping {
	public class DropboxDownloader : LinkDownloader {
		private readonly Regex m_NonceRegex;
		private string m_Nonce;

		public override string Name => "Dropbox";

		public DropboxDownloader() {
			m_NonceRegex = new Regex(@"{""CSP_SCRIPT_NONCE"": ""([A-z0-9\/\+]+)""}", RegexOptions.Compiled);
		}

		public override bool CanDownloadLink(string url) {
			string authority = new Uri(url).Authority;
			return authority == "www.dropbox.com" || authority == "dropbox.com";
		}

		public override void DownloadFiles(HttpClient client, CookieContainer cookies, string url, string folderPath) {
			if (m_Nonce == null) {
				HttpResponseMessage dropboxPage = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
				string nonceCookie = dropboxPage.Headers.First(cookie => cookie.Key.ToLower() == "set-cookie").Value.First(value => value.StartsWith("t="));
				m_Nonce = nonceCookie[2..nonceCookie.IndexOf(';')];
			}
			
			string downloadUrl = client.PostAsync("https://www.dropbox.com/sharing/fetch_user_content_link", new FormUrlEncodedContent(new Dictionary<string, string>() {
				{ "is_xhr", "true" },
				{ "t", m_Nonce },
				{ "url", url }
			})).Result.Content.ReadAsStringAsync().Result;

			HttpResponseMessage downloadInfo = client.GetAsync(downloadUrl).Result;
			using FileStream fileStream = File.Create(Path.Combine(folderPath, Util.SanitizeFilename(downloadInfo.Content.Headers.ContentDisposition.FileName[1..^1])));
			using Stream downloadStream = downloadInfo.Content.ReadAsStreamAsync().Result;
			downloadStream.CopyTo(fileStream);
		}
	}
}