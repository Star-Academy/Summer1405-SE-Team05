namespace CleanCode;

public interface ISelectBuilder
{
    string Build(List<string> columns, IParameterIdentifier paramIdentifier);
}
