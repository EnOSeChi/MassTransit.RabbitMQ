using MassTransit.RabbitMQ.Sample;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace MassTransit.RabbitMQ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Message<Message>(x => x.SetEntityName("message"));

                    cfg.Publish<Message>(x =>
                    {
                        x.Durable = true; // default: true
                        x.AutoDelete = false; // default: false
                        x.ExchangeType = ExchangeType.Topic; // default, allows any valid exchange type
                    });

                    cfg.ReceiveEndpoint("messages-test", x =>
                    {
                        x.ConfigureConsumeTopology = false;

                        x.Consumer<MessageConsumer>(context);
                        
                        x.Bind("message", s =>
                        {
                            s.RoutingKey = "#";
                            s.ExchangeType = ExchangeType.Topic;
                        });
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MassTransit.RabbitMQ", Version = "v1" });
            });

            services.AddHostedService<Worker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MassTransit.RabbitMQ v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
