using MassTransit;
using SagaJob.API.Sagas.StateMachine.Job;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.Activities
{
    public class JobReceivedActivity : IStateMachineActivity<JobStateData, ExportTokensJobReceived>
    {
        public JobReceivedActivity()
        {

        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<JobStateData, ExportTokensJobReceived> context, IBehavior<JobStateData, ExportTokensJobReceived> next)
        {
            var job = context.Saga;
            var message = context.Message;

            job.CorrelationId = message.JobId;
            job.BatchId = message.BatchId;
            job.BatchType = message.BatchType;

            // always call the next activity in the behavior
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<JobStateData, ExportTokensJobReceived, TException> context, IBehavior<JobStateData, ExportTokensJobReceived> next) where TException : Exception
        {
            // always call the next activity in the behavior
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("batch-job-received");
        }
    }
}
