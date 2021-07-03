using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Spells
{

    public class SpellItem
    {

        public ImagePanel Container;

        public bool IsDragging;

        private bool mCanDrag;

        private long mClickTime;

        private Label mCooldownLabel;

        private Guid mCurrentSpellId;

        private SpellDescWindow mDescWindow;

        private Draggable mDragIcon;

        private bool mIconCd;

        private bool mMouseOver;

        private int mMouseX = -1;

        private int mMouseY = -1;

        //Drag/Drop References
        private SpellsWindow mSpellWindow;

        private string mTexLoaded = "";

        private int mYindex;

        public ImagePanel Pnl;

        public SpellItem(SpellsWindow spellWindow, int index)
        {
            mSpellWindow = spellWindow;
            mYindex = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "SpellIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += pnl_RightClicked;
            Pnl.Clicked += pnl_Clicked;
            mCooldownLabel = new Label(Pnl, "SpellCooldownLabel");
            mCooldownLabel.IsHidden = true;
            mCooldownLabel.TextColor = new Color(0, 255, 255, 255);
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Globals.System.GetTimeMs() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryForgetSpell(mYindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            mMouseOver = false;
            mMouseX = -1;
            mMouseY = -1;
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (InputHandler.MouseFocus != null)
            {
                return;
            }

            mMouseOver = true;
            mCanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                mCanDrag = false;

                return;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }

            mDescWindow = new SpellDescWindow(Globals.Me.Spells[mYindex].SpellId, mSpellWindow.X, mSpellWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };

            return rect;
        }

        public void Update()
        {
            var spell = SpellBase.Get(Globals.Me.Spells[mYindex].SpellId);
            if (!IsDragging &&
                (mTexLoaded != "" && spell == null ||
                 spell != null && mTexLoaded != spell.Icon ||
                 mCurrentSpellId != Globals.Me.Spells[mYindex].SpellId ||
                 mIconCd !=
                 Globals.Me.GetSpellCooldown(Globals.Me.Spells[mYindex].SpellId) > Globals.System.GetTimeMs() ||
                 Globals.Me.GetSpellCooldown(Globals.Me.Spells[mYindex].SpellId) > Globals.System.GetTimeMs()))
            {
                mCooldownLabel.IsHidden = true;
                if (spell != null)
                {
                    var spellTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Icon);
                    if (spellTex != null)
                    {
                        Pnl.Texture = spellTex;
                        if (Globals.Me.GetSpellCooldown(Globals.Me.Spells[mYindex].SpellId) >
                            Globals.System.GetTimeMs())
                        {
                            Pnl.RenderColor = new Color(100, 255, 255, 255);
                        }
                        else
                        {
                            Pnl.RenderColor = new Color(255, 255, 255, 255);
                        }
                    }
                    else
                    {
                        if (Pnl.Texture != null)
                        {
                            Pnl.Texture = null;
                        }
                    }

                    mTexLoaded = spell.Icon;
                    mCurrentSpellId = Globals.Me.Spells[mYindex].SpellId;
                    mIconCd = Globals.Me.GetSpellCooldown(Globals.Me.Spells[mYindex].SpellId) >
                              Globals.System.GetTimeMs();

                    if (mIconCd)
                    {
                        mCooldownLabel.IsHidden = false;
                        var secondsRemaining =
                            (float) (Globals.Me.GetSpellCooldown(Globals.Me.Spells[mYindex].SpellId) -
                                     Globals.System.GetTimeMs()) /
                            1000f;

                        if (secondsRemaining > 10f)
                        {
                            mCooldownLabel.Text = Strings.Spells.cooldown.ToString(secondsRemaining.ToString("N0"));
                        }
                        else
                        {
                            mCooldownLabel.Text = Strings.Spells.cooldown.ToString(
                                secondsRemaining.ToString("N1").Replace(".", Strings.Numbers.dec)
                            );
                        }
                    }
                }
                else
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }

                    mTexLoaded = "";
                }
            }

            if (!IsDragging)
            {
                if (mMouseOver)
                {
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        mCanDrag = true;
                        mMouseX = -1;
                        mMouseY = -1;
                        if (Globals.System.GetTimeMs() < mClickTime)
                        {
                            Globals.Me.TryUseSpell(mYindex);
                            mClickTime = 0;
                        }
                    }
                    else
                    {
                        if (mCanDrag && Draggable.Active == null)
                        {
                            if (mMouseX == -1 || mMouseY == -1)
                            {
                                mMouseX = InputHandler.MousePosition.X - Pnl.LocalPosToCanvas(new Point(0, 0)).X;
                                mMouseY = InputHandler.MousePosition.Y - Pnl.LocalPosToCanvas(new Point(0, 0)).Y;
                            }
                            else
                            {
                                var xdiff = mMouseX -
                                            (InputHandler.MousePosition.X - Pnl.LocalPosToCanvas(new Point(0, 0)).X);

                                var ydiff = mMouseY -
                                            (InputHandler.MousePosition.Y - Pnl.LocalPosToCanvas(new Point(0, 0)).Y);

                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    mDragIcon = new Draggable(
                                        Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseX,
                                        Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseY, Pnl.Texture, Pnl.RenderColor
                                    );

                                    mTexLoaded = "";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (mDragIcon.Update())
                {
                    //Drug the item and now we stopped
                    IsDragging = false;
                    var dragRect = new FloatRect(
                        mDragIcon.X - (Container.Padding.Left + Container.Padding.Right) / 2,
                        mDragIcon.Y - (Container.Padding.Top + Container.Padding.Bottom) / 2,
                        (Container.Padding.Left + Container.Padding.Right) / 2 + Pnl.Width,
                        (Container.Padding.Top + Container.Padding.Bottom) / 2 + Pnl.Height
                    );

                    float bestIntersect = 0;
                    var bestIntersectIndex = -1;

                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check spell first.
                    if (mSpellWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (i < mSpellWindow.Items.Count &&
                                mSpellWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(mSpellWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(mSpellWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(mSpellWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(mSpellWindow.Items[i].RenderBounds(), dragRect).Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            if (mYindex != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendSwapSpells(bestIntersectIndex, mYindex);
                                Globals.Me.SwapSpells(bestIntersectIndex, mYindex);
                            }
                        }
                    }
                    else if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.MaxHotbar; i++)
                        {
                            if (Interface.GameUi.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(
                                            Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect
                                        )
                                        .Width *
                                    FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                        .Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Width *
                                        FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.AddToHotbar((byte) bestIntersectIndex, 1, mYindex);
                        }
                    }

                    mDragIcon.Dispose();
                }
            }
        }

    }

}
