using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Intersect_Editor.Classes;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_PlayAnimation : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private MapBase _currentMap;
        private EventBase _editingEvent;
        private EventCommand _myCommand;
        private int spawnX = 0;
        private int spawnY = 0;

        public EventCommand_PlayAnimation(FrmEvent eventEditor, MapBase currentMap, EventBase currentEvent,
            EventCommand editingCommand)
        {
            InitializeComponent();
            _myCommand = editingCommand;
            _eventEditor = eventEditor;
            _editingEvent = currentEvent;
            _currentMap = currentMap;
            InitLocalization();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Animation, _myCommand.Ints[0]);
            cmbConditionType.SelectedIndex = _myCommand.Ints[1];
            nudWarpX.Maximum = Options.MapWidth;
            nudWarpY.Maximum = Options.MapHeight;
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile spawn
                    //Fill in the map cmb
                    nudWarpX.Value = _myCommand.Ints[3];
                    nudWarpY.Value = _myCommand.Ints[4];
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

        private void InitLocalization()
        {
            grpPlayAnimation.Text = Strings.Get("eventplayanimation", "title");
            lblAnimation.Text = Strings.Get("eventplayanimation", "animation");
            lblSpawnType.Text = Strings.Get("eventplayanimation", "spawntype");
            cmbConditionType.Items.Clear();
            cmbConditionType.Items.Add(Strings.Get("eventplayanimation", "spawntype0"));
            cmbConditionType.Items.Add(Strings.Get("eventplayanimation", "spawntype1"));

            grpTileSpawn.Text = Strings.Get("eventplayanimation", "spawntype0");
            grpEntitySpawn.Text = Strings.Get("eventplayanimation", "spawntype1");

            lblMap.Text = Strings.Get("warping", "map", "");
            lblX.Text = Strings.Get("warping", "x", "");
            lblY.Text = Strings.Get("warping", "y", "");
            lblDir.Text = Strings.Get("warping", "direction", "");
            cmbDirection.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Get("directions", i.ToString()));
            }

            lblEntity.Text = Strings.Get("eventplayanimation", "entity");
            lblRelativeLocation.Text = Strings.Get("eventplayanimation", "relativelocation");
            chkRelativeLocation.Text = Strings.Get("eventplayanimation", "spawnrelative");
            chkRotateDirection.Text = Strings.Get("eventplayanimation", "rotaterelative");

            btnSave.Text = Strings.Get("eventplayanimation", "okay");
            btnCancel.Text = Strings.Get("eventplayanimation", "cancel");
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
                    cmbEntities.Items.Add(Strings.Get("eventplayanimation", "player"));
                    cmbEntities.SelectedIndex = 0;

                    if (!_editingEvent.CommonEvent)
                    {
                        foreach (var evt in _currentMap.Events)
                        {
                            cmbEntities.Items.Add(evt.Key == _editingEvent.Id
                                ? Strings.Get("eventplayanimation", "this") + " "
                                : "" + evt.Value.Name);
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
            Graphics g = Graphics.FromImage(destBitmap);
            g.Clear(Color.White);
            g.FillRectangle(Brushes.Red, new Rectangle((spawnX + 2) * 32, (spawnY + 2) * 32, 32, 32));
            for (int x = 0; x < 5; x++)
            {
                g.DrawLine(Pens.Black, x * 32, 0, x * 32, 32 * 5);
                g.DrawLine(Pens.Black, 0, x * 32, 32 * 5, x * 32);
            }
            g.DrawLine(Pens.Black, 0, 32 * 5 - 1, 32 * 5, 32 * 5 - 1);
            g.DrawLine(Pens.Black, 32 * 5 - 1, 0, 32 * 5 - 1, 32 * 5 - 1);
            g.DrawString("E", renderFont, Brushes.Black,
                pnlSpawnLoc.Width / 2 - g.MeasureString("E", renderFont).Width / 2,
                pnlSpawnLoc.Height / 2 - g.MeasureString("S", renderFont).Height / 2);
            g.Dispose();
            pnlSpawnLoc.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex);
            _myCommand.Ints[1] = cmbConditionType.SelectedIndex;
            switch (_myCommand.Ints[1])
            {
                case 0: //Tile Spawn
                    _myCommand.Ints[2] = MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum;
                    _myCommand.Ints[3] = (int) nudWarpX.Value;
                    _myCommand.Ints[4] = (int) nudWarpY.Value;
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
                        _myCommand.Ints[5] = 3;
                            //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRelativeLocation.Checked)
                    {
                        _myCommand.Ints[5] = 1;
                            //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRotateDirection.Checked)
                    {
                        _myCommand.Ints[5] = 2;
                            //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else
                    {
                        _myCommand.Ints[5] = 0;
                            //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
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

        private void btnVisual_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum, (int) nudWarpX.Value,
                (int) nudWarpY.Value);
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
                nudWarpX.Value = frmWarpSelection.GetX();
                nudWarpY.Value = frmWarpSelection.GetY();
            }
        }

        private void pnlSpawnLoc_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= 0 && e.Y >= 0 && e.X < pnlSpawnLoc.Width && e.Y < pnlSpawnLoc.Height)
            {
                spawnX = (int) Math.Floor((double) (e.X) / Options.TileWidth) - 2;
                spawnY = (int) Math.Floor((double) (e.Y) / Options.TileHeight) - 2;
                UpdateSpawnPreview();
            }
        }
    }
}