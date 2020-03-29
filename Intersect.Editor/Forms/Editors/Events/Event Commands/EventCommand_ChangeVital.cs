using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeVital : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventCommand mMyCommand;

        public EventCommandChangeVital(RestoreHpCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            nudVital.Value = refCommand.Amount;
            InitLocalization();
        }

        public EventCommandChangeVital(RestoreMpCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            nudVital.Value = refCommand.Amount;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeVital.Text = Strings.EventChangeVital.title;
            btnSave.Text = Strings.EventChangeVital.okay;
            btnCancel.Text = Strings.EventChangeVital.cancel;

            if (mMyCommand is RestoreHpCommand)
            {
                lblVital.Text = Strings.EventChangeVital.labelhealth;
            }

            if (mMyCommand is RestoreMpCommand)
            {
                lblVital.Text = Strings.EventChangeVital.labelmana;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mMyCommand is RestoreHpCommand)
            {
                ((RestoreHpCommand) mMyCommand).Amount = (int) nudVital.Value;
            }

            if (mMyCommand is RestoreMpCommand)
            {
                ((RestoreMpCommand) mMyCommand).Amount = (int) nudVital.Value;
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
