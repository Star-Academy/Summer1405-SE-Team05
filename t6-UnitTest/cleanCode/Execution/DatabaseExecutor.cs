using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CleanCode;

[ExcludeFromCodeCoverage]
public sealed class DatabaseExecutor : IExecuter
{
    private readonly ICompiler _compiler;
    private readonly DbConnection _dbConnection;

    public DatabaseExecutor(DbConnection dbConnection, ICompiler compiler)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public List<T> Execute<T>(Query query, Func<DbDataReader, T> mapper)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(mapper);

        if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

        using var command = CreateCommand(query);
        using var reader = command.ExecuteReader();

        var results = new List<T>();
        while (reader.Read()) results.Add(mapper(reader));

        return results;
    }

    public void PrintResults<T>(IEnumerable<T> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        Console.WriteLine("--- Database Results ---");
        foreach (var record in records) Console.WriteLine(record);
        Console.WriteLine(new string('=', 40));
    }

    public DbCommand CreateCommand(Query query)
    {
        var result = _compiler.Compile(query);
        PrintSql(result);

        var command = _dbConnection.CreateCommand();
        command.CommandText = result.QueryString;

        for (var i = 0; i < result.Bindings.Count; i++)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = _compiler.FormatParameterName(i);
            parameter.Value = result.Bindings[i] ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }

        return command;
    }


    private void PrintSql(DataBaseInput result)
    {
        Console.WriteLine($"Generated SQL: {result.QueryString}");
        Console.WriteLine("Bindings: " + string.Join(", ", result.Bindings));
        Console.WriteLine(new string('-', 40));
    }
}