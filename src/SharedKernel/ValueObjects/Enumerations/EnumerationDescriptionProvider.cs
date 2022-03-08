using System.ComponentModel;

namespace SharedKernel.ValueObjects.Enumerations;

public class EnumerationDescriptionProvider : TypeDescriptionProvider
{
    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object? instance) =>
        new EnumerationDescriptor(objectType);
}
