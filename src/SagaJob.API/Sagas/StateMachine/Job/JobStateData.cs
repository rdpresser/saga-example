using MassTransit;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.Contracts.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagaJob.API.Sagas.StateMachine.Job
{
    public class JobStateData : SagaStateMachineInstance
    {
        public Guid BatchId { get; set; }
        /// <summary>
        /// The name CorrelationId is a MassTransit convention, it is used to correlate the Job with the Batch
        /// However, despite the name, it is not a correlationId, it is the JobId field
        /// </summary>
        public Guid CorrelationId { get; set; }
        public Guid MerchantId { get; set; }
        public required string CurrentState { get; set; }
        //Iniciar sem o notmapped, mas talvez não faça sentido gravar todos os dados nessa propriedade
        [NotMapped]
        //public List<TokenDetails> TokenDetails { get; set; } = [];
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public BatchTypeEnum BatchType { get; set; }
        public string? ExceptionMessage { get; set; }
        public bool LastJob { get; set; }
        /// <summary>
        /// Field to store if the job has already been processed
        /// </summary>
        public bool Processed { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public required byte[] RowVersion { get; set; }

        public virtual BatchStateData Batch { get; set; }
    }
}
