using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandSpawnNpc : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private MapBase mCurrentMap;
        private EventBase mEditingEvent;
        private EventCommand mMyCommand;
        private int mSpawnX;
        private int mSpawnY;

        public EventCommandSpawnNpc(FrmEvent eventEditor, MapBase currentMap, EventBase currentEvent,
            EventCommand editingCommand)
        {
            InitializeComponent();
            mMyCommand = editingCommand;
            mEventEditor = eventEditor;
            mEditingEvent = currentEvent;
            mCurrentMap = currentMap;
            InitLocalization();
            cmbNpc.Items.Clear();
            cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
            cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Npc, mMyCommand.Ints[0]);
            cmbConditionType.SelectedIndex = mMyCommand.Ints[1];
            nudWarpX.Maximum = Options.MapWidth;
            nudWarpY.Maximum = Options.MapHeight;
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile spawn
                    //Fill in the map cmb
                    nudWarpX.Value = mMyCommand.Ints[3];
                    nudWarpY.Value = mMyCommand.Ints[4];
                    cmbDirection.SelectedIndex = mMyCommand.Ints[5];
                    break;
                case 1: //On/Around Entity Spawn
                    mSpawnX = mMyCommand.Ints[3];
                    mSpawnY = mMyCommand.Ints[4];
                    chkDirRelative.Checked = Convert.ToBoolean(mMyCommand.Ints[5]);
                    UpdateSpawnPreview();
                    break;
            }
        }

        private void InitLocalization()
        {
            grpSpawnNpc.Text = Strings.EventSpawnNpc.title;
            lblNpc.Text = Strings.EventSpawnNpc.npc;
            lblSpawnType.Text = Strings.EventSpawnNpc.spawntype;
            cmbConditionType.Items.Clear();
            cmbConditionType.Items.Add(Strings.EventSpawnNpc.spawntype0);
            cmbConditionType.Items.Add(Strings.EventSpawnNpc.spawntype1);

            grpTileSpawn.Text = Strings.EventSpawnNpc.spawntype0;
            grpEntitySpawn.Text = Strings.EventSpawnNpc.spawntype1;

            lblMap.Text = Strings.Warping.map.ToString( "");
            lblX.Text = Strings.Warping.x.ToString( "");
            lblY.Text = Strings.Warping.y.ToString( "");
            lblMap.Text = Strings.Warping.direction.ToString( "");
            cmbDirection.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }
            btnVisual.Text = Strings.Warping.visual;

            lblEntity.Text = Strings.EventSpawnNpc.entity;
            lblRelativeLocation.Text = Strings.EventSpawnNpc.relativelocation;
            chkDirRelative.Text = Strings.EventSpawnNpc.spawnrelative;

            btnSave.Text = Strings.EventSpawnNpc.okay;
            btnCancel.Text = Strings.EventSpawnNpc.cancel;
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
                        cmbMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
                        if (MapList.GetOrderedMaps()[i].MapNum == mMyCommand.Ints[2])
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
                    cmbEntities.Items.Add(Strings.EventSpawnNpc.player);
                    cmbEntities.SelectedIndex = 0;

                    if (!mEditingEvent.CommonEvent)
                    {
                        foreach (var evt in mCurrentMap.Events)
                        {
                            cmbEntities.Items.Add(evt.Key == mEditingEvent.Index
                                ? Strings.EventSpawnNpc.This + " "
                                : "" + evt.Value.Name);
                            if (mMyCommand.Ints[2] == evt.Key) cmbEntities.SelectedIndex = cmbEntities.Items.Count - 1;
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
            g.Clear(System.Drawing.Color.White);
            g.FillRectangle(Brushes.Red, new Rectangle((mSpawnX + 2) * 32, (mSpawnY + 2) * 32, 32, 32));
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
            mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);
            mMyCommand.Ints[1] = cmbConditionType.SelectedIndex;
            switch (mMyCommand.Ints[1])
            {
                case 0: //Tile Spawn
                    mMyCommand.Ints[2] = MapList.GetOrderedMaps()[cmbMap.SelectedIndex].MapNum;
                    mMyCommand.Ints[3] = (int) nudWarpX.Value;
                    mMyCommand.Ints[4] = (int) nudWarpY.Value;
                    mMyCommand.Ints[5] = cmbDirection.SelectedIndex;
                    break;
                case 1: //On/Around Entity Spawn
                    if (cmbEntities.SelectedIndex == 0 || cmbEntities.SelectedIndex == -1)
                    {
                        mMyCommand.Ints[2] = -1;
                    }
                    else
                    {
                        mMyCommand.Ints[2] = mCurrentMap.Events.Keys.ToList()[cmbEntities.SelectedIndex - 1];
                    }
                    mMyCommand.Ints[3] = mSpawnX;
                    mMyCommand.Ints[4] = mSpawnY;
                    mMyCommand.Ints[5] = Convert.ToInt32(chkDirRelative.Checked);
                    break;
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void btnVisual_Click(object sender, EventArgs e)
        {
            FrmWarpSelection frmWarpSelection = new FrmWarpSelection();
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
                mSpawnX = (int) Math.Floor((double) (e.X) / Options.TileWidth) - 2;
                mSpawnY = (int) Math.Floor((double) (e.Y) / Options.TileHeight) - 2;
                UpdateSpawnPreview();
            }
        }
    }
}