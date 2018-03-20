using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;

namespace NServiceBusAndDocker.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IEndpointInstance endpoint;

        protected void Application_Start()
        {
            var endpointConfiguration = new EndpointConfiguration("NServiceBusAndDocker.MVC");
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.SendOnly();

            const string connectionString = @"Data Source=sql; Initial Catalog=NsbSqlPersistenceAndTransport; Integrated Security=False; User ID=sa; Password=Password123!; Connect Timeout=60; Encrypt=False;";
            //const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=NsbSqlPersistenceAndTransport;Integrated Security=True;Max Pool Size=100";

            var transportSettings = endpointConfiguration.UseTransport<SqlServerTransport>();
            transportSettings.ConnectionString(connectionString);
            //https://docs.particular.net/transports/sql/native-delayed-delivery
            //var delayedDeliverySettings = transportSettings.UseNativeDelayedDelivery();
            //delayedDeliverySettings.TableSuffix("Delayed");
            transportSettings.UseSchemaForQueue("error", "dbo");
            transportSettings.UseSchemaForQueue("audit", "dbo");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema("mvc");
            persistence.ConnectionBuilder(() => new SqlConnection(connectionString));
            persistence.TablePrefix("");
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            SqlHelper.CreateSchema(connectionString, "mvc");

            var routingSettings = transportSettings.Routing();
            routingSettings.RouteToEndpoint(typeof(NServiceBusAndDocker.Messages.Commands.TestCommand), "NServiceBusAndDocker.Endpoint");
            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            var mvcContainerBuilder = new ContainerBuilder();
            mvcContainerBuilder.RegisterInstance(endpoint);
            mvcContainerBuilder.RegisterControllers(typeof(MvcApplication).Assembly);
            var mvcContainer = mvcContainerBuilder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(mvcContainer));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}
