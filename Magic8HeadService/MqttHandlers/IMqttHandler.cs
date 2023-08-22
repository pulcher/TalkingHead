using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic8HeadService.MqttHandlers
{
    public interface IMqttHandler
    {
        bool CanHandle(MqttHandlerMessage message);

        void Handle(MqttHandlerMessage message);
    }
}
