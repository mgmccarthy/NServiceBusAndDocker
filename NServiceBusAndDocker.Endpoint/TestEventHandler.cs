using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBusAndDocker.Messages.Events;

namespace NServiceBusAndDocker.Endpoint
{
    public class TestEventHandler : IHandleMessages<TestEvent>
    {
        static ILog log = LogManager.GetLogger<TestEventHandler>();

        public Task Handle(TestEvent message, IMessageHandlerContext context)
        {
            Debug.WriteLine("handled TestEvent");
            log.Info($"handled TestEvent at {DateTime.UtcNow} (UTC)");
            return Task.CompletedTask;
        }
    }
}
