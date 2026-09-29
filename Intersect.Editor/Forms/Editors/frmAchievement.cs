using Intersect.Utilities;
using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Achievements;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Mounts;
using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Models;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors;

public partial class FrmAchievement : EditorForm
{
    private List<AchievementDescriptor> mChanged = new List<AchievementDescriptor>();

    private string mCopiedItem;

    private AchievementDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    private bool mLoading;

    public FrmAchievement()
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
        mEditorItem = AchievementDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Achievement)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<AchievementDescriptor>.Lookup.Values.Contains(mEditorItem))
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
            chkHidden.Checked = mEditorItem.Hidden;

            cmbIcon.SelectedIndex = cmbIcon.FindString(TextUtils.NullToNone(mEditorItem.Icon));
            DrawIcon();

            nudRewardExp.Value = Math.Clamp(mEditorItem.RewardExperience, (int)nudRewardExp.Minimum, (int)nudRewardExp.Maximum);
            cmbRewardItem.SelectedIndex = mEditorItem.RewardItemId == Guid.Empty ? 0 : ItemDescriptor.ListIndex(mEditorItem.RewardItemId) + 1;
            if (cmbRewardItem.SelectedIndex < 0)
            {
                cmbRewardItem.SelectedIndex = 0;
            }

            nudRewardQuantity.Value = Math.Clamp(mEditorItem.RewardItemQuantity, (int)nudRewardQuantity.Minimum, (int)nudRewardQuantity.Maximum);

            cmbRewardTitle.SelectedIndex = mEditorItem.RewardTitleId == Guid.Empty ? 0 : TitleDescriptor.ListIndex(mEditorItem.RewardTitleId) + 1;
            if (cmbRewardTitle.SelectedIndex < 0)
            {
                cmbRewardTitle.SelectedIndex = 0;
            }

            cmbRewardMount.SelectedIndex = mEditorItem.RewardMountId == Guid.Empty ? 0 : MountDescriptor.ListIndex(mEditorItem.RewardMountId) + 1;
            if (cmbRewardMount.SelectedIndex < 0)
            {
                cmbRewardMount.SelectedIndex = 0;
            }

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

    private void DrawIcon()
    {
        var bmp = new Bitmap(picIcon.Width, picIcon.Height);
        var gfx = Graphics.FromImage(bmp);
        gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picIcon.Width, picIcon.Height));
        if (cmbIcon.SelectedIndex > 0 && File.Exists("resources/items/" + cmbIcon.Text))
        {
            var img = Image.FromFile("resources/items/" + cmbIcon.Text);
            gfx.DrawImage(img, new Rectangle(0, 0, picIcon.Width, picIcon.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            img.Dispose();
        }

        gfx.Dispose();
        picIcon.BackgroundImage = bmp;
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

    private void cmbIcon_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.Icon = TextUtils.SanitizeNone(cmbIcon.Text);
        DrawIcon();
    }

    private void chkHidden_CheckedChanged(object sender, EventArgs e)
    {
        if (!mLoading)
        {
            mEditorItem.Hidden = chkHidden.Checked;
        }
    }

    private void nudRewardExp_ValueChanged(object sender, EventArgs e)
    {
        if (!mLoading)
        {
            mEditorItem.RewardExperience = (int)nudRewardExp.Value;
        }
    }

    private void cmbRewardItem_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.RewardItemId = cmbRewardItem.SelectedIndex > 0 ? ItemDescriptor.IdFromList(cmbRewardItem.SelectedIndex - 1) : Guid.Empty;
    }

    private void nudRewardQuantity_ValueChanged(object sender, EventArgs e)
    {
        if (!mLoading)
        {
            mEditorItem.RewardItemQuantity = (int)nudRewardQuantity.Value;
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

    private void cmbRewardMount_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mLoading)
        {
            return;
        }

        mEditorItem.RewardMountId = cmbRewardMount.SelectedIndex > 0 ? MountDescriptor.IdFromList(cmbRewardMount.SelectedIndex - 1) : Guid.Empty;
    }

    #endregion

    private void FrmAchievement_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Achievement);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.AchievementEditor.deleteprompt, Strings.AchievementEditor.delete,
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
                    Strings.AchievementEditor.undoprompt, Strings.AchievementEditor.undotitle,
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

    private void frmAchievement_Load(object sender, EventArgs e)
    {
        cmbIcon.Items.Clear();
        cmbIcon.Items.Add(Strings.General.None);
        cmbIcon.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item));

        cmbRewardItem.Items.Clear();
        cmbRewardItem.Items.Add(Strings.General.None);
        cmbRewardItem.Items.AddRange(ItemDescriptor.Names);

        cmbRewardTitle.Items.Clear();
        cmbRewardTitle.Items.Add(Strings.General.None);
        cmbRewardTitle.Items.AddRange(TitleDescriptor.Names);

        cmbRewardMount.Items.Clear();
        cmbRewardMount.Items.Add(Strings.General.None);
        cmbRewardMount.Items.AddRange(MountDescriptor.Names);

        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.AchievementEditor.title;
        toolStripItemNew.Text = Strings.AchievementEditor.New;
        toolStripItemDelete.Text = Strings.AchievementEditor.delete;
        toolStripItemCopy.Text = Strings.AchievementEditor.copy;
        toolStripItemPaste.Text = Strings.AchievementEditor.paste;
        toolStripItemUndo.Text = Strings.AchievementEditor.undo;

        grpAchievements.Text = Strings.AchievementEditor.achievements;
        grpGeneral.Text = Strings.AchievementEditor.general;
        grpRewards.Text = Strings.AchievementEditor.rewards;

        lblName.Text = Strings.AchievementEditor.name;
        lblFolder.Text = Strings.AchievementEditor.folderlabel;
        lblDescription.Text = Strings.AchievementEditor.description;
        lblIcon.Text = Strings.AchievementEditor.icon;
        chkHidden.Text = Strings.AchievementEditor.hidden;

        lblRewardExp.Text = Strings.AchievementEditor.rewardexp;
        lblRewardItem.Text = Strings.AchievementEditor.rewarditem;
        lblRewardQuantity.Text = Strings.AchievementEditor.rewardquantity;
        lblRewardTitle.Text = Strings.AchievementEditor.rewardtitle;
        lblRewardMount.Text = Strings.AchievementEditor.rewardmount;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.AchievementEditor.sortalphabetically;
        txtSearch.Text = Strings.AchievementEditor.searchplaceholder;

        btnSave.Text = Strings.AchievementEditor.save;
        btnCancel.Text = Strings.AchievementEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in AchievementDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((AchievementDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((AchievementDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((AchievementDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((AchievementDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((AchievementDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = AchievementDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((AchievementDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<AchievementDescriptor>.Deleted, ((AchievementDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.AchievementEditor.folderprompt, Strings.AchievementEditor.foldertitle, ref folderName,
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
            txtSearch.Text = Strings.AchievementEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.AchievementEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.AchievementEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.AchievementEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
