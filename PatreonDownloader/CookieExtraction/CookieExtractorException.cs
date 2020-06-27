using System;
using System.Runtime.Serialization;

namespace PatreonDownloader.CookieExtraction {
	public class CookieExtractorException : Exception {
		public CookieExtractorException() { }
		public CookieExtractorException(string message) : base(message) { }
		public CookieExtractorException(string message, Exception innerException) : base(message, innerException) { }
		protected CookieExtractorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
