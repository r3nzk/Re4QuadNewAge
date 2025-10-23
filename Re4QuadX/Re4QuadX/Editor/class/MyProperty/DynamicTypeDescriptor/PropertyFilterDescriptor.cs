using System;
using System.ComponentModel;
using System.Linq;

public class PropertyFilterDescriptor : ICustomTypeDescriptor
{
    public object OriginalObject { get; }

    public PropertyFilterDescriptor(object originalObject){
        this.OriginalObject = originalObject ?? throw new ArgumentNullException(nameof(originalObject));
    }

    public PropertyDescriptorCollection GetProperties(){
        var allProperties = TypeDescriptor.GetProperties(this.OriginalObject, true);
        var filteredProperties = allProperties.Cast<PropertyDescriptor>()
            .Where(pd => pd.DisplayName.IndexOf("Unknown", StringComparison.OrdinalIgnoreCase) < 0) //remove unknown properties
            .ToArray();

        return new PropertyDescriptorCollection(filteredProperties);
    }

    public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this.OriginalObject, true);
    public string GetClassName() => TypeDescriptor.GetClassName(this.OriginalObject, true);
    public string GetComponentName() => TypeDescriptor.GetComponentName(this.OriginalObject, true);
    public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this.OriginalObject, true);
    public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this.OriginalObject, true);
    public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this.OriginalObject, true);
    public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this.OriginalObject, editorBaseType, true);
    public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this.OriginalObject, true);
    public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this.OriginalObject, attributes, true);
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();
    public object GetPropertyOwner(PropertyDescriptor pd) => this.OriginalObject;
}
