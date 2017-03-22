
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_OpenShop : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_OpenShop(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbShop.Items.Clear();
            cmbShop.Items.AddRange(Database.GetGameObjectList(GameObject.Shop));
            cmbShop.SelectedIndex = Database.GameObjectListIndex(GameObject.Shop, _myCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpShop.Text = Strings.Get("eventopenshop", "title");
            lblShop.Text = Strings.Get("eventopenshop", "label");
            btnSave.Text = Strings.Get("eventopenshop", "okay");
            btnCancel.Text = Strings.Get("eventopenshop", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbShop.SelectedIndex > -1)
                _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Shop, cmbShop.SelectedIndex);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
