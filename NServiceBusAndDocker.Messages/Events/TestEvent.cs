using NServiceBus;

namespace NServiceBusAndDocker.Messages.Events
{
    public class TestEvent : IEvent
    {
        public string TestProperty { get; set; }
    }
}
