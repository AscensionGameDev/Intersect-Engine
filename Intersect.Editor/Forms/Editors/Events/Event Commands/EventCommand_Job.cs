using System;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	public partial class EventCommand_Job : UserControl
	{
		private readonly FrmEvent mEventEditor;
		private EventCommand mMyCommand;

		public EventCommand_Job(SetJobCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			grpChangeValue.Hide();
			grpSelectJob.Show();
			mMyCommand = refCommand;
			mEventEditor = editor;
			cmbJob.Items.Clear();
			cmbJob.Items.AddRange(JobInfo.JobName);
			if (cmbJob.Items.Count > 0)
			{
				cmbJob.SelectedIndex = 0;
			}
			if (refCommand.JobIdentity >= 0 && refCommand.JobIdentity < cmbJob.Items.Count)
			{
				cmbJob.SelectedIndex = refCommand.JobIdentity;
			}
		}
		public EventCommand_Job(SetJobLevelCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			grpChangeValue.Show();
			grpSelectJob.Hide();
			mMyCommand = refCommand;
			mEventEditor = editor;
			nudValue.Value = refCommand.JobLevel;
		}
		public EventCommand_Job(AddJobLevelCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			grpChangeValue.Show();
			grpSelectJob.Hide();
			mMyCommand = refCommand;
			mEventEditor = editor;
			nudValue.Value = refCommand.JobAddLevel;
		}
		public EventCommand_Job(SetJobExpCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			grpChangeValue.Show();
			grpSelectJob.Hide();
			mMyCommand = refCommand;
			mEventEditor = editor;
			nudValue.Value = refCommand.JobExp;
		}
		public EventCommand_Job(AddJobExpCommand refCommand, FrmEvent editor)
		{
			InitializeComponent();
			grpChangeValue.Show();
			grpSelectJob.Hide();
			mMyCommand = refCommand;
			mEventEditor = editor;
			nudValue.Value = refCommand.JobAddExp;

		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (mMyCommand is SetJobCommand)
			{
				((SetJobCommand)mMyCommand).JobIdentity = cmbJob.SelectedIndex;
			}
			else if (mMyCommand is SetJobLevelCommand)
			{
				((SetJobLevelCommand)mMyCommand).JobLevel = (int)nudValue.Value;
			}
			else if (mMyCommand is AddJobLevelCommand)
			{
				((AddJobLevelCommand)mMyCommand).JobAddLevel = (int)nudValue.Value;
			}
			else if (mMyCommand is SetJobExpCommand)
			{
				((SetJobExpCommand)mMyCommand).JobExp = (int)nudValue.Value;
			}
			else if (mMyCommand is AddJobExpCommand)
			{
				((AddJobExpCommand)mMyCommand).JobAddExp = (int)nudValue.Value;
			}
			mEventEditor.FinishCommandEdit();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{

			mEventEditor.CancelCommandEdit();
		}
	}
}
