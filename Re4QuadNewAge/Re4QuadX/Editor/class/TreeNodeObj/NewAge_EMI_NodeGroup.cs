using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.Class.ObjMethods;
using Re4QuadX.Editor.Class.Interfaces;

namespace Re4QuadX.Editor.Class.TreeNodeObj
{
    public class NewAge_EMI_NodeGroup : TreeNodeGroup, INodeChangeAmount
    {
        public NewAge_EMI_NodeGroup() : base() { }
        public NewAge_EMI_NodeGroup(string text) : base(text) { }
        public NewAge_EMI_NodeGroup(string text, TreeNode[] children) : base(text, children) { }

        public NewAge_EMI_Methods PropertyMethods { get; set; }

        public NewAge_EMI_MethodsForGL MethodsForGL { get; set; }

        public NodeChangeAmountMethods ChangeAmountMethods { get; set; }
    }
}
