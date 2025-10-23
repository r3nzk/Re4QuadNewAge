using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re4QuadX.Editor.Class.CustomDelegates;

namespace Re4QuadX.Editor.Class.ObjMethods
{
    /// <summary>
    /// classe com os metodos responsaveis pelo oque sera exibido no node;
    /// </summary>
    public class NodeDisplayMethods
    {
        /// <summary>
        /// Retorna o texto do node;
        /// </summary>
        public ReturnString GetNodeText;

        /// <summary>
        /// Retorna a cor para o node;
        /// </summary>
        public ReturnColor GetNodeColor;

    }
}
