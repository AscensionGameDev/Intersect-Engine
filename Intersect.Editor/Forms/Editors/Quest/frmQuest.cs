using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.Forms.Editors.Events;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Logging;

namespace Intersect.Editor.Forms.Editors.Quest
{

    public partial class FrmQuest : EditorForm
    {

        private List<QuestBase> mChanged = new List<QuestBase>();

        private string mCopiedItem;

        private QuestBase mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmQuest()
        {
            ApplyHooks();
            InitializeComponent();
            lstQuests.LostFocus += itemList_FocusChanged;
            lstQuests.GotFocus += itemList_FocusChanged;
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.QuestEditor.title;
            toolStripItemNew.Text = Strings.QuestEditor.New;
            toolStripItemDelete.Text = Strings.QuestEditor.delete;
            toolStripItemCopy.Text = Strings.QuestEditor.copy;
            toolStripItemPaste.Text = Strings.QuestEditor.paste;
            toolStripItemUndo.Text = Strings.QuestEditor.undo;

            grpQuests.Text = Strings.QuestEditor.quests;
            grpGeneral.Text = Strings.QuestEditor.general;
            lblName.Text = Strings.QuestEditor.name;

            grpLogOptions.Text = Strings.QuestEditor.logoptions;
            chkLogAfterComplete.Text = Strings.QuestEditor.showafter;
            chkLogBeforeOffer.Text = Strings.QuestEditor.showbefore;

            grpProgessionOptions.Text = Strings.QuestEditor.options;
            chkRepeatable.Text = Strings.QuestEditor.repeatable;
            chkQuittable.Text = Strings.QuestEditor.quit;

            lblBeforeOffer.Text = Strings.QuestEditor.beforeofferdesc;
            lblOffer.Text = Strings.QuestEditor.offerdesc;
            lblInProgress.Text = Strings.QuestEditor.inprogressdesc;
            lblCompleted.Text = Strings.QuestEditor.completeddesc;

            grpQuestReqs.Text = Strings.QuestEditor.requirements;
            btnEditRequirements.Text = Strings.QuestEditor.editrequirements;

            grpQuestTasks.Text = Strings.QuestEditor.tasks;
            btnAddTask.Text = Strings.QuestEditor.addtask;
            btnRemoveTask.Text = Strings.QuestEditor.removetask;

            grpActions.Text = Strings.QuestEditor.actions;
            lblOnStart.Text = Strings.QuestEditor.onstart;
            btnEditStartEvent.Text = Strings.QuestEditor.editstartevent;
            lblOnEnd.Text = Strings.QuestEditor.onend;
            btnEditCompletionEvent.Text = Strings.QuestEditor.editendevent;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.QuestEditor.sortchronologically;
            txtSearch.Text = Strings.QuestEditor.searchplaceholder;
            lblFolder.Text = Strings.QuestEditor.folderlabel;

            btnSave.Text = Strings.QuestEditor.save;
            btnCancel.Text = Strings.QuestEditor.cancel;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Quest)
            {
                InitEditor();
                if (mEditorItem != null && !QuestBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
            {
                if (item == null)
                {
                    Log.Warn($"Unexpected null: {nameof(FrmQuest)}.{nameof(btnCancel_Click)}() {nameof(item)}");
                }
                else
                {
                    if (item.StartEvent == null)
                    {
                        Log.Warn($"Unexpected null: {nameof(FrmQuest)}.{nameof(btnCancel_Click)}() {nameof(item)}.{nameof(item.StartEvent)}");
                    }

                    if (item.EndEvent == null)
                    {
                        Log.Warn($"Unexpected null: {nameof(FrmQuest)}.{nameof(btnCancel_Click)}() {nameof(item)}.{nameof(item.EndEvent)}");
                    }
                }

                item?.StartEvent?.RestoreBackup();
                item?.StartEvent?.DeleteBackup();
                item?.EndEvent?.RestoreBackup();
                item?.EndEvent?.DeleteBackup();
                item?.RestoreBackup();
                item?.DeleteBackup();
            }

            mEditorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            mChanged?.ForEach(
                item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    foreach (var id in item.OriginalTaskEventIds.Keys)
                    {
                        var found = false;
                        for (var i = 0; i < item.Tasks.Count; i++)
                        {
                            if (item.Tasks[i].Id == id)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            item.RemoveEvents.Add(item.OriginalTaskEventIds[id]);
                        }
                    }

                    PacketSender.SendSaveObject(item);
                    PacketSender.SendSaveObject(item.StartEvent);
                    PacketSender.SendSaveObject(item.EndEvent);
                    item.Tasks?.ForEach(
                        tsk =>
                        {
                            if (tsk?.EditingEvent == null)
                            {
                                return;
                            }

                            if (tsk.EditingEvent.Id != Guid.Empty)
                            {
                                PacketSender.SendSaveObject(tsk.EditingEvent);
                            }

                            tsk.EditingEvent.DeleteBackup();
                        }
                    );

                    item.StartEvent?.DeleteBackup();
                    item.EndEvent?.DeleteBackup();
                    item.DeleteBackup();
                }
            );

            mEditorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                txtBeforeDesc.Text = mEditorItem.BeforeDescription;
                txtStartDesc.Text = mEditorItem.StartDescription;
                txtInProgressDesc.Text = mEditorItem.InProgressDescription;
                txtEndDesc.Text = mEditorItem.EndDescription;

                chkRepeatable.Checked = Convert.ToBoolean(mEditorItem.Repeatable);
                chkQuittable.Checked = Convert.ToBoolean(mEditorItem.Quitable);
                chkLogBeforeOffer.Checked = Convert.ToBoolean(mEditorItem.LogBeforeOffer);
                chkLogAfterComplete.Checked = Convert.ToBoolean(mEditorItem.LogAfterComplete);

                ListQuestTasks();

                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.StartEvent?.MakeBackup();
                    mEditorItem.EndEvent?.MakeBackup();
                    foreach (var tsk in mEditorItem.Tasks)
                    {
                        tsk.CompletionEvent?.MakeBackup();
                        tsk.EditingEvent = tsk.CompletionEvent;
                    }

                    mEditorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }

