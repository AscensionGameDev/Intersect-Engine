/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Control.Layout;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Input;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestsWindow
    {
        //Controls
        private WindowControl _questsWindow;
        private ListBox _questList;
        private Label _questTitle;
        private Label _questStatus;
        private ListBox _questDesc;
        private QuestBase _selectedQuest = null;
        private Button _backButton;
        private Button _quitButton;

        //Init
        public QuestsWindow(Canvas _gameCanvas)
        {
            _questsWindow = new WindowControl(_gameCanvas, Strings.Get("questlog","title"));
            _questsWindow.SetSize(228, 320);
            _questsWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210, GameGraphics.Renderer.GetScreenHeight() - 500);
            _questsWindow.DisableResizing();
            _questsWindow.Margin = Margin.Zero;
            _questsWindow.Padding = new Padding(8, 5, 9, 11);
            _questsWindow.IsHidden = true;

            _questsWindow.SetTitleBarHeight(24);
            _questsWindow.SetCloseButtonSize(20, 20);
            _questsWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "questsactive.png"), WindowControl.ControlState.Active);
            _questsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _questsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _questsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _questsWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _questsWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);
            
            _questList = new ListBox(_questsWindow) { IsDisabled = true };
            _questList.SetPosition(4,4);
            _questList.SetSize(204, 268);
            _questList.ShouldDrawBackground = false;
            _questList.EnableScroll(false, true);
            _questList.AutoHideBars = false;

            var _questsScrollbar = _questList.GetVerticalScrollBar();
            _questsScrollbar.RenderColor = new Color(200, 40, 40, 40);
            _questsScrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            _questsScrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            _questsScrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = _questsScrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            var downButton = _questsScrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);

            _questTitle = new Label(_questsWindow);
            _questTitle.IsHidden = true;
            _questTitle.AutoSizeToContents = false;
            _questTitle.SetText("");
            _questTitle.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);
            _questTitle.SetSize(_questsWindow.Width, 32);
            _questTitle.Alignment = Pos.CenterH;
            _questTitle.SetTextColor(Color.White, Label.ControlState.Normal);

            _questStatus = new Label(_questsWindow);
            _questStatus.IsHidden = true;
            _questStatus.AutoSizeToContents = false;
            _questStatus.SetText("");
            _questStatus.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 10);
            _questStatus.SetSize(_questsWindow.Width, 32);
            _questStatus.Y = 18;
            _questStatus.Alignment = Pos.CenterH;
            _questStatus.SetTextColor(Color.White, Label.ControlState.Normal);

            _questDesc = new ListBox(_questsWindow);
            _questDesc.IsDisabled = true;
            _questDesc.SetPosition(4, 32 + _questsWindow.Padding.Top);
            _questDesc.SetSize(204,208);
            _questDesc.ShouldDrawBackground = false;
            _questDesc.RenderColor = Color.White;
            _questDesc.IsHidden = true;

            var scrollBar = _questDesc.GetVerticalScrollBar();
            scrollBar.RenderColor = new Color(200, 40, 40, 40);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            scrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            upButton = scrollBar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            downButton = scrollBar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);

            _backButton = new Button(_questsWindow);
            _backButton.SetSize(15, 15);
            _backButton.SetPosition(4, 4);
            _backButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrownormal.png"), Button.ControlState.Normal);
            _backButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowclicked.png"), Button.ControlState.Clicked);
            _backButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowhover.png"), Button.ControlState.Hovered);
            _backButton.Hide();
            _backButton.Clicked += _backButton_Clicked;

            _quitButton = new Button(_questsWindow);
            _quitButton.SetSize(49, 18);
            _quitButton.SetText(Strings.Get("questlog", "abandon"));
            _quitButton.SetPosition(159, 256);
            _quitButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 8);
            _quitButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "quitnormal.png"), Button.ControlState.Normal);
            _quitButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "quitclicked.png"), Button.ControlState.Clicked);
            _quitButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "quithover.png"), Button.ControlState.Hovered);
            _quitButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _quitButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _quitButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _quitButton.Clicked += _quitButton_Clicked;
            _quitButton.Hide();


        }

        private void _quitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_selectedQuest != null)
            {
                new InputBox(Strings.Get("questlog", "abandontitle", _selectedQuest.Name), Strings.Get("questlog","abandonprompt",_selectedQuest.Name), true, AbandonQuest, null, _selectedQuest.GetId(), false);
            }
        }

        void AbandonQuest(Object sender, EventArgs e)
        {
            PacketSender.SendCancelQuest(((InputBox)sender).Slot);
        }

        private void _backButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _selectedQuest = null;
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
            if (_questsWindow.IsHidden) { return; }

            if (_selectedQuest != null)
            {
                if (Globals.Me.QuestProgress.ContainsKey(_selectedQuest.GetId()))
                {
                    if (Globals.Me.QuestProgress[_selectedQuest.GetId()].completed == 1 && Globals.Me.QuestProgress[_selectedQuest.GetId()].task == -1)
                    {
                        //Completed
                        if (_selectedQuest.LogAfterComplete == 0)
                        {
                            _selectedQuest = null;
                            UpdateSelectedQuest();
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                if (_selectedQuest.LogBeforeOffer == 0)
                {
                    _selectedQuest = null;
                    UpdateSelectedQuest();
                }
            }
        }

        private void UpdateQuestList()
        {
            _questList.RemoveAllRows();
            if (Globals.Me != null)
            {
                var quests = QuestBase.GetObjects();
                foreach (var quest in quests)
                {
                    if (quest.Value != null)
                    {
                        if (Globals.Me.QuestProgress.ContainsKey(quest.Key))
                        {
                            if (Globals.Me.QuestProgress[quest.Key].task != -1)
                            {
                                AddQuestToList(quest.Value.Name, Color.Yellow, quest.Key);
                            }
                            else
                            {
                                if (Globals.Me.QuestProgress[quest.Key].completed == 1)
                                {
                                    if (quest.Value.LogAfterComplete == 1)
                                    {
                                        AddQuestToList(quest.Value.Name, Color.Green, quest.Key);
                                    }
                                }
                                else
                                {
                                    if (quest.Value.LogBeforeOffer == 1)
                                    {
                                        AddQuestToList(quest.Value.Name, Color.Red, quest.Key);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (quest.Value.LogBeforeOffer == 1)
                            {
                                AddQuestToList(quest.Value.Name, Color.Red, quest.Key);
                            }
                        }
                    }
                }
            }
        }

        private void AddQuestToList(string name, Color clr, int questId)
        {
            var item = _questList.AddRow(name);
            item.UserData = questId;
            item.Clicked += QuestListItem_Clicked;
            item.Selected += Item_Selected;
            item.SetTextColor(clr);
        }

        private void Item_Selected(Base sender, ItemSelectedEventArgs arguments)
        {
            _questList.UnselectAll();
        }

        private void QuestListItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var questNum = (int) ((ListBoxRow) sender).UserData;
            var quest = QuestBase.GetQuest(questNum);
            if (quest != null)
            {
                _selectedQuest = quest;
                UpdateSelectedQuest();
            }
            _questList.UnselectAll();
        }

        private void UpdateSelectedQuest()
        {
            if (_selectedQuest == null)
            {
                _questList.Show();
                _questTitle.Hide();
                _questDesc.Hide();
                _questStatus.Hide();
                _backButton.Hide();
                _quitButton.Hide();
            }
            else
            {
                _questDesc.RemoveAllRows();
                ListBoxRow rw;
                String[] myText = null;
                List<String> taskString = new List<string>();
                if (Globals.Me.QuestProgress.ContainsKey(_selectedQuest.GetId()))
                {
                    if (Globals.Me.QuestProgress[_selectedQuest.GetId()].task != -1)
                    {
                        //In Progress
                        _questStatus.Text = Strings.Get("questlog", "inprogress");
                        _questStatus.SetTextColor(Color.Yellow, Label.ControlState.Normal);
                        myText = Gui.WrapText(_selectedQuest.InProgressDesc, _questDesc.Width - 12, _questDesc.Parent.Skin.DefaultFont);
                        taskString.Add("");
                        taskString.Add(Strings.Get("questlog", "currenttask"));
                        for (int i = 0; i < _selectedQuest.Tasks.Count; i++)
                        {
                            if (_selectedQuest.Tasks[i].Id == Globals.Me.QuestProgress[_selectedQuest.GetId()].task)
                            {
                                taskString.AddRange(Gui.WrapText(_selectedQuest.Tasks[i].Desc, _questDesc.Width - 12,
                                    _questDesc.Parent.Skin.DefaultFont));
                                if (_selectedQuest.Tasks[i].Objective == 1) //Gather Items
                                {
                                    taskString.Add("");
                                    taskString.Add(Strings.Get("questlog", "taskitem",Globals.Me.QuestProgress[_selectedQuest.GetId()].taskProgress, _selectedQuest.Tasks[i].Data2,ItemBase.GetName(_selectedQuest.Tasks[i].Data1)));
                                }
                                else if (_selectedQuest.Tasks[i].Objective == 2) //Kill Npcs
                                {
                                    taskString.Add("");
                                    taskString.Add(Strings.Get("questlog","tasknpc",Globals.Me.QuestProgress[_selectedQuest.GetId()].taskProgress, _selectedQuest.Tasks[i].Data2 ,NpcBase.GetName(_selectedQuest.Tasks[i].Data1)));
                                }
                            }
                        }
                        if (_selectedQuest.Quitable == 1)
                        {
                            _quitButton.Show();
                        }
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress[_selectedQuest.GetId()].completed == 1)
                        {
                            //Completed
                            if (_selectedQuest.LogAfterComplete == 1)
                            {
                                _questStatus.Text = Strings.Get("questlog", "completed");
                                _questStatus.SetTextColor(Color.Green, Label.ControlState.Normal);
                                myText = Gui.WrapText(_selectedQuest.EndDesc, _questDesc.Width - 12, _questDesc.Parent.Skin.DefaultFont);
                            }
                        }
                        else
                        {
                            //Not Started
                            if (_selectedQuest.LogBeforeOffer == 1)
                            {
                                _questStatus.Text = Strings.Get("questlog", "notstarted");
                                _questStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                                myText = Gui.WrapText(_selectedQuest.BeforeDesc, _questDesc.Width - 12, _questDesc.Parent.Skin.DefaultFont);
                            }
                        }
                    }
                    
                }
                else
                {
                    //Not Started
                    if (_selectedQuest.LogBeforeOffer == 1)
                    {
                        _questStatus.Text = Strings.Get("questlog", "notstarted");
                        _questStatus.SetTextColor(Color.Red, Label.ControlState.Normal);
                        myText = Gui.WrapText(_selectedQuest.BeforeDesc, _questDesc.Width - 12, _questDesc.Parent.Skin.DefaultFont);
                    }
                }
                _questList.Hide();
                _questTitle.IsHidden = false;
                _questTitle.Text = _selectedQuest.Name;
                _questTitle.Alignment = Pos.CenterH;
                _questDesc.IsHidden = false;

                _questStatus.Show();
                _questStatus.Alignment = Pos.CenterH;
                _backButton.Show();
                if (myText != null)
                {
                    foreach (var t in myText)
                    {
                        rw = _questDesc.AddRow(t);
                        rw.SetTextColor(Color.White);
                        rw.MouseInputEnabled = false;
                    }
                    foreach (var t in taskString.ToArray())
                    {
                        rw = _questDesc.AddRow(t);
                        rw.SetTextColor(Color.White);
                        rw.MouseInputEnabled = false;
                    }
                }
            }
        }

        public void Show()
        {
            _questsWindow.IsHidden = false;
        }
        public bool IsVisible()
        {
            return !_questsWindow.IsHidden;
        }
        public void Hide()
        {
            _questsWindow.IsHidden = true;
            _selectedQuest = null;
        }
    }
}
