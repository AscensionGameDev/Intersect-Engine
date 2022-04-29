using System;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Forms.Helpers;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandSpawnNpc : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private MapBase mCurrentMap;

        private EventBase mEditingEvent;

        private SpawnNpcCommand mMyCommand;

        private int mSpawnX;

        private int mSpawnY;

        public EventCommandSpawnNpc(
            FrmEvent eventEditor,
            MapBase currentMap,
            EventBase currentEvent,
            SpawnNpcCommand editingCommand
        )
        {
            InitializeComponent();
            mMyCommand = editingCommand;
            mEventEditor = eventEditor;
            mEditingEvent = currentEvent;
            mCurrentMap = currentMap;
            InitLocalization();
            cmbNpc.Items.Clear();
            cmbNpc.Items.AddRange(NpcBase.Names);
            cmbNpc.SelectedIndex = NpcBase.ListIndex(mMyCommand.NpcId);
            if (mMyCommand.MapId != Guid.Empty)
            {
                cmbConditionType.SelectedIndex = 0;
            }
            else
            {
                cmbConditionType.SelectedIndex = 1;
            }

            nudWarpX.Maximum = Options.MapWidth;
            nudWarpY.Maximum = Options.MapHeight;
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile spawn
                    //Fill in the map cmb
                    nudWarpX.Value = mMyCommand.X;
                    nudWarpY.Value = mMyCommand.Y;
                    cmbDirection.SelectedIndex = mMyCommand.Dir;

                    break;
                case 1: //On/Around Entity Spawn
                    mSpawnX = mMyCommand.X;
                    mSpawnY = mMyCommand.Y;
                    chkDirRelative.Checked = Convert.ToBoolean(mMyCommand.Dir);
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

            lblMap.Text = Strings.Warping.map.ToString("");
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y.ToString("");
            lblMap.Text = Strings.Warping.direction.ToString("");
            cmbDirection.Items.Clear();
            for (var i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }

            cmbDirection.SelectedIndex = 0;
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
                    for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                    {
                        cmbMap.Items.Add(MapList.OrderedMaps[i].Name);
                        if (MapList.OrderedMaps[i].MapId == mMyCommand.MapId)
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
                        foreach (var evt in mCurrentMap.LocalEvents)
                        {
                            cmbEntities.Items.Add(
                                evt.Key == mEditingEvent.Id ? Strings.EventSpawnNpc.This + " " : "" + evt.Value.Name
                            );

                            if (mMyCommand.EntityId == evt.Key)
                            {
                                cmbEntities.SelectedIndex = cmbEntities.Items.Count - 1;
                            }
                        }
                    }

                    UpdateSpawnPreview();

                    break;
            }
        }

        private void UpdateSpawnPreview()
        {
            pnlSpawnLoc.BackgroundImage = GridHelper.DrawGrid(pnlSpawnLoc.Width, pnlSpawnLoc.Height, 5, 5, new GridCell(2, 2, null, "E"), new GridCell(mSpawnX + 2, mSpawnY + 2, System.Drawing.Color.Red));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.NpcId = NpcBase.IdFromList(cmbNpc.SelectedIndex);
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Tile Spawn
                    mMyCommand.EntityId = Guid.Empty;
                    mMyCommand.MapId = MapList.OrderedMaps[cmbMap.SelectedIndex].MapId;
                    mMyCommand.X = (sbyte) nudWarpX.Value;
                    mMyCommand.Y = (sbyte) nudWarpY.Value;
                    mMyCommand.Dir = (byte) cmbDirection.SelectedIndex;

                    break;
                case 1: //On/Around Entity Spawn
                    mMyCommand.MapId = Guid.Empty;
                    if (cmbEntities.SelectedIndex == 0 || cmbEntities.SelectedIndex == -1)
                    {
                        mMyCommand.EntityId = Guid.Empty;
                    }
                    else
                    {
                        mMyCommand.EntityId = mCurrentMap.LocalEvents.Keys.ToList()[cmbEntities.SelectedIndex - 1];
                    }

                    mMyCommand.X = (sbyte) mSpawnX;
                    mMyCommand.Y = (sbyte) mSpawnY;
                    mMyCommand.Dir = (byte) Convert.ToInt32(chkDirRelative.Checked);

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
            var frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(
                MapList.OrderedMaps[cmbMap.SelectedIndex].MapId, (int) nudWarpX.Value, (int) nudWarpY.Value
            );

            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == frmWarpSelection.GetMap())
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
                mSpawnX = (int) Math.Floor((double) e.X / Options.TileWidth) - 2;
                mSpawnY = (int) Math.Floor((double) e.Y / Options.TileHeight) - 2;
                UpdateSpawnPreview();
            }
        }

    }

}
