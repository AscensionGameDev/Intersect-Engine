using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmNpc : EditorForm
    {
        private List<NpcBase> mChanged = new List<NpcBase>();
        private string mCopiedItem;
        private NpcBase mEditorItem;

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

        private void lstNpcs_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                NpcBase.Lookup.Get<NpcBase>(Database.GameObjectIdFromList(GameObjectType.Npc, lstNpcs.SelectedIndex));
            UpdateEditor();
        }

        private void frmNpc_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.General.none);
            cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbHostileNPC.Items.Clear();
            cmbHostileNPC.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
            cmbDropItem.Items.Clear();
            cmbDropItem.Items.Add(Strings.General.none);
            cmbDropItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
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
            lblBehavior.Text = Strings.NpcEditor.behavior;
            cmbBehavior.Items.Clear();
            for (int i = 0; i < Strings.NpcEditor.behaviors.Length; i++)
            {
                cmbBehavior.Items.Add(Strings.NpcEditor.behaviors[i]);
            }
            lblPic.Text = Strings.NpcEditor.sprite;
            lblSpawnDuration.Text = Strings.NpcEditor.spawnduration;
            lblSightRange.Text = Strings.NpcEditor.sightrange;

            grpStats.Text = Strings.NpcEditor.stats;
            lblHP.Text = Strings.NpcEditor.hp;
            lblMana.Text = Strings.NpcEditor.mana;
            lblStr.Text = Strings.NpcEditor.attack;
            lblDef.Text = Strings.NpcEditor.defense;
            lblSpd.Text = Strings.NpcEditor.speed;
            lblMag.Text = Strings.NpcEditor.abilitypower;
            lblMR.Text = Strings.NpcEditor.magicresist;
            lblExp.Text = Strings.NpcEditor.exp;

            grpSpells.Text = Strings.NpcEditor.spells;
            lblSpell.Text = Strings.NpcEditor.spell;
            btnAdd.Text = Strings.NpcEditor.addspell;
            btnRemove.Text = Strings.NpcEditor.removespell;
            lblFreq.Text = Strings.NpcEditor.frequency;
            cmbFreq.Items.Clear();
            for (int i = 0; i < Strings.NpcEditor.frequencies.Length; i++)
            {
                cmbFreq.Items.Add(Strings.NpcEditor.frequencies[i]);
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
            lblDamageType.Text = Strings.NpcEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.Combat.damagetypes.Length; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.NpcEditor.scalingstat;
            lblScaling.Text = Strings.NpcEditor.scalingamount;
            lblAttackAnimation.Text = Strings.NpcEditor.attackanimation;

            btnSave.Text = Strings.NpcEditor.save;
            btnCancel.Text = Strings.NpcEditor.cancel;
        }

        public void InitEditor()
        {
            lstNpcs.Items.Clear();
            lstNpcs.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbBehavior.SelectedIndex = Math.Min(mEditorItem.Behavior, cmbBehavior.Items.Count - 1);
                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Sprite));
                nudLevel.Value = mEditorItem.Level;
                nudSightRange.Value = mEditorItem.SightRange;
                nudSpawnDuration.Value = mEditorItem.SpawnDuration;
                nudStr.Value = mEditorItem.Stat[(int)Stats.Attack];
                nudMag.Value = mEditorItem.Stat[(int)Stats.AbilityPower];
                nudDef.Value = mEditorItem.Stat[(int)Stats.Defense];
                nudMR.Value = mEditorItem.Stat[(int)Stats.MagicResist];
                nudSpd.Value = mEditorItem.Stat[(int)Stats.Speed];
                nudHp.Value = mEditorItem.MaxVital[(int)Vitals.Health];
                nudMana.Value = mEditorItem.MaxVital[(int)Vitals.Mana];
                nudExp.Value = mEditorItem.Experience;
                chkAttackAllies.Checked = mEditorItem.AttackAllies;
                chkEnabled.Checked = mEditorItem.NpcVsNpcEnabled;

                //Combat
                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudScaling.Value = mEditorItem.Scaling;
                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.AttackAnimation) + 1;

                // Add the spells to the list
                lstSpells.Items.Clear();
                for (int i = 0; i < mEditorItem.Spells.Count; i++)
                {
                    if (mEditorItem.Spells[i] != -1)
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
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                        mEditorItem.Spells[lstSpells.SelectedIndex]);
                }
                cmbFreq.SelectedIndex = mEditorItem.SpellFrequency;

                // Add the aggro NPC's to the list
                lstAggro.Items.Clear();
                for (int i = 0; i < mEditorItem.AggroList.Count; i++)
                {
                    if (mEditorItem.AggroList[i] != -1)
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
            lstNpcs.Items[Database.GameObjectListIndex(GameObjectType.Npc, mEditorItem.Index)] = txtName.Text;
            mChangingName = false;
        }

        private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Behavior = (byte)cmbBehavior.SelectedIndex;
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
                gfx.DrawImage(img, new Rectangle(0, 0, img.Width / 4, img.Height / 4),
                    new Rectangle(0, 0, img.Width / 4, img.Height / 4), GraphicsUnit.Pixel);
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
                if (ItemBase.Lookup.Get<ItemBase>(drop.ItemNum) == null) mEditorItem.Drops.Remove(drop);
            }
            for (int i = 0; i < mEditorItem.Drops.Count; i++)
            {
                if (mEditorItem.Drops[i].ItemNum != -1)
                {
                    lstDrops.Items.Add(Strings.NpcEditor.dropdisplay.ToString(ItemBase.GetName(mEditorItem.Drops[i].ItemNum), mEditorItem.Drops[i].Amount, mEditorItem.Drops[i].Chance));
                }
                else
                {
                    lstDrops.Items.Add(TextUtils.None);
                }
            }
            if (keepIndex && index < lstDrops.Items.Count) lstDrops.SelectedIndex = index;
        }

        private void frmNpc_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Spells.Add(Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex));
            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(mEditorItem.Spells[i]));
            }
            lstSpells.SelectedIndex = n;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                int i = lstSpells.SelectedIndex;
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
            mEditorItem.AggroList.Add(Database.GameObjectIdFromList(GameObjectType.Npc, cmbHostileNPC.SelectedIndex));
            lstAggro.Items.Clear();
            for (int i = 0; i < mEditorItem.AggroList.Count; i++)
            {
                if (mEditorItem.AggroList[i] != -1)
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
                int i = lstAggro.SelectedIndex;
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
                if (DarkMessageBox.ShowWarning(Strings.NpcEditor.deleteprompt,
                        Strings.NpcEditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.NpcEditor.undoprompt,
                        Strings.NpcEditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            mEditorItem.AttackAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbAttackAnimation.SelectedIndex - 1);
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
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                    mEditorItem.Spells[lstSpells.SelectedIndex]);
            }
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1 && lstSpells.SelectedIndex < mEditorItem.Spells.Count)
            {
                mEditorItem.Spells[lstSpells.SelectedIndex] = Database.GameObjectIdFromList(GameObjectType.Spell,
                    cmbSpell.SelectedIndex);
            }

            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(mEditorItem.Spells[i]));
            }
            lstSpells.SelectedIndex = n;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int)nudScaling.Value;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnDuration = (int)nudSpawnDuration.Value;
        }

        private void nudSightRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SightRange = (int)nudSightRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stat[(int)Stats.Attack] = (int)nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stat[(int)Stats.AbilityPower] = (int)nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stat[(int)Stats.Defense] = (int)nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stat[(int)Stats.MagicResist] = (int)nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Stat[(int)Stats.Speed] = (int)nudSpd.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Damage = (int)nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int)nudCritChance.Value;
        }

        private void nudHp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxVital[(int)Vitals.Health] = (int)nudHp.Value;
        }

        private void nudMana_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxVital[(int)Vitals.Mana] = (int)nudMana.Value;
        }

        private void nudExp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Experience = (int)nudExp.Value;
        }

        private void cmbDropItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1 && lstDrops.SelectedIndex < mEditorItem.Drops.Count)
            {
                mEditorItem.Drops[lstDrops.SelectedIndex].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item, cmbDropItem.SelectedIndex - 1);
            }
            UpdateDropValues(true);
        }

        private void nudDropAmount_ValueChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex < lstDrops.Items.Count) return;
            mEditorItem.Drops[(int)lstDrops.SelectedIndex].Amount = (int)nudDropAmount.Value;
            UpdateDropValues(true);
        }

        private void lstDrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                cmbDropItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mEditorItem.Drops[lstDrops.SelectedIndex].ItemNum) + 1;
                nudDropAmount.Value = mEditorItem.Drops[lstDrops.SelectedIndex].Amount;
                nudDropChance.Value = (decimal)mEditorItem.Drops[lstDrops.SelectedIndex].Chance;
            }
        }

        private void btnDropAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Drops.Add(new NpcDrop());
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item, cmbDropItem.SelectedIndex - 1);
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Amount = (int)nudDropAmount.Value;
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Chance = (double)nudDropChance.Value;

            UpdateDropValues();
        }

        private void btnDropRemove_Click(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                int i = lstDrops.SelectedIndex;
                lstDrops.Items.RemoveAt(i);
                mEditorItem.Drops.RemoveAt(i);
            }
            UpdateDropValues(true);
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex < lstDrops.Items.Count) return;
            mEditorItem.Drops[(int)lstDrops.SelectedIndex].Chance = (double)nudDropChance.Value;
            UpdateDropValues(true);
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Level = (int)nudLevel.Value;
        }
    }
}