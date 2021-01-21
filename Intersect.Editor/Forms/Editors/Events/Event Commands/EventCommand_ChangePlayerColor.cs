using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangePlayerColor : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangePlayerColorCommand mMyCommand;

        public EventCommandChangePlayerColor(ChangePlayerColorCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();

            nudRgbaR.Value = mMyCommand.Color.R;
            nudRgbaG.Value = mMyCommand.Color.G;
            nudRgbaB.Value = mMyCommand.Color.B;
            nudRgbaA.Value = mMyCommand.Color.A;
        }

        private void InitLocalization()
        {
            grpChangeColor.Text = Strings.EventChangePlayerColor.Title;
            btnSave.Text = Strings.EventChangePlayerColor.Okay;
            btnCancel.Text = Strings.EventChangePlayerColor.Cancel;

            lblRed.Text = Strings.EventChangePlayerColor.Red;
            lblGreen.Text = Strings.EventChangePlayerColor.Green;
            lblBlue.Text = Strings.EventChangePlayerColor.Blue;
            lblAlpha.Text = Strings.EventChangePlayerColor.Alpha;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Color = new Color((byte)nudRgbaA.Value, (byte)nudRgbaR.Value, (byte)nudRgbaG.Value, (byte)nudRgbaB.Value);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
