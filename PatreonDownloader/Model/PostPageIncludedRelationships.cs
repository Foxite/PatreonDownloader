using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageIncludedRelationships {
		[JsonProperty("campaign")]
		public DataAndLinks Campaign { get; set; }
	}
}
