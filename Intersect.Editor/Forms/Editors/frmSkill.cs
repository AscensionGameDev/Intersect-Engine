using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Skills;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors;

public partial class FrmSkill : EditorForm
{
    private readonly Dictionary<Guid, SkillDescriptor> _changed = [];
    private string? _copiedItem;
    private SkillDescriptor? _editorItem;
    private readonly List<string> _knownFolders = [];

    public FrmSkill()
    {
        ApplyHooks();
        InitializeComponent();
        Icon = Program.Icon;

        _btnSave = btnSave;
        _btnCancel = btnCancel;

        lstGameObjects.Init(
            UpdateToolStripItems,
            AssignEditorItem,
            toolStripItemNew_Click,
            toolStripItemCopy_Click,
            toolStripItemUndo_Click,
            toolStripItemPaste_Click,
            toolStripItemDelete_Click
        );
    }

    #region Basic Editor Functions

    private void AssignEditorItem(Guid id)
    {
        if (!SkillDescriptor.TryGet(id, out _editorItem))
        {
            _editorItem = null;
        }

        UpdateEditor();
    }

    private void toolStripItemNew_Click(object sender, EventArgs e)
    {
        PacketSender.SendCreateObject(GameObjectType.Skill);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (_editorItem == null || !lstGameObjects.Focused)
        {
            return;
        }

        if (DarkMessageBox.ShowWarning(
                    "Are you sure you want to delete this skill?",
                    "Delete Skill",
                    DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
        {
            PacketSender.SendDeleteObject(_editorItem);
        }
    }

    private void toolStripItemCopy_Click(object sender, EventArgs e)
    {
        if (_editorItem != null && lstGameObjects.Focused)
        {
            _copiedItem = _editorItem.JsonData;
            toolStripItemPaste.Enabled = true;
        }
    }

    private void toolStripItemPaste_Click(object sender, EventArgs e)
    {
        if (_editorItem != null && _copiedItem != null && lstGameObjects.Focused)
        {
            _editorItem.Load(_copiedItem, true);
            UpdateEditor();
        }
    }

    private void toolStripItemUndo_Click(object sender, EventArgs e)
    {
        if (_editorItem == null || !_changed.ContainsKey(_editorItem.Id))
        {
            return;
        }

        if (DarkMessageBox.ShowWarning(
                    "Are you sure you want to undo your changes?",
                    "Undo Changes",
                    DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
        {
            _editorItem.RestoreBackup();
            UpdateEditor();
        }
    }

    private void UpdateToolStripItems()
    {
        var shouldEnableToolStripItems = _editorItem != null && lstGameObjects.Focused;
        toolStripItemCopy.Enabled = shouldEnableToolStripItems;
        toolStripItemPaste.Enabled = shouldEnableToolStripItems && _copiedItem != null;
        toolStripItemDelete.Enabled = shouldEnableToolStripItems;
        toolStripItemUndo.Enabled = shouldEnableToolStripItems;
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Skill)
        {
            InitEditor();
            if (_editorItem != null && !SkillDescriptor.Lookup.Values.Contains(_editorItem))
            {
                _editorItem = null;
                UpdateEditor();
            }
        }
    }

    private void btnCancel_Click(object? sender, EventArgs? e)
    {
        foreach (var item in _changed.Values)
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
        foreach (var item in _changed.Values)
        {
            PacketSender.SendSaveObject(item);
            item.DeleteBackup();
        }

        Hide();
        Globals.CurrentEditor = -1;
        Dispose();
    }

    #endregion

    #region Form Events

    private void frmSkill_Load(object sender, EventArgs e)
    {
        InitLocalization();
        UpdateEditor();
    }

    private void frmSkill_FormClosed(object sender, FormClosedEventArgs e)
    {
        btnCancel_Click(null, null);
    }

    private void form_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.N)
        {
            toolStripItemNew_Click(null, null);
        }
    }

    #endregion

    #region Intersect Setup

    private void InitLocalization()
    {
        Text = "Skill Editor";
        toolStripItemNew.Text = "New";
        toolStripItemDelete.Text = "Delete";
        toolStripItemCopy.Text = "Copy";
        toolStripItemPaste.Text = "Paste";
        toolStripItemUndo.Text = "Undo";

        grpSkills.Text = "Skills";
        grpGeneral.Text = "General";
        lblName.Text = "Name:";
        lblMaxLevel.Text = "Max Level:";
        lblBaseExperience.Text = "Base Experience:";
        lblExperienceIncrease.Text = "Experience Increase:";
        lblDescription.Text = "Description:";
        lblIcon.Text = "Icon (optional):";

        btnSave.Text = "Save";
        btnCancel.Text = "Cancel";

        //Searching/Sorting
        btnAlphabetical.ToolTipText = "Sort Alphabetically";
        txtSearch.Text = "Search...";
        lblFolder.Text = "Folder:";
    }

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in SkillDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((SkillDescriptor)itm.Value).Folder) &&
                !mFolders.Contains(((SkillDescriptor)itm.Value).Folder))
            {
                mFolders.Add(((SkillDescriptor)itm.Value).Folder);
                if (!_knownFolders.Contains(((SkillDescriptor)itm.Value).Folder))
                {
                    _knownFolders.Add(((SkillDescriptor)itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        _knownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(_knownFolders.ToArray());

        var items = SkillDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((SkillDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<SkillDescriptor>.Deleted, ((SkillDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);

        if (_editorItem != null)
        {
            var node = lstGameObjects.Nodes.Cast<TreeNode>().FirstOrDefault(n => (Guid)n.Tag == _editorItem.Id);
            if (node != null)
            {
                lstGameObjects.SelectedNode = node;
            }
        }

        UpdateEditorButtons(_editorItem != default);
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != "Search...";
    }

    private void UpdateEditor()
    {
        if (_editorItem != null)
        {
            pnlContainer.Show();

            txtName.Text = _editorItem.Name;
            cmbFolder.Text = _editorItem.Folder;
            nudMaxLevel.Value = _editorItem.MaxLevel;
            nudBaseExperience.Value = _editorItem.BaseExperience;
            nudExperienceIncrease.Value = _editorItem.ExperienceIncrease;
            txtDescription.Text = _editorItem.Description;
            txtIcon.Text = _editorItem.Icon;

            if (!_changed.TryGetValue(_editorItem.Id, out var _))
            {
                _changed.Add(_editorItem.Id, _editorItem);
                _editorItem.MakeBackup();
            }
        }
        else
        {
            pnlContainer.Hide();
        }

        UpdateEditorButtons(_editorItem != default);
        UpdateToolStripItems();
    }


    #endregion

    #region Event Handlers

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
        }
    }

    private void cmbFolder_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.Folder = cmbFolder.Text;
        }
    }

    private void nudMaxLevel_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.MaxLevel = (int)nudMaxLevel.Value;
        }
    }

    private void nudBaseExperience_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.BaseExperience = (long)nudBaseExperience.Value;
        }
    }

    private void nudExperienceIncrease_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.ExperienceIncrease = (long)nudExperienceIncrease.Value;
        }
    }

    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.Description = txtDescription.Text;
        }
    }

    private void txtIcon_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem != null)
        {
            _editorItem.Icon = txtIcon.Text;
        }
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
            txtSearch.Text = "Search...";
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == "Search...")
        {
            txtSearch.SelectAll();
        }
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "Search...";
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkUI.Forms.DarkInputBox.ShowInformation(
            "Enter folder name:", "New Folder", ref folderName,
            DarkUI.Forms.DarkDialogButton.OkCancel
        );

        if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
        {
            if (_editorItem != null && !cmbFolder.Items.Contains(folderName))
            {
                _editorItem.Folder = folderName;
                if (!_knownFolders.Contains(folderName))
                {
                    _knownFolders.Add(folderName);
                }
                InitEditor();
                cmbFolder.Text = folderName;
            }
        }
    }

    #endregion
}

