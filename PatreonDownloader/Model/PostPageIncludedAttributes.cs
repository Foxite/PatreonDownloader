using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageIncludedAttributes {
		[JsonProperty("full_name")]
		public string FullName { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
