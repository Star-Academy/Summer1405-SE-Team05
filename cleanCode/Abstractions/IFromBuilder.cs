namespace CleanCode;

internal interface IFromBuilder
{
    string Build(string? table, IParameterIdentifier paramIdentifier);
}
