using Intersect_Editor.Classes;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMapLayers : DockContent
    {
        private bool _tMouseDown;
        public frmMapLayers()
        {
            InitializeComponent();
        }

        public void Init()
        {
            cmbAutotile.SelectedIndex = 0;
            cmbMapLayer.SelectedIndex = 0;
        }

        //Tiles Tab
        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTileset(cmbTilesets.SelectedIndex);
        }
        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            _tMouseDown = true;
            Globals.CurSelX = (int)Math.Floor((double)e.X / Globals.TileWidth);
            Globals.CurSelY = (int)Math.Floor((double)e.Y / Globals.TileHeight);
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
                var tmpX = (int)Math.Floor((double)e.X / Globals.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Globals.TileHeight);
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
        public void SetTileset(int index)
        {
            cmbTilesets.SelectedIndex = index;
            if (File.Exists("Resources/Tilesets/" + Globals.Tilesets[cmbTilesets.SelectedIndex]))
            {
                Globals.CurrentTileset = cmbTilesets.SelectedIndex;
                Graphics.InitTileset(Globals.CurrentTileset, Globals.MainForm);
            }
            else
            {
                cmbTilesets.SelectedIndex = Globals.CurrentTileset;
            }
        }
        public void SetLayer(int index)
        {
            Globals.CurrentLayer = index;
            if (index < Constants.LayerCount)
            {
                cmbMapLayer.SelectedIndex = index;
            }
        }

        //Lights Tab
        private void txtLightOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var offsetY = Convert.ToInt32(txtLightOffsetY.Text);
                Globals.EditingLight.OffsetY = offsetY;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var offsetX = Convert.ToInt32(txtLightOffsetX.Text);
                Globals.EditingLight.OffsetX = offsetX;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
                Graphics.TilePreviewUpdated = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void scrlLightRange_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            txtLightRange.Text = "" + Globals.EditingLight.Range;
            if (Globals.EditingLight.Graphic != null) { Globals.EditingLight.Graphic.Dispose(); }
            Globals.EditingLight.Graphic = null;
            Globals.EditingLight.Range = scrlLightRange.Value;
            Graphics.LightsChanged = true;
            Graphics.TilePreviewUpdated = true;
        }
        private void scrlLightIntensity_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            Globals.EditingLight.Intensity = scrlLightIntensity.Value / 10000.0;
            if (Globals.EditingLight.Graphic != null) { Globals.EditingLight.Graphic.Dispose(); }
            Globals.EditingLight.Graphic = null;
            txtLightIntensity.Text = "" + Globals.EditingLight.Intensity;
            Graphics.LightsChanged = true;
            Graphics.TilePreviewUpdated = true;
        }
        private void txtLightIntensity_TextChanged(object sender, EventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            try
            {
                var intensity = Convert.ToDouble(txtLightIntensity.Text);
                if (intensity < 0)
                {
                    intensity = 0;
                }
                if (intensity > 1)
                {
                    intensity = 1;
                }
                Globals.EditingLight.Intensity = intensity;
                Globals.EditingLight.Graphic = null;
                txtLightIntensity.Text = "" + Globals.EditingLight.Intensity;
                scrlLightIntensity.Value = (int)(intensity * 10000.0);
                Graphics.LightsChanged = true;
                Graphics.TilePreviewUpdated = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var range = Convert.ToInt32(txtLightRange.Text);
                if (range < 2)
                {
                    range = 2;
                }
                if (range > 179)
                {
                    range = 179;
                }
                Globals.EditingLight.Range = range;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
                Graphics.TilePreviewUpdated = true;
                txtLightRange.Text = "" + range;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            pnlLight.Hide();
            Globals.EditingLight = null;
        }
        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            Globals.EditingLight.Intensity = Globals.BackupLight.Intensity;
            Globals.EditingLight.Range = Globals.BackupLight.Range;
            Globals.EditingLight.OffsetX = Globals.BackupLight.OffsetX;
            Globals.EditingLight.OffsetY = Globals.BackupLight.OffsetY;
            Globals.EditingLight.Graphic.Dispose();
            Globals.EditingLight.Graphic = null;
            Graphics.LightsChanged = false;
            Globals.EditingLight = null;
            Graphics.TilePreviewUpdated = true;
            pnlLight.Hide();
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
        }
        private void rbItem_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpItem.Visible = true;
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
        }
        private void rbSound_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpSound.Visible = true;
        }
        private void rbResource_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpResource.Visible = true;
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
        public void PlaceAttribute(MapStruct tmpMap, int x, int y)
        {
            if (rbBlocked.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.Item;
                tmpMap.Attributes[x, y].data1 = scrlMapItem.Value - 1;
                tmpMap.Attributes[x, y].data2 = scrlMaxItemVal.Value;
            }
            else if (rbZDimension.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.ZDimension;
                tmpMap.Attributes[x, y].data1 = GetEditorDimensionGateway();
                tmpMap.Attributes[x, y].data2 = GetEditorDimensionBlock();
            }
            else if (rbNPCAvoid.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.NPCAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.Warp;
                tmpMap.Attributes[x, y].data1 = scrlMap.Value;
                tmpMap.Attributes[x, y].data2 = scrlX.Value;
                tmpMap.Attributes[x, y].data3 = scrlY.Value;
            }
            else if (rbSound.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.Sound;
                tmpMap.Attributes[x, y].data1 = scrlSoundDistance.Value;
                tmpMap.Attributes[x, y].data2 = 0;
                tmpMap.Attributes[x, y].data3 = 0;
                tmpMap.Attributes[x, y].data4 = cmbMapAttributeSound.Text;
            }
            else if (rbResource.Checked == true)
            {
                tmpMap.Attributes[x, y].value = (int)Enums.MapAttributes.Resource;
                tmpMap.Attributes[x, y].data1 = scrlResource.Value;
            }
        }
        public bool RemoveAttribute(MapStruct tmpMap, int x, int y)
        {
            if (tmpMap.Attributes[x, y].value > 0)
            {
                tmpMap.Attributes[x, y].value = 0;
                tmpMap.Attributes[x, y].data1 = 0;
                tmpMap.Attributes[x, y].data2 = 0;
                tmpMap.Attributes[x, y].data3 = 0;
                tmpMap.Attributes[x, y].data4 = "";
                return true;
            }
            return false;
        }
        //Item Attribute
        private void scrlMapItem_ValueChanged(object sender, EventArgs e)
        {
            lblMapItem.Text = "Item: " + scrlMapItem.Value + " " + Globals.GameItems[scrlMapItem.Value - 1].Name;
        }
        private void scrlMaxItemVal_ValueChanged(object sender, EventArgs e)
        {
            lblMaxItemAmount.Text = "Quantity: x" + scrlMaxItemVal.Value;
        }
        //Warp Attribute
        private void scrlMap_ValueChanged(object sender, EventArgs e)
        {
            lblMap.Text = "Map: " + scrlMap.Value;
        }
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
        //Resource Attribute
        private void scrlResource_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlResource.Value >= 0)
            {
                lblResource.Text = "Resource: " + (scrlResource.Value + 1) + " " + Globals.GameResources[scrlResource.Value].Name;
            }
            else
            {
                lblResource.Text = "Resource: 0 None";
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.TabPages.IndexOf(tabTiles)) {
                Globals.CurrentLayer = 0;
                Graphics.TilePreviewUpdated = true;
                Globals.SelectionType = (int) Enums.SelectionTypes.AllLayers;
            }
            else if (tabControl.SelectedIndex == tabControl.TabPages.IndexOf(tabAttributes)) {
                Globals.CurrentLayer = Constants.LayerCount;
                Graphics.TilePreviewUpdated = true;
                Globals.SelectionType = (int)Enums.SelectionTypes.AllLayers;
            }  
            else if (tabControl.SelectedIndex == tabControl.TabPages.IndexOf(tabEvents)) {
                Globals.CurrentLayer = Constants.LayerCount + 2;
                Graphics.TilePreviewUpdated = true;
                Globals.SelectionType = (int)Enums.SelectionTypes.CurrentLayer;
            }  
            else if (tabControl.SelectedIndex == tabControl.TabPages.IndexOf(tabLights)) {
                Globals.CurrentLayer =  Constants.LayerCount + 1;
                Graphics.TilePreviewUpdated = true;
                Globals.SelectionType = (int)Enums.SelectionTypes.CurrentLayer;
            }
            else if (tabControl.SelectedIndex == tabControl.TabPages.IndexOf(tabNPCs ))
            {
                Globals.CurrentLayer = Constants.LayerCount + 3;
                Graphics.TilePreviewUpdated = true;
                Globals.SelectionType = (int)Enums.SelectionTypes.CurrentLayer;
                // Update the list incase npcs have been modified since form load.
                cmbNpc.Items.Clear();
                cmbNpc.Items.Add("None");
                for (int i = 0; i < Constants.MaxNpcs; i++)
                {
                    cmbNpc.Items.Add(i + ") " + Globals.GameNpcs[i].Name);
                }

                // Add the map NPCs
                lstMapNpcs.Items.Clear();
                for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
                }

                // Don't select if there are no NPCs, to avoid crashes.
                cmbNpc.SelectedIndex = 0;
                cmbDir.SelectedIndex = 0;
                rbRandom.Checked = true;
                if (lstMapNpcs.Items.Count > 0)
                {
                    lstMapNpcs.SelectedIndex = 0;
                    cmbDir.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
                    cmbNpc.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum + 1;
                    if (Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                    {
                        rbDeclared.Checked = true;
                    }
                }
            }
            if (Globals.EditingLight != null)
            {
                btnLightEditorRevert_Click(null, null);
            }
        }

        private void cmbMapLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.CurrentLayer = cmbMapLayer.SelectedIndex;
            Graphics.TilePreviewUpdated = true;
        }

        private void frmMapLayers_DockStateChanged(object sender, EventArgs e)
        {
            if (Graphics.TilesetWindow != null && !Globals.MapLayersWindow.picTileset.IsDisposed)
            {
                Graphics.TilesetWindow.Close();
                Graphics.TilesetWindow.Dispose();
                Graphics.TilesetWindow = new RenderWindow(Globals.MapLayersWindow.picTileset.Handle);
            }
        }

        //Map NPC List
        private void btnAddMapNpc_Click(object sender, EventArgs e)
        {
            var n = new NpcSpawn();

            //Don't add nothing
            if (cmbNpc.SelectedIndex > 0)
            {
                n.NpcNum = cmbNpc.SelectedIndex - 1;
                n.X = -1;
                n.Y = -1;
                n.Dir = -1;

                Globals.GameMaps[Globals.CurrentMap].Spawns.Add(n);

                lstMapNpcs.Items.Add(Convert.ToString(lstMapNpcs.Items.Count + 1) + ") " + Globals.GameNpcs[cmbNpc.SelectedIndex - 1].Name);
                lstMapNpcs.SelectedIndex = lstMapNpcs.Items.Count - 1;
            }
        }
        private void btnRemoveMapNpc_Click(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Spawns.RemoveAt(lstMapNpcs.SelectedIndex);
            lstMapNpcs.Items.RemoveAt(lstMapNpcs.SelectedIndex);

            // Refresh List
            lstMapNpcs.Items.Clear();
            for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
            {
                lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
            }

            if (lstMapNpcs.Items.Count > 0)
            {
                lstMapNpcs.SelectedIndex = 0;
            }
        }
        private void lstMapNpcs_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.Items.Count > 0)
            {
                cmbNpc.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum + 1;
                cmbDir.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
                if (Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X >= 0)
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
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X = -1;
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Y = -1;
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir = -1;
        }
        private void cmbDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir = cmbDir.SelectedIndex - 1;
            }
        }
        private void cmbNpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;

            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum = cmbNpc.SelectedIndex - 1;

                // Refresh List
                n = lstMapNpcs.SelectedIndex;
                lstMapNpcs.Items.Clear();
                for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
                }
                lstMapNpcs.SelectedIndex = n;
            }
        }
    }
}
