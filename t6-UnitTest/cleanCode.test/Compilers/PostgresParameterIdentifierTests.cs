using CleanCode;
using FluentAssertions;
using Xunit;

namespace cleanCode.test.Compilers;

public class PostgresParameterIdentifierTests
{
    private readonly PostgresParameterIdentifier _sut;

    public PostgresParameterIdentifierTests()
    {
        _sut = new PostgresParameterIdentifier();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(-1)]
    public void FormatParameterName_ShouldAlwaysReturnEmptyString_WhenIndexIsProvided(int index)
    {
        // Arrange && Act
        var result = _sut.FormatParameterName(index);

        // Assert
        result.Should().BeEmpty();
    }
}