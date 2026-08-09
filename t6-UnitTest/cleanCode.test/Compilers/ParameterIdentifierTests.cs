using CleanCode;
using FluentAssertions;
using Xunit;

namespace cleanCode.test.Compilers;

public class ParameterIdentifierTests
{
    [Fact]
    public void FormatParameterName_ShouldReturnEmptyString_WhenCalledOnPostgresParameterIdentifier()
    {
        // Arrange
        var sut = new PostgresParameterIdentifier();

        // Act
        var result = sut.FormatParameterName(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FormatParameterName_ShouldReturnFormattedName_WhenCalledOnSqlServerParameterIdentifier()
    {
        // Arrange
        var sut = new SqlServerParameterIdentifier();

        // Act
        var result = sut.FormatParameterName(0);

        // Assert
        result.Should().Be("@p0");
    }
}