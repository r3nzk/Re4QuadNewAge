using Re4QuadX.Editor.Class.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re4QuadX.Editor
{
    public enum EditorTool
    {
        None,
        Move,
        Rotate,
        Scale //TODO (may not be applied for re4 in most cases)
    }

    public enum GizmoSpace
    {
        World,
        Local
    }

}