using Intersect.Admin.Actions;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game.Admin;

public partial class AdminWindow : Window
{
    private readonly LabeledComboBox _accessDropdown;

    private readonly Panel _accessPanel;
    private readonly Button _accessSetButton;

    private readonly Panel _actionPanel;
    private readonly Table _actionTable;
    private readonly Button _banButton;
    private readonly IFont? _defaultFont;
    private readonly TexturePicker _faceTexturePicker;
    private readonly Button _kickPlayerButton;
    private readonly Button _killPlayerButton;
    private readonly Button _leaveInstanceButton;
    private readonly Label _mapListLabel;

    private readonly Panel _mapListPanel;
    private readonly Panel _mapListPanelHeader;
    private readonly LabeledCheckBox _mapSortCheckbox;
    private readonly Button _muteButton;
    private readonly TextBox _nameInput;
    private readonly Label _nameLabel;

    private readonly Panel _namePanel;

    private readonly TexturePicker _spriteTexturePicker;
    private readonly Button _unbanButton;
    private readonly Button _unmuteButton;
    private readonly Button _warpMeToPlayerButton;
    private readonly Button _warpPlayerToMeButton;

    private BanMuteBox? _banOrMuteWindow;
    private TreeControl? _mapTree;

    public AdminWindow(Base gameCanvas) : base(
        gameCanvas,
        Strings.AdminWindow.Title,
        false,
        nameof(AdminWindow)
    )
    {
        _defaultFont = Current.GetFont(TitleLabel.FontName);

        IsResizable = false;
        TitleLabel.FontSize = 14;

        MinimumSize = new Point(396, 600);
        InnerPanelPadding = new Padding(8);
        InnerPanel.DockChildSpacing = new Padding(8);

        #region Name Input

        _namePanel = new Panel(this, nameof(_namePanel))
        {
            Dock = Pos.Top, ShouldDrawBackground = false,
        };

        _nameLabel = new Label(_namePanel, nameof(_nameLabel))
        {
            Dock = Pos.Left,
            Font = _defaultFont,
            FontSize = 12,
            Margin = new Margin(0, 0, 4, 0),
            Text = Strings.AdminWindow.Name,
            TextAlign = Pos.Right,
        };
        _nameInput = new TextBox(_namePanel, nameof(_nameInput))
        {
            Font = _defaultFont,
            FontSize = 12,
            Dock = Pos.Fill,
            PlaceholderText = Strings.AdminWindow.NamePlaceholder,
        };
        Interface.FocusComponents.Add(_nameInput);

        #endregion Name Input

        #region Access

        _accessPanel = new Panel(this, nameof(_accessPanel))
        {
            Dock = Pos.Top, ShouldDrawBackground = false,
        };

        _accessSetButton = new Button(_accessPanel, nameof(_accessSetButton))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            Margin = new Margin(4, 0, 0, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.SetPower,
        };
        _accessSetButton.Clicked += AccessSetButtonOnClicked;

        _accessDropdown = new LabeledComboBox(_accessPanel, nameof(_accessDropdown))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            TextPadding = new Padding(8, 4, 0, 4),
            Label = Strings.AdminWindow.Access,
        };
        _ = _accessDropdown.AddItem(Strings.General.None, userData: "None");
        _ = _accessDropdown.AddItem(Strings.AdminWindow.Access1, userData: "Moderator");
        _ = _accessDropdown.AddItem(Strings.AdminWindow.Access2, userData: "Admin");

        #endregion Access

        #region Quick Admin Actions

        _actionPanel = new Panel(this, nameof(_actionPanel))
        {
            Dock = Pos.Top, ShouldDrawBackground = false,
        };

        _actionTable = new Table(_actionPanel, nameof(_actionTable))
        {
            CellSpacing = new Point(8, 8),
            ColumnCount = 3,
            Dock = Pos.Fill,
            FitRowHeightToContents = true,
            Font = _defaultFont,
            FontSize = 12,
            SizeToContents = true,
        };

