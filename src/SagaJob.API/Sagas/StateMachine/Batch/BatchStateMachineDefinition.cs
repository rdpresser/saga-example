using MassTransit;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.StateMachine.Batch
{
    public class BatchStateMachineDefinition : SagaDefinition<BatchStateData>
    {
        public BatchStateMachineDefinition()
        {
            //Trazer essa informação do appsettings.json via IOptions
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<BatchStateData> sagaConfigurator, IRegistrationContext context)
        {
            sagaConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
            sagaConfigurator.UseInMemoryOutbox(context);

            var partition = endpointConfigurator.CreatePartitioner(8);
            sagaConfigurator.Message<ExportTokensBatchReceived>(m => m.UsePartitioner(partition, x => x.Message.BatchId));
            sagaConfigurator.Message<ExportTokensBatchJobDone>(m => m.UsePartitioner(partition, x => x.Message.BatchId));

            base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
        }
    }
}
