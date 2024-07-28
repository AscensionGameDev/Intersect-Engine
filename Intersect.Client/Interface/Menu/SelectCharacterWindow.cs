using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Menu;

public partial class SelectCharacterWindow : ImagePanel
{
    private readonly MainMenu _mainMenu;

    private readonly Label _lblCharname;
    private readonly Label _lblInfo;
    private readonly ImagePanel _charContainer;
    private readonly Button _btnNextChar;
    private readonly Button _btnPrevChar;
    private readonly Button _btnPlay;
    private readonly Button _btnDelete;
    private readonly Button _btnNew;
    private readonly Button _btnLogout;

    private ImagePanel[]? _renderLayers;

    public Character[]? Characters;

    public int mSelectedChar = 0;

    //Init
    public SelectCharacterWindow(Canvas parent, MainMenu mainMenu) : base(parent, "CharacterSelectionWindow")
    {
        //Assign References
        _mainMenu = mainMenu;

        //Menu Header
        _ = new Label(this, "CharacterSelectionHeader") { Text = Strings.CharacterSelection.Title };

        //Character Name
        _lblCharname = new Label(this, "CharacterNameLabel") { Text = Strings.CharacterSelection.Empty };

        //Character Info
        _lblInfo = new Label(this, "CharacterInfoLabel") { Text = Strings.CharacterSelection.New };

        //Character Container
        _charContainer = new ImagePanel(this, "CharacterContainer");

        //Next char Button
        _btnNextChar = new Button(_charContainer, "NextCharacterButton");
        _btnNextChar.Clicked += _btnNextChar_Clicked;

        //Prev Char Button
        _btnPrevChar = new Button(_charContainer, "PreviousCharacterButton");
        _btnPrevChar.Clicked += _btnPrevChar_Clicked;

        //Play Button
        _btnPlay = new Button(this, "PlayButton")
        {
            Text = Strings.CharacterSelection.Play,
            IsHidden = true
        };
        _btnPlay.Clicked += PlayButton_Clicked;

        //Delete Button
        _btnDelete = new Button(this, "DeleteButton")
        {
            Text = Strings.CharacterSelection.Delete,
            IsHidden = true
        };
        _btnDelete.Clicked += _btnDelete_Clicked;

        //Create new char Button
        _btnNew = new Button(this, "NewButton") { Text = Strings.CharacterSelection.New };
        _btnNew.Clicked += _btnNew_Clicked;

        //Logout Button
        _btnLogout = new Button(this, "LogoutButton")
        {
            Text = Strings.CharacterSelection.Logout,
            IsHidden = true
        };
        _btnLogout.Clicked += _btnLogout_Clicked;

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    //Methods
    public void Update()
    {
        if (!Networking.Network.IsConnected)
        {
            Hide();
            _mainMenu.Show();
        }

        // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
        _btnPlay.IsDisabled = Globals.WaitingOnServer;
        _btnNew.IsDisabled = Globals.WaitingOnServer;
        _btnDelete.IsDisabled = Globals.WaitingOnServer;
        _btnLogout.IsDisabled = Globals.WaitingOnServer;
    }

    private void UpdateDisplay()
    {
        if (_renderLayers == default || Characters == default)
        {
            return;
        }

        _btnNextChar.IsHidden = Characters.Length <= 1;
        _btnPrevChar.IsHidden = Characters.Length <= 1;

        if (Characters.Length > 1)
        {
            _btnNextChar.BringToFront();
            _btnPrevChar.BringToFront();
        }

        foreach (var paperdollPortrait in _renderLayers)
        {
            paperdollPortrait.Texture = default;
            paperdollPortrait.Hide();
        }

        if (Characters[mSelectedChar] == default)
        {
            _btnPlay.Hide();
            _btnDelete.Hide();
            _btnNew.Show();

            _lblCharname.SetText(Strings.CharacterSelection.Empty);
            _lblInfo.SetText(string.Empty);
            return;
        }

        _lblCharname.SetText(Strings.CharacterSelection.Name.ToString(Characters[mSelectedChar].Name));
        _lblInfo.SetText(
            Strings.CharacterSelection.Info.ToString(
                Characters[mSelectedChar].Level, Characters[mSelectedChar].Class
            )
        );

        _btnPlay.Show();
        _btnDelete.Show();
        _btnNew.Hide();

        // we are rendering the player facing down, then we need to know the render order of the equipments
        for (var i = 0; i < Options.Equipment.Paperdoll.Down.Count; i++)
        {
            var equipment = Options.Equipment.Paperdoll.Down[i];
            var paperdollContainer = _renderLayers[i];
            var isFace = false;

            // handle player/equip rendering, we just need to find the correct texture
            if (string.Equals("Player", equipment, StringComparison.Ordinal))
            {
                var faceSource = Characters[mSelectedChar].Face;
                var spriteSource = Characters[mSelectedChar].Sprite;

                var faceTex = Globals.ContentManager.GetTexture(TextureType.Face, faceSource);
                var spriteTex = Globals.ContentManager.GetTexture(TextureType.Entity, spriteSource);

                isFace = faceTex != default;
                paperdollContainer.Texture = isFace ? faceTex : spriteTex;
            }
            else
            {
                if (i >= Characters[mSelectedChar].Equipment.Length)
                {
                    continue;
                }

                var equipFragment = Characters[mSelectedChar].Equipment[i];

                if (equipFragment == default)
                {
                    paperdollContainer.Texture = default;
                    continue;
                }

                paperdollContainer.Texture = Globals.ContentManager.GetTexture(TextureType.Paperdoll, equipFragment.Name);

                if (paperdollContainer.Texture != default)
                {
                    paperdollContainer.RenderColor = equipFragment.RenderColor;
                }
            }

            var layerTex = paperdollContainer.Texture;
            if (layerTex == default)
            {
                paperdollContainer.Hide();
                continue;
            }

            var imgWidth = layerTex.GetWidth();
            var imgHeight = layerTex.GetHeight();
            var textureWidth = isFace ? imgWidth : imgWidth / Options.Instance.Sprites.NormalFrames;
            var textureHeight = isFace ? imgHeight : imgHeight / Options.Instance.Sprites.Directions;

            paperdollContainer.SetTextureRect(0, 0, textureWidth, textureHeight);

            var scale = Math.Min(_charContainer.InnerWidth / (double)imgWidth, _charContainer.InnerHeight / (double)imgHeight);
            var sizeX = isFace ? (int)(imgWidth * scale) : textureWidth;
            var sizeY = isFace ? (int)(imgHeight * scale) : textureHeight;
            _ = paperdollContainer.SetSize(sizeX, sizeY);

            var centerX = (_charContainer.Width / 2) - (paperdollContainer.Width / 2);
            var centerY = (_charContainer.Height / 2) - (paperdollContainer.Height / 2);
            paperdollContainer.SetPosition(centerX, centerY);

            paperdollContainer.Show();
        }
    }

    public override void Show()
    {
        if (_renderLayers == default)
        {
            _renderLayers = new ImagePanel[Options.Equipment.Paperdoll.Down.Count];
            for (var i = 0; i < _renderLayers.Length; i++)
            {
                _renderLayers[i] = new ImagePanel(_charContainer);
            }
        }

        if (Characters == default)
        {
            Characters = new Character[Options.MaxCharacters];
        }

        mSelectedChar = 0;
        UpdateDisplay();
        base.Show();
    }

    private void _btnLogout_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Main.Logout(false, skipFade: true);
        _mainMenu.Reset();
    }

