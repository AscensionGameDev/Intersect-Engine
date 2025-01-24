using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.GameObjects;
using Intersect.Utilities;
using Label = Intersect.Client.Framework.Gwen.Control.Label;

namespace Intersect.Client.Interface.Game.EntityPanel;


public partial class SpellStatus
{
    private Guid _currentSpellId;

    private SpellDescriptionWindow? _descriptionWindow;

    private Label _durationLabel;

    private Status _status;

    private string _textureLoaded = string.Empty;

    private ImagePanel _statusIcon;

    private ImagePanel _spellStatusContainer;

    public SpellStatus(Status status)
    {
        _status = status;
    }

    public void Setup()
    {
        _statusIcon = new ImagePanel(_spellStatusContainer, "StatusIcon");
        _statusIcon.HoverEnter += pnl_HoverEnter;
        _statusIcon.HoverLeave += pnl_HoverLeave;
        _durationLabel = new Label(_spellStatusContainer, "DurationLabel");
    }

    public void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        if (_descriptionWindow != null)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = null;
        }
    }

    void pnl_HoverEnter(Base sender, EventArgs arguments)
    {
        if (_descriptionWindow != null)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = null;
        }

        var X = _statusIcon.LocalPosToCanvas(new Point(0, 0)).X;
        var Y = _statusIcon.LocalPosToCanvas(new Point(0, 0)).Y;

        _descriptionWindow = new SpellDescriptionWindow(_status.SpellId, _statusIcon);
    }

    public void UpdateStatus(Status status)
    {
        _status = status;
    }

    public static void UpdateSpellStatus(
        IEntity myEntity,
        ScrollControl spellStatusControl,
        Dictionary<Guid, SpellStatus> activeStatuses
    )
    {
        if (myEntity is not Entity entity)
        {
            return;
        }

        //Remove 'Dead' Statuses
        var statuses = activeStatuses.Keys.ToArray();
        foreach (var status in statuses)
        {
            if (!entity.StatusActive(status))
            {
                var s = activeStatuses[status];
                s._statusIcon.Texture = null;
                s._spellStatusContainer.Hide();
                s._spellStatusContainer.Texture = null;
                spellStatusControl.RemoveChild(s._spellStatusContainer, true);
                s.pnl_HoverLeave(null, null);
                activeStatuses.Remove(status);
            }
            else
            {
                activeStatuses[status].UpdateStatus(entity.GetStatus(status) as Status);
            }
        }

        //Add all of the spell status effects
        for (var i = 0; i < myEntity.Status.Count; i++)
        {
            var id = myEntity.Status[i].SpellId;
            SpellStatus itm;
            if (!activeStatuses.ContainsKey(id) && myEntity.Status[i] is Status status)
            {
                itm = new SpellStatus(status)
                {
                    _spellStatusContainer = new ImagePanel(spellStatusControl, "SpellStatusIcon")
                };

                itm.Setup();
                itm._spellStatusContainer.LoadJsonUi(
                    GameContentManager.UI.InGame,
                    Graphics.Renderer.GetResolutionString()
                );
                itm._spellStatusContainer.Name = string.Empty;
                activeStatuses.Add(id, itm);
            }
            else
            {
                itm = activeStatuses[id];
            }

            var xPadding = itm._spellStatusContainer.Margin.Left + itm._spellStatusContainer.Margin.Right;
            var yPadding = itm._spellStatusContainer.Margin.Top + itm._spellStatusContainer.Margin.Bottom;

            // Calculate how many icons fit in a row
            int iconsPerRow = spellStatusControl.Width / Math.Max(1, itm._spellStatusContainer.Width + xPadding);

            // Calculate which row the icon should be in
            int row = i / Math.Max(1, iconsPerRow);

            // Calculate which column the icon should be in
            int column = i % Math.Max(1, iconsPerRow);

            // Calculate the x and y position
            int xPos = column * (itm._spellStatusContainer.Width + xPadding) + xPadding;
            int yPos = row * (itm._spellStatusContainer.Height + yPadding) + yPadding;

            itm._spellStatusContainer.SetPosition(xPos, yPos);
        }
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
            _spellStatusContainer.Show();
            if (spell != null)
            {
                var spellTex = Globals.ContentManager.GetTexture(
                    Framework.Content.TextureType.Spell, spell.Icon
                );

                if (spellTex != null)
                {
                    _statusIcon.Texture = spellTex;
                    _statusIcon.IsHidden = false;
                }
                else
                {
                    if (_statusIcon.Texture != null)
                    {
                        _statusIcon.Texture = null;
                    }
                }

                _textureLoaded = spell.Icon;
                _currentSpellId = _status.SpellId;
            }
            else
            {
                if (_statusIcon.Texture != null)
                {
                    _statusIcon.Texture = null;
                }

                _textureLoaded = string.Empty;
            }
        }
        else if (remaining <= 0)
        {
            if (_statusIcon.Texture != null)
            {
                _statusIcon.Texture = null;
            }

            _spellStatusContainer.Hide();
            _textureLoaded = string.Empty;
        }
    }
}
