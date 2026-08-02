using Npgsql;
using t4;

TestPostgres();

// TestSqlServer();

void TestPostgres()
{
    string connectionString = "Host=localhost;Port=5432;Database=staracademy;Username=postgres;Password=postgres";

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var compiler = new PostgresCompiler();

    var query = new Query()
        .From("student")
        .Select("studentnumber", "firstname", "lastname", "grade")
        .Where("grade", ExpressionOperator.GreaterThanOrEqual, 16);

    SqlResult result = compiler.Compile(query);
    PrintResult(result);

    using var command = new NpgsqlCommand(result.Sql, connection);

    foreach (var binding in result.Bindings)
    {
        command.Parameters.AddWithValue(binding);
    }

    using var reader = command.ExecuteReader();
    Console.WriteLine("--- Database Results ---");
    while (reader.Read())
    {
        string studentNumber = reader.GetString(0);
        string firstName = reader.GetString(1);
        string lastName = reader.GetString(2);
        float grade = reader.GetFloat(3);
        Console.WriteLine($"{studentNumber} | {firstName} {lastName} | Grade: {grade}");
    }
    Console.WriteLine(new string('=', 40));
}

void TestSqlServer()
{
    var compiler = new SqlServerCompiler();

    var query = new Query()
        .From("student")
        .Select("studentnumber", "firstname", "lastname")
        .Where("ismale", ExpressionOperator.Equals, true); 

    SqlResult result = compiler.Compile(query);
    PrintResult(result);
}

void PrintResult(SqlResult result)
{
    Console.WriteLine($"Generated SQL: {result.Sql}");
    Console.WriteLine("Bindings: " + string.Join(", ", result.Bindings));
    Console.WriteLine(new string('-', 40));
}