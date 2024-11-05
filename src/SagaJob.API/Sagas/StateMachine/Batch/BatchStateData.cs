using MassTransit;
using SagaJob.API.Sagas.StateMachine.Job;
using SagaJob.Contracts.Enums;

namespace SagaJob.API.Sagas.StateMachine.Batch
{
    public class BatchStateData : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public required string CurrentState { get; set; }
        public Guid MerchantId { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public BatchTypeEnum BatchType { get; set; }
        /// <summary>
        /// The maximum amount of active Jobs allowed to be processing. Typically an amount larger than your Job Consumer can handle concurrently, to allow for some additional prefetch while the Batch Saga dispatches more
        /// </summary>
        public required int JobActiveThreshold { get; set; } = 20;
        public required int TotalRecords { get; set; }
        public Stack<Guid> UnprocessedTokenIds { get; set; } = new Stack<Guid>();
        public Dictionary<Guid, Guid> ProcessingTokenIds { get; set; } = []; // CorrelationId, TokenId
        public required byte[] RowVersion { get; set; }

        #region Navigation Properties
        public virtual ICollection<JobStateData>? Jobs { get; set; }
        #endregion
    }
}
