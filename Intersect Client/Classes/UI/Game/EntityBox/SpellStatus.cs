using System;
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
        private SpellDescWindow _descWindow;

        //Drag/Drop References
        private Intersect_Client.Classes.UI.Game.EntityBox _entityBox;

        public ImagePanel container;
        private int currentSpell = -1;

        private int myindex;
        public ImagePanel pnl;

        private string texLoaded = "";
        public Label timeLabel;

        public SpellStatus(Intersect_Client.Classes.UI.Game.EntityBox entityBox, int index)
        {
            _entityBox = entityBox;
            myindex = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container, "StatusIcon");
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
        }

        public void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (myindex >= _entityBox._myEntity.Status.Count)
            {
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            _descWindow = new SpellDescWindow(_entityBox._myEntity.Status[myindex].SpellNum,
                _entityBox._entityWindow.X + 316, _entityBox._entityWindow.Y);
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
            var spell = SpellBase.Lookup.Get<SpellBase>(_entityBox._myEntity.Status[myindex].SpellNum);
            var timeDiff = Globals.System.GetTimeMs() - _entityBox._myEntity.Status[myindex].TimeRecevied;
            var remaining = _entityBox._myEntity.Status[myindex].TimeRemaining - timeDiff;
            var fraction = (float) ((float) remaining / (float) _entityBox._myEntity.Status[myindex].TotalDuration);
            pnl.RenderColor = new IntersectClientExtras.GenericClasses.Color((int) (fraction * 255f), 255, 255, 255);
            if ((texLoaded != "" && spell == null) || (spell != null && texLoaded != spell.Pic) ||
                currentSpell != _entityBox._myEntity.Status[myindex].SpellNum)
            {
                if (spell != null)
                {
                    GameTexture spellTex =
                        Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Pic);
                    if (spellTex != null)
                    {
                        pnl.Texture = spellTex;
                        pnl.IsHidden = false;
                    }
                    else
                    {
                        if (pnl.Texture != null)
                        {
                            pnl.Texture = null;
                        }
                    }
                    texLoaded = spell.Pic;
                    currentSpell = _entityBox._myEntity.Status[myindex].SpellNum;
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
        }
    }
}