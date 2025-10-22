using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

//this converter tells the PropertyGrid how to handle a Vector3 struct.
public class Vector3sConverter : TypeConverter
{
    public override bool GetPropertiesSupported(ITypeDescriptorContext context){
        return true; //expandable properties + in the grid.
    }

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes){
        return TypeDescriptor.GetProperties(typeof(Vector3s), attributes).Sort(new string[] { "X", "Y", "Z" });
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType){
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value){
        if (value is string stringValue){
            var parts = stringValue.Split(',').Select(s => s.Trim()).ToArray();
            if (parts.Length == 3 &&
                short.TryParse(parts[0], out short x) &&
                short.TryParse(parts[1], out short y) &&
                short.TryParse(parts[2], out short z)){
                return new Vector3s(x, y, z);
            }
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType){
        if (destinationType == typeof(string) && value is Vector3s v){
            return $"{v.X}, {v.Y}, {v.Z}";
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}