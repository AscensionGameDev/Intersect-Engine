using System.Windows.Forms;

namespace Intersect.Editor.Forms.Editors;

public class BaseEditorForm : EditorForm
{
    protected Button btnSave;

    protected void SetEditorButtons(Button saveButton)
    {
        btnSave = saveButton;

        // Keeping method name as 'SetEditorButtons' in case we want to support multiple buttons (e.g. Cancel) later.
    }

    protected void UpdateEditorButtons(bool isItemSelected, bool hasChanges)
    {
        if (btnSave != null)
        {
            btnSave.Visible = isItemSelected && hasChanges;
            btnSave.Enabled = isItemSelected && hasChanges;
        }
    }
}

