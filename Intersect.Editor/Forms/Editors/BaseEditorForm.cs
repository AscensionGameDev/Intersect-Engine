using System.Windows.Forms;

namespace Intersect.Editor.Forms.Editors;

public class BaseEditorForm : EditorForm
{
    protected Button btnSave;
    protected Button btnCancel;

    protected void SetEditorButtons(Button saveButton, Button cancelButton)
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