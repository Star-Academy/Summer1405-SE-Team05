using ASP.helpers;
using FluentAssertions;
using Xunit;

namespace Asp.test;

public class StudentGeneratorTests
{
    [Fact]
    public void GenerateRandomStudent_ShouldReturnValidStudentObject()
    {
        // Act
        var student = StudentGenerator.GenerateRandomStudent();

        // Assert
        student.Should().NotBeNull();
        student.studentnumber.Should().NotBeNullOrEmpty();
        student.firstname.Should().NotBeNullOrEmpty();
        student.lastname.Should().NotBeNullOrEmpty();
        student.grade.Should().BeInRange(0, 20);
        student.leftunitscount.Should().BeGreaterThanOrEqualTo(0);
    }
}