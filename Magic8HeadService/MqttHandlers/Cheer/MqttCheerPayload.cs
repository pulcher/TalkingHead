using System.Text.Json.Serialization;

namespace Magic8HeadService.MqttHandlers.Cheer
{
    public class MqttCheerPayload
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("bits")]
        public int Bits { get; set; }
        //[JsonPropertyName("isSub")]
        //public string IsSub { get; set; }
        //[JsonPropertyName("isMod")]
        //public string IsMod { get; set; }
        //[JsonPropertyName("isVip")]
        //public string IsVip { get; set; }
    }
}