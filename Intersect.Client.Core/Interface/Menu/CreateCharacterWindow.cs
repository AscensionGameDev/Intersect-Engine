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
using Intersect.Core;
using Intersect.GameObjects;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Menu;

public partial class CreateCharacterWindow : ImagePanel
{
    // Parent
    private readonly MainMenu _mainMenu;
    private readonly SelectCharacterWindow _selectCharWindow;

    // Header
    private readonly TextBox _charNameTextbox;

    // Class Combobox
    private readonly ComboBox _classCombobox;

    // Character Image Container
    private readonly ImagePanel _charContainer;
    private readonly ImagePanel _charPortrait;
    private readonly Button _nextSpriteButton;
    private readonly Button _prevSpriteButton;

    // Gender Combobox
    private readonly LabeledCheckBox _chkMale;
    private readonly LabeledCheckBox _chkFemale;

    // Buttons
    private readonly Button _createButton;

    private int _displaySpriteIndex = -1;
    private readonly List<KeyValuePair<int, ClassSprite>> _femaleSprites = [];
    private readonly List<KeyValuePair<int, ClassSprite>> _maleSprites = [];

    public CreateCharacterWindow(Canvas parent, MainMenu mainMenu, SelectCharacterWindow selectCharacterWindow) : base(parent, "CharacterCreationWindow")
    {
        // Assign References
        _mainMenu = mainMenu;
        _selectCharWindow = selectCharacterWindow;

        // Menu Header
        _ = new Label(this, "CharacterCreationHeader")
        {
            Text = Strings.CharacterCreation.Title
        };

        // Character Name Background
        var charNameBackground = new ImagePanel(this, "CharacterNamePanel");

        // Character Name Label
        _ = new Label(charNameBackground, "CharacterNameLabel")
        {
            Text = Strings.CharacterCreation.Name
        };

        // Character Name Textbox
        _charNameTextbox = new TextBox(charNameBackground, "CharacterNameField");
        _charNameTextbox.SubmitPressed += (sender, e) => TryCreateCharacter();

        // Class Background
        var classBackground = new ImagePanel(this, "ClassPanel");

        // Class Label
        _ = new Label(classBackground, "ClassLabel")
        {
            Text = Strings.CharacterCreation.Class
        };

        // Class Combobox
        _classCombobox = new ComboBox(classBackground, "ClassCombobox");
        _classCombobox.ItemSelected += classCombobox_ItemSelected;

        // Hint Label
        _ = new Label(this, "HintLabel")
        {
            Text = Strings.CharacterCreation.Hint,
            IsHidden = true
        };

        // Hint2 Label
        _ = new Label(this, "Hint2Label")
        {
            Text = Strings.CharacterCreation.Hint2,
            IsHidden = true
        };

        // Character Container
        _charContainer = new ImagePanel(this, "CharacterContainer");

        // Character sprite
        _charPortrait = new ImagePanel(_charContainer, "CharacterPortait");
        _ = _charPortrait.SetSize(48, 48);

        // Next Sprite Button
        _nextSpriteButton = new Button(_charContainer, "NextSpriteButton");
        _nextSpriteButton.Clicked += _nextSpriteButton_Clicked;

        // Prev Sprite Button
        _prevSpriteButton = new Button(_charContainer, "PreviousSpriteButton");
        _prevSpriteButton.Clicked += _prevSpriteButton_Clicked;

        // Class Background
        var genderBackground = new ImagePanel(this, "GenderPanel");

        // Gender Label
        _ = new Label(genderBackground, "GenderLabel")
        {
            Text = Strings.CharacterCreation.Gender
        };

        // Male Checkbox
        _chkMale = new LabeledCheckBox(genderBackground, "MaleCheckbox")
        {
            Text = Strings.CharacterCreation.Male,
            IsChecked = true
        };
        _chkMale.Checked += maleChk_Checked;
        _chkMale.Unchecked += femaleChk_Checked;

        // Female Checkbox
        _chkFemale = new LabeledCheckBox(genderBackground, "FemaleCheckbox")
        {
            Text = Strings.CharacterCreation.Female
        };

        _chkFemale.Checked += femaleChk_Checked;
        _chkFemale.Unchecked += maleChk_Checked;

        // Register - Send Registration Button
        _createButton = new Button(this, "CreateButton")
        {
            Text = Strings.CharacterCreation.Create
        };
        _createButton.Clicked += CreateButton_Clicked;

        var backButton = new Button(this, "BackButton")
        {
            Text = Strings.CharacterCreation.Back,
            IsHidden = true
        };
        backButton.Clicked += BackButton_Clicked;

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    public void Init()
    {
        _classCombobox.DeleteAll();
        var classCount = 0;
        foreach (ClassBase cls in ClassBase.Lookup.Values.Cast<ClassBase>())
        {
            if (cls.Locked)
            {
                continue;
            }

            _ = _classCombobox.AddItem(cls.Name);
            classCount++;
        }

        ApplicationContext.Context.Value?.Logger.LogDebug($"Added {classCount} classes to {nameof(CreateCharacterWindow)}");

        LoadClass();
        UpdateDisplay();
    }

    public void Update()
    {
        if (!Networking.Network.IsConnected)
        {
            Hide();
            _mainMenu.Show();
            return;
        }

        // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
        if (!Globals.WaitingOnServer && _createButton.IsDisabled)
        {
            _createButton.Enable();
        }
    }

    //Methods
    private void UpdateDisplay()
    {
        bool isFace;
        var cls = GetClass();

        if (cls == default || _displaySpriteIndex == -1)
        {
            _charPortrait.IsHidden = true;
            return;
        }

        _charPortrait.IsHidden = false;

        if (cls.Sprites.Count <= 0)
        {
            return;
        }

        var source = _chkMale.IsChecked ? _maleSprites[_displaySpriteIndex] : _femaleSprites[_displaySpriteIndex];
        var faceTex = Globals.ContentManager.GetTexture(TextureType.Face, source.Value.Face);
        var entityTex = Globals.ContentManager.GetTexture(TextureType.Entity, source.Value.Sprite);

        isFace = faceTex != null;
        _charPortrait.Texture = isFace ? faceTex : entityTex;

        if (_charPortrait.Texture == null)
        {
            return;
        }

        var imgWidth = _charPortrait.Texture.Width;
        var imgHeight = _charPortrait.Texture.Height;
        var textureWidth = isFace ? imgWidth : imgWidth / Options.Instance.Sprites.NormalFrames;
        var textureHeight = isFace ? imgHeight : imgHeight / Options.Instance.Sprites.Directions;

        _charPortrait.SetTextureRect(0, 0, textureWidth, textureHeight);

        var scale = Math.Min(_charContainer.InnerWidth / (double)imgWidth, _charContainer.InnerHeight / (double)imgHeight);
        var sizeX = isFace ? (int)(imgWidth * scale) : textureWidth;
        var sizeY = isFace ? (int)(imgHeight * scale) : textureHeight;
        _ = _charPortrait.SetSize(sizeX, sizeY);

        var centerX = (_charContainer.Width / 2) - (_charPortrait.Width / 2);
        var centerY = (_charContainer.Height / 2) - (_charPortrait.Height / 2);
        _charPortrait.SetPosition(centerX, centerY);
    }

    private ClassBase? GetClass()
    {
        if (_classCombobox.SelectedItem == null)
        {
            return null;
        }

        return ClassBase.Lookup.Values.OfType<ClassBase>().FirstOrDefault(
            descriptor =>
                !descriptor.Locked &&
                string.Equals(_classCombobox.SelectedItem.Text, descriptor.Name, StringComparison.Ordinal)
        );
    }

    private void LoadClass()
    {
        var cls = GetClass();
        _maleSprites.Clear();
        _femaleSprites.Clear();
        _displaySpriteIndex = -1;
        if (cls != null)
        {
            for (var i = 0; i < cls.Sprites.Count; i++)
            {
                if (cls.Sprites[i].Gender == 0)
                {
                    _maleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                }
                else
                {
                    _femaleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                }
            }
        }

        ResetSprite();
    }

    private void ResetSprite()
    {
        _nextSpriteButton.IsHidden = true;
        _prevSpriteButton.IsHidden = true;

        if (_chkMale.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                _displaySpriteIndex = 0;
                if (_maleSprites.Count > 1)
                {
                    _nextSpriteButton.IsHidden = false;
                    _prevSpriteButton.IsHidden = false;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                _displaySpriteIndex = 0;
                if (_femaleSprites.Count > 1)
                {
                    _nextSpriteButton.IsHidden = false;
                    _prevSpriteButton.IsHidden = false;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
    }

    private void _prevSpriteButton_Clicked(Base sender, MouseButtonState arguments)
    {
        _displaySpriteIndex--;
        if (_chkMale.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                if (_displaySpriteIndex == -1)
                {
                    _displaySpriteIndex = _maleSprites.Count - 1;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                if (_displaySpriteIndex == -1)
                {
                    _displaySpriteIndex = _femaleSprites.Count - 1;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }

        UpdateDisplay();
    }

    private void _nextSpriteButton_Clicked(Base sender, MouseButtonState arguments)
    {
        _displaySpriteIndex++;
        if (_chkMale.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                if (_displaySpriteIndex >= _maleSprites.Count)
                {
                    _displaySpriteIndex = 0;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                if (_displaySpriteIndex >= _femaleSprites.Count)
                {
                    _displaySpriteIndex = 0;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }

        UpdateDisplay();
    }

    void TryCreateCharacter()
    {
        var cls = GetClass();
        if (Globals.WaitingOnServer || _displaySpriteIndex == -1 || cls == default)
        {
            return;
        }

        if (!FieldChecking.IsValidUsername(_charNameTextbox.Text, Strings.Regex.Username))
        {
            Interface.ShowAlert(Strings.CharacterCreation.InvalidName, alertType: AlertType.Error);
            return;
        }

        var charName = _charNameTextbox.Text;
        var spriteKey = _chkMale.IsChecked ? _maleSprites[_displaySpriteIndex].Key : _femaleSprites[_displaySpriteIndex].Key;

        PacketSender.SendCreateCharacter(charName, cls.Id, spriteKey);
        Globals.WaitingOnServer = true;
        _createButton.Disable();
        ChatboxMsg.ClearMessages();
    }

    //Input Handlers
    void classCombobox_ItemSelected(Base control, ItemSelectedEventArgs args)
    {
        LoadClass();
        UpdateDisplay();
    }

    void maleChk_Checked(Base sender, EventArgs arguments)
    {
        _chkMale.IsChecked = true;
        _chkFemale.IsChecked = false;
        ResetSprite();
        UpdateDisplay();
    }

    void femaleChk_Checked(Base sender, EventArgs arguments)
    {
        _chkFemale.IsChecked = true;
        _chkMale.IsChecked = false;
        ResetSprite();
        UpdateDisplay();
    }

    void CreateButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        TryCreateCharacter();
    }

    private void BackButton_Clicked(Base sender, MouseButtonState arguments)
    {
        Hide();
        if (Options.Instance.Player.MaxCharacters <= 1)
        {
            //Logout
            _mainMenu.Show();
        }
        else
        {
            //Character Selection Screen
            _selectCharWindow.Show();
        }
    }
}
