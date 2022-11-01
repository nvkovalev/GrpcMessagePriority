using Grpc.Net.Client;
using GrpcServiceAClient;

const int PRIORITY_COUNT = 5;
const int MESSAGES_MAX_COUNT = 100;
const int SLEEP_TIME_HOURS = 1;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);

var rnd = new Random();
var priorityRnd = new Random();

while (true)
{
    var messageCount = rnd.Next(MESSAGES_MAX_COUNT);
    Console.WriteLine($"Message count: {messageCount}");
    for (int i = 0; i < messageCount; i++)
    {
        var reply = await client.ProcessMessageAsync(
            new MessageRequest
            {
                Name = Guid.NewGuid().ToString(),
                Priority = priorityRnd.Next(0, PRIORITY_COUNT)
            });
        Console.WriteLine($"{reply.Message}");
    }

    await Task.Delay(TimeSpan.FromHours(SLEEP_TIME_HOURS));
}