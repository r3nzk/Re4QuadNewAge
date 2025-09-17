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
    public class NewAge_ESE_NodeGroup : TreeNodeGroup, INodeChangeAmount
    {
        public NewAge_ESE_NodeGroup() : base() { }
        public NewAge_ESE_NodeGroup(string text) : base(text) { }
        public NewAge_ESE_NodeGroup(string text, TreeNode[] children) : base(text, children) { }

        public NewAge_ESE_Methods PropertyMethods { get; set; }

        public NewAge_ESE_MethodsForGL MethodsForGL { get; set; }

        public NodeChangeAmountMethods ChangeAmountMethods { get; set; }
    }
}
