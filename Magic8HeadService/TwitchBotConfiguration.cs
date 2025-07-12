namespace Magic8HeadService
{
    public class TwitchBotConfiguration
    {
        public string UserName { get; set; }
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string SpeechSubscription { get; set; }
        public string SpeechServiceRegion { get; set; }
        public string ChannelName { get; set; }
        public string MqttUsername { get; set; }
        public string MqttPassword { get; set; }
        public string MqttBroker { get; set; }
    }
}
