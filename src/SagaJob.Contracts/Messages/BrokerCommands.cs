namespace SagaJob.Contracts.Messages
{
    public record ExportTokens(Guid CorrelationId);
    public record SubmitBatch(Guid[]? TokenIds = null, int TokenCount = 10000, int ActiveThreshold = 20);
}
