using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Telemetry Consumer started.");

var factory = new ConnectionFactory()
{
    HostName = "rabbitmq",
    UserName = "admin",
    Password = "admin@123"
};

IConnection? connection = null;

while (connection == null)
{
    try
    {
        Console.WriteLine("Trying to connect to RabbitMQ...");

        connection = factory.CreateConnection();

        Console.WriteLine("Connected!");
    }
    catch
    {
        Console.WriteLine("RabbitMQ not ready. Retrying in 5 seconds...");
        Thread.Sleep(5000);
    }
}

using (connection)
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(
        queue: "telemetry",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);


    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();

        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"[x] Received: {message}");

        File.AppendAllText(
            "telemetry.txt",
            $"{DateTime.Now}: {message}{Environment.NewLine}");
    };

    channel.BasicConsume(
        queue: "telemetry",
        autoAck: true,
        consumer: consumer);

    Console.WriteLine("Waiting for messages...");

    while (true)
    {
        Thread.Sleep(1000);
    }
}