using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<string> mKnownFolders = new List<string>();

        public FrmQuest()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = QuestBase.Get(id);
            UpdateEditor();
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


            chkDoNotShowUnlessReqsMet.Text = Strings.QuestEditor.donotshowunlessreqsmet;

            //Categories
            lblUnstartedCategory.Text = Strings.QuestEditor.unstartedcategory;
            lblInProgressCategory.Text = Strings.QuestEditor.inprogressgategory;
            lblCompletedCategory.Text = Strings.QuestEditor.completedcategory;
            cmbUnstartedCategory.Items.Add(Strings.General.none);
            cmbInProgressCategory.Items.Add(Strings.General.none);
            cmbCompletedCategory.Items.Add(Strings.General.none);

            foreach (var questCategory in Options.Instance.Quest.Categories)
            {
                cmbUnstartedCategory.Items.Add(questCategory);
                cmbInProgressCategory.Items.Add(questCategory);
                cmbCompletedCategory.Items.Add(questCategory);
            }

            lblSortOrder.Text = Strings.QuestEditor.order;

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

                chkDoNotShowUnlessReqsMet.Checked = Convert.ToBoolean(mEditorItem.DoNotShowUnlessRequirementsMet);

                cmbUnstartedCategory.SelectedIndex = cmbUnstartedCategory.Items.IndexOf(mEditorItem.UnstartedCategory ?? "");
                cmbInProgressCategory.SelectedIndex = cmbInProgressCategory.Items.IndexOf(mEditorItem.InProgressCategory ?? "");
                cmbCompletedCategory.SelectedIndex = cmbCompletedCategory.Items.IndexOf(mEditorItem.CompletedCategory ?? "");

                if (cmbUnstartedCategory.SelectedIndex == -1)
                {
                    cmbUnstartedCategory.SelectedIndex = 0;
                }

                if (cmbInProgressCategory.SelectedIndex == -1)
                {
                    cmbInProgressCategory.SelectedIndex = 0;
                }

                if (cmbCompletedCategory.SelectedIndex == -1)
                {
                    cmbCompletedCategory.SelectedIndex = 0;
                }

                nudOrderValue.Value = mEditorItem.OrderValue;

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

                lstGameObjects.UpdateText(txtName?.Text ?? "");
            }
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
            if (mEditorItem != null && lstGameObjects.Focused)
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
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused)
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

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
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

            var items = QuestBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((QuestBase)pair.Value)?.Name ?? Models.DatabaseObject<QuestBase>.Deleted, ((QuestBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
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
                    lstGameObjects.ExpandFolder(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
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

        private void chkDoNotShowUnlessReqsMet_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.DoNotShowUnlessRequirementsMet = chkDoNotShowUnlessReqsMet.Checked;
        }

        private void cmbUnstartedCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnstartedCategory.SelectedIndex > -1)
            {
                mEditorItem.UnstartedCategory = cmbUnstartedCategory.Text;
            }
        }

        private void cmbInProgressCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInProgressCategory.SelectedIndex > -1)
            {
                mEditorItem.InProgressCategory = cmbInProgressCategory.Text;
            }
        }

        private void cmbCompletedCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCompletedCategory.SelectedIndex > -1)
            {
                mEditorItem.CompletedCategory = cmbCompletedCategory.Text;
            }
        }

        private void nudOrderValue_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.OrderValue = (int)nudOrderValue.Value; 
        }
    }

}
