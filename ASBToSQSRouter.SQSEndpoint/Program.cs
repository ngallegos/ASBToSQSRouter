using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace ASBToSQSRouter.SQSEndpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("ASBToSQSRouter.SQSEndpoint");

                    var transport = endpointConfiguration.UseTransport<SqsTransport>();
                    
                    transport.UnrestrictedDurationDelayedDelivery();

                    transport.Transactions(TransportTransactionMode.ReceiveOnly);

                    endpointConfiguration.EnableInstallers();
                    
                    return endpointConfiguration;
                })
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
    }
}