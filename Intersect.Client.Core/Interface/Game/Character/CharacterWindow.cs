using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.PlayerClass;

namespace Intersect.Client.Interface.Game.Character;


public partial class CharacterWindow
{

    //Equipment List
    public List<EquipmentItem> Items = new List<EquipmentItem>();

    Label mAbilityPwrLabel;

    Button mAddAbilityPwrBtn;

    Button mAddAttackBtn;

    Button mAddDefenseBtn;

    Button mAddMagicResistBtn;

    Button mAddSpeedBtn;

    //Stats
    Label mAttackLabel;

    private ImagePanel mCharacterContainer;

    private Label mCharacterLevelAndClass;

    private Label mCharacterName;

    private ImagePanel mCharacterPortrait;

    private string mCharacterPortraitImg = string.Empty;

    //Controls
    private WindowControl mCharacterWindow;

    private string mCurrentSprite = string.Empty;

    Label mDefenseLabel;

    private ItemProperties mItemProperties = null;

    Label mMagicRstLabel;

    Label mPointsLabel;

    Label mSpeedLabel;

    public ImagePanel[] PaperdollPanels;

    public string[] PaperdollTextures;

    //Location
    public int X;

    public int Y;

    //Extra Buffs
    Button _detailsButton;
    
    ClassDescriptor mPlayer;

    long HpRegenAmount;

    long ManaRegenAmount;

    int LifeStealAmount = 0;

    int ExtraExpAmount = 0;

    int LuckAmount = 0;

    int TenacityAmount = 0;

    int CooldownAmount = 0;

    int ManaStealAmount = 0;

    //Init
    public CharacterWindow(Canvas gameCanvas)
    {
        mCharacterWindow = new WindowControl(gameCanvas, Strings.Character.Title, false, "CharacterWindow");
        mCharacterWindow.DisableResizing();

        mCharacterName = new Label(mCharacterWindow, "CharacterNameLabel");
        mCharacterName.SetTextColor(Color.White, ComponentState.Normal);

        mCharacterLevelAndClass = new Label(mCharacterWindow, "ChatacterInfoLabel");
        mCharacterLevelAndClass.SetText("");

        mCharacterContainer = new ImagePanel(mCharacterWindow, "CharacterContainer");

        mCharacterPortrait = new ImagePanel(mCharacterContainer);

        PaperdollPanels = new ImagePanel[Options.Instance.Equipment.Slots.Count + 1];
        PaperdollTextures = new string[Options.Instance.Equipment.Slots.Count + 1];
        for (var i = 0; i <= Options.Instance.Equipment.Slots.Count; i++)
        {
            PaperdollPanels[i] = new ImagePanel(mCharacterContainer);
            PaperdollTextures[i] = string.Empty;
            PaperdollPanels[i].Hide();
        }

        var equipmentLabel = new Label(mCharacterWindow, "EquipmentLabel");
        equipmentLabel.SetText(Strings.Character.Equipment);

        var statsLabel = new Label(mCharacterWindow, "StatsLabel");
        statsLabel.SetText(Strings.Character.Stats);

        mAttackLabel = new Label(mCharacterWindow, "AttackLabel");

        mAddAttackBtn = new Button(mCharacterWindow, "IncreaseAttackButton");
        mAddAttackBtn.Clicked += _addAttackBtn_Clicked;

        mDefenseLabel = new Label(mCharacterWindow, "DefenseLabel");
        mAddDefenseBtn = new Button(mCharacterWindow, "IncreaseDefenseButton");
        mAddDefenseBtn.Clicked += _addDefenseBtn_Clicked;

        mSpeedLabel = new Label(mCharacterWindow, "SpeedLabel");
        mAddSpeedBtn = new Button(mCharacterWindow, "IncreaseSpeedButton");
        mAddSpeedBtn.Clicked += _addSpeedBtn_Clicked;

        mAbilityPwrLabel = new Label(mCharacterWindow, "AbilityPowerLabel");
        mAddAbilityPwrBtn = new Button(mCharacterWindow, "IncreaseAbilityPowerButton");
        mAddAbilityPwrBtn.Clicked += _addAbilityPwrBtn_Clicked;

        mMagicRstLabel = new Label(mCharacterWindow, "MagicResistLabel");
        mAddMagicResistBtn = new Button(mCharacterWindow, "IncreaseMagicResistButton");
        mAddMagicResistBtn.Clicked += _addMagicResistBtn_Clicked;

        mPointsLabel = new Label(mCharacterWindow, "PointsLabel");

        for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
        {
            Items.Add(new EquipmentItem(i, mCharacterWindow));
            Items[i].Pnl = new ImagePanel(mCharacterWindow, "EquipmentItem" + i);
            Items[i].Setup();
        }

        _detailsButton = new Button(mCharacterWindow, nameof(_detailsButton))
        {
            Text = Strings.Character.ExtraBuffDetails,
        };
        _detailsButton.HoverEnter += UpdateExtraBuffTooltip; // Update Tooltip on hover.
        UpdateExtraBuffTooltip(null, null); // Initial tooltip update.

        mCharacterWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    //Update Button Event Handlers
    void _addMagicResistBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendUpgradeStat((int) Stat.MagicResist);
    }

