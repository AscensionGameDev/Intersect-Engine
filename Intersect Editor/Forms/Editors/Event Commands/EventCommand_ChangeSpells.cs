using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChangeSpells : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private EventCommand mMyCommand;

        public EventCommandChangeSpells(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbAction.SelectedIndex = mMyCommand.Ints[0];
            cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell, mMyCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpChangeSpells.Text = Strings.eventchangespells.title;
            cmbAction.Items.Clear();
            for (int i = 0; i < Strings.eventchangespells.actions.Length; i++)
            {
                cmbAction.Items.Add(Strings.eventchangespells.actions[i]);
            }
            lblAction.Text = Strings.eventchangespells.action;
            lblSpell.Text = Strings.eventchangespells.spell;
            btnSave.Text = Strings.eventchangespells.okay;
            btnCancel.Text = Strings.eventchangespells.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = cmbAction.SelectedIndex;
            mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex);
            if (mMyCommand.Ints[4] == 0)
                // command.Ints[4, and 5] are reserved for when the action succeeds or fails
            {
                for (var i = 0; i < 2; i++)
                {
                    mCurrentPage.CommandLists.Add(new CommandList());
                    mMyCommand.Ints[4 + i] = mCurrentPage.CommandLists.Count - 1;
                }
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}