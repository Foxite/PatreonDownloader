using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class Links {
		[JsonProperty("related")]
		public string Related { get; set; }
	}
}