using System;

namespace SharedKernel.Domain.Entities
{
    public interface IEntityBase : IComparable
    {
        bool IsTransient();
    }
}
