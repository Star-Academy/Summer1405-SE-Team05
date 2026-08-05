using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data.Common;
using CleanCode;

DotNetEnv.Env.Load();

ISelectBuilder selectBuilder = new SqlSelectBuilder();
IFromBuilder fromBuilder = new SqlFromBuilder();
IWhereBuilder whereBuilder = new SqlWhereBuilder();
var commonCompiler = new SqlCompilerCommon();


Console.WriteLine(">>> Executing Postgres Query...");

var pgConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
using var pgConnection = new NpgsqlConnection(pgConnectionString);

ICompiler pgCompiler = new PostgresCompiler(
    new PostgresParameterIdentifier(),
    selectBuilder,
    fromBuilder,
    whereBuilder,
    commonCompiler
);

var pgExecutor = new DatabaseExecutor(pgConnection, pgCompiler);

var pgQuery = new Query()
    .From("student")
    .Select("studentnumber", "firstname", "lastname", "grade")
    .Where("grade", ExpressionOperator.GreaterThanOrEqual, 16);

try
{
    var pgResults = pgExecutor.Execute(pgQuery, reader =>
        $"{reader.GetString(0)} | {reader.GetString(1)} {reader.GetString(2)} | Grade: {reader.GetFloat(3)}"
    );
    pgExecutor.PrintResults(pgResults);
}
catch (PostgresException ex)
{
    Console.WriteLine($"[Postgres Server Error Code {ex.SqlState}]: {ex.MessageText}");
    Console.WriteLine(new string('=', 40));
}
catch (NpgsqlException ex)
{
    Console.WriteLine($"[Postgres Driver/Conn Error]: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[Postgres Invalid Operation]: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}


Console.WriteLine("\n>>> Executing SQL Server Query...");

var sqlServerConnectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
using var sqlConnection = new SqlConnection(sqlServerConnectionString);

ICompiler sqlCompiler = new SqlServerCompiler(
    new SqlServerParameterIdentifier(),
    selectBuilder,
    fromBuilder,
    whereBuilder,
    commonCompiler
);

var sqlExecutor = new DatabaseExecutor(sqlConnection, sqlCompiler);

var sqlServerQuery = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName", "LastName")
    .Where("IsMale", ExpressionOperator.Equals, true);

try
{
    var sqlResults = sqlExecutor.Execute(sqlServerQuery, reader =>
        $"{reader.GetString(0)} | {reader.GetString(1)} {reader.GetString(2)}"
    );

    sqlExecutor.PrintResults(sqlResults);
}
catch (SqlException ex)
{
    Console.WriteLine($"[SQL Server Error #{ex.Number}]: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}
catch (DbException ex)
{
    Console.WriteLine($"[Database Exception]: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[SQL Server Invalid Operation]: {ex.Message}");
    Console.WriteLine(new string('=', 40));
}