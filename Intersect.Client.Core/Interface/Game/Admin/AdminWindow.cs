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
using Intersect.Core;
using Intersect.Framework.Reflection;
using Intersect.GameObjects.Maps.MapList;
using Microsoft.Extensions.Logging;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game.Admin;

public partial class AdminWindow : WindowControl
{
    private readonly Checkbox _checkboxChronological;
    private readonly ComboBox _dropdownAccess;
    private readonly ComboBox _dropdownSprite;
    private readonly ComboBox _dropdownFace;
    private readonly ImagePanel _facePanel;
    private readonly ImagePanel _spritePanel;
    private readonly TextBox _textboxName;

    private BanMuteBox? _banOrMuteWindow;
    private TreeControl? _mapList;

    public AdminWindow(Base gameCanvas) : base(
        gameCanvas,
        Strings.Admin.Title,
        false,
        nameof(AdminWindow)
    )
    {
        IsResizable = false;
        MinimumSize = new Point(320, 320);
        Margin = Margin.Zero;
        Padding = Padding.Zero;

        // Name label and textbox
        _ = new Label(this, "LabelName")
        {
            Text = Strings.Admin.Name,
        };
        _textboxName = new TextBox(this, "TextboxName");
        Interface.FocusComponents.Add(_textboxName);

        // Access label, dropdown and set power button
        _ = new Label(this, "LabelAccess")
        {
            Text = Strings.Admin.Access,
        };
        _dropdownAccess = new ComboBox(this, name: nameof(_dropdownAccess));
        _ = _dropdownAccess.AddItem(Strings.Admin.Access0, userData: "None");
        _ = _dropdownAccess.AddItem(Strings.Admin.Access1, userData: "Moderator");
        _ = _dropdownAccess.AddItem(Strings.Admin.Access2, userData: "Admin");

        var buttonSetPower = new Button(this, "ButtonSetPower")
        {
            Text = Strings.Admin.SetPower,
        };
        buttonSetPower.Clicked += OnButtonSetPowerOnClicked;

        #region Quick Admin Actions

        // Warp to me admin action
        var buttonWarpToMe = new Button(this, "ButtonWarpToMe")
        {
            Text = Strings.Admin.Warp2Me,
        };
        buttonWarpToMe.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpToMeAction(_textboxName.Text));
            }
        };

        // Warp to player admin action
        var buttonWarpMeTo = new Button(this, "ButtonWarpMeTo")
        {
            Text = Strings.Admin.WarpMe2,
        };
        buttonWarpMeTo.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpMeToAction(_textboxName.Text));
            }
        };

        // Warp to overworld admin action
        var buttonOverworldReturn = new Button(this, "ButtonOverworldReturn")
        {
            Text = Strings.Admin.OverworldReturn,
        };
        buttonOverworldReturn.Clicked += (_, _) =>
        {
            if (!string.IsNullOrEmpty(_textboxName.Text))
            {
                PacketSender.SendAdminAction(new ReturnToOverworldAction(_textboxName.Text));
            }
        };

        // Kick player admin action
        var buttonKick = new Button(this, "ButtonKick")
        {
            Text = Strings.Admin.Kick,
        };
        buttonKick.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KickAction(_textboxName.Text));
            }
        };

        // Kill player admin action
        var buttonKill = new Button(this, "ButtonKill")
        {
            Text = Strings.Admin.Kill,
        };
        buttonKill.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KillAction(_textboxName.Text));
            }
        };

        // Ban and Unban player admin actions
        var buttonBan = new Button(this, "ButtonBan")
        {
            Text = Strings.Admin.Ban,
        };
        buttonBan.Clicked += banButton_Clicked;

        var buttonUnban = new Button(this, "ButtonUnban")
        {
            Text = Strings.Admin.Unban,
        };
        buttonUnban.Clicked += (s, e) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                _ = new InputBox(
                    Strings.Admin.UnbanCaption.ToString(_textboxName.Text),
                    Strings.Admin.UnbanPrompt.ToString(_textboxName.Text),
                    InputBox.InputType.YesNo,
                    (_, _) => PacketSender.SendAdminAction(new UnbanAction(_textboxName.Text))
                );
            }
        };

        // Mute and Unmute player admin actions
        var buttonMute = new Button(this, "ButtonMute")
        {
            Text = Strings.Admin.Mute,
        };
        buttonMute.Clicked += muteButton_Clicked;

        var buttonUnmute = new Button(this, "ButtonUnmute")
        {
            Text = Strings.Admin.Unmute,
        };
        buttonUnmute.Clicked += (s, e) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                _ = new InputBox(
                    Strings.Admin.UnmuteCaption.ToString(_textboxName.Text),
                    Strings.Admin.UnmutePrompt.ToString(_textboxName.Text),
                    InputBox.InputType.YesNo,
                    (_, _) => PacketSender.SendAdminAction(new UnmuteAction(_textboxName.Text))
                );
            }
        };

        #endregion Quick Admin Actions

        #region Change Player Sprite and Face

        // Change player sprite admin action
        _ = new Label(this, "LabelSprite")
        {
            Text = Strings.Admin.Sprite,
        };
        _dropdownSprite = new ComboBox(this, "DropdownSprite");
        _ = _dropdownSprite.AddItem(Strings.Admin.None);
        _dropdownSprite.ItemSelected += _dropdownSprite_ItemSelected;

        var sprites = Globals.ContentManager.GetTextureNames(TextureType.Entity);
        Array.Sort(sprites, new AlphanumComparatorFast());
        foreach (var sprite in sprites)
        {
            _ = _dropdownSprite.AddItem(sprite);
        }

        var buttonSetSprite = new Button(this, "ButtonSetSprite")
        {
            Text = Strings.Admin.SetSprite,
        };
        buttonSetSprite.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetSpriteAction(_textboxName.Text, _dropdownSprite.Text));
            }
        };

        var panelSprite = new ImagePanel(this, "PanelSprite");
        _spritePanel = new ImagePanel(panelSprite);

        // Change player face admin action
        _ = new Label(this, "LabelFace")
        {
            Text = Strings.Admin.Face,
        };
        _dropdownFace = new ComboBox(this, "DropdownFace");
        _ = _dropdownFace.AddItem(Strings.Admin.None);
        _dropdownFace.ItemSelected += _dropdownFace_ItemSelected;

        var faces = Globals.ContentManager.GetTextureNames(TextureType.Face);
        Array.Sort(faces, new AlphanumComparatorFast());
        foreach (var face in faces)
        {
            _ = _dropdownFace.AddItem(face);
        }

        var buttonSetFace = new Button(this, "ButtonSetFace")
        {
            Text = Strings.Admin.SetFace,
        };
        buttonSetFace.Clicked += (_, _) =>
        {
            if (_textboxName.Text?.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetFaceAction(_textboxName.Text, _dropdownFace.Text));
            }
        };

        var panelFace = new ImagePanel(this, "PanelFace");
        _facePanel = new ImagePanel(panelFace);

        #endregion Change Player Sprite and Face

        // Map list
        _ = new Label(this, "LabelMapList")
        {
            Text = Strings.Admin.MapList,
        };
        _checkboxChronological = new Checkbox(this, "CheckboxChronological");
        _checkboxChronological.SetToolTipText(Strings.Admin.ChronologicalTip);
        _checkboxChronological.CheckChanged += (_, _) => UpdateMapList();
        _ = new Label(this, "LabelChronological")
        {
            Text = Strings.Admin.Chronological,
        };

        LoadJsonUi(UI.InGame, Graphics.Renderer?.GetResolutionString(), saveOutput: true);
        UpdateMapList();
    }

    private void OnButtonSetPowerOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        var name = _textboxName.Text?.Trim();
        if (name is null or { Length: < 1 })
        {
            return;
        }

        var power = _dropdownAccess.SelectedItem?.UserData.ToString()?.Trim();
        if (power is null or { Length: < 1 })
        {
            return;
        }

        PacketSender.SendAdminAction(new SetAccessAction(name, power));
    }

    public void SetName(string name)
    {
        _textboxName.Text = name;
    }

    private void UpdateMapList()
    {
        _mapList?.Dispose();
        var mapListY = 330;
        var mapListHeight = (_innerPanel?.Height ?? Height) - (mapListY + 4);
        _mapList = new TreeControl(this, nameof(_mapList))
        {
            X = 4,
            Y = mapListY,
            Width = Width - 8,
            Height = mapListHeight,
            MaximumSize = new Point(4096, 999999),
            Font = Current.GetFont("sourcesansproblack", 10),
            TextColorOverride = Color.White,
        };
        _mapList.SelectionChanged += MapList_SelectionChanged;

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
                    break;
            }
        }
    }

    #region Action Handlers

    private void banButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (string.IsNullOrWhiteSpace(_textboxName.Text))
        {
            return;
        }

        var name = _textboxName.Text?.Trim();
        if (string.Equals(name, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            Strings.Admin.BanCaption.ToString(name),
            Strings.Admin.BanPrompt.ToString(_textboxName.Text),
            (_, _) =>
            {
                PacketSender.SendAdminAction(
                    new BanAction(
                        _textboxName.Text,
                        _banOrMuteWindow?.GetDuration() ?? 0,
                        _banOrMuteWindow?.GetReason() ?? string.Empty,
                        _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void muteButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (string.IsNullOrWhiteSpace(_textboxName.Text))
        {
            return;
        }

        var name = _textboxName.Text?.Trim();
        if (string.Equals(name, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            Strings.Admin.MuteCaption.ToString(name),
            Strings.Admin.MutePrompt.ToString(_textboxName.Text),
            (_, _) =>
            {
                PacketSender.SendAdminAction(
                    new MuteAction(
                        _textboxName.Text,
                        _banOrMuteWindow?.GetDuration() ?? 0,
                        _banOrMuteWindow?.GetReason() ?? string.Empty,
                        _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void _dropdownSprite_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        if (_dropdownSprite.Text is not { } spriteName)
        {
            return;
        }

        var spriteTexture = Globals.ContentManager.GetTexture(TextureType.Entity, spriteName);

        if (spriteTexture == null)
        {
            return;
        }

        var textFrameWidth = spriteTexture.Width / Options.Instance.Sprites.NormalFrames;
        var textFrameHeight = spriteTexture.Height / Options.Instance.Sprites.Directions;
        _spritePanel.SetTextureRect(
            0,
            0,
            textFrameWidth,
            textFrameHeight
        );
        _ = _spritePanel.SetSize(Math.Min(textFrameWidth, 46), Math.Min(textFrameHeight, 46));
        Align.Center(_spritePanel);
    }

    private void _dropdownFace_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        if (_dropdownFace.Text is not { } faceName)
        {
            return;
        }

        var faceTexture = Globals.ContentManager.GetTexture(TextureType.Face, faceName);
        _facePanel.Texture = faceTexture;

        if (faceTexture == null)
        {
            return;
        }

        var textFrameWidth = faceTexture.Width;
        var textFrameHeight = faceTexture.Height;
        _facePanel.SetTextureRect(
            0,
            0,
            textFrameWidth,
            textFrameHeight
        );
        _ = _facePanel.SetSize(Math.Min(textFrameWidth, 46), Math.Min(textFrameHeight, 46));
        Align.Center(_facePanel);
    }

    private void MapList_SelectionChanged(Base sender, EventArgs arguments)
    {
        if (sender is not TreeNode treeNode)
        {
            ApplicationContext.Context.Value?.Logger.LogDebug(
                "MapList selection triggered by a sender of type {SenderType} instead of a {TreeNodeType}",
                sender.GetType().GetName(qualified: true),
                typeof(TreeNode).GetName(qualified: true)
            );
            return;
        }

        if (!treeNode.IsSelected)
        {
            // We don't care about unselected nodes
            return;
        }

        if (treeNode is not { UserData: Guid mapId } || mapId == default)
        {
            if (treeNode.UserData is MapListFolder folder)
            {
                ApplicationContext.Context.Value?.Logger.LogInformation(
                    "Selected map list folder '{FolderName}' ({FolderId}) ({ChildrenCount} direct children)",
                    folder.Name,
                    folder.FolderId,
                    folder.Children.Items.Count
                );
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    "Selected non-map map list node '{TreeNodeText}'",
                    treeNode.Text
                );
            }
            return;
        }

        if (Globals.Me?.MapId == mapId)
        {
            ApplicationContext.CurrentContext.Logger.LogInformation(
                "Ignoring warp to map '{MapName}' ({MapId}) because the player is already on the map",
                treeNode.Text,
                mapId
            );
            return;
        }

        PacketSender.SendAdminAction(new WarpToMapAction(mapId));
    }

    #endregion
}