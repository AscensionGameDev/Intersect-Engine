using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmTitle : EditorForm
{

    private List<TitleDescriptor> mChanged = new List<TitleDescriptor>();

    private string mCopiedItem;

    private TitleDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    public FrmTitle()
    {
        ApplyHooks();
        InitializeComponent();
        Icon = Program.Icon;
        _btnSave = btnSave;
        _btnCancel = btnCancel;

        lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
    }

    private void AssignEditorItem(Guid id)
    {
        mEditorItem = TitleDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Title)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<TitleDescriptor>.Lookup.Values.Contains(mEditorItem))
            {
                mEditorItem = null;
                UpdateEditor();
            }
        }
    }

    private void UpdateEditor()
    {
        if (mEditorItem != null)
        {
            pnlContainer.Show();

            txtName.Text = mEditorItem.Name;
            cmbFolder.Text = mEditorItem.Folder;
            UpdatePreview();

            if (mChanged.IndexOf(mEditorItem) == -1)
            {
                mChanged.Add(mEditorItem);
                mEditorItem.MakeBackup();
            }
        }
        else
        {
            pnlContainer.Hide();
        }

        var hasItem = mEditorItem != null;
        UpdateEditorButtons(hasItem);
        UpdateToolStripItems();
    }

    private void UpdatePreview()
    {
        var color = mEditorItem?.Color ?? Color.White;
        var systemColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        pnlColor.BackColor = systemColor;
        lblPreview.ForeColor = systemColor;
        lblPreview.Text = mEditorItem?.Name ?? string.Empty;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        mEditorItem.Name = txtName.Text;
        lstGameObjects.UpdateText(txtName.Text);
        UpdatePreview();
    }

    private void btnSelectColor_Click(object sender, EventArgs e)
    {
        if (mEditorItem == null)
        {
            return;
        }

        colorDialog.Color = pnlColor.BackColor;
        if (colorDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        mEditorItem.Color = Color.FromArgb(
            colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B
        );

        UpdatePreview();
    }

    private void FrmTitle_FormClosed(object sender, FormClosedEventArgs e)
    {
        btnCancel_Click(null, null);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        foreach (var item in mChanged)
        {
            item.RestoreBackup();
            item.DeleteBackup();
        }

        Hide();
        Globals.CurrentEditor = -1;
        Dispose();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        //Send Changed items
        foreach (var item in mChanged)
        {
            PacketSender.SendSaveObject(item);
            item.DeleteBackup();
        }

        Hide();
        Globals.CurrentEditor = -1;
        Dispose();
    }

    private void toolStripItemNew_Click(object sender, EventArgs e)
    {
        PacketSender.SendCreateObject(GameObjectType.Title);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.TitleEditor.deleteprompt, Strings.TitleEditor.delete,
                    DarkDialogButton.YesNo, Icon
                ) ==
                DialogResult.Yes)
            {
                PacketSender.SendDeleteObject(mEditorItem);
            }
        }
    }

    private void toolStripItemCopy_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            mCopiedItem = mEditorItem.JsonData;
            toolStripItemPaste.Enabled = true;
        }
    }

    private void toolStripItemPaste_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused)
        {
            mEditorItem.Load(mCopiedItem, true);
            UpdateEditor();
        }
    }

    private void toolStripItemUndo_Click(object sender, EventArgs e)
    {
        if (mChanged.Contains(mEditorItem) && mEditorItem != null)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.TitleEditor.undoprompt, Strings.TitleEditor.undotitle,
                    DarkDialogButton.YesNo, Icon
                ) ==
                DialogResult.Yes)
            {
                mEditorItem.RestoreBackup();
                UpdateEditor();
            }
        }
    }

    private void UpdateToolStripItems()
    {
        toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
        toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
        toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
        toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
    }

    private void form_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            if (e.KeyCode == Keys.N)
            {
                toolStripItemNew_Click(null, null);
            }
        }
    }

    private void frmTitle_Load(object sender, EventArgs e)
    {
        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.TitleEditor.title;
        toolStripItemNew.Text = Strings.TitleEditor.New;
        toolStripItemDelete.Text = Strings.TitleEditor.delete;
        toolStripItemCopy.Text = Strings.TitleEditor.copy;
        toolStripItemPaste.Text = Strings.TitleEditor.paste;
        toolStripItemUndo.Text = Strings.TitleEditor.undo;

        grpTitles.Text = Strings.TitleEditor.titles;
        grpGeneral.Text = Strings.TitleEditor.general;
        grpPreview.Text = Strings.TitleEditor.preview;
        lblName.Text = Strings.TitleEditor.name;
        lblColor.Text = Strings.TitleEditor.color;
        btnSelectColor.Text = Strings.TitleEditor.selectcolor;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.TitleEditor.sortalphabetically;
        txtSearch.Text = Strings.TitleEditor.searchplaceholder;
        lblFolder.Text = Strings.TitleEditor.folderlabel;

        btnSave.Text = Strings.TitleEditor.save;
        btnCancel.Text = Strings.TitleEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in TitleDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((TitleDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((TitleDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((TitleDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((TitleDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((TitleDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = TitleDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((TitleDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<TitleDescriptor>.Deleted, ((TitleDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.TitleEditor.folderprompt, Strings.TitleEditor.foldertitle, ref folderName,
            DarkDialogButton.OkCancel
        );

        if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
        {
            if (!cmbFolder.Items.Contains(folderName))
            {
                mEditorItem.Folder = folderName;
                lstGameObjects.ExpandFolder(folderName);
                InitEditor();
                cmbFolder.Text = folderName;
            }
        }
    }

    private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
    {
        mEditorItem.Folder = cmbFolder.Text;
        InitEditor();
    }

    private void btnAlphabetical_Click(object sender, EventArgs e)
    {
        btnAlphabetical.Checked = !btnAlphabetical.Checked;
        InitEditor();
    }

    private void txtSearch_TextChanged(object sender, EventArgs e)
    {
        InitEditor();
    }

    private void txtSearch_Leave(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            txtSearch.Text = Strings.TitleEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.TitleEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.TitleEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.TitleEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
