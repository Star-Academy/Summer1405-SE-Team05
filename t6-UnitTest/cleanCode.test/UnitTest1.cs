using CleanCode;
using NSubstitute;

namespace cleanCode.test;

public class UnitTest1
{
    [Fact]
    public void PostgresCompiler_Should_Generate_Correct_SqlInput()
    {
        // Arrange
        var expressionOperator = new SqlExpressionOperator();
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var compiler = new PostgresCompiler(
            postgresParameterIdentifier,
            commonCompiler
        );

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
    }


    [Fact]
    public void PostgresCompiler_Should_Delegate_Compilation_To_CommonCompiler()
    {
        // Arrange
        var substituteParamIdentifier = Substitute.For<IParameterIdentifier>();
        var substituteCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var expectedResult = new DataBaseInput("SELECT * FROM \"student\"", new List<object>());
        var query = new Query().From("student");

        substituteCommonCompiler
            .Compile(query)
            .Returns(expectedResult);

        var compiler = new PostgresCompiler(
            substituteParamIdentifier,
            substituteCommonCompiler
        );

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal(expectedResult.QueryString, result.QueryString);

        substituteCommonCompiler
            .Received(1)
            .Compile(query);
    }


    [Theory]
    [ClassData(typeof(PostgresTestData))]
    public void PostgresCompiler_Should_Generate_Expected_Sql_Using_ClassData(
        Query query,
        string expectedSql,
        object[] expectedBindings)
    {
        // Arrange
        var expressionOperator = new SqlExpressionOperator();
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var compiler = new PostgresCompiler(
            postgresParameterIdentifier,
            commonCompiler);

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal(expectedSql, result.QueryString);
        Assert.Equal(expectedBindings, result.Bindings);
    }
}