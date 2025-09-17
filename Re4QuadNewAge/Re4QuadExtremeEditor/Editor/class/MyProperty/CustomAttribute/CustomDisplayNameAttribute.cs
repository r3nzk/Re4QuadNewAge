using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Re4QuadExtremeEditor.Editor.Class.Enums;

namespace Re4QuadExtremeEditor.Editor.Class.MyProperty.CustomAttribute
{
    public class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        public CustomDisplayNameAttribute(aLang AttributeTextId) : base(Lang.GetAttributeText(AttributeTextId)) {}
    }

}
