namespace CleanCode;

public interface IFromBuilder
{
    string Build(string? table, IParameterIdentifier paramIdentifier);
}
