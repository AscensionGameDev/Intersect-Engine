using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game
{

    public class QuestsWindow
    {

        private Button mBackButton;

        private ScrollControl mQuestDescArea;

        private RichLabel mQuestDescLabel;

        private Label mQuestDescTemplateLabel;

        private ListBox mQuestList;

        private Label mQuestStatus;

        //Controls
        private WindowControl mQuestsWindow;

        private Label mQuestTitle;

        private Button mQuitButton;

        private QuestBase mSelectedQuest;

        //Init
        public QuestsWindow(Canvas gameCanvas)
        {
            mQuestsWindow = new WindowControl(gameCanvas, Strings.QuestLog.title, false, "QuestsWindow");
            mQuestsWindow.DisableResizing();

            mQuestList = new ListBox(mQuestsWindow, "QuestList");
            mQuestList.EnableScroll(false, true);

            mQuestTitle = new Label(mQuestsWindow, "QuestTitle");
            mQuestTitle.SetText("");

            mQuestStatus = new Label(mQuestsWindow, "QuestStatus");
            mQuestStatus.SetText("");

            mQuestDescArea = new ScrollControl(mQuestsWindow, "QuestDescription");

            mQuestDescTemplateLabel = new Label(mQuestsWindow, "QuestDescriptionTemplate");

            mQuestDescLabel = new RichLabel(mQuestDescArea);

            mBackButton = new Button(mQuestsWindow, "BackButton");
            mBackButton.Text = Strings.QuestLog.back;
            mBackButton.Clicked += _backButton_Clicked;

            mQuitButton = new Button(mQuestsWindow, "AbandonQuestButton");
            mQuitButton.SetText(Strings.QuestLog.abandon);
            mQuitButton.Clicked += _quitButton_Clicked;

            mQuestsWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        private void _quitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mSelectedQuest != null)
            {
                new InputBox(
                    Strings.QuestLog.abandontitle.ToString(mSelectedQuest.Name),
                    Strings.QuestLog.abandonprompt.ToString(mSelectedQuest.Name), true, InputBox.InputType.YesNo,
                    AbandonQuest, null, mSelectedQuest.Id
                );
            }
        }

        void AbandonQuest(object sender, EventArgs e)
        {
            PacketSender.SendAbandonQuest((Guid) ((InputBox) sender).UserData);
        }

        private void _backButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mSelectedQuest = null;
            UpdateSelectedQuest();
        }

        //Methods
        public void Update(bool shouldUpdateList)
        {
            if (shouldUpdateList)
            {
                UpdateQuestList();
                UpdateSelectedQuest();
            }

            if (mQuestsWindow.IsHidden)
            {
                return;
            }

            if (mSelectedQuest != null)
            {
                if (Globals.Me.QuestProgress.ContainsKey(mSelectedQuest.Id))
                {
                    if (Globals.Me.QuestProgress[mSelectedQuest.Id].Completed &&
                        Globals.Me.QuestProgress[mSelectedQuest.Id].TaskId == Guid.Empty)
                    {
                        //Completed
                        if (!mSelectedQuest.LogAfterComplete)
                        {
                            mSelectedQuest = null;
                            UpdateSelectedQuest();
                        }

                        return;
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress[mSelectedQuest.Id].TaskId == Guid.Empty)
                        {
                            //Not Started
                            if (!mSelectedQuest.LogBeforeOffer)
                            {
                                mSelectedQuest = null;
                                UpdateSelectedQuest();
                            }
                        }

                        return;
                    }
                }

                if (!mSelectedQuest.LogBeforeOffer)
                {
                    mSelectedQuest = null;
                    UpdateSelectedQuest();
                }
            }
        }

        private void UpdateQuestList()
        {
            mQuestList.RemoveAllRows();
            if (Globals.Me != null)
            {
                var quests = QuestBase.Lookup.Values;

                var dict = new Dictionary<string, List<Tuple<QuestBase, int, Color>>>();

                foreach (QuestBase quest in quests)
                {
                    if (quest != null)
                    {
                        AddQuestToDict(dict, quest);
                    }
                }


                foreach (var category in Options.Instance.Quest.Categories)
                {
                    if (dict.ContainsKey(category))
                    {
                        AddCategoryToList(category, Color.White);
                        var sortedList = dict[category].OrderBy(l => l.Item2).ThenBy(l => l.Item1.OrderValue).ToList();
                        foreach (var qst in sortedList)
                        {
                            AddQuestToList(qst.Item1.Name, qst.Item3, qst.Item1.Id, true);
                        }
                    }
                }

                if (dict.ContainsKey(""))
                {
                    var sortedList = dict[""].OrderBy(l => l.Item2).ThenBy(l => l.Item1.OrderValue).ToList();
                    foreach (var qst in sortedList)
                    {
                        AddQuestToList(qst.Item1.Name, qst.Item3, qst.Item1.Id, false);
                    }
                }

            }
        }

        private void AddQuestToDict(Dictionary<string, List<Tuple<QuestBase, int, Color>>> dict, QuestBase quest)
        {
            var category = "";
            var add = false;
            var color = Color.White;
            var orderVal = -1;
            if (Globals.Me.QuestProgress.ContainsKey(quest.Id))
            {
                if (Globals.Me.QuestProgress[quest.Id].TaskId != Guid.Empty)
                {
                    add = true;
                    category = !TextUtils.IsNone(quest.InProgressCategory) ? quest.InProgressCategory : "";
                    color = Color.Yellow;
                    orderVal = 1;
                }
                else
                {
                    if (Globals.Me.QuestProgress[quest.Id].Completed)
                    {
                        if (quest.LogAfterComplete)
                        {
                            add = true;
                            category = !TextUtils.IsNone(quest.CompletedCategory) ? quest.CompletedCategory : "";
                            color = Color.Green;
                            orderVal = 3;
                        }
                    }
                    else
                    {
                        if (quest.LogBeforeOffer && !Globals.Me.HiddenQuests.Contains(quest.Id))
                        {
                            add = true;
                            category = !TextUtils.IsNone(quest.UnstartedCategory) ? quest.UnstartedCategory : "";
                            color = Color.Red;
                            orderVal = 2;
                        }
                    }
                }
            }
            else
            {
                if (quest.LogBeforeOffer && !Globals.Me.HiddenQuests.Contains(quest.Id))
                {
                    add = true;
                    category = !TextUtils.IsNone(quest.UnstartedCategory) ? quest.UnstartedCategory : "";
                    color = Color.Red;
                    orderVal = 2;
                }
            }

            if (add)
            {
                if (!dict.ContainsKey(category))
                {
                    dict.Add(category, new List<Tuple<QuestBase, int, Color>>());
                }

                dict[category].Add(new Tuple<QuestBase, int, Color>(quest, orderVal, color));
            }
        }

        private void AddQuestToList(string name, Color clr, Guid questId, bool indented = true)
        {
            var item = mQuestList.AddRow((indented ? "\t\t\t" : "") + name);
            item.UserData = questId;
            item.Clicked += QuestListItem_Clicked;
            item.Selected += Item_Selected;
            item.SetTextColor(clr);
            item.RenderColor = new Color(50, 255, 255, 255);
        }

        private void AddCategoryToList(string name, Color clr)
        {
            var item = mQuestList.AddRow(name);
            item.MouseInputEnabled = false;
            item.SetTextColor(clr);
            item.RenderColor = new Color(0, 255, 255, 255);
        }

        private void Item_Selected(Base sender, ItemSelectedEventArgs arguments)
        {
            mQuestList.UnselectAll();
        }

        private void QuestListItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var questNum = (Guid) ((ListBoxRow) sender).UserData;
            var quest = QuestBase.Get(questNum);
            if (quest != null)
            {
                mSelectedQuest = quest;
                UpdateSelectedQuest();
            }

            mQuestList.UnselectAll();
        }

        private void UpdateSelectedQuest()
        {
            if (mSelectedQuest == null)
            {
                mQuestList.Show();
                mQuestTitle.Hide();
                mQuestDescArea.Hide();
                mQuestStatus.Hide();
                mBackButton.Hide();
                mQuitButton.Hide();
            }
            else
            {
                mQuestDescLabel.ClearText();
                mQuitButton.IsDisabled = true;
                ListBoxRow rw;
                string[] myText = null;
                var taskString = new List<string>();
                if (Globals.Me.QuestProgress.ContainsKey(mSelectedQuest.Id))
                {
                    if (Globals.Me.QuestProgress[mSelectedQuest.Id].TaskId != Guid.Empty)
                    {
                        //In Progress
                        mQuestStatus.SetText(Strings.QuestLog.inprogress);
                        mQuestStatus.SetTextColor(Color.Yellow, Label.ControlState.Normal);
                        if (mSelectedQuest.InProgressDescription.Length > 0)
                        {
                            mQuestDescLabel.AddText(
                                mSelectedQuest.InProgressDescription, Color.White, Alignments.Left,
                                mQuestDescTemplateLabel.Font
                            );

                            mQuestDescLabel.AddLineBreak();
                            mQuestDescLabel.AddLineBreak();
                        }

                        mQuestDescLabel.AddText(
                            Strings.QuestLog.currenttask, Color.White, Alignments.Left, mQuestDescTemplateLabel.Font
                        );

                        mQuestDescLabel.AddLineBreak();
                        for (var i = 0; i < mSelectedQuest.Tasks.Count; i++)
                        {
                            if (mSelectedQuest.Tasks[i].Id == Globals.Me.QuestProgress[mSelectedQuest.Id].TaskId)
                            {
                                if (mSelectedQuest.Tasks[i].Description.Length > 0)
                                {
                                    mQuestDescLabel.AddText(
                                        mSelectedQuest.Tasks[i].Description, Color.White, Alignments.Left,
                                        mQuestDescTemplateLabel.Font
                                    );

                                    mQuestDescLabel.AddLineBreak();
                                    mQuestDescLabel.AddLineBreak();
                                }

                                if (mSelectedQuest.Tasks[i].Objective == QuestObjective.GatherItems) //Gather Items
                                {
                                    mQuestDescLabel.AddText(
                                        Strings.QuestLog.taskitem.ToString(
                                            Globals.Me.QuestProgress[mSelectedQuest.Id].TaskProgress,
                                            mSelectedQuest.Tasks[i].Quantity,
                                            ItemBase.GetName(mSelectedQuest.Tasks[i].TargetId)
                                        ), Color.White, Alignments.Left, mQuestDescTemplateLabel.Font
                                    );
                                }
                                else if (mSelectedQuest.Tasks[i].Objective == QuestObjective.KillNpcs) //Kill Npcs
                                {
                                    mQuestDescLabel.AddText(
                                        Strings.QuestLog.tasknpc.ToString(
                                            Globals.Me.QuestProgress[mSelectedQuest.Id].TaskProgress,
                                            mSelectedQuest.Tasks[i].Quantity,
                                            NpcBase.GetName(mSelectedQuest.Tasks[i].TargetId)
                                        ), Color.White, Alignments.Left, mQuestDescTemplateLabel.Font
                                    );
                                }
                            }
                        }

                        mQuitButton.IsDisabled = !mSelectedQuest.Quitable;
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress[mSelectedQuest.Id].Completed)
                        {
                            //Completed
                            if (mSelectedQuest.LogAfterComplete)
                            {
                                mQuestStatus.SetText(Strings.QuestLog.completed);
                                mQuestStatus.SetTextColor(Color.Green, Label.ControlState.Normal);
                                mQuestDescLabel.AddText(
                                    mSelectedQuest.EndDescription, Color.White, Alignments.Left,
                                    mQuestDescTemplateLabel.Font
                                );
                            }
                        }
                        else
                        {
                            //Not Started
                            if (mSelectedQuest.LogBeforeOffer)
                            {
                                mQuestStatus.SetText(Strings.QuestLog.notstarted);
                                mQuestStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                                mQuestDescLabel.AddText(
                                    mSelectedQuest.BeforeDescription, Color.White, Alignments.Left,
                                    mQuestDescTemplateLabel.Font
                                );

                                mQuitButton?.Hide();
                            }
                        }
                    }
                }
                else
                {
                    //Not Started
                    if (mSelectedQuest.LogBeforeOffer)
                    {
                        mQuestStatus.SetText(Strings.QuestLog.notstarted);
                        mQuestStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                        mQuestDescLabel.AddText(
                            mSelectedQuest.BeforeDescription, Color.White, Alignments.Left, mQuestDescTemplateLabel.Font
                        );
                    }
                }

                mQuestList.Hide();
                mQuestTitle.IsHidden = false;
                mQuestTitle.Text = mSelectedQuest.Name;
                mQuestDescArea.IsHidden = false;
                mQuestDescLabel.Width = mQuestDescArea.Width - mQuestDescArea.GetVerticalScrollBar().Width;
                mQuestDescLabel.SizeToChildren(false, true);
                mQuestStatus.Show();
                mBackButton.Show();
                mQuitButton.Show();
            }
        }

        public void Show()
        {
            mQuestsWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mQuestsWindow.IsHidden;
        }

        public void Hide()
        {
            mQuestsWindow.IsHidden = true;
            mSelectedQuest = null;
        }

    }

}
