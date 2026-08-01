using System.Text;

namespace ClassLibrary1;

public class Query
{
    private String table;
    private List<String> columns = new List<String>(); 
    private Dictionary<String , Object> values =  new Dictionary<String, Object>();

    public Query From (String table)
    {
        this.table = table;
        return this;
    }
    public Query Select(params String[] columns)
    {
        this.columns.AddRange(columns);
        return this;
    }
    public Query Where(String column, Object value)
    {
        values[column] = value;
        return this;
    }

    
}