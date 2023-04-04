using Azure.Messaging.ServiceBus;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.ServiceBus.Sinks
{
    public class ServiceBusSink : ILogEventSink
    {
        readonly ServiceBusClient _client;
        readonly ServiceBusSender _sender;
        readonly IFormatProvider? _formatProvider;

        public ServiceBusSink(ServiceBusClient client, string queueName, IFormatProvider? formatProvider = null)
        {
            _formatProvider = formatProvider;
            _client = client;
            _sender = _client.CreateSender(queueName);
        }

        public void Emit(LogEvent logEvent)
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(logEvent.RenderMessage(_formatProvider)));
            _sender.SendMessageAsync(message).GetAwaiter().GetResult();     
        }
    }
}
