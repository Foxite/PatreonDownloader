using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class MediaMetadata {
		[JsonProperty("w")]
		public int Width { get; set; }

		[JsonProperty("h")]
		public int Height { get; set; }
	}
}
