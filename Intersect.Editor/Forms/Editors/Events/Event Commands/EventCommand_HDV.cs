using System;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	public partial class EventCommand_HDV : UserControl
	{
		private readonly FrmEvent mEventEditor;
		private HDVCommand mMyCommand;

		public EventCommand_HDV(HDVCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			mMyCommand = refCommand;
			mEventEditor = editor;
			cmbHDV.Items.Clear();
			cmbHDV.Items.AddRange(HDVBase.Names);
			cmbHDV.SelectedIndex = HDVBase.ListIndex(mMyCommand.HDVid);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{

			mEventEditor.CancelCommandEdit();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (cmbHDV.SelectedIndex > -1)
			{
				mMyCommand.HDVid = HDVBase.IdFromList(cmbHDV.SelectedIndex);
			}
			mEventEditor.FinishCommandEdit();
		}
	}
}
