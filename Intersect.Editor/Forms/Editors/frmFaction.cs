using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Factions;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmFaction : EditorForm
{

    private List<FactionDescriptor> mChanged = new List<FactionDescriptor>();

    private string mCopiedItem;

    private FactionDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    private bool mLoading;

    public FrmFaction()
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
        mEditorItem = FactionDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Faction)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<FactionDescriptor>.Lookup.Values.Contains(mEditorItem))
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
            mLoading = true;
            pnlContainer.Show();

            txtName.Text = mEditorItem.Name;
            cmbFolder.Text = mEditorItem.Folder;
            SelectMap(mEditorItem.StartMapId);
            nudEntryX.Value = Math.Clamp(mEditorItem.StartX, nudEntryX.Minimum, nudEntryX.Maximum);
            nudEntryY.Value = Math.Clamp(mEditorItem.StartY, nudEntryY.Minimum, nudEntryY.Maximum);
            if (cmbEntryDir.Items.Count > 0)
            {
                cmbEntryDir.SelectedIndex = Math.Clamp(mEditorItem.StartDir, 0, cmbEntryDir.Items.Count - 1);
            }

            var systemColor = System.Drawing.Color.FromArgb(mEditorItem.Color.A, mEditorItem.Color.R, mEditorItem.Color.G, mEditorItem.Color.B);
            pnlColor.BackColor = systemColor;
            mLoading = false;

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

    private void SelectMap(Guid mapId)
    {
        cmbEntryMap.SelectedIndex = -1;
        for (var i = 0; i < MapList.OrderedMaps.Count; i++)
        {
            if (MapList.OrderedMaps[i].MapId == mapId)
            {
                cmbEntryMap.SelectedIndex = i;

                break;
            }
        }
    }

    #region Field handlers

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.Name = txtName.Text;
        lstGameObjects.UpdateText(txtName.Text);
    }

    private void cmbEntryMap_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!mLoading && cmbEntryMap.SelectedIndex > -1)
        {
            mEditorItem.StartMapId = MapList.OrderedMaps[cmbEntryMap.SelectedIndex].MapId;
        }
    }

    private void nudEntry_ValueChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.StartX = (int)nudEntryX.Value;
        mEditorItem.StartY = (int)nudEntryY.Value;
    }

    private void cmbEntryDir_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!mLoading && cmbEntryDir.SelectedIndex > -1)
        {
            mEditorItem.StartDir = cmbEntryDir.SelectedIndex;
        }
    }

    private void btnSelectColor_Click(object sender, EventArgs e)
    {
        if (mEditorItem == null)
        {
            return;
        }

        var colorDialog = new ColorDialog { Color = pnlColor.BackColor };
        if (colorDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        mEditorItem.Color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
        pnlColor.BackColor = colorDialog.Color;
    }

    private void btnVisual_Click(object sender, EventArgs e)
    {
        if (cmbEntryMap.SelectedIndex < 0)
        {
            return;
        }

        var frmWarpSelection = new FrmWarpSelection();
        frmWarpSelection.SelectTile(MapList.OrderedMaps[cmbEntryMap.SelectedIndex].MapId, (int)nudEntryX.Value, (int)nudEntryY.Value);
        frmWarpSelection.ShowDialog();
        if (!frmWarpSelection.GetResult())
        {
            return;
        }

        SelectMap(frmWarpSelection.GetMap());
        nudEntryX.Value = frmWarpSelection.GetX();
        nudEntryY.Value = frmWarpSelection.GetY();
    }

    #endregion

    private void FrmFaction_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Faction);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.FactionEditor.deleteprompt, Strings.FactionEditor.delete,
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
                    Strings.FactionEditor.undoprompt, Strings.FactionEditor.undotitle,
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

    private void frmDungeon_Load(object sender, EventArgs e)
    {
        cmbEntryMap.Items.Clear();
        foreach (var map in MapList.OrderedMaps)
        {
            cmbEntryMap.Items.Add(map.Name);
        }

        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.FactionEditor.title;
        toolStripItemNew.Text = Strings.FactionEditor.New;
        toolStripItemDelete.Text = Strings.FactionEditor.delete;
        toolStripItemCopy.Text = Strings.FactionEditor.copy;
        toolStripItemPaste.Text = Strings.FactionEditor.paste;
        toolStripItemUndo.Text = Strings.FactionEditor.undo;

        grpFactions.Text = Strings.FactionEditor.factions;
        grpGeneral.Text = Strings.FactionEditor.general;
        grpEntry.Text = Strings.FactionEditor.spawnpoint;
        grpRules.Text = Strings.FactionEditor.appearance;

        lblName.Text = Strings.FactionEditor.name;
        lblFolder.Text = Strings.FactionEditor.folderlabel;
        lblEntryMap.Text = Strings.FactionEditor.map;
        lblEntryDir.Text = Strings.FactionEditor.direction;
        btnVisual.Text = Strings.FactionEditor.visual;
        lblColor.Text = Strings.FactionEditor.color;
        btnSelectColor.Text = Strings.FactionEditor.selectcolor;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.FactionEditor.sortalphabetically;
        txtSearch.Text = Strings.FactionEditor.searchplaceholder;

        btnSave.Text = Strings.FactionEditor.save;
        btnCancel.Text = Strings.FactionEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in FactionDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((FactionDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((FactionDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((FactionDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((FactionDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((FactionDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = FactionDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((FactionDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<FactionDescriptor>.Deleted, ((FactionDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.FactionEditor.folderprompt, Strings.FactionEditor.foldertitle, ref folderName,
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
        if (mLoading)
        {
            return;
        }

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
            txtSearch.Text = Strings.FactionEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.FactionEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.FactionEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.FactionEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
