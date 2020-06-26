using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageIncluded {
		[JsonProperty("attributes")]
		public PostPageIncludedAttributes Attributes { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("relationships")]
		public PostPageIncludedRelationships Relationships { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}
