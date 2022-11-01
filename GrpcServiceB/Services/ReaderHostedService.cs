using System.Threading;

namespace GrpcServiceB.Services
{
    public class ReaderHostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ReaderHostedService");
            new Thread(async () => await GreeterService.ReadMessageFromChannel()).Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
