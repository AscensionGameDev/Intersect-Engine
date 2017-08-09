using System;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Spells
{
    public class SpellItem
    {
        private SpellDescWindow _descWindow;

        //Drag/Drop References
        private SpellsWindow _spellWindow;

        private bool CanDrag;
        private long ClickTime;
        public ImagePanel container;
        private int currentSpell = -1;
        private Draggable dragIcon;
        private bool iconCD;
        public bool IsDragging;

        private bool MouseOver;
        private int MouseX = -1;
        private int MouseY = -1;

        private int myindex;
        public ImagePanel pnl;

        private string texLoaded = "";

        public SpellItem(SpellsWindow spellWindow, int index)
        {
            _spellWindow = spellWindow;
            myindex = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container, "SpellIcon");
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;
            pnl.Clicked += pnl_Clicked;
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryForgetSpell(myindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                CanDrag = false;
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            _descWindow = new SpellDescWindow(Globals.Me.Spells[myindex].SpellNum, _spellWindow.X - 255,
                _spellWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
                Width = pnl.Width,
                Height = pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            var spell = SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[myindex].SpellNum);
            if (!IsDragging &&
                ((texLoaded != "" && spell == null) || (spell != null && texLoaded != spell.Pic) ||
                 currentSpell != Globals.Me.Spells[myindex].SpellNum ||
                 iconCD != (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS())))
            {
                if (spell != null)
                {
                    GameTexture spellTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell,
                        spell.Pic);
                    if (spellTex != null)
                    {
                        pnl.Texture = spellTex;
                        if ((Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS()))
                        {
                            pnl.RenderColor = new IntersectClientExtras.GenericClasses.Color(100, 255, 255, 255);
                        }
                        else
                        {
                            pnl.RenderColor = new IntersectClientExtras.GenericClasses.Color(255, 255, 255, 255);
                        }
                    }
                    else
                    {
                        if (pnl.Texture != null)
                        {
                            pnl.Texture = null;
                        }
                    }
                    texLoaded = spell.Pic;
                    currentSpell = Globals.Me.Spells[myindex].SpellNum;
                    iconCD = (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS());
                }
                else
                {
                    if (pnl.Texture != null)
                    {
                        pnl.Texture = null;
                    }
                    texLoaded = "";
                }
            }
            if (!IsDragging)
            {
                if (MouseOver)
                {
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        CanDrag = true;
                        MouseX = -1;
                        MouseY = -1;
                        if (Globals.System.GetTimeMS() < ClickTime)
                        {
                            Globals.Me.TryUseSpell(myindex);
                            ClickTime = 0;
                        }
                    }
                    else
                    {
                        if (CanDrag)
                        {
                            if (MouseX == -1 || MouseY == -1)
                            {
                                MouseX = InputHandler.MousePosition.X -
                                         pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X;
                                MouseY = InputHandler.MousePosition.Y -
                                         pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y;
                            }
                            else
                            {
                                int xdiff = MouseX -
                                            (InputHandler.MousePosition.X -
                                             pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .X);
                                int ydiff = MouseY -
                                            (InputHandler.MousePosition.Y -
                                             pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(
                                        pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X +
                                        MouseX,
                                        pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X +
                                        MouseY, pnl.Texture);
                                    texLoaded = "";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (dragIcon.Update())
                {
                    //Drug the item and now we stopped
                    IsDragging = false;
                    FloatRect dragRect = new FloatRect(
                        dragIcon.X - (container.Padding.Left + container.Padding.Right) / 2,
                        dragIcon.Y - (container.Padding.Top + container.Padding.Bottom) / 2,
                        (container.Padding.Left + container.Padding.Right) / 2 + pnl.Width,
                        (container.Padding.Top + container.Padding.Bottom) / 2 + pnl.Height);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check spell first.
                    if (_spellWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_spellWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (myindex != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendSwapSpells(bestIntersectIndex, myindex);
                                Globals.Me.SwapSpells(bestIntersectIndex, myindex);
                            }
                        }
                    }
                    else if (Gui.GameUI.Hotbar.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxHotbar; i++)
                        {
                            if (Gui.GameUI.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.AddToHotbar(bestIntersectIndex, 1, myindex);
                        }
                    }
                    dragIcon.Dispose();
                }
            }
        }
    }
}