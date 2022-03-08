using System.ComponentModel;
using System.Globalization;

namespace SharedKernel.ValueObjects.Enumerations;

public class EnumerationConverter<TEnumeration, TId> : TypeConverter
    where TEnumeration : Enumeration<TId, TEnumeration>
    where TId : IComparable<TId>, IEquatable<TId>
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return false;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(TId) || sourceType == typeof(string))
            return true;

        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is TId)
        {
            var id = (TId)Convert.ChangeType(value, typeof(TId));

            return Enumeration<TId, TEnumeration>.FromValue(id);
        }

        return base.ConvertFrom(context, culture, value);
    }
}
