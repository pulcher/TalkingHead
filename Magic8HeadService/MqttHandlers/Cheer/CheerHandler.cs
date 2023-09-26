using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;

namespace Magic8HeadService.MqttHandlers.Cheer
{
    public class CheerHandler : IMqttHandler
    {
        private ITwitchClient client;
        private readonly ISayingResponse sayingResponse;
        private ILogger<Worker> logger;

        public CheerHandler(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
        {
            this.client = client;
            this.sayingResponse = sayingResponse;
            this.logger = logger;
        }

        public bool CanHandle(MqttHandlerMessage message)
        {
            if (message == null) return false;

            var payloadString = Encoding.ASCII.GetString(message.Payload);

            var cheer = JsonSerializer.Deserialize<MqttCheerPayload>(payloadString);

            if (cheer != null && cheer.Type == "channel.cheer") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            var payloadString = Encoding.ASCII.GetString(message.Payload);
            var cheer = JsonSerializer.Deserialize<MqttCheerPayload>(payloadString);

            var messageToSay = $"Thanks for the cheer {cheer.UserName}";

            sayingResponse.SaySomethingNiceAsync(messageToSay, client, 
                client.JoinedChannels.FirstOrDefault().ToString(), string.Empty).Wait();
        }
    }
}
