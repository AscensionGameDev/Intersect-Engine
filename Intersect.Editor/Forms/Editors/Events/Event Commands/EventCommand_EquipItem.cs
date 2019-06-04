using System;
using System.Windows.Forms;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    public partial class EventCommandEquipItems : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EquipItemCommand mMyCommand;

        public EventCommandEquipItems(EquipItemCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
 
            InitLocalization();
            cmbItem.Items.Clear();
            cmbItem.Items.AddRange(ItemBase.Names);
            cmbItem.SelectedIndex = ItemBase.ListIndex(mMyCommand.ItemId);
        }

        private void InitLocalization()
        {
            grpChangeItems.Text = Strings.EventChangeItems.title;
            btnSave.Text = Strings.EventChangeItems.okay;
            btnCancel.Text = Strings.EventChangeItems.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}