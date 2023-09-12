using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class StreeeeeeeeeetchHandler : IMqttHandler
    {
        private ITwitchClient client;
        private readonly ISayingResponse sayingResponse;
        private ILogger<Worker> logger;

        public StreeeeeeeeeetchHandler(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
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

            if (redeem != null && redeem.RewardName == "Streeeeeeeeeetch") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            var payloadString = Encoding.ASCII.GetString(message.Payload);
            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            var messageToSay = $"Initiate stretch subroutine to optimize human performance!";

            sayingResponse.SaySomethingNiceAsync(messageToSay, client, 
                client.JoinedChannels.FirstOrDefault().ToString(), string.Empty).Wait();
        }
    }
}
