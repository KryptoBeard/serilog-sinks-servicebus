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
        readonly ITextFormatter _formatter;

        public ServiceBusSink(ServiceBusClient client, string queueName, ITextFormatter formatter )
        {
            _formatter = formatter;
            _client = client;
            _sender = _client.CreateSender(queueName);
        }

        public void Emit(LogEvent logEvent)
        {
            byte[] body;
            using (var render = new StringWriter())
            {
                _formatter.Format(logEvent, render);
                body = Encoding.UTF8.GetBytes(render.ToString());
            }

            var message = new ServiceBusMessage(body);
            _sender.SendMessageAsync(message).GetAwaiter().GetResult();     
        }
    }
}
