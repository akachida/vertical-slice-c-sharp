using FluentValidation.Results;
using SharedKernel.Domain;

namespace SharedKernel.Extensions;

public static class ValidationExtensions
{
    public static Result<T> ToResult<T>(this ValidationResult validationResult, T value)
    {
        return validationResult.IsValid
            ? Result.Success(value)
            : Result.Failure<T>(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
    }

    public static Result ToResult(this ValidationResult validationResult)
    {
        return validationResult.IsValid
            ? Result.Success()
            : Result.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
    }
}
