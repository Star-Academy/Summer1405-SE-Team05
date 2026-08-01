using ClassLibrary1;
using Npgsql;
using Microsoft.Data.SqlClient;

var query = new Query()
    .From("student")
    .Select("studentnumber", "firstname")
    .Where("grade", 16);

Console.WriteLine("=== Testing PostgreSQL ===");
RunPostgres(query);

Console.WriteLine("\n=== Testing SQL Server ===");
RunSqlServer(query);

void RunPostgres(Query q)
{
    var compiler = new PostgresCompiler();
    SqlResult result = compiler.Compile(q);

    string connStr = "Host=localhost;Username=postgres;Password=postgres;Database=staracademy";

    using var conn = new NpgsqlConnection(connStr);
    conn.Open();

    using var cmd = new NpgsqlCommand(result.Sql, conn);
    foreach (var val in result.Bindings)
    {
        cmd.Parameters.AddWithValue(val);
    }

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"[PG] Student: {reader["studentnumber"]} - {reader["firstname"]}");
    }
}

void RunSqlServer(Query q)
{
    var compiler = new SqlServerCompiler();
    SqlResult result = compiler.Compile(q);

    string connStr = "Server=localhost,1433;Database=staracademy;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;";

    using var conn = new SqlConnection(connStr);
    conn.Open();

    using var cmd = new SqlCommand(result.Sql, conn);
    for (int i = 0; i < result.Bindings.Count; i++)
    {
        cmd.Parameters.AddWithValue($"@p{i}", result.Bindings[i]);
    }

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"[MS SQL] Student: {reader["studentnumber"]} - {reader["firstname"]}");
    }
}