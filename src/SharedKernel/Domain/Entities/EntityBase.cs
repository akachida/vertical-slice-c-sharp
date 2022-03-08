using System.Diagnostics.CodeAnalysis;
using MediatR;
using SharedKernel.Helpers;

namespace SharedKernel.Domain.Entities;

public abstract class EntityBase : EntityBase<Guid>
{
}

public abstract class EntityBase<TId> : IEntityBase, IEquatable<EntityBase<TId>>
    where TId : IComparable
{
    public TId Id { get; protected set;  }

    public bool IsTransient() => Id.IsDefault();

    #region Domain events

    private List<INotification> _domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

    protected void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    protected void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    #endregion Domain events

    public void FakeId(TId id)
    {
        Id = id;
    }

    public bool Equals([AllowNull] EntityBase<TId> other)
    {
        if (other == null) return false;
        if (other.IsTransient() || IsTransient()) return false;
        return other.Id.Equals(Id);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (!(obj is EntityBase)) return false;
        if (GetType() != obj.GetType()) return false;
        return Equals((EntityBase<TId>) obj);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode() ^ 31;
        }

        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    public static bool operator ==(EntityBase<TId> a, EntityBase<TId> b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }

    public static bool operator !=(EntityBase<TId> left, EntityBase<TId> right) =>
        !(left == right);

    #region Comparison methods

    public int CompareTo(object obj) =>
        Id.CompareTo(((EntityBase) obj).Id);

    public static bool operator <(EntityBase<TId> left, EntityBase<TId> right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(EntityBase<TId> left, EntityBase<TId> right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(EntityBase<TId> left, EntityBase<TId> right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(EntityBase<TId> left, EntityBase<TId> right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    #endregion Comparison methods
}
