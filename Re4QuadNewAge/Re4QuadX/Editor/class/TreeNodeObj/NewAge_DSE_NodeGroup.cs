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
    public class NewAge_DSE_NodeGroup : TreeNodeGroup, INodeChangeAmount
    {
        public NewAge_DSE_NodeGroup() : base() { }
        public NewAge_DSE_NodeGroup(string text) : base(text) { }
        public NewAge_DSE_NodeGroup(string text, TreeNode[] children) : base(text, children) { }

        public NodeChangeAmountMethods ChangeAmountMethods { get; set; }
        public NewAge_DSE_Methods PropertyMethods { get; set; }
    }
}
