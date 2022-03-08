namespace SharedKernel.Helpers;

public static class TypeHelper
{
    public static bool IsDefault<T>(this T val) =>
        EqualityComparer<T>.Default.Equals(val, default);
}
