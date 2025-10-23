using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.Class.ObjMethods;

namespace Re4QuadX.Editor.Class.TreeNodeObj
{
    public class EnemyNodeGroup : TreeNodeGroup
    {
        public EnemyNodeGroup() : base() { }
        public EnemyNodeGroup(string text) : base(text) { }
        public EnemyNodeGroup(string text, TreeNode[] children) : base(text, children) { }

        public EnemyMethods PropertyMethods { get; set; }

        public EnemyMethodsForGL MethodsForGL { get; set; }
    }
}
