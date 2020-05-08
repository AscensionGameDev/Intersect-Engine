using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	public partial class EventCommand_DropChance : UserControl
	{
		private readonly FrmEvent mEventEditor;

		private DropChanceItemCommand mMyCommand;

		public EventCommand_DropChance(DropChanceItemCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			mMyCommand = refCommand;
			mEventEditor = editor;

			cmbItem.Items.Clear();
			cmbItem.Items.AddRange(ItemBase.Names);
			
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			mMyCommand.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
			mMyCommand.Min = (int)nudMin.Value;
			mMyCommand.Max = (int)nudMax.Value;
			mMyCommand.DropChance = (double)nudDropChance.Value;
			mEventEditor.FinishCommandEdit();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{

			mEventEditor.CancelCommandEdit();
		}
	}
}
