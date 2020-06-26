using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class ImageUrls {
		[JsonProperty("default")]
		public string Default { get; set; }

		[JsonProperty("original")]
		public string Original { get; set; }

		[JsonProperty("thumbnail")]
		public string Thumbnail { get; set; }
	}
}