using CleanCode;
using FluentAssertions;
using Xunit;

namespace cleanCode.test.Compilers;
public class ParameterIdentifierTests
{
    [Fact]
    public void PostgresParameterIdentifier_FormatParameterName_Should_Return_Empty_String()
    {
        // Arrange
        var sut = new PostgresParameterIdentifier();

        // Act
        var result = sut.FormatParameterName(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SqlServerParameterIdentifier_FormatParameterName_Should_Return_Formatted_Name()
    {
        // Arrange
        var sut = new SqlServerParameterIdentifier();

        // Act
        var result = sut.FormatParameterName(0);

        // Assert
        result.Should().Be("@p0");
    }
}