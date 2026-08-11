using asp.services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace asp.test;

public class DbContextManagerTests
{
    private readonly IConfiguration _configMock;
    private readonly IHttpContextAccessor _httpContextAccessorMock;

    public DbContextManagerTests()
    {
        _configMock = Substitute.For<IConfiguration>();
        _httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();

        // English Comment: Mock connection strings in configuration
        _configMock.GetConnectionString("SqlServerConnection").Returns("Server=localhost;Database=TestDb;");
        _configMock.GetConnectionString("PostgresConnection").Returns("Host=localhost;Database=TestDb;");
    }

    [Fact]
    public void CurrentDatabase_ShouldReturnDefaultPostgresql_WhenHeaderIsMissing()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _httpContextAccessorMock.HttpContext.Returns(context);

        var manager = new DbContextManager(_configMock, _httpContextAccessorMock);

        // Act & Assert
        manager.CurrentDatabase.Should().Be("Postgresql");
    }

    [Fact]
    public void CurrentDatabase_ShouldReturnHeaderValue_WhenHeaderIsPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "SqlServer";
        _httpContextAccessorMock.HttpContext.Returns(context);

        var manager = new DbContextManager(_configMock, _httpContextAccessorMock);

        // Act & Assert
        manager.CurrentDatabase.Should().Be("SqlServer");
    }

    [Fact]
    public void GetQueryFactory_ShouldReturnQueryFactory_ForSqlServer()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "SqlServer";
        _httpContextAccessorMock.HttpContext.Returns(context);

        var manager = new DbContextManager(_configMock, _httpContextAccessorMock);

        // Act
        var factory = manager.GetQueryFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Compiler.GetType().Name.Should().Be("SqlServerCompiler");
    }

    [Fact]
    public void GetQueryFactory_ShouldReturnQueryFactory_ForPostgresql()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "Postgresql";
        _httpContextAccessorMock.HttpContext.Returns(context);

        var manager = new DbContextManager(_configMock, _httpContextAccessorMock);

        // Act
        var factory = manager.GetQueryFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Compiler.GetType().Name.Should().Be("PostgresCompiler");
    }

    [Fact]
    public void GetQueryFactory_ShouldThrowArgumentException_WhenDatabaseTypeIsInvalid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "MongoDB";
        _httpContextAccessorMock.HttpContext.Returns(context);

        var manager = new DbContextManager(_configMock, _httpContextAccessorMock);

        // Act
        Action act = () => manager.GetQueryFactory();

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Invalid database type: 'MongoDB'*");
    }
}