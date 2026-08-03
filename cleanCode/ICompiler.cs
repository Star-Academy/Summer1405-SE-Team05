namespace t4;

public interface ICompiler
{
    SqlResult Compile(Query query);
    string FormatParameter(int index);   
}