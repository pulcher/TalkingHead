using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class HydrateHandler : IMqttHandler
    {
        private ITwitchClient client;
        private ILogger<Worker> logger;

        public HydrateHandler(ITwitchClient client, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public bool CanHandle(MqttHandlerMessage message)
        {
            if (message == null) return false;

            var payloadString = Encoding.ASCII.GetString(message.Payload);

            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            if (redeem != null && redeem.RewardName == "Hydrate!") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            // Encoding.ASCII.GetString(message.Payload)
            client.SendMessage(client.JoinedChannels.FirstOrDefault(), "!mbh say Yo time for some hydration!");
        }
    }
}
