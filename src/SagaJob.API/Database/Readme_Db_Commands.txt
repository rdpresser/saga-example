==> command to add a new migration
Shell: dotnet ef migrations add <migration-name> -c AppDbContext
PMC: Add-Migration <migration-name> -Context AppDbContext -Project SagaJob.API -StartupProject SagaJob.API

==> command to remove the last migration created
Shell: dotnet ef migrations remove -c AppDbContext
PMC: Remove-Migration -Context AppDbContext -Project SagaJob.API -StartupProject SagaJob.API

==> command to update the database with all pending migrations
Shell: dotnet ef database update -c AppDbContext -s SagaJob.API
PMC: Update-Database -Context AppDbContext -Project SagaJob.API -StartupProject SagaJob.API

==> command to remove all migrations from database but not the files from project
Shell: dotnet ef database update 0 -c AppDbContext
PMC: Update-Database -Migration 0 -Context AppDbContext -Project SagaJob.API -StartupProject SagaJob.API

==> command to undo to a specific migration
Shell: dotnet ef database update <migration-name> -c AppDbContext -s SagaJob.API
PMC: Update-Database <migration-name> -Context AppDbContext -Project SagaJob.API -StartupProject SagaJob.API

==> command to generate the SQL Script DB for ALL database
Shell: dotnet ef migrations script -c AppDbContext | out-file ./scriptFile.sql
PMC: Script-Migration <migration-name>

==> command to upgrade a specific database without having to change the environment
PMC: $env:ASPNETCORE_ENVIRONMENT="DIT"

==> Command to install EF Core Migration CLI Tool
dotnet tool install --global dotnet-ef

==> Command to update EF Core Migration CLI Tool with the latest version
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef

==> Command to list all dotnet Tools installed
dotnet tool list