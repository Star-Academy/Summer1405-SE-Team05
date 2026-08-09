using System.Data.Common;

namespace CleanCode;

public interface IExecuter
{
    public List<T> Execute<T>(Query query, Func<DbDataReader, T> mapper);
    public void PrintResults<T>(IEnumerable<T> records);
    public DbCommand CreateCommand(Query query);
}