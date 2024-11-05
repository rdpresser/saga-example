
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaJob.API.Database;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.API.Sagas.StateMachine.Job;
using JobStateMachine = SagaJob.API.Sagas.StateMachine.Job.JobStateMachine;

namespace SagaJob.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers(typeof(Program).Assembly);
                x.AddActivities(typeof(Program).Assembly);

                #region Configuring the Saga State Machine
                x.AddSagaStateMachine<BatchStateMachine, BatchStateData>(typeof(BatchStateMachineDefinition))
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.ExistingDbContext<AppDbContext>();
                        r.UseSqlServer();
                    });
                #endregion

                x.AddSagaStateMachine<JobStateMachine, JobStateData>(typeof(JobStateMachineDefinition))
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.ExistingDbContext<AppDbContext>();
                        r.UseSqlServer();
                    });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri("amqp://sagajob-mq:5672"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    //https://masstransit.io/documentation/configuration
                    //cfg.ReceiveEndpoint("export-tokens-batch", e =>
                    //{
                    //    e.ConfigureConsumer<ExportTokensBatchReceivedConsumer>(context);
                    //    e.ConfigureConsumer<ExportTokensBatchJobDoneConsumer>(context);
                    //    e.ConfigureConsumer<ExportTokensBatchFailedConsumer>(context);
                    //});

                    cfg.UseInMemoryOutbox(context);

                    cfg.ConfigureEndpoints(context);
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
