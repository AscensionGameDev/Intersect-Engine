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
            cmbbench.Items.Clear();
            cmbbench.Items.AddRange(Database.GetGameObjectList(GameObjectType.CraftTables));
            cmbbench.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CraftTables, mMyCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpBench.Text = Strings.EventOpenBench.title;
            lblBench.Text = Strings.EventOpenBench.label;
            btnSave.Text = Strings.EventOpenBench.okay;
            btnCancel.Text = Strings.EventOpenBench.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbbench.SelectedIndex > -1)
                mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.CraftTables, cmbbench.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}