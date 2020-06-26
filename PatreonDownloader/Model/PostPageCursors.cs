using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageCursors {
		[JsonProperty("next")]
		public string Next { get; set; }
	}
}
