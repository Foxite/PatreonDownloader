using System.Net;
using System.Net.Http;

namespace PatreonDownloader.LinkScraping {
	public abstract class LinkDownloader {
		public abstract string Name { get; }

		/// <summary>
		/// Return a value indicating if this LinkDownloader can download a file based on the given <paramref name="url"/>.
		/// </summary>
		public abstract bool CanDownloadLink(string url);

		/// <summary>
		/// Download the file located at <paramref name="url"/> to the folder located at <paramref name="folderPath"/> using <paramref name="client"/>.
		/// 
		/// If this function is called, then <see cref="CanDownloadLink(string)"/> has returned true for <paramref name="url"/>.
		/// </summary>
		public abstract void DownloadFiles(HttpClient client, CookieContainer cookies, string url, string folderPath);
	}
}
