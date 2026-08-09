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

    [Fact]
    public void FormatParameterName_ShouldReturnFormattedName_WhenIndexIsZero()
    {
        // Act
        var result = _sut.FormatParameterName(0);

        // Assert
        result.Should().Be("@p0");
    }
}