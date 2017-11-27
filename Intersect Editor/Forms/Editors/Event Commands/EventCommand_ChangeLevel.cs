using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChangeLevel : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandChangeLevel(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            if (mMyCommand.Ints[0] <= 0 || mMyCommand.Ints[0] > Options.MaxLevel) mMyCommand.Ints[0] = 1;
            nudLevel.Maximum = Options.MaxLevel;
            nudLevel.Value = mMyCommand.Ints[0];
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeLevel.Text = Strings.eventchangelevel.title;
            lblLevel.Text = Strings.eventchangelevel.label;
            btnSave.Text = Strings.eventchangelevel.okay;
            btnCancel.Text = Strings.eventchangelevel.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = (int) nudLevel.Value;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}