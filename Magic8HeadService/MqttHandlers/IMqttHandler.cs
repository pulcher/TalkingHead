namespace Magic8HeadService.MqttHandlers
{
    public interface IMqttHandler
    {
        bool CanHandle(MqttHandlerMessage message);

        void Handle(MqttHandlerMessage message);
    }
}
