using System;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Builders;

public class SqlBuilderAndOperatorTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenParamIdentifierIsNull()
    {
        // Arrange
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
        // Arrange
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var sut = new SqlFromBuilder(mockParamIdentifier);

        // Act
        Action act = () => sut.Build(tableName);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetSymbol_ShouldThrowArgumentOutOfRangeException_WhenOperatorTypeIsInvalid()
    {
        // Arrange
        var sut = new SqlExpressionOperator();
        var invalidType = (ExpressionOperatorType)999;

        // Act
        Action act = () => sut.GetSymbol(invalidType);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}