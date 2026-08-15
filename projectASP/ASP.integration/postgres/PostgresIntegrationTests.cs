using System.Net;
using System.Net.Http.Json;
using ASP.integration.fixtures;
using ASP.models;
using FluentAssertions;

namespace ASP.integration.fixtures;

[Collection("PostgresIntegrationTests")]
public class PostgresIntegrationTests : IClassFixture<PostgresContainerFixture>
{
    private readonly HttpClient _client;

    public PostgresIntegrationTests(PostgresContainerFixture fixture)
    {
        var factory = new PostgresWebApplicationFactory(fixture);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnStudents_FromPostgres()
    {
        // Arrange
        var testStudentNumber = "98100201";
        var request = new HttpRequestMessage(HttpMethod.Get, "/DataBase/students");
        request.Headers.Add("X-Database-Type", "Postgresql");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<student>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSpecific_ShouldReturnStudent_WhenStudentExistsInPostgres()
    {
        // Arrange
        var studentNumber = "99100305";
        var request = new HttpRequestMessage(HttpMethod.Get, $"/DataBase/students/{studentNumber}");
        request.Headers.Add("X-Database-Type", "Postgresql");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<student>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.firstname.Should().Be("علی");
    }

    [Fact]
    public async Task AddStudent_ShouldInsertSuccessfully_ToPostgres()
    {
        // Arrange
        var newStudent = new student
        {
            studentnumber = "00100412",
            firstname = "رضا",
            lastname = "محمدی",
            grade = 15.0f,
            ismale = true,
            leftunitscount = 5,
            dateofbirth = DateTime.UtcNow
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/DataBase/students")
        {
            Content = JsonContent.Create(newStudent)
        };
        request.Headers.Add("X-Database-Type", "Postgresql");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStudent_ShouldRemoveStudent_FromPostgres()
    {
        // Arrange
        // English Comment: Delete a student specifically added or seed item
        var studentNumberToDelete = "98100201";
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/DataBase/students/{studentNumberToDelete}");
        request.Headers.Add("X-Database-Type", "Postgresql");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }
}