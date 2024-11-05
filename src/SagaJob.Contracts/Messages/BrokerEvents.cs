using SagaJob.Contracts.Enums;

namespace SagaJob.Contracts.Messages
{
    #region Batch Saga Contracts
    public interface ExportTokensBatchReceived
    {
        Guid BatchId { get; }
        DateTime Timestamp { get; }
        Guid MerchantId { get; }
        Guid[] TokenIds { get; }
        int ActiveThreshold { get; }
    }

    public interface ExportTokensBatchJobDone
    {
        Guid BatchJobId { get; }
        Guid BatchId { get; }
        Guid TokenId { get; }
        DateTime Timestamp { get; }
        bool LastMessage { get; }
        string ExceptionInfo { get; }
    }
    #endregion

    #region Job Saga Contracts

    public interface ExportTokensJobReceived
    {
        Guid JobId { get; }
        Guid BatchId { get; }
        Guid MerchantId { get; }
        BatchTypeEnum BatchType { get; }
        int CurrentPage { get; }
        int PageSize { get; }
    }

    public record ExportTokensJobCompleted(Guid JobId, Guid BatchId, Guid MerchantId, int CurrentPage, int PageSize, DateTime Timestamp, bool LastJob, string ExceptionInfo);
    //public record ExportTokensJobFailed(Guid JobId, Guid BatchId, Guid MerchantId, int CurrentPage, int PageSize, DateTime Timestamp, string ExceptionInfo);
    #endregion
}