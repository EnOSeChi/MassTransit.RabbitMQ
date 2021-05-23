using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.RabbitMQ.Sample
{
    public class Worker : BackgroundService
    {
        readonly IBus _bus;

        public Worker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // var endpoint = await _bus.GetSendEndpoint(new Uri("queue:messages-test"));

                // Set CorrelationId using SendContext<T>
                // await endpoint.Send<Message>(new Message { Text = $"The time is {DateTimeOffset.Now}" }, context =>
                    //context.SetRoutingKey("test"));
                await _bus.Publish(new Message { Text = $"The time is {DateTimeOffset.Now}" }, ctx => ctx.SetRoutingKey("ttt.ttt"));

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
