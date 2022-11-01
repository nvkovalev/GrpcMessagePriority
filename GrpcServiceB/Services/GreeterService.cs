using Grpc.Core;
using System.Threading.Channels;

namespace GrpcServiceB.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private const int CHANNELS_COUNT = 5;

        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        private static readonly Channel<MessageRequest>[] _channels = new Channel<MessageRequest>[CHANNELS_COUNT];
      
        static GreeterService()
        {
            for (int i = 0; i < CHANNELS_COUNT; i++)
            {
                _channels[i] = Channel.CreateUnbounded<MessageRequest>();
            }
        }

        public override async Task<MessageReply> ProcessMessage(MessageRequest request, ServerCallContext context)
        {
            var priority = request.Priority;
            if (priority < CHANNELS_COUNT)
            {
                await _channels[priority].Writer.WriteAsync(request);
            }

            return new MessageReply { Message = $"{request.Priority}: Message {request.Name} is on processing" };
        }

        public static async Task ReadMessageFromChannel()
        {
            while (true)
            {
                for(int i = 0; i < CHANNELS_COUNT; i++)
                {
                    var reader = _channels[i].Reader;
                    if(reader.Count > 0)
                    {
                        for(var j = 0; j < reader.Count; j++)
                        {
                            if (reader.TryRead(out var msg)) {
                                await ProcessMessageAsync(msg);
                            }
                        }

                        break;
                    }
                }
            }
        }

        private static async Task ProcessMessageAsync(MessageRequest msg)
        {
            await Task.Delay(1000);
            Console.WriteLine($"Priority = {msg.Priority}: Message processed: {msg.Name}");
        }
    }
}