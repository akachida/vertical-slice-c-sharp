using FluentAssertions;
using SharedKernel.Domain;
using Xunit;

namespace SharedKernal.UnitTest.Domain;

public class ResultTests
{
    [Fact]
    public void Success_Should_Create_Successful_Result()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_Should_Create_Failed_Result()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Success_Generic_Should_Create_Successful_Result_With_Value()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_Generic_Should_Create_Failed_Result_Without_Value()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure<string>(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void ImplicitConversion_Should_Create_Successful_Result()
    {
        // Arrange
        var value = "test value";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Success_Result_Cannot_Have_Error_Message()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_Result_Must_Have_Error_Message()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.Error.Should().Be(errorMessage);
        result.Error.Should().NotBeEmpty();
    }
}
