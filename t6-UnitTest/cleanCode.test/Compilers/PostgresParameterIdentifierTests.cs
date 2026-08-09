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

    [Fact]
    public void FormatParameterName_ShouldReturnEmptyString_WhenIndexIsZero()
    {
        // Act
        var result = _sut.FormatParameterName(0);

        // Assert
        result.Should().BeEmpty();
    }
}

