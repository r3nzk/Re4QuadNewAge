using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using Re4QuadExtremeEditor.Editor.Class.ObjMethods;
using Re4QuadExtremeEditor.Editor.Class.Interfaces;

namespace Re4QuadExtremeEditor.Editor.Class.TreeNodeObj
{
    public class ExtraNodeGroup : TreeNodeGroup
    {
        public ExtraNodeGroup() : base() { }
        public ExtraNodeGroup(string text) : base(text) { }
        public ExtraNodeGroup(string text, TreeNode[] children) : base(text, children) { }
    }
}
