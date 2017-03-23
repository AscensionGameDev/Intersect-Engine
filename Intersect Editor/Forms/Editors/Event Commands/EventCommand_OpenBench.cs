using System;
using System.Windows.Forms;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_OpenBench : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_OpenBench(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbbench.Items.Clear();
            cmbbench.Items.AddRange(Database.GetGameObjectList(GameObject.Bench));
            cmbbench.SelectedIndex = Database.GameObjectListIndex(GameObject.Bench, _myCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpBench.Text = Strings.Get("eventopenbench", "title");
            lblBench.Text = Strings.Get("eventopenbench", "label");
            btnSave.Text = Strings.Get("eventopenbench", "okay");
            btnCancel.Text = Strings.Get("eventopenbench", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbbench.SelectedIndex > -1)
                _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Bench, cmbbench.SelectedIndex);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}