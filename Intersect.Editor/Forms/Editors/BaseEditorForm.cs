using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors;

public class BaseEditorForm : EditorForm
{
    private DarkButton? btnSave;
    private DarkButton? btnCancel;

    protected void SetSaveButton(DarkButton saveButton)
    {
        btnSave = saveButton;
    }

    protected void SetCancelButton(DarkButton cancelButton)
    {
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