using Intersect.Admin.Actions;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects.Maps.MapList;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game.Admin;

partial class AdminWindow : WindowControl
{
    private readonly TextBox _textboxName;
    private readonly ComboBox _dropdownAccess;
    private BanMuteBox? _banOrMuteWindow;
    private readonly ComboBox _dropdownSprite;
    public ImagePanel _spritePanel;
    private readonly ComboBox _dropdownFace;
    public ImagePanel _facePanel;
    private readonly CheckBox _checkboxChronological;
    private TreeControl? _mapList;

    public AdminWindow(Base gameCanvas) : base(gameCanvas, Strings.Admin.Title, false, nameof(AdminWindow))
    {
        DisableResizing();
        Margin = Margin.Zero;
        Padding = Padding.Zero;

        // Name label and textbox
        _ = new Label(this, "LabelName") { Text = Strings.Admin.Name };
        _textboxName = new TextBox(this, "TextboxName");
        Interface.FocusElements.Add(_textboxName);

        // Access label, dropdown and set power button
        _ = new Label(this, "LabelAccess") { Text = Strings.Admin.Access };
        _dropdownAccess = new ComboBox(this, "DropdownAccess");
        _ = _dropdownAccess.AddItem(label: Strings.Admin.Access0, userData: "None");
        _ = _dropdownAccess.AddItem(label: Strings.Admin.Access1, userData: "Moderator");
        _ = _dropdownAccess.AddItem(label: Strings.Admin.Access2, userData: "Admin");
        var buttonSetPower = new Button(this, "ButtonSetPower") { Text = Strings.Admin.SetPower };
        buttonSetPower.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                string? power = _dropdownAccess.SelectedItem.UserData.ToString();
                if (!string.IsNullOrEmpty(power) && power.Trim().Length > 0)
                {
                    PacketSender.SendAdminAction(new SetAccessAction(_textboxName.Text, power));
                }
            }
        };

        #region Quick Admin Actions
        // Warp to me admin action
        var buttonWarpToMe = new Button(this, "ButtonWarpToMe") { Text = Strings.Admin.Warp2Me };
        buttonWarpToMe.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpToMeAction(_textboxName.Text));
            }
        };

        // Warp to player admin action
        var buttonWarpMeTo = new Button(this, "ButtonWarpMeTo") { Text = Strings.Admin.WarpMe2 };
        buttonWarpMeTo.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpMeToAction(_textboxName.Text));
            }
        };

        // Warp to overworld admin action
        var buttonOverworldReturn = new Button(this, "ButtonOverworldReturn") { Text = Strings.Admin.OverworldReturn };
        buttonOverworldReturn.Clicked += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_textboxName.Text))
            {
                PacketSender.SendAdminAction(new ReturnToOverworldAction(_textboxName.Text));
            }
        };

        // Kick player admin action
        var buttonKick = new Button(this, "ButtonKick") { Text = Strings.Admin.Kick };
        buttonKick.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KickAction(_textboxName.Text));
            }
        };

        // Kill player admin action
        var buttonKill = new Button(this, "ButtonKill") { Text = Strings.Admin.Kill };
        buttonKill.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KillAction(_textboxName.Text));
            }
        };

        // Ban and Unban player admin actions
        var buttonBan = new Button(this, "ButtonBan") { Text = Strings.Admin.Ban };
        buttonBan.Clicked += banButton_Clicked;

        var buttonUnban = new Button(this, "ButtonUnban") { Text = Strings.Admin.Unban };
        buttonUnban.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                _ = new InputBox(
                    title: Strings.Admin.UnbanCaption.ToString(_textboxName.Text),
                    prompt: Strings.Admin.UnbanPrompt.ToString(_textboxName.Text),
                    inputType: InputBox.InputType.YesNo,
                    onSuccess: (s, e) => PacketSender.SendAdminAction(new UnbanAction(_textboxName.Text))
                );
            }
        };

        // Mute and Unmute player admin actions
        var buttonMute = new Button(this, "ButtonMute") { Text = Strings.Admin.Mute };
        buttonMute.Clicked += muteButton_Clicked;

        var buttonUnmute = new Button(this, "ButtonUnmute") { Text = Strings.Admin.Unmute };
        buttonUnmute.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                _ = new InputBox(
                    title: Strings.Admin.UnmuteCaption.ToString(_textboxName.Text),
                    prompt: Strings.Admin.UnmutePrompt.ToString(_textboxName.Text),
                    inputType: InputBox.InputType.YesNo,
                    onSuccess: (s, e) => PacketSender.SendAdminAction(new UnmuteAction(_textboxName.Text))
                );
            }
        };
        #endregion Quick Admin Actions

        #region Change Player Sprite and Face
        // Change player sprite admin action
        _ = new Label(this, "LabelSprite") { Text = Strings.Admin.Sprite };
        _dropdownSprite = new ComboBox(this, "DropdownSprite");
        _ = _dropdownSprite.AddItem(Strings.Admin.None);
        _dropdownSprite.ItemSelected += _dropdownSprite_ItemSelected;

        var sprites = Globals.ContentManager.GetTextureNames(TextureType.Entity);
        Array.Sort(sprites, new AlphanumComparatorFast());
        foreach (var sprite in sprites)
        {
            _ = _dropdownSprite.AddItem(sprite);
        }

        var buttonSetSprite = new Button(this, "ButtonSetSprite") { Text = Strings.Admin.SetSprite };
        buttonSetSprite.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetSpriteAction(_textboxName.Text, _dropdownSprite.Text));
            }
        };

        var panelSprite = new ImagePanel(this, "PanelSprite");
        _spritePanel = new ImagePanel(panelSprite);

        // Change player face admin action
        _ = new Label(this, "LabelFace") { Text = Strings.Admin.Face };
        _dropdownFace = new ComboBox(this, "DropdownFace");
        _ = _dropdownFace.AddItem(Strings.Admin.None);
        _dropdownFace.ItemSelected += _dropdownFace_ItemSelected;

        var faces = Globals.ContentManager.GetTextureNames(TextureType.Face);
        Array.Sort(faces, new AlphanumComparatorFast());
        foreach (var face in faces)
        {
            _ = _dropdownFace.AddItem(face);
        }

        var buttonSetFace = new Button(this, "ButtonSetFace") { Text = Strings.Admin.SetFace };
        buttonSetFace.Clicked += (s, e) =>
        {
            if (_textboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetFaceAction(_textboxName.Text, _dropdownFace.Text));
            }
        };

        var panelFace = new ImagePanel(this, "PanelFace");
        _facePanel = new ImagePanel(panelFace);

        #endregion Change Player Sprite and Face

        // Map list
        _ = new Label(this, "LabelMapList") { Text = Strings.Admin.MapList };
        _checkboxChronological = new CheckBox(this, "CheckboxChronological");
        _checkboxChronological.SetToolTipText(Strings.Admin.ChronologicalTip);
        _checkboxChronological.CheckChanged += (s, e) => UpdateMapList();
        _ = new Label(this, "LabelChronological") { Text = Strings.Admin.Chronological };

        LoadJsonUi(UI.InGame, Graphics.Renderer?.GetResolutionString(), true);
        UpdateMapList();
    }

    public void SetName(string name) => _textboxName.Text = name;

    private void UpdateMapList()
    {
        _mapList?.Dispose();
        _mapList = new TreeControl(this)
        {
            X = 4,
            Y = 330,
            Width = Width - 8,
            Height = 80,
            RenderColor = Color.FromArgb(255, 255, 255, 255),
            MaximumSize = new Point(4096, 999999)
        };

        AddMapListToTree(MapList.List, null);
    }

    private void AddMapListToTree(MapList mapList, TreeNode? parent)
    {
        if (_mapList == default)
        {
            return;
        }

        TreeNode tmpNode;
        if (_checkboxChronological.IsChecked)
        {
            for (var i = MapList.OrderedMaps.Count - 1; i >= 0; i--)
            {
                tmpNode = _mapList.AddNode(MapList.OrderedMaps[i].Name);
                tmpNode.UserData = MapList.OrderedMaps[i].MapId;
                tmpNode.Clicked += tmpNode_Clicked;
                tmpNode.DoubleClicked += tmpNode_Clicked;
            }

            return;
        }

        foreach (var item in mapList.Items)
        {
            switch (item)
            {
                case MapListFolder folder:
                    tmpNode = parent?.AddNode(item.Name) ?? _mapList.AddNode(item.Name);
                    tmpNode.UserData = folder;
                    AddMapListToTree(folder.Children, tmpNode);
                    break;
                case MapListMap map:
                    tmpNode = parent?.AddNode(item.Name) ?? _mapList.AddNode(item.Name);
                    tmpNode.UserData = map.MapId;
                    tmpNode.Clicked += tmpNode_Clicked;
                    tmpNode.DoubleClicked += tmpNode_Clicked;
                    break;
            }
        }
    }

    #region Action Handlers
    private void banButton_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (string.IsNullOrWhiteSpace(_textboxName.Text))
        {
            return;
        }

        var name = _textboxName.Text.Trim();
        if (string.Equals(name, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            title: Strings.Admin.BanCaption.ToString(name),
            prompt: Strings.Admin.BanPrompt.ToString(_textboxName.Text),
            okayHandler: (s, e) =>
            {
                PacketSender.SendAdminAction(
                    new BanAction(
                        name: _textboxName.Text,
                        durationDays: _banOrMuteWindow?.GetDuration() ?? 0,
                        reason: _banOrMuteWindow?.GetReason() ?? string.Empty,
                        banIp: _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void muteButton_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (string.IsNullOrWhiteSpace(_textboxName.Text))
        {
            return;
        }

        var name = _textboxName.Text.Trim();
        if (string.Equals(name, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            title: Strings.Admin.MuteCaption.ToString(name),
            prompt: Strings.Admin.MutePrompt.ToString(_textboxName.Text),
            okayHandler: (s, e) =>
            {
                PacketSender.SendAdminAction(
                    new MuteAction(
                        name: _textboxName.Text,
                        durationDays: _banOrMuteWindow?.GetDuration() ?? 0,
                        reason: _banOrMuteWindow?.GetReason() ?? string.Empty,
                        banIp: _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void _dropdownSprite_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        _spritePanel.Texture = Globals.ContentManager.GetTexture(TextureType.Entity, _dropdownSprite.Text);

        if (_spritePanel.Texture == null)
        {
            return;
        }

        var textFrameWidth = _spritePanel.Texture.Width / Options.Instance.Sprites.NormalFrames;
        var textFrameHeight = _spritePanel.Texture.Height / Options.Instance.Sprites.Directions;
        _spritePanel.SetTextureRect(0, 0, textFrameWidth, textFrameHeight);
        _ = _spritePanel.SetSize(Math.Min(textFrameWidth, 46), Math.Min(textFrameHeight, 46));
        Align.Center(_spritePanel);
    }

    private void _dropdownFace_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        _facePanel.Texture = Globals.ContentManager.GetTexture(TextureType.Face, _dropdownFace.Text);

        if (_facePanel.Texture == null)
        {
            return;
        }

        var textFrameWidth = _facePanel.Texture.Width;
        var textFrameHeight = _facePanel.Texture.Height;
        _facePanel.SetTextureRect(0, 0, textFrameWidth, textFrameHeight);
        _ = _facePanel.SetSize(Math.Min(textFrameWidth, 46), Math.Min(textFrameHeight, 46));
        Align.Center(_facePanel);
    }

    private void tmpNode_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (sender is TreeNode treeNode && treeNode.UserData is Guid mapId)
        {
            PacketSender.SendAdminAction(new WarpToMapAction(mapId));
        }
    }
    #endregion
}
