using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public abstract class PostPageIncludedAttributes { }

	[Serializable]
	public class PostPageIncludedUser : PostPageIncludedAttributes {
		[JsonProperty("full_name")]
		public string FullName { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	[Serializable]
	public class PostPageIncludedCampaign : PostPageIncludedAttributes {
		[JsonProperty("avatar_photo_url")]
		public string AvatarUrl { get; set; }

		[JsonProperty("currency")]
		public Currency Currency { get; set; }

		[JsonProperty("earnings_visiblity")]
		public string EarningsVisibility { get; set; }

		[JsonProperty("is_monthly")]
		public bool IsMonthly { get; set; }

		[JsonProperty("is_nsfw")]
		public bool IsNsfw { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("show_audio_post_download_links")]
		public bool ShowAudioPostDownloadLinks { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	[Serializable]
	public class PostPageIncludedPoll : PostPageIncludedAttributes {
		[JsonProperty("closes_at")]
		public DateTime ClosesAt { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("num_responses")]
		public int ResponseCount { get; }

		[JsonProperty("question_type")]
		public string QuestionText { get; set; }

		[JsonProperty("created_type")]
		public string QuestionType { get; set; } // TODO make enum
	}

	[Serializable]
	public class PostPageIncludedMedia : PostPageIncludedAttributes {
		[JsonProperty("download_url")]
		public string DownloadUrl { get; }

		[JsonProperty("file_name")]
		public string Filename { get; }

		[JsonProperty("image_urls")]
		public ImageUrls ImageUrls { get; set; }

		[JsonProperty("metadata")]
		public MediaMetadata Metadata { get; set; }
	}

	[Serializable]
	public class PostPageIncludedAccessRule : PostPageIncludedAttributes {
		[JsonProperty("access_rule_type")]
		public string AccessRuleType { get; set; } // TODO make enum

		/// <summary>
		/// Unknown.
		/// </summary>
		[JsonProperty("amount_cents")]
		public object AmountCents { get; set; }

		[JsonProperty("currency")]
		public Currency Currency { get; set; }

		[JsonProperty("post_count")]
		public int PostCount { get; set; }
	}

	[Serializable]
	public class PostPageIncludedReward : PostPageIncludedAttributes {
		[JsonProperty("amount")]
		public int Amount { get; }

		[JsonProperty("amount_cents")]
		public int AmountCents { get; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
		
		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("patron_currency")]
		public Currency PatronCurrency { get; set; }

		[JsonProperty("remaining")]
		public int? Remaining { get; set; }

		[JsonProperty("requires_shipping")]
		public bool RequiresShipping { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("user_limit")]
		public int? UserLimit { get; set; }

		[JsonProperty("discord_role_ids")]
		public string[] DiscordRoleIds { get; set; }

		[JsonProperty("edited_at")]
		public DateTime EditedAt { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("post_count")]
		public int? PostCount { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("published")]
		public bool? Published { get; set; }

		[JsonProperty("published_at")]
		public DateTime? PublishedAt { get; set; }

		[JsonProperty("welcome_message")]
		public string WelcomeMessage { get; set; }

		[JsonProperty("welcome_message_unsafe")]
		public string WelcomeMessageUnsafe { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		[JsonProperty("welcome_video_embed")]
		public object WelcomeVideoEmbed { get; set; }

		[JsonProperty("welcome_video_url")]
		public string WelcomeVideoUrl { get; set; }
	}

	[Serializable]
	public class PostPageIncludedGoal : PostPageIncludedAttributes {
		[JsonProperty("completed_percentage")]
		public int CompletedPercentage { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("currency")]
		public Currency Currency { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("reached_at")]
		public DateTime ReachedAt { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }
	}

	[Serializable]
	public class PostPageIncludedPollChoice : PostPageIncludedAttributes {
		[JsonProperty("choice_type")]
		public string ChoiceType { get; set; } // TODO make enum

		[JsonProperty("num_responses")]
		public int ResponseCount { get; set; }
		
		[JsonProperty("position")]
		public int Position { get; set; }

		[JsonProperty("text_content")]
		public string TextContent { get; set; }
	}

	[Serializable]
	public class PostPageIncludedAttachment : PostPageIncludedAttributes {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
