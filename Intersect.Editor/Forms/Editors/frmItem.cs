using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Ranges;
using Intersect.Localization;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using static Intersect.GameObjects.EquipmentProperties;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmItem : EditorForm
    {

        private List<ItemBase> mChanged = new List<ItemBase>();

        private string mCopiedItem;

        private ItemBase mEditorItem;

        private List<string> mKnownFolders = new List<string>();

        private List<string> mKnownCooldownGroups = new List<string>();

        private bool EffectValueUpdating = false;

        public FrmItem()
        {
            ApplyHooks();
            InitializeComponent();
            Icon = Program.Icon;

            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.General.None);
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());

            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add(Strings.General.None);
            cmbProjectile.Items.AddRange(ProjectileBase.Names);

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = ItemBase.Get(id);
            UpdateEditor();
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Item)
            {
                InitEditor();
                if (mEditorItem != null && !ItemBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.Class ||
                     type == GameObjectType.Projectile ||
                     type == GameObjectType.Animation ||
                     type == GameObjectType.Spell)
            {
                frmItem_Load(null, null);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in mChanged)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            cmbPic.Items.Clear();
            cmbPic.Items.Add(Strings.General.None);

            var itemnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbWeaponSprite.Items.Clear();
            cmbWeaponSprite.Items.Add(Strings.General.None);
            cmbWeaponSprite.Items.AddRange(
                GameContentManager.GetOverridesFor(GameContentManager.TextureType.Entity, "weapon").ToArray()
            );
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.None);
            cmbAttackAnimation.Items.AddRange(AnimationBase.Names);
            cmbScalingStat.Items.Clear();
            for (var x = 0; x < Enum.GetValues<Stat>().Length; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.None);
            cmbAnimation.Items.AddRange(AnimationBase.Names);
            cmbEquipmentAnimation.Items.Clear();
            cmbEquipmentAnimation.Items.Add(Strings.General.None);
            cmbEquipmentAnimation.Items.AddRange(AnimationBase.Names);
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add(Strings.General.None);
            cmbTeachSpell.Items.AddRange(SpellBase.Names);

            var events = EventBase.Names;
            var eventElements = new List<ComboBox>() { cmbEvent, cmbOnEquip, cmbOnDrop, cmbOnPickup, cmbOnUnequip, cmbOnHit, cmbOnUse };
            foreach (var element in eventElements)
            {
                element.Items.Clear();
                element.Items.Add(Strings.General.None);
                element.Items.AddRange(events);
            }

            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add(Strings.General.None);
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add(Strings.General.None);
            var paperdollnames =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Paperdoll);

            for (var i = 0; i < paperdollnames.Length; i++)
            {
                cmbMalePaperdoll.Items.Add(paperdollnames[i]);
                cmbFemalePaperdoll.Items.Add(paperdollnames[i]);
            }

            nudStr.Maximum = Options.MaxStatValue;
            nudMag.Maximum = Options.MaxStatValue;
            nudDef.Maximum = Options.MaxStatValue;
            nudMR.Maximum = Options.MaxStatValue;
            nudSpd.Maximum = Options.MaxStatValue;

            nudStr.Minimum = -Options.MaxStatValue;
            nudMag.Minimum = -Options.MaxStatValue;
            nudDef.Minimum = -Options.MaxStatValue;
            nudMR.Minimum = -Options.MaxStatValue;
            nudSpd.Minimum = -Options.MaxStatValue;

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.ItemEditor.title;
            toolStripItemNew.Text = Strings.ItemEditor.New;
            toolStripItemDelete.Text = Strings.ItemEditor.delete;
            toolStripItemCopy.Text = Strings.ItemEditor.copy;
            toolStripItemPaste.Text = Strings.ItemEditor.paste;
            toolStripItemUndo.Text = Strings.ItemEditor.undo;

            grpItems.Text = Strings.ItemEditor.items;
            grpGeneral.Text = Strings.ItemEditor.general;
            lblName.Text = Strings.ItemEditor.name;
            lblType.Text = Strings.ItemEditor.type;
            cmbType.Items.Clear();
            for (var i = 0; i < Strings.ItemEditor.types.Count; i++)
            {
                cmbType.Items.Add(Strings.ItemEditor.types[i]);
            }

            lblDesc.Text = Strings.ItemEditor.description;
            lblPic.Text = Strings.ItemEditor.picture;
            lblRed.Text = Strings.ItemEditor.Red;
            lblGreen.Text = Strings.ItemEditor.Green;
            lblBlue.Text = Strings.ItemEditor.Blue;
            lblAlpha.Text = Strings.ItemEditor.Alpha;
            lblPrice.Text = Strings.ItemEditor.price;
            lblAnim.Text = Strings.ItemEditor.animation;
            chkCanDrop.Text = Strings.ItemEditor.CanDrop;
            lblDeathDropChance.Text = Strings.ItemEditor.DeathDropChance;
            lblDespawnTime.Text = Strings.ItemEditor.DespawnTime;
            tooltips.SetToolTip(lblDespawnTime, Strings.ItemEditor.DespawnTimeTooltip);
            tooltips.SetToolTip(nudItemDespawnTime, Strings.ItemEditor.DespawnTimeTooltip);
            chkCanBank.Text = Strings.ItemEditor.CanBank;
            chkCanGuildBank.Text = Strings.ItemEditor.CanGuildBank;
            chkCanBag.Text = Strings.ItemEditor.CanBag;
            chkCanTrade.Text = Strings.ItemEditor.CanTrade;
            chkCanSell.Text = Strings.ItemEditor.CanSell;

            grpStack.Text = Strings.ItemEditor.StackOptions;
            chkStackable.Text = Strings.ItemEditor.stackable;
            lblInvStackLimit.Text = Strings.ItemEditor.InventoryStackLimit;
            lblBankStackLimit.Text = Strings.ItemEditor.BankStackLimit;

            cmbRarity.Items.Clear();
            for (var i = 0; i < Options.Instance.Items.RarityTiers.Count; i++)
            {
                var rarityName = Options.Instance.Items.RarityTiers[i];
                cmbRarity.Items.Add(Strings.ItemEditor.rarity[rarityName]);
            }

            grpEvents.Text = Strings.ItemEditor.EventGroup;
            lblOnDrop.Text = Strings.ItemEditor.EventOnDrop;
            lblOnEquip.Text = Strings.ItemEditor.EventOnEquip;
            lblOnHit.Text = Strings.ItemEditor.EventOnHit;
            lblOnPickup.Text = Strings.ItemEditor.EventOnPickup;
            lblOnUnequip.Text = Strings.ItemEditor.EventOnUnequip;
            lblOnUse.Text = Strings.ItemEditor.EventOnUse;

            grpEquipment.Text = Strings.ItemEditor.equipment;
            lblEquipmentSlot.Text = Strings.ItemEditor.slot;
            grpStatBonuses.Text = Strings.ItemEditor.bonuses;
            lblStr.Text = Strings.ItemEditor.attackbonus;
            lblDef.Text = Strings.ItemEditor.defensebonus;
            lblSpd.Text = Strings.ItemEditor.speedbonus;
            lblMag.Text = Strings.ItemEditor.abilitypowerbonus;
            lblMR.Text = Strings.ItemEditor.magicresistbonus;
            lblEffectPercent.Text = Strings.ItemEditor.bonusamount;
            lblEquipmentAnimation.Text = Strings.ItemEditor.equipmentanimation;

            grpStatRanges.Text = Strings.ItemEditor.StatRangeTitle;
            lblStatRangeFrom.Text = Strings.ItemEditor.StatRangeFrom;
            lblStatRangeTo.Text = Strings.ItemEditor.StatRangeTo;

            grpWeaponProperties.Text = Strings.ItemEditor.weaponproperties;
            chk2Hand.Text = Strings.ItemEditor.twohanded;
            lblDamage.Text = Strings.ItemEditor.basedamage;
            lblCritChance.Text = Strings.ItemEditor.critchance;
            lblCritMultiplier.Text = Strings.ItemEditor.critmultiplier;
            lblDamageType.Text = Strings.ItemEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (var i = 0; i < Strings.Combat.damagetypes.Count; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }

            lblScalingStat.Text = Strings.ItemEditor.scalingstat;
            lblScalingAmount.Text = Strings.ItemEditor.scalingamount;
            lblAttackAnimation.Text = Strings.ItemEditor.attackanimation;
            lblSpriteAttack.Text = Strings.ItemEditor.AttackSpriteOverride;
            lblProjectile.Text = Strings.ItemEditor.projectile;
            lblToolType.Text = Strings.ItemEditor.tooltype;

            grpCooldown.Text = Strings.ItemEditor.CooldownOptions;
            lblCooldown.Text = Strings.ItemEditor.cooldown;
            lblCooldownGroup.Text = Strings.ItemEditor.CooldownGroup;
            chkIgnoreGlobalCooldown.Text = Strings.ItemEditor.IgnoreGlobalCooldown;
            chkIgnoreCdr.Text = Strings.ItemEditor.IgnoreCooldownReduction;

            grpVitalBonuses.Text = Strings.ItemEditor.vitalbonuses;
            lblHealthBonus.Text = Strings.ItemEditor.health;
            lblManaBonus.Text = Strings.ItemEditor.mana;

            grpRegen.Text = Strings.ItemEditor.regen;
            lblHpRegen.Text = Strings.ItemEditor.hpregen;
            lblManaRegen.Text = Strings.ItemEditor.mpregen;
            lblRegenHint.Text = Strings.ItemEditor.regenhint;

            grpAttackSpeed.Text = Strings.ItemEditor.attackspeed;
            lblAttackSpeedModifier.Text = Strings.ItemEditor.attackspeedmodifier;
            lblAttackSpeedValue.Text = Strings.ItemEditor.attackspeedvalue;
            cmbAttackSpeedModifier.Items.Clear();
            foreach (var val in Strings.ItemEditor.attackspeedmodifiers.Values)
            {
                cmbAttackSpeedModifier.Items.Add(val.ToString());
            }

            grpShieldProperties.Text = Strings.ItemEditor.ShieldProperties;
            lblBlockChance.Text = Strings.ItemEditor.BlockChance;
            lblBlockAmount.Text = Strings.ItemEditor.BlockAmount;
            lblBlockDmgAbs.Text = Strings.ItemEditor.BlockAbsorption;

            lblMalePaperdoll.Text = Strings.ItemEditor.malepaperdoll;
            lblFemalePaperdoll.Text = Strings.ItemEditor.femalepaperdoll;

            grpBags.Text = Strings.ItemEditor.bagpanel;
            lblBag.Text = Strings.ItemEditor.bagslots;

            grpSpell.Text = Strings.ItemEditor.spellpanel;
            lblSpell.Text = Strings.ItemEditor.spell;
            chkQuickCast.Text = Strings.ItemEditor.quickcast;
            chkSingleUseSpell.Text = Strings.ItemEditor.destroyspell;

            grpEvent.Text = Strings.ItemEditor.eventpanel;
            chkSingleUseEvent.Text = Strings.ItemEditor.SingleUseEvent;

            grpConsumable.Text = Strings.ItemEditor.consumeablepanel;
            lblVital.Text = Strings.ItemEditor.vital;
            lblInterval.Text = Strings.ItemEditor.consumeamount;
            cmbConsume.Items.Clear();
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                cmbConsume.Items.Add(Strings.Combat.vitals[i]);
            }

            cmbConsume.Items.Add(Strings.Combat.exp);

            grpRequirements.Text = Strings.ItemEditor.requirementsgroup;
            lblCannotUse.Text = Strings.ItemEditor.cannotuse;
            btnEditRequirements.Text = Strings.ItemEditor.requirements;

            //Searching/Sorting
            btnAlphabetical.ToolTipText = Strings.ItemEditor.sortalphabetically;
            txtSearch.Text = Strings.ItemEditor.searchplaceholder;
            lblFolder.Text = Strings.ItemEditor.folderlabel;

            btnSave.Text = Strings.ItemEditor.save;
            btnCancel.Text = Strings.ItemEditor.cancel;

            grpEffects.Text = Strings.ItemEditor.BonusEffectGroup;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                txtDesc.Text = mEditorItem.Description;
                cmbType.SelectedIndex = (int)mEditorItem.ItemType;
                cmbPic.SelectedIndex = cmbPic.FindString(TextUtils.NullToNone(mEditorItem.Icon));
                nudRgbaR.Value = mEditorItem.Color.R;
                nudRgbaG.Value = mEditorItem.Color.G;
                nudRgbaB.Value = mEditorItem.Color.B;
                nudRgbaA.Value = mEditorItem.Color.A;
                cmbEquipmentAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.EquipmentAnimationId) + 1;
                nudPrice.Value = mEditorItem.Price;
                cmbRarity.SelectedIndex = mEditorItem.Rarity;

                nudStr.Value = mEditorItem.StatsGiven[0];
                nudMag.Value = mEditorItem.StatsGiven[1];
                nudDef.Value = mEditorItem.StatsGiven[2];
                nudMR.Value = mEditorItem.StatsGiven[3];
                nudSpd.Value = mEditorItem.StatsGiven[4];

                nudStrPercentage.Value = mEditorItem.PercentageStatsGiven[0];
                nudMagPercentage.Value = mEditorItem.PercentageStatsGiven[1];
                nudDefPercentage.Value = mEditorItem.PercentageStatsGiven[2];
                nudMRPercentage.Value = mEditorItem.PercentageStatsGiven[3];
                nudSpdPercentage.Value = mEditorItem.PercentageStatsGiven[4];

                nudHealthBonus.Value = mEditorItem.VitalsGiven[0];
                nudManaBonus.Value = mEditorItem.VitalsGiven[1];
                nudHPPercentage.Value = mEditorItem.PercentageVitalsGiven[0];
                nudMPPercentage.Value = mEditorItem.PercentageVitalsGiven[1];
                nudHPRegen.Value = mEditorItem.VitalsRegen[0];
                nudMpRegen.Value = mEditorItem.VitalsRegen[1];

                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudCritMultiplier.Value = (decimal)mEditorItem.CritMultiplier;
                cmbAttackSpeedModifier.SelectedIndex = mEditorItem.AttackSpeedModifier;
                nudAttackSpeedValue.Value = mEditorItem.AttackSpeedValue;
                nudScaling.Value = mEditorItem.Scaling;
                // This will be removed after conversion to a per-stat editor. Reminder that pre-migration LowRange == HighRange - Day
                nudStatRangeHigh.Value = mEditorItem.StatRanges?.FirstOrDefault()?.HighRange ?? 0;
                chkCanDrop.Checked = Convert.ToBoolean(mEditorItem.CanDrop);
                chkCanBank.Checked = Convert.ToBoolean(mEditorItem.CanBank);
                chkCanGuildBank.Checked = Convert.ToBoolean(mEditorItem.CanGuildBank);
                chkCanBag.Checked = Convert.ToBoolean(mEditorItem.CanBag);
                chkCanSell.Checked = Convert.ToBoolean(mEditorItem.CanSell);
                chkCanTrade.Checked = Convert.ToBoolean(mEditorItem.CanTrade);
                chkStackable.Checked = Convert.ToBoolean(mEditorItem.Stackable);
                nudInvStackLimit.Value = mEditorItem.MaxInventoryStack;
                nudBankStackLimit.Value = mEditorItem.MaxBankStack;
                nudDeathDropChance.Value = mEditorItem.DropChanceOnDeath;
                nudItemDespawnTime.Value = mEditorItem.DespawnTime;
                cmbToolType.SelectedIndex = mEditorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.AttackAnimationId) + 1;
                cmbWeaponSprite.SelectedIndex = cmbWeaponSprite.FindString(
                        TextUtils.NullToNone(mEditorItem.WeaponSpriteOverride)
                );
                nudBlockChance.Value = mEditorItem.BlockChance;
                nudBlockAmount.Value = mEditorItem.BlockAmount;
                nudBlockDmgAbs.Value = mEditorItem.BlockAbsorption;
                RefreshExtendedData();

                chk2Hand.Checked = mEditorItem.TwoHanded;
                cmbMalePaperdoll.SelectedIndex =
                    cmbMalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.MalePaperdoll));

                cmbFemalePaperdoll.SelectedIndex =
                    cmbFemalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.FemalePaperdoll));

                if (mEditorItem.ItemType == ItemType.Consumable)
                {
                    cmbConsume.SelectedIndex = (int)mEditorItem.Consumable.Type;
                    nudInterval.Value = mEditorItem.Consumable.Value;
                    nudIntervalPercentage.Value = mEditorItem.Consumable.Percentage;
                }

                picItem.BackgroundImage?.Dispose();
                picItem.BackgroundImage = null;
                if (cmbPic.SelectedIndex > 0)
                {
                    DrawItemIcon();
                }

                picMalePaperdoll.BackgroundImage?.Dispose();
                picMalePaperdoll.BackgroundImage = null;
                if (cmbMalePaperdoll.SelectedIndex > 0)
                {
                    DrawItemPaperdoll(Gender.Male);
                }

                picFemalePaperdoll.BackgroundImage?.Dispose();
                picFemalePaperdoll.BackgroundImage = null;
                if (cmbFemalePaperdoll.SelectedIndex > 0)
                {
                    DrawItemPaperdoll(Gender.Female);
                }

                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;

                //External References
                cmbProjectile.SelectedIndex = ProjectileBase.ListIndex(mEditorItem.ProjectileId) + 1;
                cmbAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.AnimationId) + 1;

                nudCooldown.Value = mEditorItem.Cooldown;
                cmbCooldownGroup.Text = mEditorItem.CooldownGroup;
                chkIgnoreGlobalCooldown.Checked = mEditorItem.IgnoreGlobalCooldown;
                chkIgnoreCdr.Checked = mEditorItem.IgnoreCooldownReduction;

                txtCannotUse.Text = mEditorItem.CannotUseMessage;

                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }

            UpdateToolStripItems();
        }

        private void RefreshExtendedData()
        {
            grpConsumable.Visible = false;
            grpSpell.Visible = false;
            grpEquipment.Visible = false;
            grpEvent.Visible = false;
            grpBags.Visible = false;
            chkStackable.Enabled = true;

            if ((int)mEditorItem.ItemType != cmbType.SelectedIndex)
            {
                mEditorItem.Consumable.Type = ConsumableType.Health;
                mEditorItem.Consumable.Value = 0;

                mEditorItem.TwoHanded = false;
                mEditorItem.EquipmentSlot = 0;

                mEditorItem.SlotCount = 0;

                mEditorItem.Damage = 0;
                mEditorItem.Tool = -1;

                mEditorItem.Spell = null;
                mEditorItem.Event = null;
            }

            UpdateEventTriggerAvailability();

            if (cmbType.SelectedIndex == (int)ItemType.Consumable)
            {
                cmbConsume.SelectedIndex = (int)mEditorItem.Consumable.Type;
                nudInterval.Value = mEditorItem.Consumable.Value;
                nudIntervalPercentage.Value = mEditorItem.Consumable.Percentage;
                grpConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int)ItemType.Spell)
            {
                cmbTeachSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.SpellId) + 1;
                chkQuickCast.Checked = mEditorItem.QuickCast;
                chkSingleUseSpell.Checked = mEditorItem.SingleUse;
                grpSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int)ItemType.Event)
            {
                cmbEvent.SelectedIndex = EventBase.ListIndex(mEditorItem.EventId) + 1;
                chkSingleUseEvent.Checked = mEditorItem.SingleUse;
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int)ItemType.Equipment)
            {
                grpEquipment.Visible = true;
                if (mEditorItem.EquipmentSlot < -1 || mEditorItem.EquipmentSlot >= cmbEquipmentSlot.Items.Count)
                {
                    mEditorItem.EquipmentSlot = 0;
                }

                cmbEquipmentSlot.SelectedIndex = mEditorItem.EquipmentSlot;

                // Whether this item type is stackable is not up for debate.
                chkStackable.Checked = false;
                chkStackable.Enabled = false;

                RefreshBonusList();
                RefreshStatRangeList();
            }
            else if (cmbType.SelectedIndex == (int)ItemType.Bag)
            {
                // Cant have no space or negative space.
                mEditorItem.SlotCount = Math.Max(1, mEditorItem.SlotCount);
                grpBags.Visible = true;
                nudBag.Value = mEditorItem.SlotCount;

                // Whether this item type is stackable is not up for debate.
                chkStackable.Checked = false;
                chkStackable.Enabled = false;
            }
            else if (cmbType.SelectedIndex == (int)ItemType.Currency)
            {
                // Whether this item type is stackable is not up for debate.
                chkStackable.Checked = true;
                chkStackable.Enabled = false;
            }

            cmbOnDrop.SelectedIndex = EventBase.ListIndex(mEditorItem.DropEventId) + 1;
            cmbOnEquip.SelectedIndex = EventBase.ListIndex(mEditorItem.EquipEventId) + 1;
            cmbOnHit.SelectedIndex = EventBase.ListIndex(mEditorItem.OnHitEventId) + 1;
            cmbOnPickup.SelectedIndex = EventBase.ListIndex(mEditorItem.PickupEventId) + 1;
            cmbOnUnequip.SelectedIndex = EventBase.ListIndex(mEditorItem.UnequipEventId) + 1;
            cmbOnUse.SelectedIndex = EventBase.ListIndex(mEditorItem.UseEventId) + 1;

            mEditorItem.ItemType = (ItemType)cmbType.SelectedIndex;
        }

        private void UpdateEventTriggerAvailability()
        {
            cmbOnDrop.Enabled = true;
            cmbOnEquip.Enabled = true;
            cmbOnHit.Enabled = true;
            cmbOnPickup.Enabled = true;
            cmbOnUnequip.Enabled = true;
            cmbOnUse.Enabled = true;

            switch (mEditorItem.ItemType)
            {
                case ItemType.Event:
                    cmbOnUse.Enabled = false;
                    cmbOnEquip.Enabled = false;
                    cmbOnUnequip.Enabled = false;
                    cmbOnHit.Enabled = false;
                    break;
                case ItemType.Currency:
                case ItemType.Spell:
                case ItemType.Bag:
                case ItemType.Consumable:
                case ItemType.None:
                {
                    cmbOnEquip.Enabled = false;
                    cmbOnHit.Enabled = false;
                    cmbOnUnequip.Enabled = false;
                    break;
                }
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshExtendedData();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Icon = cmbPic.SelectedIndex < 1 ? null : cmbPic.Text;
            picItem.BackgroundImage?.Dispose();
            picItem.BackgroundImage = null;
            if (cmbPic.SelectedIndex > 0)
            {
                DrawItemIcon();
            }
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Type = (ConsumableType)cmbConsume.SelectedIndex;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.MalePaperdoll = TextUtils.SanitizeNone(cmbMalePaperdoll.Text);
            picMalePaperdoll.BackgroundImage?.Dispose();
            picMalePaperdoll.BackgroundImage = null;
            if (cmbMalePaperdoll.SelectedIndex > 0)
            {
                DrawItemPaperdoll(Gender.Male);
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Description = txtDesc.Text;
        }

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EquipmentSlot = cmbEquipmentSlot.SelectedIndex;
            if (cmbEquipmentSlot.SelectedIndex == Options.WeaponIndex)
            {
                grpShieldProperties.Hide();
                grpWeaponProperties.Show();
            }
            else if (cmbEquipmentSlot.SelectedIndex == Options.ShieldIndex)
            {
                grpWeaponProperties.Hide();
                grpShieldProperties.Show();
            }
            else
            {
                grpWeaponProperties.Hide();
                grpShieldProperties.Hide();

                mEditorItem.Projectile = null;
                mEditorItem.Tool = -1;
                mEditorItem.Damage = 0;
                mEditorItem.TwoHanded = false;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Tool = cmbToolType.SelectedIndex - 1;
        }

        private void cmbEquipmentBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void chk2Hand_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.TwoHanded = chk2Hand.Checked;
        }

        private void FrmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void cmbFemalePaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.FemalePaperdoll = TextUtils.SanitizeNone(cmbFemalePaperdoll.Text);
            picFemalePaperdoll.BackgroundImage?.Dispose();
            picFemalePaperdoll.BackgroundImage = null;
            if (cmbFemalePaperdoll.SelectedIndex > 0)
            {
                DrawItemPaperdoll(Gender.Female);
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Item);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ItemEditor.deleteprompt, Strings.ItemEditor.deletetitle, DarkDialogButton.YesNo,
                        Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused)
            {
                mEditorItem.Load(mCopiedItem, true);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ItemEditor.undoprompt, Strings.ItemEditor.undotitle, DarkDialogButton.YesNo,
                        Icon
                    ) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }

        private void cmbWeaponSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.WeaponSpriteOverride = TextUtils.SanitizeNone(cmbWeaponSprite?.Text);
        }

        private void cmbAttackAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackAnimation =
                AnimationBase.Get(AnimationBase.IdFromList(cmbAttackAnimation.SelectedIndex - 1));
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Projectile = ProjectileBase.Get(ProjectileBase.IdFromList(cmbProjectile.SelectedIndex - 1));
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.UsageRequirements, RequirementType.Item);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animation = AnimationBase.Get(AnimationBase.IdFromList(cmbAnimation.SelectedIndex - 1));
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Event = EventBase.Get(EventBase.IdFromList(cmbEvent.SelectedIndex - 1));
        }

        private void cmbTeachSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Spell = SpellBase.Get(SpellBase.IdFromList(cmbTeachSpell.SelectedIndex - 1));
        }

        private void nudPrice_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Price = (int)nudPrice.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int)nudScaling.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Damage = (int)nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int)nudCritChance.Value;
        }

        private void nudEffectPercent_ValueChanged(object sender, EventArgs e)
        {
            if (!IsValidBonusSelection || EffectValueUpdating)
            {
                return;
            }

            mEditorItem.SetEffectOfType(SelectedEffect, (int)nudEffectPercent.Value);
            lstBonusEffects.Items[lstBonusEffects.SelectedIndex] = GetBonusEffectRow(SelectedEffect);
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[0] = (int)nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[1] = (int)nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[2] = (int)nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[3] = (int)nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[4] = (int)nudSpd.Value;
        }

        private void nudStrPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[0] = (int)nudStrPercentage.Value;
        }

        private void nudMagPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[1] = (int)nudMagPercentage.Value;
        }

        private void nudDefPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[2] = (int)nudDefPercentage.Value;
        }

        private void nudMRPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[3] = (int)nudMRPercentage.Value;
        }

        private void nudSpdPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[4] = (int)nudSpdPercentage.Value;
        }

        private void nudBag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SlotCount = (int)nudBag.Value;
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Value = (int)nudInterval.Value;
        }

        private void nudIntervalPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Percentage = (int)nudIntervalPercentage.Value;
        }

        private void chkBound_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanDrop = chkCanDrop.Checked;
        }

        private void chkCanBank_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanBank = chkCanBank.Checked;
        }

        private void chkCanGuildBank_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanGuildBank = chkCanGuildBank.Checked;
        }

        private void chkCanBag_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanBag = chkCanBag.Checked;
        }

        private void chkCanTrade_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanTrade = chkCanTrade.Checked;
        }

        private void chkCanSell_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.CanSell = chkCanSell.Checked;
        }

        private void nudDeathDropChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.DropChanceOnDeath = (int)nudDeathDropChance.Value;
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Stackable = chkStackable.Checked;

            if (chkStackable.Checked)
            {
                nudInvStackLimit.Enabled = true;
                nudBankStackLimit.Enabled = true;
            }
            else
            {
                nudInvStackLimit.Enabled = false;
                nudInvStackLimit.Value = 1;
                nudBankStackLimit.Enabled = false;
                nudBankStackLimit.Value = 1;
            }
        }

        private void nudInvStackLimit_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxInventoryStack = (int)nudInvStackLimit.Value;
        }

        private void nudBankStackLimit_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxBankStack = (int)nudBankStackLimit.Value;
        }

        private void nudCritMultiplier_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritMultiplier = (double)nudCritMultiplier.Value;
        }

        private void nudCooldown_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Cooldown = (int)nudCooldown.Value;
        }

        private void nudHealthBonus_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsGiven[0] = (int)nudHealthBonus.Value;
        }

        private void nudManaBonus_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsGiven[1] = (int)nudManaBonus.Value;
        }

        private void nudHPPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageVitalsGiven[0] = (int)nudHPPercentage.Value;
        }

        private void nudMPPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageVitalsGiven[1] = (int)nudMPPercentage.Value;
        }

        private void nudHPRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsRegen[0] = (int)nudHPRegen.Value;
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsRegen[1] = (int)nudMpRegen.Value;
        }

        private void cmbEquipmentAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EquipmentAnimation =
                AnimationBase.Get(AnimationBase.IdFromList(cmbEquipmentAnimation.SelectedIndex - 1));
        }

        private void cmbAttackSpeedModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedModifier = cmbAttackSpeedModifier.SelectedIndex;
            nudAttackSpeedValue.Enabled = cmbAttackSpeedModifier.SelectedIndex > 0;
        }

        private void nudAttackSpeedValue_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedValue = (int)nudAttackSpeedValue.Value;
        }

        private void chkQuickCast_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.QuickCast = chkQuickCast.Checked;
        }

        private void chkSingleUse_CheckedChanged(object sender, EventArgs e)
        {
            switch ((ItemType)cmbType.SelectedIndex)
            {
                case ItemType.Spell:
                    mEditorItem.SingleUse = chkSingleUseSpell.Checked;
                    break;
                case ItemType.Event:
                    mEditorItem.SingleUse = chkSingleUseEvent.Checked;
                    break;
            }
        }

        private void cmbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Rarity = cmbRarity.SelectedIndex;
        }

        private void cmbCooldownGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.CooldownGroup = cmbCooldownGroup.Text;
        }

        private void btnAddCooldownGroup_Click(object sender, EventArgs e)
        {
            var cdGroupName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ItemEditor.CooldownGroupPrompt, Strings.ItemEditor.CooldownGroupTitle, ref cdGroupName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(cdGroupName))
            {
                if (!cmbCooldownGroup.Items.Contains(cdGroupName))
                {
                    mEditorItem.CooldownGroup = cdGroupName;
                    mKnownCooldownGroups.Add(cdGroupName);
                    InitEditor();
                    cmbCooldownGroup.Text = cdGroupName;
                }
            }
        }

        private void chkIgnoreGlobalCooldown_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreGlobalCooldown = chkIgnoreGlobalCooldown.Checked;
        }

        private void chkIgnoreCdr_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreCooldownReduction = chkIgnoreCdr.Checked;
        }

        private void nudRgbaR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Color.R = (byte)nudRgbaR.Value;
            DrawItemIcon();
            DrawItemPaperdoll(Gender.Male);
            DrawItemPaperdoll(Gender.Female);
        }

        private void nudRgbaG_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Color.G = (byte)nudRgbaG.Value;
            DrawItemIcon();
            DrawItemPaperdoll(Gender.Male);
            DrawItemPaperdoll(Gender.Female);
        }

        private void nudRgbaB_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Color.B = (byte)nudRgbaB.Value;
            DrawItemIcon();
            DrawItemPaperdoll(Gender.Male);
            DrawItemPaperdoll(Gender.Female);
        }

        private void nudRgbaA_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Color.A = (byte)nudRgbaA.Value;
            DrawItemIcon();
            DrawItemPaperdoll(Gender.Male);
            DrawItemPaperdoll(Gender.Female);
        }

        private void nudBlockChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BlockChance = (int)nudBlockChance.Value;
        }

        private void nudBlockAmount_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BlockAmount = (int)nudBlockAmount.Value;
        }

        private void nudBlockDmgAbs_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BlockAbsorption = (int)nudBlockDmgAbs.Value;
        }

        private void nudItemDespawnTime_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.DespawnTime = (long)nudItemDespawnTime.Value;
        }

        /// <summary>
        /// Draw the item Icon to the form.
        /// </summary>
        private void DrawItemIcon()
        {
            var picItemBmp = new Bitmap(picItem.Width, picItem.Height);
            var gfx = Graphics.FromImage(picItemBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picItem.Width, picItem.Height));
            if (cmbPic.SelectedIndex > 0)
            {
                var img = Image.FromFile("resources/items/" + cmbPic.Text);
                var imgAttributes = new ImageAttributes();

                // Microsoft, what the heck is this crap?
                imgAttributes.SetColorMatrix(
                    new ColorMatrix(
                        new float[][]
                        {
                            new float[] { (float)nudRgbaR.Value / 255,  0,  0,  0, 0},  // Modify the red space
                            new float[] {0, (float)nudRgbaG.Value / 255,  0,  0, 0},    // Modify the green space
                            new float[] {0,  0, (float)nudRgbaB.Value / 255,  0, 0},    // Modify the blue space
                            new float[] {0,  0,  0, (float)nudRgbaA.Value / 255, 0},    // Modify the alpha space
                            new float[] {0, 0, 0, 0, 1}                                 // We're not adding any non-linear changes. Value of 1 at the end is a dummy value!
                        }
                    )
                );

                gfx.DrawImage(
                    img, new Rectangle(0, 0, img.Width, img.Height),
                    0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes
                );

                img.Dispose();
                imgAttributes.Dispose();
            }

            gfx.Dispose();

            picItem.BackgroundImage = picItemBmp;
        }

        /// <summary>
        /// Draw the item Paperdoll to the form for the specified Gender.
        /// </summary>
        /// <param name="gender"></param>
        private void DrawItemPaperdoll(Gender gender)
        {
            PictureBox picPaperdoll;
            ComboBox cmbPaperdoll;
            switch (gender)
            {
                case Gender.Male:
                    picPaperdoll = picMalePaperdoll;
                    cmbPaperdoll = cmbMalePaperdoll;
                    break;

                case Gender.Female:
                    picPaperdoll = picFemalePaperdoll;
                    cmbPaperdoll = cmbFemalePaperdoll;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var picItemBmp = new Bitmap(picPaperdoll.Width, picPaperdoll.Height);
            var gfx = Graphics.FromImage(picItemBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picPaperdoll.Width, picPaperdoll.Height));
            if (cmbPaperdoll.SelectedIndex > 0)
            {
                var img = Image.FromFile("resources/paperdolls/" + cmbPaperdoll.Text);
                var imgAttributes = new ImageAttributes();

                // Microsoft, what the heck is this crap?
                imgAttributes.SetColorMatrix(
                    new ColorMatrix(
                        new float[][]
                        {
                            new float[] { (float)nudRgbaR.Value / 255,  0,  0,  0, 0},  // Modify the red space
                            new float[] {0, (float)nudRgbaG.Value / 255,  0,  0, 0},    // Modify the green space
                            new float[] {0,  0, (float)nudRgbaB.Value / 255,  0, 0},    // Modify the blue space
                            new float[] {0,  0,  0, (float)nudRgbaA.Value / 255, 0},    // Modify the alpha space
                            new float[] {0, 0, 0, 0, 1}                                 // We're not adding any non-linear changes. Value of 1 at the end is a dummy value!
                        }
                    )
                );

                gfx.DrawImage(
                    img, new Rectangle(0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions),
                    0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions, GraphicsUnit.Pixel, imgAttributes
                );

                img.Dispose();
                imgAttributes.Dispose();
            }

            gfx.Dispose();

            picPaperdoll.BackgroundImage = picItemBmp;
        }

        private void txtCannotUse_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.CannotUseMessage = txtCannotUse.Text;
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            //Collect folders and cooldown groups
            var mFolders = new List<string>();
            foreach (var itm in ItemBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ItemBase)itm.Value).Folder) &&
                    !mFolders.Contains(((ItemBase)itm.Value).Folder))
                {
                    mFolders.Add(((ItemBase)itm.Value).Folder);
                    if (!mKnownFolders.Contains(((ItemBase)itm.Value).Folder))
                    {
                        mKnownFolders.Add(((ItemBase)itm.Value).Folder);
                    }
                }

                if (!string.IsNullOrWhiteSpace(((ItemBase)itm.Value).CooldownGroup) &&
                    !mKnownCooldownGroups.Contains(((ItemBase)itm.Value).CooldownGroup))
                {
                    mKnownCooldownGroups.Add(((ItemBase)itm.Value).CooldownGroup);
                }
            }

            // Do we add spell cooldown groups as well?
            if (Options.Combat.LinkSpellAndItemCooldowns)
            {
                foreach (var itm in SpellBase.Lookup)
                {
                    if (!string.IsNullOrWhiteSpace(((SpellBase)itm.Value).CooldownGroup) &&
                    !mKnownCooldownGroups.Contains(((SpellBase)itm.Value).CooldownGroup))
                    {
                        mKnownCooldownGroups.Add(((SpellBase)itm.Value).CooldownGroup);
                    }
                }
            }

            mKnownCooldownGroups.Sort();
            cmbCooldownGroup.Items.Clear();
            cmbCooldownGroup.Items.Add(string.Empty);
            cmbCooldownGroup.Items.AddRange(mKnownCooldownGroups.ToArray());

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            var items = ItemBase.Lookup.OrderBy(p => p.Value?.Name).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((ItemBase)pair.Value)?.Name ?? Models.DatabaseObject<ItemBase>.Deleted, ((ItemBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnAlphabetical.Checked, CustomSearch(), txtSearch.Text);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ItemEditor.folderprompt, Strings.ItemEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    lstGameObjects.UpdateText(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnAlphabetical_Click(object sender, EventArgs e)
        {
            btnAlphabetical.Checked = !btnAlphabetical.Checked;
            InitEditor();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = Strings.ItemEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.ItemEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != Strings.ItemEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.ItemEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }


        #endregion

        private void RefreshBonusList()
        {
            lstBonusEffects.Items.Clear();
            // Skip the "none" value - we don't care about that anymore, that's legacy
            var idx = 1;
            foreach (var effectName in Strings.ItemEditor.bonuseffects.Skip(1))
            {
                lstBonusEffects.Items.Add(GetBonusEffectRow((ItemEffect)idx));
                idx++;
            }
        }

        private void RefreshStatRangeList()
        {
            lstStatRanges.Items.Clear();
            foreach (var (stat, statName) in Strings.Combat.stats)
            {
                lstStatRanges.Items.Add(GetStatRangeRowText((Stat)stat, statName));
            }
        }

        private string GetStatRangeRowText(Stat stat, LocalizedString? statName = null)
        {
            if (statName == null && !Strings.Combat.stats.TryGetValue((int)stat, out statName))
            {
                statName = Strings.General.None;
            }

            mEditorItem.TryGetRangeFor(stat, out var range);
            return Strings.ItemEditor.StatRangeItem.ToString(statName, range?.LowRange ?? 0, range?.HighRange ?? 0);
        }

        private bool IsValidBonusSelection
        {
            get => lstBonusEffects.SelectedIndex > -1 && lstBonusEffects.SelectedIndex < lstBonusEffects.Items.Count;
        }

        private ItemEffect SelectedEffect
        {
            get => IsValidBonusSelection ? (ItemEffect)(lstBonusEffects.SelectedIndex + 1) : ItemEffect.None;
        }

        private string GetBonusEffectRow(ItemEffect itemEffect)
        {
            var effectName = Strings.ItemEditor.bonuseffects[(int)itemEffect];
            var effectAmt = mEditorItem.GetEffectPercentage(itemEffect);
            return Strings.ItemEditor.BonusEffectItem.ToString(effectName, effectAmt);
        }

        private Stat? SelectedStatRange
        {
            get => Enum.IsDefined((Stat)lstStatRanges.SelectedIndex) ? (Stat)(lstStatRanges.SelectedIndex) : null;
        }

        private void lstBonusEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsValidBonusSelection)
            {
                return;
            }

            var selected = SelectedEffect;
            if (!mEditorItem.EffectsEnabled.Contains(selected))
            {
                mEditorItem.Effects.Add(new EffectData(selected, 0));
            }

            EffectValueUpdating = true;
            nudEffectPercent.Value = mEditorItem.GetEffectPercentage(selected);
            EffectValueUpdating = false;
        }

        private void nudStatRangeLow_ValueChanged(object sender, EventArgs e)
        {
            if (!SelectedStatRange.HasValue)
            {
                return;
            }

            mEditorItem.ModifyStatRangeLow(SelectedStatRange.Value, (int)nudStatRangeLow.Value);
            UpdateStatRangeRow(lstStatRanges.SelectedIndex);
            nudStatRangeLow.Focus();
        }

        private void nudStatRangeHigh_ValueChanged(object sender, EventArgs e)
        {
            if (!SelectedStatRange.HasValue)
            {
                return;
            }

            mEditorItem.ModifyStatRangeHigh(SelectedStatRange.Value, (int)nudStatRangeHigh.Value);
            UpdateStatRangeRow(lstStatRanges.SelectedIndex);
            nudStatRangeHigh.Focus();
        }

        private void UpdateStatRangeRow(int selectedIndex)
        {
            if (!SelectedStatRange.HasValue)
            {
                return;
            }

            lstStatRanges.Items[selectedIndex] = GetStatRangeRowText(SelectedStatRange.Value);
        }

        private void lstStatRanges_SelectedIndexChanged(object sender, EventArgs e)
        {
            var statSelected = lstStatRanges.SelectedIndex >= 0;
            nudStatRangeLow.Enabled = statSelected;
            nudStatRangeHigh.Enabled = statSelected;

            if (!SelectedStatRange.HasValue)
            {
                return;
            }

            if (!mEditorItem.TryGetRangeFor(SelectedStatRange.Value, out var range))
            {
                return;
            }

            nudStatRangeLow.Value = range.LowRange;
            nudStatRangeHigh.Value = range.HighRange;
        }

        private void cmbOnPickup_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.PickupEventId = EventBase.IdFromList(cmbOnPickup.SelectedIndex - 1);
        }

        private void cmbOnDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.DropEventId = EventBase.IdFromList(cmbOnDrop.SelectedIndex - 1);
        }

        private void cmbOnEquip_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EquipEventId = EventBase.IdFromList(cmbOnEquip.SelectedIndex - 1);
        }

        private void cmbOnUnequip_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.UnequipEventId = EventBase.IdFromList(cmbOnUnequip.SelectedIndex - 1);
        }

        private void cmbOnHit_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.OnHitEventId = EventBase.IdFromList(cmbOnHit.SelectedIndex - 1);
        }

        private void cmbOnUse_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.UseEventId = EventBase.IdFromList(cmbOnUse.SelectedIndex - 1);
        }
    }

}
