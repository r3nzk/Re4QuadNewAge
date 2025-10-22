using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re4QuadX.Editor.Class.MyProperty.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    public class DecNegativeNumberAttribute : Attribute
    {
    }
}
