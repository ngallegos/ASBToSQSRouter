using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASBToSQSRouter.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace ASBToSQSRouter.ASBEndpoint
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
                    var endpointConfiguration = new EndpointConfiguration("ASBToSQSRouter.ASBEndpoint");

                    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                    
                    transport.SubscriptionRuleNamingConvention((entityType) =>
                    {
                        var entityPathOrName = entityType.Name;
                        if (entityPathOrName.Length >= 50)
                        {
                            return entityPathOrName.Split('.').Last();
                        }

                        return entityPathOrName;
                    });

                    transport.Transactions(TransportTransactionMode.ReceiveOnly);
                    transport.ConnectionString(Environment.GetEnvironmentVariable("ASB_CONNECTION_STRING"));

                    var bridge = transport.Routing().ConnectToRouter("ASBToSQS.Router");
                    
                    bridge.RouteToEndpoint(typeof(ASBToSQSCommand), "ASBToSQSRouter.SQSEndpoint");
                    bridge.RegisterPublisher(typeof(ASBToSQSEvent), "ASBToSQSRouter.SQSEndpoint");

                    endpointConfiguration.EnableInstallers();
                    return endpointConfiguration;
                })
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
    }
}