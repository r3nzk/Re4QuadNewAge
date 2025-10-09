using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re4QuadExtremeEditor.Editor
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