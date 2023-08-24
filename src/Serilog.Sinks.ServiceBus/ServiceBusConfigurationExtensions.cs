using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.ServiceBus.Sinks;

namespace Serilog.Sinks.ServiceBus
{
    public static class ServiceBusConfigurationExtensions
    {
        /// <summary>
        /// Add a sink that writes log events as messages to an Azure Service Bus queue.
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="connectionString"></param>
        /// <param name="queueName"></param>
        /// <param name="textFormatter"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="levelSwitch"></param>
        /// <param name="ignorePropertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration ServiceBus(
            this LoggerSinkConfiguration sinkConfiguration,
            string connectionString,
            string queueName,
            ITextFormatter textFormatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null,
            string? pushOnlyWithProperty = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));

            var client = new ServiceBusClient(connectionString);
            var adminClient = new ServiceBusAdministrationClient(connectionString);

            if (!adminClient.QueueExistsAsync(queueName).GetAwaiter().GetResult())
            {
                adminClient.CreateQueueAsync(queueName).GetAwaiter().GetResult();
            }

            return sinkConfiguration.Sink(new ServiceBusSink(client, queueName, textFormatter, pushOnlyWithProperty), restrictedToMinimumLevel, levelSwitch);
        }
        /// <summary>
        /// Add a sink that writes log events as messages to an Azure Service Bus queue.
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="nameSpace"></param>
        /// <param name="queueName"></param>
        /// <param name="tokenCredential"></param>
        /// <param name="textFormatter"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="levelSwitch"></param>
        /// <param name="ignorePropertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration ServiceBus(
        this LoggerSinkConfiguration sinkConfiguration,
        string nameSpace,
        string queueName,
        TokenCredential? tokenCredential,
        ITextFormatter textFormatter,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        LoggingLevelSwitch? levelSwitch = null,
        string? pushOnlyWithProperty = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (tokenCredential == null) throw new ArgumentNullException(nameof(tokenCredential));

            var client = new ServiceBusClient(nameSpace, tokenCredential);
            var adminClient = new ServiceBusAdministrationClient(nameSpace, tokenCredential);

            if (!adminClient.QueueExistsAsync(queueName).GetAwaiter().GetResult())
            {
                adminClient.CreateQueueAsync(queueName).GetAwaiter().GetResult();
            }

            return sinkConfiguration.Sink(new ServiceBusSink(client, queueName, textFormatter, pushOnlyWithProperty), restrictedToMinimumLevel, levelSwitch);
        }

    }
}