    void _addAbilityPwrBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendUpgradeStat((int) Stat.AbilityPower);
    }

    void _addSpeedBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendUpgradeStat((int) Stat.Speed);
    }

    void _addDefenseBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendUpgradeStat((int) Stat.Defense);
    }

    void _addAttackBtn_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendUpgradeStat((int) Stat.Attack);
    }

    //Methods
    public void Update()
    {
        if (mCharacterWindow.IsHidden || Globals.Me is null)
        {
            return;
        }

        mCharacterName.Text = Globals.Me.Name;
        mCharacterLevelAndClass.Text = Strings.Character.LevelAndClass.ToString(
            Globals.Me.Level, ClassDescriptor.GetName(Globals.Me.Class)
        );

        //Load Portrait
        //UNCOMMENT THIS LINE IF YOU'D RATHER HAVE A FACE HERE IGameTexture faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, Globals.Me.Face);
        var entityTex = Globals.ContentManager.GetTexture(
            Framework.Content.TextureType.Entity, Globals.Me.Sprite
        );

        /* UNCOMMENT THIS BLOCK IF YOU"D RATHER HAVE A FACE HERE if (Globals.Me.Face != "" && Globals.Me.Face != _currentSprite && faceTex != null)
         {
             _characterPortrait.Texture = faceTex;
             _characterPortrait.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
             _characterPortrait.SizeToContents();
             Align.Center(_characterPortrait);
             _characterPortrait.IsHidden = false;
             for (int i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
             {
                 _paperdollPanels[i].Hide();
             }
         }
         else */
        if (!string.IsNullOrWhiteSpace(Globals.Me.Sprite) && Globals.Me.Sprite != mCurrentSprite && entityTex != null)
        {
            for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[1].Count; z++)
            {
                var paperdoll = string.Empty;
                if (Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z]) > -1)
                {
                    var equipment = Globals.Me.MyEquipment;
                    if (equipment[Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z])] > -1 &&
                        equipment[Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z])] <
                        Options.Instance.Player.MaxInventory)
                    {
                        var itemNum = Globals.Me
                            .Inventory[equipment[Options.Instance.Equipment.Slots.IndexOf(Options.Instance.Equipment.Paperdoll.Directions[1][z])]]
                            .ItemId;

                        if (ItemDescriptor.TryGet(itemNum, out var itemDescriptor))
                        {
                            paperdoll = Globals.Me.Gender == 0
                                ? itemDescriptor.MalePaperdoll : itemDescriptor.FemalePaperdoll;
                            PaperdollPanels[z].RenderColor = itemDescriptor.Color;
                        }
                    }
                }
                else if (Options.Instance.Equipment.Paperdoll.Directions[1][z] == "Player")
                {
                    PaperdollPanels[z].Show();
                    PaperdollPanels[z].Texture = entityTex;
                    PaperdollPanels[z].SetTextureRect(0, 0, entityTex.Width / Options.Instance.Sprites.NormalFrames, entityTex.Height / Options.Instance.Sprites.Directions);
                    PaperdollPanels[z].SizeToContents();
                    PaperdollPanels[z].RenderColor = Globals.Me.Color;
                    Align.Center(PaperdollPanels[z]);
                }

                if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(PaperdollTextures[z]) && Options.Instance.Equipment.Paperdoll.Directions[1][z] != "Player")
                {
                    PaperdollPanels[z].Texture = null;
                    PaperdollPanels[z].Hide();
                    PaperdollTextures[z] = string.Empty;
                }
                else if (paperdoll != "" && paperdoll != PaperdollTextures[z])
                {
                    var paperdollTex = Globals.ContentManager.GetTexture(
                        Framework.Content.TextureType.Paperdoll, paperdoll
                    );

                    PaperdollPanels[z].Texture = paperdollTex;
                    if (paperdollTex != null)
                    {
                        PaperdollPanels[z]
                            .SetTextureRect(
                                0, 0, PaperdollPanels[z].Texture.Width / Options.Instance.Sprites.NormalFrames,
                                PaperdollPanels[z].Texture.Height / Options.Instance.Sprites.Directions
                            );

                        PaperdollPanels[z]
                            .SetSize(
                                PaperdollPanels[z].Texture.Width / Options.Instance.Sprites.NormalFrames,
                                PaperdollPanels[z].Texture.Height / Options.Instance.Sprites.Directions
                            );

                        PaperdollPanels[z]
                            .SetPosition(
                                mCharacterContainer.Width / 2 - PaperdollPanels[z].Width / 2,
                                mCharacterContainer.Height / 2 - PaperdollPanels[z].Height / 2
                            );
                    }

                    PaperdollPanels[z].Show();
                    PaperdollTextures[z] = paperdoll;
                }
            }
        }
        else if (Globals.Me.Sprite != mCurrentSprite && Globals.Me.Face != mCurrentSprite)
        {
            mCharacterPortrait.IsHidden = true;
            for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
            {
                PaperdollPanels[i].Hide();
            }
        }

        mAttackLabel.SetText(
            Strings.Character.StatLabelValue.ToString(
                Strings.Combat.Stats[Stat.Attack],
                Globals.Me.Stat[(int)Stat.Attack]
            )
        );

        mAbilityPwrLabel.SetText(
            Strings.Character.StatLabelValue.ToString(
                Strings.Combat.Stats[Stat.AbilityPower],
                Globals.Me.Stat[(int)Stat.AbilityPower]
            )
        );

        mDefenseLabel.SetText(
            Strings.Character.StatLabelValue.ToString(
                Strings.Combat.Stats[Stat.Defense],
                Globals.Me.Stat[(int)Stat.Defense]
            )
        );

        mMagicRstLabel.SetText(
            Strings.Character.StatLabelValue.ToString(
                Strings.Combat.Stats[Stat.MagicResist],
                Globals.Me.Stat[(int)Stat.MagicResist]
            )
        );

        mSpeedLabel.SetText(
            Strings.Character.StatLabelValue.ToString(
                Strings.Combat.Stats[Stat.Speed],
                Globals.Me.Stat[(int)Stat.Speed]
            )
        );

        mPointsLabel.SetText(Strings.Character.Points.ToString(Globals.Me.StatPoints));
        mAddAbilityPwrBtn.IsHidden = Globals.Me.StatPoints == 0 ||
                                     Globals.Me.Stat[(int) Stat.AbilityPower] == Options.Instance.Player.MaxStat;

        mAddAttackBtn.IsHidden =
            Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int) Stat.Attack] == Options.Instance.Player.MaxStat;

        mAddDefenseBtn.IsHidden = Globals.Me.StatPoints == 0 ||
                                  Globals.Me.Stat[(int) Stat.Defense] == Options.Instance.Player.MaxStat;

        mAddMagicResistBtn.IsHidden = Globals.Me.StatPoints == 0 ||
                                      Globals.Me.Stat[(int) Stat.MagicResist] == Options.Instance.Player.MaxStat;

        mAddSpeedBtn.IsHidden =
            Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int) Stat.Speed] == Options.Instance.Player.MaxStat;

        UpdateEquippedItems();
    }

    private void UpdateEquippedItems(bool updateExtraBuffs = false)
    {
        if (Globals.Me is not { } player)
        {
            return;
        }

        for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
        {
            if (player.MyEquipment[i] > -1 && player.MyEquipment[i] < Options.Instance.Player.MaxInventory)
            {
                if (player.Inventory[player.MyEquipment[i]].ItemId != Guid.Empty)
                {
                    Items[i]
                        .Update(
                            player.Inventory[player.MyEquipment[i]].ItemId,
                            player.Inventory[player.MyEquipment[i]].ItemProperties
                        );
                    if (updateExtraBuffs)
                    {
                        UpdateExtraBuffs(player.Inventory[player.MyEquipment[i]].ItemId);
                    }
                }
                else
                {
                    Items[i].Update(Guid.Empty, mItemProperties);
                }
            }
            else
            {
                Items[i].Update(Guid.Empty, mItemProperties);
            }
        }
    }

    /// <summary>
    /// Update Extra Buffs Effects like hp/mana regen and items effect types
    /// </summary>
    /// <param name="itemId">Id of item to update extra buffs</param>
    private void UpdateExtraBuffs(Guid itemId)
    {
        var item = ItemDescriptor.Get(itemId);

        if (item == null)
        {
            return;
        }

        //Getting HP and Mana Regen from items
        if (item.VitalsRegen[(int)Vital.Health] != 0)
        {
            HpRegenAmount += item.VitalsRegen[(int)Vital.Health];
        }

        if (item.VitalsRegen[(int)Vital.Mana] != 0)
        {
            ManaRegenAmount += item.VitalsRegen[(int)Vital.Mana];
        }

        //Getting extra buffs from items
        if (item.Effects.Find(effect => effect.Type != ItemEffect.None && effect.Percentage > 0) != default)
        {
            foreach (var effect in item.Effects)
            {
                if (effect.Percentage <= 0)
                {
                    continue;
                }

                switch (effect.Type)
                {
                    case ItemEffect.CooldownReduction:
                        CooldownAmount += effect.Percentage;
                        break;
                    case ItemEffect.Lifesteal:
                        LifeStealAmount += effect.Percentage;
                        break;
                    case ItemEffect.Tenacity:
                        TenacityAmount += effect.Percentage;
                        break;
                    case ItemEffect.Luck:
                        LuckAmount += effect.Percentage;
                        break;
                    case ItemEffect.EXP:
                        ExtraExpAmount += effect.Percentage;
                        break;
                    case ItemEffect.Manasteal:
                        ManaStealAmount += effect.Percentage;
                        break;
                }
            }
        }
    }

    private void UpdateExtraBuffTooltip(Base? sender, EventArgs? arguments)
    {
        //Reset all values
        HpRegenAmount = mPlayer?.VitalRegen[(int)Vital.Health] ?? 0;
        ManaRegenAmount = mPlayer?.VitalRegen[(int)Vital.Mana] ?? 0;
        CooldownAmount = 0;
        LifeStealAmount = 0;
        TenacityAmount = 0;
        LuckAmount = 0;
        ExtraExpAmount = 0;
        ManaStealAmount = 0;

        // Update extra buffs from equipped items
        UpdateEquippedItems(true);

        // Update tooltip with the current extra buffs
        var tooltip = new System.Text.StringBuilder();
        tooltip.AppendLine(Strings.Character.HealthRegen.ToString(HpRegenAmount));
        tooltip.AppendLine(Strings.Character.ManaRegen.ToString(ManaRegenAmount));
        tooltip.AppendLine(Strings.Character.Lifesteal.ToString(LifeStealAmount));
        tooltip.AppendLine(Strings.Character.AttackSpeed.ToString(Globals.Me?.CalculateAttackTime() / 1000f));
        tooltip.AppendLine(Strings.Character.ExtraExp.ToString(ExtraExpAmount));
        tooltip.AppendLine(Strings.Character.Luck.ToString(LuckAmount));
        tooltip.AppendLine(Strings.Character.Tenacity.ToString(TenacityAmount));
        tooltip.AppendLine(Strings.Character.CooldownReduction.ToString(CooldownAmount));
        tooltip.AppendLine(Strings.Character.Manasteal.ToString(ManaStealAmount));
        _detailsButton.SetToolTipText(tooltip.ToString());
    }

    /// <summary>
    /// Show the window
    /// </summary>
    public void Show()
    {
        mCharacterWindow.IsHidden = false;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns if window is visible</returns>
    public bool IsVisible()
    {
        return !mCharacterWindow.IsHidden;
    }

    /// <summary>
    /// Hide the window
    /// </summary>
    public void Hide()
    {
        mCharacterWindow.IsHidden = true;
    }

}
