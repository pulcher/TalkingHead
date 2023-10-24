using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class OutburstHandler : IMqttHandler
    {
        private ITwitchClient client;
        private ILogger<Worker> logger;

        public OutburstHandler(ITwitchClient client, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public bool CanHandle(MqttHandlerMessage message)
        {
            if (message == null) return false;

            var payloadString = Encoding.ASCII.GetString(message.Payload);

            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            if (redeem != null && redeem.RewardName == "Outburst") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            var payloadString = Encoding.ASCII.GetString(message.Payload);
            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);
            var messageToChannel = $"Ahh thanks {redeem.UserName} now I can speak freely!";

            var sendToChannel = client.JoinedChannels.FirstOrDefault().Channel;

            this.client.SendMessage(sendToChannel, messageToChannel);
        }
    }
}
