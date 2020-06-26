using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class Data {
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; } // TODO make enum
	}
}
