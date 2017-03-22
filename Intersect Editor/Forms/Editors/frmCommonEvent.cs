
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmCommonEvent : Form
    {
        public frmCommonEvent()
        {
            InitializeComponent();
            InitLocalization();
            ListCommonEvents();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void InitLocalization()
        {
            Text = Strings.Get("commoneventeditor", "title");
            grpCommonEvents.Text = Strings.Get("commoneventeditor", "events");
            btnNew.Text = Strings.Get("commoneventeditor", "new");
            btnDelete.Text = Strings.Get("commoneventeditor", "delete");
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.CommonEvent)
            {
                ListCommonEvents();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.CommonEvent);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1 && EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent,lstCommonEvents.SelectedIndex)) != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent, lstCommonEvents.SelectedIndex)));
                }
            }
        }

        private void ListCommonEvents()
        {
            lstCommonEvents.Items.Clear();
            lstCommonEvents.Items.AddRange(Database.GetGameObjectList(GameObject.CommonEvent));
        }

        private void lstCommonEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedIndex > -1)
            {
                FrmEvent editor = new FrmEvent(null)
                {
                    MyEvent = EventBase.GetEvent(Database.GameObjectIdFromList(GameObject.CommonEvent, lstCommonEvents.SelectedIndex))
                };
                editor.InitEditor();
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
