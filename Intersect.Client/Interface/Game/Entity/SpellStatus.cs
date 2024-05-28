using Intersect.Client.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Entity
{

    public partial class SpellStatus
    {

        public ImagePanel Container;

        private Guid _currentSpellId;

        private SpellDescriptionWindow _descriptionWindow;

        private Label _durationLabel;

        //Drag/Drop References
        private TargetWindows _targetStatusWindow;

        private PlayerStatusWindows _playerStatusWindow;

        private Status _status;

        private string _textureLoaded = "";

        public ImagePanel StatusIcon;

        public SpellStatus(TargetWindows targetStatusWindow, Status status)
        {
            _targetStatusWindow = targetStatusWindow;
            _status = status;
        }

        public SpellStatus(PlayerStatusWindows playerStatusWindow, Status status)
        {
            _playerStatusWindow = playerStatusWindow;
            _status = status;
        }

        public void Setup(bool playerWindow = false)
        {
            StatusIcon = new ImagePanel(Container, "StatusIcon");
            StatusIcon.HoverEnter += (sender, arguments) => pnl_HoverEnter(sender, arguments, playerWindow);
            StatusIcon.HoverLeave += pnl_HoverLeave;
            _durationLabel = new Label(Container, "DurationLabel");
        }

        public void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (_descriptionWindow != null)
            {
                _descriptionWindow.Dispose();
                _descriptionWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments, bool playerWindow = false)
        {
            if (_descriptionWindow != null)
            {
                _descriptionWindow.Dispose();
                _descriptionWindow = null;
            }

            int X, Y;
            X = playerWindow ? _playerStatusWindow.PlayerStatusWindow.X : _targetStatusWindow.TargetWindow.X;
            Y = playerWindow ? _playerStatusWindow.PlayerStatusWindow.Y : _targetStatusWindow.TargetWindow.Y;
            _descriptionWindow = new SpellDescriptionWindow(_status.SpellId, X + StatusIcon.X + 16, Y + Container.Parent.Y + Container.Bottom + 2);
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect
            {
                X = StatusIcon.LocalPosToCanvas(new Point(0, 0)).X,
                Y = StatusIcon.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = StatusIcon.Width,
                Height = StatusIcon.Height
            };

            return rect;
        }

        public void UpdateStatus(Status status)
        {
            _status = status;
        }

        public void Update()
        {
            if (_status == null)
            {
                return;
            }

            var remaining = _status.RemainingMs;
            var spell = SpellBase.Get(_status.SpellId);

            _durationLabel.Text = TimeSpan.FromMilliseconds(remaining).WithSuffix();

            if ((_textureLoaded != "" && spell == null ||
                 spell != null && _textureLoaded != spell.Icon ||
                 _currentSpellId != _status.SpellId) &&
                remaining > 0)
            {
                Container.Show();
                if (spell != null)
                {
                    var spellTex = Globals.ContentManager.GetTexture(
                        Framework.Content.TextureType.Spell, spell.Icon
                    );

                    if (spellTex != null)
                    {
                        StatusIcon.Texture = spellTex;
                        StatusIcon.IsHidden = false;
                    }
                    else
                    {
                        if (StatusIcon.Texture != null)
                        {
                            StatusIcon.Texture = null;
                        }
                    }

                    _textureLoaded = spell.Icon;
                    _currentSpellId = _status.SpellId;
                }
                else
                {
                    if (StatusIcon.Texture != null)
                    {
                        StatusIcon.Texture = null;
                    }

                    _textureLoaded = "";
                }
            }
            else if (remaining <= 0)
            {
                if (StatusIcon.Texture != null)
                {
                    StatusIcon.Texture = null;
                }

                Container.Hide();
                _textureLoaded = "";
            }
        }

    }

}
