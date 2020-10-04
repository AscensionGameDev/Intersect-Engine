using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Utilities;

using JetBrains.Annotations;

using Microsoft.Xna.Framework.Graphics;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms.DockingElements
{

    public enum LayerTabs
    {

        Tiles = 0,

        Attributes,

        Lights,

        Events,

        Npcs

    }

    public partial class FrmMapLayers : DockContent
    {

        public LayerTabs CurrentTab = LayerTabs.Tiles;

        public List<bool> LayerVisibility = new List<bool>();

        //MonoGame Swap Chain
        private SwapChainRenderTarget mChain;

        private int mLastTileLayer;

        private List<PictureBox> mMapLayers = new List<PictureBox>();

        private bool mTMouseDown;

        public FrmMapLayers()
        {
            InitializeComponent();
            mMapLayers.Add(picGround);
            LayerVisibility.Add(true);
            mMapLayers.Add(picMask);
            LayerVisibility.Add(true);
            mMapLayers.Add(picMask2);
            LayerVisibility.Add(true);
            mMapLayers.Add(picFringe);
            LayerVisibility.Add(true);
            mMapLayers.Add(picFringe2);
            LayerVisibility.Add(true);
        }

        public void Init()
        {
            cmbAutotile.SelectedIndex = 0;
            SetLayer(0);
            if (cmbTilesets.Items.Count > 0)
            {
                SetTileset(cmbTilesets.Items[0].ToString());
            }

            grpZDimension.Visible = Options.ZDimensionVisible;
            rbZDimension.Visible = Options.ZDimensionVisible;
            grpZResource.Visible = Options.ZDimensionVisible;
        }

        //Tiles Tab
        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTileset(cmbTilesets.Text);
        }

        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height)
            {
                return;
            }

            mTMouseDown = true;
            Globals.CurSelX = (int) Math.Floor((double) e.X / Options.TileWidth);
            Globals.CurSelY = (int) Math.Floor((double) e.Y / Options.TileHeight);
            Globals.CurSelW = 0;
            Globals.CurSelH = 0;
            if (Globals.CurSelX < 0)
            {
                Globals.CurSelX = 0;
            }

            if (Globals.CurSelY < 0)
            {
                Globals.CurSelY = 0;
            }

            switch (Globals.Autotilemode)
            {
                case 1:
                case 5:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 2;

                    break;
                case 2:
                    Globals.CurSelW = 0;
                    Globals.CurSelH = 0;

                    break;
                case 3:
                    Globals.CurSelW = 5;
                    Globals.CurSelH = 2;

                    break;
                case 4:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 1;

                    break;
                case 6:
                    Globals.CurSelW = 2;
                    Globals.CurSelH = 3;

                    break;
                case 7:
                    Globals.CurSelW = 8;
                    Globals.CurSelH = 3;

                    break;
            }
        }

        private void picTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height)
            {
                return;
            }

            if (mTMouseDown && Globals.Autotilemode == 0)
            {
                var tmpX = (int) Math.Floor((double) e.X / Options.TileWidth);
                var tmpY = (int) Math.Floor((double) e.Y / Options.TileHeight);
                Globals.CurSelW = tmpX - Globals.CurSelX;
                Globals.CurSelH = tmpY - Globals.CurSelY;
            }
        }

        private void picTileset_MouseUp(object sender, MouseEventArgs e)
        {
            var selX = Globals.CurSelX;
            var selY = Globals.CurSelY;
            var selW = Globals.CurSelW;
            var selH = Globals.CurSelH;
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

            Globals.CurSelX = selX;
            Globals.CurSelY = selY;
            Globals.CurSelW = selW;
            Globals.CurSelH = selH;
            mTMouseDown = false;
            Globals.MapEditorWindow.DockPanel.Focus();
        }

        private void cmbAutotile_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAutoTile(cmbAutotile.SelectedIndex);
        }

        public void SetAutoTile(int index)
        {
            Globals.Autotilemode = index;
            cmbAutotile.SelectedIndex = index;
            switch (Globals.Autotilemode)
            {
                case 1:
                case 5:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 2;

                    break;
                case 2:
                    Globals.CurSelW = 0;
                    Globals.CurSelH = 0;

                    break;
                case 3:
                    Globals.CurSelW = 5;
                    Globals.CurSelH = 2;

                    break;
                case 4:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 1;

                    break;
                case 6:
                    Globals.CurSelW = 2;
                    Globals.CurSelH = 3;

                    break;
                case 7:
                    Globals.CurSelW = 8;
                    Globals.CurSelH = 3;

                    break;
            }
        }

        public void InitTilesets()
        {
            Globals.MapLayersWindow.cmbTilesets.Items.Clear();
            var tilesetList = new List<string>();
            tilesetList.AddRange(TilesetBase.Names);
            tilesetList.Sort(new AlphanumComparatorFast());
            foreach (var filename in tilesetList)
            {
                if (File.Exists("resources/tilesets/" + filename))
                {
                    Globals.MapLayersWindow.cmbTilesets.Items.Add(filename);
                }
                else
                {
                }
            }

            if (TilesetBase.Lookup.Count > 0)
            {
                if (Globals.MapLayersWindow.cmbTilesets.Items.Count > 0)
                {
                    Globals.MapLayersWindow.cmbTilesets.SelectedIndex = 0;
                }

                Globals.CurrentTileset = (TilesetBase) TilesetBase.Lookup.Values.ToArray()[0];
            }
        }

        public void SetTileset(string name)
        {
            TilesetBase tSet = null;
            var tilesets = TilesetBase.Lookup;
            var id = Guid.Empty;
            foreach (var tileset in tilesets.Pairs)
            {
                if (tileset.Value.Name.ToLower() == name.ToLower())
                {
                    id = tileset.Key;

                    break;
                }
            }

            if (id != Guid.Empty)
            {
                tSet = TilesetBase.Get(id);
            }

            if (tSet != null)
            {
                if (File.Exists("resources/tilesets/" + tSet.Name))
                {
                    picTileset.Show();
                    Globals.CurrentTileset = tSet;
                    Globals.CurSelX = 0;
                    Globals.CurSelY = 0;
                    var tilesetTex = GameContentManager.GetTexture(GameContentManager.TextureType.Tileset, tSet.Name);
                    if (tilesetTex != null)
                    {
                        picTileset.Width = tilesetTex.Width;
                        picTileset.Height = tilesetTex.Height;
                    }

                    cmbTilesets.SelectedItem = name;
                    CreateSwapChain();
                }
            }
        }

        public void SetLayer(int index)
        {
            Globals.CurrentLayer = index;
            if (index < Options.LayerCount)
            {
                for (var i = 0; i < mMapLayers.Count; i++)
                {
                    if (i == index)
                    {
                        if (!LayerVisibility[i])
                        {
                            mMapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_A_Hide");
                        }
                        else
                        {
                            mMapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_A");
                        }
                    }
                    else
                    {
                        if (!LayerVisibility[i])
                        {
                            mMapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_B_Hide");
                        }
                        else
                        {
                            mMapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_B");
                        }
                    }
                }

                mLastTileLayer = index;
            }
            else
            {
            }

            Core.Graphics.TilePreviewUpdated = true;
        }

        //Mapping Attribute Functions
        /// <summary>
        ///     A method that hides all of the extra group boxes for tile data related to the map attributes.
        /// </summary>
        private void HideAttributeMenus()
        {
            grpItem.Visible = false;
            grpZDimension.Visible = false;
            grpWarp.Visible = false;
            grpSound.Visible = false;
            grpResource.Visible = false;
            grpAnimation.Visible = false;
            grpSlide.Visible = false;
        }

        private void rbItem_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
            grpItem.Visible = true;
            cmbItemAttribute.Items.Clear();
            cmbItemAttribute.Items.AddRange(ItemBase.Names);
            if (cmbItemAttribute.Items.Count > 0)
            {
                cmbItemAttribute.SelectedIndex = 0;
            }
        }

        private void rbBlocked_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
        }

        private void rbNPCAvoid_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
        }

        private void rbZDimension_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
            grpZDimension.Visible = true;
        }

        private void rbWarp_CheckedChanged(object sender, EventArgs e)
        {
            nudWarpX.Maximum = Options.MapWidth;
            nudWarpY.Maximum = Options.MapHeight;
            cmbWarpMap.Items.Clear();
            for (var i = 0; i < MapList.OrderedMaps.Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.OrderedMaps[i].Name);
            }

            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
            if (!rbWarp.Checked)
            {
                return;
            }

            HideAttributeMenus();
            grpWarp.Visible = true;
        }

        private void rbSound_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
            grpSound.Visible = true;
            cmbMapAttributeSound.Items.Clear();
            cmbMapAttributeSound.Items.Add(Strings.General.none);
            cmbMapAttributeSound.Items.AddRange(GameContentManager.SmartSortedSoundNames);
            cmbMapAttributeSound.SelectedIndex = 0;
        }

        private void rbResource_CheckedChanged(object sender, EventArgs e)
        {
            cmbResourceAttribute.Items.Clear();
            cmbResourceAttribute.Items.AddRange(ResourceBase.Names);
            if (cmbResourceAttribute.Items.Count > 0)
            {
                cmbResourceAttribute.SelectedIndex = 0;
            }

            if (!rbResource.Checked)
            {
                return;
            }

            HideAttributeMenus();
            grpResource.Visible = true;
        }

        // Used for returning an integer value depending on which radio button is selected on the forms. This is merely used to make PlaceAtrribute less messy.
        private byte GetEditorDimensionGateway()
        {
            if (rbGateway1.Checked == true)
            {
                return 1;
            }
            else if (rbGateway2.Checked == true)
            {
                return 2;
            }

            return 0;
        }

        private byte GetEditorDimensionBlock()
        {
            if (rbBlock1.Checked == true)
            {
                return 1;
            }
            else if (rbBlock2.Checked == true)
            {
                return 2;
            }

            return 0;
        }

        public int GetAttributeFromEditor()
        {
            if (rbBlocked.Checked == true)
            {
                return (int) MapAttributes.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                return (int) MapAttributes.Item;
            }
            else if (rbZDimension.Checked == true)
            {
                return (int) MapAttributes.ZDimension;
            }
            else if (rbNPCAvoid.Checked == true)
            {
                return (int) MapAttributes.NpcAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                return (int) MapAttributes.Warp;
            }
            else if (rbSound.Checked == true)
            {
                return (int) MapAttributes.Sound;
            }
            else if (rbResource.Checked == true)
            {
                return (int) MapAttributes.Resource;
            }
            else if (rbAnimation.Checked == true)
            {
                return (int) MapAttributes.Animation;
            }
            else if (rbGrappleStone.Checked == true)
            {
                return (int) MapAttributes.GrappleStone;
            }
            else if (rbSlide.Checked == true)
            {
                return (int) MapAttributes.Slide;
            }

            return (int) MapAttributes.Walkable;
        }

        private MapAttributes SelectedMapAttributeType
        {
            get
            {
                if (rbBlocked.Checked)
                {
                    return MapAttributes.Blocked;
                }
                
                if (rbItem.Checked)
                {
                    return MapAttributes.Item;
                }

                if (rbZDimension.Checked)
                {
                    return MapAttributes.ZDimension;
                }

                if (rbNPCAvoid.Checked)
                {
                    return MapAttributes.NpcAvoid;
                }

                if (rbWarp.Checked)
                {
                    return MapAttributes.Warp;
                }

                if (rbSound.Checked)
                {
                    return MapAttributes.Sound;
                }

                if (rbResource.Checked)
                {
                    return MapAttributes.Resource;
                }

                if (rbAnimation.Checked)
                {
                    return MapAttributes.Animation;
                }

                if (rbGrappleStone.Checked)
                {
                    return MapAttributes.GrappleStone;
                }

                if (rbSlide.Checked)
                {
                    return MapAttributes.Slide;
                }

                return (MapAttributes) byte.MaxValue;
            }
        }

        [Obsolete("The entire switch statement should be implemented as a parameterized CreateAttribute().")]
        [NotNull]
        public MapAttribute CreateAttribute()
        {
            var attributeType = SelectedMapAttributeType;
            var attribute = MapAttribute.CreateAttribute(attributeType);
            switch (SelectedMapAttributeType)
            {
                case MapAttributes.Walkable:
                case MapAttributes.Blocked:
                case MapAttributes.GrappleStone:
                case MapAttributes.NpcAvoid:
                    break;

                case MapAttributes.Item:
                    var itemAttribute = attribute as MapItemAttribute;
                    itemAttribute.ItemId = ItemBase.IdFromList(cmbItemAttribute.SelectedIndex);
                    itemAttribute.Quantity = (int)nudItemQuantity.Value;
                    break;

                case MapAttributes.ZDimension:
                    var zDimensionAttribute = attribute as MapZDimensionAttribute;
                    zDimensionAttribute.GatewayTo = GetEditorDimensionGateway();
                    zDimensionAttribute.BlockedLevel = GetEditorDimensionBlock();
                    break;

                case MapAttributes.Warp:
                    var warpAttribute = attribute as MapWarpAttribute;
                    warpAttribute.MapId = MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId;
                    warpAttribute.X = (byte)nudWarpX.Value;
                    warpAttribute.Y = (byte)nudWarpY.Value;
                    warpAttribute.Direction = (WarpDirection)cmbDirection.SelectedIndex;
                    break;

                case MapAttributes.Sound:
                    var soundAttribute = attribute as MapSoundAttribute;
                    soundAttribute.Distance = (byte)nudSoundDistance.Value;
                    soundAttribute.File = TextUtils.SanitizeNone(cmbMapAttributeSound.Text);
                    break;

                case MapAttributes.Resource:
                    var resourceAttribute = attribute as MapResourceAttribute;
                    resourceAttribute.ResourceId = ResourceBase.IdFromList(cmbResourceAttribute.SelectedIndex);
                    resourceAttribute.SpawnLevel = (byte)(rbLevel1.Checked ? 0 : 1);
                    break;

                case MapAttributes.Animation:
                    var animationAttribute = attribute as MapAnimationAttribute;
                    animationAttribute.AnimationId = AnimationBase.IdFromList(cmbAnimationAttribute.SelectedIndex);
                    break;

                case MapAttributes.Slide:
                    var slideAttribute = attribute as MapSlideAttribute;
                    slideAttribute.Direction = (byte)cmbSlideDir.SelectedIndex;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectedMapAttributeType), @"The currently selected attribute type has not been fully implemented.");
            }

            return attribute;
        }

        public MapAttribute PlaceAttribute(MapBase mapDescriptor, int x, int y, MapAttribute attribute = null)
        {
            if (attribute == null)
            {
                attribute = CreateAttribute();
            }

            mapDescriptor.Attributes[x, y] = attribute;

            return attribute;
        }

        public bool RemoveAttribute(MapBase tmpMap, int x, int y)
        {
            if (tmpMap.Attributes[x, y] != null && tmpMap.Attributes[x, y].Type != MapAttributes.Walkable)
            {
                tmpMap.Attributes[x, y] = null;

                return true;
            }

            return false;
        }

        public void RefreshNpcList()
        {
            // Update the list incase npcs have been modified since form load.
            cmbNpc.Items.Clear();
            cmbNpc.Items.AddRange(NpcBase.Names);

            // Add the map NPCs
            lstMapNpcs.Items.Clear();
            for (var i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
            {
                lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcId));
            }

            // Don't select if there are no NPCs, to avoid crashes.
            if (cmbNpc.Items.Count > 0)
            {
                cmbNpc.SelectedIndex = 0;
            }

            cmbDir.SelectedIndex = 0;
            rbRandom.Checked = true;
            if (lstMapNpcs.Items.Count > 0)
            {
                lstMapNpcs.SelectedIndex = 0;
                if (lstMapNpcs.SelectedIndex < Globals.CurrentMap.Spawns.Count)
                {
                    cmbDir.SelectedIndex = (int) Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Direction;
                    cmbNpc.SelectedIndex = NpcBase.ListIndex(Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcId);
                    if (Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                    {
                        rbDeclared.Checked = true;
                    }
                }
            }
        }

        private void frmMapLayers_DockStateChanged(object sender, EventArgs e)
        {
            CreateSwapChain();
        }

        //Map NPC List
        private void btnAddMapNpc_Click(object sender, EventArgs e)
        {
            var n = new NpcSpawn();

            //Don't add nothing
            if (cmbNpc.SelectedIndex > -1)
            {
                n.NpcId = NpcBase.IdFromList(cmbNpc.SelectedIndex);
                n.X = -1;
                n.Y = -1;
                n.Direction = NpcSpawnDirection.Random;

                Globals.CurrentMap.Spawns.Add(n);
                lstMapNpcs.Items.Add(NpcBase.GetName(n.NpcId));
                lstMapNpcs.SelectedIndex = lstMapNpcs.Items.Count - 1;
            }
        }

        private void btnRemoveMapNpc_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex > -1)
            {
                Globals.CurrentMap.Spawns.RemoveAt(lstMapNpcs.SelectedIndex);
                lstMapNpcs.Items.RemoveAt(lstMapNpcs.SelectedIndex);

                // Refresh List
                lstMapNpcs.Items.Clear();
                for (var i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcId));
                }

                if (lstMapNpcs.Items.Count > 0)
                {
                    lstMapNpcs.SelectedIndex = 0;
                }
            }
        }

        private void lstMapNpcs_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.Items.Count > 0 && lstMapNpcs.SelectedIndex > -1)
            {
                cmbNpc.SelectedIndex = NpcBase.ListIndex(Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcId);
                cmbDir.SelectedIndex = (int) Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Direction;
                if (Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                {
                    rbDeclared.Checked = true;
                }
                else
                {
                    rbRandom.Checked = true;
                }
            }
        }

        private void rbRandom_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex > -1)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X = -1;
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Y = -1;
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Direction = NpcSpawnDirection.Random;
            }
        }

        private void cmbDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Direction =
                    (NpcSpawnDirection) cmbDir.SelectedIndex;
            }
        }

        private void cmbNpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            var n = 0;

            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcId = NpcBase.IdFromList(cmbNpc.SelectedIndex);

                // Refresh List
                n = lstMapNpcs.SelectedIndex;
                lstMapNpcs.Items.Clear();
                for (var i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcId));
                }

                lstMapNpcs.SelectedIndex = n;
            }
        }

        private void lightEditor_Load(object sender, EventArgs e)
        {
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            var frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(
                MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId, (int) nudWarpX.Value, (int) nudWarpY.Value
            );

            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                cmbWarpMap.Items.Clear();
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    cmbWarpMap.Items.Add(MapList.OrderedMaps[i].Name);
                }

                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;

                        break;
                    }
                }

                nudWarpX.Value = frmWarpSelection.GetX();
                nudWarpY.Value = frmWarpSelection.GetY();
            }
        }

        private void rbAnimation_CheckedChanged(object sender, EventArgs e)
        {
            cmbAnimationAttribute.Items.Clear();
            cmbAnimationAttribute.Items.AddRange(AnimationBase.Names);
            if (cmbAnimationAttribute.Items.Count > 0)
            {
                cmbAnimationAttribute.SelectedIndex = 0;
            }

            if (!rbAnimation.Checked)
            {
                return;
            }

            HideAttributeMenus();
            grpAnimation.Visible = true;
        }

        private void frmMapLayers_Load(object sender, EventArgs e)
        {
            CreateSwapChain();
            picTileset.MouseDown += pnlTilesetContainer.AutoDragPanel_MouseDown;
            picTileset.MouseUp += pnlTilesetContainer.AutoDragPanel_MouseUp;
            pnlTiles.BringToFront();
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.MapLayers.title;
            btnTileHeader.Text = Strings.MapLayers.tiles;
            btnAttributeHeader.Text = Strings.MapLayers.attributes;
            btnEventsHeader.Text = Strings.MapLayers.events;
            btnLightsHeader.Text = Strings.MapLayers.lights;
            btnNpcsHeader.Text = Strings.MapLayers.npcs;

            //Tiles Panel
            lblLayer.Text = Strings.Tiles.layer;
            lblTileset.Text = Strings.Tiles.tileset;
            lblTileType.Text = Strings.Tiles.tiletype;
            cmbAutotile.Items.Clear();
            cmbAutotile.Items.Add(Strings.Tiles.normal);
            cmbAutotile.Items.Add(Strings.Tiles.autotile);
            cmbAutotile.Items.Add(Strings.Tiles.fake);
            cmbAutotile.Items.Add(Strings.Tiles.animated);
            cmbAutotile.Items.Add(Strings.Tiles.cliff);
            cmbAutotile.Items.Add(Strings.Tiles.waterfall);
            cmbAutotile.Items.Add(Strings.Tiles.autotilexp);
            cmbAutotile.Items.Add(Strings.Tiles.animatedxp);

            //Attributes Panel
            rbBlocked.Text = Strings.Attributes.blocked;
            rbZDimension.Text = Strings.Attributes.zdimension;
            rbNPCAvoid.Text = Strings.Attributes.npcavoid;
            rbWarp.Text = Strings.Attributes.warp;
            rbItem.Text = Strings.Attributes.itemspawn;
            rbSound.Text = Strings.Attributes.mapsound;
            rbResource.Text = Strings.Attributes.resourcespawn;
            rbAnimation.Text = Strings.Attributes.mapanimation;
            rbGrappleStone.Text = Strings.Attributes.grapple;
            rbSlide.Text = Strings.Attributes.slide;

            //Map Animation Groupbox
            grpAnimation.Text = Strings.Attributes.mapanimation;
            lblAnimation.Text = Strings.Attributes.mapanimation;

            //Slide Groupbox
            grpSlide.Text = Strings.Attributes.slide;
            lblSlideDir.Text = Strings.Attributes.dir;
            cmbSlideDir.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbSlideDir.Items.Add(Strings.Directions.dir[i]);
            }

            //Map Sound
            grpSound.Text = Strings.Attributes.mapsound;
            lblMapSound.Text = Strings.Attributes.sound;
            lblSoundDistance.Text = Strings.Attributes.distance;

            //Map Item
            grpItem.Text = Strings.Attributes.itemspawn;
            lblMapItem.Text = Strings.Attributes.item;
            lblMaxItemAmount.Text = Strings.Attributes.quantity;

            //Z-Dimension
            grpZDimension.Text = Strings.Attributes.zdimension;
            grpGateway.Text = Strings.Attributes.zgateway;
            grpDimBlock.Text = Strings.Attributes.zblock;
            rbGatewayNone.Text = Strings.Attributes.znone;
            rbGateway1.Text = Strings.Attributes.zlevel1;
            rbGateway2.Text = Strings.Attributes.zlevel2;
            rbBlockNone.Text = Strings.Attributes.znone;
            rbBlock1.Text = Strings.Attributes.zlevel1;
            rbBlock2.Text = Strings.Attributes.zlevel2;

            //Warp
            grpWarp.Text = Strings.Attributes.warp;
            lblMap.Text = Strings.Warping.map.ToString("");
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y.ToString("");
            lblWarpDir.Text = Strings.Warping.direction.ToString("");
            cmbDirection.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }

            btnVisualMapSelector.Text = Strings.Warping.visual;

            //Resource
            grpResource.Text = Strings.Attributes.resourcespawn;
            lblResource.Text = Strings.Attributes.resource;
            grpZResource.Text = Strings.Attributes.zdimension;
            rbLevel1.Text = Strings.Attributes.zlevel1;
            rbLevel2.Text = Strings.Attributes.zlevel2;

            //NPCS Tab
            grpSpawnLoc.Text = rbDeclared.Checked ? Strings.NpcSpawns.spawndeclared : Strings.NpcSpawns.spawnrandom;
            rbDeclared.Text = Strings.NpcSpawns.declaredlocation;
            rbRandom.Text = Strings.NpcSpawns.randomlocation;
            lblDir.Text = Strings.NpcSpawns.direction;
            cmbDir.Items.Clear();
            cmbDir.Items.Add(Strings.NpcSpawns.randomdirection);
            for (var i = 0; i < 4; i++)
            {
                cmbDir.Items.Add(Strings.Directions.dir[i]);
            }

            grpNpcList.Text = Strings.NpcSpawns.addremove;
            btnAddMapNpc.Text = Strings.NpcSpawns.add;
            btnRemoveMapNpc.Text = Strings.NpcSpawns.remove;

            lblEventInstructions.Text = Strings.MapLayers.eventinstructions;
            lblLightInstructions.Text = Strings.MapLayers.lightinstructions;

            for (var i = 0; i < mMapLayers.Count; i++)
            {
                mMapLayers[i].Text = Strings.Tiles.layers[i];
            }
        }

        public void InitMapLayers()
        {
            CreateSwapChain();
        }

        private void CreateSwapChain()
        {
            if (!Globals.ClosingEditor)
            {
                if (mChain != null)
                {
                    mChain.Dispose();
                }

                if (Core.Graphics.GetGraphicsDevice() != null)
                {
                    mChain = new SwapChainRenderTarget(
                        Core.Graphics.GetGraphicsDevice(), picTileset.Handle, picTileset.Width, picTileset.Height,
                        false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents,
                        PresentInterval.Immediate
                    );

                    Core.Graphics.SetTilesetChain(mChain);
                }
            }
        }

        private void rbGrappleStone_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
        }

        private void rbSlide_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
            grpSlide.Visible = true;
            cmbSlideDir.SelectedIndex = 0;
        }

        private void lstMapNpcs_MouseDown(object sender, MouseEventArgs e)
        {
            lstMapNpcs.SelectedIndex = lstMapNpcs.IndexFromPoint(e.Location);
        }

        private void ChangeTab()
        {
            btnTileHeader.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            btnAttributeHeader.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            btnLightsHeader.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            btnEventsHeader.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            btnNpcsHeader.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            pnlTiles.Hide();
            pnlAttributes.Hide();
            pnlLights.Hide();
            pnlEvents.Hide();
            pnlNpcs.Hide();

            //Force Game Object Lists to Refresh
            rbAnimation_CheckedChanged(null, null);
            rbWarp_CheckedChanged(null, null);
            rbResource_CheckedChanged(null, null);

            if (Globals.EditingLight != null)
            {
                Globals.MapLayersWindow.lightEditor.Cancel();
            }
        }

        private void btnTileHeader_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = Globals.SavedTool;
            ChangeTab();
            SetLayer(mLastTileLayer);
            Core.Graphics.TilePreviewUpdated = true;
            btnTileHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Tiles;
            pnlTiles.Show();
        }

        private void btnAttributeHeader_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = Globals.SavedTool;
            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount;
            Core.Graphics.TilePreviewUpdated = true;
            btnAttributeHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Attributes;
            pnlAttributes.Show();
        }

        public void btnLightsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer < Options.LayerCount + 1)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 1;
            Core.Graphics.TilePreviewUpdated = true;
            btnLightsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Lights;
            pnlLights.Show();
        }

        private void btnEventsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer < Options.LayerCount + 1)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 2;
            Core.Graphics.TilePreviewUpdated = true;
            btnEventsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Events;
            pnlEvents.Show();
        }

        private void btnNpcsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer < Options.LayerCount + 1)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 3;
            Core.Graphics.TilePreviewUpdated = true;
            RefreshNpcList();
            btnNpcsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Npcs;
            pnlNpcs.Show();
        }

        private void picMapLayer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetLayer(mMapLayers.IndexOf((PictureBox) sender));
            }
            else
            {
                ToggleLayerVisibility(mMapLayers.IndexOf((PictureBox) sender));
            }
        }

        private void ToggleLayerVisibility(int index)
        {
            LayerVisibility[index] = !LayerVisibility[index];
            SetLayer(Globals.CurrentLayer);
        }

        private void picMapLayer_MouseHover(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip((PictureBox) sender, Strings.Tiles.layers[mMapLayers.IndexOf((PictureBox) sender)]);
        }

        private void cmbTilesets_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && Globals.CurrentTileset != null)
            {
                Clipboard.SetText(Globals.CurrentTileset.Id.ToString());
            }
        }

        private void NudItemQuantity_ValueChanged(object sender, System.EventArgs e)
        {
            nudItemQuantity.Value = Math.Max(1, nudItemQuantity.Value);
        }

    }

}
