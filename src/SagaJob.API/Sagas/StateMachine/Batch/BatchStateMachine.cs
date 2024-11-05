using MassTransit;
using SagaJob.API.Sagas.Activities;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Sagas.StateMachine.Batch
{
    public class BatchStateMachine : MassTransitStateMachine<BatchStateData>
    {
        public State BatchStartedState { get; private set; }
        public State BatchFailedState { get; private set; }
        public State BatchFinishedState { get; private set; }

        public Event<ExportTokensBatchReceived> BatchReceivedEvent { get; private set; }
        public Event<ExportTokensBatchJobDone> BatchJobDoneEvent { get; private set; }
        public Event<Fault<ExportTokensBatchReceived>> BatchReceivedFailedEvent { get; private set; }
        public Event<Fault<ExportTokensBatchJobDone>> BatchJobDoneFailedEvent { get; private set; }

        public BatchStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BatchReceivedEvent, x => x.CorrelateById(context => context.Message.BatchId));
            Event(() => BatchJobDoneEvent, x => x.CorrelateById(context => context.Message.BatchId));
            Event(() => BatchReceivedFailedEvent, x => x.CorrelateById(context => context.Message.Message.BatchId));
            Event(() => BatchJobDoneFailedEvent, x => x.CorrelateById(context => context.Message.Message.BatchId));

            Initially(
                When(BatchReceivedEvent)
                    .Activity(x => x.OfType<BatchReceivedActivity>())
                    .TransitionTo(BatchStartedState),
                When(BatchReceivedFailedEvent)
                    .Activity(x => x.OfType<FaultActivity<BatchStateData, ExportTokensBatchReceived>>())
                    .TransitionTo(BatchFailedState)
            );

            During(BatchStartedState,
                When(BatchJobDoneEvent)
                    .Activity(x => x.OfType<BatchJobDoneActivity>())
                    .ThenAsync(context => Console.Out.WriteLineAsync($"BatchMainStateMachine: Received ExportTokensBatchJobDone"))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"BatchMainStateMachine: Transitioning to Finished"))
                    .TransitionTo(BatchFinishedState),
                When(BatchJobDoneFailedEvent)
                    .Activity(x => x.OfType<FaultActivity<BatchStateData, ExportTokensBatchJobDone>>())
                    .TransitionTo(BatchFailedState),
                Ignore(BatchReceivedEvent)
            );

            During(BatchFinishedState,
                Ignore(BatchJobDoneEvent));

            //SetCompletedWhenFinalized();

            static void Initialize(BehaviorContext<BatchStateData, ExportTokensBatchReceived> context)
            {
                context.Saga.CorrelationId = context.Message.BatchId;
                context.Saga.CreateTimestamp = context.Message.Timestamp;
                context.Saga.TotalRecords = context.Message.TokenIds.Length;
            }
        }
    }
}
