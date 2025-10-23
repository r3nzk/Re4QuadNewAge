using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re4QuadX.Editor.Class.Enums;

namespace Re4QuadX.Editor.Class.Interfaces
{
   public interface IInternalID
    {
        ushort GetInternalID();

        GroupType GetGroupType();

    }
}
