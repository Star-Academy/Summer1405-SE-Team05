using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ASP.services;

public class DbContextManager
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbContextManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string CurrentDatabase
    {
        get
        {
            var headerValue = _httpContextAccessor.HttpContext?.Request.Headers["X-Database-Type"].FirstOrDefault();
            return string.IsNullOrEmpty(headerValue) ? "Postgresql" : headerValue;
        }
    }

    public QueryFactory GetQueryFactory()
    {
        IDbConnection connection;
        Compiler compiler;

        if (CurrentDatabase.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            string connStr = _configuration.GetConnectionString("SqlServerConnection") 
                             ?? throw new InvalidOperationException("SqlServerConnection string is missing");
            connection = new SqlConnection(connStr);
            compiler = new SqlServerCompiler();
        }
        else if (CurrentDatabase.Equals("Postgresql", StringComparison.OrdinalIgnoreCase))
        {
            string connStr = _configuration.GetConnectionString("PostgresConnection") 
                             ?? throw new InvalidOperationException("PostgresConnection string is missing");
            connection = new NpgsqlConnection(connStr);
            compiler = new PostgresCompiler();
        }
        else
        {
            throw new ArgumentException($"Invalid database type: '{CurrentDatabase}'. Supported types are 'Postgresql' and 'SqlServer'.");
        }

        return new QueryFactory(connection, compiler);
    }
}