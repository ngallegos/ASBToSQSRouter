using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ASBToSQSRouter.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace ASBToSQSRouter.ASBEndpoint
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageSession _messageSession;

        private bool _messageSent = false;
        
        public Worker(ILogger<Worker> logger, IMessageSession messageSession)
        {
            _logger = logger;
            _messageSession = messageSession;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_messageSent)
                {
                    await _messageSession.Send(new ASBToSQSCommand());
                    _messageSent = true;
                }
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}