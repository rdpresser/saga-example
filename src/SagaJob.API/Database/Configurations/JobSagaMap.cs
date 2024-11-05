using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SagaJob.API.Sagas.StateMachine.Job;
using SagaJob.Contracts.Enums;

namespace SagaJob.API.Database.Configurations
{
    public class JobSagaMap : SagaClassMap<JobStateData>
    {
        protected override void Configure(EntityTypeBuilder<JobStateData> entity, ModelBuilder model)
        {
            entity.HasKey(x => x.CorrelationId);

            entity.Property(x => x.CorrelationId)
                .ValueGeneratedNever()
                .HasColumnName("JobId");

            entity.Property(x => x.CurrentState)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.BatchType)
                .HasConversion(new EnumToStringConverter<BatchTypeEnum>())
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(x => x.ExceptionMessage)
                .HasColumnType("varchar(max)");

            //if using Optimistic Concurrency, you need to add a RowVersion property to your Saga State otherwise, you can remove this line
            entity.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsRequired();
        }
    }
}
