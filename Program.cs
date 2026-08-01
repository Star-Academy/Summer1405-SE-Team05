using Microsoft.VisualBasic;

namespace ClassLibrary1;

public class Program{
public static void Main(){var query = new Query()
    .From("Students")
    .Select("Id", "Name")
    .Where("IsMale", true)
    .Where("Age", 20);
    PostgresCompiler p = new PostgresCompiler();
    var result = p.Compile(query);
    Console.WriteLine(result.Sql);
    foreach(var v in result.Bindings) 
    {
        Console.WriteLine(v);
    };

    SqlServerCompiler s = new SqlServerCompiler();
    var result2 = s.Compile(query);
    Console.WriteLine(result2.Sql);
    foreach(var v in result2.Bindings) 
    {
        Console.WriteLine(v);
    };

    }
    

 
    }