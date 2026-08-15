using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ASP.integration.fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("hub.hamdocker.ir/library/postgres:16")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        await SetupDatabaseAsync();
    }

    private async Task SetupDatabaseAsync()
    {
        await using var connection = await CreateConnectionAsync();

        var sqlScript = @"
        CREATE TABLE IF NOT EXISTS student (
            studentnumber VARCHAR(50) PRIMARY KEY,
            firstname VARCHAR(100) NOT NULL,
            lastname VARCHAR(100) NOT NULL,
            grade FLOAT NOT NULL,
            ismale BOOLEAN NOT NULL,
            leftunitscount INT NOT NULL,
            dateofbirth TIMESTAMP NOT NULL
        );

        INSERT INTO student (studentnumber, firstname, lastname, grade, ismale, leftunitscount, dateofbirth) VALUES
            ('98100201', 'سارا', 'رضایی', 18.75, false, 12, '2000-01-01 00:00:00'),
            ('99100305', 'علی', 'احمدی', 13.25, true, 20, '2001-05-15 00:00:00');";

        await using var command = new NpgsqlCommand(sqlScript, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}