    private void _btnPrevChar_Clicked(Base sender, ClickedEventArgs arguments)
    {
        mSelectedChar--;
        if (mSelectedChar < 0)
        {
            mSelectedChar = Characters.Length - 1;
        }

        UpdateDisplay();
    }

    private void _btnNextChar_Clicked(Base sender, ClickedEventArgs arguments)
    {
        mSelectedChar++;
        if (mSelectedChar >= Characters.Length)
        {
            mSelectedChar = 0;
        }

        UpdateDisplay();
    }

    private void _btnDelete_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        _ = new InputBox(
            title: Strings.CharacterSelection.DeleteTitle.ToString(Characters[mSelectedChar].Name),
            prompt: Strings.CharacterSelection.DeletePrompt.ToString(Characters[mSelectedChar].Name),
            inputType: InputBox.InputType.YesNo,
            userData: Characters[mSelectedChar].Id,
            onSuccess: DeleteCharacter
        );
    }

    private void DeleteCharacter(object? sender, EventArgs e)
    {
        if (sender is InputBox inputBox && inputBox.UserData is Guid charId)
        {
            PacketSender.SendDeleteCharacter(charId);

            Globals.WaitingOnServer = true;
            _btnPlay.Disable();
            _btnNew.Disable();
            _btnDelete.Disable();
            _btnLogout.Disable();

            mSelectedChar = 0;
            UpdateDisplay();
        }
    }

    private void _btnNew_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        PacketSender.SendNewCharacter();

        Globals.WaitingOnServer = true;
        _btnPlay.Disable();
        _btnNew.Disable();
        _btnDelete.Disable();
        _btnLogout.Disable();
    }

    public void PlayButton_Clicked(Base? sender, ClickedEventArgs? arguments)
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        ChatboxMsg.ClearMessages();
        PacketSender.SendSelectCharacter(Characters[mSelectedChar].Id);

        Globals.WaitingOnServer = true;
        _btnPlay.Disable();
        _btnNew.Disable();
        _btnDelete.Disable();
        _btnLogout.Disable();
    }
}

public partial class Character(
    Guid id,
    string name,
    string sprite,
    string face,
    int level,
    string charClass,
    EquipmentFragment[] equipment
    )
{
    public Guid Id = id;

    public string Class = charClass;

    public EquipmentFragment?[] Equipment = equipment;

    public bool Exists = true;

    public string Face = face;

    public int Level = level;

    public string Name = name;

    public string Sprite = sprite;
}
