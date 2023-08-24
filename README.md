# serilog-sinks-servicebus

![alt text](https://img.shields.io/nuget/v/Serilog.Sinks.ServiceBus)

```
dotnet add package Serilog.Sinks.ServiceBus --version 1.0.4
```

### With Connection string
```c#
var log = new LoggerConfiguration()
    .WriteTo.ServiceBus("<connectionString>","<queueName>")
    .CreateLogger();
```

### With Identity
```c#
var log = new LoggerConfiguration()
    .WriteTo.ServiceBus("<serviceBusNameSpace>","<queueName>", new DefaultAzureCredential())
    .CreateLogger();
```



### pushOnlyWithProperty

Added support to ignore specific log events. Passing a defined property in the log context allows you to not send it to service bus. Can be useful if you only want specific events to make it to the bus without changing log level.
