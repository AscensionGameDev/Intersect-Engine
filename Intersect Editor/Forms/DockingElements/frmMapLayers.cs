using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Core;
using Microsoft.Xna.Framework.Graphics;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public enum LayerTabs
    {
        Tiles = 0,
        Attributes,
        Lights,
        Events,
        Npcs
    }

    public partial class frmMapLayers : DockContent
    {
        //MonoGame Swap Chain
        private SwapChainRenderTarget _chain;
        private int _lastTileLayer;
        private List<PictureBox> _mapLayers = new List<PictureBox>();
        private bool _tMouseDown;
        public LayerTabs CurrentTab = LayerTabs.Tiles;
        public List<bool> LayerVisibility = new List<bool>();

        public frmMapLayers()
        {
            InitializeComponent();
            _mapLayers.Add(picGround);
            LayerVisibility.Add(true);
            _mapLayers.Add(picMask);
            LayerVisibility.Add(true);
            _mapLayers.Add(picMask2);
            LayerVisibility.Add(true);
            _mapLayers.Add(picFringe);
            LayerVisibility.Add(true);
            _mapLayers.Add(picFringe2);
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
            _tMouseDown = true;
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
            }
        }

        private void picTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height)
            {
                return;
            }
            if (_tMouseDown && Globals.Autotilemode == 0)
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
            _tMouseDown = false;
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
            }
        }

        public void InitTilesets()
        {
            Globals.MapLayersWindow.cmbTilesets.Items.Clear();
            foreach (var filename in Database.GetGameObjectList(GameObjectType.Tileset))
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
                Globals.MapLayersWindow.cmbTilesets.SelectedIndex = 0;
                Globals.CurrentTileset = TilesetBase.Lookup.Get(Database.GameObjectListIndex(GameObjectType.Tileset, 0));
            }
        }

        public void SetTileset(string name)
        {
            TilesetBase tSet = null;
            var tilesets = TilesetBase.Lookup;
            var index = -1;
            foreach (var tileset in tilesets)
            {
                if (tileset.Value.Name.ToLower() == name.ToLower())
                {
                    index = tileset.Key;
                    break;
                }
            }
            if (index > -1)
            {
                tSet = TilesetBase.Lookup.Get(index);
            }
            if (tSet != null)
            {
                if (File.Exists("resources/tilesets/" + tSet.Name))
                {
                    picTileset.Show();
                    Globals.CurrentTileset = tSet;
                    Globals.CurSelX = 0;
                    Globals.CurSelY = 0;
                    Texture2D tilesetTex = GameContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                        tSet.Name);
                    if (tilesetTex != null)
                    {
                        picTileset.Width = tilesetTex.Width;
                        picTileset.Height = tilesetTex.Height;
                    }
                    CreateSwapChain();
                }
            }
        }

        public void SetLayer(int index)
        {
            Globals.CurrentLayer = index;
            if (index < Options.LayerCount)
            {
                for (int i = 0; i < _mapLayers.Count; i++)
                {
                    if (i == index)
                    {
                        if (!LayerVisibility[i])
                        {
                            _mapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_A_Hide");
                        }
                        else
                        {
                            _mapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_A");
                        }
                    }
                    else
                    {
                        if (!LayerVisibility[i])
                        {
                            _mapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_B_Hide");
                        }
                        else
                        {
                            _mapLayers[i].BackgroundImage =
                                (Bitmap) Properties.Resources.ResourceManager.GetObject("_" + (i + 1) + "_B");
                        }
                    }
                }
                _lastTileLayer = index;
            }
            else
            {
            }
            EditorGraphics.TilePreviewUpdated = true;
        }

        //Mapping Attribute Functions
        /// <summary>
        ///     A method that hides all of the extra group boxes for tile data related to the map attributes.
        /// </summary>
        private void hideAttributeMenus()
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
            hideAttributeMenus();
            grpItem.Visible = true;
            cmbItemAttribute.Items.Clear();
            cmbItemAttribute.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            if (cmbItemAttribute.Items.Count > 0) cmbItemAttribute.SelectedIndex = 0;
        }

        private void rbBlocked_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
        }

        private void rbNPCAvoid_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
        }

        private void rbZDimension_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpZDimension.Visible = true;
        }

        private void rbWarp_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpWarp.Visible = true;
            nudWarpX.Maximum = Options.MapWidth;
            nudWarpY.Maximum = Options.MapHeight;
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
        }

        private void rbSound_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpSound.Visible = true;
            cmbMapAttributeSound.Items.Clear();
            cmbMapAttributeSound.Items.Add(Strings.Get("general", "none"));
            cmbMapAttributeSound.Items.AddRange(GameContentManager.GetSoundNames());
            cmbMapAttributeSound.SelectedIndex = 0;
        }

        private void rbResource_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpResource.Visible = true;
            cmbResourceAttribute.Items.Clear();
            cmbResourceAttribute.Items.AddRange(Database.GetGameObjectList(GameObjectType.Resource));
            if (cmbResourceAttribute.Items.Count > 0) cmbResourceAttribute.SelectedIndex = 0;
        }

        // Used for returning an integer value depending on which radio button is selected on the forms. This is merely used to make PlaceAtrribute less messy.
        private int GetEditorDimensionGateway()
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

        private int GetEditorDimensionBlock()
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

        public void PlaceAttribute(MapBase tmpMap, int x, int y)
        {
            tmpMap.Attributes[x, y] = new Intersect.GameObjects.Maps.Attribute();
            if (rbBlocked.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Item;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObjectType.Item,
                    cmbItemAttribute.SelectedIndex);
                tmpMap.Attributes[x, y].data2 = (int) nudItemQuantity.Value;
            }
            else if (rbZDimension.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.ZDimension;
                tmpMap.Attributes[x, y].data1 = GetEditorDimensionGateway();
                tmpMap.Attributes[x, y].data2 = GetEditorDimensionBlock();
            }
            else if (rbNPCAvoid.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.NPCAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Warp;
                tmpMap.Attributes[x, y].data1 = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                tmpMap.Attributes[x, y].data2 = (int) nudWarpX.Value;
                tmpMap.Attributes[x, y].data3 = (int) nudWarpY.Value;
                tmpMap.Attributes[x, y].data4 = (cmbDirection.SelectedIndex - 1).ToString();
            }
            else if (rbSound.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Sound;
                tmpMap.Attributes[x, y].data1 = (int) nudSoundDistance.Value;
                tmpMap.Attributes[x, y].data2 = 0;
                tmpMap.Attributes[x, y].data3 = 0;
                tmpMap.Attributes[x, y].data4 = cmbMapAttributeSound.Text;
            }
            else if (rbResource.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Resource;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObjectType.Resource,
                    cmbResourceAttribute.SelectedIndex);
                if (rbLevel1.Checked == true)
                {
                    tmpMap.Attributes[x, y].data2 = 0;
                }
                else
                {
                    tmpMap.Attributes[x, y].data2 = 1;
                }
            }
            else if (rbAnimation.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Animation;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObjectType.Animation,
                    cmbAnimationAttribute.SelectedIndex);
            }
            else if (rbGrappleStone.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.GrappleStone;
            }
            else if (rbSlide.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int) MapAttributes.Slide;
                tmpMap.Attributes[x, y].data1 = cmbSlideDir.SelectedIndex;
            }
        }

        public bool RemoveAttribute(MapBase tmpMap, int x, int y)
        {
            if (tmpMap.Attributes[x, y] != null && tmpMap.Attributes[x, y].value > 0)
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
            cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));

            // Add the map NPCs
            lstMapNpcs.Items.Clear();
            for (int i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
            {
                lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcNum));
            }

            // Don't select if there are no NPCs, to avoid crashes.
            if (cmbNpc.Items.Count > 0) cmbNpc.SelectedIndex = 0;
            cmbDir.SelectedIndex = 0;
            rbRandom.Checked = true;
            if (lstMapNpcs.Items.Count > 0)
            {
                lstMapNpcs.SelectedIndex = 0;
                cmbDir.SelectedIndex = Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
                cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Npc,
                    Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum);
                if (Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                {
                    rbDeclared.Checked = true;
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
                n.NpcNum = Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);
                n.X = -1;
                n.Y = -1;
                n.Dir = -1;

                Globals.CurrentMap.Spawns.Add(n);
                lstMapNpcs.Items.Add(NpcBase.GetName(n.NpcNum));
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
                for (int i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcNum));
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
                cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Npc,
                    Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum);
                cmbDir.SelectedIndex = Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
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
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Dir = -1;
            }
        }

        private void cmbDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].Dir = cmbDir.SelectedIndex - 1;
            }
        }

        private void cmbNpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;

            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum =
                    Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);

                // Refresh List
                n = lstMapNpcs.SelectedIndex;
                lstMapNpcs.Items.Clear();
                for (int i = 0; i < Globals.CurrentMap.Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(NpcBase.GetName(Globals.CurrentMap.Spawns[i].NpcNum));
                }
                lstMapNpcs.SelectedIndex = n;
            }
        }

        private void lightEditor_Load(object sender, EventArgs e)
        {
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, (int) nudWarpX.Value,
                (int) nudWarpY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                cmbWarpMap.Items.Clear();
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
                }
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
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
            hideAttributeMenus();
            grpAnimation.Visible = true;
            cmbAnimationAttribute.Items.Clear();
            cmbAnimationAttribute.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (cmbAnimationAttribute.Items.Count > 0) cmbAnimationAttribute.SelectedIndex = 0;
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
            Text = Strings.Get("maplayers", "title");
            btnTileHeader.Text = Strings.Get("maplayers", "tiles");
            btnAttributeHeader.Text = Strings.Get("maplayers", "attributes");
            btnEventsHeader.Text = Strings.Get("maplayers", "events");
            btnLightsHeader.Text = Strings.Get("maplayers", "lights");
            btnNpcsHeader.Text = Strings.Get("maplayers", "npcs");

            //Tiles Panel
            lblLayer.Text = Strings.Get("tiles", "layer");
            lblTileset.Text = Strings.Get("tiles", "tileset");
            lblTileType.Text = Strings.Get("tiles", "tiletype");
            cmbAutotile.Items.Clear();
            cmbAutotile.Items.Add(Strings.Get("tiles", "normal"));
            cmbAutotile.Items.Add(Strings.Get("tiles", "autotile"));
            cmbAutotile.Items.Add(Strings.Get("tiles", "fake"));
            cmbAutotile.Items.Add(Strings.Get("tiles", "animated"));
            cmbAutotile.Items.Add(Strings.Get("tiles", "cliff"));
            cmbAutotile.Items.Add(Strings.Get("tiles", "waterfall"));

            //Attributes Panel
            rbBlocked.Text = Strings.Get("attributes", "blocked");
            rbZDimension.Text = Strings.Get("attributes", "zdimension");
            rbNPCAvoid.Text = Strings.Get("attributes", "npcavoid");
            rbWarp.Text = Strings.Get("attributes", "warp");
            rbItem.Text = Strings.Get("attributes", "itemspawn");
            rbSound.Text = Strings.Get("attributes", "mapsound");
            rbResource.Text = Strings.Get("attributes", "resourcespawn");
            rbAnimation.Text = Strings.Get("attributes", "mapanimation");
            rbGrappleStone.Text = Strings.Get("attributes", "grapple");
            rbSlide.Text = Strings.Get("attributes", "slide");

            //Map Animation Groupbox
            grpAnimation.Text = Strings.Get("attributes", "mapanimation");
            lblAnimation.Text = Strings.Get("attributes", "mapanimation");

            //Slide Groupbox
            grpSlide.Text = Strings.Get("attributes", "slide");
            lblSlideDir.Text = Strings.Get("attributes", "dir");
            cmbSlideDir.Items.Clear();
            for (int i = -1; i < 4; i++)
            {
                cmbSlideDir.Items.Add(Strings.Get("directions", i.ToString()));
            }

            //Map Sound
            grpSound.Text = Strings.Get("attributes", "mapsound");
            lblMapSound.Text = Strings.Get("attributes", "sound");
            lblSoundDistance.Text = Strings.Get("attributes", "distance");

            //Map Item
            grpItem.Text = Strings.Get("attributes", "itemspawn");
            lblMapItem.Text = Strings.Get("attributes", "item");
            lblMaxItemAmount.Text = Strings.Get("attributes", "quantity");

            //Z-Dimension
            grpZDimension.Text = Strings.Get("attributes", "zdimension");
            grpGateway.Text = Strings.Get("attributes", "zgateway");
            grpDimBlock.Text = Strings.Get("attributes", "zBlock");
            rbGatewayNone.Text = Strings.Get("attributes", "znone");
            rbGateway1.Text = Strings.Get("attributes", "zlevel1");
            rbGateway2.Text = Strings.Get("attributes", "zlevel2");
            rbBlockNone.Text = Strings.Get("attributes", "znone");
            rbBlock1.Text = Strings.Get("attributes", "zlevel1");
            rbBlock2.Text = Strings.Get("attributes", "zlevel2");

            //Warp
            grpWarp.Text = Strings.Get("attributes", "warp");
            lblMap.Text = Strings.Get("warping", "map", "");
            lblX.Text = Strings.Get("warping", "x", "");
            lblY.Text = Strings.Get("warping", "y", "");
            lblWarpDir.Text = Strings.Get("warping", "direction", "");
            cmbDirection.Items.Clear();
            for (int i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Get("directions", i.ToString()));
            }
            btnVisualMapSelector.Text = Strings.Get("warping", "visual");

            //Resource
            grpResource.Text = Strings.Get("attributes", "resourcespawn");
            lblResource.Text = Strings.Get("attributes", "resource");
            grpZResource.Text = Strings.Get("attributes", "zdimension");
            rbLevel1.Text = Strings.Get("attributes", "zlevel1");
            rbLevel2.Text = Strings.Get("attributes", "zlevel2");

            //NPCS Tab
            grpSpawnLoc.Text = rbDeclared.Checked
                ? Strings.Get("npcspawns", "spawndeclared")
                : Strings.Get("npcspawns", "spawnrandom");
            rbDeclared.Text = Strings.Get("npcspawns", "declaredlocation");
            rbRandom.Text = Strings.Get("npcspawns", "randomlocation");
            lblDir.Text = Strings.Get("npcspawns", "direction");
            cmbDir.Items.Clear();
            cmbDir.Items.Add(Strings.Get("npcspawns", "randomdirection"));
            for (int i = 0; i < 4; i++)
            {
                cmbDir.Items.Add(Strings.Get("directions", i.ToString()));
            }
            grpNpcList.Text = Strings.Get("npcspawns", "addremove");
            btnAddMapNpc.Text = Strings.Get("npcspawns", "add");
            btnRemoveMapNpc.Text = Strings.Get("npcspawns", "remove");

            lblEventInstructions.Text = Strings.Get("maplayers", "eventinstructions");
            lblLightInstructions.Text = Strings.Get("maplayers", "lightinstructions");

            for (int i = 0; i < _mapLayers.Count; i++)
            {
                _mapLayers[i].Text = Strings.Get("tiles", "layer" + i);
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
                if (_chain != null)
                {
                    _chain.Dispose();
                }
                if (EditorGraphics.GetGraphicsDevice() != null)
                {
                    _chain = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(), picTileset.Handle,
                        picTileset.Width, picTileset.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0,
                        RenderTargetUsage.DiscardContents, PresentInterval.Immediate);
                    EditorGraphics.SetTilesetChain(_chain);
                }
            }
        }

        private void rbGrappleStone_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
        }

        private void rbSlide_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
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
            if (Globals.EditingLight != null)
            {
                Globals.MapLayersWindow.lightEditor.Cancel();
            }
        }

        private void btnTileHeader_Click(object sender, EventArgs e)
        {
            ChangeTab();
            SetLayer(_lastTileLayer);
            EditorGraphics.TilePreviewUpdated = true;
            btnTileHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Tiles;
            pnlTiles.Show();
        }

        private void btnAttributeHeader_Click(object sender, EventArgs e)
        {
            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount;
            EditorGraphics.TilePreviewUpdated = true;
            btnAttributeHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Attributes;
            pnlAttributes.Show();
        }

        public void btnLightsHeader_Click(object sender, EventArgs e)
        {
            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 1;
            EditorGraphics.TilePreviewUpdated = true;
            btnLightsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Lights;
            pnlLights.Show();
        }

        private void btnEventsHeader_Click(object sender, EventArgs e)
        {
            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 2;
            EditorGraphics.TilePreviewUpdated = true;
            btnEventsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Events;
            pnlEvents.Show();
        }

        private void btnNpcsHeader_Click(object sender, EventArgs e)
        {
            ChangeTab();
            Globals.CurrentLayer = Options.LayerCount + 3;
            EditorGraphics.TilePreviewUpdated = true;
            RefreshNpcList();
            btnNpcsHeader.BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
            CurrentTab = LayerTabs.Npcs;
            pnlNpcs.Show();
        }

        private void picMapLayer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetLayer(_mapLayers.IndexOf((PictureBox) sender));
            }
            else
            {
                ToggleLayerVisibility(_mapLayers.IndexOf((PictureBox) sender));
            }
        }

        private void ToggleLayerVisibility(int index)
        {
            LayerVisibility[index] = !LayerVisibility[index];
            SetLayer(Globals.CurrentLayer);
        }

        private void picMapLayer_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip((PictureBox) sender, Strings.Get("tiles", "layer" + _mapLayers.IndexOf((PictureBox) sender)));
        }
    }
}