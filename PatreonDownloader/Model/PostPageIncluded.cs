﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageIncluded {
		[JsonProperty("attributes")]
		public JObject RawAttributes { get; set; }

		public PostPageIncludedAttributes Attributes => Type switch {
			IncludedType.AccessRule => RawAttributes.ToObject<PostPageIncludedAccessRule>(),
			IncludedType.Media      => RawAttributes.ToObject<PostPageIncludedMedia>(),
			IncludedType.Poll       => RawAttributes.ToObject<PostPageIncludedPoll>(),
			IncludedType.Campaign   => RawAttributes.ToObject<PostPageIncludedCampaign>(),
			IncludedType.User       => RawAttributes.ToObject<PostPageIncludedUser>(),
			IncludedType.Reward     => RawAttributes.ToObject<PostPageIncludedReward>(),
			IncludedType.PollChoice => RawAttributes.ToObject<PostPageIncludedPollChoice>(),
			IncludedType.Goal       => RawAttributes.ToObject<PostPageIncludedGoal>(),
			_ => throw new InvalidOperationException("This should not happen. PostPageIncluded.Type was " + Type.ToString())
		};

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("relationships")]
		public PostPageIncludedRelationships Relationships { get; set; }

		[JsonProperty("type")]
		public IncludedType Type { get; set; }

		public enum IncludedType {
			[JsonProperty("access-rule")]
			AccessRule,

			[JsonProperty("media")]
			Media,

			[JsonProperty("media")]
			Poll,

			[JsonProperty("campaign")]
			Campaign,

			[JsonProperty("user")]
			User,

			[JsonProperty("reward")]
			Reward,

			[JsonProperty("poll-choice")]
			PollChoice,

			[JsonProperty("goal")]
			Goal,
		}
	}
}
