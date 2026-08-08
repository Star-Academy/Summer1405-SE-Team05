using System;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Builders;
public class SqlBuilderAndOperatorTests
{
    [Fact]
    public void SqlFromBuilder_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
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
    public void SqlFromBuilder_Build_NullOrWhiteSpaceTable_ThrowsArgumentException(string? tableName)
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
    public void SqlExpressionOperator_GetSymbol_InvalidOperatorType_ThrowsArgumentOutOfRangeException()
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