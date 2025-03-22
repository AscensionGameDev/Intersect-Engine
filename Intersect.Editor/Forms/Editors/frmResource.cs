using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Utilities;
using EventDescriptor = Intersect.Framework.Core.GameObjects.Events.EventDescriptor;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors;

public partial class FrmResource : EditorForm
{
    private readonly List<ResourceDescriptor> _changed = [];
    private string? _copiedItem;
    private ResourceDescriptor? _editorItem;
    private Bitmap? _graphicBitmap;
    private Bitmap? _resourceGraphic;
    private bool _mouseDown;
    private readonly List<string> _knownFolders = [];
    private readonly BindingList<NotifiableDrop> _dropList = [];

    public FrmResource()
    {
        ApplyHooks();
        InitializeComponent();
        Icon = Program.Icon;

        _btnSave = btnSave;
        _btnCancel = btnCancel;
        cmbToolType.Items.Clear();
        cmbToolType.Items.Add(Strings.General.None);
        cmbToolType.Items.AddRange(Options.Instance.Equipment.ToolTypes.ToArray());
        cmbEvent.Items.Clear();
        cmbEvent.Items.Add(Strings.General.None);
        cmbEvent.Items.AddRange(EventDescriptor.Names);

        lstDrops.DataSource = _dropList;
        lstDrops.DisplayMember = nameof(NotifiableDrop.DisplayName);

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
        if (!ResourceDescriptor.TryGet(id, out _editorItem))
        {
            _editorItem = null;
        }

        UpdateEditor();
    }

    private void toolStripItemNew_Click(object sender, EventArgs e)
    {
        PacketSender.SendCreateObject(GameObjectType.Resource);
    }

