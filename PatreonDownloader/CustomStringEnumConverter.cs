using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace PatreonDownloader {
	public class CustomStringEnumConverter : JsonConverter {
		private static readonly Dictionary<Type, Dictionary<string, object>> s_EnumCache = new Dictionary<Type, Dictionary<string, object>>();

		private static Dictionary<string, object> GetCache(Type objectType) {
			if (!s_EnumCache.TryGetValue(objectType, out Dictionary<string, object> cache)) {
				cache = s_EnumCache[objectType] = objectType
					.GetFields(BindingFlags.Public | BindingFlags.Static)
					.ToDictionary(
						field => field.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? field.Name,
						field => field.GetValue(null)
					);
			}

			return cache;
		}

		public override bool CanConvert(Type objectType) => objectType.IsEnum;
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			Dictionary<string, object> dictionaries = GetCache(objectType);
			try {
				return dictionaries[(string) reader.Value];
			} catch (KeyNotFoundException knfe) {
				Console.WriteLine("Warning: found unknown data inside the scraped information.");
				Console.WriteLine(knfe.Message);
				return existingValue;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Dictionary<string, object> dictionaries = GetCache(value.GetType());
			writer.WriteValue(dictionaries.First(kvp => {
				return kvp.Value.Equals(value);
			}).Key);
		}
	}
}
