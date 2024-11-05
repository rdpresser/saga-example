using MassTransit;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.StateMachine.Job
{
    public class JobStateMachineDefinition : SagaDefinition<JobStateData>
    {
        public JobStateMachineDefinition()
        {
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<JobStateData> sagaConfigurator, IRegistrationContext context)
        {
            sagaConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
            sagaConfigurator.UseInMemoryOutbox(context);

            var partition = endpointConfigurator.CreatePartitioner(8);
            sagaConfigurator.Message<ExportTokensJobReceived>(m => m.UsePartitioner(partition, x => x.Message.BatchId));
            sagaConfigurator.Message<ExportTokensJobCompleted>(m => m.UsePartitioner(partition, x => x.Message.BatchId));

            base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
        }
    }
}