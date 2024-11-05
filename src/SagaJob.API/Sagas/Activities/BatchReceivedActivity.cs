using MassTransit;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.Contracts.Messages;
using SagaJob.Contracts.Enums;

namespace SagaJob.API.Sagas.Activities
{
    public class BatchReceivedActivity : IStateMachineActivity<BatchStateData, ExportTokensBatchReceived>
    {
        public BatchReceivedActivity()
        {
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<BatchStateData, ExportTokensBatchReceived> context, IBehavior<BatchStateData, ExportTokensBatchReceived> next)
        {
            var batch = context.Saga;
            var message = context.Message;

            batch.CorrelationId = message.BatchId;
            batch.BatchType = message.MerchantId == Guid.Empty ? BatchTypeEnum.ByList : BatchTypeEnum.ByMerchandId;
            batch.CreateTimestamp = message.Timestamp;
            //
            //batch.BatchType ==>  decidir qual o tipo de batch baseado se veio apenas o MerchantId ou se veio também a Lista de TokenIds
            //
            batch.JobActiveThreshold = message.ActiveThreshold;
            batch.UnprocessedTokenIds = new Stack<Guid>(message.TokenIds);

            await context.Publish<ExportTokensJobReceived>(new
            {
                JobId = NewId.NextGuid(),
                message.BatchId,
                message.MerchantId,
                batch.BatchType,
                CurrentPage = 1,
                PageSize = 100
            });

            //context.PublishBatch<ExportTokensJobReceived>(new ExportTokensJobReceived
            //{

            //});

            // always call the next activity in the behavior
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<BatchStateData, ExportTokensBatchReceived, TException> context, IBehavior<BatchStateData, ExportTokensBatchReceived> next) where TException : Exception
        {
            // always call the next activity in the behavior
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("batch-received");
        }
    }
}
