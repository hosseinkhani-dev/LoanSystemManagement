using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

string connectionString = GetConnectionString();

CreateDatabaseIfNotExist(connectionString);

using (var serviceProvider = CreateServices(connectionString))
using (var scope = serviceProvider.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider);
}

/// <summary>
/// Read connection string from .json
/// </summary>
static string GetConnectionString()
{
    string filePath = "migrations.json";
    string json = File.ReadAllText(filePath);
    var settings = JsonSerializer.Deserialize<AppSettings>(json);
    return settings.ConnectionStrings.LoanManagementConnectionString;
}

/// <summary>
/// Configure the dependency injection services
/// </summary>
static ServiceProvider CreateServices(
   string connectionString)
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddSqlServer()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(Program).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

/// <summary>
/// Create Database if it isn't exist
/// </summary>
static void CreateDatabaseIfNotExist(string connectionString)
{
    var builder = new SqlConnectionStringBuilder(connectionString);
    string databaseName = builder.InitialCatalog;
    builder.InitialCatalog = "master";
    using (var connection = new SqlConnection(builder.ConnectionString))
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText =
                $"IF NOT EXISTS (SELECT" +
                $" name FROM sys.databases WHERE" +
                $" name = N'{databaseName}') " +
                $"CREATE DATABASE [{databaseName}]";
            command.ExecuteNonQuery();
        }
    }
}


/// <summary>
/// Update the database
/// </summary>
static void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

    runner.MigrateUp();
}

class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }
}

class ConnectionStrings
{
    public string LoanManagementConnectionString { get; set; }
}