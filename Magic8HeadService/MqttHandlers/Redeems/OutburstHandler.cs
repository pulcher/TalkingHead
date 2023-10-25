using Microsoft.Extensions.Logging;
using System.Linq;
using TwitchLib.Client.Interfaces;
using System.Text.Json;
using System.Text;
using System;
using System.Timers;

namespace Magic8HeadService.MqttHandlers.Redeems
{
    public class OutburstHandler : IMqttHandler
    {
        private ITwitchClient client;
        private readonly ISayingResponse sayingResponse;
        private ILogger<Worker> logger;
        private bool outburstEnabled = false;
        private string sendToChannel;
        private Random random = new();
        private Timer outBurstTime;
        private Timer outburstNextSaying;

        public OutburstHandler(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
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

            if (redeem != null && redeem.RewardName == "Outburst") 
                return true;
            else
                return false;
        }

        public void Handle(MqttHandlerMessage message)
        {
            var payloadString = Encoding.ASCII.GetString(message.Payload);
            var redeem = JsonSerializer.Deserialize<MqttRedeemPayload>(payloadString);

            var outburstLength = random.Next(10);
            var messageToChannel = $"Ahh thanks {redeem.UserName}! now I can speak freely for {outburstLength} minutes!";

            sendToChannel = client.JoinedChannels.FirstOrDefault().Channel;
            this.client.SendMessage(sendToChannel, messageToChannel);

            EnableOutburstMode(outburstLength);
        }

        public void EnableOutburstMode(int outburstLength)
        {
            // pick a random number of seconds until the next outburst
            // set a timer to do fire at those seconds
            // do outburst
            outburstEnabled = true;

            // Create a timer with a two second interval.
            outBurstTime = new Timer(outburstLength * 1000 * 60);
            outBurstTime.Elapsed += EndOutburstEvent;
            outBurstTime.Enabled = true;

            Outburst();

            SayingTimer();
        }

        private void SayingTimer()
        {
            var seconds = random.Next(30, 120);
            logger.LogInformation($"Outburst: next message in {seconds} seconds....");

            if(outburstNextSaying != null)
            {
                outburstNextSaying.Stop();
            }

            outburstNextSaying = new Timer(TimeSpan.FromSeconds(seconds));
            outburstNextSaying.Elapsed += NextSayingEvent;
            outburstNextSaying.Enabled = true;
        }

        private void NextSayingEvent(object sender, ElapsedEventArgs e)
        {
            if (outburstEnabled)
            {
                Outburst();

                SayingTimer();
            }
        }

        private void EndOutburstEvent(object sender, ElapsedEventArgs e)
        {
            outburstEnabled = false;
            outburstNextSaying.Stop();

            this.client.SendMessage(sendToChannel, "Outburst mode terminated....");
        }

        private void Outburst()
        {
            var saying = sayingResponse.PickSaying();

            this.client.SendMessage(sendToChannel, saying);
        }
    }
}
