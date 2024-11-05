using MassTransit;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.Activities
{
    public class BatchJobDoneActivity : IStateMachineActivity<BatchStateData, ExportTokensBatchJobDone>
    {
        public BatchJobDoneActivity()
        {
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<BatchStateData, ExportTokensBatchJobDone> context, IBehavior<BatchStateData, ExportTokensBatchJobDone> next)
        {
            var batch = context.Saga;
            var message = context.Message;

            batch.CorrelationId = message.BatchId;

            // always call the next activity in the behavior
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<BatchStateData, ExportTokensBatchJobDone, TException> context, IBehavior<BatchStateData, ExportTokensBatchJobDone> next) where TException : Exception
        {
            // always call the next activity in the behavior
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("batch-job-done");
        }
    }
}
