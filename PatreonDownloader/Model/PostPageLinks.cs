using Newtonsoft.Json;
using System;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageLinks {
		[JsonProperty("next")]
		public string Next { get; set; }
	}
}
