using Azure.Messaging.ServiceBus;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System.Text;

namespace Serilog.Sinks.ServiceBus.Sinks
{
    public class ServiceBusSink : ILogEventSink
    {
        readonly ServiceBusClient _client;
        readonly ServiceBusSender _sender;
        readonly ITextFormatter _formatter;
        readonly string? _pushOnlyWithProperty = null;
        public int LogCount { get; set; } = 0;

        public ServiceBusSink(ServiceBusClient client, string queueName, ITextFormatter formatter, string? pushOnlyWithProperty = null)
        {
            _formatter = formatter;
            _client = client;
            _sender = _client.CreateSender(queueName);
            _pushOnlyWithProperty = pushOnlyWithProperty;
        }

        public void Emit(LogEvent logEvent)
        {
            if (!ShouldPush(logEvent)) return;

            if (IsWarmup(logEvent)) return;

            byte[] body;
            using (var render = new StringWriter())
            {
                _formatter.Format(logEvent, render);
                body = Encoding.UTF8.GetBytes(render.ToString());
            }

            var message = new ServiceBusMessage(body);
            _sender.SendMessageAsync(message).GetAwaiter().GetResult();
        }

        private bool IsWarmup(LogEvent logEvent)
        {
            try
            {
                //Keeps Service bus connection alive;
                if (logEvent.Properties.TryGetValue("Warmup", out var propertyValue))
                {
                    _sender.CreateMessageBatchAsync().GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (Exception)
            {
                //Eat
            }

            return false;
        }

        /// <summary>
        /// If you don't want to log certain events, you can add a property to the log event with the name specified in the ignorePropertyName parameter.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        private bool ShouldPush(LogEvent logEvent)
        {
            if (string.IsNullOrEmpty(_pushOnlyWithProperty))
                return true;

            return (logEvent.Properties.TryGetValue(_pushOnlyWithProperty, out var propertyValue));
        }
    }
}
