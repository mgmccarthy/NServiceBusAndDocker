
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

            const string connectionString = @"Data Source=sql; Initial Catalog=NsbSqlPersistenceAndTransport; Integrated Security=False; User ID=sa; Password=Password123!; Connect Timeout=60; Encrypt=False;";
            //const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=NsbSqlPersistenceAndTransport;Integrated Security=True;Max Pool Size=100";

            var transportSettings = endpointConfiguration.UseTransport<SqlServerTransport>();
            transportSettings.ConnectionString(connectionString);
            transportSettings.UseSchemaForQueue("error", "dbo");
            transportSettings.UseSchemaForQueue("audit", "dbo");
            transportSettings.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            SqlHelper.CreateSchema(connectionString, "endpoint");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema("endpoint");
            persistence.ConnectionBuilder(connectionBuilder: () => { return new SqlConnection(connectionString); });
            persistence.TablePrefix("");
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            var routingSettings = transportSettings.Routing();
            routingSettings.RegisterPublisher(typeof(NServiceBusAndDocker.Messages.Events.TestEvent), "NServiceBusAndDocker.Endpoint");
        }
    }
}
