using MassTransit;
using SagaJob.API.Sagas.Activities;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.StateMachine.Job
{
    public class JobStateMachine : MassTransitStateMachine<JobStateData>
    {
        public State JobReceivedState { get; private set; }
        public State JobCompletedState { get; private set; }
        public State JobReceivedFailedState { get; private set; }
        public State JobCompletedFailedState { get; private set; }

        public Event<ExportTokensJobReceived> JobReceivedEvent { get; private set; }
        public Event<ExportTokensJobCompleted> JobCompletedEvent { get; private set; }
        public Event<Fault<ExportTokensJobReceived>> JobReceivedFailedEvent { get; private set; }
        public Event<Fault<ExportTokensJobCompleted>> JobCompletedFailedEvent { get; private set; }

        public JobStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => JobReceivedEvent, x => x.CorrelateById(context => context.Message.JobId));
            Event(() => JobCompletedEvent, x => x.CorrelateById(context => context.Message.JobId));
            Event(() => JobReceivedFailedEvent, x => x.CorrelateById(context => context.Message.Message.JobId));
            Event(() => JobCompletedFailedEvent, x => x.CorrelateById(context => context.Message.Message.JobId));

            Initially(
                When(JobReceivedEvent)
                    .Activity(x => x.OfType<JobReceivedActivity>())
                    .TransitionTo(JobReceivedState),
                When(JobReceivedFailedEvent)
                    .Activity(x => x.OfType<FaultActivity<JobStateData, ExportTokensJobReceived>>())
                    .TransitionTo(JobReceivedFailedState)
            );

            During(JobReceivedState,
                When(JobCompletedEvent)
                    .Activity(x => x.OfType<JobCompletedActivity>())
                    .TransitionTo(JobCompletedState),
                When(JobCompletedFailedEvent)
                    .Activity(x => x.OfType<FaultActivity<JobStateData, ExportTokensJobCompleted>>())
                    .TransitionTo(JobCompletedFailedState)
            );

            During(JobCompletedState,
                Ignore(JobReceivedEvent),
                Ignore(JobCompletedEvent)
            );
        }
    }
}
