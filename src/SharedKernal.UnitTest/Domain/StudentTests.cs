using Domain.Students;
using Domain.Users;
using FluentAssertions;
using SharedKernel.Domain;
using SharedKernel.Domain.Enumerations.Users;
using SharedKernal.UnitTest.Fixtures;
using Xunit;

namespace SharedKernal.UnitTest.Domain;

public class StudentTests
{
    [Fact]
    public void Create() 
    {
        // Arrange
        var user = UserFixture.AStudentUser();

        // Act
        var result = Student.Create(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.User.Should().Be(user);
        result.Value.Grade.Should().Be(0);
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldThrow_When_UserIsNull() 
    {
        // Act
        var result = Student.Create(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User cannot be null");
    }

    [Fact]
    public void UpdateGrade() 
    {
        // Arrange
        var student = StudentFixture.AStudentWithZeroGrade();
        var newGrade = 85.5;

        // Act
        var result = student.UpdateGrade(newGrade);

        // Assert
        result.IsSuccess.Should().BeTrue();
        student.Grade.Should().Be(newGrade);
    }

    [Fact]
    public void UpdateGrade_ShouldThrow_When_GradeIsNegative() 
    {
        // Arrange
        var student = StudentFixture.AStudentWithZeroGrade();
        var invalidGrade = -10;

        // Act
        var result = student.UpdateGrade(invalidGrade);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Grade must be between 0 and 100");
        student.Grade.Should().Be(0); 
    }

    [Fact]
    public void UpdateGrade_ShouldThrow_When_GradeIsOver100() 
    {
        // Arrange
        var student = StudentFixture.AStudentWithZeroGrade();
        var invalidGrade = 150;

        // Act
        var result = student.UpdateGrade(invalidGrade);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Grade must be between 0 and 100");
        student.Grade.Should().Be(0); 
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void UpdateGrade_WithValidBoundaryValues(double grade) 
    {
        // Arrange
        var student = StudentFixture.AStudentWithZeroGrade();

        // Act
        var result = student.UpdateGrade(grade);

        // Assert
        result.IsSuccess.Should().BeTrue();
        student.Grade.Should().Be(grade);
    }
}
