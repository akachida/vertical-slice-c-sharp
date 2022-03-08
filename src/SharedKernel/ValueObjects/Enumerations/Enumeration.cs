using System.ComponentModel;
using System.Reflection;
using SharedKernel.ValueObjects.Enumerations;

namespace SharedKernel.ValueObjects.Enumerations;

public abstract class Enumeration<TEnumeration> : Enumeration<int, TEnumeration>
    where TEnumeration : Enumeration<int, TEnumeration>
{
    protected Enumeration(int id, string name)
        : base(id, name)
    {
    }
}

[TypeDescriptionProvider(typeof(EnumerationDescriptionProvider))]
public abstract class Enumeration<TId, TEnumeration> : ValueObject, IComparable<Enumeration<TId, TEnumeration>>
    where TId : IComparable<TId>, IEquatable<TId>
    where TEnumeration : Enumeration<TId, TEnumeration>
{
    private static readonly Lazy<List<TEnumeration>> GetAllLazy =
        new Lazy<List<TEnumeration>>(
            () => GetAllInternal().ToList(),
            LazyThreadSafetyMode.ExecutionAndPublication);

    public TId Id { get; private set; }

    public string Name { get; private set; }

    protected Enumeration(TId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() =>
        Name;

    public static List<TEnumeration> GetAll() =>
        GetAllLazy.Value;

    private static IEnumerable<TEnumeration> GetAllInternal()
    {
        var fields = typeof(TEnumeration)
                .GetTypeInfo()
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            if (info.GetValue(null) is TEnumeration locatedValue)
            {
                yield return locatedValue;
            }
        }
    }

    private static bool EqualsValue(TId value, TId otherValue)
    {
        var valueStr = value as string;

        return valueStr?.Equals(otherValue?.ToString(), StringComparison.InvariantCultureIgnoreCase) ??
               value?.Equals(otherValue) == true;
    }

    public static TEnumeration FromValue(TId id) =>
        Parse(id, nameof(id),
            i => EqualsValue(i.Id, id));

    public static bool TryFromValue(TId id, out TEnumeration matchingItem) =>
        TryParse(id, nameof(id),
            i => EqualsValue(i.Id, id),
            out matchingItem);

    public static TEnumeration FromName(string name) =>
        Parse(name, nameof(name),
            i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

    public static bool TryFromName(string name, out TEnumeration item) =>
        TryParse(name, nameof(name),
            i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase),
            out item);

    protected static TEnumeration Parse<TK>(TK value, string description,
        Func<TEnumeration, bool> predicate)
    {
        if (TryParse(value, description,
            predicate, out var item))
            return item;

        var message = $"'{value}' is not a valid {description} in {typeof(TEnumeration)}";

        throw new ApplicationException(message);
    }

    protected static bool TryParse<TK>(TK value, string description,
        Func<TEnumeration, bool> predicate, out TEnumeration item)
    {
        item = GetAll().FirstOrDefault(predicate);

        return item != null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        var idStr = Id as string;

        if (idStr != null)
            yield return idStr.ToLowerInvariant();
        else
            yield return Id;
    }

    #region Comparison methods

    public int CompareTo(Enumeration<TId, TEnumeration> other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Id.CompareTo(other.Id);
    }

    public static bool operator <(Enumeration<TId, TEnumeration> left, Enumeration<TId, TEnumeration> right)
    {
        return Comparer<Enumeration<TId, TEnumeration>>.Default.Compare(left, right) < 0;
    }

    public static bool operator >(Enumeration<TId, TEnumeration> left, Enumeration<TId, TEnumeration> right)
    {
        return Comparer<Enumeration<TId, TEnumeration>>.Default.Compare(left, right) > 0;
    }

    public static bool operator <=(Enumeration<TId, TEnumeration> left, Enumeration<TId, TEnumeration> right)
    {
        return Comparer<Enumeration<TId, TEnumeration>>.Default.Compare(left, right) <= 0;
    }

    public static bool operator >=(Enumeration<TId, TEnumeration> left, Enumeration<TId, TEnumeration> right)
    {
        return Comparer<Enumeration<TId, TEnumeration>>.Default.Compare(left, right) >= 0;
    }

    #endregion Comparison methods
}
