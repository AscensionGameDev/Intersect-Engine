using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Game.EntityPanel;


public partial class EntityBox
{
    public readonly Label EntityLevel;

    public readonly Label EntityMap;

    public readonly Label EntityName;

    public readonly Label EntityNameAndLevel;

    //Controls
    public readonly ImagePanel EntityWindow;

    public float CurExpSize = -1;

    public float CurHpSize = -1;

    public float CurMpSize = -1;

    public float CurShieldSize = -1;

    public ImagePanel EntityFace;

    public ImagePanel EntityFaceContainer;

    public ImagePanel EntityInfoPanel;

    private ImagePanel EntityStatusWindow;

    public ScrollControl EntityStatusPanel;

    public EntityType EntityType;

    public RichLabel EventDesc;

    public ImagePanel ExpBackground;

    public ImagePanel ExpBar;

    public Label ExpLbl;

    public Label ExpTitle;

    //public Button FriendLabel;

    public ImagePanel HpBackground;

    public ImagePanel HpBar;

    public Label HpLbl;

    public Label HpTitle;

    private Dictionary<Guid, SpellStatus> mActiveStatuses = new Dictionary<Guid, SpellStatus>();

    private string mCurrentSprite = string.Empty;

    private long mLastUpdateTime;

    public ImagePanel MpBackground;

    public ImagePanel MpBar;

    public Label MpLbl;

    public Label MpTitle;

    public Entity MyEntity;

    public ImagePanel[] PaperdollPanels;

    public string[] PaperdollTextures;

    private bool _isPlayerBox;

    public ImagePanel ShieldBar;

    public bool ShouldUpdateStatuses;

    // Context menu
    private readonly Button _contextMenuButton;

    //Init
    public EntityBox(Canvas gameCanvas, EntityType entityType, Entity? myEntity, bool isPlayerBox = false)
    {
        MyEntity = myEntity;
        EntityType = entityType;
        _isPlayerBox = isPlayerBox;
        EntityWindow =
            _isPlayerBox ? new ImagePanel(gameCanvas, "PlayerBox") : new ImagePanel(gameCanvas, "TargetBox");

        EntityWindow.ShouldCacheToTexture = true;

        EntityInfoPanel = new ImagePanel(EntityWindow, "EntityInfoPanel");

        EntityName = new Label(EntityInfoPanel, "EntityNameLabel") { Text = myEntity?.Name };
        EntityLevel = new Label(EntityInfoPanel, "EntityLevelLabel");
        EntityNameAndLevel = new Label(EntityInfoPanel, "NameAndLevelLabel")
        { IsHidden = true };

        EntityMap = new Label(EntityInfoPanel, "EntityMapLabel");

        PaperdollPanels = new ImagePanel[Options.Instance.Equipment.Slots.Count];
        PaperdollTextures = new string[Options.Instance.Equipment.Slots.Count];
        var i = 0;
        for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[1].Count; z++)
        {
            if (Options.Instance.Equipment.Paperdoll.Directions[1][z] == "Player")
            {
                EntityFaceContainer = new ImagePanel(EntityInfoPanel, "EntityGraphicContainer");

                EntityFace = new ImagePanel(EntityFaceContainer);
                EntityFace.SetSize(64, 64);
                EntityFace.AddAlignment(Alignments.Center);
            }
            else
            {
                PaperdollPanels[i] = new ImagePanel(EntityFaceContainer);
                PaperdollTextures[i] = string.Empty;
                PaperdollPanels[i].Hide();
                i++;
            }
        }

        if (!_isPlayerBox)
        {
            EventDesc = new RichLabel(EntityInfoPanel, "EventDescLabel");
        }

        HpBackground = new ImagePanel(EntityInfoPanel, "HPBarBackground");
        HpBar = new ImagePanel(EntityInfoPanel, "HPBar");
        ShieldBar = new ImagePanel(EntityInfoPanel, "ShieldBar");
        HpTitle = new Label(EntityInfoPanel, "HPTitle");
        HpTitle.SetText(Strings.EntityBox.Vital0);
        HpLbl = new Label(EntityInfoPanel, "HPLabel");

        MpBackground = new ImagePanel(EntityInfoPanel, "MPBackground");
        MpBar = new ImagePanel(EntityInfoPanel, "MPBar");
        MpTitle = new Label(EntityInfoPanel, "MPTitle");
        MpTitle.SetText(Strings.EntityBox.Vital1);
        MpLbl = new Label(EntityInfoPanel, "MPLabel");

