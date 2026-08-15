using System.Threading.Tasks;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace cleanCode.integration.postgres;

public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("hub.hamdocker.ir/library/postgres:16")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await SetupDatabaseAsync();
    }
//breack
    private async Task SetupDatabaseAsync()
    {
        await using var connection = await CreateConnectionAsync();

        var sqlScript = @"
        CREATE TABLE student (
            studentnumber VARCHAR(20) PRIMARY KEY,
            firstname VARCHAR(50) NOT NULL,
            lastname VARCHAR(50) NOT NULL,
            grade REAL NOT NULL,
            age INT NOT NULL
        );";
            

        var insertcmd = @"INSERT INTO student (studentnumber, firstname, lastname, grade, age) VALUES
            ('98100201', 'سارا', 'رضایی', 18.75, 22),
            ('97100112', 'زهرا', 'کریمی', 19.50, 24),
            ('99100305', 'علی', 'احمدی', 13.25, 20),
            ('97100999', 'مهدی', 'باقری', 16.00, 25),
            ('00100412', 'رضا', 'محمدی', 2.50, 19),
            ('01100523', 'مریم', 'حسینی', 20.00, 18),
            ('96100888', 'امیر', 'قاسمی', 0.00, 25),
            ('02100644', 'نیلوفر', 'کاظمی', 11.80, 17),
            ('03100755', 'پارسای', 'شریفی', 8.25, 16),
            ('04100866', 'آرمینا', 'سهرابی', 17.50, 15),
            ('99100123', 'سینا', 'نوری', 5.00, 21),
            ('00100987', 'الهه', 'جعفری', 14.90, 19),
            ('98100456', 'کیوان', 'عباسی', 1.25, 23),
            ('02100321', 'فاطمه', 'ابراهیمی', 10.00, 17),
            ('03100111', 'دانیال', 'موسوی', 15.50, 16);";

        await using var command = new NpgsqlCommand(sqlScript + insertcmd, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}