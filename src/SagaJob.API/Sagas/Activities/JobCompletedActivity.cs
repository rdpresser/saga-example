using MassTransit;
using SagaJob.API.Sagas.StateMachine.Job;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.Activities
{
    public class JobCompletedActivity : IStateMachineActivity<JobStateData, ExportTokensJobCompleted>
    {
        public JobCompletedActivity()
        {

        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<JobStateData, ExportTokensJobCompleted> context, IBehavior<JobStateData, ExportTokensJobCompleted> next)
        {
            var job = context.Saga;
            var message = context.Message;

            job.CorrelationId = message.JobId;
            job.BatchId = message.BatchId;

            // always call the next activity in the behavior
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<JobStateData, ExportTokensJobCompleted, TException> context, IBehavior<JobStateData, ExportTokensJobCompleted> next) where TException : Exception
        {
            // always call the next activity in the behavior
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("batch-job-completed");
        }
    }
}
