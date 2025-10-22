using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

public class CheckBoxUITypeEditor : UITypeEditor
{

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
        return UITypeEditorEditStyle.None;
    }

    public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
        if (value is bool bValue)
        {
            return !bValue;
        }
        return value;
    }

    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    {
        return true;
    }

    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e.Value is bool value)
        {
            CheckBoxState state = value ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            CheckBoxRenderer.DrawCheckBox(e.Graphics, e.Bounds.Location, state);
        }
    }

}