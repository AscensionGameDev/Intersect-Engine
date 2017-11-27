using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Editor.Classes.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms
{
    public partial class FrmSpell : EditorForm
    {
        private List<SpellBase> mChanged = new List<SpellBase>();
        private byte[] mCopiedItem;
        private SpellBase mEditorItem;

        public FrmSpell()
        {
            ApplyHooks();
            InitializeComponent();
            lstSpells.LostFocus += itemList_FocusChanged;
            lstSpells.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Spell)
            {
                InitEditor();
                if (mEditorItem != null && !SpellBase.Lookup.Values.Contains(mEditorItem))
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

        private void lstSpells_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                SpellBase.Lookup.Get<SpellBase>(
                    Database.GameObjectIdFromList(GameObjectType.Spell, lstSpells.SelectedIndex));
            UpdateEditor();
        }

        private void frmSpell_Load(object sender, EventArgs e)
        {
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
            cmbCastAnimation.Items.Clear();
            cmbCastAnimation.Items.Add(Strings.general.none);
            cmbCastAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbHitAnimation.Items.Clear();
            cmbHitAnimation.Items.Add(Strings.general.none);
            cmbHitAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.general.none);
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));

            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.general.none);
            var spellNames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Spell);
            cmbSprite.Items.AddRange(spellNames);

            cmbTransform.Items.Clear();
            cmbTransform.Items.Add(Strings.general.none);
            var spriteNames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity);
            cmbTransform.Items.AddRange(spriteNames);

            nudWarpX.Maximum = (int) Options.MapWidth;
            nudWarpY.Maximum = (int) Options.MapHeight;

            cmbWarpMap.Items.Clear();
            cmbWarpMap.Items.AddRange(MapList.GetOrderedMaps().Select(map => map?.Name).ToArray());
            cmbWarpMap.SelectedIndex = 0;

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
            Text = Strings.spelleditor.title;
            toolStripItemNew.Text = Strings.spelleditor.New;
            toolStripItemDelete.Text = Strings.spelleditor.delete;
            toolStripItemCopy.Text = Strings.spelleditor.copy;
            toolStripItemPaste.Text = Strings.spelleditor.paste;
            toolStripItemUndo.Text = Strings.spelleditor.undo;

            grpSpells.Text = Strings.spelleditor.spells;

            grpGeneral.Text = Strings.spelleditor.general;
            lblName.Text = Strings.spelleditor.name;
            lblType.Text = Strings.spelleditor.type;
            cmbType.Items.Clear();
            for (int i = 0; i < Strings.spelleditor.types.Length; i++)
            {
                cmbType.Items.Add(Strings.spelleditor.types[i]);
            }
            lblIcon.Text = Strings.spelleditor.icon;
            lblDesc.Text = Strings.spelleditor.description;
            lblCastAnimation.Text = Strings.spelleditor.castanimation;
            lblHitAnimation.Text = Strings.spelleditor.hitanimation;

            grpRequirements.Text = Strings.spelleditor.requirements;
            btnDynamicRequirements.Text = Strings.spelleditor.requirementsbutton;

            grpSpellCost.Text = Strings.spelleditor.cost;
            lblHPCost.Text = Strings.spelleditor.hpcost;
            lblMPCost.Text = Strings.spelleditor.manacost;
            lblCastDuration.Text = Strings.spelleditor.casttime;
            lblCooldownDuration.Text = Strings.spelleditor.cooldown;

            grpTargetInfo.Text = Strings.spelleditor.targetting;
            lblTargetType.Text = Strings.spelleditor.targettype;
            cmbTargetType.Items.Clear();
            for (int i = 0; i < Strings.spelleditor.targettypes.Length; i++)
            {
                cmbTargetType.Items.Add(Strings.spelleditor.targettypes[i]);
            }
            lblCastRange.Text = Strings.spelleditor.castrange;
            lblProjectile.Text = Strings.spelleditor.projectile;
            lblHitRadius.Text = Strings.spelleditor.hitradius;

            grpCombat.Text = Strings.spelleditor.combatspell;
            grpDamage.Text = Strings.spelleditor.damagegroup;
            lblCritChance.Text = Strings.spelleditor.critchance;
            lblDamageType.Text = Strings.spelleditor.damagetype;
            lblHPDamage.Text = Strings.spelleditor.hpdamage;
            lblManaDamage.Text = Strings.spelleditor.mpdamage;
            chkFriendly.Text = Strings.spelleditor.friendly;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.combat.damagetypes.Length; i++)
            {
                cmbDamageType.Items.Add(Strings.combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.spelleditor.scalingstat;
            lblScaling.Text = Strings.spelleditor.scalingamount;

            grpHotDot.Text = Strings.spelleditor.hotdot;
            chkHOTDOT.Text = Strings.spelleditor.ishotdot;
            lblTick.Text = Strings.spelleditor.hotdottick;

            grpStats.Text = Strings.spelleditor.stats;
            lblStr.Text = Strings.spelleditor.attack;
            lblDef.Text = Strings.spelleditor.defense;
            lblSpd.Text = Strings.spelleditor.speed;
            lblMag.Text = Strings.spelleditor.abilitypower;
            lblMR.Text = Strings.spelleditor.magicresist;

            grpEffectDuration.Text = Strings.spelleditor.boostduration;
            lblBuffDuration.Text = Strings.spelleditor.duration;
            grpEffect.Text = Strings.spelleditor.effectgroup;
            lblEffect.Text = Strings.spelleditor.effectlabel;
            cmbExtraEffect.Items.Clear();
            for (int i = 0; i < Strings.spelleditor.effects.Length; i++)
            {
                cmbExtraEffect.Items.Add(Strings.spelleditor.effects[i]);
            }
            lblSprite.Text = Strings.spelleditor.transformsprite;

            grpDash.Text = Strings.spelleditor.dash;
            lblRange.Text = Strings.spelleditor.dashrange.ToString( scrlRange.Value);
            grpDashCollisions.Text = Strings.spelleditor.dashcollisions;
            chkIgnoreMapBlocks.Text = Strings.spelleditor.ignoreblocks;
            chkIgnoreActiveResources.Text = Strings.spelleditor.ignoreactiveresources;
            chkIgnoreInactiveResources.Text = Strings.spelleditor.ignoreinactiveresources;
            chkIgnoreZDimensionBlocks.Text = Strings.spelleditor.ignorezdimension;

            grpWarp.Text = Strings.spelleditor.warptomap;
            lblMap.Text = Strings.warping.map.ToString( "");
            lblX.Text = Strings.warping.x.ToString( "");
            lblY.Text = Strings.warping.y.ToString( "");
            lblWarpDir.Text = Strings.warping.direction.ToString( "");
            cmbDirection.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.directions.dir[i]);
            }
            btnVisualMapSelector.Text = Strings.warping.visual;

            grpEvent.Text = Strings.spelleditor.Event;
            lblEvent.Text = Strings.spelleditor.eventlabel;

            btnSave.Text = Strings.spelleditor.save;
            btnCancel.Text = Strings.spelleditor.cancel;
        }

        public void InitEditor()
        {
            lstSpells.Items.Clear();
            lstSpells.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbScalingStat.Items.Clear();
            for (var i = 0; i < Options.MaxStats; i++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(i));
            }
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                txtDesc.Text = mEditorItem.Desc;
                cmbType.SelectedIndex = mEditorItem.SpellType;

                nudCastDuration.Value = mEditorItem.CastDuration * 100;
                nudCooldownDuration.Value = mEditorItem.CooldownDuration * 100;

                cmbCastAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.CastAnimation) + 1;
                cmbHitAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.HitAnimation) + 1;

                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Pic));
                if (cmbSprite.SelectedIndex > 0)
                {
                    picSpell.BackgroundImage = Image.FromFile("resources/spells/" + cmbSprite.Text);
                }
                else
                {
                    picSpell.BackgroundImage = null;
                }
                nudHPCost.Value = mEditorItem.VitalCost[(int) Vitals.Health];
                nudMpCost.Value = mEditorItem.VitalCost[(int) Vitals.Mana];

                UpdateSpellTypePanels();
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

        private void UpdateSpellTypePanels()
        {
            grpTargetInfo.Hide();
            grpCombat.Hide();
            grpWarp.Hide();
            grpDash.Hide();
            grpEvent.Hide();
            cmbTargetType.Enabled = true;

            if (cmbType.SelectedIndex == (int) SpellTypes.CombatSpell)
            {
                grpTargetInfo.Show();
                grpCombat.Show();
                cmbTargetType.SelectedIndex = mEditorItem.TargetType;
                UpdateTargetTypePanel();

                nudHPDamage.Value = mEditorItem.VitalDiff[(int) Vitals.Health];
                nudMPDamage.Value = mEditorItem.VitalDiff[(int) Vitals.Mana];
                nudStr.Value = mEditorItem.StatDiff[(int) Stats.Attack];
                nudDef.Value = mEditorItem.StatDiff[(int) Stats.Defense];
                nudSpd.Value = mEditorItem.StatDiff[(int) Stats.Speed];
                nudMag.Value = mEditorItem.StatDiff[(int) Stats.AbilityPower];
                nudMR.Value = mEditorItem.StatDiff[(int) Stats.MagicResist];

                chkFriendly.Checked = Convert.ToBoolean(mEditorItem.Friendly);
                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;
                nudScaling.Value = mEditorItem.Scaling;
                nudCritChance.Value = mEditorItem.CritChance;

                chkHOTDOT.Checked = Convert.ToBoolean(mEditorItem.Data1);
                nudBuffDuration.Value = mEditorItem.Data2 * 100;
                nudTick.Value = mEditorItem.Data4 * 100;
                cmbExtraEffect.SelectedIndex = mEditorItem.Data3;
                cmbExtraEffect_SelectedIndexChanged(null, null);
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Warp)
            {
                grpWarp.Show();
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == mEditorItem.Data1)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                nudWarpX.Value = mEditorItem.Data2;
                nudWarpY.Value = mEditorItem.Data3;
                cmbDirection.SelectedIndex = mEditorItem.Data4;
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.WarpTo)
            {
                grpTargetInfo.Show();
                cmbTargetType.SelectedIndex = (int) SpellTargetTypes.Single;
                cmbTargetType.Enabled = false;
                UpdateTargetTypePanel();
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Dash)
            {
                grpDash.Show();
                scrlRange.Value = mEditorItem.CastRange;
                lblRange.Text = Strings.spelleditor.dashrange.ToString(scrlRange.Value);
                chkIgnoreMapBlocks.Checked = Convert.ToBoolean(mEditorItem.Data1);
                chkIgnoreActiveResources.Checked = Convert.ToBoolean(mEditorItem.Data2);
                chkIgnoreInactiveResources.Checked = Convert.ToBoolean(mEditorItem.Data3);
                chkIgnoreZDimensionBlocks.Checked = Convert.ToBoolean(mEditorItem.Data4);
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Event)
            {
                grpEvent.Show();
                cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CommonEvent, mEditorItem.Data1) +
                                         1;
            }
        }

        private void UpdateTargetTypePanel()
        {
            lblHitRadius.Hide();
            nudHitRadius.Hide();
            lblCastRange.Hide();
            nudCastRange.Hide();
            lblProjectile.Hide();
            cmbProjectile.Hide();
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.Single)
            {
                lblCastRange.Show();
                nudCastRange.Show();
                nudCastRange.Value = mEditorItem.CastRange;
                if (cmbType.SelectedIndex == (int) SpellTypes.CombatSpell)
                {
                    lblHitRadius.Show();
                    nudHitRadius.Show();
                    nudHitRadius.Value = mEditorItem.HitRadius;
                }
            }
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.AoE &&
                cmbType.SelectedIndex == (int) SpellTypes.CombatSpell)
            {
                lblHitRadius.Show();
                nudHitRadius.Show();
                nudHitRadius.Value = mEditorItem.HitRadius;
            }
            if (cmbTargetType.SelectedIndex < (int) SpellTargetTypes.Self)
            {
                lblCastRange.Show();
                nudCastRange.Show();
                nudCastRange.Value = mEditorItem.CastRange;
            }
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.Projectile)
            {
                lblProjectile.Show();
                cmbProjectile.Show();
                cmbProjectile.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Projectile, mEditorItem.Projectile);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstSpells.Items[Database.GameObjectListIndex(GameObjectType.Spell, mEditorItem.Index)] = txtName.Text;
            mChangingName = false;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex != mEditorItem.SpellType)
            {
                mEditorItem.SpellType = (byte) cmbType.SelectedIndex;
                mEditorItem.Data1 = 0;
                mEditorItem.Data2 = 0;
                mEditorItem.Data3 = 0;
                mEditorItem.Data4 = 0;
                UpdateSpellTypePanels();
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Pic = cmbSprite.Text;
            picSpell.BackgroundImage = cmbSprite.SelectedIndex > 0 ? Image.FromFile("resources/spells/" + cmbSprite.Text) : null;
        }

        private void cmbTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.TargetType = cmbTargetType.SelectedIndex;
            UpdateTargetTypePanel();
        }

        private void chkHOTDOT_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = Convert.ToInt32(chkHOTDOT.Checked);
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Desc = txtDesc.Text;
        }

        private void cmbExtraEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data3 = cmbExtraEffect.SelectedIndex;

            lblSprite.Visible = false;
            cmbTransform.Visible = false;
            picSprite.Visible = false;

            if (cmbExtraEffect.SelectedIndex == 6) //Transform
            {
                lblSprite.Visible = true;
                cmbTransform.Visible = true;
                picSprite.Visible = true;

                cmbTransform.SelectedIndex = cmbTransform.FindString(TextUtils.NullToNone(mEditorItem.Data5));
                if (cmbTransform.SelectedIndex > 0)
                {
                    Bitmap bmp = new Bitmap(picSprite.Width, picSprite.Height);
                    var g = Graphics.FromImage(bmp);
                    Image src = Image.FromFile("resources/entities/" + cmbTransform.Text);
                    g.DrawImage(src,
                        new Rectangle(picSprite.Width / 2 - src.Width / 8, picSprite.Height / 2 - src.Height / 8,
                            src.Width / 4, src.Height / 4),
                        new Rectangle(0, 0, src.Width / 4, src.Height / 4), GraphicsUnit.Pixel);
                    g.Dispose();
                    src.Dispose();
                    picSprite.BackgroundImage = bmp;
                }
                else
                {
                    picSprite.BackgroundImage = null;
                }
            }
        }

        private void frmSpell_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void scrlRange_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblRange.Text = Strings.spelleditor.dashrange.ToString( scrlRange.Value);
            mEditorItem.CastRange = scrlRange.Value;
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = Convert.ToInt32(chkIgnoreMapBlocks.Checked);
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data2 = Convert.ToInt32(chkIgnoreActiveResources.Checked);
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data3 = Convert.ToInt32(chkIgnoreInactiveResources.Checked);
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data4 = Convert.ToInt32(chkIgnoreZDimensionBlocks.Checked);
        }

        private void cmbTransform_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data5 = cmbTransform.Text;
            if (cmbTransform.SelectedIndex > 0)
            {
                Bitmap bmp = new Bitmap(picSprite.Width, picSprite.Height);
                var g = Graphics.FromImage(bmp);
                Image src = Image.FromFile("resources/entities/" + cmbTransform.Text);
                g.DrawImage(src,
                    new Rectangle(picSprite.Width / 2 - src.Width / 8, picSprite.Height / 2 - src.Height / 8,
                        src.Width / 4, src.Height / 4),
                    new Rectangle(0, 0, src.Width / 4, src.Height / 4), GraphicsUnit.Pixel);
                g.Dispose();
                src.Dispose();
                picSprite.BackgroundImage = bmp;
            }
            else
            {
                picSprite.BackgroundImage = null;
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Spell);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstSpells.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.spelleditor.deleteprompt,
                        Strings.spelleditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstSpells.Focused)
            {
                mCopiedItem = mEditorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstSpells.Focused)
            {
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.spelleditor.undoprompt,
                        Strings.spelleditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstSpells.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstSpells.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstSpells.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstSpells.Focused;
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

        private void chkFriendly_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Friendly = Convert.ToInt32(chkFriendly.Checked);
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void btnDynamicRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.CastingReqs, RequirementType.Spell);
            frm.ShowDialog();
        }

        private void cmbCastAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.CastAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbCastAnimation.SelectedIndex - 1);
        }

        private void cmbHitAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.HitAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbHitAnimation.SelectedIndex - 1);
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Projectile =
                Database.GameObjectIdFromList(GameObjectType.Projectile, cmbProjectile.SelectedIndex);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = Database.GameObjectIdFromList(GameObjectType.CommonEvent, cmbEvent.SelectedIndex - 1);
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            FrmWarpSelection frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, (int) nudWarpX.Value,
                (int) nudWarpY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                nudWarpX.Value = frmWarpSelection.GetX();
                nudWarpY.Value = frmWarpSelection.GetY();
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWarpMap.SelectedIndex > -1 && mEditorItem != null)
            {
                mEditorItem.Data1 = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
            }
        }

        private void nudWarpX_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data2 = (int) nudWarpX.Value;
        }

        private void nudWarpY_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data3 = (int) nudWarpY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data4 = cmbDirection.SelectedIndex;
        }

        private void nudCastDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CastDuration = (int) nudCastDuration.Value / 100;
        }

        private void nudCooldownDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CooldownDuration = (int) nudCooldownDuration.Value / 100;
        }

        private void nudHitRadius_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.HitRadius = (int) nudHitRadius.Value;
        }

        private void nudHPCost_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalCost[(int) Vitals.Health] = (int) nudHPCost.Value;
        }

        private void nudMpCost_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalCost[(int) Vitals.Mana] = (int) nudMpCost.Value;
        }

        private void nudHPDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalDiff[(int) Vitals.Health] = (int) nudHPDamage.Value;
        }

        private void nudMPDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalDiff[(int) Vitals.Mana] = (int) nudMPDamage.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatDiff[(int) Stats.Attack] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatDiff[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatDiff[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatDiff[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatDiff[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudBuffDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data2 = (int) nudBuffDuration.Value / 100;
        }

        private void nudTick_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data4 = (int) nudTick.Value / 100;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudCastRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CastRange = (int) nudCastRange.Value;
        }
    }
}