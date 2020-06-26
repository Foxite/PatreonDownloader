using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class DataArray {
		[JsonProperty("data")]
		public Data[] Data { get; set; }
	}
}
