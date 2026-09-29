using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Mounts;
using Intersect.Models;
using Intersect.Utilities;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmMount : EditorForm
{

    private List<MountDescriptor> mChanged = new List<MountDescriptor>();

    private string mCopiedItem;

    private MountDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    public FrmMount()
    {
        ApplyHooks();
        InitializeComponent();
        Icon = Program.Icon;
        _btnSave = btnSave;
        _btnCancel = btnCancel;

        lblPickupRange.Visible = false;
        nudPickupRange.Visible = false;
        chkPickupOwnerDrops.Visible = false;
        chkPickupUnowned.Visible = false;
        grpBehavior.Height = 50;

        lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
    }

    private void AssignEditorItem(Guid id)
    {
        mEditorItem = MountDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Mount)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<MountDescriptor>.Lookup.Values.Contains(mEditorItem))
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
            cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Sprite));
            nudSpeed.Value = Math.Clamp(mEditorItem.SpeedBonus, nudSpeed.Minimum, nudSpeed.Maximum);
            DrawMountSprite();

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

    private void DrawMountSprite()
    {
        var picSpriteBmp = new Bitmap(picMount.Width, picMount.Height);
        var gfx = Graphics.FromImage(picSpriteBmp);
        gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picMount.Width, picMount.Height));
        if (cmbSprite.SelectedIndex > 0 && File.Exists("resources/entities/" + cmbSprite.Text))
        {
            var img = Image.FromFile("resources/entities/" + cmbSprite.Text);
            var frameWidth = img.Width / Options.Instance.Sprites.NormalFrames;
            var frameHeight = img.Height / Options.Instance.Sprites.Directions;
            gfx.DrawImage(
                img, new Rectangle(0, 0, frameWidth, frameHeight),
                new Rectangle(0, 0, frameWidth, frameHeight), GraphicsUnit.Pixel
            );

            img.Dispose();
        }

        gfx.Dispose();

        picMount.BackgroundImage = picSpriteBmp;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        mEditorItem.Name = txtName.Text;
        lstGameObjects.UpdateText(txtName.Text);
    }

    private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
    {
        mEditorItem.Sprite = TextUtils.SanitizeNone(cmbSprite.Text);
        DrawMountSprite();
    }

    private void nudSpeed_ValueChanged(object sender, EventArgs e)
    {
        mEditorItem.SpeedBonus = (int)nudSpeed.Value;
    }

    private void FrmMount_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Mount);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.MountEditor.deleteprompt, Strings.MountEditor.delete,
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
                    Strings.MountEditor.undoprompt, Strings.MountEditor.undotitle,
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

    private void frmMount_Load(object sender, EventArgs e)
    {
        cmbSprite.Items.Clear();
        cmbSprite.Items.Add(Strings.General.None);
        cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));

        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.MountEditor.title;
        toolStripItemNew.Text = Strings.MountEditor.New;
        toolStripItemDelete.Text = Strings.MountEditor.delete;
        toolStripItemCopy.Text = Strings.MountEditor.copy;
        toolStripItemPaste.Text = Strings.MountEditor.paste;
        toolStripItemUndo.Text = Strings.MountEditor.undo;

        grpMounts.Text = Strings.MountEditor.mounts;
        grpGeneral.Text = Strings.MountEditor.general;
        grpBehavior.Text = Strings.MountEditor.behavior;
        lblName.Text = Strings.MountEditor.name;
        lblSprite.Text = Strings.MountEditor.sprite;
        lblSpeed.Text = Strings.MountEditor.speed;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.MountEditor.sortalphabetically;
        txtSearch.Text = Strings.MountEditor.searchplaceholder;
        lblFolder.Text = Strings.MountEditor.folderlabel;

        btnSave.Text = Strings.MountEditor.save;
        btnCancel.Text = Strings.MountEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in MountDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((MountDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((MountDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((MountDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((MountDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((MountDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = MountDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((MountDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<MountDescriptor>.Deleted, ((MountDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.MountEditor.folderprompt, Strings.MountEditor.foldertitle, ref folderName,
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
            txtSearch.Text = Strings.MountEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.MountEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.MountEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.MountEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
