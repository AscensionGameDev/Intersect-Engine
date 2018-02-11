using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            grpGiveExperience.Text = Strings.Get("eventgiveexperience", "title");
            lblExperience.Text = Strings.Get("eventgiveexperience", "label");
            btnSave.Text = Strings.Get("eventgiveexperience", "okay");
            btnCancel.Text = Strings.Get("eventgiveexperience", "cancel");
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