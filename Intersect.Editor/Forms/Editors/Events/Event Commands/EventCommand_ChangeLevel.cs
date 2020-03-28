using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeLevel : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeLevelCommand mMyCommand;

        public EventCommandChangeLevel(ChangeLevelCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            if (mMyCommand.Level <= 0 || mMyCommand.Level > Options.MaxLevel)
            {
                mMyCommand.Level = 1;
            }

            nudLevel.Maximum = Options.MaxLevel;
            nudLevel.Value = mMyCommand.Level;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeLevel.Text = Strings.EventChangeLevel.title;
            lblLevel.Text = Strings.EventChangeLevel.label;
            btnSave.Text = Strings.EventChangeLevel.okay;
            btnCancel.Text = Strings.EventChangeLevel.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Level = (int) nudLevel.Value;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
