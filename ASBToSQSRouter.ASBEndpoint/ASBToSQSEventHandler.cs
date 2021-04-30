using System.Threading.Tasks;
using ASBToSQSRouter.Messages;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace ASBToSQSRouter.ASBEndpoint
{
    public class ASBToSQSEventHandler : IHandleMessages<ASBToSQSEvent>
    {
        private readonly ILogger<ASBToSQSEventHandler> _logger;

        public ASBToSQSEventHandler(ILogger<ASBToSQSEventHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task Handle(ASBToSQSEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation("ASB Endpoint handled ASBToSQSEvent");
        }
    }
}