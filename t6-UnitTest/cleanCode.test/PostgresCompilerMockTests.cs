using CleanCode;
using NSubstitute;

namespace cleanCode.test;

public class PostgresCompilerMockTests
{
    [Fact]
    public void PostgresCompiler_Should_Use_Mocked_ExpressionOperator()
    {
        // Arrange
        var pgIdentifier = new PostgresParameterIdentifier();

        var mockOperator = Substitute.For<IExpressionOperator>();
        mockOperator.GetSymbol(ExpressionOperatorType.GreaterThanOrEqual).Returns(">=");
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, mockOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var compiler = new PostgresCompiler(pgIdentifier, commonCompiler);

        var query = new Query()
            .From("student")
            .Select("studentnumber", "firstname")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16);

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal("SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1",
            result.QueryString);
        Assert.Single(result.Bindings);
        Assert.Equal(16, result.Bindings[0]);

        // بررسی فراخوانی متد ماک
        mockOperator.Received(1).GetSymbol(ExpressionOperatorType.GreaterThanOrEqual);
    }
}