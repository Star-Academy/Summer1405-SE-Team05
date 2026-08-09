using CleanCode;
using FluentAssertions;
using Xunit;

namespace cleanCode.test.Compilers;

public class SqlServerParameterIdentifierTests
{
    private readonly SqlServerParameterIdentifier _sut;

    public SqlServerParameterIdentifierTests()
    {
        _sut = new SqlServerParameterIdentifier();
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(10, "@p10")]
    [InlineData(-1, "@p-1")]
    public void FormatParameterName_ShouldReturnFormattedName_WhenIndexIsProvided(int index, string expected)
    {
        // Arrange && Act
        var result = _sut.FormatParameterName(index);

        // Assert
        result.Should().Be(expected);
    }
}