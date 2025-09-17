using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re4QuadExtremeEditor.Editor.Class.CustomDelegates;

namespace Re4QuadExtremeEditor.Editor.Class.ObjMethods
{
    public abstract class BaseTriggerZoneMethodsForGL
    {
        public ReturnVector2Array GetTriggerZone;

        public ReturnVector2Array GetCircleTriggerZone;

        public ReturnTriggerZoneCategory GetZoneCategory;

        public ReturnMatrix4 GetTriggerZoneMatrix4;
    }
}
