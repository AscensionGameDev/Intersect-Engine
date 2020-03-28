using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandPlayAnimation : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private MapBase mCurrentMap;

        private EventBase mEditingEvent;

        private PlayAnimationCommand mMyCommand;

        private int mSpawnX;

        private int mSpawnY;

        public EventCommandPlayAnimation(
            FrmEvent eventEditor,
            MapBase currentMap,
            EventBase currentEvent,
            PlayAnimationCommand editingCommand
        )
        {
            InitializeComponent();
            mMyCommand = editingCommand;
            mEventEditor = eventEditor;
            mEditingEvent = currentEvent;
            mCurrentMap = currentMap;
            InitLocalization();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.AddRange(AnimationBase.Names);
            cmbAnimation.SelectedIndex = AnimationBase.ListIndex(mMyCommand.AnimationId);
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
                    switch (mMyCommand.Dir)
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
            grpPlayAnimation.Text = Strings.EventPlayAnimation.title;
            lblAnimation.Text = Strings.EventPlayAnimation.animation;
            lblSpawnType.Text = Strings.EventPlayAnimation.spawntype;
            cmbConditionType.Items.Clear();
            cmbConditionType.Items.Add(Strings.EventPlayAnimation.spawntype0);
            cmbConditionType.Items.Add(Strings.EventPlayAnimation.spawntype1);

            grpTileSpawn.Text = Strings.EventPlayAnimation.spawntype0;
            grpEntitySpawn.Text = Strings.EventPlayAnimation.spawntype1;

            lblMap.Text = Strings.Warping.map.ToString("");
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y.ToString("");
            lblDir.Text = Strings.Warping.direction.ToString("");
            cmbDirection.Items.Clear();
            for (var i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }

            cmbDirection.SelectedIndex = 0;

            lblEntity.Text = Strings.EventPlayAnimation.entity;
            lblRelativeLocation.Text = Strings.EventPlayAnimation.relativelocation;
            chkRelativeLocation.Text = Strings.EventPlayAnimation.spawnrelative;
            chkRotateDirection.Text = Strings.EventPlayAnimation.rotaterelative;

            btnSave.Text = Strings.EventPlayAnimation.okay;
            btnCancel.Text = Strings.EventPlayAnimation.cancel;
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
                        cmbMap.Items.Add($@"{MapList.OrderedMaps[i].Name} ({MapList.OrderedMaps[i].MapId})");
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
                    cmbEntities.Items.Add(Strings.EventPlayAnimation.player);
                    cmbEntities.SelectedIndex = 0;

                    if (!mEditingEvent.CommonEvent)
                    {
                        foreach (var evt in mCurrentMap.LocalEvents)
                        {
                            cmbEntities.Items.Add(
                                evt.Key == mEditingEvent.Id
                                    ? Strings.EventPlayAnimation.This + " "
                                    : "" + evt.Value.Name
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
            var destBitmap = new Bitmap(pnlSpawnLoc.Width, pnlSpawnLoc.Height);
            var renderFont = new Font(new FontFamily("Arial"), 14);
            var cellWidth = pnlSpawnLoc.Width / 5;
            var cellHeight = pnlSpawnLoc.Height / 5;

            var g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.White);
            g.FillRectangle(
                Brushes.Red,
                new Rectangle(
                    (pnlSpawnLoc.Width - cellWidth) / 2, (pnlSpawnLoc.Height - cellHeight) / 2, cellWidth, cellHeight
                )
            );

            for (var i = 1; i < 5; ++i)
            {
                g.DrawLine(Pens.Black, 0, cellHeight * i, pnlSpawnLoc.Width, cellHeight * i);
                g.DrawLine(Pens.Black, cellWidth * i, 0, cellWidth * i, pnlSpawnLoc.Height);
            }

            //            g.FillRectangle(Brushes.Red, new Rectangle((mSpawnX + 2) * 32, (mSpawnY + 2) * 32, 32, 32));
            //            for (int x = 0; x < 5; x++)
            //            {
            //                g.DrawLine(Pens.Black, x * 32, 0, x * 32, 32 * 5);
            //                g.DrawLine(Pens.Black, 0, x * 32, 32 * 5, x * 32);
            //            }
            //            g.DrawLine(Pens.Black, 0, 32 * 5 - 1, 32 * 5, 32 * 5 - 1);
            //            g.DrawLine(Pens.Black, 32 * 5 - 1, 0, 32 * 5 - 1, 32 * 5 - 1);

            g.DrawString(
                "E", renderFont, Brushes.Black, pnlSpawnLoc.Width / 2 - g.MeasureString("E", renderFont).Width / 2,
                pnlSpawnLoc.Height / 2 - g.MeasureString("S", renderFont).Height / 2
            );

            g.Dispose();
            pnlSpawnLoc.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.AnimationId = AnimationBase.IdFromList(cmbAnimation.SelectedIndex);
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
                    if (chkRelativeLocation.Checked && chkRotateDirection.Checked)
                    {
                        mMyCommand.Dir = 3;

                        //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRelativeLocation.Checked)
                    {
                        mMyCommand.Dir = 1;

                        //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else if (chkRotateDirection.Checked)
                    {
                        mMyCommand.Dir = 2;

                        //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }
                    else
                    {
                        mMyCommand.Dir = 0;

                        //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                    }

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
