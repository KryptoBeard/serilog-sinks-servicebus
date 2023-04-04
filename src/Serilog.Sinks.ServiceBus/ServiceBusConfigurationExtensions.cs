using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.ServiceBus.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.ServiceBus
{
    public static class ServiceBusConfigurationExtensions
    {
        public static LoggerConfiguration ServiceBus(
            this LoggerSinkConfiguration sinkConfiguration,
            string connectionString,
            string queueName,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider? formatProvider = null,
            LoggingLevelSwitch? levelSwitch = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));

            var client = new ServiceBusClient(connectionString);

            return sinkConfiguration.Sink(new ServiceBusSink(client, queueName, formatProvider), restrictedToMinimumLevel, levelSwitch);
        }

        public static LoggerConfiguration ServiceBus(
        this LoggerSinkConfiguration sinkConfiguration,
        string nameSpace,
        string queueName,
        TokenCredential? tokenCredential,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        IFormatProvider? formatProvider = null,
        LoggingLevelSwitch? levelSwitch = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if(tokenCredential == null) throw new ArgumentNullException(nameof(tokenCredential));

            var client = new ServiceBusClient(nameSpace, tokenCredential);

            return sinkConfiguration.Sink(new ServiceBusSink(client,queueName,formatProvider),restrictedToMinimumLevel,levelSwitch);
        }

    }
}
