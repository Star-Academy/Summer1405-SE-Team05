using System;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Builders;

public class SqlFromBuilderTests
{
    private readonly IParameterIdentifier _paramIdentifierMock;
    private readonly SqlFromBuilder _sut;

    public SqlFromBuilderTests()
    {
        _paramIdentifierMock = Substitute.For<IParameterIdentifier>();
        _sut = new SqlFromBuilder(_paramIdentifierMock);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenParamIdentifierIsNull()
    {
        // Act
        Action act = () => new SqlFromBuilder(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Build_ShouldThrowArgumentException_WhenTableNameIsNullOrEmptyOrWhiteSpace(string? tableName)
    {
        // Act
        Action act = () => _sut.Build(tableName);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}