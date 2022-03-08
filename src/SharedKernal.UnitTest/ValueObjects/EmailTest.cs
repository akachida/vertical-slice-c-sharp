using System;
using FluentAssertions;
using SharedKernel.ValueObjects;
using Xunit;

namespace SharedKernal.UnitTest.ValueObjects;

public class EmailTest
{
    [Fact]
    public void Create_email_with_success()
    {
        // arrange
        var validEmail = "admin@snowmanlabs.com";

        // act
        var email = new Email(validEmail);

        // assert
        email.Should().BeAssignableTo<Email>();

        email.Value.Should().Be(validEmail);
    }

    [Fact]
    public void Validate_value_not_null()
    {
        // arrange
        var validEmail = string.Empty;

        // act & assert
        FluentActions.Invoking(() => new Email(validEmail))
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(
                "Value cannot be null. (Parameter 'value')");
    }

    [Fact]
    public void Validate_value_greater_than_50()
    {
        // arrange
        var validEmail = "asdfghjklqwertyuiopzxcvbnmasdfghjklqwertyuiopzxcvbnm@email.com";

        // act & assert
        FluentActions.Invoking(() => new Email(validEmail))
            .Should()
            .Throw<Exception>()
            .WithMessage(
                "Email should not have more than 50 characters");
    }

    [Fact]
    public void Throw_exception_if_its_an_invalid_email_address()
    {
        // arrange
        var validEmail = "email_wtih_,@email.com";

        // act & assert
        FluentActions.Invoking(() => new Email(validEmail))
            .Should()
            .Throw<Exception>()
            .WithMessage(
                "Email is not in a valida format");
    }
}
