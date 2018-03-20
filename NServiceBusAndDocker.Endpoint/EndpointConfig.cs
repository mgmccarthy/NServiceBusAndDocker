
namespace NServiceBusAndDocker.Endpoint
{
    using NServiceBus;
    using NServiceBus.Persistence.Sql;
    using NServiceBus.Transport.SQLServer;
    using System;
    using System.Data.SqlClient;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            //var connection = @"Data Source=sql; Initial Catalog=NsbSqlPersistenceAndTransport; Integrated Security=False; User ID=sa; Password=Password123!; Connect Timeout=60; Encrypt=False;";
            var connection = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=NsbSqlPersistenceAndTransport;Integrated Security=True;Max Pool Size=100";

            var transportSettings = endpointConfiguration.UseTransport<SqlServerTransport>();
            transportSettings.ConnectionString(connection);
            transportSettings.UseSchemaForQueue("error", "dbo");
            transportSettings.UseSchemaForQueue("audit", "dbo");
            transportSettings.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            SqlHelper.CreateSchema(connection, "endpoint");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema("endpoint");
            persistence.ConnectionBuilder(connectionBuilder: () => { return new SqlConnection(connection); });
            persistence.TablePrefix("");
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            var routingSettings = transportSettings.Routing();
            routingSettings.RegisterPublisher(typeof(NServiceBusAndDocker.Messages.Events.TestEvent), "NServiceBusAndDocker.Endpoint");
        }
    }
}
