using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeSpells : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventPage mCurrentPage;

        private ChangeSpellsCommand mMyCommand;

        public EventCommandChangeSpells(ChangeSpellsCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(SpellBase.Names);
            cmbAction.SelectedIndex = refCommand.Add ? 0 : 1;
            cmbSpell.SelectedIndex = SpellBase.ListIndex(mMyCommand.SpellId);
        }

        private void InitLocalization()
        {
            grpChangeSpells.Text = Strings.EventChangeSpells.title;
            cmbAction.Items.Clear();
            for (var i = 0; i < Strings.EventChangeSpells.actions.Count; i++)
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
            mMyCommand.Add = !Convert.ToBoolean(cmbAction.SelectedIndex);
            mMyCommand.SpellId = SpellBase.IdFromList(cmbSpell.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
