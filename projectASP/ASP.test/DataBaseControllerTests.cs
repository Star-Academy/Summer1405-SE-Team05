using ASP.controllers;
using ASP.models;
using ASP.services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Asp.test;

public class DataBaseControllerTests
{
    private readonly IDBService _dbService;
    private readonly DataBaseController _controller;

    public DataBaseControllerTests()
    {
        _dbService = Substitute.For<IDBService>();
        _controller = new DataBaseController(_dbService);
    }

    [Fact]
    public void GetAll_ShouldReturnSuccessApiResponse_WhenStudentsExist()
    {
        // Arrange
        var Students = new List<student>
        {
            new student { studentnumber = "40112345", firstname = "Ali", lastname = "Mohammadi", grade = 18.5f, ismale = true, leftunitscount = 20, dateofbirth = DateTime.Now },
            new student { studentnumber = "40112346", firstname = "Sara", lastname = "Ahmadi", grade = 19.0f, ismale = false, leftunitscount = 15, dateofbirth = DateTime.Now }
        };

        var expectedResponse = new ApiResponse<List<student>>(true, "Students retrieved successfully.", Students);

        _dbService.getAll().Returns(expectedResponse);

        // Act
        var result = _controller.GetAll();

        // Assert 
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].firstname.Should().Be("Ali");
        result.Data.Should().BeEquivalentTo(Students);
        
        _dbService.Received(1).getAll();
    }

    [Fact]
    public void Getspecific_ShouldReturnStudent_WhenStudentNumberExists()
    {
        // Arrange
        var studentNumber = "40112345";
        var mockStudent = new student { studentnumber = studentNumber, firstname = "Ali", lastname = "Mohammadi" };
        var expectedResponse = new ApiResponse<student>(true, "Student retrieved successfully.", mockStudent);

        _dbService.GetStudentByStudentNumber(studentNumber).Returns(expectedResponse);

        // Act
        var result = _controller.Getspecific(studentNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.firstname.Should().Be("Ali");

        _dbService.Received(1).GetStudentByStudentNumber(studentNumber);
    }

    [Fact]
    public void Add_ShouldReturnFailureResponse_WhenStudentAlreadyExists()
    {
        // Arrange
        var newStudent = new student { studentnumber = "40112345", firstname = "Ali", lastname = "Mohammadi" };
        var expectedResponse = new ApiResponse<string>(false, "already exists", null);

        _dbService.AddStudent(Arg.Any<student>()).Returns(expectedResponse);

        // Act
        var result = _controller.Add(newStudent);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("already exists");

        _dbService.Received(1).AddStudent(Arg.Any<student>());
    }

    [Fact]
    public void Delete_ShouldCallDeleteStudentOnService_WhenStudentNumberIsValid()
    {
        // Arrange
        var studentNumber = "40112345";
        var expectedResponse = new ApiResponse<string>(true, "Student deleted successfully.", "deleted successfully");

        _dbService.DeleteStudent(studentNumber).Returns(expectedResponse);

        // Act
        var result = _controller.Delete(studentNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("deleted successfully");

        _dbService.Received(1).DeleteStudent(studentNumber);
    }
    
    [Fact]
    public void Update_ShouldReturnResponseFromService_WhenCalled()
    {
        // Arrange
        var studentToUpdate = new student { studentnumber = "40112345", firstname = "AliUpdated" };
        var expectedResponse = new ApiResponse<string>(true, "Student updated successfully.", "updated successfully");

        _dbService.UpdateStudent(Arg.Any<student>()).Returns(expectedResponse);

        // Act
        var result = _controller.Update(studentToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Student updated successfully.");
        _dbService.Received(1).UpdateStudent(Arg.Any<student>());
    }

    [Fact]
    public void AddRandomStudent_ShouldReturnResponseFromService_WhenRequested()
    {
        // Arrange
        var count = 3;
        var mockStudents = new List<student> { new student { studentnumber = "1001" } };
        var expectedResponse = new ApiResponse<List<student>>(true, "3 random students generated successfully.", mockStudents);

        _dbService.GenerateRandomStudents(count).Returns(expectedResponse);

        // Act
        var result = _controller.AddRandomStudent(count);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        _dbService.Received(1).GenerateRandomStudents(count);
    }
}