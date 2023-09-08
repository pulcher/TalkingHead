using System.Text.Json.Serialization;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class MqttRedeemPayload
    {
        [JsonPropertyName("rewardName")]
        public string RewardName { get; set; }
        [JsonPropertyName("rawInputEscaped")]
        public string RawInputEscaped { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("isSub")]
        public string IsSub { get; set; }
        [JsonPropertyName("isMod")]
        public string IsMod { get; set; }
        [JsonPropertyName("isVip")]
        public string IsVip { get; set; }
    }
}