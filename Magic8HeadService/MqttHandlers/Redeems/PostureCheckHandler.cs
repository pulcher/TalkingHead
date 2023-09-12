using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class PostureCheckHandler : IMqttHandler
    {
        private ITwitchClient client;
        private readonly ISayingResponse sayingResponse;
        private ILogger<Worker> logger;

        public PostureCheckHandler(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
        {
            this.client = client;
            this.sayingResponse = sayingResponse;
            this.logger = logger;
        }

        public bool CanHandle(MqttHandlerMessage message)
        {
            if (message == null) return false;

            var payloadString = Encoding.ASCII.GetString(message.Payload);

            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            if (redeem != null && redeem.RewardName == "Posture Check!") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            var payloadString = Encoding.ASCII.GetString(message.Payload);
            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            var messageToSay = $"Reticulated Spine detected by {redeem.UserName}! Please sit up straight!";

            sayingResponse.SaySomethingNiceAsync(messageToSay, client, 
                client.JoinedChannels.FirstOrDefault().ToString(), string.Empty).Wait();
        }
    }
}
