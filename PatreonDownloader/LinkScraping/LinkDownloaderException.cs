using System;
using System.Runtime.Serialization;

namespace PatreonDownloader.LinkScraping {
	public class LinkDownloaderException : Exception {
		public LinkDownloaderException() { }
		public LinkDownloaderException(string message) : base(message) { }
		public LinkDownloaderException(string message, Exception innerException) : base(message, innerException) { }
		protected LinkDownloaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