    private void toolStripItemDelete_Click(object sender, EventArgs e)
    {
        if (_editorItem != null && lstGameObjects.Focused)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.ResourceEditor.deleteprompt, Strings.ResourceEditor.deletetitle, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
            {
                PacketSender.SendDeleteObject(_editorItem);
            }
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
        if (_editorItem != null && _changed.Contains(_editorItem) && _editorItem != null)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.ResourceEditor.undoprompt, Strings.ResourceEditor.undotitle, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }
    }

    private void UpdateToolStripItems()
    {
        toolStripItemCopy.Enabled = _editorItem != null && lstGameObjects.Focused;
        toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstGameObjects.Focused;
        toolStripItemDelete.Enabled = _editorItem != null && lstGameObjects.Focused;
        toolStripItemUndo.Enabled = _editorItem != null && lstGameObjects.Focused;
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Resource)
        {
            InitEditor();
            if (_editorItem != null && !ResourceDescriptor.Lookup.Values.Contains(_editorItem))
            {
                _editorItem = null;
                UpdateEditor();
            }
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        foreach (var item in _changed)
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
        foreach (var item in _changed)
        {
            item.StatesGraphics = item.StatesGraphics
                .OrderBy(s => s.Value.MinimumHealth)
                .ThenBy(s => s.Value.MaximumHealth)
                .ToDictionary(p => p.Key, p => p.Value);

            PacketSender.SendSaveObject(item);
            item.DeleteBackup();
        }

        Hide();
        Globals.CurrentEditor = -1;
        Dispose();
    }

    #endregion

    #region Form Events

    private void frmResource_Load(object sender, EventArgs e)
    {
        _graphicBitmap = new Bitmap(picResource.Width, picResource.Height);

        cmbDeathAnimation.Items.Clear();
        cmbDeathAnimation.Items.Add(Strings.General.None);
        cmbDeathAnimation.Items.AddRange(AnimationDescriptor.Names);
        cmbDropItem.Items.Clear();
        cmbDropItem.Items.Add(Strings.General.None);
        cmbDropItem.Items.AddRange(ItemDescriptor.Names);
        InitLocalization();
        UpdateEditor();
    }

    private void frmResource_FormClosed(object sender, FormClosedEventArgs e)
    {
        Globals.CurrentEditor = -1;
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
        Text = Strings.ResourceEditor.title;
        toolStripItemNew.Text = Strings.ResourceEditor.New;
        toolStripItemDelete.Text = Strings.ResourceEditor.delete;
        toolStripItemCopy.Text = Strings.ResourceEditor.copy;
        toolStripItemPaste.Text = Strings.ResourceEditor.paste;
        toolStripItemUndo.Text = Strings.ResourceEditor.undo;

        grpResources.Text = Strings.ResourceEditor.resources;

        grpGeneral.Text = Strings.ResourceEditor.general;
        lblName.Text = Strings.ResourceEditor.name;
        lblToolType.Text = Strings.ResourceEditor.tooltype;
        lblHP.Text = Strings.ResourceEditor.minhp;
        lblMaxHp.Text = Strings.ResourceEditor.maxhp;
        lblSpawnDuration.Text = Strings.ResourceEditor.spawnduration;
        lblDeathAnimation.Text = Strings.ResourceEditor.DeathAnimation;
        chkWalkableBefore.Text = Strings.ResourceEditor.walkablebefore;
        chkWalkableAfter.Text = Strings.ResourceEditor.walkableafter;
        chkUseExplicitMaxHealthForResourceStates.Text = Strings.ResourceEditor.UseExplicitMaxHealthForResourceStates;

        grpDrops.Text = Strings.ResourceEditor.drops;
        lblDropItem.Text = Strings.ResourceEditor.dropitem;
        lblDropMaxAmount.Text = Strings.ResourceEditor.DropMaxAmount;
        lblDropMinAmount.Text = Strings.ResourceEditor.DropMinAmount;
        lblDropChance.Text = Strings.ResourceEditor.dropchance;
        btnDropAdd.Text = Strings.ResourceEditor.dropadd;
        btnDropRemove.Text = Strings.ResourceEditor.dropremove;

        grpRegen.Text = Strings.ResourceEditor.regen;
        lblHpRegen.Text = Strings.ResourceEditor.hpregen;
        new ToolTip().SetToolTip(grpRegen, Strings.ResourceEditor.regenhint);

        grpGraphics.Text = Strings.ResourceEditor.Appearance;
        lblStates.Text = Strings.ResourceEditor.StatesLabel;
        lblStateName.Text = Strings.ResourceEditor.StateName;
        btnAddState.Text = Strings.ResourceEditor.AddState;
        btnRemoveState.Text = Strings.ResourceEditor.RemoveState;
        grpGraphicData.Text = Strings.ResourceEditor.StateProperties;
        lblTextureType.Text = Strings.ResourceEditor.TextureSource;
        cmbTextureType.Items.Clear();
        cmbTextureType.Items.AddRange([.. Strings.ResourceEditor.TextureSources.Values]);
        chkRenderBelowEntity.Text = Strings.ResourceEditor.BelowEntities;
        lblTerxtureSource.Text = Strings.ResourceEditor.TextureName;
        lblAnimation.Text = Strings.ResourceEditor.Animation;

        grpCommonEvent.Text = Strings.ResourceEditor.commonevent;
        lblEvent.Text = Strings.ResourceEditor.harvestevent;

        grpRequirements.Text = Strings.ResourceEditor.requirementsgroup;
        lblCannotHarvest.Text = Strings.ResourceEditor.cannotharvest;
        btnRequirements.Text = Strings.ResourceEditor.requirements;

        //Searching/Sorting
        btnAlphabetical.ToolTipText = Strings.ResourceEditor.sortalphabetically;
        txtSearch.Text = Strings.ResourceEditor.searchplaceholder;
        lblFolder.Text = Strings.ResourceEditor.folderlabel;

        btnSave.Text = Strings.ResourceEditor.save;
        btnCancel.Text = Strings.ResourceEditor.cancel;
    }

    private void UpdateEditor()
    {
        if (_editorItem != null)
        {
            pnlContainer.Show();

            txtName.Text = _editorItem.Name;
            cmbFolder.Text = _editorItem.Folder;
            cmbToolType.SelectedIndex = _editorItem.Tool + 1;
            nudSpawnDuration.Value = _editorItem.SpawnDuration;
            cmbDeathAnimation.SelectedIndex = AnimationDescriptor.ListIndex(_editorItem.AnimationId) + 1;
            nudMinHp.Value = _editorItem.MinHp;
            nudMaxHp.Value = _editorItem.MaxHp;
            chkWalkableBefore.Checked = _editorItem.WalkableBefore;
            chkWalkableAfter.Checked = _editorItem.WalkableAfter;
            chkUseExplicitMaxHealthForResourceStates.Checked = _editorItem.UseExplicitMaxHealthForResourceStates;
            cmbEvent.SelectedIndex = EventDescriptor.ListIndex(_editorItem.EventId) + 1;
            txtCannotHarvest.Text = _editorItem.CannotHarvestMessage;
            nudHpRegen.Value = _editorItem.VitalRegen;

            picResource.Hide();

            _editorItem.StatesGraphics = _editorItem.StatesGraphics
                .OrderBy(s => s.Value.MinimumHealth)
                .ThenBy(s => s.Value.MaximumHealth)
                .ToDictionary(p => p.Key, p => p.Value);

            lstStates.Items.Clear();
            foreach (var state in _editorItem.StatesGraphics.Values)
            {
                lstStates.Items.Add(state.Name);
            }

            if (lstStates.Items.Count > 0)
            {
                lstStates.SelectedIndex = 0;
            }
            else
            {
                txtStateName.Text = string.Empty;
                cmbTextureType.SelectedIndex = (int)ResourceTextureSource.Resource;
                chkRenderBelowEntity.Checked = false;
                cmbTextureSource.Items.Clear();
                cmbAnimation.Items.Clear();
                nudStateRangeMin.Value = 0;
                nudStateRangeMax.Value = 0;
                picResource.Hide();
            }

            UpdateDropValues();

            if (_changed.IndexOf(_editorItem) == -1)
            {
                _changed.Add(_editorItem);
                _editorItem.MakeBackup();
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

    #endregion

    #region Helpers

    private bool TryGetCurrentState([NotNullWhen(true)] out ResourceStateDescriptor? state)
    {
        state = null;

        if (_editorItem is null)
        {
            return false;
        }

        if (lstStates.SelectedIndex < 0)
        {
            return false;
        }

        var selectedIndex = lstStates.SelectedIndex;
        var stateKey = _editorItem.StatesGraphics.Keys.ToList()[selectedIndex];
        if (!_editorItem.StatesGraphics.TryGetValue(stateKey, out state))
        {
            return false;
        }

        return true;
    }

    private void UpdateDropValues()
    {
        if (_editorItem is null)
        {
            return;
        }

        _dropList.Clear();
        foreach (var drop in _editorItem.Drops)
        {
            _dropList.Add(new NotifiableDrop
            {
                ItemId = drop.ItemId,
                MinQuantity = drop.MinQuantity,
                MaxQuantity = drop.MaxQuantity,
                Chance = drop.Chance
            });
        }
    }

    private void UpdateStateControls()
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        cmbTextureType.SelectedIndex = (int)currentState.TextureType;
        chkRenderBelowEntity.Checked = currentState.RenderBelowEntities;
        nudStateRangeMin.Value = currentState.MinimumHealth;
        nudStateRangeMax.Value = currentState.MaximumHealth;
        UpdateGraphicFileControl(currentState);
    }

    private void UpdateGraphicFileControl(ResourceStateDescriptor currentState)
    {
        cmbTextureSource.Items.Clear();
        cmbTextureSource.Items.Add(Strings.General.None);
        cmbAnimation.Items.Clear();
        cmbAnimation.Items.Add(Strings.General.None);

        if (currentState.TextureType == ResourceTextureSource.Animation)
        {
            cmbTextureSource.Enabled = false;
            picResource.Visible = false;
            cmbAnimation.Enabled = true;
            cmbAnimation.Items.AddRange(AnimationDescriptor.Names);
            cmbAnimation.SelectedIndex = AnimationDescriptor.ListIndex(currentState.AnimationId) + 1;
            return;
        }

        cmbTextureSource.Enabled = true;
        picResource.Visible = true;
        cmbAnimation.Enabled = false;

        var resources = GameContentManager.GetSmartSortedTextureNames(
            currentState.TextureType == ResourceTextureSource.Tileset
                ? GameContentManager.TextureType.Tileset
                : GameContentManager.TextureType.Resource
        );

        cmbTextureSource.Items.AddRange(resources);

        if (_editorItem is null)
        {
            return;
        }

        if (currentState.Texture != null && cmbTextureSource.Items.Contains(currentState.Texture))
        {
            cmbTextureSource.SelectedIndex = cmbTextureSource.FindString(TextUtils.NullToNone(currentState.Texture));
            return;
        }

        cmbTextureSource.SelectedIndex = 0;
    }

    private void Render(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (_graphicBitmap is null)
        {
            return;
        }

        if (_resourceGraphic is null)
        {
            return;
        }

        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        if (currentState.Texture is null)
        {
            return;
        }

        if (currentState.TextureType == ResourceTextureSource.Animation)
        {
            return;
        }

        var gfx = Graphics.FromImage(_graphicBitmap);
        gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picResource.Width, picResource.Height));

        if (cmbTextureSource.SelectedIndex > 0 && cmbTextureSource.SelectedIndex > 0)
        {
            gfx.DrawImage(
                _resourceGraphic,
                new Rectangle(0, 0, _resourceGraphic.Width, _resourceGraphic.Height),
                new Rectangle(0, 0, _resourceGraphic.Width, _resourceGraphic.Height),
                GraphicsUnit.Pixel
            );
        }

        if (currentState.TextureType == ResourceTextureSource.Tileset)
        {
            var selX = currentState.X;
            var selY = currentState.Y;
            var selW = currentState.Width;
            var selH = currentState.Height;

            if (selW < 0)
            {
                selX -= Math.Abs(selW);
                selW = Math.Abs(selW);
            }

            if (selH < 0)
            {
                selY -= Math.Abs(selH);
                selH = Math.Abs(selH);
            }

            var tileWidth = Options.Instance.Map.TileWidth;
            var tileHeight = Options.Instance.Map.TileHeight;

            gfx.DrawRectangle(
                new Pen(System.Drawing.Color.White, 2f),
                new Rectangle(
                    selX * tileWidth,
                    selY * tileHeight,
                    tileWidth + selW * tileWidth,
                    tileHeight + selH * tileHeight
                )
            );
        }

        gfx.Dispose();
        gfx = picResource.CreateGraphics();
        gfx.DrawImageUnscaled(_graphicBitmap, new System.Drawing.Point(0, 0));
        gfx.Dispose();
    }

    #endregion

    #region Form Events

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.Name = txtName.Text;
        lstGameObjects.UpdateText(txtName.Text);
    }

    private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.Tool = cmbToolType.SelectedIndex - 1;
    }

    private void nudMinHp_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.MinHp = (int)nudMinHp.Value;
    }

    private void nudMaxHp_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.MaxHp = (int)nudMaxHp.Value;
    }

    private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.SpawnDuration = (int)nudSpawnDuration.Value;
    }

    private void cmbDeathAnimation_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.Animation = AnimationDescriptor.Get(AnimationDescriptor.IdFromList(cmbDeathAnimation.SelectedIndex - 1));
    }

    private void chkWalkableBefore_CheckedChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.WalkableBefore = chkWalkableBefore.Checked;
    }

    private void chkWalkableAfter_CheckedChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.WalkableAfter = chkWalkableAfter.Checked;
    }

    private void chkUseExplicitMaxHealthForResourceStates_CheckedChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.UseExplicitMaxHealthForResourceStates = chkUseExplicitMaxHealthForResourceStates.Checked;
    }

    private void lstDrops_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstDrops.SelectedIndex > -1)
        {
            cmbDropItem.SelectedIndex = ItemDescriptor.ListIndex(_editorItem.Drops[lstDrops.SelectedIndex].ItemId) + 1;
            nudDropMaxAmount.Value = _editorItem.Drops[lstDrops.SelectedIndex].MaxQuantity;
            nudDropMinAmount.Value = _editorItem.Drops[lstDrops.SelectedIndex].MinQuantity;
            nudDropChance.Value = (decimal)_editorItem.Drops[lstDrops.SelectedIndex].Chance;
        }
    }

    private void cmbDropItem_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        int index = lstDrops.SelectedIndex;
        if (index < 0 || index > lstDrops.Items.Count)
        {
            return;
        }

        _editorItem.Drops[index].ItemId = ItemDescriptor.IdFromList(cmbDropItem.SelectedIndex - 1);
        _dropList[index].ItemId = _editorItem.Drops[index].ItemId;
    }

    private void nudDropMaxAmount_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        int index = lstDrops.SelectedIndex;
        if (index < 0 || index > lstDrops.Items.Count)
        {
            return;
        }

        _editorItem.Drops[index].MaxQuantity = (int)nudDropMaxAmount.Value;
        _dropList[index].MaxQuantity = _editorItem.Drops[index].MaxQuantity;
    }

    private void nudDropMinAmount_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        int index = lstDrops.SelectedIndex;
        if (index < 0 || index > lstDrops.Items.Count)
        {
            return;
        }

        _editorItem.Drops[index].MinQuantity = (int)nudDropMinAmount.Value;
        _dropList[index].MinQuantity = _editorItem.Drops[index].MinQuantity;
    }

    private void nudDropChance_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        int index = lstDrops.SelectedIndex;
        if (index < 0 || index > lstDrops.Items.Count)
        {
            return;
        }

        _editorItem.Drops[index].Chance = (double)nudDropChance.Value;
        _dropList[index].Chance = _editorItem.Drops[index].Chance;
    }

    private void btnDropAdd_Click(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        var drop = new Drop()
        {
            ItemId = ItemDescriptor.IdFromList(cmbDropItem.SelectedIndex - 1),
            MaxQuantity = (int)nudDropMaxAmount.Value,
            MinQuantity = (int)nudDropMinAmount.Value,
            Chance = (double)nudDropChance.Value
        };

        _editorItem.Drops.Add(drop);

        _dropList.Add(new NotifiableDrop
        {
            ItemId = drop.ItemId,
            MinQuantity = drop.MinQuantity,
            MaxQuantity = drop.MaxQuantity,
            Chance = drop.Chance
        });

        lstDrops.SelectedIndex = lstDrops.Items.Count - 1;
    }

    private void btnDropRemove_Click(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (lstDrops.SelectedIndex < 0)
        {
            return;
        }

        var index = lstDrops.SelectedIndex;
        _editorItem.Drops.RemoveAt(index);
        _dropList.RemoveAt(index);
    }

    private void nudHpRegen_ValueChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.VitalRegen = (int)nudHpRegen.Value;
    }

    private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.Event = EventDescriptor.Get(EventDescriptor.IdFromList(cmbEvent.SelectedIndex - 1));
    }

    private void btnRequirements_Click(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        var frm = new FrmDynamicRequirements(_editorItem.HarvestingRequirements, RequirementType.Resource);
        frm.ShowDialog();
    }

    private void txtCannotHarvest_TextChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        _editorItem.CannotHarvestMessage = txtCannotHarvest.Text;
    }

    private void lstStates_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (lstStates.SelectedIndex < 0)
        {
            return;
        }

        UpdateStateControls();
    }

    private void btnAddState_Click(object sender, EventArgs e)
    {
        //alert if name is less than 3 characters
        if (txtStateName.Text.Length <= 0)
        {
            DarkMessageBox.ShowError(
                Strings.ResourceEditor.StateNameError,
                Strings.ResourceEditor.StateErrorTitle
            );
            return;
        }

        if (_editorItem is null)
        {
            return;
        }

        var state = new ResourceStateDescriptor
        {
            Id = Guid.NewGuid(),
            Name = txtStateName.Text,
        };

        _editorItem.StatesGraphics.Add(state.Id, state);
        _editorItem.StatesGraphics = _editorItem.StatesGraphics
            .OrderBy(s => s.Value.MinimumHealth)
            .ThenBy(s => s.Value.MaximumHealth)
            .ToDictionary(p => p.Key, p => p.Value);

        lstStates.Items.Clear();
        foreach (var stateName in _editorItem.StatesGraphics.Values.Select(s => s.Name))
        {
            lstStates.Items.Add(stateName);
        }
        lstStates.SelectedIndex = lstStates.Items.IndexOf(state.Name);
        txtStateName.Text = string.Empty;
    }

    private void btnRemoveState_Click(object sender, EventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        var selectedIndex = lstStates.SelectedIndex;
        if (selectedIndex < 0)
        {
            return;
        }

        var stateKey = _editorItem.StatesGraphics.Keys.ToList()[selectedIndex];
        if (!_editorItem.StatesGraphics.TryGetValue(stateKey, out _))
        {
            return;
        }

        _editorItem.StatesGraphics.Remove(stateKey);
        lstStates.Items.RemoveAt(selectedIndex);
        if (lstStates.Items.Count > 0)
        {
            lstStates.SelectedIndex = 0;
        }
    }

    private void cmbGraphicType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        currentState.TextureType = (ResourceTextureSource)cmbTextureType.SelectedIndex;
        UpdateGraphicFileControl(currentState);
    }

    private void chkRenderBelowEntity_CheckedChanged(object sender, EventArgs e)
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        currentState.RenderBelowEntities = chkRenderBelowEntity.Checked;
    }

    private void cmbGraphicFile_SelectedIndexChanged(object sender, EventArgs e)
    {
        _resourceGraphic?.Dispose();
        _resourceGraphic = null;

        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        if (cmbTextureSource.SelectedIndex > 0)
        {
            currentState.Texture = cmbTextureSource.Text;
            var graphic = Path.Combine(
                "resources", currentState.TextureType == ResourceTextureSource.Tileset ? "tilesets" : "resources", cmbTextureSource.Text
            );

            if (File.Exists(graphic))
            {
                _resourceGraphic = (Bitmap)Image.FromFile(graphic);
                picResource.Width = _resourceGraphic.Width;
                picResource.Height = _resourceGraphic.Height;
                _graphicBitmap = new Bitmap(picResource.Width, picResource.Height);
            }
        }
        else
        {
            currentState.Texture = null;
        }

        picResource.Visible = _resourceGraphic != null;
    }

    private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        currentState.AnimationId = AnimationDescriptor.IdFromList(cmbAnimation.SelectedIndex - 1);
    }

    private void nudStateRangeMin_ValueChanged(object sender, EventArgs e)
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        currentState.MinimumHealth = (int)nudStateRangeMin.Value;
    }

    private void nudStateRangeMax_ValueChanged(object sender, EventArgs e)
    {
        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        currentState.MaximumHealth = (int)nudStateRangeMax.Value;
    }

    private void picResource_MouseDown(object sender, MouseEventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (e.X > picResource.Width || e.Y > picResource.Height)
        {
            return;
        }

        if ((ResourceTextureSource)cmbTextureType.SelectedIndex != ResourceTextureSource.Tileset)
        {
            return;
        }

        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        _mouseDown = true;
        currentState.X = Math.Max(0, (int)Math.Floor((double)e.X / Options.Instance.Map.TileWidth));
        currentState.Y = Math.Max(0, (int)Math.Floor((double)e.Y / Options.Instance.Map.TileHeight));
        currentState.Width = 0;
        currentState.Height = 0;
    }

    private void picResource_MouseUp(object sender, MouseEventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (e.X > picResource.Width || e.Y > picResource.Height)
        {
            return;
        }

        if ((ResourceTextureSource)cmbTextureType.SelectedIndex != ResourceTextureSource.Tileset)
        {
            return;
        }

        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        var selX = currentState.X;
        var selY = currentState.Y;
        var selW = currentState.Width;
        var selH = currentState.Height;

        if (selW < 0)
        {
            selX -= Math.Abs(selW);
            selW = Math.Abs(selW);
        }

        if (selH < 0)
        {
            selY -= Math.Abs(selH);
            selH = Math.Abs(selH);
        }

        currentState.X = selX;
        currentState.Y = selY;
        currentState.Width = selW;
        currentState.Height = selH;
        _mouseDown = false;
    }

    private void picResource_MouseMove(object sender, MouseEventArgs e)
    {
        if (_editorItem is null)
        {
            return;
        }

        if (e.X > picResource.Width || e.Y > picResource.Height)
        {
            return;
        }

        if ((ResourceTextureSource)cmbTextureType.SelectedIndex != ResourceTextureSource.Tileset)
        {
            return;
        }

        if (!TryGetCurrentState(out var currentState))
        {
            return;
        }

        if (_mouseDown)
        {
            var tmpX = Math.Max(0, (int)Math.Floor((double)e.X / Options.Instance.Map.TileWidth));
            var tmpY = Math.Max(0, (int)Math.Floor((double)e.Y / Options.Instance.Map.TileHeight));
            currentState.Width = tmpX - currentState.X;
            currentState.Height = tmpY - currentState.Y;
        }
    }

    #endregion

    #region "Item List - Folders, Searching, Sorting, Etc"

    public void InitEditor()
    {
        //Collect folders
        var mFolders = new List<string>();
        foreach (var itm in ResourceDescriptor.Lookup)
        {
            if (!string.IsNullOrEmpty(((ResourceDescriptor)itm.Value).Folder) &&
                !mFolders.Contains(((ResourceDescriptor)itm.Value).Folder))
            {
                mFolders.Add(((ResourceDescriptor)itm.Value).Folder);
                if (!_knownFolders.Contains(((ResourceDescriptor)itm.Value).Folder))
                {
                    _knownFolders.Add(((ResourceDescriptor)itm.Value).Folder);
                }
            }
        }

        mFolders.Sort();
        _knownFolders.Sort();
        cmbFolder.Items.Clear();
        cmbFolder.Items.Add("");
        cmbFolder.Items.AddRange(_knownFolders.ToArray());

        var items = ResourceDescriptor.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
            new KeyValuePair<string, string>(((ResourceDescriptor)pair.Value)?.Name ?? Models.DatabaseObject<ResourceDescriptor>.Deleted, ((ResourceDescriptor)pair.Value)?.Folder ?? ""))).ToArray();
        lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var folderName = string.Empty;
        var result = DarkInputBox.ShowInformation(
            Strings.ResourceEditor.folderprompt, Strings.ResourceEditor.foldertitle, ref folderName,
            DarkDialogButton.OkCancel
        );

        if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
        {
            if (!cmbFolder.Items.Contains(folderName))
            {
                _editorItem.Folder = folderName;
                lstGameObjects.ExpandFolder(folderName);
                InitEditor();
                cmbFolder.Text = folderName;
            }
        }
    }

    private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
    {
        _editorItem.Folder = cmbFolder.Text;
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
            txtSearch.Text = Strings.ResourceEditor.searchplaceholder;
        }
    }

    private void txtSearch_Enter(object sender, EventArgs e)
    {
        txtSearch.SelectAll();
        txtSearch.Focus();
    }

    private void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = Strings.ResourceEditor.searchplaceholder;
    }

    private bool CustomSearch()
    {
        return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
               txtSearch.Text != Strings.ResourceEditor.searchplaceholder;
    }

    private void txtSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == Strings.ResourceEditor.searchplaceholder)
        {
            txtSearch.SelectAll();
        }
    }

    #endregion
}
