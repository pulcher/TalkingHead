using System;
using System.Collections.Generic;

namespace Magic8HeadService.MqttHandlers
{
    public class MqttHandlerMessage
    {
        public string Topic { get; set; }
        public ArraySegment<byte> Payload { get; set; }
        public bool IsHandled { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();

        public MqttHandlerMessage() { }

        public MqttHandlerMessage(string topic, ArraySegment<byte> payload)
        {
            Topic = topic;
            Payload = payload;
        }
    }
}
