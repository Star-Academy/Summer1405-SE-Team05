using asp.controllers;
using asp.models;
using asp.services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace asp.test;

public class DataBaseControllerTests
{
    private readonly IDBService _dbServiceMock;
    private readonly DataBaseController _controller;

    public DataBaseControllerTests()
    {
        // English Comment: Create a substitute for IDBService
        _dbServiceMock = Substitute.For<IDBService>();
        _controller = new DataBaseController(_dbServiceMock);
    }

    [Fact]
    public void GetAll_ShouldReturnSuccessApiResponse_WhenStudentsExist()
    {
        // Arrange
        var mockStudents = new List<student>
        {
            new student { studentnumber = "40112345", firstname = "Ali", lastname = "Mohammadi", grade = 18.5f, ismale = true, leftunitscount = 20, dateofbirth = DateTime.Now },
            new student { studentnumber = "40112346", firstname = "Sara", lastname = "Ahmadi", grade = 19.0f, ismale = false, leftunitscount = 15, dateofbirth = DateTime.Now }
        };

        var expectedResponse = new ApiResponse<List<student>>(true, "Students retrieved successfully.", mockStudents);

        _dbServiceMock.getAll().Returns(expectedResponse);

        // Act
        var result = _controller.GetAll();

        // Assert (FluentAssertions Syntax)
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].firstname.Should().Be("Ali");
        result.Data.Should().BeEquivalentTo(mockStudents);
        
        _dbServiceMock.Received(1).getAll();
    }

    [Fact]
    public void Getspecific_ShouldReturnStudent_WhenStudentNumberExists()
    {
        // Arrange
        var studentNumber = "40112345";
        var mockStudent = new student { studentnumber = studentNumber, firstname = "Ali", lastname = "Mohammadi" };
        var expectedResponse = new ApiResponse<student>(true, "Student retrieved successfully.", mockStudent);

        _dbServiceMock.getStudentByStudentNumber(studentNumber).Returns(expectedResponse);

        // Act
        var result = _controller.Getspecific(studentNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.firstname.Should().Be("Ali");

        _dbServiceMock.Received(1).getStudentByStudentNumber(studentNumber);
    }

    [Fact]
    public void Add_ShouldReturnFailureResponse_WhenStudentAlreadyExists()
    {
        // Arrange
        var newStudent = new student { studentnumber = "40112345", firstname = "Ali", lastname = "Mohammadi" };
        var expectedResponse = new ApiResponse<string>(false, "already exists", null);

        _dbServiceMock.addStudent(Arg.Any<student>()).Returns(expectedResponse);

        // Act
        var result = _controller.Add(newStudent);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("already exists");

        _dbServiceMock.Received(1).addStudent(Arg.Any<student>());
    }

    [Fact]
    public void Delete_ShouldCallDeleteStudentOnService_WithCorrectStudentNumber()
    {
        // Arrange
        var studentNumber = "40112345";
        var expectedResponse = new ApiResponse<string>(true, "Student deleted successfully.", "deleted successfully");

        _dbServiceMock.deleteStudent(studentNumber).Returns(expectedResponse);

        // Act
        var result = _controller.Delete(studentNumber);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("deleted successfully");

        _dbServiceMock.Received(1).deleteStudent(studentNumber);
    }
    
    [Fact]
    public void Update_ShouldReturnResponseFromService()
    {
        // Arrange
        var studentToUpdate = new student { studentnumber = "40112345", firstname = "AliUpdated" };
        var expectedResponse = new ApiResponse<string>(true, "Student updated successfully.", "updated successfully");

        _dbServiceMock.updateStudent(Arg.Any<student>()).Returns(expectedResponse);

        // Act
        var result = _controller.Update(studentToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Student updated successfully.");
        _dbServiceMock.Received(1).updateStudent(Arg.Any<student>());
    }

    [Fact]
    public void AddRandomStudent_ShouldReturnResponseFromService()
    {
        // Arrange
        var count = 3;
        var mockStudents = new List<student> { new student { studentnumber = "1001" } };
        var expectedResponse = new ApiResponse<List<student>>(true, "3 random students generated successfully.", mockStudents);

        _dbServiceMock.generateRandomStudents(count).Returns(expectedResponse);

        // Act
        var result = _controller.AddRandomStudent(count);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        _dbServiceMock.Received(1).generateRandomStudents(count);
    }
}