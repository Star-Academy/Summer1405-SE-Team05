namespace CleanCode;

using System.Data.Common;

public sealed class DatabaseExecutor
{
    private readonly ICompiler _compiler;
    private readonly DbConnection _connection;

    public DatabaseExecutor(DbConnection connection, ICompiler compiler)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public List<T> Execute<T>(Query query, Func<DbDataReader, T> mapper)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(mapper);

        EnsureConnectionIsOpen();

        using var command = CreateCommand(query);
        using var reader = command.ExecuteReader();

        var results = new List<T>();
        while (reader.Read())
        {
            results.Add(mapper(reader));
        }

        return results;
    }

    public void PrintResults<T>(IEnumerable<T> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        Console.WriteLine("--- Database Results ---");
        foreach (var record in records)
        {
            Console.WriteLine(record);
        }
        Console.WriteLine(new string('=', 40));
    }

    private DbCommand CreateCommand(Query query)
    {
        var result = _compiler.Compile(query);
        PrintSql(result);

        var command = _connection.CreateCommand();
        command.CommandText = result.Sql;

        for (var i = 0; i < result.Bindings.Count; i++)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = _compiler.FormatParameterName(i);
            parameter.Value = result.Bindings[i] ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }

        return command;
    }

    private void EnsureConnectionIsOpen()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    private void PrintSql(SqlInput result)
    {
        Console.WriteLine($"Generated SQL: {result.Sql}");
        Console.WriteLine("Bindings: " + string.Join(", ", result.Bindings));
        Console.WriteLine(new string('-', 40));
    }
}