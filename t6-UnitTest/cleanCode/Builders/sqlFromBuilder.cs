namespace CleanCode;

internal sealed class SqlFromBuilder : IFromBuilder
{
    private readonly IParameterIdentifier _paramIdentifier;

    public SqlFromBuilder(IParameterIdentifier paramIdentifier)
    {
        _paramIdentifier = paramIdentifier ?? throw new ArgumentNullException(nameof(paramIdentifier));
    }

    public string Build(string? table)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name cannot be null or empty.");

        return $"FROM {_paramIdentifier.WrapIdentifier(table)}";
    }
}