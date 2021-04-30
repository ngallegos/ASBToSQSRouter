using System.Threading.Tasks;
using ASBToSQSRouter.Messages;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace ASBToSQSRouter.SQSEndpoint
{
    public class ASBToSQSCommandHandler : IHandleMessages<ASBToSQSCommand>
    {
        private readonly ILogger<ASBToSQSCommandHandler> _logger;

        public ASBToSQSCommandHandler(ILogger<ASBToSQSCommandHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task Handle(ASBToSQSCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation("SQS Endpoint handled ASBToSQSCommand");
            await context.Publish<ASBToSQSEvent>();
        }
    }
}