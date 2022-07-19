using System;

using Intersect.Admin.Actions;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects.Maps.MapList;

using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game
{

    partial class AdminWindow
    {

        //Graphics
        public ImagePanel PanelFace;

        private ComboBox DropdownAccess;

        private Label LabelAccess;

        //Controls
        private WindowControl mAdminWindow;

        private Button ButtonBan;

        //Windows
        BanMuteBox mBanMuteWindow;

        private CheckBox CheckboxChronological;

        private ComboBox DropdownFace;

        private Label LabelFace;

        //Player Mod Buttons
        private Button ButtonKick;

        private Button ButtonKill;

        private Label LabelChronological;

        private TreeControl mMapList;

        private Label LabelMapList;

        private Button ButtonMute;

        //Player Mod Labels
        private Label LabelName;

        //Player Mod Textboxes
        private TextBox TextboxName;

        private Button ButtonSetFace;

        private Button ButtonSetPower;

        private Button ButtonSetSprite;

        private ComboBox DropdownSprite;

        private Label LabelSprite;

        private Button ButtonUnban;

        private Button ButtonUnmute;

        private Button ButtonWarpMeTo;

        private Button ButtonWarpToMe;
        
        private Button ButtonOverworldReturn;

        public ImagePanel PanelSprite;

        public ImagePanel SpritePanel;

        //Init
        public AdminWindow(Canvas gameCanvas)
        {
            mAdminWindow = new WindowControl(gameCanvas, Strings.Admin.title, false, nameof(AdminWindow));
            mAdminWindow.SetSize(200, 540);
            mAdminWindow.SetPosition(
                Graphics.Renderer.GetScreenWidth() / 2 - mAdminWindow.Width / 2,
                Graphics.Renderer.GetScreenHeight() / 2 - mAdminWindow.Height / 2
            );

            mAdminWindow.DisableResizing();
            mAdminWindow.Margin = Margin.Zero;
            mAdminWindow.Padding = Padding.Zero;

            //Player Mods
            LabelName = new Label(mAdminWindow, nameof(LabelName));
            LabelName.SetPosition(6, 4);
            LabelName.Text = Strings.Admin.name;

            TextboxName = new TextBox(mAdminWindow, nameof(TextboxName));
            TextboxName.SetBounds(6, 22, 188, 18);
            Interface.FocusElements.Add(TextboxName);

            ButtonWarpToMe = new Button(mAdminWindow, nameof(ButtonWarpToMe))
            {
                Text = Strings.Admin.warp2me
            };

            ButtonWarpToMe.SetBounds(6, 44, 80, 18);
            ButtonWarpToMe.Clicked += _warpToMeButton_Clicked;

            ButtonWarpMeTo = new Button(mAdminWindow, nameof(ButtonWarpMeTo))
            {
                Text = Strings.Admin.warpme2
            };

            ButtonWarpMeTo.SetBounds(6, 64, 80, 18);
            ButtonWarpMeTo.Clicked += _warpMeToButton_Clicked;

            ButtonOverworldReturn = new Button(mAdminWindow, nameof(ButtonOverworldReturn))
            {
                Text = Strings.Admin.OverworldReturn
            };

            ButtonOverworldReturn.SetBounds(6, 84, 80, 18);
            ButtonOverworldReturn.Clicked += _overworldReturn_Clicked;

            ButtonKick = new Button(mAdminWindow, nameof(ButtonKick))
            {
                Text = Strings.Admin.kick
            };

            ButtonKick.SetBounds(90, 44, 50, 18);
            ButtonKick.Clicked += _kickButton_Clicked;

            ButtonKill = new Button(mAdminWindow, nameof(ButtonKill))
            {
                Text = Strings.Admin.kill
            };

            ButtonKill.SetBounds(144, 44, 50, 18);
            ButtonKill.Clicked += _killButton_Clicked;

            ButtonBan = new Button(mAdminWindow, nameof(ButtonBan))
            {
                Text = Strings.Admin.ban
            };

            ButtonBan.SetBounds(90, 64, 50, 18);
            ButtonBan.Clicked += _banButton_Clicked;

            ButtonUnban = new Button(mAdminWindow, nameof(ButtonUnban))
            {
                Text = Strings.Admin.unban
            };

            ButtonUnban.SetBounds(90, 84, 50, 18);
            ButtonUnban.Clicked += _unbanButton_Clicked;

            ButtonMute = new Button(mAdminWindow, nameof(ButtonMute))
            {
                Text = Strings.Admin.mute
            };

            ButtonMute.SetBounds(144, 64, 50, 18);
            ButtonMute.Clicked += _muteButton_Clicked;

            ButtonUnmute = new Button(mAdminWindow, nameof(ButtonUnmute))
            {
                Text = Strings.Admin.unmute
            };

            ButtonUnmute.SetBounds(144, 84, 50, 18);
            ButtonUnmute.Clicked += _unmuteButton_Clicked;

            LabelSprite = new Label(mAdminWindow, nameof(LabelSprite));
            LabelSprite.SetPosition(6, 112);
            LabelSprite.Text = Strings.Admin.sprite;

            DropdownSprite = new ComboBox(mAdminWindow, nameof(DropdownSprite));
            DropdownSprite.SetBounds(6, 128, 80, 18);
            DropdownSprite.AddItem(Strings.Admin.none);
            var sprites = Globals.ContentManager.GetTextureNames(Framework.Content.TextureType.Entity);
            Array.Sort(sprites, new AlphanumComparatorFast());
            foreach (var sprite in sprites)
            {
                DropdownSprite.AddItem(sprite);
            }

            DropdownSprite.ItemSelected += _spriteDropdown_ItemSelected;

            ButtonSetSprite = new Button(mAdminWindow, nameof(ButtonSetSprite))
            {
                Text = Strings.Admin.setsprite
            };

            ButtonSetSprite.SetBounds(6, 148, 80, 18);
            ButtonSetSprite.Clicked += _setSpriteButton_Clicked;

            PanelSprite = new ImagePanel(mAdminWindow, nameof(PanelSprite));
            PanelSprite.SetSize(50, 50);
            PanelSprite.SetPosition(115, 114);
            SpritePanel = new ImagePanel(PanelSprite);

            LabelFace = new Label(mAdminWindow, nameof(LabelFace));
            LabelFace.SetPosition(6, 172);
            LabelFace.Text = Strings.Admin.face;

            DropdownFace = new ComboBox(mAdminWindow, nameof(DropdownFace));
            DropdownFace.SetBounds(6, 188, 80, 18);
            DropdownFace.AddItem(Strings.Admin.none);
            var faces = Globals.ContentManager.GetTextureNames(Framework.Content.TextureType.Face);
            Array.Sort(faces, new AlphanumComparatorFast());
            foreach (var face in faces)
            {
                DropdownFace.AddItem(face);
            }

            DropdownFace.ItemSelected += _faceDropdown_ItemSelected;

            ButtonSetFace = new Button(mAdminWindow, nameof(ButtonSetFace))
            {
                Text = Strings.Admin.setface
            };

            ButtonSetFace.SetBounds(6, 208, 80, 18);
            ButtonSetFace.Clicked += _setFaceButton_Clicked;

            PanelFace = new ImagePanel(mAdminWindow, nameof(PanelFace));
            PanelFace.SetSize(50, 50);
            PanelFace.SetPosition(115, 174);

            LabelAccess = new Label(mAdminWindow, nameof(LabelAccess));
            LabelAccess.SetPosition(6, 232);
            LabelAccess.Text = Strings.Admin.access;

            DropdownAccess = new ComboBox(mAdminWindow, nameof(DropdownAccess));
            DropdownAccess.SetBounds(6, 248, 80, 18);
            DropdownAccess.AddItem(Strings.Admin.access0).UserData = "None";
            DropdownAccess.AddItem(Strings.Admin.access1).UserData = "Moderator";
            DropdownAccess.AddItem(Strings.Admin.access2).UserData = "Admin";

            ButtonSetPower = new Button(mAdminWindow, nameof(ButtonSetPower))
            {
                Text = Strings.Admin.setpower
            };

            ButtonSetPower.SetBounds(6, 268, 80, 18);
            ButtonSetPower.Clicked += _setPowerButton_Clicked;

            CreateMapList();
            LabelMapList = new Label(mAdminWindow, nameof(LabelMapList))
            {
                Text = Strings.Admin.maplist
            };

            LabelMapList.SetPosition(4f, 294);

            CheckboxChronological = new CheckBox(mAdminWindow, nameof(CheckboxChronological));
            CheckboxChronological.SetToolTipText(Strings.Admin.chronologicaltip);
            CheckboxChronological.SetPosition(mAdminWindow.Width - 24, 294);
            CheckboxChronological.CheckChanged += _chkChronological_CheckChanged;

            LabelChronological = new Label(mAdminWindow, nameof(LabelChronological))
            {
                Text = Strings.Admin.chronological
            };

            LabelChronological.SetPosition(CheckboxChronological.X - 30, 294);

            mAdminWindow.LoadJsonUi(UI.InGame, Graphics.Renderer.GetResolutionString(), true);

            UpdateMapList();
        }

        private void _spriteDropdown_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            SpritePanel.Texture = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Entity, DropdownSprite.Text
            );

            if (SpritePanel.Texture != null)
            {
                SpritePanel.SetTextureRect(0, 0, SpritePanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames, SpritePanel.Texture.GetHeight() / Options.Instance.Sprites.Directions);
                SpritePanel.SetSize(SpritePanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames, SpritePanel.Texture.GetHeight() / Options.Instance.Sprites.Directions);
                Align.AlignTop(SpritePanel);
                Align.CenterHorizontally(SpritePanel);
            }
        }

        private void _faceDropdown_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            PanelFace.Texture = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Face, DropdownFace.Text
            );
        }

        //Methods
        public void SetName(string name)
        {
            TextboxName.Text = name;
        }

        private void CreateMapList()
        {
            mMapList = new TreeControl(mAdminWindow);
            mMapList.SetPosition(4f, 316);
            mMapList.Height = 188;
            mMapList.Width = mAdminWindow.Width - 8;
            mMapList.MaximumSize = new Point(4096, 999999);
        }

        public void UpdateMapList()
        {
            mMapList.Dispose();
            CreateMapList();
            AddMapListToTree(MapList.List, null);
        }

        private void AddMapListToTree(MapList mapList, TreeNode parent)
        {
            TreeNode tmpNode;
            if (CheckboxChronological.IsChecked)
            {
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    tmpNode = mMapList.AddNode(MapList.OrderedMaps[i].Name);
                    tmpNode.UserData = MapList.OrderedMaps[i].MapId;
                    tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                    tmpNode.Clicked += tmpNode_DoubleClicked;
                }
            }
            else
            {
                foreach (var item in mapList.Items)
                {
                    switch (item)
                    {
                        case MapListFolder folder:
                            tmpNode = parent?.AddNode(item.Name) ?? mMapList.AddNode(item.Name);
                            tmpNode.UserData = folder;
                            AddMapListToTree(folder.Children, tmpNode);
                            break;
                        case MapListMap map:
                            tmpNode = parent?.AddNode(item.Name) ?? mMapList.AddNode(item.Name);
                            tmpNode.UserData = map.MapId;
                            tmpNode.DoubleClicked += tmpNode_DoubleClicked;
                            tmpNode.Clicked += tmpNode_DoubleClicked;
                            break;
                    }
                }
            }
        }

        void _kickButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KickAction(TextboxName.Text));
            }
        }

        void _killButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new KillAction(TextboxName.Text));
            }
        }

        void _warpToMeButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpToMeAction(TextboxName.Text));
            }
        }

        void _warpMeToButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new WarpMeToAction(TextboxName.Text));
            }
        }

        void _overworldReturn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (!string.IsNullOrEmpty(TextboxName.Text))
            {
                PacketSender.SendAdminAction(new ReturnToOverworldAction(TextboxName.Text));
            }
        }

        void _muteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                mBanMuteWindow = new BanMuteBox(
                    Strings.Admin.mutecaption.ToString(TextboxName.Text),
                    Strings.Admin.muteprompt.ToString(TextboxName.Text), true, MuteUser
                );
            }
        }

        void MuteUser(object sender, EventArgs e)
        {
            PacketSender.SendAdminAction(
                new MuteAction(
                    TextboxName.Text, mBanMuteWindow.GetDuration(), mBanMuteWindow.GetReason(), mBanMuteWindow.BanIp()
                )
            );

            mBanMuteWindow.Dispose();
        }

        void BanUser(object sender, EventArgs e)
        {
            PacketSender.SendAdminAction(
                new BanAction(
                    TextboxName.Text, mBanMuteWindow.GetDuration(), mBanMuteWindow.GetReason(), mBanMuteWindow.BanIp()
                )
            );

            mBanMuteWindow.Dispose();
        }

        void _banButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0 &&
                TextboxName.Text.Trim().ToLower() != Globals.Me.Name.Trim().ToLower())
            {
                mBanMuteWindow = new BanMuteBox(
                    Strings.Admin.bancaption.ToString(TextboxName.Text),
                    Strings.Admin.banprompt.ToString(TextboxName.Text), true, BanUser
                );
            }
        }

        private void _setFaceButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetFaceAction(TextboxName.Text, DropdownFace.Text));
            }
        }

        void _unmuteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                var confirmWindow = new InputBox(
                    Strings.Admin.unmutecaption.ToString(TextboxName.Text),
                    Strings.Admin.unmuteprompt.ToString(TextboxName.Text), true, InputBox.InputType.YesNo, UnmuteUser,
                    null, -1
                );
            }
        }

        void _unbanButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                var confirmWindow = new InputBox(
                    Strings.Admin.unbancaption.ToString(TextboxName.Text),
                    Strings.Admin.unbanprompt.ToString(TextboxName.Text), true, InputBox.InputType.YesNo, UnbanUser,
                    null, -1
                );
            }
        }

        void UnmuteUser(object sender, EventArgs e)
        {
            PacketSender.SendAdminAction(new UnmuteAction(TextboxName.Text));
        }

        void UnbanUser(object sender, EventArgs e)
        {
            PacketSender.SendAdminAction(new UnbanAction(TextboxName.Text));
        }

        void _setSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(new SetSpriteAction(TextboxName.Text, DropdownSprite.Text));
            }
        }

        void _setPowerButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (TextboxName.Text.Trim().Length > 0)
            {
                PacketSender.SendAdminAction(
                    new SetAccessAction(TextboxName.Text, DropdownAccess.SelectedItem.UserData.ToString())
                );
            }
        }

        void _chkChronological_CheckChanged(Base sender, EventArgs arguments)
        {
            UpdateMapList();
        }

        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendAdminAction(new WarpToMapAction((Guid) ((TreeNode) sender).UserData));
        }

        public void Update()
        {
        }

        public void Show()
        {
            mAdminWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mAdminWindow.IsHidden;
        }

        public void Hide()
        {
            mAdminWindow.IsHidden = true;
        }

    }

}
