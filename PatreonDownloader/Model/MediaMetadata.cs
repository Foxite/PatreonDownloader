using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class MediaMetadata {
		[JsonProperty("dimensions")]
		public Dimensions Dimensions { get; set; }
	}
}
