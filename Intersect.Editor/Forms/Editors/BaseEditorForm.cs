using System.Windows.Forms;
using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors;

public class BaseEditorForm : EditorForm
{
    protected DarkButton? btnSave;
    protected DarkButton? btnCancel;

    protected void SetEditorButtons(DarkButton saveButton, DarkButton cancelButton)
    {
        btnSave = saveButton;
        btnCancel = cancelButton;
    }

    protected void UpdateEditorButtons(bool isItemSelected)
    {
        if (btnSave != null)
        {
            btnSave.Visible = isItemSelected;
            btnSave.Enabled = isItemSelected;
        }

        if (btnCancel != null)
        {
            btnCancel.Visible = isItemSelected;
            btnCancel.Enabled = isItemSelected;
        }
    }
}