using System.Text.Json.Serialization;

namespace MrBigHead.Shared
{
    public class TwitchSubscriptionInformation
    {
        [JsonPropertyName("broadcaster_id ")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_login")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("gifter_id")]
        public string GifterId { get; set; }
        [JsonPropertyName("gifter_login")]
        public string GifterLogin { get; set; }
        [JsonPropertyName("gifter_name")]
        public string GifterName { get; set; }
        [JsonPropertyName("is_gift")]
        public bool IsGift { get; set; }
        [JsonPropertyName("tier")]
        public string Tier { get; set; }
    }
}