        _warpMeToPlayerButton = new Button(_actionPanel, nameof(_warpMeToPlayerButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.WarpMeToPlayer,
        };
        _warpMeToPlayerButton.Clicked += WarpMeToPlayerButtonOnClicked;

        _kickPlayerButton = new Button(_actionPanel, nameof(_kickPlayerButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.KickPlayer,
        };
        _kickPlayerButton.Clicked += KickPlayerButtonOnClicked;

        _killPlayerButton = new Button(_actionPanel, nameof(_killPlayerButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.KillPlayer,
        };
        _killPlayerButton.Clicked += KillPlayerButtonOnClicked;

        _warpPlayerToMeButton = new Button(_actionPanel, nameof(_warpPlayerToMeButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.WarpPlayerToMe,
        };
        _warpPlayerToMeButton.Clicked += WarpPlayerToMeButtonOnClicked;

        _muteButton = new Button(_actionPanel, nameof(_muteButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.Mute,
        };
        _muteButton.Clicked += MuteButtonOnClicked;

        _unmuteButton = new Button(_actionPanel, nameof(_unmuteButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.Unmute,
        };
        _unmuteButton.Clicked += UnmuteButtonOnClicked;

        _leaveInstanceButton = new Button(_actionPanel, nameof(_leaveInstanceButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.LeaveInstance,
        };
        _leaveInstanceButton.Clicked += LeaveInstanceButtonOnClicked;

        _banButton = new Button(_actionPanel, nameof(_banButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.Ban,
        };
        _banButton.Clicked += BanButtonOnClicked;

        _unbanButton = new Button(_actionPanel, nameof(_unbanButton))
        {
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 0),
            Padding = new Padding(8, 4),
            Text = Strings.AdminWindow.Unban,
        };
        _unbanButton.Clicked += UnbanButtonOnClicked;

        var rowsAdded = _actionTable.AddCells(
            _warpMeToPlayerButton,
            _kickPlayerButton,
            _killPlayerButton,
            _warpPlayerToMeButton,
            _muteButton,
            _unmuteButton,
            _leaveInstanceButton,
            _banButton,
            _unbanButton
        );

        #endregion Quick Admin Actions

        #region Sprite/Face Pickers

        _spriteTexturePicker = new TexturePicker(this, nameof(_spriteTexturePicker))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            ButtonText = Strings.AdminWindow.SetSprite,
            LabelText = Strings.AdminWindow.Sprite,
            TextureType = TextureType.Entity,
        };
        _spriteTexturePicker.Submitted += SpriteTexturePickerOnSubmitted;

        _faceTexturePicker = new TexturePicker(this, nameof(_faceTexturePicker))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            ButtonText = Strings.AdminWindow.SetFace,
            LabelText = Strings.AdminWindow.Face,
            TextureType = TextureType.Face,
        };
        _faceTexturePicker.Submitted += FaceTexturePickerOnSubmitted;

        #endregion Sprite/Face Pickers

        #region Map List

        _mapListPanel = new Panel(this, nameof(_mapListPanel))
        {
            Dock = Pos.Fill, ShouldDrawBackground = false,
        };

        _mapListPanelHeader = new Panel(_mapListPanel, nameof(_mapListPanelHeader))
        {
            Dock = Pos.Top, ShouldDrawBackground = false,
        };

        _mapListLabel = new Label(_mapListPanelHeader, nameof(_mapListLabel))
        {
            Dock = Pos.Left,
            Font = _defaultFont,
            FontSize = 12,
            Text = Strings.AdminWindow.MapList,
        };

        _mapSortCheckbox = new LabeledCheckBox(_mapListPanelHeader, nameof(_mapSortCheckbox))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            Text = Strings.AdminWindow.SortMapList,
            TooltipText = Strings.AdminWindow.SortMapListTooltip,
            TooltipFont = _defaultFont,
            TooltipFontSize = 12,
        };

        _mapSortCheckbox.CheckChanged += MapSortCheckboxOnCheckChanged;

        _mapListPanel.SizeToChildren(recursive: true);

        #endregion Map List

        SkipRender();
    }

    protected override void EnsureInitialized()
    {
        InnerPanel.SizeToChildren(recursive: true);

        LoadJsonUi(UI.InGame, Graphics.Renderer?.GetResolutionString(), saveOutput: true);

        UpdateMapList();
    }

    #region Action Handlers

    private void UnbanButtonOnClicked(Base s, MouseButtonState e)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        _ = new InputBox(
            Strings.AdminWindow.UnbanCaption.ToString(args: playerName),
            Strings.AdminWindow.UnbanPrompt.ToString(args: playerName),
            InputType.YesNo,
            (_, _) => PacketSender.SendAdminAction(new UnbanAction(playerName))
        );
    }

    private void UnmuteButtonOnClicked(Base s, MouseButtonState e)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        _ = new InputBox(
            Strings.AdminWindow.UnmuteCaption.ToString(args: playerName),
            Strings.AdminWindow.UnmutePrompt.ToString(args: playerName),
            InputType.YesNo,
            (_, _) => PacketSender.SendAdminAction(new UnmuteAction(playerName))
        );
    }

    private void WarpPlayerToMeButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        PacketSender.SendAdminAction(new WarpToMeAction(playerName));
    }

