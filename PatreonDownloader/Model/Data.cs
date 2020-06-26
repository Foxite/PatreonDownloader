using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class Data {
		[JsonProperty("data")]
		public int Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; } // TODO make enum
	}
}
