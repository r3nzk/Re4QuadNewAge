using System;
using System.ComponentModel;
using System.Drawing.Design;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using Re4QuadExtremeEditor.Editor.Class.TreeNodeObj;

namespace Re4QuadExtremeEditor.Editor.Class.MyProperty.CustomUITypeEditor
{
    public class FocusNodeObjectEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){
            return UITypeEditorEditStyle.Modal;
        }

        private TreeNodeGroup GetParentNodeFromFileType(string fileType){
            switch (fileType?.ToUpperInvariant())
            {
                case "ITA": return DataBase.NodeITA;
                case "AEV": return DataBase.NodeAEV;
                default: return null;
            }
        }

        private void PerformFocusAction(RefInteractionType type, ushort id){
            string fileType = DataBase.Extras.AssociatedSpecialEventFromFile(type, id);
            string lineIdStr = DataBase.Extras.GetAssociatedSpecialEventLineID(type, id);

            if (fileType != "" && lineIdStr != ""){
                TreeNodeGroup parentNode = GetParentNodeFromFileType(fileType);
                if (parentNode != null && MainForm.instance != null){
                    int index = parentNode.Nodes.IndexOfKey(lineIdStr);
                    if (index > -1)
                    {
                        MainForm.instance.SelectNode(parentNode.Nodes[index]);
                    }
                    else
                    {
                        Editor.Console.Error($"Associated object node not found (ID: {lineIdStr} in {fileType}).");
                    }
                }
                else if (parentNode == null)
                {
                    Editor.Console.Error($"Unable to determine the parent node for file type '{fileType}'.");
                }
            }
            else
            {
                Editor.Console.Warning("No association assigned to this object. Check if the associated object is present or null.");
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object instance = context?.Instance;
            if (instance is PropertyFilterDescriptor wrapper){
                instance = wrapper.OriginalObject;
            }

            //enemy
            if (instance is EnemyProperty enemyProp)
            {
                PerformFocusAction(RefInteractionType.Enemy, enemyProp.GetInternalID());
            }
            //ets (etcmodel)
            else if (instance is EtcModelProperty etcProp)
            {
                PerformFocusAction(RefInteractionType.EtcModel, etcProp.PROP_ETS_ID);
            }

            return value;
        }
    }
}

