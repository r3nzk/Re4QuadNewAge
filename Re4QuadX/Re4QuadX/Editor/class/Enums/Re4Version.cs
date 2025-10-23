using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re4QuadX.Editor.Class.Enums
{
    public enum Re4Version : byte
    {
        NULL = 0,
        V2007PS2 = 1,
        UHD = 2
    }

    public enum IsRe4Version : byte
    {
        NULL = 0,
        V2007PS2 = 1,
        UHD = 2,
        PS4NS = 3,
        BIG_ENDIAN = 4
    }

    public enum EditorRe4Ver : byte
    {
        None = 0,
        UHD = 1,
        SourceNext2007 = 2,
        PS2 = 3,
        PS4NS = 4
    }
}
