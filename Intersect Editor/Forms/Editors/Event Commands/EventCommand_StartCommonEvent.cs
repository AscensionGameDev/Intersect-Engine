using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));
            cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CommonEvent, refCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpCommonEvent.Text = Strings.Get("eventstartcommonevent", "title");
            lblCommonEvent.Text = Strings.Get("eventstartcommonevent", "label");
            btnSave.Text = Strings.Get("eventstartcommonevent", "okay");
            btnCancel.Text = Strings.Get("eventstartcommonevent", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.CommonEvent, cmbEvent.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}