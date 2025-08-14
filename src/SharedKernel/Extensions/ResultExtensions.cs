using SharedKernel.Domain;

namespace SharedKernel.Extensions;

public static class ResultExtensions
{
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value) 
            ? result 
            : Result.Failure<T>(error);
    }

    public static Result<K> Map<T, K>(this Result<T> result, Func<T, K> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(mapper(result.Value)) 
            : Result.Failure<K>(result.Error);
    }

    public static async Task<Result<K>> MapAsync<T, K>(this Result<T> result, Func<T, Task<K>> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(await mapper(result.Value)) 
            : Result.Failure<K>(result.Error);
    }

    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> onFailure)
    {
        if (result.IsFailure)
            onFailure(result.Error);
        return result;
    }

    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result.IsSuccess)
            onSuccess(result.Value);
        return result;
    }

    public static Result OnSuccess(this Result result, Action onSuccess)
    {
        if (result.IsSuccess)
            onSuccess();
        return result;
    }

    public static Result<K> Bind<T, K>(this Result<T> result, Func<T, Result<K>> binder)
    {
        return result.IsSuccess 
            ? binder(result.Value) 
            : Result.Failure<K>(result.Error);
    }

    public static async Task<Result<K>> BindAsync<T, K>(this Result<T> result, Func<T, Task<Result<K>>> binder)
    {
        return result.IsSuccess 
            ? await binder(result.Value) 
            : Result.Failure<K>(result.Error);
    }
}
