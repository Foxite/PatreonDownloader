using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageDataAttributesImage {
		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("large_url")]
		public string LargeUrl { get; set; }

		[JsonProperty("thumb_url")]
		public string ThumbUrl { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
