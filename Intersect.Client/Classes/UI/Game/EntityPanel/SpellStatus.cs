using System;
using System.Diagnostics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.GameObjects;

namespace Intersect.Client.UI.Game.EntityPanel
{
    public class SpellStatus
    {
        private SpellDescWindow mDescWindow;

        //Drag/Drop References
        private EntityBox mEntityBox;

        public ImagePanel Container;
        private Guid mCurrentSpellId;

        private int mYindex;
        public ImagePanel Pnl;

        private string mTexLoaded = "";
        public Label TimeLabel;

        public SpellStatus(EntityBox entityBox, int index)
        {
            mEntityBox = entityBox;
            mYindex = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "StatusIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
        }

        public void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (mYindex >= mEntityBox.MyEntity.Status.Count)
            {
                return;
            }
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
            mDescWindow = new SpellDescWindow(mEntityBox.MyEntity.Status[mYindex].SpellId,
                mEntityBox.EntityWindow.X + 316, mEntityBox.EntityWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect
            {
                X = Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            if (mYindex < mEntityBox.MyEntity.Status.Count && mEntityBox.MyEntity.Status[mYindex] != null)
            {
                var spell = SpellBase.Get(mEntityBox.MyEntity.Status[mYindex].SpellId);
                var timeDiff = Globals.System.GetTimeMs() - mEntityBox.MyEntity.Status[mYindex].TimeRecevied;
                var remaining = mEntityBox.MyEntity.Status[mYindex].TimeRemaining - timeDiff;
                var fraction = (float)((float)remaining / (float)mEntityBox.MyEntity.Status[mYindex].TotalDuration);
                Debug.WriteLine(Pnl.RenderColor.A.ToString());
                Pnl.RenderColor = new Framework.GenericClasses.Color((int)(fraction * 255f), 255, 255, 255);
                Debug.WriteLine(Pnl.RenderColor.A.ToString());
                if ((mTexLoaded != "" && spell == null) || (spell != null && mTexLoaded != spell.Icon) || mCurrentSpellId != mEntityBox.MyEntity.Status[mYindex].SpellId)
                {
                    if (spell != null)
                    {
                        GameTexture spellTex =
                            Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Icon);
                        if (spellTex != null)
                        {
                            Pnl.Texture = spellTex;
                            Pnl.IsHidden = false;
                        }
                        else
                        {
                            if (Pnl.Texture != null)
                            {
                                Pnl.Texture = null;
                            }
                        }
                        mTexLoaded = spell.Icon;
                        mCurrentSpellId = mEntityBox.MyEntity.Status[mYindex].SpellId;
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
                else if (remaining <= 0)
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }
                    mTexLoaded = "";
                }
            }
        }
    }
}