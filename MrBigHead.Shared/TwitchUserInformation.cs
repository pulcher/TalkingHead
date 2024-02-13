using System.Text.Json.Serialization;


namespace MrBigHead.Shared
{
    public class TwitchUserInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("Broadcaster_type")]
        public string Broadcaster_type { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        [JsonPropertyName("offline_image_url")]
        public string OfflineImageUrl { get; set; }
        [JsonPropertyName("view_count")]
        public int ViewCount { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("created_at")]
        public string Created_at { get; set; }
    }
}
