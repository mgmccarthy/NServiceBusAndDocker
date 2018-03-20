using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBusAndDocker.Messages.Commands;
using NServiceBusAndDocker.Messages.Events;

namespace NServiceBusAndDocker.Endpoint
{
    public class TestCommandHandler : IHandleMessages<TestCommand>
    {
        static ILog log = LogManager.GetLogger<TestCommandHandler>();

        public async Task Handle(TestCommand message, IMessageHandlerContext context)
        {
            Debug.WriteLine("handled TestCommand");
            log.Info($"handled TestCommand at {DateTime.UtcNow} (UTC)");

            await context.Publish(new TestEvent { TestProperty = "TestEvent" });
        }
    }
}
