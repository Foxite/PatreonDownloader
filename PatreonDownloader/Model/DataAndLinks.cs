using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class DataAndLinks {
		[JsonProperty("data")]
		public Data Data { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }
	}
}