            UpdateToolStripItems();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            if (mEditorItem != null)
            {
                mEditorItem.Name = txtName?.Text ?? "";

                //Rename all events
                if (mEditorItem.StartEvent != null)
                {
                    mEditorItem.StartEvent.Name = Strings.QuestEditor.startevent.ToString(mEditorItem.Name);
                }

                if (mEditorItem.EndEvent != null)
                {
                    mEditorItem.EndEvent.Name = Strings.QuestEditor.endevent.ToString(mEditorItem.Name);
                }

                if (mEditorItem.Tasks != null)
                {
                    foreach (var tsk in mEditorItem.Tasks)
                    {
                        if (tsk.CompletionEvent != null)
                        {
                            tsk.CompletionEvent.Name = Strings.TaskEditor.completionevent.ToString(mEditorItem.Name);
                        }
                    }
                }

                if (lstQuests != null)
                {
                    if (lstQuests.SelectedNode != null && lstQuests.SelectedNode.Tag != null)
                    {
                        lstQuests.SelectedNode.Text = txtName?.Text ?? "";
                    }
                }
            }

            mChangingName = false;
        }

        private void txtStartDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.StartDescription = txtStartDesc.Text;
        }

        private void txtEndDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.EndDescription = txtEndDesc.Text;
        }

        private void btnEditStartEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.StartEvent.Name = Strings.QuestEditor.startevent.ToString(mEditorItem.Name);
            OpenQuestEvent(mEditorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.EndEvent.Name = Strings.QuestEditor.endevent.ToString(mEditorItem.Name);
            OpenQuestEvent(mEditorItem.EndEvent);
        }

        private void OpenQuestEvent(EventBase evt)
        {
            var editor = new FrmEvent(null) {MyEvent = evt};
            editor.InitEditor(true, true, true);
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var questTask = new QuestBase.QuestTask(Guid.NewGuid());
            questTask.EditingEvent = new EventBase(Guid.Empty, Guid.Empty, 0, 0, false);
            questTask.EditingEvent.Name = Strings.TaskEditor.completionevent.ToString(mEditorItem.Name);
            mEditorItem.AddEvents.Add(questTask.Id, questTask.EditingEvent);
            if (OpenTaskEditor(questTask))
            {
                mEditorItem.Tasks.Add(questTask);
                ListQuestTasks();
            }
        }

        private void ListQuestTasks()
        {
            lstTasks.Items.Clear();
            foreach (var task in mEditorItem.Tasks)
            {
                lstTasks.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
            }
        }

        private bool OpenTaskEditor(QuestBase.QuestTask task)
        {
            var cmdWindow = new QuestTaskEditor(mEditorItem, task);
            var frm = new Form
            {
                Text = Strings.TaskEditor.title
            };

            frm.Controls.Add(cmdWindow);
            frm.Size = new Size(0, 0);
            frm.AutoSize = true;
            frm.ControlBox = false;
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.BackColor = cmdWindow.BackColor;
            cmdWindow.BringToFront();
            frm.ShowDialog();
            if (!cmdWindow.Cancelled)
            {
                return true;
            }

            return false;
        }

        private void btnRemoveTask_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1)
            {
                if (mEditorItem.AddEvents.ContainsKey(mEditorItem.Tasks[lstTasks.SelectedIndex].Id))
                {
                    mEditorItem.AddEvents.Remove(mEditorItem.Tasks[lstTasks.SelectedIndex].Id);
                }

                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskUp_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > 0)
            {
                var item = mEditorItem.Tasks[lstTasks.SelectedIndex];
                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                mEditorItem.Tasks.Insert(lstTasks.SelectedIndex - 1, item);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskDown_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && lstTasks.SelectedIndex != lstTasks.Items.Count - 1)
            {
                var item = mEditorItem.Tasks[lstTasks.SelectedIndex];
                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                mEditorItem.Tasks.Insert(lstTasks.SelectedIndex + 1, item);
                ListQuestTasks();
            }
        }

        private void txtInProgressDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.InProgressDescription = txtInProgressDesc.Text;
        }

        private void chkRepeatable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Repeatable = chkRepeatable.Checked;
        }

        private void chkQuittable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Quitable = chkQuittable.Checked;
        }

        private void lstTasks_DoubleClick(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && mEditorItem.Tasks.Count > lstTasks.SelectedIndex)
            {
                if (OpenTaskEditor(mEditorItem.Tasks[lstTasks.SelectedIndex]))
                {
                    ListQuestTasks();
                }
            }
        }

        private void txtBeforeDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.BeforeDescription = txtBeforeDesc.Text;
        }

        private void chkLogBeforeOffer_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.LogBeforeOffer = chkLogBeforeOffer.Checked;
        }

        private void chkLogAfterComplete_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.LogAfterComplete = chkLogAfterComplete.Checked;
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Quest);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstQuests.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.QuestEditor.deleteprompt, Strings.QuestEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstQuests.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstQuests.Focused)
            {
                var startEventId = mEditorItem.StartEventId;
                var endEventId = mEditorItem.EndEventId;
                mEditorItem.Load(mCopiedItem, true);

                EventBase.Get(startEventId).Load(mEditorItem.StartEvent.JsonData);
                EventBase.Get(endEventId).Load(mEditorItem.EndEvent.JsonData);

                mEditorItem.StartEventId = startEventId;
                mEditorItem.EndEventId = endEventId;

                //Fix tasks
                foreach (var tsk in mEditorItem.Tasks)
                {
                    var oldId = tsk.Id;
                    tsk.Id = Guid.NewGuid();

                    if (mEditorItem.AddEvents.ContainsKey(oldId))
                    {
                        mEditorItem.AddEvents.Add(tsk.Id, mEditorItem.AddEvents[oldId]);
                        tsk.EditingEvent = mEditorItem.AddEvents[tsk.Id];
                        mEditorItem.AddEvents.Remove(oldId);
                    }
                    else
                    {
                        var tskEventData = EventBase.Get(tsk.CompletionEventId).JsonData;
                        tsk.CompletionEventId = Guid.Empty;
                        tsk.EditingEvent = new EventBase(Guid.Empty, Guid.Empty, 0, 0, false);
                        tsk.EditingEvent.Name = Strings.TaskEditor.completionevent.ToString(mEditorItem.Name);
                        tsk.EditingEvent.Load(tskEventData);
                        mEditorItem.AddEvents.Add(tsk.Id, tsk.EditingEvent);
                    }
                }

                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.QuestEditor.undoprompt, Strings.QuestEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstQuests.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstQuests.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstQuests.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstQuests.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.Requirements, RequirementType.Quest);
            frm.ShowDialog();
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstQuests.SelectedNode != null && lstQuests.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstQuests.SelectedNode.Tag;
            }

            lstQuests.Nodes.Clear();

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in QuestBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((QuestBase) itm.Value).Folder) &&
                    !mFolders.Contains(((QuestBase) itm.Value).Folder))
                {
                    mFolders.Add(((QuestBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((QuestBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((QuestBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstQuests.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstQuests.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in QuestBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = QuestBase.Get(itm.Key).Folder;
                if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked && !CustomSearch())
                {
                    var folderNode = folderNodes[folder];
                    folderNode.Nodes.Add(node);
                    if (itm.Key == selectedId)
                    {
                        folderNode.Expand();
                    }
                }
                else
                {
                    lstQuests.Nodes.Add(node);
                }

                if (CustomSearch())
                {
                    if (!node.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        node.Remove();
                    }
                }

                if (itm.Key == selectedId)
                {
                    lstQuests.SelectedNode = node;
                }
            }

            var selectedNode = lstQuests.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstQuests.Sort();
            }

            lstQuests.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.QuestEditor.folderprompt, Strings.QuestEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstQuests_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(Guid))
                    {
                        Clipboard.SetText(e.Node.Tag.ToString());
                    }
                }

                var hitTest = lstQuests.HitTest(e.Location);
                if (hitTest.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                        {
                            node.Collapse();
                        }
                        else
                        {
                            node.Expand();
                        }
                    }
                }

                if (node.IsExpanded)
                {
                    if (!mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        private void lstQuests_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstQuests.SelectedNode == null || lstQuests.SelectedNode.Tag == null)
            {
                return;
            }

            mEditorItem = QuestBase.Get((Guid) lstQuests.SelectedNode.Tag);
            UpdateEditor();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            InitEditor();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = Strings.QuestEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.QuestEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.QuestEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.QuestEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
