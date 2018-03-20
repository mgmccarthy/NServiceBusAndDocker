using NServiceBus;

namespace NServiceBusAndDocker.Messages.Commands
{
    public class TestCommand : ICommand
    {
        public string TestProperty { get; set; }
    }
}