        ExpBackground = new ImagePanel(EntityInfoPanel, "EXPBackground");
        ExpBar = new ImagePanel(EntityInfoPanel, "EXPBar");
        ExpTitle = new Label(EntityInfoPanel, "EXPTitle");
        ExpTitle.SetText(Strings.EntityBox.Exp);
        ExpLbl = new Label(EntityInfoPanel, "EXPLabel");

        // Target context menu with basic options.
        if (!_isPlayerBox)
        {
            _contextMenuButton = new Button(EntityInfoPanel, "ContextMenuButton");
            _contextMenuButton.Clicked += (_, _) =>
            {
                Interface.GameUi.TargetContextMenu.ToggleHidden(_contextMenuButton);
            };

            EntityStatusWindow = new ImagePanel(EntityWindow, "EntityStatusWindow");
            EntityStatusPanel = new ScrollControl(EntityStatusWindow, "StatusArea");
        }

        EntityWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        SetEntity(myEntity);

        i = 0;
        for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[1].Count; z++)
        {
            if (Options.Instance.Equipment.Paperdoll.Directions[1][z] == "Player")
            {
                EntityFace.RenderColor = EntityFaceContainer.RenderColor;
            }
            else
            {
                PaperdollPanels[i].RenderColor = EntityFaceContainer.RenderColor;
                i++;
            }
        }

        EntityWindow.Hide();

        mLastUpdateTime = Timing.Global.MillisecondsUtc;
    }

    public void SetEntity(Entity? entity)
    {
        MyEntity = entity;
        if (MyEntity != null)
        {
            SetupEntityElements();
            ShouldUpdateStatuses = !_isPlayerBox;

            if (EntityType == EntityType.Event)
            {
                EventDesc.ClearText();
                EventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                EventDesc.SizeToChildren(false, true);
            }
        }
    }

    public void SetEntity(Entity entity, EntityType type)
    {
        MyEntity = entity;
        EntityType = type;
        if (MyEntity != null)
        {
            SetupEntityElements();
            ShouldUpdateStatuses = !_isPlayerBox;
            
            if (EntityType == EntityType.Event)
            {
                EventDesc.ClearText();
                EventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                EventDesc.SizeToChildren(false, true);
            }
        }
    }

    public void ShowAllElements()
    {
        ExpBackground.Show();
        ExpBar.Show();
        ExpLbl.Show();
        ExpTitle.Show();
        EntityMap.Show();

        if (!_isPlayerBox)
        {
            _contextMenuButton.Show();
            EventDesc.Show();
        }

        MpBackground.Show();
        MpBar.Show();
        MpTitle.Show();
        MpLbl.Show();
        HpBackground.Show();
        HpBar.Show();
        HpLbl.Show();
        HpTitle.Show();
    }

    public void SetupEntityElements()
    {
        ShowAllElements();

        //Update Bars
        CurHpSize = -1;
        CurShieldSize = -1;
        CurMpSize = -1;
        CurExpSize = -1;
        ShieldBar.Hide();
        UpdateHpBar(0, true);
        UpdateMpBar(0, true);
        if (MyEntity is Player)
        {
            UpdateXpBar(0, true);
        }

        switch (EntityType)
        {
            case EntityType.Player:
                if (Globals.Me != null && Globals.Me == MyEntity)
                {
                    if (!_isPlayerBox)
                    {
                        _contextMenuButton.Hide();
                        ExpBackground.Hide();
                        ExpBar.Hide();
                        ExpLbl.Hide();
                        ExpTitle.Hide();
                        EntityMap.Hide();
                    }
                }
                else
                {
                    ExpBackground.Hide();
                    ExpBar.Hide();
                    ExpLbl.Hide();
                    ExpTitle.Hide();
                    EntityMap.Hide();
                }

                EventDesc?.Hide();
                EntityLevel.Show();

                break;
            case EntityType.GlobalEntity:
                EventDesc.Hide();
                ExpBackground.Hide();
                ExpBar.Hide();
                ExpLbl.Hide();
                ExpTitle.Hide();
                _contextMenuButton.Hide();
                EntityMap.Hide();
                EntityLevel.Show();

                break;
            case EntityType.Event:
                EventDesc.Show();
                EntityLevel.Hide();
                ExpBackground.Hide();
                ExpBar.Hide();
                ExpLbl.Hide();
                ExpTitle.Hide();
                MpBackground.Hide();
                MpBar.Hide();
                MpTitle.Hide();
                MpLbl.Hide();
                HpBackground.Hide();
                HpBar.Hide();
                HpLbl.Hide();
                HpTitle.Hide();
                _contextMenuButton.Hide();
                EntityMap.Hide();

                break;
        }

        EntityName.SetText(MyEntity.Name);
    }

    //Update
    public void Update()
    {
        if (MyEntity == null || MyEntity.IsDisposed())
        {
            if (!EntityWindow.IsHidden)
            {
                EntityWindow.Hide();
            }

            return;
        }
        else
        {
            if (EntityWindow.IsHidden)
            {
                Show();
            }
        }

        if (_isPlayerBox)
        {
            if (EntityWindow.IsHidden)
            {
                Show();
            }

            if (MyEntity.IsDisposed())
            {
                Dispose();
            }
        }

        //Time since this window was last updated (for bar animations)
        var elapsedTime = (Timing.Global.MillisecondsUtc - mLastUpdateTime) / 1000.0f;

        //Update the event/entity face.
        UpdateImage();

        if (EntityType != EntityType.Event)
        {
            EntityName.SetText(MyEntity.Name);
            UpdateLevel();
            UpdateMap();
            UpdateHpBar(elapsedTime);
            UpdateMpBar(elapsedTime);
        }
        else
        {
            if (!EntityNameAndLevel.IsHidden)
            {
                EntityNameAndLevel.Text = MyEntity.Name;
            }
        }

        //If player draw exp bar
        if (_isPlayerBox && MyEntity == Globals.Me)
        {
            UpdateXpBar(elapsedTime);
        }

        if (MyEntity.Type == EntityType.Player && MyEntity != Globals.Me)
        {
            if (MyEntity.Vital[(int)Vital.Health] <= 0)
            {
                _contextMenuButton.Hide();
            }
            else if (_contextMenuButton.IsHidden && !_isPlayerBox)
            {
                _contextMenuButton.Show();
            }
        }

        ShouldUpdateStatuses = !_isPlayerBox;
        if (ShouldUpdateStatuses)
        {
            SpellStatus.UpdateSpellStatus(MyEntity, EntityStatusPanel, mActiveStatuses);
            EntityStatusWindow.IsHidden = mActiveStatuses.Count < 1;

            foreach (var itm in mActiveStatuses)
            {
                itm.Value.Update();
            }

            ShouldUpdateStatuses = false;
        }

        mLastUpdateTime = Timing.Global.MillisecondsUtc;
    }

    private void UpdateLevel()
    {
        var levelString = Strings.EntityBox.Level.ToString(MyEntity.Level);
        if (!EntityLevel.IsHidden)
        {
            EntityLevel.Text = levelString;
        }

        if (!EntityNameAndLevel.IsHidden)
        {
            EntityNameAndLevel.Text = Strings.EntityBox.NameAndLevel.ToString(MyEntity.Name, levelString);
        }
    }

    private void UpdateMap()
    {
        if (Globals.Me.MapInstance != null)
        {
            EntityMap.SetText(Strings.EntityBox.Map.ToString(Globals.Me.MapInstance.Name));
        }
        else
        {
            EntityMap.SetText(Strings.EntityBox.Map.ToString(""));
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
                ? HpBackground.Width
                : HpBackground.Height;

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
            HpLbl.Text = barPercentageSetting ? hpPercentageText : hpValueText;
            HpBackground.SetToolTipText(barPercentageSetting ? hpValueText : hpPercentageText);
            targetHpSize = SetTargetBarSize(entityVitalRatio, vitalSize);
            targetShieldSize = SetTargetBarSize(entityShieldRatio, vitalSize);
        }
        else
        {
            HpLbl.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital);
            HpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital0Value.ToString(0, entityMaxVital) : "0%");
            targetHpSize = 0;
            targetShieldSize = 0;
        }

        if ((int)targetHpSize != (int)CurHpSize)
        {
            CurHpSize = SetCurrentBarSize(elapsedTime, instant, targetHpSize, CurHpSize);

            if (CurHpSize == 0)
            {
                HpBar.IsHidden = true;
            }
            else
            {
                UpdateGauge(HpBackground, HpBar, CurHpSize, barDirectionSetting);
            }
        }

        if ((int)targetShieldSize != (int)CurShieldSize)
        {
            CurShieldSize = SetCurrentBarSize(elapsedTime, instant, targetShieldSize, CurShieldSize);

            if (CurShieldSize == 0)
            {
                ShieldBar.IsHidden = true;
            }
            else
            {
                UpdateGauge(HpBackground, ShieldBar, CurShieldSize, barDirectionSetting, true);
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
                ? MpBackground.Width
                : MpBackground.Height;
            float mpPercentage = entityVitalRatio * 100;
            var mpPercentageText = $"{mpPercentage:0.##}%";
            var mpValueText = Strings.EntityBox.Vital1Value.ToString(entityVital, entityMaxVital);
            MpLbl.Text = barPercentageSetting ? mpPercentageText : mpValueText;
            MpBackground.SetToolTipText(barPercentageSetting ? mpValueText : mpPercentageText);
            targetMpSize = SetTargetBarSize(entityVitalRatio, vitalSize);
        }
        else
        {
            MpLbl.Text = barPercentageSetting ? "0%" : Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital);
            MpBackground.SetToolTipText(barPercentageSetting ? Strings.EntityBox.Vital1Value.ToString(0, entityMaxVital) : "0%");
            targetMpSize = 0;
        }

        if ((int)targetMpSize != (int)CurMpSize)
        {
            CurMpSize = SetCurrentBarSize(elapsedTime, instant, targetMpSize, CurMpSize);

            if (CurMpSize == 0)
            {
                MpBar.IsHidden = true;
            }
            else
            {
                UpdateGauge(MpBackground, MpBar, CurMpSize, barDirectionSetting);
            }
        }
    }

    private void UpdateXpBar(float elapsedTime, bool instant = false)
    {
        float targetExpSize;
        var barDirectionSetting = ClientConfiguration.Instance.EntityBarDirections[Enum.GetValues<Vital>().Length];
        var barPercentageSetting = Globals.Database.ShowExperienceAsPercentage;
        var entityExperienceToNextLevel = ((Player)MyEntity).GetNextLevelExperience();

        if (entityExperienceToNextLevel > 0)
        {
            var entityExperience = ((Player)MyEntity).Experience;
            var entityExperienceRatio = (float)entityExperience / entityExperienceToNextLevel;
            var vitalSize = (int)barDirectionSetting < (int)DisplayDirection.TopToBottom
                ? ExpBackground.Width
                : ExpBackground.Height;
            var expPercentage = entityExperienceRatio * 100;
            var expPercentageText = $"{expPercentage:0.##}%";
            var expValueText = Strings.EntityBox.ExpValue.ToString(entityExperience, entityExperienceToNextLevel);
            ExpLbl.Text = barPercentageSetting ? expPercentageText : expValueText;
            ExpBackground.SetToolTipText(barPercentageSetting ? expValueText : expPercentageText);
            targetExpSize = SetTargetBarSize(entityExperienceRatio, vitalSize);
        }
        else
        {
            targetExpSize = 1f;
            ExpLbl.Text = Strings.EntityBox.MaxLevel;
            ExpBackground.SetToolTipText(Strings.EntityBox.MaxLevel);
        }

        if (Math.Abs((int)targetExpSize - CurExpSize) < 0.01)
        {
            return;
        }

        CurExpSize = SetCurrentBarSize(elapsedTime, instant, targetExpSize, CurExpSize);

        if (CurExpSize == 0)
        {
            ExpBar.IsHidden = true;
        }
        else
        {
            UpdateGauge(ExpBackground, ExpBar, CurExpSize, barDirectionSetting);
        }
    }

    private void UpdateImage()
    {
        var faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, MyEntity.Face);
        var entityTex = MyEntity.Texture;
        if (faceTex != null && faceTex != EntityFace.Texture)
        {
            EntityFace.Texture = faceTex;
            EntityFace.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
            EntityFace.SetTextureRect(0, 0, faceTex.Width, faceTex.Height);
            EntityFace.SizeToContents();
            Align.Center(EntityFace);
            mCurrentSprite = MyEntity.Face;
            EntityFace.IsHidden = false;
            var i = 0;
            for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[1].Count; z++)
            {
                if (Options.Instance.Equipment.Paperdoll.Directions[1][z] != "Player")
                {
                    if (PaperdollPanels == null)
                    {
                        ApplicationContext.Context.Value?.Logger.LogWarning($@"{nameof(PaperdollPanels)} is null.");
                    }
                    else if (PaperdollPanels[i] == null)
                    {
                        ApplicationContext.Context.Value?.Logger.LogWarning($@"{nameof(PaperdollPanels)}[{i}] is null.");
                    }

                    PaperdollPanels?[i]?.Hide();
                    i++;
                }
            }
        }
        else if (entityTex != null && faceTex == null || faceTex != null && faceTex != EntityFace.Texture)
        {
            if (entityTex != EntityFace.Texture)
            {
                EntityFace.Texture = entityTex;
                EntityFace.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                EntityFace.SetTextureRect(0, 0, entityTex.Width / Options.Instance.Sprites.NormalFrames, entityTex.Height / Options.Instance.Sprites.Directions);
                EntityFace.SizeToContents();
                Align.Center(EntityFace);
                mCurrentSprite = MyEntity.Sprite;
                EntityFace.IsHidden = false;
            }

            var equipment = MyEntity.Equipment;
            if (MyEntity == Globals.Me)
            {
                for (var i = 0; i < MyEntity.MyEquipment.Length; i++)
                {
                    var eqp = MyEntity.MyEquipment[i];
                    if (eqp > -1 && eqp < Options.Instance.Player.MaxInventory)
                    {
                        equipment[i] = MyEntity.Inventory[eqp].ItemId;
                    }
                    else
                    {
                        equipment[i] = Guid.Empty;
                    }
                }
            }

            var n = 0;
            for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[1].Count; z++)
            {
                var paperdollPanel = PaperdollPanels[n];
                var paperdoll = string.Empty;
                if (Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z]) > -1 &&
                    equipment.Length == Options.Instance.Equipment.Slots.Count)
                {
                    if (equipment[Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z])] != Guid.Empty)
                    {
                        var itemId = equipment[Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z])];
                        if (ItemBase.TryGet(itemId, out var itemDescriptor))
                        {
                            paperdoll = MyEntity.Gender == 0
                                ? itemDescriptor.MalePaperdoll : itemDescriptor.FemalePaperdoll;
                            paperdollPanel.RenderColor = itemDescriptor.Color;
                        }
                    }
                }

                //Check for Player layer
                if (Options.Instance.Equipment.Paperdoll.Directions[1][z] == "Player")
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(PaperdollTextures[n]))
                {
                    paperdollPanel.Texture = null;
                    paperdollPanel.Hide();
                    PaperdollTextures[n] = string.Empty;
                }
                else if (!string.IsNullOrWhiteSpace(paperdoll) && paperdoll != PaperdollTextures[n])
                {
                    var paperdollTex = Globals.ContentManager.GetTexture(
                        Framework.Content.TextureType.Paperdoll, paperdoll
                    );

                    paperdollPanel.Texture = paperdollTex;
                    if (paperdollTex != null)
                    {
                        paperdollPanel
                            .SetTextureRect(
                                0, 0, paperdollPanel.Texture.Width / Options.Instance.Sprites.NormalFrames,
                                paperdollPanel.Texture.Height / Options.Instance.Sprites.Directions
                            );

                        paperdollPanel
                            .SetSize(
                                paperdollPanel.Texture.Width / Options.Instance.Sprites.NormalFrames,
                                paperdollPanel.Texture.Height / Options.Instance.Sprites.Directions
                            );

                        paperdollPanel
                            .SetPosition(
                                (EntityFaceContainer.Width - paperdollPanel.Width) / 2,
                                (EntityFaceContainer.Height - paperdollPanel.Height) / 2
                            );
                    }

                    paperdollPanel.Show();
                    PaperdollTextures[n] = paperdoll;
                }

                //Check for Player layer
                if (Options.Instance.Equipment.Paperdoll.Directions[1][z] != "Player")
                {
                    n++;
                }
            }
        }
        else if (MyEntity.Sprite != mCurrentSprite && MyEntity.Face != mCurrentSprite)
        {
            EntityFace.IsHidden = true;
            for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
            {
                PaperdollPanels[i]?.Hide();
            }
        }

        if (EntityFace.RenderColor != MyEntity.Color)
        {
            EntityFace.RenderColor = MyEntity.Color;
        }
    }

    public void Dispose()
    {
        EntityWindow.Hide();
        Interface.GameUi.GameCanvas.RemoveChild(EntityWindow, false);
        EntityWindow.Dispose();
    }

    public void Hide()
    {
        EntityWindow.Hide();
    }

    public void Show()
    {
        if (!Options.Instance.Combat.EnableTargetWindow && !_isPlayerBox)
        {
            return;
        }

        EntityWindow.Show();
    }

}
