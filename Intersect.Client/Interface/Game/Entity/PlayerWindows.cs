using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Entity
{

    public partial class PlayerWindows
    {
        private string _currentSprite = "";

        private float _currentExpSize = -1;

        private float _currentHpSize = -1;

        private float _currentMpSize = -1;

        private float _currentShieldSize = -1;

        private readonly ImagePanel _experienceBackground;

        private readonly ImagePanel _experienceBackgroundMinimized;

        private readonly ImagePanel _experienceBar;

        private readonly ImagePanel _experienceBarMinimized;

        private readonly Label _experienceValue;

        private readonly Label _experienceValueMinimized;

        private readonly Label _experienceLabel;

        private readonly Label _experienceLabelMinimized;

        private readonly ImagePanel _hpBackground;

        private readonly ImagePanel _hpBackgroundMinimized;

        private readonly ImagePanel _hpBar;

        private readonly ImagePanel _hpBarMinimized;

        private readonly Label _hpValue;

        private readonly Label _hpValueMinimized;

        private readonly Label _hpLabel;

        private readonly Label _hpLabelMinimized;

        public bool IsHidden;

        private long _lastUpdateTime;

        private readonly ImagePanel _mpBackground;

        private readonly ImagePanel _mpBackgroundMinimized;

        private readonly ImagePanel _mpBar;

        private readonly ImagePanel _mpBarMinimized;

        private readonly Label _mpValue;

        private readonly Label _mpValueMinimized;

        private readonly Label _mpLabel;

        private readonly Label _mpLabelMinimized;

        public Entities.Entity MyEntity;

        private readonly ImagePanel[] _paperdollPanel;

        private readonly string[] _paperdollTexture;

        private readonly Label _playerLevel;

        private readonly Label _playerLevelMinimized;

        private readonly Label _playerMap;

        private readonly Button _windowMinimizeButton;

        private readonly ImagePanel? _playerSprite;

        private readonly ImagePanel? _playerSpriteContainer;

        private readonly ImagePanel _window;

        private readonly Label _playerName;

        private readonly Label _playerNameAndLevel;

        public readonly ImagePanel PlayerWindow;

        private readonly ImagePanel _windowMinimized;

        private readonly ImagePanel _shieldBar;

        //Init
        public PlayerWindows(Canvas gameCanvas, Entities.Entity myEntity)
        {
            MyEntity = myEntity;
            PlayerWindow = new ImagePanel(gameCanvas, nameof(PlayerWindow))
            {
                ShouldCacheToTexture = true
            };

            // Player Information
            _window = new ImagePanel(PlayerWindow, nameof(_window));
            _playerName = new Label(_window, nameof(_playerName)) { Text = myEntity?.Name };
            _playerLevel = new Label(_window, nameof(_playerLevel));
            _playerNameAndLevel = new Label(_window, nameof(_playerNameAndLevel)) { IsHidden = true };
            _playerMap = new Label(_window, nameof(_playerMap));
            _paperdollPanel = new ImagePanel[Options.EquipmentSlots.Count];
            _paperdollTexture = new string[Options.EquipmentSlots.Count];

            // Player Sprite and Face
            var i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
            {
                if (Options.PaperdollOrder[1][z] == "Player")
                {
                    _playerSpriteContainer = new ImagePanel(_window, nameof(_playerSpriteContainer));
                    _playerSprite = new ImagePanel(_playerSpriteContainer);
                    _playerSprite.SetSize(64, 64);
                    _playerSprite.AddAlignment(Alignments.Center);
                }
                else
                {
                    if (_playerSpriteContainer != null)
                    {
                        _paperdollPanel[i] = new ImagePanel(_playerSpriteContainer);
                    }
                    _paperdollTexture[i] = "";
                    _paperdollPanel[i].Hide();
                    i++;
                }
            }

            // Player Window 
            _hpBackground = new ImagePanel(_window, nameof(_hpBackground));
            _hpBar = new ImagePanel(_window, nameof(_hpBar));
            _shieldBar = new ImagePanel(_window, nameof(_shieldBar));
            _hpLabel = new Label(_window, nameof(_hpLabel));
            _hpLabel.SetText(Strings.EntityBox.Vital0);
            _hpValue = new Label(_window, nameof(_hpValue));

            _mpBackground = new ImagePanel(_window, nameof(_mpBackground));
            _mpBar = new ImagePanel(_window, nameof(_mpBar));
            _mpLabel = new Label(_window, nameof(_mpLabel));
            _mpLabel.SetText(Strings.EntityBox.Vital1);
            _mpValue = new Label(_window, nameof(_mpValue));

            _experienceBackground = new ImagePanel(_window, nameof(_experienceBackground));
            _experienceBar = new ImagePanel(_window, nameof(_experienceBar));
            _experienceLabel = new Label(_window, nameof(_experienceLabel));
            _experienceLabel.SetText(Strings.EntityBox.Exp);
            _experienceValue = new Label(_window, nameof(_experienceValue));

            // Player Window Minimized
            _windowMinimized = new ImagePanel(PlayerWindow, nameof(_windowMinimized));

            _hpBackgroundMinimized = new ImagePanel(_windowMinimized, nameof(_hpBackgroundMinimized));
            _hpBarMinimized = new ImagePanel(_windowMinimized, nameof(_hpBarMinimized));
            _hpLabelMinimized = new Label(_windowMinimized, nameof(_hpLabelMinimized));
            _hpLabelMinimized.SetText(Strings.EntityBox.Vital0);
            _hpValueMinimized = new Label(_windowMinimized, nameof(_hpValueMinimized));

            _mpBackgroundMinimized = new ImagePanel(_windowMinimized, nameof(_mpBackgroundMinimized));
            _mpBarMinimized = new ImagePanel(_windowMinimized, nameof(_mpBarMinimized));
            _mpLabelMinimized = new Label(_windowMinimized, nameof(_mpLabelMinimized));
            _mpLabelMinimized.SetText(Strings.EntityBox.Vital1);
            _mpValueMinimized = new Label(_windowMinimized, nameof(_mpValueMinimized));

            _experienceBackgroundMinimized = new ImagePanel(_windowMinimized, nameof(_experienceBackgroundMinimized));
            _experienceBarMinimized = new ImagePanel(_windowMinimized, nameof(_experienceBarMinimized));
            _experienceLabelMinimized = new Label(_windowMinimized, nameof(_experienceLabelMinimized));
            _experienceLabelMinimized.SetText(Strings.EntityBox.Exp);
            _experienceValueMinimized = new Label(_windowMinimized, nameof(_experienceValueMinimized));

            _playerLevelMinimized = new Label(_windowMinimized, nameof(_playerLevelMinimized));
            _windowMinimized.Hide();

            // Minimize Button
            _windowMinimizeButton = new Button(PlayerWindow, nameof(_windowMinimizeButton));
            _windowMinimizeButton.Clicked += PlayerWindowMinimizeBtn_Clicked;

            SetEntity(myEntity);

            PlayerWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
            {
                if (Options.PaperdollOrder[1][z] == "Player")
                {
                    if (_playerSprite != null && _playerSpriteContainer != null)
                    {
                        _playerSprite.RenderColor = _playerSpriteContainer.RenderColor;
                    }
                }
                else
                {
                    if (_paperdollPanel[i] != null && _playerSpriteContainer != null)
                    {
                        _paperdollPanel[i].RenderColor = _playerSpriteContainer.RenderColor;
                    }
                    i++;
                }
            }

            PlayerWindow.Hide();

            _lastUpdateTime = Timing.Global.MillisecondsUtc;
        }

        public void SetEntity(Entities.Entity entity)
        {
            MyEntity = entity;
            if (MyEntity != null)
            {
                SetupEntityElements();
            }
        }

        public void ShowAllElements()
        {
            _experienceBackground.Show();
            _experienceBar.Show();
            _experienceValue.Show();
            _experienceLabel.Show();
            _playerMap.Show();
            _mpBackground.Show();
            _mpBar.Show();
            _mpLabel.Show();
            _mpValue.Show();
            _hpBackground.Show();
            _hpBar.Show();
            _hpValue.Show();
            _hpLabel.Show();
            _hpValueMinimized.Show();
            _mpValueMinimized.Show();
            _experienceValueMinimized.Show();
        }

        public void SetupEntityElements()
        {
            ShowAllElements();

            //Update Bars
            _currentHpSize = -1;
            _currentShieldSize = -1;
            _currentMpSize = -1;
            _currentExpSize = -1;
            _shieldBar.Hide();
            UpdateHpBar(0, true);
            UpdateMpBar(0, true);
            UpdateXpBar(0, true);
            _playerName.SetText(MyEntity.Name);
        }

        //Update
        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!PlayerWindow.IsHidden)
                {
                    PlayerWindow.Hide();
                }

                return;
            }
            else
            {
                if (PlayerWindow.IsHidden)
                {
                    PlayerWindow.Show();
                }
            }

            if (PlayerWindow.IsHidden)
            {
                PlayerWindow.Show();
            }

            if (MyEntity.IsDisposed())
            {
                Dispose();
            }

            //Time since this window was last updated (for bar animations)
            var elapsedTime = (Timing.Global.MillisecondsUtc - _lastUpdateTime) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            IsHidden = true;
            _playerName.SetText(MyEntity.Name);
            UpdateLevel();
            UpdateMap();
            UpdateHpBar(elapsedTime);
            UpdateMpBar(elapsedTime);
            UpdateXpBar(elapsedTime);
            IsHidden = false;

            _lastUpdateTime = Timing.Global.MillisecondsUtc;
        }

        private void UpdateLevel()
        {
            var levelString = Strings.EntityBox.Level.ToString(MyEntity.Level);
            if (!_playerLevel.IsHidden)
            {
                _playerLevel.Text = levelString;
            }

            if (!_playerLevelMinimized.IsHidden)
            {
                _playerLevelMinimized.Text = levelString;
            }

            if (!_playerNameAndLevel.IsHidden)
            {
                _playerNameAndLevel.Text = Strings.EntityBox.NameAndLevel.ToString(MyEntity.Name, levelString);
            }
        }

        private void UpdateMap()
        {
            _playerMap.SetText(Globals.Me.MapInstance != null ?
            Strings.EntityBox.Map.ToString(Globals.Me.MapInstance.Name) : Strings.EntityBox.Map.ToString(""));
        }

        private static float SetBarSize(float barRatio, int barSize)
        {
            var barFillRatio = Math.Min(1, Math.Max(0, barRatio));

            return (float)Math.Ceiling(barFillRatio * barSize);
        }

        private static float SetCurrentBarSize(float elapsedTime, bool instant, float size, float currentSize)
        {
            if (instant)
            {
                return (int)size;
            }

            if ((int)size > currentSize)
            {
                currentSize += 100f * elapsedTime;
                if (currentSize > (int)size)
                {
                    currentSize = size;
                }
            }
            else
            {
                currentSize -= 100f * elapsedTime;
                if (currentSize < size)
                {
                    currentSize = size;
                }
            }

            return currentSize;
        }

        private static void UpdateGauge(
            Base backgroundBar,
            ImagePanel foregroundBar,
            float currentBarSize,
            DisplayDirection direction,
            bool isShield = false
        )
        {
            //If this method is called to update the shield, we need to invert the directions
            if (isShield)
            {
                switch (direction)
                {
                    case DisplayDirection.StartToEnd:
                        direction = DisplayDirection.EndToStart;
                        foregroundBar.X = backgroundBar.X;
                        break;

                    case DisplayDirection.EndToStart:
                        direction = DisplayDirection.StartToEnd;
                        foregroundBar.X = backgroundBar.X;
                        break;

                    case DisplayDirection.TopToBottom:
                        direction = DisplayDirection.BottomToTop;
                        foregroundBar.X = backgroundBar.X;
                        break;

                    case DisplayDirection.BottomToTop:
                        direction = DisplayDirection.TopToBottom;
                        foregroundBar.X = backgroundBar.X;
                        break;
                }
            }

            var backgroundWidthFactor = backgroundBar.Width - (int)currentBarSize;
            var backgroundHeightFactor = backgroundBar.Height - (int)currentBarSize;

            switch (direction)
            {
                case DisplayDirection.StartToEnd:
                    foregroundBar.SetBounds(
                        foregroundBar.X,
                        foregroundBar.Y,
                        (int)currentBarSize,
                        foregroundBar.Height
                    );
                    foregroundBar.SetTextureRect(
                        0, 0, (int)currentBarSize, foregroundBar.Height
                    );
                    break;

                case DisplayDirection.EndToStart:
                    foregroundBar.SetBounds(
                        backgroundBar.X + backgroundWidthFactor,
                        foregroundBar.Y,
                        (int)currentBarSize,
                        foregroundBar.Height
                    );
                    foregroundBar.SetTextureRect(
                        backgroundWidthFactor, 0, (int)currentBarSize, foregroundBar.Height
                    );
                    break;

                case DisplayDirection.TopToBottom:
                    foregroundBar.SetBounds(
                        foregroundBar.X,
                        foregroundBar.Y,
                        foregroundBar.Width,
                        (int)currentBarSize
                    );
                    foregroundBar.SetTextureRect(
                        0, 0, foregroundBar.Width, (int)currentBarSize
                    );
                    break;

                case DisplayDirection.BottomToTop:
                    foregroundBar.SetBounds(
                        foregroundBar.X,
                        backgroundBar.Y + backgroundHeightFactor,
                        foregroundBar.Width,
                        (int)currentBarSize
                    );
                    foregroundBar.SetTextureRect(
                        0, backgroundHeightFactor, foregroundBar.Width, (int)currentBarSize
                    );
                    break;
            }

            foregroundBar.IsHidden = false;
        }

        private void UpdateHpBar(float elapsedTime, bool instant = false)
        {
            float hpSize;
            float shieldSize;
            var barDirectionSetting = ClientConfiguration.Instance.EntityBarDirections[(int)Vital.Health];
            var barPercentageSetting = Globals.Database.ShowHealthAsPercentage;
            var entityVital = MyEntity.Vital[(int)Vital.Health];
            var entityMaxVital = MyEntity.MaxVital[(int)Vital.Health];

            if (entityVital > 0)
            {

                var currentShieldSize = MyEntity.GetShieldSize();
                var vitalSize = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _hpBackground.Width
                    : _hpBackground.Height;
                var vitalSizeMinimized = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _hpBackgroundMinimized.Width
                    : _hpBackgroundMinimized.Height;

                //We have to get the maxVital value before being changed by the shield
                //Shield changes vitalMax only on client, showing incorrect values
                if (currentShieldSize + entityVital > entityMaxVital)
                {
                    entityMaxVital = currentShieldSize + entityVital;
                }

                var entityVitalRatio = (float)entityVital / entityMaxVital;
                var entityShieldRatio = (float)currentShieldSize / entityMaxVital;
                var hpPercentage = entityVitalRatio * 100;
                var hpPercentageText = $"{hpPercentage:0.##}%";
                var hpValueText = Strings.EntityBox.Vital0Value.ToString(entityVital, entityMaxVital);
                _hpValue.Text = barPercentageSetting ? hpPercentageText : hpValueText;
                _hpValueMinimized.Text = $"{(int)hpPercentage}%";
                _hpBackground.SetToolTipText(barPercentageSetting ? hpValueText : hpPercentageText);
                hpSize = SetBarSize(entityVitalRatio, !_playerLevelMinimized.IsVisible ? vitalSize : vitalSizeMinimized);
                shieldSize = SetBarSize(entityShieldRatio, vitalSize);
            }
            else
            {
                _hpValue.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital);
                _hpValueMinimized.Text = "0%";
                _hpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital) : "0%");
                hpSize = 0;
                shieldSize = 0;
            }

            if ((int)hpSize != (int)_currentHpSize)
            {
                _currentHpSize = SetCurrentBarSize(elapsedTime, instant, hpSize, _currentHpSize);

                if (_currentHpSize == 0)
                {
                    _hpBar.IsHidden = true;
                    _hpBarMinimized.IsHidden = true;
                }
                else
                {
                    UpdateGauge(!_playerLevelMinimized.IsVisible ? _hpBackground : _hpBackgroundMinimized,
                    !_playerLevelMinimized.IsVisible ? _hpBar : _hpBarMinimized,
                    _currentHpSize, barDirectionSetting);
                }
            }

            if ((int)shieldSize != (int)_currentShieldSize)
            {
                _currentShieldSize = SetCurrentBarSize(elapsedTime, instant, shieldSize, _currentShieldSize);

                if (_currentShieldSize == 0)
                {
                    _shieldBar.IsHidden = true;
                }
                else
                {
                    UpdateGauge(_hpBackground, _shieldBar, _currentShieldSize, barDirectionSetting, true);
                }
            }
        }

        private void UpdateMpBar(float elapsedTime, bool instant = false)
        {
            float mpSize;
            var barDirectionSetting = ClientConfiguration.Instance.EntityBarDirections[(int)Vital.Mana];
            var barPercentageSetting = Globals.Database.ShowManaAsPercentage;
            var entityVital = MyEntity.Vital[(int)Vital.Mana];
            var entityMaxVital = MyEntity.MaxVital[(int)Vital.Mana];

            if (entityVital > 0)
            {

                var entityVitalRatio = (float)entityVital / entityMaxVital;
                var vitalSize = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _mpBackground.Width
                    : _mpBackground.Height;
                var vitalSizeMinimized = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _mpBackgroundMinimized.Width
                    : _mpBackgroundMinimized.Height;
                float mpPercentage = entityVitalRatio * 100;
                var mpPercentageText = $"{mpPercentage:0.##}%";
                var mpValueText = Strings.EntityBox.Vital1Value.ToString(entityVital, entityMaxVital);
                _mpValue.Text = barPercentageSetting ? mpPercentageText : mpValueText;
                _mpValueMinimized.Text = $"{(int)mpPercentage}%";
                _mpBackground.SetToolTipText(barPercentageSetting ? mpValueText : mpPercentageText);
                mpSize = SetBarSize(entityVitalRatio, !_playerLevelMinimized.IsVisible ? vitalSize : vitalSizeMinimized);
            }
            else
            {
                _mpValue.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital);
                _mpValueMinimized.Text = "0%";
                _mpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital) : "0%");
                mpSize = 0;
            }

            if ((int)mpSize != (int)_currentMpSize)
            {
                _currentMpSize = SetCurrentBarSize(elapsedTime, instant, mpSize, _currentMpSize);

                if (_currentMpSize == 0)
                {
                    _mpBar.IsHidden = true;
                    _mpBarMinimized.IsHidden = true;
                }
                else
                {
                    UpdateGauge(!_playerLevelMinimized.IsVisible ? _mpBackground : _mpBackgroundMinimized,
                    !_playerLevelMinimized.IsVisible ? _mpBar : _mpBarMinimized,
                    _currentMpSize, barDirectionSetting);
                }
            }
        }

        private void UpdateXpBar(float elapsedTime, bool instant = false)
        {
            float expSize;
            var barDirectionSetting = ClientConfiguration.Instance.EntityBarDirections[Enum.GetValues<Vital>().Length];
            var barPercentageSetting = Globals.Database.ShowExperienceAsPercentage;
            var entityExperienceToNextLevel = ((Entities.Player)MyEntity).GetNextLevelExperience();

            if (entityExperienceToNextLevel > 0)
            {
                var entityExperience = ((Entities.Player)MyEntity).Experience;
                var entityExperienceRatio = (float)entityExperience / entityExperienceToNextLevel;
                var vitalSize = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _experienceBackground.Width
                    : _experienceBackground.Height;
                var vitalSizeMinimized = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _experienceBackgroundMinimized.Width
                    : _experienceBackgroundMinimized.Height;
                
                var expPercentage = entityExperienceRatio * 100;
                var expPercentageText = $"{expPercentage:0.##}%";
                var expValueText = Strings.EntityBox.ExpValue.ToString(entityExperience, entityExperienceToNextLevel);
                _experienceValue.Text = barPercentageSetting ? expPercentageText : expValueText;
                _experienceValueMinimized.Text = $"{(int)expPercentage}%";
                _experienceBackground.SetToolTipText(barPercentageSetting ? expValueText : expPercentageText);
                expSize = SetBarSize(entityExperienceRatio, !_playerLevelMinimized.IsVisible ? vitalSize : vitalSizeMinimized);
            }
            else
            {
                expSize = 1f;
                _experienceValue.Text = Strings.EntityBox.MaxLevel;
                _experienceBackground.SetToolTipText(Strings.EntityBox.MaxLevel);
            }

            if (Math.Abs((int)expSize - _currentExpSize) < 0.01)
            {
                return;
            }

            _currentExpSize = SetCurrentBarSize(elapsedTime, instant, expSize, _currentExpSize);

            if (_currentExpSize == 0)
            {
                _experienceBar.IsHidden = true;
                _experienceBarMinimized.IsHidden = true;
            }
            else
            {
                UpdateGauge(!_playerLevelMinimized.IsVisible ? _experienceBackground : _experienceBackgroundMinimized,
                    !_playerLevelMinimized.IsVisible ? _experienceBar : _experienceBarMinimized,
                     _currentExpSize, barDirectionSetting);
            }
        }

        private void UpdateImage()
        {
            var faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, MyEntity.Face);
            var entityTex = MyEntity.Texture;
            if (faceTex != null && faceTex != _playerSprite.Texture)
            {
                _playerSprite.Texture = faceTex;
                _playerSprite.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                _playerSprite.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                _playerSprite.SizeToContents();
                Align.Center(_playerSprite);
                _currentSprite = MyEntity.Face;
                _playerSprite.IsHidden = false;
                var i = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    if (Options.PaperdollOrder[1][z] != "Player")
                    {
                        if (_paperdollPanel == null)
                        {
                            Log.Warn($@"{nameof(_paperdollPanel)} is null.");
                        }
                        else if (_paperdollPanel[i] == null)
                        {
                            Log.Warn($@"{nameof(_paperdollPanel)}[{i}] is null.");
                        }

                        _paperdollPanel?[i]?.Hide();
                        i++;
                    }
                }
            }
            else if (entityTex != null && faceTex == null || faceTex != null && faceTex != _playerSprite.Texture)
            {
                if (entityTex != _playerSprite.Texture)
                {
                    _playerSprite.Texture = entityTex;
                    _playerSprite.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                    _playerSprite.SetTextureRect(0, 0, entityTex.GetWidth() / Options.Instance.Sprites.NormalFrames, entityTex.GetHeight() / Options.Instance.Sprites.Directions);
                    _playerSprite.SizeToContents();
                    Align.Center(_playerSprite);
                    _currentSprite = MyEntity.Sprite;
                    _playerSprite.IsHidden = false;
                }

                var equipment = MyEntity.Equipment;

                for (var i = 0; i < MyEntity.MyEquipment.Length; i++)
                {
                    var eqp = MyEntity.MyEquipment[i];
                    if (eqp > -1 && eqp < Options.MaxInvItems)
                    {
                        equipment[i] = MyEntity.Inventory[eqp].ItemId;
                    }
                    else
                    {
                        equipment[i] = Guid.Empty;
                    }
                }

                var n = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    var paperdollPanel = _paperdollPanel[n];
                    var paperdoll = string.Empty;
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1 &&
                        equipment.Length == Options.EquipmentSlots.Count)
                    {
                        if (equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] != Guid.Empty)
                        {
                            var itemId = equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])];
                            if (ItemBase.TryGet(itemId, out var itemDescriptor))
                            {
                                paperdoll = MyEntity.Gender == 0
                                    ? itemDescriptor.MalePaperdoll : itemDescriptor.FemalePaperdoll;
                                paperdollPanel.RenderColor = itemDescriptor.Color;
                            }
                        }
                    }

                    //Check for Player layer
                    if (Options.PaperdollOrder[1][z] == "Player")
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(_paperdollTexture[n]))
                    {
                        paperdollPanel.Texture = null;
                        paperdollPanel.Hide();
                        _paperdollTexture[n] = string.Empty;
                    }
                    else if (!string.IsNullOrWhiteSpace(paperdoll) && paperdoll != _paperdollTexture[n])
                    {
                        var paperdollTex = Globals.ContentManager.GetTexture(
                            Framework.Content.TextureType.Paperdoll, paperdoll
                        );

                        paperdollPanel.Texture = paperdollTex;
                        if (paperdollTex != null)
                        {
                            paperdollPanel
                                .SetTextureRect(
                                    0, 0, paperdollPanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    paperdollPanel.Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            paperdollPanel
                                .SetSize(
                                    paperdollPanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    paperdollPanel.Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            paperdollPanel
                                .SetPosition(
                                    (_playerSpriteContainer.Width - paperdollPanel.Width) / 2,
                                    (_playerSpriteContainer.Height - paperdollPanel.Height) / 2
                                );
                        }

                        paperdollPanel.Show();
                        _paperdollTexture[n] = paperdoll;
                    }

                    //Check for Player layer
                    if (Options.PaperdollOrder[1][z] != "Player")
                    {
                        n++;
                    }
                }
            }
            else if (MyEntity.Sprite != _currentSprite && MyEntity.Face != _currentSprite)
            {
                _playerSprite.IsHidden = true;
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPanel[i]?.Hide();
                }
            }

            if (_playerSprite.RenderColor != MyEntity.Color)
            {
                _playerSprite.RenderColor = MyEntity.Color;
            }
        }

        void PlayerWindowMinimizeBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (!_window.IsHidden)
            {
                HidePlayerInformation();
            }
            else
            {
                ShowPlayerInformation();
            }
        }

        private void ShowPlayerInformation()
        {
            _windowMinimized.Hide();
            _window.Show();
            UpdateHpBar(0, true);
            UpdateMpBar(0, true);
            UpdateXpBar(0, true);
        }

        private void HidePlayerInformation()
        {
            _window.Hide();
            _windowMinimized.Show();
            UpdateHpBar(0, true);
            UpdateMpBar(0, true);
            UpdateXpBar(0, true);
        }

        public void Dispose()
        {
            PlayerWindow.Hide();
            Interface.GameUi.GameCanvas.RemoveChild(PlayerWindow, false);
            PlayerWindow.Dispose();
        }

        public void Hide()
        {
            PlayerWindow.Hide();
        }

        public void Show()
        {
            PlayerWindow.Show();
        }

    }

}
