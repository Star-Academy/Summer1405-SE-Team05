namespace CleanCode;

public interface ICompiler
{
    SqlInput Compile(Query query );
    string FormatParameterName(int index);
    
    }
