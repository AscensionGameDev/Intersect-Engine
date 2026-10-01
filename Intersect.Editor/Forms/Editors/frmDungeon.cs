using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Framework.Core.GameObjects.Achievements;
using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.NPCs;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmDungeon : EditorForm
{

    private List<DungeonDescriptor> mChanged = new List<DungeonDescriptor>();

    private string mCopiedItem;

    private DungeonDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    private bool mLoading;

    public FrmDungeon()
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
        mEditorItem = DungeonDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Dungeon)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<DungeonDescriptor>.Lookup.Values.Contains(mEditorItem))
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

            SelectMap(mEditorItem.EntryMapId);
            nudEntryX.Value = Math.Clamp(mEditorItem.EntryX, nudEntryX.Minimum, nudEntryX.Maximum);
            nudEntryY.Value = Math.Clamp(mEditorItem.EntryY, nudEntryY.Minimum, nudEntryY.Maximum);
            if (cmbEntryDir.Items.Count > 0)
            {
                cmbEntryDir.SelectedIndex = Math.Clamp((int)mEditorItem.EntryDirection, 0, cmbEntryDir.Items.Count - 1);
            }

            if (cmbAccess.Items.Count > 0)
            {
                cmbAccess.SelectedIndex = Math.Clamp((int)mEditorItem.AccessType, 0, cmbAccess.Items.Count - 1);
            }
            nudMinPlayers.Value = Math.Clamp(mEditorItem.MinPlayers, nudMinPlayers.Minimum, nudMinPlayers.Maximum);
            nudMaxPlayers.Value = Math.Clamp(mEditorItem.MaxPlayers, nudMaxPlayers.Minimum, nudMaxPlayers.Maximum);
            nudLevel.Value = Math.Clamp(mEditorItem.RequiredLevel, nudLevel.Minimum, nudLevel.Maximum);
            nudTimeLimit.Value = Math.Clamp(mEditorItem.TimeLimitSeconds, nudTimeLimit.Minimum, nudTimeLimit.Maximum);
            nudLives.Value = Math.Clamp(mEditorItem.Lives, nudLives.Minimum, nudLives.Maximum);
            nudCooldown.Value = Math.Clamp(mEditorItem.CooldownSeconds, nudCooldown.Minimum, nudCooldown.Maximum);
            nudExitDelay.Value = Math.Clamp(mEditorItem.ExitDelaySeconds, nudExitDelay.Minimum, nudExitDelay.Maximum);

            nudRewardExp.Value = Math.Clamp(mEditorItem.RewardExperience, nudRewardExp.Minimum, nudRewardExp.Maximum);
            cmbRewardTitle.SelectedIndex = mEditorItem.RewardTitleId == Guid.Empty ? 0 : TitleDescriptor.ListIndex(mEditorItem.RewardTitleId) + 1;
            if (cmbRewardTitle.SelectedIndex < 0)
            {
                cmbRewardTitle.SelectedIndex = 0;
            }

            cmbRewardAchievement.SelectedIndex = mEditorItem.RewardAchievementId == Guid.Empty ? 0 : AchievementDescriptor.ListIndex(mEditorItem.RewardAchievementId) + 1;
            if (cmbRewardAchievement.SelectedIndex < 0)
            {
                cmbRewardAchievement.SelectedIndex = 0;
            }

            UpdateRewardList();
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

    private void UpdateRewardList()
    {
        lstRewards.Items.Clear();
        foreach (var reward in mEditorItem.Rewards)
        {
            lstRewards.Items.Add(Strings.DungeonEditor.rewardentry.ToString(ItemDescriptor.GetName(reward.ItemId), reward.Quantity));
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
            mEditorItem.EntryMapId = MapList.OrderedMaps[cmbEntryMap.SelectedIndex].MapId;
        }
    }

    private void nudEntry_ValueChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.EntryX = (int)nudEntryX.Value;
        mEditorItem.EntryY = (int)nudEntryY.Value;
    }

    private void cmbEntryDir_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!mLoading && cmbEntryDir.SelectedIndex > -1)
        {
            mEditorItem.EntryDirection = (Direction)cmbEntryDir.SelectedIndex;
        }
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

    private void cmbAccess_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!mLoading && cmbAccess.SelectedIndex > -1)
        {
            mEditorItem.AccessType = (DungeonAccessType)cmbAccess.SelectedIndex;
        }
    }

    private void nudRules_ValueChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.MinPlayers = (int)nudMinPlayers.Value;
        mEditorItem.MaxPlayers = (int)nudMaxPlayers.Value;
        mEditorItem.RequiredLevel = (int)nudLevel.Value;
        mEditorItem.TimeLimitSeconds = (int)nudTimeLimit.Value;
        mEditorItem.Lives = (int)nudLives.Value;
        mEditorItem.CooldownSeconds = (int)nudCooldown.Value;
        mEditorItem.ExitDelaySeconds = (int)nudExitDelay.Value;
    }

    private void btnAddReward_Click(object sender, EventArgs e)
    {
        if (cmbRewardItem.SelectedIndex < 0)
        {
            return;
        }

        var itemId = ItemDescriptor.IdFromList(cmbRewardItem.SelectedIndex);
        if (itemId == Guid.Empty)
        {
            return;
        }

        mEditorItem.Rewards.Add(new DungeonReward(itemId, (int)nudRewardQuantity.Value));
        UpdateRewardList();
    }

    private void btnRemoveReward_Click(object sender, EventArgs e)
    {
        if (lstRewards.SelectedIndex > -1 && lstRewards.SelectedIndex < mEditorItem.Rewards.Count)
        {
            mEditorItem.Rewards.RemoveAt(lstRewards.SelectedIndex);
            UpdateRewardList();
        }
    }

    private void nudRewardExp_ValueChanged(object sender, EventArgs e)
    {
        if (!mLoading)
        {
            mEditorItem.RewardExperience = (long)nudRewardExp.Value;
        }
    }

    private void cmbRewardTitle_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.RewardTitleId = cmbRewardTitle.SelectedIndex > 0 ? TitleDescriptor.IdFromList(cmbRewardTitle.SelectedIndex - 1) : Guid.Empty;
    }

    private void cmbRewardAchievement_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.RewardAchievementId = cmbRewardAchievement.SelectedIndex > 0 ? AchievementDescriptor.IdFromList(cmbRewardAchievement.SelectedIndex - 1) : Guid.Empty;
    }

    #endregion

    private void FrmDungeon_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Dungeon);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.DungeonEditor.deleteprompt, Strings.DungeonEditor.delete,
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
                    Strings.DungeonEditor.undoprompt, Strings.DungeonEditor.undotitle,
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

        cmbEntryDir.Items.Clear();
        cmbEntryDir.Items.AddRange(new object[] { Strings.Direction.dir[Direction.Up], Strings.Direction.dir[Direction.Down], Strings.Direction.dir[Direction.Left], Strings.Direction.dir[Direction.Right] });

        cmbAccess.Items.Clear();
        cmbAccess.Items.AddRange(new object[] { Strings.DungeonEditor.accesssolo, Strings.DungeonEditor.accessparty, Strings.DungeonEditor.accessguild });

        cmbRewardItem.Items.Clear();
        cmbRewardItem.Items.AddRange(ItemDescriptor.Names);

        cmbRewardTitle.Items.Clear();
        cmbRewardTitle.Items.Add(Strings.General.None);
        cmbRewardTitle.Items.AddRange(TitleDescriptor.Names);

        cmbRewardAchievement.Items.Clear();
        cmbRewardAchievement.Items.Add(Strings.General.None);
        cmbRewardAchievement.Items.AddRange(AchievementDescriptor.Names);

        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.DungeonEditor.title;
        toolStripItemNew.Text = Strings.DungeonEditor.New;
        toolStripItemDelete.Text = Strings.DungeonEditor.delete;
        toolStripItemCopy.Text = Strings.DungeonEditor.copy;
        toolStripItemPaste.Text = Strings.DungeonEditor.paste;
        toolStripItemUndo.Text = Strings.DungeonEditor.undo;

        grpDungeons.Text = Strings.DungeonEditor.dungeons;
        grpGeneral.Text = Strings.DungeonEditor.general;
        grpEntry.Text = Strings.DungeonEditor.entrance;
        grpRules.Text = Strings.DungeonEditor.rules;
        grpRewards.Text = Strings.DungeonEditor.rewards;

        lblName.Text = Strings.DungeonEditor.name;
        lblFolder.Text = Strings.DungeonEditor.folderlabel;
        lblDescription.Text = Strings.DungeonEditor.description;
        lblEntryMap.Text = Strings.DungeonEditor.map;
        lblEntryDir.Text = Strings.DungeonEditor.direction;
        btnVisual.Text = Strings.DungeonEditor.visual;
        lblAccess.Text = Strings.DungeonEditor.access;
        lblMinPlayers.Text = Strings.DungeonEditor.minplayers;
        lblMaxPlayers.Text = Strings.DungeonEditor.maxplayers;
        lblLevel.Text = Strings.DungeonEditor.level;
        lblTimeLimit.Text = Strings.DungeonEditor.timelimit;
        lblLives.Text = Strings.DungeonEditor.lives;
        lblCooldown.Text = Strings.DungeonEditor.cooldown;
        lblExitDelay.Text = Strings.DungeonEditor.exitdelay;
        lblRewardItem.Text = Strings.DungeonEditor.rewarditem;
        lblRewardQuantity.Text = Strings.DungeonEditor.rewardquantity;
        btnAddReward.Text = Strings.DungeonEditor.addreward;
        btnRemoveReward.Text = Strings.DungeonEditor.removereward;
        lblRewardExp.Text = Strings.DungeonEditor.rewardexp;
        lblRewardTitle.Text = Strings.DungeonEditor.rewardtitle;
        lblRewardAchievement.Text = Strings.DungeonEditor.rewardachievement;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.DungeonEditor.sortalphabetically;
        txtSearch.Text = Strings.DungeonEditor.searchplaceholder;

        btnSave.Text = Strings.DungeonEditor.save;
        btnCancel.Text = Strings.DungeonEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in DungeonDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((DungeonDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((DungeonDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((DungeonDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((DungeonDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((DungeonDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = DungeonDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((DungeonDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<DungeonDescriptor>.Deleted, ((DungeonDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.DungeonEditor.folderprompt, Strings.DungeonEditor.foldertitle, ref folderName,
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
            txtSearch.Text = Strings.DungeonEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.DungeonEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.DungeonEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.DungeonEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
