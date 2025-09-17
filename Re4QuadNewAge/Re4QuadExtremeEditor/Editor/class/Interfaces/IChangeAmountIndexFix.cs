using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re4QuadExtremeEditor.Editor.Class.ObjMethods;

namespace Re4QuadExtremeEditor.Editor.Class.Interfaces
{
    public interface IChangeAmountIndexFix : INodeChangeAmount
    {
        NodeChangeAmountCallbackMethods ChangeAmountCallbackMethods { get; }
        void OnMoveNode();
        void OnDeleteNode();
    }
}
