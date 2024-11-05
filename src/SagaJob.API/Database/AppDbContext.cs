using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SagaJob.API.Database.Configurations;
using SagaJob.API.Sagas.StateMachine.Batch;
using SagaJob.API.Sagas.StateMachine.Job;
using JobSagaMap = SagaJob.API.Database.Configurations.JobSagaMap;

namespace SagaJob.API.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : SagaDbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });

            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Model.GetEntityTypes().ToList().ForEach(entityType =>
            {
                entityType.SetTableName(entityType.DisplayName());

                entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(string))
                    .Select(p => modelBuilder.Entity(p.DeclaringType.ClrType).Property(p.Name))
                    .ToList()
                    .ForEach(property =>
                    {
                        property.IsUnicode(false);
                        property.HasMaxLength(2000);
                    });
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<BatchStateData> BatchStates { get; set; }
        public DbSet<JobStateData> JobStates { get; set; }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new BatchSagaMap();
                yield return new JobSagaMap();
            }
        }
    }
}
