using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandOpenCraftingTable : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandOpenCraftingTable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbTable.Items.Clear();
            cmbTable.Items.AddRange(Database.GetGameObjectList(GameObjectType.CraftTables));
            cmbTable.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CraftTables, mMyCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpTable.Text = Strings.EventOpenCrafting.title;
            lblTable.Text = Strings.EventOpenCrafting.label;
            btnSave.Text = Strings.EventOpenCrafting.okay;
            btnCancel.Text = Strings.EventOpenCrafting.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbTable.SelectedIndex > -1)
                mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.CraftTables, cmbTable.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}