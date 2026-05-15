using System.Text;
using RabbitMQ.Client;

Console.WriteLine("Noise Sensor started.");

Send();

void Send()
{
   var factory = new ConnectionFactory()
    {
        //HostName = "localhost", // When you run locally 
        HostName = "rabbitmq", // When run in docker compose 
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

        while (true)
        {
            Console.Write("Please enter decibels: ");

            string? decibels = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(decibels))
            {
                var body = Encoding.UTF8.GetBytes(decibels);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "telemetry",
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"[x] Sent {decibels}");
            }
        }
    }
}