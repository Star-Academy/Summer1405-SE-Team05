namespace CleanCode;

internal interface ISelectBuilder
{
    string Build(List<string> columns, IParameterIdentifier paramIdentifier);
}
