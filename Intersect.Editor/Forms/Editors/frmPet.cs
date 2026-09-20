using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Pets;
using Intersect.Models;
using Intersect.Utilities;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors;


public partial class FrmPet : EditorForm
{

    private List<PetDescriptor> mChanged = new List<PetDescriptor>();

    private string mCopiedItem;

    private PetDescriptor mEditorItem;

    private List<string> mKnownFolders = new List<string>();

    public FrmPet()
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
        mEditorItem = PetDescriptor.Get(id);
        UpdateEditor();
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Pet)
        {
            InitEditor();
            if (mEditorItem != null && !DatabaseObject<PetDescriptor>.Lookup.Values.Contains(mEditorItem))
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
            nudSpeed.Value = Math.Clamp(mEditorItem.Speed, nudSpeed.Minimum, nudSpeed.Maximum);
            nudPickupRange.Value = Math.Clamp(mEditorItem.PickupRange, nudPickupRange.Minimum, nudPickupRange.Maximum);
            chkPickupOwnerDrops.Checked = mEditorItem.PickupOwnerDrops;
            chkPickupUnowned.Checked = mEditorItem.PickupUnownedItems;
            DrawPetSprite();

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

    private void DrawPetSprite()
    {
        var picSpriteBmp = new Bitmap(picPet.Width, picPet.Height);
        var gfx = Graphics.FromImage(picSpriteBmp);
        gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picPet.Width, picPet.Height));
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

        picPet.BackgroundImage = picSpriteBmp;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        mEditorItem.Name = txtName.Text;
        lstGameObjects.UpdateText(txtName.Text);
    }

    private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
    {
        mEditorItem.Sprite = TextUtils.SanitizeNone(cmbSprite.Text);
        DrawPetSprite();
    }

    private void nudSpeed_ValueChanged(object sender, EventArgs e)
    {
        mEditorItem.Speed = (int)nudSpeed.Value;
    }

    private void nudPickupRange_ValueChanged(object sender, EventArgs e)
    {
        mEditorItem.PickupRange = (int)nudPickupRange.Value;
    }

    private void chkPickupOwnerDrops_CheckedChanged(object sender, EventArgs e)
    {
        mEditorItem.PickupOwnerDrops = chkPickupOwnerDrops.Checked;
    }

    private void chkPickupUnowned_CheckedChanged(object sender, EventArgs e)
    {
        mEditorItem.PickupUnownedItems = chkPickupUnowned.Checked;
    }

    private void FrmPet_FormClosed(object sender, FormClosedEventArgs e)
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
        PacketSender.SendCreateObject(GameObjectType.Pet);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (mEditorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.PetEditor.deleteprompt, Strings.PetEditor.delete,
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
                    Strings.PetEditor.undoprompt, Strings.PetEditor.undotitle,
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

    private void frmPet_Load(object sender, EventArgs e)
    {
        cmbSprite.Items.Clear();
        cmbSprite.Items.Add(Strings.General.None);
        cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));

        UpdateEditor();
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.PetEditor.title;
        toolStripItemNew.Text = Strings.PetEditor.New;
        toolStripItemDelete.Text = Strings.PetEditor.delete;
        toolStripItemCopy.Text = Strings.PetEditor.copy;
        toolStripItemPaste.Text = Strings.PetEditor.paste;
        toolStripItemUndo.Text = Strings.PetEditor.undo;

        grpPets.Text = Strings.PetEditor.pets;
        grpGeneral.Text = Strings.PetEditor.general;
        grpBehavior.Text = Strings.PetEditor.behavior;
        lblName.Text = Strings.PetEditor.name;
        lblSprite.Text = Strings.PetEditor.sprite;
        lblSpeed.Text = Strings.PetEditor.speed;
        lblPickupRange.Text = Strings.PetEditor.pickuprange;
        chkPickupOwnerDrops.Text = Strings.PetEditor.pickupownerdrops;
        chkPickupUnowned.Text = Strings.PetEditor.pickupunowned;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.PetEditor.sortalphabetically;
        txtSearch.Text = Strings.PetEditor.searchplaceholder;
        lblFolder.Text = Strings.PetEditor.folderlabel;

        btnSave.Text = Strings.PetEditor.save;
        btnCancel.Text = Strings.PetEditor.cancel;
    }

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in PetDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((PetDescriptor) itm.Value).Folder) &&
                !mFolders.Contains(((PetDescriptor) itm.Value).Folder))
            {
                mFolders.Add(((PetDescriptor) itm.Value).Folder);
                if (!mKnownFolders.Contains(((PetDescriptor) itm.Value).Folder))
                {
                    mKnownFolders.Add(((PetDescriptor) itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        mKnownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(mKnownFolders.ToArray());

        var items = PetDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((PetDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<PetDescriptor>.Deleted, ((PetDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.PetEditor.folderprompt, Strings.PetEditor.foldertitle, ref folderName,
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
            txtSearch.Text = Strings.PetEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.PetEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.PetEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.PetEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
