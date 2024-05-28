using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Entity
{

    public partial class TargetWindows
    {
        private readonly Dictionary<Guid, SpellStatus> _activeStatuses = new Dictionary<Guid, SpellStatus>();

        private readonly ImagePanel _entitySprite;

        private readonly ImagePanel _entitySpriteContainer;

        private readonly Label _entityLevel;

        private readonly Label _entityName;

        private readonly Label _entityNameAndLevel;

        private EntityType _entityType;

        private readonly RichLabel _eventDesc;

        private readonly Button _friendButton;

        private readonly Button _guildButton;

        private readonly ImagePanel _hpBackground;

        private readonly ImagePanel _hpBar;

        private readonly Label _hpValue;

        private readonly Label _hpLabel;

        public bool IsHidden;

        private string _currentSprite = "";

        private float _currentHpSize = -1;

        public float _currentShieldSize = -1;

        private float _currentMpSize = -1;

        private readonly ImagePanel _mpBackground;

        private readonly ImagePanel _mpBar;

        private readonly Label _mpValue;

        private readonly Label _mpLabel;

        private long _lastUpdateTime;

        public Entities.Entity MyEntity;

        private readonly ImagePanel[] _paperdollPanels;

        private readonly string[] _paperdollTextures;

        private readonly Button _partyButton;

        private readonly ImagePanel _shieldBar;

        private readonly Button _tradeButton;

        public bool UpdateStatuses;

        public readonly ImagePanel _targetStatus;

        public readonly ImagePanel TargetWindow;

        //Init
        public TargetWindows(Canvas gameCanvas, EntityType entityType, Entities.Entity myEntity)
        {
            MyEntity = myEntity;
            _entityType = entityType;

            TargetWindow = new ImagePanel(gameCanvas, nameof(TargetWindow))
            {
                ShouldCacheToTexture = true
            };

            // Target Information
            _entityName = new Label(TargetWindow, nameof(_entityName)) { Text = myEntity?.Name };
            _entityLevel = new Label(TargetWindow, nameof(_entityLevel));
            _entityNameAndLevel = new Label(TargetWindow, nameof(_entityNameAndLevel)) { IsHidden = true };
            _paperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            _paperdollTextures = new string[Options.EquipmentSlots.Count];

            // Target Sprite and Face
            var i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
            {
                if (Options.PaperdollOrder[1][z] == "Player")
                {
                    _entitySpriteContainer = new ImagePanel(TargetWindow, nameof(_entitySpriteContainer));
                    _entitySprite = new ImagePanel(_entitySpriteContainer);
                    _entitySprite.SetSize(64, 64);
                    _entitySprite.AddAlignment(Alignments.Center);
                }
                else
                {
                    _paperdollPanels[i] = new ImagePanel(_entitySpriteContainer);
                    _paperdollTextures[i] = "";
                    _paperdollPanels[i].Hide();
                    i++;
                }
            }

            // Target Window
            _eventDesc = new RichLabel(TargetWindow, nameof(_eventDesc));
            _hpBackground = new ImagePanel(TargetWindow, nameof(_hpBackground));
            _hpBar = new ImagePanel(TargetWindow, nameof(_hpBar));
            _shieldBar = new ImagePanel(TargetWindow, nameof(_shieldBar));
            _hpLabel = new Label(TargetWindow, nameof(_hpLabel));
            _hpLabel.SetText(Strings.EntityBox.Vital0);
            _hpValue = new Label(TargetWindow, nameof(_hpValue));

            _mpBackground = new ImagePanel(TargetWindow, nameof(_mpBackground));
            _mpBar = new ImagePanel(TargetWindow, nameof(_mpBar));
            _mpLabel = new Label(TargetWindow, nameof(_mpLabel));
            _mpLabel.SetText(Strings.EntityBox.Vital1);
            _mpValue = new Label(TargetWindow, nameof(_mpValue));

            _tradeButton = new Button(TargetWindow, nameof(_tradeButton));
            _tradeButton.SetText(Strings.EntityBox.Trade);
            _tradeButton.Clicked += tradeRequest_Clicked;
            _tradeButton.HoverEnter += (sender, e) => _tradeButton.SetToolTipText(Strings.EntityBox.TradeTip.ToString(MyEntity?.Name));

            _partyButton = new Button(TargetWindow, nameof(_partyButton));
            _partyButton.SetText(Strings.EntityBox.Party);
            _partyButton.Clicked += invite_Clicked;
            _partyButton.HoverEnter += (sender, e) => _partyButton.SetToolTipText(Strings.EntityBox.PartyTip.ToString(MyEntity?.Name));

            _friendButton = new Button(TargetWindow, nameof(_friendButton));
            _friendButton.SetText(Strings.EntityBox.Friend);
            _friendButton.Clicked += friendRequest_Clicked;
            _friendButton.HoverEnter += (sender, e) => _friendButton.SetToolTipText(Strings.EntityBox.FriendTip.ToString(MyEntity?.Name));
            _friendButton.IsHidden = true;

            _guildButton = new Button(TargetWindow, nameof(_guildButton));
            _guildButton.SetText(Strings.Guilds.Guild);
            _guildButton.Clicked += guildRequest_Clicked;
            _guildButton.HoverEnter += (sender, e) => _guildButton.SetToolTipText(Strings.Guilds.GuildTip.ToString(MyEntity?.Name));
            _guildButton.IsHidden = true;

            // Target Status Panel
            _targetStatus = new ImagePanel(TargetWindow, nameof(_targetStatus));

            SetEntity(myEntity);

            TargetWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            UpdateSpellStatus();

            i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
            {
                if (Options.PaperdollOrder[1][z] == "Player")
                {
                    _entitySprite.RenderColor = _entitySpriteContainer.RenderColor;
                }
                else
                {
                    _paperdollPanels[i].RenderColor = _entitySpriteContainer.RenderColor;
                    i++;
                }
            }

            TargetWindow.Hide();

            _lastUpdateTime = Timing.Global.MillisecondsUtc;
        }

        public void SetEntity(Entities.Entity entity)
        {
            MyEntity = entity;
            if (MyEntity != null)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (_entityType == EntityType.Event)
                {
                    _eventDesc.ClearText();
                    _eventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                    _eventDesc.SizeToChildren(false, true);
                }
            }
        }

        public void SetEntity(Entities.Entity entity, EntityType type)
        {
            MyEntity = entity;
            _entityType = type;
            if (MyEntity != null)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (_entityType == EntityType.Event)
                {
                    _eventDesc.ClearText();
                    _eventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                    _eventDesc.SizeToChildren(false, true);
                }
            }
        }

        public void ShowAllElements()
        {
            _eventDesc.Show();
            _mpBackground.Show();
            _mpBar.Show();
            _mpLabel.Show();
            _mpValue.Show();
            _hpBackground.Show();
            _hpBar.Show();
            _hpValue.Show();
            _hpLabel.Show();

            if (MyEntity.Type == EntityType.Player && MyEntity != Globals.Me)
            {
                _tradeButton.Show();
                _partyButton.Show();
                _friendButton.Show();
            }

            TryShowGuildButton();
        }

        public void SetupEntityElements()
        {
            ShowAllElements();

            //Update Bars
            _currentHpSize = -1;
            _currentShieldSize = -1;
            _currentMpSize = -1;
            _shieldBar.Hide();
            UpdateHpBar(0, true);
            UpdateMpBar(0, true);

            switch (_entityType)
            {
                case EntityType.Player:
                    _eventDesc.Hide();

                    break;
                case EntityType.GlobalEntity:
                    _eventDesc.Hide();
                    _tradeButton.Hide();
                    _partyButton.Hide();
                    _guildButton.Hide();
                    _friendButton.Hide();

                    break;
                case EntityType.Event:
                    _eventDesc.Show();
                    _mpBackground.Hide();
                    _mpBar.Hide();
                    _mpLabel.Hide();
                    _mpValue.Hide();
                    _hpBackground.Hide();
                    _hpBar.Hide();
                    _hpValue.Hide();
                    _hpLabel.Hide();
                    _tradeButton.Hide();
                    _partyButton.Hide();
                    _friendButton.Hide();
                    _guildButton.Hide();

                    break;
            }

            _entityName.SetText(MyEntity.Name);
            _shieldBar.Hide();
        }

        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!TargetWindow.IsHidden)
                {
                    TargetWindow.Hide();
                }

                return;
            }
            else
            {
                if (TargetWindow.IsHidden)
                {
                    TargetWindow.Show();
                }
            }

            UpdateSpellStatus();

            //Time since this window was last updated (for bar animations)
            var elapsedTime = (Timing.Global.MillisecondsUtc - _lastUpdateTime) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            IsHidden = true;
            if (_entityType != EntityType.Event)
            {
                _entityName.SetText(MyEntity.Name);
                UpdateLevel();
                UpdateHpBar(elapsedTime);
                UpdateMpBar(elapsedTime);
                IsHidden = false;
            }
            else
            {
                if (!_entityNameAndLevel.IsHidden)
                {
                    _entityNameAndLevel.Text = MyEntity.Name;
                }
            }

            if (MyEntity.Type == EntityType.Player)
            {
                if (MyEntity.Vital[(int)Vital.Health] <= 0 || MyEntity == Globals.Me)
                {
                    _tradeButton.Hide();
                    _partyButton.Hide();
                    _friendButton.Hide();
                    _guildButton.Hide();
                }
                else if (MyEntity != Globals.Me && (_tradeButton.IsHidden || _partyButton.IsHidden || _friendButton.IsHidden))
                {
                    _tradeButton.Show();
                    _partyButton.Show();
                    _friendButton.Show();
                    TryShowGuildButton();
                }
            }

            if (UpdateStatuses)
            {
                UpdateSpellStatus();
                UpdateStatuses = false;
            }

            foreach (var itm in _activeStatuses)
            {
                itm.Value.Update();
            }

            _lastUpdateTime = Timing.Global.MillisecondsUtc;
        }

        public void UpdateSpellStatus()
        {
            if (MyEntity == null)
            {
                return;
            }

            //Remove 'Dead' Statuses
            var statuses = _activeStatuses.Keys.ToArray();
            foreach (var status in statuses)
            {
                if (!MyEntity.StatusActive(status))
                {
                    var s = _activeStatuses[status];
                    s.StatusIcon.Texture = null;
                    s.Container.Hide();
                    s.Container.Texture = null;
                    _targetStatus.RemoveChild(s.Container, true);
                    s.pnl_HoverLeave(null, null);
                    _activeStatuses.Remove(status);
                }
                else
                {
                    _activeStatuses[status].UpdateStatus(MyEntity.GetStatus(status) as Status);
                }
            }

            //Add all of the spell status effects
            for (var i = 0; i < MyEntity.Status.Count; i++)
            {
                var id = MyEntity.Status[i].SpellId;
                SpellStatus itm;
                if (!_activeStatuses.ContainsKey(id) && MyEntity.Status[i] is Status status)
                {
                    itm = new SpellStatus(this, status)
                    {
                        Container = new ImagePanel(_targetStatus, "TargetStatusIcon")
                    };

                    itm.Setup();

                    itm.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                    itm.Container.Name = "";
                    _activeStatuses.Add(id, itm);
                }
                else
                {
                    itm = _activeStatuses[id];
                }

                var xPadding = itm.Container.Margin.Left + itm.Container.Margin.Right;
                var yPadding = itm.Container.Margin.Top + itm.Container.Margin.Bottom;

                itm.Container.SetPosition(
                    i %
                    (_targetStatus.Width /
                     Math.Max(1, _targetStatus.Width / (itm.Container.Width + xPadding))) *
                    (itm.Container.Width + xPadding) +
                    xPadding,
                    i /
                    Math.Max(1, _targetStatus.Width / (itm.Container.Width + xPadding)) *
                    (itm.Container.Height + yPadding) +
                    yPadding
                );
            }
        }

        private void UpdateLevel()
        {
            var levelString = Strings.EntityBox.Level.ToString(MyEntity.Level);
            if (!_entityLevel.IsHidden)
            {
                _entityLevel.Text = levelString;
            }

            if (!_entityNameAndLevel.IsHidden)
            {
                _entityNameAndLevel.Text = Strings.EntityBox.NameAndLevel.ToString(MyEntity.Name, levelString);
            }
        }

        private static float SetTargetBarSize(float barRatio, int barSize)
        {
            var barFillRatio = Math.Min(1, Math.Max(0, barRatio));

            return (float)Math.Ceiling((barFillRatio * barSize));
        }

        private static float SetCurrentBarSize(float elapsedTime, bool instant, float targetSize, float currentSize)
        {
            if (instant)
            {
                return (int)targetSize;
            }

            if ((int)targetSize > currentSize)
            {
                currentSize += 100f * elapsedTime;
                if (currentSize > (int)targetSize)
                {
                    currentSize = targetSize;
                }
            }
            else
            {
                currentSize -= 100f * elapsedTime;
                if (currentSize < targetSize)
                {
                    currentSize = targetSize;
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
            float targetHpSize;
            float targetShieldSize;
            var barDirectionSetting = ClientConfiguration.Instance.EntityBarDirections[(int)Vital.Health];
            var barPercentageSetting = Globals.Database.ShowHealthAsPercentage;
            var entityVital = MyEntity.Vital[(int)Vital.Health];
            var entityMaxVital = MyEntity.MaxVital[(int)Vital.Health];

            if (entityVital > 0)
            {

                var shieldSize = MyEntity.GetShieldSize();
                var vitalSize = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                    ? _hpBackground.Width
                    : _hpBackground.Height;

                //We have to get the maxVital value before being changed by the shield
                //Shield changes vitalMax only on client, showing incorrect values
                if (shieldSize + entityVital > entityMaxVital)
                {
                    entityMaxVital = shieldSize + entityVital;
                }

                var entityVitalRatio = (float)entityVital / entityMaxVital;
                var entityShieldRatio = (float)shieldSize / entityMaxVital;
                var hpPercentage = entityVitalRatio * 100;
                var hpPercentageText = $"{hpPercentage:0.##}%";
                var hpValueText = Strings.EntityBox.Vital0Value.ToString(entityVital, entityMaxVital);
                _hpValue.Text = barPercentageSetting ? hpPercentageText : hpValueText;
                _hpBackground.SetToolTipText(barPercentageSetting ? hpValueText : hpPercentageText);
                targetHpSize = SetTargetBarSize(entityVitalRatio, vitalSize);
                targetShieldSize = SetTargetBarSize(entityShieldRatio, vitalSize);
            }
            else
            {
                _hpValue.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital);
                _hpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital) : "0%");
                targetHpSize = 0;
                targetShieldSize = 0;
            }

            if ((int)targetHpSize != (int)_currentHpSize)
            {
                _currentHpSize = SetCurrentBarSize(elapsedTime, instant, targetHpSize, _currentHpSize);

                if (_currentHpSize == 0)
                {
                    _hpBar.IsHidden = true;
                }
                else
                {
                    UpdateGauge(_hpBackground, _hpBar, _currentHpSize, barDirectionSetting);
                }
            }

            if ((int)targetShieldSize != (int)_currentShieldSize)
            {
                _currentShieldSize = SetCurrentBarSize(elapsedTime, instant, targetShieldSize, _currentShieldSize);

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
            float targetMpSize;
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
                float mpPercentage = entityVitalRatio * 100;
                var mpPercentageText = $"{mpPercentage:0.##}%";
                var mpValueText = Strings.EntityBox.Vital1Value.ToString(entityVital, entityMaxVital);
                _mpValue.Text = barPercentageSetting ? mpPercentageText : mpValueText;
                _mpBackground.SetToolTipText(barPercentageSetting ? mpValueText : mpPercentageText);
                targetMpSize = SetTargetBarSize(entityVitalRatio, vitalSize);
            }
            else
            {
                _mpValue.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital);
                _mpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital) : "0%");
                targetMpSize = 0;
            }

            if ((int)targetMpSize != (int)_currentMpSize)
            {
                _currentMpSize = SetCurrentBarSize(elapsedTime, instant, targetMpSize, _currentMpSize);

                if (_currentMpSize == 0)
                {
                    _mpBar.IsHidden = true;
                }
                else
                {
                    UpdateGauge(_mpBackground, _mpBar, _currentMpSize, barDirectionSetting);
                }
            }
        }

        private void UpdateImage()
        {
            var faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, MyEntity.Face);
            var entityTex = MyEntity.Texture;
            if (faceTex != null && faceTex != _entitySprite.Texture)
            {
                _entitySprite.Texture = faceTex;
                _entitySprite.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                _entitySprite.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                _entitySprite.SizeToContents();
                Align.Center(_entitySprite);
                _currentSprite = MyEntity.Face;
                _entitySprite.IsHidden = false;
                var i = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    if (Options.PaperdollOrder[1][z] != "Player")
                    {
                        if (_paperdollPanels == null)
                        {
                            Log.Warn($@"{nameof(_paperdollPanels)} is null.");
                        }
                        else if (_paperdollPanels[i] == null)
                        {
                            Log.Warn($@"{nameof(_paperdollPanels)}[{i}] is null.");
                        }

                        _paperdollPanels?[i]?.Hide();
                        i++;
                    }
                }
            }
            else if (entityTex != null && faceTex == null || faceTex != null && faceTex != _entitySprite.Texture)
            {
                if (entityTex != _entitySprite.Texture)
                {
                    _entitySprite.Texture = entityTex;
                    _entitySprite.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                    _entitySprite.SetTextureRect(0, 0, entityTex.GetWidth() / Options.Instance.Sprites.NormalFrames, entityTex.GetHeight() / Options.Instance.Sprites.Directions);
                    _entitySprite.SizeToContents();
                    Align.Center(_entitySprite);
                    _currentSprite = MyEntity.Sprite;
                    _entitySprite.IsHidden = false;
                }

                var equipment = MyEntity.Equipment;
                var n = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    var paperdollPanel = _paperdollPanels[n];
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

                    if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(_paperdollTextures[n]))
                    {
                        paperdollPanel.Texture = null;
                        paperdollPanel.Hide();
                        _paperdollTextures[n] = string.Empty;
                    }
                    else if (!string.IsNullOrWhiteSpace(paperdoll) && paperdoll != _paperdollTextures[n])
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
                                    (_entitySpriteContainer.Width - paperdollPanel.Width) / 2,
                                    (_entitySpriteContainer.Height - paperdollPanel.Height) / 2
                                );
                        }

                        paperdollPanel.Show();
                        _paperdollTextures[n] = paperdoll;
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
                _entitySprite.IsHidden = true;
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPanels[i]?.Hide();
                }
            }

            if (_entitySprite.RenderColor != MyEntity.Color)
            {
                _entitySprite.RenderColor = MyEntity.Color;
            }
        }

        public void Dispose()
        {
            TargetWindow.Hide();
            Interface.GameUi.GameCanvas.RemoveChild(TargetWindow, false);
            TargetWindow.Dispose();
        }

        //Input Handlers
        void invite_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendPartyInvite(Globals.Me.TargetIndex);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Parties.InFight.ToString(), 4);
                }
            }
        }

        //Input Handlers
        void tradeRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendTradeRequest(Globals.Me.TargetIndex);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Trading.InFight.ToString(), 4);
                }
            }
        }

        //Input Handlers
        void friendRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendAddFriend(MyEntity.Name);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                }
            }
        }


        void guildRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (MyEntity is Entities.Player plyr && MyEntity != Globals.Me)
            {
                if (string.IsNullOrWhiteSpace(plyr.Guild))
                {
                    if (Globals.Me?.GuildRank?.Permissions?.Invite ?? false)
                    {
                        if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                        {
                            PacketSender.SendInviteGuild(MyEntity.Name);
                        }
                        else
                        {
                            PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                        }
                    }
                }
                else
                {
                    Chat.ChatboxMsg.AddMessage(new Chat.ChatboxMsg(Strings.Guilds.InviteAlreadyInGuild, Color.Red, ChatMessageType.Guild));
                }
            }
        }

        void TryShowGuildButton()
        {
            var show = false;
            if (MyEntity is Entities.Player plyr && MyEntity != Globals.Me && string.IsNullOrWhiteSpace(plyr.Guild))
            {
                if (Globals.Me?.GuildRank?.Permissions?.Invite ?? false)
                {
                    show = true;
                }
            }

            _guildButton.IsHidden = !show;
        }


        public void Hide()
        {
            TargetWindow.Hide();
        }

        public void Show()
        {
            TargetWindow.Show();
        }

    }

}
