using System;
using System.ComponentModel;
using System.Linq;

public class PropertyFilter : ICustomTypeDescriptor
{
    private readonly object _wrappedObject;
    private readonly string _filter;

    public PropertyFilter(object obj, string filter){_wrappedObject = obj;_filter = filter?.ToLowerInvariant() ?? "";
    }

    public PropertyDescriptorCollection GetProperties(){
        var allProperties = TypeDescriptor.GetProperties(_wrappedObject, true);

        if (string.IsNullOrWhiteSpace(_filter))
            return allProperties;

        var filteredProperties = allProperties.Cast<PropertyDescriptor>().Where(p => p.DisplayName.ToLowerInvariant().Contains(_filter)).ToArray();

        return new PropertyDescriptorCollection(filteredProperties);
    }
    public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(_wrappedObject, true);
    public string GetClassName() => TypeDescriptor.GetClassName(_wrappedObject, true);
    public string GetComponentName() => TypeDescriptor.GetComponentName(_wrappedObject, true);
    public TypeConverter GetConverter() => TypeDescriptor.GetConverter(_wrappedObject, true);
    public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(_wrappedObject, true);
    public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(_wrappedObject, true);
    public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(_wrappedObject, editorBaseType, true);
    public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(_wrappedObject, true);
    public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(_wrappedObject, attributes, true);
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();
    public object GetPropertyOwner(PropertyDescriptor pd) => _wrappedObject;
}