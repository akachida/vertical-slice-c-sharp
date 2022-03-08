using System.ComponentModel;
using SharedKernel.ValueObjects.Enumerations;

namespace SharedKernel.ValueObjects.Enumerations;


public class EnumerationDescriptor : CustomTypeDescriptor
{
    private readonly Type _objectType;

    public EnumerationDescriptor(Type objectType)
    {
        _objectType = objectType;
    }

    public override TypeConverter GetConverter()
    {
        var baseType = _objectType.BaseType;

        if (baseType == null)
            return base.GetConverter();

        while (baseType.BaseType != null && baseType.GetGenericTypeDefinition() != typeof(Enumeration<,>))
            baseType = baseType.BaseType;

        if (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(Enumeration<,>))
            return base.GetConverter();

        var genericArg = baseType.GenericTypeArguments[0];
        var converterType = typeof(EnumerationConverter<,>).MakeGenericType(_objectType, genericArg);

        return (TypeConverter)Activator.CreateInstance(converterType);
    }
}
