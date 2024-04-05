using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Intersect.Utilities;

using Microsoft.Xna.Framework.Graphics;

using WeifenLuo.WinFormsUI.Docking;
using Graphics = System.Drawing.Graphics;
using MapAttribute = Intersect.Enums.MapAttribute;

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

        public Dictionary<string, bool> LayerVisibility = new Dictionary<string,bool>();

        //MonoGame Swap Chain
        private SwapChainRenderTarget mChain;

        private string mLastTileLayer;

        private List<PictureBox> mMapLayers = new List<PictureBox>();

        private bool mTMouseDown;

        public static System.Drawing.Color NpcPulseColor;

        public FrmMapLayers()
        {
            InitializeComponent();
            Icon = Program.Icon;

            mMapLayers.Add(picLayer1);
            mMapLayers.Add(picLayer2);
            mMapLayers.Add(picLayer3);
            mMapLayers.Add(picLayer4);
            mMapLayers.Add(picLayer5);
        }

        public void Init()
        {
            cmbAutotile.SelectedIndex = 0;

            // Selected Npc highlight color from preferences
            NpcPulseColor = ColorTranslator.FromHtml(Preferences.LoadPreference("NpcPulseColor"));
            
            //See if we can use the old style icons instead of a combobox
            if (Options.Instance.MapOpts.Layers.All.Count <= mMapLayers.Count)
            {
                //Hide combobox...
                cmbMapLayer.Hide();
                for (int i = 0; i < mMapLayers.Count; i++)
                {
                    if (i < Options.Instance.MapOpts.Layers.All.Count)
                    {
                        Strings.Tiles.maplayers.TryGetValue(Options.Instance.MapOpts.Layers.All[i].ToLower(), out LocalizedString layerName);
                        if (layerName == null) layerName = Options.Instance.MapOpts.Layers.All[i];
                        mMapLayers[i].Text = layerName;
                        mMapLayers[i].Show();
                    }
                    else
                    {
                        mMapLayers[i].Hide();
                    }
                }
            }
            else
            {
                foreach(var layer in mMapLayers)
                {
                    layer.Hide();
                }
                //Show Combobox
                cmbMapLayer.Show();
                cmbMapLayer.Items.AddRange(Options.Instance.MapOpts.Layers.All.ToArray());
                cmbMapLayer.SelectedIndex = 0;
            }

            foreach (var layer in Options.Instance.MapOpts.Layers.All)
            {
                LayerVisibility.Add(layer, true);
            }

            SetLayer(Options.Instance.MapOpts.Layers.All[0]);
            if (cmbTilesets.Items.Count > 0)
            {
                SetTileset(cmbTilesets.Items[0].ToString());
            }

            rbZDimension.Visible = Options.ZDimensionVisible;
            grpZResource.Visible = Options.ZDimensionVisible;
            grpInstanceSettings.Visible = chkChangeInstance.Checked;

            cmbInstanceType.Items.Clear();
            // We do not want to iterate over the "NoChange" enum
            foreach (MapInstanceType instanceType in Enum.GetValues(typeof(MapInstanceType)))
            {
                cmbInstanceType.Items.Add(instanceType.ToString());
            }
            cmbInstanceType.SelectedIndex = 0;

            cmbWarpSound.Items.Clear();
            RefreshMapWarpSounds();
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

        public void SetLayer(string name)
        {
            Globals.CurrentLayer = name;

            var index = Options.Instance.MapOpts.Layers.All.IndexOf(name);

            if (!cmbMapLayer.Visible)
            {
                for (var i = 0; i < mMapLayers.Count; i++)
                {
                    if (mMapLayers[i].BackgroundImage != null)
                    {
                        mMapLayers[i].BackgroundImage.Dispose();
                        mMapLayers[i].BackgroundImage = null;
                    }
                    mMapLayers[i].BackgroundImage = DrawLayerImage(i, i == index, !LayerVisibility[Options.Instance.MapOpts.Layers.All[i]]);
                }
            }
            else
            {
                if (cmbMapLayer.Items.IndexOf(name) > -1)
                {
                    cmbMapLayer.SelectedIndex = cmbMapLayer.Items.IndexOf(name);
                }
            }

            mLastTileLayer = name;

            Core.Graphics.TilePreviewUpdated = true;
        }

        private Bitmap DrawLayerImage(int layerIndex, bool selected, bool hidden)
        {
            var img = new Bitmap(32, 32);
            img.MakeTransparent(img.GetPixel(0, 0));

            var g = Graphics.FromImage(img);

            var layer = (Bitmap)Properties.Resources.ResourceManager.GetObject("layer");
            var layerSel = (Bitmap)Properties.Resources.ResourceManager.GetObject("layer_sel");
            var face = (Bitmap)Properties.Resources.ResourceManager.GetObject("layer_face");
            var faceSel = (Bitmap)Properties.Resources.ResourceManager.GetObject("layer_face_sel");
            var hiddenIcon = (Bitmap)Properties.Resources.ResourceManager.GetObject("layer_hidden");
            var drawFace = selected ? faceSel : face;

            var drawIndex = 0;

            //Draw Lower & Middle Layers
            foreach (var l in Options.Instance.MapOpts.Layers.LowerLayers)
            {
                var drawImg = layer;
                if (drawIndex == layerIndex)
                {
                    drawImg = layerSel;
                }
                g.DrawImage(drawImg, new PointF(3, 23 - ((drawIndex) * (layer.Height - 4))));
                drawIndex++;
            }


            //If this image for is an upper layer, render the face below the next layers
            if (!Options.Instance.MapOpts.Layers.LowerLayers.Contains(Options.Instance.MapOpts.Layers.All[layerIndex]))
            {
                g.DrawImage(drawFace, new PointF(13, 13));
            }


            //Draw Upper Layers
            var middleUpperLayers = Options.Instance.MapOpts.Layers.LowerLayers.ToList();
            middleUpperLayers.AddRange(Options.Instance.MapOpts.Layers.MiddleLayers);
            foreach (var l in middleUpperLayers)
            {
                var drawImg = layer;
                if (drawIndex == layerIndex)
                {
                    drawImg = layerSel;
                }
                g.DrawImage(drawImg, new PointF(3, 23 - ((drawIndex) * (layer.Height - 4))));
                drawIndex++;
            }

            //If this image for is a lower layer, render the face above everything
            if (Options.Instance.MapOpts.Layers.LowerLayers.Contains(Options.Instance.MapOpts.Layers.All[layerIndex]))
            {
                g.DrawImage(drawFace, new PointF(13, 13));
            }


            //Draw Hidden Icon
            if (hidden)
            {
                g.DrawImage(hiddenIcon, new PointF(32 - hiddenIcon.Width, 0));
            }

            g.Dispose();
            return img;
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
            grpCritter.Visible = false;
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
            nudItemRespawnTime.Value = 0;
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
            RefreshMapWarpSounds();
            grpWarp.Visible = true;
        }

        private void rbSound_CheckedChanged(object sender, EventArgs e)
        {
            HideAttributeMenus();
            grpSound.Visible = true;
            cmbMapAttributeSound.Items.Clear();
            cmbMapAttributeSound.Items.Add(Strings.General.None);
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
                return (int) MapAttribute.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                return (int) MapAttribute.Item;
            }
            else if (rbZDimension.Checked == true)
            {
                return (int) MapAttribute.ZDimension;
            }
            else if (rbNPCAvoid.Checked == true)
            {
                return (int) MapAttribute.NpcAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                return (int) MapAttribute.Warp;
            }
            else if (rbSound.Checked == true)
            {
                return (int) MapAttribute.Sound;
            }
            else if (rbResource.Checked == true)
            {
                return (int) MapAttribute.Resource;
            }
            else if (rbAnimation.Checked == true)
            {
                return (int) MapAttribute.Animation;
            }
            else if (rbGrappleStone.Checked == true)
            {
                return (int) MapAttribute.GrappleStone;
            }
            else if (rbSlide.Checked == true)
            {
                return (int) MapAttribute.Slide;
            }
            else if (rbCritter.Checked == true)
            {
                return (int) MapAttribute.Critter;
            }

            return (int) MapAttribute.Walkable;
        }

        private MapAttribute SelectedMapAttributeType
        {
            get
            {
                if (rbBlocked.Checked)
                {
                    return MapAttribute.Blocked;
                }

                if (rbItem.Checked)
                {
                    return MapAttribute.Item;
                }

                if (rbZDimension.Checked)
                {
                    return MapAttribute.ZDimension;
                }

                if (rbNPCAvoid.Checked)
                {
                    return MapAttribute.NpcAvoid;
                }

                if (rbWarp.Checked)
                {
                    return MapAttribute.Warp;
                }

                if (rbSound.Checked)
                {
                    return MapAttribute.Sound;
                }

                if (rbResource.Checked)
                {
                    return MapAttribute.Resource;
                }

                if (rbAnimation.Checked)
                {
                    return MapAttribute.Animation;
                }

                if (rbGrappleStone.Checked)
                {
                    return MapAttribute.GrappleStone;
                }

                if (rbSlide.Checked)
                {
                    return MapAttribute.Slide;
                }

                if (rbCritter.Checked)
                {
                    return MapAttribute.Critter;
                }

                return (MapAttribute) byte.MaxValue;
            }
        }

        [Obsolete("The entire switch statement should be implemented as a parameterized CreateAttribute().")]
        public GameObjects.Maps.MapAttribute CreateAttribute()
        {
            var attributeType = SelectedMapAttributeType;
            var attribute = GameObjects.Maps.MapAttribute.CreateAttribute(attributeType);
            switch (SelectedMapAttributeType)
            {
                case MapAttribute.Walkable:
                case MapAttribute.Blocked:
                case MapAttribute.GrappleStone:
                case MapAttribute.NpcAvoid:
                    break;

                case MapAttribute.Item:
                    var itemAttribute = attribute as MapItemAttribute;
                    itemAttribute.ItemId = ItemBase.IdFromList(cmbItemAttribute.SelectedIndex);
                    itemAttribute.Quantity = (int)nudItemQuantity.Value;
                    itemAttribute.RespawnTime = (long)nudItemRespawnTime.Value;
                    break;

                case MapAttribute.ZDimension:
                    var zDimensionAttribute = attribute as MapZDimensionAttribute;
                    zDimensionAttribute.GatewayTo = GetEditorDimensionGateway();
                    zDimensionAttribute.BlockedLevel = GetEditorDimensionBlock();
                    break;

                case MapAttribute.Warp:
                    var warpAttribute = attribute as MapWarpAttribute;
                    warpAttribute.MapId = MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId;
                    warpAttribute.X = (byte)nudWarpX.Value;
                    warpAttribute.Y = (byte)nudWarpY.Value;
                    warpAttribute.Direction = (WarpDirection)cmbDirection.SelectedIndex;
                    warpAttribute.ChangeInstance = chkChangeInstance.Checked;
                    warpAttribute.InstanceType = (MapInstanceType)cmbInstanceType.SelectedIndex;
                    warpAttribute.WarpSound = TextUtils.SanitizeNone(cmbWarpSound.Text);
                    break;

                case MapAttribute.Sound:
                    var soundAttribute = attribute as MapSoundAttribute;
                    soundAttribute.Distance = (byte)nudSoundDistance.Value;
                    soundAttribute.File = TextUtils.SanitizeNone(cmbMapAttributeSound.Text);
                    soundAttribute.LoopInterval = (int)nudSoundLoopInterval.Value;
                    break;

                case MapAttribute.Resource:
                    var resourceAttribute = attribute as MapResourceAttribute;
                    resourceAttribute.ResourceId = ResourceBase.IdFromList(cmbResourceAttribute.SelectedIndex);
                    resourceAttribute.SpawnLevel = (byte)(rbLevel1.Checked ? 0 : 1);
                    break;

                case MapAttribute.Animation:
                    var animationAttribute = attribute as MapAnimationAttribute;
                    animationAttribute.AnimationId = AnimationBase.IdFromList(cmbAnimationAttribute.SelectedIndex);
                    animationAttribute.IsBlock = chkAnimationBlock.Checked;
                    break;

                case MapAttribute.Slide:
                    var slideAttribute = attribute as MapSlideAttribute;
                    slideAttribute.Direction = (Direction)cmbSlideDir.SelectedIndex;
                    break;

                case MapAttribute.Critter:
                    var critterAttribute = attribute as MapCritterAttribute;
                    critterAttribute.Sprite = cmbCritterSprite.Text;
                    critterAttribute.AnimationId = AnimationBase.IdFromList(cmbCritterAnimation.SelectedIndex - 1);
                    critterAttribute.Movement = (byte)cmbCritterMovement.SelectedIndex;
                    critterAttribute.Layer = (byte)cmbCritterLayer.SelectedIndex;
                    critterAttribute.Speed = (int)nudCritterMoveSpeed.Value;
                    critterAttribute.Frequency = (int)nudCritterMoveFrequency.Value;
                    critterAttribute.IgnoreNpcAvoids = chkCritterIgnoreNpcAvoids.Checked;
                    critterAttribute.BlockPlayers = chkCritterBlockPlayers.Checked;
                    critterAttribute.Direction = (byte)cmbCritterDirection.SelectedIndex;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectedMapAttributeType), @"The currently selected attribute type has not been fully implemented.");
            }

            return attribute;
        }

        public GameObjects.Maps.MapAttribute PlaceAttribute(MapBase mapDescriptor, int x, int y, GameObjects.Maps.MapAttribute attribute = null)
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
            if (tmpMap.Attributes[x, y] != null && tmpMap.Attributes[x, y].Type != MapAttribute.Walkable)
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

            UpdateNpcCountLabel();
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

            UpdateNpcCountLabel();
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

                Core.Graphics.TilePreviewUpdated = true;
            }

            UpdateNpcCountLabel();
        }

        private void UpdateNpcCountLabel()
        {
            lblNpcCount.Text = Strings.NpcSpawns.SpawnCount.ToString(lstMapNpcs.Items.Count);
        }

        private void lstMapNpcs_Click(object sender, EventArgs e)
        {
            lstMapNpcs_Update();
        }
        
        private void lstMapNpcs_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstMapNpcs_Update();
        }

        private void lstMapNpcs_Update()
        {
            if (lstMapNpcs.Items.Count <= 0 || lstMapNpcs.SelectedIndex <= -1)
            {
                return;
            }

            var selectedSpawn = Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex];

            cmbNpc.SelectedIndex = NpcBase.ListIndex(selectedSpawn.NpcId);
            cmbDir.SelectedIndex = (int)selectedSpawn.Direction;
            rbDeclared.Checked = selectedSpawn.X >= 0;
            rbRandom.Checked = !rbDeclared.Checked;

            UpdateSpawnLocationLabel();
        }

        private void rbRandom_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex > -1)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X = -1;
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Y = -1;
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Direction = NpcSpawnDirection.Random;
                Core.Graphics.TilePreviewUpdated = true;
                UpdateSpawnLocationLabel();
            }
        }
        
        public void UpdateSpawnLocationLabel()
        {
            grpSpawnLoc.Text = rbDeclared.Checked ? Strings.NpcSpawns.spawndeclared : Strings.NpcSpawns.spawnrandom;
            grpSpawnLoc.ForeColor = rbDeclared.Checked ? System.Drawing.Color.LawnGreen : System.Drawing.Color.HotPink;
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

        private void rbCritter_CheckedChanged(object sender, EventArgs e)
        {
            cmbCritterAnimation.Items.Clear();
            cmbCritterAnimation.Items.Add(Strings.General.None);
            cmbCritterAnimation.Items.AddRange(AnimationBase.Names);
            cmbCritterAnimation.SelectedIndex = 0;

            cmbCritterSprite.Items.Clear();
            cmbCritterSprite.Items.Add(Strings.General.None);
            cmbCritterSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));
            cmbCritterSprite.SelectedIndex = 0;

            if (nudCritterMoveFrequency.Value == 0)
            {
                nudCritterMoveFrequency.Value = 1000;
            }

            if (nudCritterMoveSpeed.Value == 0)
            {
                nudCritterMoveSpeed.Value = 400;
            }

            if (!rbCritter.Checked)
            {
                return;
            }

            HideAttributeMenus();
            grpCritter.Visible = true;
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
            rbBlocked.Text = Strings.Attributes.Blocked;
            rbZDimension.Text = Strings.Attributes.ZDimension;
            rbNPCAvoid.Text = Strings.Attributes.NpcAvoid;
            rbWarp.Text = Strings.Attributes.Warp;
            rbItem.Text = Strings.Attributes.ItemSpawn;
            rbSound.Text = Strings.Attributes.MapSound;
            rbResource.Text = Strings.Attributes.ResourceSpawn;
            rbAnimation.Text = Strings.Attributes.MapAnimation;
            rbGrappleStone.Text = Strings.Attributes.Grapple;
            rbSlide.Text = Strings.Attributes.Slide;
            rbCritter.Text = Strings.Attributes.Critter;

            //Map Animation Groupbox
            grpAnimation.Text = Strings.Attributes.MapAnimation;
            lblAnimation.Text = Strings.Attributes.MapAnimation;
            chkAnimationBlock.Text = Strings.Attributes.MapAnimationBlock;

            //Slide Groupbox
            grpSlide.Text = Strings.Attributes.Slide;
            lblSlideDir.Text = Strings.Attributes.Direction;
            cmbSlideDir.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbSlideDir.Items.Add(Strings.Direction.dir[(Direction)i]);
            }

            //Map Sound
            grpSound.Text = Strings.Attributes.MapSound;
            lblMapSound.Text = Strings.Attributes.Sound;
            lblSoundDistance.Text = Strings.Attributes.Distance;
            lblSoundInterval.Text = Strings.Attributes.SoundInterval;

            //Map Item
            grpItem.Text = Strings.Attributes.ItemSpawn;
            lblMapItem.Text = Strings.Attributes.Item;
            lblMaxItemAmount.Text = Strings.Attributes.Quantity;
            lblItemRespawnTime.Text = Strings.Attributes.RespawnTime;
            tooltips.SetToolTip(lblItemRespawnTime, Strings.Attributes.RespawnTimeTooltip);
            tooltips.SetToolTip(nudItemRespawnTime, Strings.Attributes.RespawnTimeTooltip);

            //Z-Dimension
            grpZDimension.Text = Strings.Attributes.ZDimension;
            grpGateway.Text = Strings.Attributes.ZGateway;
            grpDimBlock.Text = Strings.Attributes.ZBlock;
            rbGatewayNone.Text = Strings.Attributes.ZNone;
            rbGateway1.Text = Strings.Attributes.ZLevel1;
            rbGateway2.Text = Strings.Attributes.ZLevel2;
            rbBlockNone.Text = Strings.Attributes.ZNone;
            rbBlock1.Text = Strings.Attributes.ZLevel1;
            rbBlock2.Text = Strings.Attributes.ZLevel2;

            //Warp
            grpWarp.Text = Strings.Attributes.Warp;
            lblMap.Text = Strings.Attributes.Map;
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y.ToString("");
            lblWarpDir.Text = Strings.Warping.direction.ToString("");
            cmbDirection.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Direction.dir[(Direction)i]);
            }
            lblInstance.Text = Strings.Warping.InstanceType;
            chkChangeInstance.Text = Strings.Warping.ChangeInstance;
            grpInstanceSettings.Text = Strings.Warping.MapInstancingGroup;
            lblWarpSound.Text = Strings.Warping.WarpSound;

            btnVisualMapSelector.Text = Strings.Warping.visual;

            //Resource
            grpResource.Text = Strings.Attributes.ResourceSpawn;
            lblResource.Text = Strings.Attributes.Resource;
            grpZResource.Text = Strings.Attributes.ZDimension;
            rbLevel1.Text = Strings.Attributes.ZLevel1;
            rbLevel2.Text = Strings.Attributes.ZLevel2;

            //Critter
            grpCritter.Text = Strings.Attributes.Critter;
            lblCritterSprite.Text = Strings.Attributes.CritterSprite;
            lblCritterAnimation.Text = Strings.Attributes.CritterAnimation;
            lblCritterMovement.Text = Strings.Attributes.CritterMovement;
            lblCritterLayer.Text = Strings.Attributes.CritterLayer;
            lblCritterMoveSpeed.Text = Strings.Attributes.CritterSpeed;
            lblCritterMoveFrequency.Text = Strings.Attributes.CritterFrequency;
            chkCritterIgnoreNpcAvoids.Text = Strings.Attributes.CritterIgnoreNpcAvoids;
            chkCritterBlockPlayers.Text = Strings.Attributes.CritterBlockPlayers;
            lblCritterDirection.Text = Strings.Attributes.CritterDirection;

            cmbCritterDirection.Items.Clear();
            cmbCritterDirection.Items.Add(Strings.NpcSpawns.randomdirection);
            for (var i = 0; i < 4; i++)
            {
                cmbCritterDirection.Items.Add(Strings.Direction.dir[(Direction)i]);
            }
            cmbCritterDirection.SelectedIndex = 0;

            cmbCritterMovement.Items.Clear();
            for (var i = 0; i < Strings.Attributes.CritterMovements.Count; i++)
            {
                cmbCritterMovement.Items.Add(Strings.Attributes.CritterMovements[i]);
            }
            cmbCritterMovement.SelectedIndex = 0;

            cmbCritterLayer.Items.Clear();
            for (var i = 0; i < Strings.Attributes.CritterLayers.Count; i++)
            {
                cmbCritterLayer.Items.Add(Strings.Attributes.CritterLayers[i]);
            }
            cmbCritterLayer.SelectedIndex = 1;

            //NPCS Tab
            grpSpawnDirection.Text = Strings.NpcSpawns.Direction;
            rbDeclared.Text = Strings.NpcSpawns.declaredlocation;
            rbRandom.Text = Strings.NpcSpawns.randomlocation;
            btnNpcPulseColor.Text = Strings.NpcSpawns.PulseColorButton;
            UpdateSpawnLocationLabel();
            UpdateNpcCountLabel();
            cmbDir.Items.Clear();
            cmbDir.Items.Add(Strings.NpcSpawns.randomdirection);
            for (var i = 0; i < 4; i++)
            {
                cmbDir.Items.Add(Strings.Direction.dir[(Direction)i]);
            }

            grpNpcList.Text = Strings.NpcSpawns.AddRemove;
            btnAddMapNpc.Text = Strings.NpcSpawns.add;
            btnRemoveMapNpc.Text = Strings.NpcSpawns.remove;

            lblEventInstructions.Text = Strings.MapLayers.eventinstructions;
            lblLightInstructions.Text = Strings.MapLayers.lightinstructions;
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

        private void RefreshMapWarpSounds()
        {
            cmbWarpSound.Items.Add(Strings.General.None);
            cmbWarpSound.Items.AddRange(GameContentManager.SmartSortedSoundNames);
            cmbWarpSound.SelectedIndex = 0;
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
            Globals.CurrentLayer = LayerOptions.Attributes;
            Core.Graphics.TilePreviewUpdated = true;
            btnAttributeHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Attributes;
            pnlAttributes.Show();
        }

        public void btnLightsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer != LayerOptions.Lights && Globals.CurrentLayer != LayerOptions.Events && Globals.CurrentLayer != LayerOptions.Npcs)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = LayerOptions.Lights;
            Core.Graphics.TilePreviewUpdated = true;
            btnLightsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Lights;
            pnlLights.Show();
        }

        private void btnEventsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer != LayerOptions.Lights && Globals.CurrentLayer != LayerOptions.Events && Globals.CurrentLayer != LayerOptions.Npcs)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = LayerOptions.Events;
            Core.Graphics.TilePreviewUpdated = true;
            btnEventsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Events;
            pnlEvents.Show();
        }

        private void btnNpcsHeader_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer != LayerOptions.Lights && Globals.CurrentLayer != LayerOptions.Events && Globals.CurrentLayer != LayerOptions.Npcs)
            {
                Globals.SavedTool = Globals.CurrentTool;
            }

            ChangeTab();
            Globals.CurrentLayer = LayerOptions.Npcs;
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
                var index = mMapLayers.IndexOf((PictureBox)sender);
                if (index > -1 && index < Options.Instance.MapOpts.Layers.All.Count)
                {
                    SetLayer(Options.Instance.MapOpts.Layers.All[index]);
                }
            }
            else
            {
                ToggleLayerVisibility(mMapLayers.IndexOf((PictureBox) sender));
            }
        }

        private void ToggleLayerVisibility(int index)
        {
            if (index > -1 && index < Options.Instance.MapOpts.Layers.All.Count)
            {
                LayerVisibility[Options.Instance.MapOpts.Layers.All[index]] = !LayerVisibility[Options.Instance.MapOpts.Layers.All[index]];
                SetLayer(Globals.CurrentLayer);
            }

        }

        private void picMapLayer_MouseHover(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip((PictureBox) sender, Options.Instance.MapOpts.Layers.All[mMapLayers.IndexOf((PictureBox)sender)]);
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

        private void NudItemRespawnTime_ValueChanged(object sender, System.EventArgs e)
        {
            nudItemRespawnTime.Value = Math.Max(0, nudItemRespawnTime.Value);
        }

        private void cmbMapLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMapLayer.SelectedIndex > -1)
            {
                SetLayer(Options.Instance.MapOpts.Layers.All[cmbMapLayer.SelectedIndex]);
            }
        }

        private void chkChangeInstance_CheckedChanged(object sender, EventArgs e)
        {
            grpInstanceSettings.Visible = chkChangeInstance.Checked;
        }

        private void btnNpcPulseColor_Click(object sender, EventArgs e)
        {
            NpcPulseColor = ColorTranslator.FromHtml(Preferences.LoadPreference("NpcPulseColor"));
            npcColorPulseDialog.Color = NpcPulseColor == default ? System.Drawing.Color.Red : NpcPulseColor;
            npcColorPulseDialog.FullOpen = true;
            npcColorPulseDialog.ShowDialog();
            Preferences.SavePreference("NpcPulseColor", npcColorPulseDialog.Color.ToArgb().ToString());
            NpcPulseColor = ColorTranslator.FromHtml(Preferences.LoadPreference("NpcPulseColor"));
        }
    }

}
