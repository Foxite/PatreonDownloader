﻿using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageData {
		[JsonProperty("attributes")]
		public PostPageDataAttributes Attributes { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("relationships")]
		public PostPageDataRelationships Relationships { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; } // TODO make enum
	}
}
