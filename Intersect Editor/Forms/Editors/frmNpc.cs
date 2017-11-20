using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms
{
    public partial class frmNpc : EditorForm
    {
        private List<NpcBase> _changed = new List<NpcBase>();
        private byte[] _copiedItem;
        private NpcBase _editorItem;

        public frmNpc()
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
                if (_editorItem != null && !NpcBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
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
            foreach (var item in _changed)
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
            if (changingName) return;
            _editorItem =
                NpcBase.Lookup.Get<NpcBase>(Database.GameObjectIdFromList(GameObjectType.Npc, lstNpcs.SelectedIndex));
            UpdateEditor();
        }

        private void frmNpc_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.Get("general", "none"));
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbHostileNPC.Items.Clear();
            cmbHostileNPC.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
            cmbDropItem.Items.Clear();
            cmbDropItem.Items.Add(Strings.Get("general", "none"));
            cmbDropItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("npceditor", "title");
            toolStripItemNew.Text = Strings.Get("npceditor", "new");
            toolStripItemDelete.Text = Strings.Get("npceditor", "delete");
            toolStripItemCopy.Text = Strings.Get("npceditor", "copy");
            toolStripItemPaste.Text = Strings.Get("npceditor", "paste");
            toolStripItemUndo.Text = Strings.Get("npceditor", "undo");

            grpNpcs.Text = Strings.Get("npceditor", "npcs");

            grpGeneral.Text = Strings.Get("npceditor", "general");
            lblName.Text = Strings.Get("npceditor", "name");
            lblBehavior.Text = Strings.Get("npceditor", "behavior");
            cmbBehavior.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbBehavior.Items.Add(Strings.Get("npceditor", "behavior" + i));
            }
            lblPic.Text = Strings.Get("npceditor", "sprite");
            lblSpawnDuration.Text = Strings.Get("npceditor", "spawnduration");
            lblSightRange.Text = Strings.Get("npceditor", "sightrange");

            grpStats.Text = Strings.Get("npceditor", "stats");
            lblHP.Text = Strings.Get("npceditor", "hp");
            lblMana.Text = Strings.Get("npceditor", "mana");
            lblStr.Text = Strings.Get("npceditor", "attack");
            lblDef.Text = Strings.Get("npceditor", "defense");
            lblSpd.Text = Strings.Get("npceditor", "speed");
            lblMag.Text = Strings.Get("npceditor", "abilitypower");
            lblMR.Text = Strings.Get("npceditor", "magicresist");
            lblExp.Text = Strings.Get("npceditor", "exp");

            grpSpells.Text = Strings.Get("npceditor", "spells");
            lblSpell.Text = Strings.Get("npceditor", "spell");
            btnAdd.Text = Strings.Get("npceditor", "addspell");
            btnRemove.Text = Strings.Get("npceditor", "removespell");
            lblFreq.Text = Strings.Get("npceditor", "frequency");
            cmbFreq.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                cmbFreq.Items.Add(Strings.Get("npceditor", "frequency" + i));
            }

            grpNpcVsNpc.Text = Strings.Get("npceditor", "npcvsnpc");
            chkEnabled.Text = Strings.Get("npceditor", "enabled");
            chkAttackAllies.Text = Strings.Get("npceditor", "attackallies");
            lblNPC.Text = Strings.Get("npceditor", "npc");
            btnAddAggro.Text = Strings.Get("npceditor", "addhostility");
            btnRemoveAggro.Text = Strings.Get("npceditor", "removehostility");

            grpDrops.Text = Strings.Get("npceditor", "drops");
            lblDropIndex.Text = Strings.Get("npceditor", "dropindex");
            lblDropItem.Text = Strings.Get("npceditor", "dropitem");
            lblDropAmount.Text = Strings.Get("npceditor", "dropamount");
            lblDropChance.Text = Strings.Get("npceditor", "dropchance");

            grpCombat.Text = Strings.Get("npceditor", "combat");
            lblDamage.Text = Strings.Get("npceditor", "basedamage");
            lblCritChance.Text = Strings.Get("npceditor", "critchance");
            lblDamageType.Text = Strings.Get("npceditor", "damagetype");
            cmbDamageType.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbDamageType.Items.Add(Strings.Get("npceditor", "damagetype" + i));
            }
            lblScalingStat.Text = Strings.Get("npceditor", "scalingstat");
            lblScaling.Text = Strings.Get("npceditor", "scalingamount");
            lblAttackAnimation.Text = Strings.Get("npceditor", "attackanimation");

            btnSave.Text = Strings.Get("npceditor", "save");
            btnCancel.Text = Strings.Get("npceditor", "cancel");
        }

        public void InitEditor()
        {
            lstNpcs.Items.Clear();
            lstNpcs.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbBehavior.SelectedIndex = Math.Min(_editorItem.Behavior,cmbBehavior.Items.Count-1);
                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(_editorItem.Sprite));
                nudLevel.Value = _editorItem.Level;
                nudSightRange.Value = _editorItem.SightRange;
                nudSpawnDuration.Value = _editorItem.SpawnDuration;
                nudStr.Value = _editorItem.Stat[(int) Stats.Attack];
                nudMag.Value = _editorItem.Stat[(int) Stats.AbilityPower];
                nudDef.Value = _editorItem.Stat[(int) Stats.Defense];
                nudMR.Value = _editorItem.Stat[(int) Stats.MagicResist];
                nudSpd.Value = _editorItem.Stat[(int) Stats.Speed];
                nudHp.Value = _editorItem.MaxVital[(int) Vitals.Health];
                nudMana.Value = _editorItem.MaxVital[(int) Vitals.Mana];
                nudExp.Value = _editorItem.Experience;
                chkAttackAllies.Checked = _editorItem.AttackAllies;
                chkEnabled.Checked = _editorItem.NpcVsNpcEnabled;

                //Combat
                nudDamage.Value = _editorItem.Damage;
                nudCritChance.Value = _editorItem.CritChance;
                nudScaling.Value = _editorItem.Scaling;
                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.AttackAnimation) + 1;

                // Add the spells to the list
                lstSpells.Items.Clear();
                for (int i = 0; i < _editorItem.Spells.Count; i++)
                {
                    if (_editorItem.Spells[i] != -1)
                    {
                        lstSpells.Items.Add(SpellBase.GetName(_editorItem.Spells[i]));
                    }
                    else
                    {
                        lstSpells.Items.Add(Strings.Get("general", "none"));
                    }
                }
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                        _editorItem.Spells[lstSpells.SelectedIndex]);
                }
                cmbFreq.SelectedIndex = _editorItem.SpellFrequency;

                // Add the aggro NPC's to the list
                lstAggro.Items.Clear();
                for (int i = 0; i < _editorItem.AggroList.Count; i++)
                {
                    if (_editorItem.AggroList[i] != -1)
                    {
                        lstAggro.Items.Add(NpcBase.GetName(_editorItem.AggroList[i]));
                    }
                    else
                    {
                        lstAggro.Items.Add(Strings.Get("general", "none"));
                    }
                }

                nudDropIndex.Value = 1;
                UpdateDropValues();
                DrawNpcSprite();
                if (_changed.IndexOf(_editorItem) == -1)
                {
                    _changed.Add(_editorItem);
                    _editorItem.MakeBackup();
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
            changingName = true;
            _editorItem.Name = txtName.Text;
            lstNpcs.Items[Database.GameObjectListIndex(GameObjectType.Npc, _editorItem.Index)] = txtName.Text;
            changingName = false;
        }

        private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Behavior = (byte) cmbBehavior.SelectedIndex;
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Sprite = TextUtils.SanitizeNone(cmbSprite.Text);
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

        private void UpdateDropValues()
        {
            int index = (int) nudDropIndex.Value - 1;
            cmbDropItem.SelectedIndex =
                Database.GameObjectListIndex(GameObjectType.Item, _editorItem.Drops[index].ItemNum) + 1;
            nudDropAmount.Value = _editorItem.Drops[index].Amount;
            nudDropChance.Value = _editorItem.Drops[index].Chance;
        }

        private void frmNpc_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _editorItem.Spells.Add(Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex));
            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < _editorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(_editorItem.Spells[i]));
            }
            lstSpells.SelectedIndex = n;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                int i = lstSpells.SelectedIndex;
                lstSpells.Items.RemoveAt(i);
                _editorItem.Spells.RemoveAt(i);
            }
        }

        private void cmbFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.SpellFrequency = cmbFreq.SelectedIndex;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.NpcVsNpcEnabled = chkEnabled.Checked;
        }

        private void chkAttackAllies_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.AttackAllies = chkAttackAllies.Checked;
        }

        private void btnAddAggro_Click(object sender, EventArgs e)
        {
            _editorItem.AggroList.Add(Database.GameObjectIdFromList(GameObjectType.Npc, cmbHostileNPC.SelectedIndex));
            lstAggro.Items.Clear();
            for (int i = 0; i < _editorItem.AggroList.Count; i++)
            {
                if (_editorItem.AggroList[i] != -1)
                {
                    lstAggro.Items.Add(NpcBase.GetName(_editorItem.AggroList[i]));
                }
                else
                {
                    lstAggro.Items.Add(Strings.Get("general", "none"));
                }
            }
        }

        private void btnRemoveAggro_Click(object sender, EventArgs e)
        {
            if (lstAggro.SelectedIndex > -1)
            {
                int i = lstAggro.SelectedIndex;
                lstAggro.Items.RemoveAt(i);
                _editorItem.AggroList.RemoveAt(i);
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Npc);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstNpcs.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("npceditor", "deleteprompt"),
                        Strings.Get("npceditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstNpcs.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstNpcs.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("npceditor", "undoprompt"),
                        Strings.Get("npceditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    _editorItem.RestoreBackup();
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
            toolStripItemCopy.Enabled = _editorItem != null && lstNpcs.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstNpcs.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstNpcs.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstNpcs.Focused;
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
            _editorItem.AttackAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbAttackAnimation.SelectedIndex - 1);
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void cmbDropItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[(int) nudDropIndex.Value - 1].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item,
                cmbDropItem.SelectedIndex - 1);
        }

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                    _editorItem.Spells[lstSpells.SelectedIndex]);
            }
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1 && lstSpells.SelectedIndex < _editorItem.Spells.Count)
            {
                _editorItem.Spells[lstSpells.SelectedIndex] = Database.GameObjectIdFromList(GameObjectType.Spell,
                    cmbSpell.SelectedIndex);
            }

            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < _editorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(SpellBase.GetName(_editorItem.Spells[i]));
            }
            lstSpells.SelectedIndex = n;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnDuration = (int) nudSpawnDuration.Value;
        }

        private void nudSightRange_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SightRange = (int) nudSightRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Stat[(int) Stats.Attack] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Stat[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Stat[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Stat[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Stat[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[(int) nudDropIndex.Value - 1].Chance = (int) nudDropChance.Value;
        }

        private void nudHp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.MaxVital[(int) Vitals.Health] = (int) nudHp.Value;
        }

        private void nudMana_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.MaxVital[(int) Vitals.Mana] = (int) nudMana.Value;
        }

        private void nudExp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Experience = (int) nudExp.Value;
        }

        private void nudDropAmount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[(int) nudDropIndex.Value - 1].Amount = (int) nudDropAmount.Value;
        }

        private void nudDropIndex_ValueChanged(object sender, EventArgs e)
        {
            UpdateDropValues();
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Level = (int) nudLevel.Value;
        }
    }
}