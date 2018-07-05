using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmSpell : EditorForm
    {
        private List<SpellBase> mChanged = new List<SpellBase>();
        private string mCopiedItem;
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
            mEditorItem = SpellBase.Get(SpellBase.IdFromList(lstSpells.SelectedIndex));
            UpdateEditor();
        }

        private void frmSpell_Load(object sender, EventArgs e)
        {
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.AddRange(ProjectileBase.Names);
            cmbCastAnimation.Items.Clear();
            cmbCastAnimation.Items.Add(Strings.General.none);
            cmbCastAnimation.Items.AddRange(AnimationBase.Names);
            cmbHitAnimation.Items.Clear();
            cmbHitAnimation.Items.Add(Strings.General.none);
            cmbHitAnimation.Items.AddRange(AnimationBase.Names);
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.General.none);
            cmbEvent.Items.AddRange(EventBase.Names);

            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.General.none);
            var spellNames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Spell);
            cmbSprite.Items.AddRange(spellNames);

            cmbTransform.Items.Clear();
            cmbTransform.Items.Add(Strings.General.none);
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
            Text = Strings.SpellEditor.title;
            toolStripItemNew.Text = Strings.SpellEditor.New;
            toolStripItemDelete.Text = Strings.SpellEditor.delete;
            toolStripItemCopy.Text = Strings.SpellEditor.copy;
            toolStripItemPaste.Text = Strings.SpellEditor.paste;
            toolStripItemUndo.Text = Strings.SpellEditor.undo;

            grpSpells.Text = Strings.SpellEditor.spells;

            grpGeneral.Text = Strings.SpellEditor.general;
            lblName.Text = Strings.SpellEditor.name;
            lblType.Text = Strings.SpellEditor.type;
            cmbType.Items.Clear();
            for (int i = 0; i < Strings.SpellEditor.types.Count; i++)
            {
                cmbType.Items.Add(Strings.SpellEditor.types[i]);
            }
            lblIcon.Text = Strings.SpellEditor.icon;
            lblDesc.Text = Strings.SpellEditor.description;
            lblCastAnimation.Text = Strings.SpellEditor.castanimation;
            lblHitAnimation.Text = Strings.SpellEditor.hitanimation;

            grpRequirements.Text = Strings.SpellEditor.requirements;
            btnDynamicRequirements.Text = Strings.SpellEditor.requirementsbutton;

            grpSpellCost.Text = Strings.SpellEditor.cost;
            lblHPCost.Text = Strings.SpellEditor.hpcost;
            lblMPCost.Text = Strings.SpellEditor.manacost;
            lblCastDuration.Text = Strings.SpellEditor.casttime;
            lblCooldownDuration.Text = Strings.SpellEditor.cooldown;

            grpTargetInfo.Text = Strings.SpellEditor.targetting;
            lblTargetType.Text = Strings.SpellEditor.targettype;
            cmbTargetType.Items.Clear();
            for (int i = 0; i < Strings.SpellEditor.targettypes.Count; i++)
            {
                cmbTargetType.Items.Add(Strings.SpellEditor.targettypes[i]);
            }
            lblCastRange.Text = Strings.SpellEditor.castrange;
            lblProjectile.Text = Strings.SpellEditor.projectile;
            lblHitRadius.Text = Strings.SpellEditor.hitradius;

            grpCombat.Text = Strings.SpellEditor.combatspell;
            grpDamage.Text = Strings.SpellEditor.damagegroup;
            lblCritChance.Text = Strings.SpellEditor.critchance;
            lblDamageType.Text = Strings.SpellEditor.damagetype;
            lblHPDamage.Text = Strings.SpellEditor.hpdamage;
            lblManaDamage.Text = Strings.SpellEditor.mpdamage;
            chkFriendly.Text = Strings.SpellEditor.friendly;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.Combat.damagetypes.Count; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.SpellEditor.scalingstat;
            lblScaling.Text = Strings.SpellEditor.scalingamount;

            grpHotDot.Text = Strings.SpellEditor.hotdot;
            chkHOTDOT.Text = Strings.SpellEditor.ishotdot;
            lblTick.Text = Strings.SpellEditor.hotdottick;

            grpStats.Text = Strings.SpellEditor.stats;
            lblStr.Text = Strings.SpellEditor.attack;
            lblDef.Text = Strings.SpellEditor.defense;
            lblSpd.Text = Strings.SpellEditor.speed;
            lblMag.Text = Strings.SpellEditor.abilitypower;
            lblMR.Text = Strings.SpellEditor.magicresist;

            grpEffectDuration.Text = Strings.SpellEditor.boostduration;
            lblBuffDuration.Text = Strings.SpellEditor.duration;
            grpEffect.Text = Strings.SpellEditor.effectgroup;
            lblEffect.Text = Strings.SpellEditor.effectlabel;
            cmbExtraEffect.Items.Clear();
            for (int i = 0; i < Strings.SpellEditor.effects.Count; i++)
            {
                cmbExtraEffect.Items.Add(Strings.SpellEditor.effects[i]);
            }
            lblSprite.Text = Strings.SpellEditor.transformsprite;

            grpDash.Text = Strings.SpellEditor.dash;
            lblRange.Text = Strings.SpellEditor.dashrange.ToString( scrlRange.Value);
            grpDashCollisions.Text = Strings.SpellEditor.dashcollisions;
            chkIgnoreMapBlocks.Text = Strings.SpellEditor.ignoreblocks;
            chkIgnoreActiveResources.Text = Strings.SpellEditor.ignoreactiveresources;
            chkIgnoreInactiveResources.Text = Strings.SpellEditor.ignoreinactiveresources;
            chkIgnoreZDimensionBlocks.Text = Strings.SpellEditor.ignorezdimension;

            grpWarp.Text = Strings.SpellEditor.warptomap;
            lblMap.Text = Strings.Warping.map.ToString( "");
            lblX.Text = Strings.Warping.x.ToString( "");
            lblY.Text = Strings.Warping.y.ToString( "");
            lblWarpDir.Text = Strings.Warping.direction.ToString( "");
            cmbDirection.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }
            btnVisualMapSelector.Text = Strings.Warping.visual;

            grpEvent.Text = Strings.SpellEditor.Event;
            lblEvent.Text = Strings.SpellEditor.eventlabel;

            btnSave.Text = Strings.SpellEditor.save;
            btnCancel.Text = Strings.SpellEditor.cancel;
        }

        public void InitEditor()
        {
            lstSpells.Items.Clear();
            lstSpells.Items.AddRange(SpellBase.Names);
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

                nudCastDuration.Value = mEditorItem.CastDuration;
                nudCooldownDuration.Value = mEditorItem.CooldownDuration;

                cmbCastAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.CastAnimationId) + 1;
                cmbHitAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.HitAnimationId) + 1;

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
                cmbTargetType.SelectedIndex = mEditorItem.Combat.TargetType;
                UpdateTargetTypePanel();

                nudHPDamage.Value = mEditorItem.Combat.VitalDiff[(int) Vitals.Health];
                nudMPDamage.Value = mEditorItem.Combat.VitalDiff[(int) Vitals.Mana];
                nudStr.Value = mEditorItem.Combat.StatDiff[(int) Stats.Attack];
                nudDef.Value = mEditorItem.Combat.StatDiff[(int) Stats.Defense];
                nudSpd.Value = mEditorItem.Combat.StatDiff[(int) Stats.Speed];
                nudMag.Value = mEditorItem.Combat.StatDiff[(int) Stats.AbilityPower];
                nudMR.Value = mEditorItem.Combat.StatDiff[(int) Stats.MagicResist];

                chkFriendly.Checked = Convert.ToBoolean(mEditorItem.Combat.Friendly);
                cmbDamageType.SelectedIndex = mEditorItem.Combat.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.Combat.ScalingStat;
                nudScaling.Value = mEditorItem.Combat.Scaling;
                nudCritChance.Value = mEditorItem.Combat.CritChance;

                chkHOTDOT.Checked = mEditorItem.Combat.HoTDoT;
                nudBuffDuration.Value = mEditorItem.Combat.Duration;
                nudTick.Value = mEditorItem.Combat.HotDotInterval;
                cmbExtraEffect.SelectedIndex = (int)mEditorItem.Combat.Effect;
                cmbExtraEffect_SelectedIndexChanged(null, null);
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Warp)
            {
                grpWarp.Show();
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapId == mEditorItem.Warp.MapId)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                nudWarpX.Value = mEditorItem.Warp.X;
                nudWarpY.Value = mEditorItem.Warp.Y;
                cmbDirection.SelectedIndex = mEditorItem.Warp.Dir;
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
                scrlRange.Value = mEditorItem.Combat.CastRange;
                lblRange.Text = Strings.SpellEditor.dashrange.ToString(scrlRange.Value);
                chkIgnoreMapBlocks.Checked = mEditorItem.Dash.IgnoreMapBlocks;
                chkIgnoreActiveResources.Checked = mEditorItem.Dash.IgnoreActiveResources;
                chkIgnoreInactiveResources.Checked = mEditorItem.Dash.IgnoreInactiveResources;
                chkIgnoreZDimensionBlocks.Checked = mEditorItem.Dash.IgnoreZDimensionAttributes;
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Event)
            {
                grpEvent.Show();
                cmbEvent.SelectedIndex = EventBase.ListIndex(mEditorItem.EventId) + 1;
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
                nudCastRange.Value = mEditorItem.Combat.CastRange;
                if (cmbType.SelectedIndex == (int) SpellTypes.CombatSpell)
                {
                    lblHitRadius.Show();
                    nudHitRadius.Show();
                    nudHitRadius.Value = mEditorItem.Combat.HitRadius;
                }
            }
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.AoE &&
                cmbType.SelectedIndex == (int) SpellTypes.CombatSpell)
            {
                lblHitRadius.Show();
                nudHitRadius.Show();
                nudHitRadius.Value = mEditorItem.Combat.HitRadius;
            }
            if (cmbTargetType.SelectedIndex < (int) SpellTargetTypes.Self)
            {
                lblCastRange.Show();
                nudCastRange.Show();
                nudCastRange.Value = mEditorItem.Combat.CastRange;
            }
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.Projectile)
            {
                lblProjectile.Show();
                cmbProjectile.Show();
                cmbProjectile.SelectedIndex =  ProjectileBase.ListIndex(mEditorItem.Combat.ProjectileId);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstSpells.Items[SpellBase.ListIndex(mEditorItem.Id)] = txtName.Text;
            mChangingName = false;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex != mEditorItem.SpellType)
            {
                mEditorItem.SpellType = (byte) cmbType.SelectedIndex;
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
            mEditorItem.Combat.TargetType = cmbTargetType.SelectedIndex;
            UpdateTargetTypePanel();
        }

        private void chkHOTDOT_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.HoTDoT = chkHOTDOT.Checked;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Desc = txtDesc.Text;
        }

        private void cmbExtraEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.Effect = (StatusTypes)cmbExtraEffect.SelectedIndex;

            lblSprite.Visible = false;
            cmbTransform.Visible = false;
            picSprite.Visible = false;

            if (cmbExtraEffect.SelectedIndex == 6) //Transform
            {
                lblSprite.Visible = true;
                cmbTransform.Visible = true;
                picSprite.Visible = true;

                cmbTransform.SelectedIndex = cmbTransform.FindString(TextUtils.NullToNone(mEditorItem.Combat.TransformSprite));
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
            lblRange.Text = Strings.SpellEditor.dashrange.ToString( scrlRange.Value);
            mEditorItem.Combat.CastRange = scrlRange.Value;
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Dash.IgnoreMapBlocks = chkIgnoreMapBlocks.Checked;
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Dash.IgnoreActiveResources = chkIgnoreActiveResources.Checked;
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Dash.IgnoreInactiveResources = chkIgnoreInactiveResources.Checked;
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Dash.IgnoreZDimensionAttributes = chkIgnoreZDimensionBlocks.Checked;
        }

        private void cmbTransform_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.TransformSprite = cmbTransform.Text;
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
                if (DarkMessageBox.ShowWarning(Strings.SpellEditor.deleteprompt,
                        Strings.SpellEditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
                mCopiedItem = mEditorItem.JsonData;
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
                if (DarkMessageBox.ShowWarning(Strings.SpellEditor.undoprompt,
                        Strings.SpellEditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            mEditorItem.Combat.Friendly = chkFriendly.Checked;
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void btnDynamicRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.CastingReqs, RequirementType.Spell);
            frm.ShowDialog();
        }

        private void cmbCastAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.CastAnimation = AnimationBase.Get(AnimationBase.IdFromList(cmbCastAnimation.SelectedIndex - 1));
        }

        private void cmbHitAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.HitAnimation = AnimationBase.Get(AnimationBase.IdFromList(cmbHitAnimation.SelectedIndex - 1));
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.ProjectileId = ProjectileBase.IdFromList(cmbProjectile.SelectedIndex);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EventId = EventBase.IdFromList(cmbEvent.SelectedIndex - 1);
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            FrmWarpSelection frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapId, (int) nudWarpX.Value,
                (int) nudWarpY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapId == frmWarpSelection.GetMap())
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
                mEditorItem.Warp.MapId = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapId;
            }
        }

        private void nudWarpX_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.X = (int) nudWarpX.Value;
        }

        private void nudWarpY_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.Y = (int) nudWarpY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.Dir = (byte)cmbDirection.SelectedIndex;
        }

        private void nudCastDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CastDuration = (int) nudCastDuration.Value;
        }

        private void nudCooldownDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CooldownDuration = (int) nudCooldownDuration.Value;
        }

        private void nudHitRadius_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.HitRadius = (int) nudHitRadius.Value;
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
            mEditorItem.Combat.VitalDiff[(int) Vitals.Health] = (int) nudHPDamage.Value;
        }

        private void nudMPDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.VitalDiff[(int) Vitals.Mana] = (int) nudMPDamage.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.StatDiff[(int) Stats.Attack] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.StatDiff[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.StatDiff[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.StatDiff[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.StatDiff[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudBuffDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.Duration = (int) nudBuffDuration.Value;
        }

        private void nudTick_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.HotDotInterval = (int) nudTick.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.CritChance = (int) nudCritChance.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.Scaling = (int) nudScaling.Value;
        }

        private void nudCastRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.CastRange = (int) nudCastRange.Value;
        }
    }
}