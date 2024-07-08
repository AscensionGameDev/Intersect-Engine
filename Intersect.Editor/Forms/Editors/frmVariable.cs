using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmSwitchVariable : EditorForm
{

    private List<IDatabaseObject> mChanged = new List<IDatabaseObject>();

    private IDatabaseObject mEditorItem;

    private List<string> mExpandedFolders = new List<string>();

    private List<string> mGlobalExpandedFolders = new List<string>();

    private List<string> mGlobalKnownFolders = new List<string>();

    private List<string> mKnownFolders = new List<string>();

    private List<string> mGuildKnownFolders = new List<string>();

    private List<string> mGuildExpandedFolders = new List<string>();

    private List<string> mUserKnownFolders = new List<string>();

    private List<string> mUserExpandedFolders = new List<string>();

    public FrmSwitchVariable()
    {
        ApplyHooks();
        InitializeComponent();
        Icon = Program.Icon;

        InitLocalization();
        nudVariableValue.Minimum = long.MinValue;
        nudVariableValue.Maximum = long.MaxValue;

        lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, null, toolStripItemUndo_Click, null, toolStripItemDelete_Click);
    }

    private void AssignEditorItem(Guid id)
    {
        if (id != Guid.Empty)
        {
            IDatabaseObject obj = null;
            if (rdoPlayerVariables.Checked)
            {
                obj = PlayerVariableBase.Get(id);
            }
            else if (rdoGlobalVariables.Checked)
            {
                obj = ServerVariableBase.Get(id);
            }
            else if (rdoGuildVariables.Checked)
            {
                obj = GuildVariableBase.Get(id);
            }
            else if (rdoUserVariables.Checked)
            {
                obj = UserVariableBase.Get(id);
            }

            if (obj != null)
            {
                mEditorItem = obj;
                if (!mChanged.Contains(obj))
                {
                    mChanged.Add(obj);
                    obj.MakeBackup();
                }
            }
        }

        UpdateEditor();
    }

    private void InitLocalization()
    {
        Text = Strings.VariableEditor.title;
        grpTypes.Text = Strings.VariableEditor.type;
        grpList.Text = Strings.VariableEditor.list;
        rdoPlayerVariables.Text = Strings.VariableEditor.playervariables;
        rdoGlobalVariables.Text = Strings.VariableEditor.globalvariables;
        rdoGuildVariables.Text = Strings.VariableEditor.guildvariables;
        rdoUserVariables.Text = Strings.GameObjectStrings.UserVariables;
        grpEditor.Text = Strings.VariableEditor.editor;
        lblName.Text = Strings.VariableEditor.name;
        grpValue.Text = Strings.VariableEditor.value;
        cmbBooleanValue.Items.Clear();
        cmbBooleanValue.Items.Add(Strings.VariableEditor.False);
        cmbBooleanValue.Items.Add(Strings.VariableEditor.True);
        cmbVariableType.Items.Clear();
        foreach (var itm in Strings.VariableEditor.types)
        {
            cmbVariableType.Items.Add(itm.Value);
        }

        toolStripItemNew.ToolTipText = Strings.VariableEditor.New;
        toolStripItemDelete.ToolTipText = Strings.VariableEditor.delete;
        toolStripItemUndo.ToolTipText = Strings.VariableEditor.undo;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.VariableEditor.sortalphabetically;
        txtSearch.Text = Strings.VariableEditor.searchplaceholder;
        lblFolder.Text = Strings.VariableEditor.folderlabel;

        btnSave.Text = Strings.VariableEditor.save;
        btnCancel.Text = Strings.VariableEditor.cancel;
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.PlayerVariable)
        {
            InitEditor();
            if (mEditorItem != null && !PlayerVariableBase.Lookup.Values.Contains(mEditorItem))
            {
                mEditorItem = null;
                UpdateEditor();
            }
        }
        else if (type == GameObjectType.ServerVariable)
        {
            InitEditor();
            if (mEditorItem != null && !ServerVariableBase.Lookup.Values.Contains(mEditorItem))
            {
                mEditorItem = null;
                UpdateEditor();
            }
        }
        else if (type == GameObjectType.GuildVariable)
        {
            InitEditor();
            if (mEditorItem != null && !GuildVariableBase.Lookup.Values.Contains(mEditorItem))
            {
                mEditorItem = null;
                UpdateEditor();
            }
        }
        else if (type == GameObjectType.UserVariable)
        {
            InitEditor();
            if (mEditorItem != null && !UserVariableBase.Lookup.Values.Contains(mEditorItem))
            {
                mEditorItem = null;
                UpdateEditor();
            }
        }
    }

    private void toolStripItemNew_Click(object sender, EventArgs e)
    {
        if (rdoPlayerVariables.Checked)
        {
            PacketSender.SendCreateObject(GameObjectType.PlayerVariable);
        }
        else if (rdoGlobalVariables.Checked)
        {
            PacketSender.SendCreateObject(GameObjectType.ServerVariable);
        }
        else if (rdoGuildVariables.Checked)
        {
            PacketSender.SendCreateObject(GameObjectType.GuildVariable);
        }
        else if (rdoUserVariables.Checked)
        {
            PacketSender.SendCreateObject(GameObjectType.UserVariable);
        }
    }

    private void toolStripItemUndo_Click(object sender, EventArgs e)
    {
        if (mChanged.Contains(mEditorItem) && mEditorItem != null)
        {
            mEditorItem.RestoreBackup();
            UpdateEditor();
        }
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.VariableEditor.deleteprompt, Strings.VariableEditor.deletecaption,
                    DarkDialogButton.YesNo, Icon
                ) ==
                DialogResult.Yes)
            {
                PacketSender.SendDeleteObject(mEditorItem);
            }
        }
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

    private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();

    }

    private void rdoGuildVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();

    }
    private void rdoUserVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void VariableRadioChanged()
    {
        mEditorItem = null;
        lstGameObjects.ClearExpandedFolders();
        InitEditor();
    }

    private void UpdateToolStripItems()
    {
        toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
        toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
    }

    private void UpdateEditor()
    {
        if (mEditorItem != null)
        {
            grpEditor.Show();
            grpValue.Hide();
            if (rdoPlayerVariables.Checked)
            {
                lblObject.Text = Strings.VariableEditor.playervariable;
                txtObjectName.Text = ((PlayerVariableBase) mEditorItem).Name;
                txtId.Text = ((PlayerVariableBase) mEditorItem).TextId;
                cmbFolder.Text = ((PlayerVariableBase) mEditorItem).Folder;
                cmbVariableType.SelectedIndex = (int) (((PlayerVariableBase) mEditorItem).Type - 1);
            }
            else if (rdoGlobalVariables.Checked)
            {
                lblObject.Text = Strings.VariableEditor.globalvariable;
                txtObjectName.Text = ((ServerVariableBase) mEditorItem).Name;
                txtId.Text = ((ServerVariableBase) mEditorItem).TextId;
                cmbFolder.Text = ((ServerVariableBase) mEditorItem).Folder;
                cmbVariableType.SelectedIndex = (int) (((ServerVariableBase) mEditorItem).Type - 1);
                grpValue.Show();
            }
            else if (rdoGuildVariables.Checked)
            {
                lblObject.Text = Strings.VariableEditor.guildvariable;
                txtObjectName.Text = ((GuildVariableBase)mEditorItem).Name;
                txtId.Text = ((GuildVariableBase)mEditorItem).TextId;
                cmbFolder.Text = ((GuildVariableBase)mEditorItem).Folder;
                cmbVariableType.SelectedIndex = (int)(((GuildVariableBase)mEditorItem).Type - 1);
            }
            else if (rdoUserVariables.Checked)
            {
                lblObject.Text = Strings.GameObjectStrings.UserVariable;
                txtObjectName.Text = ((UserVariableBase)mEditorItem).Name;
                txtId.Text = ((UserVariableBase)mEditorItem).TextId;
                cmbFolder.Text = ((UserVariableBase)mEditorItem).Folder;
                cmbVariableType.SelectedIndex = (int)(((UserVariableBase)mEditorItem).Type - 1);
            }

            InitValueGroup();
        }
        else
        {
            grpEditor.Hide();
        }

        UpdateToolStripItems();
    }

    private void UpdateSelection()
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            grpEditor.Show();
            if (rdoPlayerVariables.Checked)
            {
                var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                lstGameObjects.SelectedNode.Text = obj.Name;
                grpValue.Hide();
            }
            else if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                lstGameObjects.SelectedNode.Text = obj.Name + " = " + obj.Value.ToString(obj.Type);
            }
            else if (rdoPlayerVariables.Checked)
            {
                var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                lstGameObjects.SelectedNode.Text = obj.Name;
                grpValue.Hide();
            }
            else if (rdoUserVariables.Checked)
            {
                var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                lstGameObjects.SelectedNode.Text = obj.Name;
                grpValue.Hide();
            }
        }
    }

    private void txtObjectName_TextChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            grpEditor.Show();
            grpValue.Hide();
            if (rdoPlayerVariables.Checked)
            {
                var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Name = txtObjectName.Text;
                lstGameObjects.UpdateText(obj.Name);
            }
            else if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Name = txtObjectName.Text;
                lstGameObjects.UpdateText(obj.Name + " = " + obj.Value.ToString());
            }
            else if (rdoGuildVariables.Checked)
            {
                var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Name = txtObjectName.Text;
                lstGameObjects.UpdateText(obj.Name);
            }
            else if (rdoUserVariables.Checked)
            {
                var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Name = txtObjectName.Text;
                lstGameObjects.UpdateText(obj.Name);
            }
        }
    }

    private void txtId_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.Handled = e.KeyChar == ' ';
    }

    private void txtId_TextChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoPlayerVariables.Checked)
            {
                var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.TextId = txtId.Text;
            }
            else if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.TextId = txtId.Text;
            }
            else if (rdoGuildVariables.Checked)
            {
                var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.TextId = txtId.Text;
            }
            else if (rdoUserVariables.Checked)
            {
                var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.TextId = txtId.Text;
            }
        }
    }

    private void cmbVariableType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoPlayerVariables.Checked)
            {
                var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Type = (VariableDataType) (cmbVariableType.SelectedIndex + 1);
            }
            else if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Type = (VariableDataType) (cmbVariableType.SelectedIndex + 1);
            }
            else if (rdoGuildVariables.Checked)
            {
                var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Type = (VariableDataType)(cmbVariableType.SelectedIndex + 1);
            }
            else if (rdoUserVariables.Checked)
            {
                var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Type = (VariableDataType)(cmbVariableType.SelectedIndex + 1);
            }

            InitValueGroup();
            UpdateSelection();
        }
    }

    private void InitValueGroup()
    {
        if (rdoPlayerVariables.Checked || rdoGuildVariables.Checked || rdoUserVariables.Checked)
        {
            grpValue.Hide();
        }
        else
        {
            if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                cmbBooleanValue.Hide();
                nudVariableValue.Hide();
                txtStringValue.Hide();
                switch (obj.Type)
                {
                    case VariableDataType.Boolean:
                        cmbBooleanValue.Show();
                        cmbBooleanValue.SelectedIndex = Convert.ToInt32(obj.Value.Boolean);

                        break;

                    case VariableDataType.Integer:
                        nudVariableValue.Show();
                        nudVariableValue.Value = obj.Value.Integer;

                        break;

                    case VariableDataType.Number:
                        break;

                    case VariableDataType.String:
                        txtStringValue.Show();
                        txtStringValue.Text = obj.Value.String;

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void nudVariableValue_ValueChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                if (obj != null)
                {
                    obj.Value.Integer = (long)nudVariableValue.Value;
                    UpdateSelection();
                }
            }
        }
    }

    private void cmbBooleanValue_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                if (obj != null)
                {
                    obj.Value.Boolean = Convert.ToBoolean(cmbBooleanValue.SelectedIndex);
                    UpdateSelection();
                }
            }

            UpdateSelection();
        }
    }

    private void txtStringValue_TextChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                if (obj != null)
                {
                    obj.Value.String = txtStringValue.Text;
                    UpdateSelection();
                }
            }
        }
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Fix Title
        if (rdoPlayerVariables.Checked)
        {
            grpVariables.Text = rdoPlayerVariables.Text;
        }
        else if (rdoGlobalVariables.Checked)
        {
            grpVariables.Text = rdoGlobalVariables.Text;
        }
        else if (rdoGuildVariables.Checked)
        {
            grpVariables.Text = rdoGuildVariables.Text;
        }
        else if (rdoUserVariables.Checked)
        {
            grpVariables.Text = rdoUserVariables.Text;
        }

        grpEditor.Hide();
        cmbBooleanValue.Hide();
        nudVariableValue.Hide();
        txtStringValue.Hide();

        //Collect folders
        var mFolders = new List<string>();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");

        if (rdoPlayerVariables.Checked)
        {
            foreach (var itm in PlayerVariableBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((PlayerVariableBase) itm.Value).Folder) &&
                    !mFolders.Contains(((PlayerVariableBase) itm.Value).Folder))
                {
                    mFolders.Add(((PlayerVariableBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((PlayerVariableBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((PlayerVariableBase) itm.Value).Folder);
                    }
                }
            }

            mKnownFolders.Sort();
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());
            lblId.Text = Strings.VariableEditor.textidpv;
        }
        else if (rdoGlobalVariables.Checked)
        {
            foreach (var itm in ServerVariableBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ServerVariableBase) itm.Value).Folder) &&
                    !mFolders.Contains(((ServerVariableBase) itm.Value).Folder))
                {
                    mFolders.Add(((ServerVariableBase) itm.Value).Folder);
                    if (!mGlobalKnownFolders.Contains(((ServerVariableBase) itm.Value).Folder))
                    {
                        mGlobalKnownFolders.Add(((ServerVariableBase) itm.Value).Folder);
                    }
                }
            }

            mGlobalKnownFolders.Sort();
            cmbFolder.Items.AddRange(mGlobalKnownFolders.ToArray());
            lblId.Text = Strings.VariableEditor.textidgv;
        }
        else if (rdoGuildVariables.Checked)
        {
            foreach (var itm in GuildVariableBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((GuildVariableBase)itm.Value).Folder) &&
                    !mFolders.Contains(((GuildVariableBase)itm.Value).Folder))
                {
                    mFolders.Add(((GuildVariableBase)itm.Value).Folder);
                    if (!mGuildKnownFolders.Contains(((GuildVariableBase)itm.Value).Folder))
                    {
                        mGuildKnownFolders.Add(((GuildVariableBase)itm.Value).Folder);
                    }
                }
            }

            mGuildKnownFolders.Sort();
            cmbFolder.Items.AddRange(mGuildKnownFolders.ToArray());
            lblId.Text = Strings.VariableEditor.textidguildvar;
        }
        else if (rdoUserVariables.Checked)
        {
            foreach (var itm in UserVariableBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((UserVariableBase)itm.Value).Folder) &&
                    !mFolders.Contains(((UserVariableBase)itm.Value).Folder))
                {
                    mFolders.Add(((UserVariableBase)itm.Value).Folder);
                    if (!mUserKnownFolders.Contains(((UserVariableBase)itm.Value).Folder))
                    {
                        mUserKnownFolders.Add(((UserVariableBase)itm.Value).Folder);
                    }
                }
            }

            mUserKnownFolders.Sort();
            cmbFolder.Items.AddRange(mUserKnownFolders.ToArray());
            lblId.Text = Strings.VariableEditor.UserVariableId;
        }

        mFolders.Sort();

        KeyValuePair<Guid, KeyValuePair<string, string>>[] items = null;

        if (rdoPlayerVariables.Checked)
        {
            items = PlayerVariableBase.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((PlayerVariableBase)pair.Value)?.Name ?? DatabaseObject<PlayerVariableBase>.Deleted, ((PlayerVariableBase)pair.Value)?.Folder ?? ""))).ToArray();
        }
        else if (rdoGlobalVariables.Checked)
        {
            items = ServerVariableBase.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((ServerVariableBase)pair.Value)?.Name ?? DatabaseObject<ServerVariableBase>.Deleted + " = " + ((ServerVariableBase)pair.Value)?.Value.ToString(((ServerVariableBase)pair.Value).Type) ?? "", ((ServerVariableBase)pair.Value)?.Folder ?? ""))).ToArray();
        }
        else if (rdoGuildVariables.Checked)
        {
            items = GuildVariableBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((GuildVariableBase)pair.Value)?.Name ?? DatabaseObject<GuildVariableBase>.Deleted, ((GuildVariableBase)pair.Value)?.Folder ?? ""))).ToArray();
        }
        else if (rdoUserVariables.Checked)
        {
            items = UserVariableBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((UserVariableBase)pair.Value)?.Name ?? DatabaseObject<UserVariableBase>.Deleted, ((UserVariableBase)pair.Value)?.Folder ?? ""))).ToArray();
        }

        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);

        UpdateEditor();
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = "";
        var result = DarkInputBox.ShowInformation(
            Strings.VariableEditor.folderprompt, Strings.VariableEditor.foldertitle, ref folderName,
            DarkDialogButton.OkCancel
        );

        if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
        {
            if (!cmbFolder.Items.Contains(folderName))
            {
                if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
                {
                    if (rdoPlayerVariables.Checked)
                    {
                        var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                        obj.Folder = folderName;
                        mExpandedFolders.Add(folderName);
                    }
                    else if (rdoGlobalVariables.Checked)
                    {
                        var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                        obj.Folder = folderName;
                        mGlobalExpandedFolders.Add(folderName);
                    }
                    else if (rdoGuildVariables.Checked)
                    {
                        var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                        obj.Folder = folderName;
                        mGuildExpandedFolders.Add(folderName);
                    }
                    else if (rdoUserVariables.Checked)
                    {
                        var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                        obj.Folder = folderName;
                        mUserExpandedFolders.Add(folderName);
                    }

                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }
    }

    private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstGameObjects.SelectedNode != null && lstGameObjects.SelectedNode.Tag != null)
        {
            if (rdoPlayerVariables.Checked)
            {
                var obj = PlayerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Folder = cmbFolder.Text;
            }
            else if (rdoGlobalVariables.Checked)
            {
                var obj = ServerVariableBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
                obj.Folder = cmbFolder.Text;
            }
            else if (rdoGuildVariables.Checked)
            {
                var obj = GuildVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Folder = cmbFolder.Text;
            }
            else if (rdoUserVariables.Checked)
            {
                var obj = UserVariableBase.Get((Guid)lstGameObjects.SelectedNode.Tag);
                obj.Folder = cmbFolder.Text;
            }

            InitEditor();
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
            txtSearch.Text = Strings.VariableEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.VariableEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.VariableEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.VariableEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion

}
