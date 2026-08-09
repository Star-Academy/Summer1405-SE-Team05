using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace cleanCode.integration.sqlserver;

public class MsSqlDatabaseFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; } = new MsSqlBuilder("mcr.hamdocker.ir/mssql/server:2022-latest")
        .WithPassword("Your_strong_Password123")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        await SeedDatabaseAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        await using var connection = await CreateConnectionAsync();

        var sqlScript = @"
        CREATE TABLE student (
            studentnumber VARCHAR(20) PRIMARY KEY,
            firstname NVARCHAR(50) NOT NULL,
            lastname NVARCHAR(50) NOT NULL,
            grade DECIMAL(4,2) NOT NULL
        );

        INSERT INTO student (studentnumber, firstname, lastname, grade) VALUES
            ('98100201', N'سارا', N'رضایی', 19.00),
            ('97100112', N'زهرا', N'کریمی', 19.00),
            ('97100166', N'ریحانه', N'امینی', 17.00),
            ('99100305', N'علی', N'احمدی', 13.25),
            ('97100999', N'مهدی', N'باقری', 16.00);";

        await using var command = new SqlCommand(sqlScript, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<SqlConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}