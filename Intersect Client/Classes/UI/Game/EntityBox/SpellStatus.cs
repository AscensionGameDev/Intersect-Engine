using System;
using System.Diagnostics;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.EntityBox
{
    public class SpellStatus
    {
        private SpellDescWindow mDescWindow;

        //Drag/Drop References
        private Intersect_Client.Classes.UI.Game.EntityBox mEntityBox;

        public ImagePanel Container;
        private int mCurrentSpell = -1;

        private int mYindex;
        public ImagePanel Pnl;

        private string mTexLoaded = "";
        public Label TimeLabel;

        public SpellStatus(Intersect_Client.Classes.UI.Game.EntityBox entityBox, int index)
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
            mDescWindow = new SpellDescWindow(mEntityBox.MyEntity.Status[mYindex].SpellNum,
                mEntityBox.EntityWindow.X + 316, mEntityBox.EntityWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            if (mYindex < mEntityBox.MyEntity.Status.Count && mEntityBox.MyEntity.Status[mYindex] != null)
            {
                var spell = SpellBase.Lookup.Get<SpellBase>(mEntityBox.MyEntity.Status[mYindex].SpellNum);
                var timeDiff = Globals.System.GetTimeMs() - mEntityBox.MyEntity.Status[mYindex].TimeRecevied;
                var remaining = mEntityBox.MyEntity.Status[mYindex].TimeRemaining - timeDiff;
                var fraction = (float)((float)remaining / (float)mEntityBox.MyEntity.Status[mYindex].TotalDuration);
                Debug.WriteLine(Pnl.RenderColor.A.ToString());
                Pnl.RenderColor = new IntersectClientExtras.GenericClasses.Color((int)(fraction * 255f), 255, 255, 255);
                Debug.WriteLine(Pnl.RenderColor.A.ToString());
                if ((mTexLoaded != "" && spell == null) || (spell != null && mTexLoaded != spell.Pic) || mCurrentSpell != mEntityBox.MyEntity.Status[mYindex].SpellNum)
                {
                    if (spell != null)
                    {
                        GameTexture spellTex =
                            Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Pic);
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
                        mTexLoaded = spell.Pic;
                        mCurrentSpell = mEntityBox.MyEntity.Status[mYindex].SpellNum;
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