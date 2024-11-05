using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.Contracts.Enums;

namespace SagaJob.API.Database.Configurations
{
    public class BatchSagaMap : SagaClassMap<BatchStateData>
    {
        protected override void Configure(EntityTypeBuilder<BatchStateData> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState)
                .HasMaxLength(100)
                .IsRequired();

            //if using Optimistic Concurrency, you need to add a RowVersion property to your Saga State otherwise, you can remove this line
            entity.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsRequired();

            entity.HasKey(x => x.CorrelationId);

            entity.Property(x => x.CorrelationId)
                .ValueGeneratedNever()
                .HasColumnName("BatchId");

            entity.Property(c => c.UnprocessedTokenIds)
                .HasConversion(new JsonValueConverter<Stack<Guid>>())
                .Metadata.SetValueComparer(new JsonValueComparer<Stack<Guid>>());

            entity.Property(c => c.ProcessingTokenIds)
                .HasConversion(new JsonValueConverter<Dictionary<Guid, Guid>>())
                .Metadata.SetValueComparer(new JsonValueComparer<Dictionary<Guid, Guid>>());

            entity.Property(x => x.BatchType)
                .HasConversion(new EnumToStringConverter<BatchTypeEnum>())
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
