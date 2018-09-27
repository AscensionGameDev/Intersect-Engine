using System;
using System.Windows.Forms;
using Intersect.Editor.Forms.Editors.Events;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmCommonEvent : EditorForm
    {
        public FrmCommonEvent()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();
            ListCommonEvents();
        }

        private void InitLocalization()
        {
            Text = Strings.CommoneEventEditor.title;
            grpCommonEvents.Text = Strings.CommoneEventEditor.events;
            btnNew.Text = Strings.CommoneEventEditor.New;
            btnDelete.Text = Strings.CommoneEventEditor.delete;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Event)
            {
                ListCommonEvents();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Event);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1 && EventBase.FromList(lstCommonEvents.SelectedIndex) != null)
            {
                if (MessageBox.Show(Strings.CommoneEventEditor.deleteprompt, Strings.CommoneEventEditor.delete, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(EventBase.FromList(lstCommonEvents.SelectedIndex));
                }
            }
        }

        private void ListCommonEvents()
        {
            lstCommonEvents.Items.Clear();
            lstCommonEvents.Items.AddRange(EventBase.Names);
        }

        private void lstCommonEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1)
            {
                FrmEvent editor = new FrmEvent(null)
                {
                    MyEvent = EventBase.FromList(lstCommonEvents.SelectedIndex)
                };
                editor.InitEditor(false,false);
                editor.ShowDialog();
                ListCommonEvents();
                Globals.MainForm.BringToFront();
                BringToFront();
            }
        }

        private void frmCommonEvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void lstCommonEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}