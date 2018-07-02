using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

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
            cmbSpell.Items.AddRange(SpellBase.Names);
            cmbAction.SelectedIndex = mMyCommand.Ints[0];
            cmbSpell.SelectedIndex = SpellBase.ListIndex(mMyCommand.Guids[1]);
        }

        private void InitLocalization()
        {
            grpChangeSpells.Text = Strings.EventChangeSpells.title;
            cmbAction.Items.Clear();
            for (int i = 0; i < Strings.EventChangeSpells.actions.Length; i++)
            {
                cmbAction.Items.Add(Strings.EventChangeSpells.actions[i]);
            }
            lblAction.Text = Strings.EventChangeSpells.action;
            lblSpell.Text = Strings.EventChangeSpells.spell;
            btnSave.Text = Strings.EventChangeSpells.okay;
            btnCancel.Text = Strings.EventChangeSpells.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = cmbAction.SelectedIndex;
            mMyCommand.Guids[1] = SpellBase.IdFromList(cmbSpell.SelectedIndex);
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