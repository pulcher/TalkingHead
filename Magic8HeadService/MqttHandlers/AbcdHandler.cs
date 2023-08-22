using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService.MqttHandlers
{
    public class AbcdHandler : IMqttHandler
    {
        private ITwitchClient client;
        private ILogger<Worker> logger;

        public AbcdHandler(ITwitchClient client, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public bool CanHandle(MqttHandlerMessage message)
        {
            if(message == null) return false;

            if(message.Topic.Contains("mbh/redeem"))
                return true;
            else 
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            // Encoding.ASCII.GetString(message.Payload)
            client.SendMessage(client.JoinedChannels.FirstOrDefault(), "!mbh say easy as 1, 2, 3" );
        }
    }
}
