using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Router;
using NServiceBus.Router.Hosting;
using NServiceBus.Unicast.Messages;

namespace ASBToSQSRouter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBusRouter(context =>
                {
                    var routerConfig = new RouterConfiguration("ASBToSQS.Router");

                    var azureInterface = routerConfig.AddInterface<AzureServiceBusTransport>("ASB", t =>
                    {
                        t.ConnectionString(Environment.GetEnvironmentVariable("ASB_CONNECTION_STRING"));

                        t.Transactions(TransportTransactionMode.ReceiveOnly);
                        t.SubscriptionRuleNamingConvention((entityType) =>
                        {
                            var entityPathOrName = entityType.Name;
                            if (entityPathOrName.Length >= 50)
                            {
                                return entityPathOrName.Split('.').Last();
                            }

                            return entityPathOrName;
                        });
                    });
                    
                    var sqsInterface = routerConfig.AddInterface<SqsTransport>("SQS", t =>
                    {
                        t.UnrestrictedDurationDelayedDelivery();

                        t.Transactions(TransportTransactionMode.ReceiveOnly);

                        var settings = t.GetSettings();

                        // Avoids a missing setting error
                        //https://github.com/SzymonPobiega/NServiceBus.Raw/blob/master/src/AcceptanceTests.SQS/Helper.cs#L18
                        bool isMessageType(Type t) => true;
                        var ctor = typeof(MessageMetadataRegistry).GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                            new[] {typeof(Func<Type, bool>)}, null);
#pragma warning disable CS0618 // Type or member is obsolete
                        settings.Set<MessageMetadataRegistry>(ctor.Invoke(new object[] {(Func<Type, bool>) isMessageType}));
#pragma warning restore CS0618 // Type or member is obsolete

                    });

                    var staticRouting = routerConfig.UseStaticRoutingProtocol();

                    staticRouting.AddForwardRoute("ASB", "SQS");

                    routerConfig.AutoCreateQueues();
                    
                    return routerConfig;
                })
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
    }
}