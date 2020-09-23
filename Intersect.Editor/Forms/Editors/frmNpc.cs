using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmNpc : EditorForm
    {

        private List<NpcBase> mChanged = new List<NpcBase>();

        private string mCopiedItem;

        private NpcBase mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmNpc()
        {
            ApplyHooks();
            InitializeComponent();
            lstNpcs.LostFocus += itemList_FocusChanged;
            lstNpcs.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Npc)
            {
                InitEditor();
                if (mEditorItem != null && !NpcBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
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

        private void frmNpc_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.General.none);
            cmbSprite.Items.AddRange(
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity)
            );

            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(SpellBase.Names);
            cmbHostileNPC.Items.Clear();
            cmbHostileNPC.Items.AddRange(NpcBase.Names);
            cmbDropItem.Items.Clear();
            cmbDropItem.Items.Add(Strings.General.none);
            cmbDropItem.Items.AddRange(ItemBase.Names);
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(AnimationBase.Names);
            cmbOnDeathEventKiller.Items.Clear();
            cmbOnDeathEventKiller.Items.Add(Strings.General.none);
            cmbOnDeathEventKiller.Items.AddRange(EventBase.Names);
            cmbOnDeathEventParty.Items.Clear();
            cmbOnDeathEventParty.Items.Add(Strings.General.none);
            cmbOnDeathEventParty.Items.AddRange(EventBase.Names);
            cmbScalingStat.Items.Clear();
            for (var x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            nudStr.Maximum = Options.MaxStatValue;
            nudMag.Maximum = Options.MaxStatValue;
            nudDef.Maximum = Options.MaxStatValue;
            nudMR.Maximum = Options.MaxStatValue;
            nudSpd.Maximum = Options.MaxStatValue;
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.NpcEditor.title;
            toolStripItemNew.Text = Strings.NpcEditor.New;
            toolStripItemDelete.Text = Strings.NpcEditor.delete;
            toolStripItemCopy.Text = Strings.NpcEditor.copy;
            toolStripItemPaste.Text = Strings.NpcEditor.paste;
            toolStripItemUndo.Text = Strings.NpcEditor.undo;

            grpNpcs.Text = Strings.NpcEditor.npcs;

            grpGeneral.Text = Strings.NpcEditor.general;
            lblName.Text = Strings.NpcEditor.name;
            grpBehavior.Text = Strings.NpcEditor.behavior;

            lblPic.Text = Strings.NpcEditor.sprite;
            lblSpawnDuration.Text = Strings.NpcEditor.spawnduration;

            //Behavior
            lblAggressive.Text = Strings.NpcEditor.aggressive;
            lblSightRange.Text = Strings.NpcEditor.sightrange;
            lblMovement.Text = Strings.NpcEditor.movement;
            cmbMovement.Items.Clear();
            for (var i = 0; i < Strings.NpcEditor.movements.Count; i++)
            {
                cmbMovement.Items.Add(Strings.NpcEditor.movements[i]);
            }

            lblSwarm.Text = Strings.NpcEditor.swarm;
            lblFlee.Text = Strings.NpcEditor.flee;
            grpConditions.Text = Strings.NpcEditor.conditions;
            btnPlayerFriendProtectorCond.Text = Strings.NpcEditor.playerfriendprotectorconditions;
            btnAttackOnSightCond.Text = Strings.NpcEditor.attackonsightconditions;
            btnPlayerCanAttackCond.Text = Strings.NpcEditor.playercanattackconditions;
            lblFocusDamageDealer.Text = Strings.NpcEditor.focusdamagedealer;

            grpCommonEvents.Text = Strings.NpcEditor.commonevents;
            lblOnDeathEventKiller.Text = Strings.NpcEditor.ondeathevent;
            lblOnDeathEventParty.Text = Strings.NpcEditor.ondeathpartyevent;

            grpStats.Text = Strings.NpcEditor.stats;
            lblHP.Text = Strings.NpcEditor.hp;
            lblMana.Text = Strings.NpcEditor.mana;
            lblStr.Text = Strings.NpcEditor.attack;
            lblDef.Text = Strings.NpcEditor.defense;
            lblSpd.Text = Strings.NpcEditor.speed;
            lblMag.Text = Strings.NpcEditor.abilitypower;
            lblMR.Text = Strings.NpcEditor.magicresist;
            lblExp.Text = Strings.NpcEditor.exp;

            grpRegen.Text = Strings.NpcEditor.regen;
            lblHpRegen.Text = Strings.NpcEditor.hpregen;
            lblManaRegen.Text = Strings.NpcEditor.mpregen;
            lblRegenHint.Text = Strings.NpcEditor.regenhint;

            grpSpells.Text = Strings.NpcEditor.spells;
            lblSpell.Text = Strings.NpcEditor.spell;
            btnAdd.Text = Strings.NpcEditor.addspell;
            btnRemove.Text = Strings.NpcEditor.removespell;
            lblFreq.Text = Strings.NpcEditor.frequency;
            cmbFreq.Items.Clear();
            for (var i = 0; i < Strings.NpcEditor.frequencies.Count; i++)
            {
                cmbFreq.Items.Add(Strings.NpcEditor.frequencies[i]);
            }

            grpAttackSpeed.Text = Strings.NpcEditor.attackspeed;
            lblAttackSpeedModifier.Text = Strings.NpcEditor.attackspeedmodifier;
            lblAttackSpeedValue.Text = Strings.NpcEditor.attackspeedvalue;
            cmbAttackSpeedModifier.Items.Clear();
            foreach (var val in Strings.NpcEditor.attackspeedmodifiers.Values)
            {
                cmbAttackSpeedModifier.Items.Add(val.ToString());
            }

            grpNpcVsNpc.Text = Strings.NpcEditor.npcvsnpc;
            chkEnabled.Text = Strings.NpcEditor.enabled;
            chkAttackAllies.Text = Strings.NpcEditor.attackallies;
            lblNPC.Text = Strings.NpcEditor.npc;
            btnAddAggro.Text = Strings.NpcEditor.addhostility;
            btnRemoveAggro.Text = Strings.NpcEditor.removehostility;

            grpDrops.Text = Strings.NpcEditor.drops;
            lblDropItem.Text = Strings.NpcEditor.dropitem;
            lblDropAmount.Text = Strings.NpcEditor.dropamount;
            lblDropChance.Text = Strings.NpcEditor.dropchance;
            btnDropAdd.Text = Strings.NpcEditor.dropadd;
            btnDropRemove.Text = Strings.NpcEditor.dropremove;

            grpCombat.Text = Strings.NpcEditor.combat;
            lblDamage.Text = Strings.NpcEditor.basedamage;
            lblCritChance.Text = Strings.NpcEditor.critchance;
            lblCritMultiplier.Text = Strings.NpcEditor.critmultiplier;
            lblDamageType.Text = Strings.NpcEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (var i = 0; i < Strings.Combat.damagetypes.Count; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }

            lblScalingStat.Text = Strings.NpcEditor.scalingstat;
            lblScaling.Text = Strings.NpcEditor.scalingamount;
            lblAttackAnimation.Text = Strings.NpcEditor.attackanimation;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.NpcEditor.sortchronologically;
            txtSearch.Text = Strings.NpcEditor.searchplaceholder;
            lblFolder.Text = Strings.NpcEditor.folderlabel;

            btnSave.Text = Strings.NpcEditor.save;
            btnCancel.Text = Strings.NpcEditor.cancel;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Sprite));
                nudLevel.Value = mEditorItem.Level;
                nudSpawnDuration.Value = mEditorItem.SpawnDuration;

                //Behavior
                chkAggressive.Checked = mEditorItem.Aggressive;
                if (mEditorItem.Aggressive)
                {
                    btnAttackOnSightCond.Text = Strings.NpcEditor.dontattackonsightconditions;
                }
                else
                {
                    btnAttackOnSightCond.Text = Strings.NpcEditor.attackonsightconditions;
                }

                nudSightRange.Value = mEditorItem.SightRange;
                cmbMovement.SelectedIndex = Math.Min(mEditorItem.Movement, cmbMovement.Items.Count - 1);
                chkSwarm.Checked = mEditorItem.Swarm;
                nudFlee.Value = mEditorItem.FleeHealthPercentage;
                chkFocusDamageDealer.Checked = mEditorItem.FocusHighestDamageDealer;

                //Common Events
                cmbOnDeathEventKiller.SelectedIndex = EventBase.ListIndex(mEditorItem.OnDeathEventId) + 1;
                cmbOnDeathEventParty.SelectedIndex = EventBase.ListIndex(mEditorItem.OnDeathPartyEventId) + 1;

                nudStr.Value = mEditorItem.Stats[(int) Stats.Attack];
                nudMag.Value = mEditorItem.Stats[(int) Stats.AbilityPower];
                nudDef.Value = mEditorItem.Stats[(int) Stats.Defense];
                nudMR.Value = mEditorItem.Stats[(int) Stats.MagicResist];
                nudSpd.Value = mEditorItem.Stats[(int) Stats.Speed];
                nudHp.Value = mEditorItem.MaxVital[(int) Vitals.Health];
                nudMana.Value = mEditorItem.MaxVital[(int) Vitals.Mana];
                nudExp.Value = mEditorItem.Experience;
                chkAttackAllies.Checked = mEditorItem.AttackAllies;
                chkEnabled.Checked = mEditorItem.NpcVsNpcEnabled;

                //Combat
                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudCritMultiplier.Value = (decimal) mEditorItem.CritMultiplier;
                nudScaling.Value = mEditorItem.Scaling;
                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.AttackAnimationId) + 1;
                cmbAttackSpeedModifier.SelectedIndex = mEditorItem.AttackSpeedModifier;
                nudAttackSpeedValue.Value = mEditorItem.AttackSpeedValue;

                //Regen
                nudHpRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Health];
                nudMpRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Mana];

                // Add the spells to the list
                lstSpells.Items.Clear();
                for (var i = 0; i < mEditorItem.Spells.Count; i++)
                {
                    if (mEditorItem.Spells[i] != Guid.Empty)
                    {
                        lstSpells.Items.Add(SpellBase.GetName(mEditorItem.Spells[i]));
                    }
                    else
                    {
                        lstSpells.Items.Add(Strings.General.none);
                    }
                }

                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    cmbSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.Spells[lstSpells.SelectedIndex]);
                }

                cmbFreq.SelectedIndex = mEditorItem.SpellFrequency;

                // Add the aggro NPC's to the list
                lstAggro.Items.Clear();
                for (var i = 0; i < mEditorItem.AggroList.Count; i++)
                {
                    if (mEditorItem.AggroList[i] != Guid.Empty)
                    {
                        lstAggro.Items.Add(NpcBase.GetName(mEditorItem.AggroList[i]));
                    }
                    else
                    {
                        lstAggro.Items.Add(Strings.General.none);
                    }
                }

                UpdateDropValues();

                DrawNpcSprite();
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

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            if (lstNpcs.SelectedNode != null && lstNpcs.SelectedNode.Tag != null)
            {
                lstNpcs.SelectedNode.Text = txtName.Text;
            }

            mChangingName = false;
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Sprite = TextUtils.SanitizeNone(cmbSprite.Text);
            DrawNpcSprite();
        }

        private void DrawNpcSprite()
        {
            var picSpriteBmp = new Bitmap(picNpc.Width, picNpc.Height);
            var gfx = Graphics.FromImage(picSpriteBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picNpc.Width, picNpc.Height));
            if (cmbSprite.SelectedIndex > 0)
            {
                var img = Image.FromFile("resources/entities/" + cmbSprite.Text);
                gfx.DrawImage(
                    img, new Rectangle(0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions),
                    new Rectangle(0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions), GraphicsUnit.Pixel
                );

                img.Dispose();
            }

            gfx.Dispose();
            picNpc.BackgroundImage = picSpriteBmp;
        }

        private void UpdateDropValues(bool keepIndex = false)
        {
            var index = lstDrops.SelectedIndex;
            lstDrops.Items.Clear();

            var drops = mEditorItem.Drops.ToArray();
            foreach (var drop in drops)
            {
                if (ItemBase.Get(drop.ItemId) == null)
                {
                    mEditorItem.Drops.Remove(drop);
                }
            }

            for (var i = 0; i < mEditorItem.Drops.Count; i++)
            {
                if (mEditorItem.Drops[i].ItemId != Guid.Empty)
                {
                    lstDrops.Items.Add(
                        Strings.NpcEditor.dropdisplay.ToString(
                            ItemBase.GetName(mEditorItem.Drops[i].ItemId), mEditorItem.Drops[i].Quantity,
                            mEditorItem.Drops[i].Chance
                        )
                    );
                }
                else
                {
                    lstDrops.Items.Add(TextUtils.None);
                }
            }

            if (keepIndex && index < lstDrops.Items.Count)
            {
                lstDrops.SelectedIndex = index;
            }
        }

        private void frmNpc_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Spells.Add(SpellBase.IdFromList(cmbSpell.SelectedIndex));
            var n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (var i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(mEditorItem.Spells[i]));
            }

            lstSpells.SelectedIndex = n;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                var i = lstSpells.SelectedIndex;
                lstSpells.Items.RemoveAt(i);
                mEditorItem.Spells.RemoveAt(i);
            }
        }

        private void cmbFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.SpellFrequency = cmbFreq.SelectedIndex;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.NpcVsNpcEnabled = chkEnabled.Checked;
        }

        private void chkAttackAllies_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackAllies = chkAttackAllies.Checked;
        }

        private void btnAddAggro_Click(object sender, EventArgs e)
        {
            mEditorItem.AggroList.Add(NpcBase.IdFromList(cmbHostileNPC.SelectedIndex));
            lstAggro.Items.Clear();
            for (var i = 0; i < mEditorItem.AggroList.Count; i++)
            {
                if (mEditorItem.AggroList[i] != Guid.Empty)
                {
                    lstAggro.Items.Add(NpcBase.GetName(mEditorItem.AggroList[i]));
                }
                else
                {
                    lstAggro.Items.Add(Strings.General.none);
                }
            }
        }

        private void btnRemoveAggro_Click(object sender, EventArgs e)
        {
            if (lstAggro.SelectedIndex > -1)
            {
                var i = lstAggro.SelectedIndex;
                lstAggro.Items.RemoveAt(i);
                mEditorItem.AggroList.RemoveAt(i);
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Npc);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstNpcs.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.NpcEditor.deleteprompt, Strings.NpcEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstNpcs.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstNpcs.Focused)
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
                        Strings.NpcEditor.undoprompt, Strings.NpcEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstNpcs.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstNpcs.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstNpcs.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstNpcs.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
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

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.Spells[lstSpells.SelectedIndex]);
            }
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1 && lstSpells.SelectedIndex < mEditorItem.Spells.Count)
            {
                mEditorItem.Spells[lstSpells.SelectedIndex] = SpellBase.IdFromList(cmbSpell.SelectedIndex);
            }

            var n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (var i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(mEditorItem.Spells[i]));
            }

            lstSpells.SelectedIndex = n;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnDuration = (int) nudSpawnDuration.Value;
        }

        private void nudSightRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SightRange = (int) nudSightRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stats[(int) Stats.Attack] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stats[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stats[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stats[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stats[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudHp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxVital[(int) Vitals.Health] = (int) nudHp.Value;
        }

        private void nudMana_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxVital[(int) Vitals.Mana] = (int) nudMana.Value;
        }

        private void nudExp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Experience = (int) nudExp.Value;
        }

        private void cmbDropItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1 && lstDrops.SelectedIndex < mEditorItem.Drops.Count)
            {
                mEditorItem.Drops[lstDrops.SelectedIndex].ItemId = ItemBase.IdFromList(cmbDropItem.SelectedIndex - 1);
            }

            UpdateDropValues(true);
        }

        private void nudDropAmount_ValueChanged(object sender, EventArgs e)
        {
            // This should never be below 1. We shouldn't accept giving 0 items!
            nudDropAmount.Value = Math.Max(1, nudDropAmount.Value);

            if (lstDrops.SelectedIndex < lstDrops.Items.Count)
            {
                return;
            }

            mEditorItem.Drops[(int) lstDrops.SelectedIndex].Quantity = (int) nudDropAmount.Value;
            UpdateDropValues(true);
        }

        private void lstDrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                cmbDropItem.SelectedIndex = ItemBase.ListIndex(mEditorItem.Drops[lstDrops.SelectedIndex].ItemId) + 1;
                nudDropAmount.Value = mEditorItem.Drops[lstDrops.SelectedIndex].Quantity;
                nudDropChance.Value = (decimal) mEditorItem.Drops[lstDrops.SelectedIndex].Chance;
            }
        }

        private void btnDropAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Drops.Add(new NpcDrop());
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].ItemId = ItemBase.IdFromList(cmbDropItem.SelectedIndex - 1);
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Quantity = (int) nudDropAmount.Value;
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Chance = (double) nudDropChance.Value;

            UpdateDropValues();
        }

        private void btnDropRemove_Click(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                var i = lstDrops.SelectedIndex;
                lstDrops.Items.RemoveAt(i);
                mEditorItem.Drops.RemoveAt(i);
            }

            UpdateDropValues(true);
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex < lstDrops.Items.Count)
            {
                return;
            }

            mEditorItem.Drops[(int) lstDrops.SelectedIndex].Chance = (double) nudDropChance.Value;
            UpdateDropValues(true);
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Level = (int) nudLevel.Value;
        }

        private void nudHpRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalRegen[(int) Vitals.Health] = (int) nudHpRegen.Value;
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalRegen[(int) Vitals.Mana] = (int) nudMpRegen.Value;
        }

        private void chkAggressive_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Aggressive = chkAggressive.Checked;
            if (mEditorItem.Aggressive)
            {
                btnAttackOnSightCond.Text = Strings.NpcEditor.dontattackonsightconditions;
            }
            else
            {
                btnAttackOnSightCond.Text = Strings.NpcEditor.attackonsightconditions;
            }
        }

        private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Movement = (byte) cmbMovement.SelectedIndex;
        }

        private void chkSwarm_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Swarm = chkSwarm.Checked;
        }

        private void nudFlee_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.FleeHealthPercentage = (byte) nudFlee.Value;
        }

        private void btnPlayerFriendProtectorCond_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.PlayerFriendConditions, RequirementType.NpcFriend);
            frm.TopMost = true;
            frm.ShowDialog();
        }

        private void btnAttackOnSightCond_Click(object sender, EventArgs e)
        {
            if (chkAggressive.Checked)
            {
                var frm = new FrmDynamicRequirements(
                    mEditorItem.AttackOnSightConditions, RequirementType.NpcDontAttackOnSight
                );

                frm.TopMost = true;
                frm.ShowDialog();
            }
            else
            {
                var frm = new FrmDynamicRequirements(
                    mEditorItem.AttackOnSightConditions, RequirementType.NpcAttackOnSight
                );

                frm.TopMost = true;
                frm.ShowDialog();
            }
        }

        private void btnPlayerCanAttackCond_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(
                mEditorItem.PlayerCanAttackConditions, RequirementType.NpcCanBeAttacked
            );

            frm.TopMost = true;
            frm.ShowDialog();
        }

        private void cmbOnDeathEventKiller_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.OnDeathEvent = EventBase.Get(EventBase.IdFromList(cmbOnDeathEventKiller.SelectedIndex - 1));
        }

        private void cmbOnDeathEventParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.OnDeathPartyEvent = EventBase.Get(EventBase.IdFromList(cmbOnDeathEventParty.SelectedIndex - 1));
        }

        private void chkFocusDamageDealer_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.FocusHighestDamageDealer = chkFocusDamageDealer.Checked;
        }

        private void nudCritMultiplier_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritMultiplier = (double) nudCritMultiplier.Value;
        }

        private void cmbAttackSpeedModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedModifier = cmbAttackSpeedModifier.SelectedIndex;
            nudAttackSpeedValue.Enabled = cmbAttackSpeedModifier.SelectedIndex > 0;
        }

        private void nudAttackSpeedValue_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedValue = (int) nudAttackSpeedValue.Value;
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstNpcs.SelectedNode != null && lstNpcs.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstNpcs.SelectedNode.Tag;
            }

            lstNpcs.Nodes.Clear();

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in NpcBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((NpcBase) itm.Value).Folder) &&
                    !mFolders.Contains(((NpcBase) itm.Value).Folder))
                {
                    mFolders.Add(((NpcBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((NpcBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((NpcBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstNpcs.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstNpcs.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in NpcBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = NpcBase.Get(itm.Key).Folder;
                if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked && !CustomSearch())
                {
                    var folderNode = folderNodes[folder];
                    folderNode.Nodes.Add(node);
                    if (itm.Key == selectedId)
                    {
                        folderNode.Expand();
                    }
                }
                else
                {
                    lstNpcs.Nodes.Add(node);
                }

                if (CustomSearch())
                {
                    if (!node.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        node.Remove();
                    }
                }

                if (itm.Key == selectedId)
                {
                    lstNpcs.SelectedNode = node;
                }
            }

            var selectedNode = lstNpcs.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstNpcs.Sort();
            }

            lstNpcs.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }

//            searchableDarkTreeView1.ItemProvider = NpcBase.Lookup;
//            searchableDarkTreeView1?.Refresh();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.NpcEditor.folderprompt, Strings.NpcEditor.foldertitle, ref folderName, DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstNpcs_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(Guid))
                    {
                        Clipboard.SetText(e.Node.Tag.ToString());
                    }
                }

                var hitTest = lstNpcs.HitTest(e.Location);
                if (hitTest.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                        {
                            node.Collapse();
                        }
                        else
                        {
                            node.Expand();
                        }
                    }
                }

                if (node.IsExpanded)
                {
                    if (!mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        private void lstNpcs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstNpcs.SelectedNode == null || lstNpcs.SelectedNode.Tag == null)
            {
                return;
            }

            mEditorItem = NpcBase.Get((Guid) lstNpcs.SelectedNode.Tag);
            UpdateEditor();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
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
                txtSearch.Text = Strings.NpcEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.NpcEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != Strings.NpcEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.NpcEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
