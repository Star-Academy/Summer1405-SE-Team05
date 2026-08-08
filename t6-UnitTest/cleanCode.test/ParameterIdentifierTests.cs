using CleanCode;
using Xunit;

namespace cleanCode.test;

public class ParameterIdentifierTests
{
    [Fact]
    public void PostgresParameterIdentifier_FormatParameterName_Should_Return_Empty_String()
    {
        // Arrange
        var identifier = new PostgresParameterIdentifier();

        // Act
        var result = identifier.FormatParameterName(0);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void SqlServerParameterIdentifier_FormatParameterName_Should_Return_Formatted_Name()
    {
        // Arrange
        var identifier = new SqlServerParameterIdentifier();

        // Act
        var result = identifier.FormatParameterName(0);

        // Assert
        Assert.Equal("@p0", result);
    }
}