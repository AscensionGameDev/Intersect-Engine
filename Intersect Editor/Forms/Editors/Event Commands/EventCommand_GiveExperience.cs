using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandGiveExperience : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandGiveExperience(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            nudExperience.Value = mMyCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpGiveExperience.Text = Strings.eventgiveexperience.title;
            lblExperience.Text = Strings.eventgiveexperience.label;
            btnSave.Text = Strings.eventgiveexperience.okay;
            btnCancel.Text = Strings.eventgiveexperience.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = (int) nudExperience.Value;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}