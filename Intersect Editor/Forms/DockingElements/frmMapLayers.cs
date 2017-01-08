using Intersect_Editor.Classes;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
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
        private bool _tMouseDown;
        public LayerTabs CurrentTab = LayerTabs.Tiles;
        public frmMapLayers()
        {
            InitializeComponent();
        }

        public void Init()
        {
            cmbAutotile.SelectedIndex = 0;
            cmbMapLayer.SelectedIndex = 0;
            if (cmbTilesets.Items.Count > 0)
            {
                SetTileset(cmbTilesets.Items[0].ToString());
            }
        }

        //Tiles Tab
        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTileset(cmbTilesets.Text);
        }
        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            _tMouseDown = true;
            Globals.CurSelX = (int)Math.Floor((double)e.X / Options.TileWidth);
            Globals.CurSelY = (int)Math.Floor((double)e.Y / Options.TileHeight);
            Globals.CurSelW = 0;
            Globals.CurSelH = 0;
            if (Globals.CurSelX < 0) { Globals.CurSelX = 0; }
            if (Globals.CurSelY < 0) { Globals.CurSelY = 0; }
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
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            if (_tMouseDown && Globals.Autotilemode == 0)
            {
                var tmpX = (int)Math.Floor((double)e.X / Options.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Options.TileHeight);
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
            foreach (var filename in Database.GetGameObjectList(GameObject.Tileset))
            {
                if (File.Exists("resources/tilesets/" + filename))
                {
                    Globals.MapLayersWindow.cmbTilesets.Items.Add(filename);
                }
                else
                {
                }
            }
            if (TilesetBase.ObjectCount() > 0)
            {
                Globals.MapLayersWindow.cmbTilesets.SelectedIndex = 0;
                Globals.CurrentTileset = TilesetBase.GetTileset(Database.GameObjectListIndex(GameObject.Tileset,0));
            }
        }
        public void SetTileset(string name)
        {
            TilesetBase tSet = null;
            var tilesets = TilesetBase.GetObjects();
            var index = -1;
            foreach (var tileset in tilesets)
            {
                if (tileset.Value.Value.ToLower() == name.ToLower())
                {
                    index = tileset.Key;
                    break;
                }
            }
            if (index > -1)
            {
                tSet = TilesetBase.GetTileset(index);
            }
            if (tSet != null)
            {
                if (File.Exists("resources/tilesets/" + tSet.Value))
                {
                    picTileset.Show();
                    Globals.CurrentTileset = tSet;
                    Globals.CurSelX = 0;
                    Globals.CurSelY = 0;
                    Texture2D tilesetTex = GameContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                        tSet.Value);
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
                cmbMapLayer.SelectedIndex = index;
            }
        }

        //Mapping Attribute Functions
        /// <summary>
        /// A method that hides all of the extra group boxes for tile data related to the map attributes.
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
            cmbItemAttribute.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
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
            cmbMapAttributeSound.Items.Add("None");
            cmbMapAttributeSound.Items.AddRange(GameContentManager.GetSoundNames());
            cmbMapAttributeSound.SelectedIndex = 0;
        }
        private void rbResource_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpResource.Visible = true;
            cmbResourceAttribute.Items.Clear();
            cmbResourceAttribute.Items.AddRange(Database.GetGameObjectList(GameObject.Resource));
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
            tmpMap.Attributes[x, y] = new Intersect_Library.GameObjects.Maps.Attribute();
            if (rbBlocked.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Item;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObject.Item,cmbItemAttribute.SelectedIndex);
                tmpMap.Attributes[x, y].data2 = scrlMaxItemVal.Value;
            }
            else if (rbZDimension.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.ZDimension;
                tmpMap.Attributes[x, y].data1 = GetEditorDimensionGateway();
                tmpMap.Attributes[x, y].data2 = GetEditorDimensionBlock();
            }
            else if (rbNPCAvoid.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.NPCAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Warp;
                tmpMap.Attributes[x, y].data1 = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                tmpMap.Attributes[x, y].data2 = scrlX.Value;
                tmpMap.Attributes[x, y].data3 = scrlY.Value;
                tmpMap.Attributes[x, y].data4 = (cmbDirection.SelectedIndex - 1).ToString();
            }
            else if (rbSound.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Sound;
                tmpMap.Attributes[x, y].data1 = scrlSoundDistance.Value;
                tmpMap.Attributes[x, y].data2 = 0;
                tmpMap.Attributes[x, y].data3 = 0;
                tmpMap.Attributes[x, y].data4 = cmbMapAttributeSound.Text;
            }
            else if (rbResource.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Resource;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObject.Resource,cmbResourceAttribute.SelectedIndex);
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
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Animation;
                tmpMap.Attributes[x, y].data1 = Database.GameObjectIdFromList(GameObject.Animation, cmbAnimationAttribute.SelectedIndex);
            }
            else if (rbGrappleStone.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.GrappleStone;
            }
            else if (rbSlide.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)MapAttributes.Slide;
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
        //Item Attribute
        private void scrlMaxItemVal_ValueChanged(object sender, EventArgs e)
        {
            lblMaxItemAmount.Text = "Quantity: x" + scrlMaxItemVal.Value;
        }
        //Warp Attribute
        private void scrlX_ValueChanged(object sender, EventArgs e)
        {
            lblX.Text = "X: " + scrlX.Value;
        }
        private void scrlY_ValueChanged(object sender, EventArgs e)
        {
            lblY.Text = "Y: " + scrlY.Value;
        }
        //Sound Attribute
        private void scrlSoundDistance_Scroll(object sender, ScrollEventArgs e)
        {
            lblSoundDistance.Text = "Distance: " + scrlSoundDistance.Value + " Tile(s)";
        }

        public void RefreshNpcList()
        {
            // Update the list incase npcs have been modified since form load.
            cmbNpc.Items.Clear();
            cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObject.Npc));

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
                cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObject.Npc, Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum);
                if (Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                {
                    rbDeclared.Checked = true;
                }
            }
        }

        private void cmbMapLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.CurrentLayer = cmbMapLayer.SelectedIndex;
            EditorGraphics.TilePreviewUpdated = true;
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
                n.NpcNum = Database.GameObjectIdFromList(GameObject.Npc, cmbNpc.SelectedIndex);
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
                cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObject.Npc,Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum);
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
                Globals.CurrentMap.Spawns[lstMapNpcs.SelectedIndex].NpcNum = Database.GameObjectIdFromList(GameObject.Npc,cmbNpc.SelectedIndex);

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
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, scrlX.Value, scrlY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                scrlX.Value = frmWarpSelection.GetX();
                scrlY.Value = frmWarpSelection.GetY();
                lblX.Text = @"X: " + scrlX.Value;
                lblY.Text = @"Y: " + scrlY.Value;
            }
        }

        private void rbAnimation_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpAnimation.Visible = true;
            cmbAnimationAttribute.Items.Clear();
            cmbAnimationAttribute.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            if (cmbAnimationAttribute.Items.Count > 0) cmbAnimationAttribute.SelectedIndex = 0;
        }

        private void frmMapLayers_Load(object sender, EventArgs e)
        {
            CreateSwapChain();
            picTileset.MouseDown += pnlTilesetContainer.AutoDragPanel_MouseDown;
            picTileset.MouseUp += pnlTilesetContainer.AutoDragPanel_MouseUp;
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
                    _chain = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(), this.picTileset.Handle,
                        this.picTileset.Width, this.picTileset.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0,
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
            Globals.CurrentLayer = cmbMapLayer.SelectedIndex;
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
    }
}
