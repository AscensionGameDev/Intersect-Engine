using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandOpenShop : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private OpenShopCommand mMyCommand;

        public EventCommandOpenShop(OpenShopCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbShop.Items.Clear();
            cmbShop.Items.AddRange(ShopBase.Names);
            cmbShop.SelectedIndex = ShopBase.ListIndex(mMyCommand.ShopId);
        }

        private void InitLocalization()
        {
            grpShop.Text = Strings.EventOpenShop.title;
            lblShop.Text = Strings.EventOpenShop.label;
            btnSave.Text = Strings.EventOpenShop.okay;
            btnCancel.Text = Strings.EventOpenShop.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbShop.SelectedIndex > -1)
            {
                mMyCommand.ShopId = ShopBase.IdFromList(cmbShop.SelectedIndex);
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
