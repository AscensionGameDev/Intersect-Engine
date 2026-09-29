using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Territories;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmTerritory : EditorForm
{

    private List<TerritoryDescriptor> mChanged = new List<TerritoryDescriptor>();

    private string mCopiedItem;

    private TerritoryDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    private bool mLoading;

    public FrmTerritory()
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
        mEditorItem = TerritoryDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Territory)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<TerritoryDescriptor>.Lookup.Values.Contains(mEditorItem))
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
            txtDescription.Text = mEditorItem.Description;

            SelectMap(mEditorItem.MapId);
            nudEntryX.Value = Math.Clamp(mEditorItem.X, nudEntryX.Minimum, nudEntryX.Maximum);
            nudEntryY.Value = Math.Clamp(mEditorItem.Y, nudEntryY.Minimum, nudEntryY.Maximum);

            nudRadius.Value = Math.Clamp(mEditorItem.Radius, nudRadius.Minimum, nudRadius.Maximum);
            nudCaptureTime.Value = Math.Clamp(mEditorItem.CaptureTimeSeconds, nudCaptureTime.Minimum, nudCaptureTime.Maximum);
            nudRewardExp.Value = Math.Clamp(mEditorItem.RewardExperience, nudRewardExp.Minimum, nudRewardExp.Maximum);
            nudRewardGold.Value = Math.Clamp(mEditorItem.RewardGold, nudRewardGold.Minimum, nudRewardGold.Maximum);
            nudRewardTick.Value = Math.Clamp(mEditorItem.RewardTickSeconds, nudRewardTick.Minimum, nudRewardTick.Maximum);
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

    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
        if (!mLoading)
        {
            mEditorItem.Description = txtDescription.Text;
        }
    }

    private void cmbEntryMap_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!mLoading && cmbEntryMap.SelectedIndex > -1)
        {
            mEditorItem.MapId = MapList.OrderedMaps[cmbEntryMap.SelectedIndex].MapId;
        }
    }

    private void nudEntry_ValueChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.X = (int)nudEntryX.Value;
        mEditorItem.Y = (int)nudEntryY.Value;
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

    private void nudRules_ValueChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.Radius = (int)nudRadius.Value;
        mEditorItem.CaptureTimeSeconds = (int)nudCaptureTime.Value;
        mEditorItem.RewardExperience = (int)nudRewardExp.Value;
        mEditorItem.RewardGold = (int)nudRewardGold.Value;
        mEditorItem.RewardTickSeconds = (int)nudRewardTick.Value;
    }

    #endregion

    private void FrmTerritory_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Territory);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.TerritoryEditor.deleteprompt, Strings.TerritoryEditor.delete,
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
                    Strings.TerritoryEditor.undoprompt, Strings.TerritoryEditor.undotitle,
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
        Text = Strings.TerritoryEditor.title;
        toolStripItemNew.Text = Strings.TerritoryEditor.New;
        toolStripItemDelete.Text = Strings.TerritoryEditor.delete;
        toolStripItemCopy.Text = Strings.TerritoryEditor.copy;
        toolStripItemPaste.Text = Strings.TerritoryEditor.paste;
        toolStripItemUndo.Text = Strings.TerritoryEditor.undo;

        grpTerritories.Text = Strings.TerritoryEditor.territories;
        grpGeneral.Text = Strings.TerritoryEditor.general;
        grpEntry.Text = Strings.TerritoryEditor.capturepoint;
        grpRules.Text = Strings.TerritoryEditor.capturesettings;

        lblName.Text = Strings.TerritoryEditor.name;
        lblFolder.Text = Strings.TerritoryEditor.folderlabel;
        lblDescription.Text = Strings.TerritoryEditor.description;
        lblEntryMap.Text = Strings.TerritoryEditor.map;
        btnVisual.Text = Strings.TerritoryEditor.visual;
        lblRadius.Text = Strings.TerritoryEditor.radius;
        lblCaptureTime.Text = Strings.TerritoryEditor.capturetime;
        lblRewardExp.Text = Strings.TerritoryEditor.rewardexp;
        lblRewardGold.Text = Strings.TerritoryEditor.rewardgold;
        lblRewardTick.Text = Strings.TerritoryEditor.rewardtick;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.TerritoryEditor.sortalphabetically;
        txtSearch.Text = Strings.TerritoryEditor.searchplaceholder;

        btnSave.Text = Strings.TerritoryEditor.save;
        btnCancel.Text = Strings.TerritoryEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in TerritoryDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((TerritoryDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((TerritoryDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((TerritoryDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((TerritoryDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((TerritoryDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = TerritoryDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((TerritoryDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<TerritoryDescriptor>.Deleted, ((TerritoryDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.TerritoryEditor.folderprompt, Strings.TerritoryEditor.foldertitle, ref folderName,
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
            txtSearch.Text = Strings.TerritoryEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.TerritoryEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.TerritoryEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.TerritoryEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
