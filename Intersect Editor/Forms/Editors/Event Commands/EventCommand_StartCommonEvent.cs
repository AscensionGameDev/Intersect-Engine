using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandStartCommonEvent : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandStartCommonEvent(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbEvent.Items.Clear();
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.Event));
            cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Event, refCommand.Guids[0]);
        }

        private void InitLocalization()
        {
            grpCommonEvent.Text = Strings.EventStartCommonEvent.title;
            lblCommonEvent.Text = Strings.EventStartCommonEvent.label;
            btnSave.Text = Strings.EventStartCommonEvent.okay;
            btnCancel.Text = Strings.EventStartCommonEvent.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Guids[0] = Database.GameObjectIdFromList(GameObjectType.Event, cmbEvent.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}