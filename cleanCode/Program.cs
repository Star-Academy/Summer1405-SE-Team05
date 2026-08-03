using Npgsql;
using Microsoft.Data.SqlClient;
using t4;

// ==========================================
// ۱. تست PostgreSQL
// ==========================================
Console.WriteLine(">>> Executing Postgres Query...");

string pgConnectionString = "Host=localhost;Port=5432;Database=staracademy;Username=postgres;Password=postgres";
using var pgConnection = new NpgsqlConnection(pgConnectionString);

var pgCompiler = new PostgresCompiler();
var pgExecutor = new DatabaseExecutor(pgConnection, pgCompiler);

var pgQuery = new Query()
    .From("student")
    .Select("studentnumber", "firstname", "lastname", "grade")
    .Where("grade", ExpressionOperator.GreaterThanOrEqual, 16);

try
{
    pgExecutor.ExecuteAndPrint(pgQuery, reader => 
        $"{reader.GetString(0)} | {reader.GetString(1)} {reader.GetString(2)} | Grade: {reader.GetFloat(3)}"
    );
}
catch (Exception ex)
{
    Console.WriteLine($"Postgres Error: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}


// ==========================================
// ۲. تست SQL Server
// ==========================================
Console.WriteLine("\n>>> Executing SQL Server Query...");

string sqlServerConnectionString = "Server=localhost,1433;Database=StarAcademy;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;";
using var sqlConnection = new SqlConnection(sqlServerConnectionString);

var sqlCompiler = new SqlServerCompiler();
var sqlExecutor = new DatabaseExecutor(sqlConnection, sqlCompiler);

var sqlServerQuery = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName", "LastName")
    .Where("IsMale", ExpressionOperator.Equals, true);

try
{
    sqlExecutor.ExecuteAndPrint(sqlServerQuery, reader => 
        $"{reader.GetString(0)} | {reader.GetString(1)} {reader.GetString(2)}"
    );
}
catch (Exception ex)
{
    Console.WriteLine($"SQL Server Error: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}