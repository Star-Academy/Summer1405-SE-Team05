namespace CleanCode;

public interface ICompiler
{
    DataBaseInput Compile(Query query);
    string FormatParameterName(int index);
}