using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps.MapList;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	public partial class EventCommand_SetSpawn : UserControl
	{
		private readonly FrmEvent mEventEditor;

		private SetSpawnCommand mMyCommand;

		public EventCommand_SetSpawn(SetSpawnCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			mMyCommand = refCommand;
			mEventEditor = editor;
		
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

			scrlX.Maximum = Options.MapWidth - 1;
			scrlY.Maximum = Options.MapHeight - 1;
			scrlX.Value = mMyCommand.X;
			scrlY.Value = mMyCommand.Y;
			lblX.Text = Strings.Warping.x.ToString(scrlX.Value);
			lblY.Text = Strings.Warping.y.ToString(scrlY.Value);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			mEventEditor.CancelCommandEdit();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{

			mMyCommand.MapId = MapList.OrderedMaps[cmbMap.SelectedIndex].MapId;
			mMyCommand.X = (sbyte)scrlX.Value;
			mMyCommand.Y = (sbyte)scrlY.Value;
			mEventEditor.FinishCommandEdit();
		}

		private void btnVisual_Click(object sender, EventArgs e)
		{
			var frmWarpSelection = new FrmWarpSelection();
			frmWarpSelection.SelectTile(MapList.OrderedMaps[cmbMap.SelectedIndex].MapId, scrlX.Value, scrlY.Value);
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

				scrlX.Value = frmWarpSelection.GetX();
				scrlY.Value = frmWarpSelection.GetY();
				lblX.Text = Strings.Warping.x.ToString(scrlX.Value);
				lblY.Text = Strings.Warping.y.ToString(scrlY.Value);
			}
		}

		private void cmbMap_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void scrlX_ValueChanged(object sender, DarkUI.Controls.ScrollValueEventArgs e)
		{
			lblX.Text = Strings.Warping.x.ToString(scrlX.Value);
		}

		private void scrlY_ValueChanged(object sender, DarkUI.Controls.ScrollValueEventArgs e)
		{
			lblY.Text = Strings.Warping.y.ToString(scrlY.Value);
		}
	}
}
