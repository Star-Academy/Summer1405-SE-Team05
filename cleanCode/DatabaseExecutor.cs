namespace t4;

using System.Data.Common;
using Npgsql;

public class DatabaseExecutor
{
    private readonly DbConnection _connection;
    private readonly ICompiler _compiler;

    public DatabaseExecutor(DbConnection connection, ICompiler compiler)
    {
        _connection = connection;
        _compiler = compiler;
    }

    public void ExecuteAndPrint<T>(Query query, Func<DbDataReader, T> mapper)
    {
        _connection.Open();

        SqlResult result = _compiler.Compile(query);
        PrintResult(result);

        using var command = _connection.CreateCommand();
        command.CommandText = result.Sql;

        for (int i = 0; i < result.Bindings.Count; i++)
        {
            var parameter = command.CreateParameter();
            object value = result.Bindings[i] ?? DBNull.Value;

    
            if (_connection is NpgsqlConnection)
            {
                parameter.Value = value; 
            }
            else
            {
                parameter.ParameterName = _compiler.FormatParameter(i);
                parameter.Value = value;
            }
            
            command.Parameters.Add(parameter);
        }

        using var reader = command.ExecuteReader();
        Console.WriteLine("--- Database Results ---");
        while (reader.Read())
        {
            T record = mapper(reader);
            Console.WriteLine(record);
        }
        Console.WriteLine(new string('=', 40));
    }

    private void PrintResult(SqlResult result)
    {
        Console.WriteLine($"Generated SQL: {result.Sql}");
        Console.WriteLine("Bindings: " + string.Join(", ", result.Bindings));
        Console.WriteLine(new string('-', 40));
    }
}