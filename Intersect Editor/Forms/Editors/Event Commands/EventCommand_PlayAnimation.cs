/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_PlayAnimation : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        private MapBase _currentMap;
        private EventBase _editingEvent;
        private int spawnX = 0;
        private int spawnY = 0;
        public EventCommand_PlayAnimation(FrmEvent eventEditor, MapBase currentMap, EventBase currentEvent, EventCommand editingCommand)
        {
            InitializeComponent();
            _myCommand = editingCommand;
            _eventEditor = eventEditor;
            _editingEvent = currentEvent;
            _currentMap = currentMap;
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbAnimation.SelectedIndex = _myCommand.Ints[0];
            cmbConditionType.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _myCommand.Ints[1]);
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile spawn
                    //Fill in the map cmb
                    scrlX.Value = _myCommand.Ints[3];
                    scrlY.Value = _myCommand.Ints[4];
                    lblX.Text = @"X: " + scrlX.Value;
                    lblY.Text = @"Y: " + scrlY.Value;
                    cmbDirection.SelectedIndex = _myCommand.Ints[5];
                    break;
                case 1: //On/Around Entity Spawn
                    spawnX = _myCommand.Ints[3];
                    spawnY = _myCommand.Ints[4];
                    switch (_myCommand.Ints[5])
                    {
                        //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                        case 1:
                            chkRelativeLocation.Checked = true;
                            break;
                        case 2:
                            chkRotateDirection.Checked = true;
                            break;
                        case 3:
                            chkRelativeLocation.Checked = true;
                            chkRotateDirection.Checked = true;
                            break;
                    }
                    UpdateSpawnPreview();
                    break;
            }
        }

        private void UpdateFormElements()
        {
            grpTileSpawn.Hide();
            grpEntitySpawn.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile Spawn
                    grpTileSpawn.Show();
                    cmbMap.Items.Clear();
                    for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                    {
                        cmbMap.Items.Add(MapList.GetOrderedMaps()[i].MapNum + ". " + MapList.GetOrderedMaps()[i].Name);
                        if (MapList.GetOrderedMaps()[i].MapNum == _myCommand.Ints[2])
                        {
                            cmbMap.SelectedIndex = i;
                        }
                    }
                    if (cmbMap.SelectedIndex == -1)
                    {
                        cmbMap.SelectedIndex = 0;
                    }
                    break;
                case 1: //On/Around Entity Spawn
                    grpEntitySpawn.Show();
                    cmbEntities.Items.Clear();
                    cmbEntities.Items.Add("Player");
                    cmbEntities.SelectedIndex = 0;

                    if (!_editingEvent.CommonEvent)
                    {
                        foreach (var evt in _currentMap.Events)
                        {
                            cmbEntities.Items.Add(evt.Key == _editingEvent.MyIndex ? "[THIS EVENT] " : "" + evt.Value.MyName);
                            if (_myCommand.Ints[2] == evt.Key) cmbEntities.SelectedIndex = cmbEntities.Items.Count - 1;
                        }
                    }
                    UpdateSpawnPreview();


                    break;
            }
        }

        private void UpdateSpawnPreview()
        {
            Bitmap destBitmap = new Bitmap(pnlSpawnLoc.Width, pnlSpawnLoc.Height);
            Font renderFont = new Font(new FontFamily("Arial"), 14);
            ;
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(destBitmap);
            g.Clear(Color.White);
            g.FillRectangle(Brushes.Red, new Rectangle((spawnX + 2) * 32, (spawnY + 2) * 32, 32, 32));
            for (int x = 0; x < 5; x++)
            {
                g.DrawLine(Pens.Black, x * 32, 0, x * 32, 32 * 5);
                g.DrawLine(Pens.Black, 0, x * 32, 32 * 5, x * 32);
            }
            g.DrawLine(Pens.Black, 0, 32 * 5 - 1, 32 * 5, 32 * 5 - 1);
            g.DrawLine(Pens.Black, 32 * 5 - 1, 0, 32 * 5 - 1, 32 * 5 - 1);
            g.DrawString("E", renderFont, Brushes.Black, pnlSpawnLoc.Width / 2 - g.MeasureString("E", renderFont).Width / 2,
                pnlSpawnLoc.Height / 2 - g.MeasureString("S", renderFont).Height / 2);
            g.Dispose();
            pnlSpawnLoc.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Animation, cmbAnimation.SelectedIndex);
            _myCommand.Ints[1] = cmbConditionType.SelectedIndex;
            switch (_myCommand.Ints[1])
            {
                case 0: //Tile Spawn
                    _myCommand.Ints[2] = MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum;
                    _myCommand.Ints[3] = scrlX.Value;
                    _myCommand.Ints[4] = scrlY.Value;
                    _myCommand.Ints[5] = cmbDirection.SelectedIndex;
                    break;
                case 1: //On/Around Entity Spawn
                    if (cmbEntities.SelectedIndex == 0 || cmbEntities.SelectedIndex == -1)
                    {
                        _myCommand.Ints[2] = -1;
                    }
                    else
                    {
                        _myCommand.Ints[2] = _currentMap.Events.Keys.ToList()[cmbEntities.SelectedIndex - 1];
                    }
                    _myCommand.Ints[3] = spawnX;
                    _myCommand.Ints[4] = spawnY;
                    if (chkRelativeLocation.Checked && chkRotateDirection.Checked)
                    {
                        _myCommand.Ints[5] = 3; //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRelativeLocation.Checked)
                    {
                        _myCommand.Ints[5] = 1; //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRotateDirection.Checked)
                    {
                        _myCommand.Ints[5] = 2; //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else
                    {
                        _myCommand.Ints[5] = 0; //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }

                    break;
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void scrlX_Scroll(object sender, ScrollEventArgs e)
        {
            lblX.Text = @"X: " + scrlX.Value;
        }

        private void scrlY_Scroll(object sender, ScrollEventArgs e)
        {
            lblY.Text = @"Y: " + scrlY.Value;
        }

        private void btnVisual_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum, scrlX.Value, scrlY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
                    {
                        cmbMap.SelectedIndex = i;
                        break;
                    }
                }
                scrlX.Value = frmWarpSelection.GetX();
                scrlY.Value = frmWarpSelection.GetY();
                lblX.Text = @"X: " + scrlX.Value;
                lblY.Text = @"Y: " + scrlY.Value;
            }
        }

        private void pnlSpawnLoc_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= 0 && e.Y >= 0 && e.X < pnlSpawnLoc.Width && e.Y < pnlSpawnLoc.Height)
            {
                spawnX = (int)Math.Floor((double)(e.X) / Options.TileWidth) - 2;
                spawnY = (int)Math.Floor((double)(e.Y) / Options.TileHeight) - 2;
                UpdateSpawnPreview();
            }
        }
    }
}