    private void KillPlayerButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        PacketSender.SendAdminAction(new KillAction(playerName));
    }

    private void KickPlayerButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        PacketSender.SendAdminAction(new KickAction(playerName));
    }

    private void WarpMeToPlayerButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        PacketSender.SendAdminAction(new WarpMeToAction(playerName));
    }

    public string? PlayerName
    {
        get => _nameInput.Text?.Trim();
        set => _nameInput.Text = value;
    }

    private void FaceTexturePickerOnSubmitted(TexturePicker sender, ValueChangedEventArgs<string?> arguments)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        var textureName = arguments.Value?.Trim();
        PacketSender.SendAdminAction(new SetFaceAction(playerName, textureName));
    }

    private void SpriteTexturePickerOnSubmitted(TexturePicker sender, ValueChangedEventArgs<string?> arguments)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        var textureName = arguments.Value?.Trim();
        PacketSender.SendAdminAction(new SetSpriteAction(playerName, textureName));
    }

    private void LeaveInstanceButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        PacketSender.SendAdminAction(new ReturnToOverworldAction(playerName));
    }

    private void AccessSetButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        var power = _accessDropdown.SelectedItem?.UserData?.ToString()?.Trim();
        if (power is null or { Length: < 1 })
        {
            return;
        }

        PacketSender.SendAdminAction(new SetAccessAction(playerName, power));
    }

    private void BanButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        if (string.Equals(playerName, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            Strings.AdminWindow.BanCaption.ToString(playerName),
            Strings.AdminWindow.BanPrompt.ToString(playerName),
            (_, _) =>
            {
                PacketSender.SendAdminAction(
                    new BanAction(
                        playerName,
                        _banOrMuteWindow?.GetDuration() ?? 0,
                        _banOrMuteWindow?.GetReason() ?? string.Empty,
                        _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void MuteButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        if (PlayerName is not { Length: > 0 } playerName)
        {
            return;
        }

        if (string.Equals(playerName, Globals.Me?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        _banOrMuteWindow = new BanMuteBox(
            Strings.AdminWindow.MuteCaption.ToString(playerName),
            Strings.AdminWindow.MutePrompt.ToString(playerName),
            (_, _) =>
            {
                PacketSender.SendAdminAction(
                    new MuteAction(
                        playerName,
                        _banOrMuteWindow?.GetDuration() ?? 0,
                        _banOrMuteWindow?.GetReason() ?? string.Empty,
                        _banOrMuteWindow?.BanIp() ?? false
                    )
                );

                _banOrMuteWindow?.Dispose();
            }
        );
    }

    private void MapSortCheckboxOnCheckChanged(ICheckbox sender, ValueChangedEventArgs<bool> eventArgs)
    {
        UpdateMapList();
    }

    private void UpdateMapList()
    {
        _mapTree?.Dispose();

        _mapTree = new TreeControl(_mapListPanel, nameof(_mapTree))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
        };

        _mapTree.SelectionChanged += MapTreeSelectionChanged;

        AddMapListToTree(MapList.List, _mapTree);
    }

    private void AddMapListToTree(MapList mapList, TreeNode parent)
    {
        if (_mapSortCheckbox.IsChecked)
        {
            foreach (var mapListMap in MapList.OrderedMaps)
            {
                _ = parent.AddNode(mapListMap.Name, mapListMap.MapId);
            }

            return;
        }

        foreach (var item in mapList.Items)
        {
            switch (item)
            {
                case MapListFolder folder:
                    AddMapListToTree(folder.Children, parent.AddNode(item.Name, folder));
                    break;
                case MapListMap map:
                    parent.AddNode(item.Name, map.MapId);
                    break;
            }
        }
    }

    private static void MapTreeSelectionChanged(Base sender, EventArgs arguments)
    {
        if (sender is not TreeNode treeNode)
        {
            ApplicationContext.Context.Value?.Logger.LogDebug(
                "MapList selection triggered by a sender of type {SenderType} instead of a {TreeNodeType}",
                sender.GetType().GetName(true),
                typeof(TreeNode).GetName(true)
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