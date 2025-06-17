using GenApi.Models;

using MassTransit;

namespace GenApi.Services.RabbitMQ
{
    public class SubmitAnimeConsumer : IConsumer<Anime>
    {
        public Task Consume(ConsumeContext<Anime> context)
        {
            Anime message = context.Message;
            Log.Information($"[✔] Anime Received: {message.ToJsonString()}");
            return Task.CompletedTask;
        }
    }
}
