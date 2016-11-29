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
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Input;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.UI.Game
{
    public class QuestsWindow
    {
        //Controls
        private WindowControl _questsWindow;
        private ListBox _questList;
        private List<ListBoxRow> _questItems = new List<ListBoxRow>();

        //Init
        public QuestsWindow(Canvas _gameCanvas)
        {
            _questsWindow = new WindowControl(_gameCanvas, "Quest Log");
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
        }

        //Methods
        public void Update(bool shouldUpdateList)
        {
            if (shouldUpdateList)
            {
                foreach (var quest in _questItems)
                {
                    _questList.RemoveChild(quest, true);
                }
                _questItems.Clear();
                if (Globals.Me != null)
                {
                    var quests = QuestBase.GetObjects();
                    foreach (var quest in quests)
                    {
                        if (quest.Value != null)
                        {
                            if (quest.Value.LogBeforeOffer == 1)
                            {

                            }
                            else if (quest.Value.LogAfterComplete == 1)
                            {

                            }
                            _questList.AddRow(quest.Value.Name);

                        }
                    }
                }
            }

            if (_questsWindow.IsHidden) { return; }
            
            
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
        }
    }
}
