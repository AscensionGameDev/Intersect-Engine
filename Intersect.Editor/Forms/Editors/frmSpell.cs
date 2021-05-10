using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Controls;
using DarkUI.Forms;

using Intersect.Editor.Content;
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

        private List<string> mKnownFolders = new List<string>();

        private List<string> mKnownCooldownGroups = new List<string>();

        public FrmSpell()
        {
            ApplyHooks();
            InitializeComponent();

            cmbScalingStat.Items.Clear();
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(i));
            }

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = SpellBase.Get(id);
            UpdateEditor();
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
            cmbWarpMap.Items.AddRange(MapList.OrderedMaps.Select(map => map?.Name).ToArray());
            cmbWarpMap.SelectedIndex = 0;

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

            nudCastDuration.Maximum = Int32.MaxValue;
            nudCooldownDuration.Maximum = Int32.MaxValue;

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
            for (var i = 0; i < Strings.SpellEditor.types.Count; i++)
            {
                cmbType.Items.Add(Strings.SpellEditor.types[i]);
            }

            lblIcon.Text = Strings.SpellEditor.icon;
            lblDesc.Text = Strings.SpellEditor.description;
            lblCastAnimation.Text = Strings.SpellEditor.castanimation;
            lblHitAnimation.Text = Strings.SpellEditor.hitanimation;
            chkBound.Text = Strings.SpellEditor.bound;

            grpRequirements.Text = Strings.SpellEditor.requirements;
            btnDynamicRequirements.Text = Strings.SpellEditor.requirementsbutton;

            grpSpellCost.Text = Strings.SpellEditor.cost;
            lblHPCost.Text = Strings.SpellEditor.hpcost;
            lblMPCost.Text = Strings.SpellEditor.manacost;
            lblCastDuration.Text = Strings.SpellEditor.casttime;
            lblCooldownDuration.Text = Strings.SpellEditor.cooldown;
            lblCooldownGroup.Text = Strings.SpellEditor.CooldownGroup;
            chkIgnoreGlobalCooldown.Text = Strings.SpellEditor.IgnoreGlobalCooldown;
            chkIgnoreCdr.Text = Strings.SpellEditor.IgnoreCooldownReduction;

            grpTargetInfo.Text = Strings.SpellEditor.targetting;
            lblTargetType.Text = Strings.SpellEditor.targettype;
            cmbTargetType.Items.Clear();
            for (var i = 0; i < Strings.SpellEditor.targettypes.Count; i++)
            {
                cmbTargetType.Items.Add(Strings.SpellEditor.targettypes[i]);
            }

            lblCastRange.Text = Strings.SpellEditor.castrange;
            lblProjectile.Text = Strings.SpellEditor.projectile;
            lblHitRadius.Text = Strings.SpellEditor.hitradius;
            lblDuration.Text = Strings.SpellEditor.duration;

            grpCombat.Text = Strings.SpellEditor.combatspell;
            grpDamage.Text = Strings.SpellEditor.damagegroup;
            lblCritChance.Text = Strings.SpellEditor.critchance;
            lblCritMultiplier.Text = Strings.SpellEditor.critmultiplier;
            lblDamageType.Text = Strings.SpellEditor.damagetype;
            lblHPDamage.Text = Strings.SpellEditor.hpdamage;
            lblManaDamage.Text = Strings.SpellEditor.mpdamage;
            chkFriendly.Text = Strings.SpellEditor.friendly;
            cmbDamageType.Items.Clear();
            for (var i = 0; i < Strings.Combat.damagetypes.Count; i++)
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
            for (var i = 0; i < Strings.SpellEditor.effects.Count; i++)
            {
                cmbExtraEffect.Items.Add(Strings.SpellEditor.effects[i]);
            }

            lblSprite.Text = Strings.SpellEditor.transformsprite;

            grpDash.Text = Strings.SpellEditor.dash;
            lblRange.Text = Strings.SpellEditor.dashrange.ToString(scrlRange.Value);
            grpDashCollisions.Text = Strings.SpellEditor.dashcollisions;
            chkIgnoreMapBlocks.Text = Strings.SpellEditor.ignoreblocks;
            chkIgnoreActiveResources.Text = Strings.SpellEditor.ignoreactiveresources;
            chkIgnoreInactiveResources.Text = Strings.SpellEditor.ignoreinactiveresources;
            chkIgnoreZDimensionBlocks.Text = Strings.SpellEditor.ignorezdimension;

            grpWarp.Text = Strings.SpellEditor.warptomap;
            lblMap.Text = Strings.Warping.map.ToString("");
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y.ToString("");
            lblWarpDir.Text = Strings.Warping.direction.ToString("");
            cmbDirection.Items.Clear();
            for (var i = -1; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }

            btnVisualMapSelector.Text = Strings.Warping.visual;

            grpEvent.Text = Strings.SpellEditor.Event;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.SpellEditor.sortchronologically;
            txtSearch.Text = Strings.SpellEditor.searchplaceholder;
            lblFolder.Text = Strings.SpellEditor.folderlabel;

            btnSave.Text = Strings.SpellEditor.save;
            btnCancel.Text = Strings.SpellEditor.cancel;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                txtDesc.Text = mEditorItem.Description;
                cmbType.SelectedIndex = (int) mEditorItem.SpellType;

                nudCastDuration.Value = mEditorItem.CastDuration;
                nudCooldownDuration.Value = mEditorItem.CooldownDuration;
                cmbCooldownGroup.SelectedItem = mEditorItem.CooldownGroup;
                chkIgnoreGlobalCooldown.Checked = mEditorItem.IgnoreGlobalCooldown;
                chkIgnoreCdr.Checked = mEditorItem.IgnoreCooldownReduction;

                cmbCastAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.CastAnimationId) + 1;
                cmbHitAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.HitAnimationId) + 1;

                chkBound.Checked = mEditorItem.Bound;

                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Icon));
                picSpell.BackgroundImage?.Dispose();
                picSpell.BackgroundImage = null;
                if (cmbSprite.SelectedIndex > 0)
                {
                    picSpell.BackgroundImage = Image.FromFile("resources/spells/" + cmbSprite.Text);
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

            if (cmbType.SelectedIndex == (int) SpellTypes.CombatSpell ||
                cmbType.SelectedIndex == (int) SpellTypes.WarpTo ||
                cmbType.SelectedIndex == (int) SpellTypes.Event)
            {
                grpTargetInfo.Show();
                grpCombat.Show();
                cmbTargetType.SelectedIndex = (int) mEditorItem.Combat.TargetType;
                UpdateTargetTypePanel();

                nudHPDamage.Value = mEditorItem.Combat.VitalDiff[(int) Vitals.Health];
                nudMPDamage.Value = mEditorItem.Combat.VitalDiff[(int) Vitals.Mana];

                nudStr.Value = mEditorItem.Combat.StatDiff[(int) Stats.Attack];
                nudDef.Value = mEditorItem.Combat.StatDiff[(int) Stats.Defense];
                nudSpd.Value = mEditorItem.Combat.StatDiff[(int) Stats.Speed];
                nudMag.Value = mEditorItem.Combat.StatDiff[(int) Stats.AbilityPower];
                nudMR.Value = mEditorItem.Combat.StatDiff[(int) Stats.MagicResist];

                nudStrPercentage.Value = mEditorItem.Combat.PercentageStatDiff[(int) Stats.Attack];
                nudDefPercentage.Value = mEditorItem.Combat.PercentageStatDiff[(int) Stats.Defense];
                nudMagPercentage.Value = mEditorItem.Combat.PercentageStatDiff[(int) Stats.AbilityPower];
                nudMRPercentage.Value = mEditorItem.Combat.PercentageStatDiff[(int) Stats.MagicResist];
                nudSpdPercentage.Value = mEditorItem.Combat.PercentageStatDiff[(int) Stats.Speed];

                chkFriendly.Checked = Convert.ToBoolean(mEditorItem.Combat.Friendly);
                cmbDamageType.SelectedIndex = mEditorItem.Combat.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.Combat.ScalingStat;
                nudScaling.Value = mEditorItem.Combat.Scaling;
                nudCritChance.Value = mEditorItem.Combat.CritChance;
                nudCritMultiplier.Value = (decimal) mEditorItem.Combat.CritMultiplier;

                chkHOTDOT.Checked = mEditorItem.Combat.HoTDoT;
                nudBuffDuration.Value = mEditorItem.Combat.Duration;
                nudTick.Value = mEditorItem.Combat.HotDotInterval;
                cmbExtraEffect.SelectedIndex = (int) mEditorItem.Combat.Effect;
                cmbExtraEffect_SelectedIndexChanged(null, null);
            }
            else if (cmbType.SelectedIndex == (int) SpellTypes.Warp)
            {
                grpWarp.Show();
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == mEditorItem.Warp.MapId)
                    {
                        cmbWarpMap.SelectedIndex = i;

                        break;
                    }
                }

                nudWarpX.Value = mEditorItem.Warp.X;
                nudWarpY.Value = mEditorItem.Warp.Y;
                cmbDirection.SelectedIndex = mEditorItem.Warp.Dir;
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

            if (cmbType.SelectedIndex == (int) SpellTypes.Event)
            {
                grpEvent.Show();
                cmbEvent.SelectedIndex = EventBase.ListIndex(mEditorItem.EventId) + 1;
            }

            if (cmbType.SelectedIndex == (int) SpellTypes.WarpTo)
            {
                grpTargetInfo.Show();
                cmbTargetType.SelectedIndex = (int) SpellTargetTypes.Single;
                cmbTargetType.Enabled = false;
                UpdateTargetTypePanel();
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
            lblDuration.Hide();
            nudDuration.Hide();

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
                cmbProjectile.SelectedIndex = ProjectileBase.ListIndex(mEditorItem.Combat.ProjectileId);
            }

            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.OnHit)
            {
                lblDuration.Show();
                nudDuration.Show();
                nudDuration.Value = mEditorItem.Combat.OnHitDuration;
            }

            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.Trap)
            {
                lblDuration.Show();
                nudDuration.Show();
                nudDuration.Value = mEditorItem.Combat.TrapDuration;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex != (int) mEditorItem.SpellType)
            {
                mEditorItem.SpellType = (SpellTypes) cmbType.SelectedIndex;
                UpdateSpellTypePanels();
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Icon = cmbSprite.Text;
            picSpell.BackgroundImage?.Dispose();
            picSpell.BackgroundImage = null;
            picSpell.BackgroundImage = cmbSprite.SelectedIndex > 0
                ? Image.FromFile("resources/spells/" + cmbSprite.Text)
                : null;
        }

        private void cmbTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.TargetType = (SpellTargetTypes) cmbTargetType.SelectedIndex;
            UpdateTargetTypePanel();
        }

        private void chkHOTDOT_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.HoTDoT = chkHOTDOT.Checked;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Description = txtDesc.Text;
        }

        private void cmbExtraEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.Effect = (StatusTypes) cmbExtraEffect.SelectedIndex;

            lblSprite.Visible = false;
            cmbTransform.Visible = false;
            picSprite.Visible = false;

            if (cmbExtraEffect.SelectedIndex == 6) //Transform
            {
                lblSprite.Visible = true;
                cmbTransform.Visible = true;
                picSprite.Visible = true;

                cmbTransform.SelectedIndex =
                    cmbTransform.FindString(TextUtils.NullToNone(mEditorItem.Combat.TransformSprite));

                if (cmbTransform.SelectedIndex > 0)
                {
                    var bmp = new Bitmap(picSprite.Width, picSprite.Height);
                    var g = Graphics.FromImage(bmp);
                    var src = Image.FromFile("resources/entities/" + cmbTransform.Text);
                    g.DrawImage(
                        src,
                        new Rectangle(
                            picSprite.Width / 2 - src.Width / (Options.Instance.Sprites.NormalFrames * 2), picSprite.Height / 2 - src.Height / (Options.Instance.Sprites.Directions * 2), src.Width / Options.Instance.Sprites.NormalFrames,
                            src.Height / Options.Instance.Sprites.Directions
                        ), new Rectangle(0, 0, src.Width / Options.Instance.Sprites.NormalFrames, src.Height / Options.Instance.Sprites.Directions), GraphicsUnit.Pixel
                    );

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
            lblRange.Text = Strings.SpellEditor.dashrange.ToString(scrlRange.Value);
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
                var bmp = new Bitmap(picSprite.Width, picSprite.Height);
                var g = Graphics.FromImage(bmp);
                var src = Image.FromFile("resources/entities/" + cmbTransform.Text);
                g.DrawImage(
                    src,
                    new Rectangle(
                        picSprite.Width / 2 - src.Width / (Options.Instance.Sprites.NormalFrames * 2), picSprite.Height / 2 - src.Height / (Options.Instance.Sprites.Directions * 2), src.Width / Options.Instance.Sprites.NormalFrames,
                        src.Height / Options.Instance.Sprites.Directions
                    ), new Rectangle(0, 0, src.Width / Options.Instance.Sprites.NormalFrames, src.Height / Options.Instance.Sprites.Directions), GraphicsUnit.Pixel
                );

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
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.SpellEditor.deleteprompt, Strings.SpellEditor.deletetitle, DarkDialogButton.YesNo,
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
                        Strings.SpellEditor.undoprompt, Strings.SpellEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
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
            var frm = new FrmDynamicRequirements(mEditorItem.CastingRequirements, RequirementType.Spell);
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
            var frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(
                MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId, (int) nudWarpX.Value, (int) nudWarpY.Value
            );

            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;
                        mEditorItem.Warp.MapId = MapList.OrderedMaps[i].MapId;

                        break;
                    }
                }

                nudWarpX.Value = frmWarpSelection.GetX();
                mEditorItem.Warp.X = frmWarpSelection.GetX();
                nudWarpY.Value = frmWarpSelection.GetY();
                mEditorItem.Warp.Y = frmWarpSelection.GetY();
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWarpMap.SelectedIndex > -1 && mEditorItem != null)
            {
                mEditorItem.Warp.MapId = MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId;
            }
        }

        private void nudWarpX_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.X = (byte) nudWarpX.Value;
        }

        private void nudWarpY_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.Y = (byte) nudWarpY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Warp.Dir = (byte) cmbDirection.SelectedIndex;
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

        private void nudStrPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.PercentageStatDiff[(int) Stats.Attack] = (int) nudStrPercentage.Value;
        }

        private void nudMagPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.PercentageStatDiff[(int) Stats.AbilityPower] = (int) nudMagPercentage.Value;
        }

        private void nudDefPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.PercentageStatDiff[(int) Stats.Defense] = (int) nudDefPercentage.Value;
        }

        private void nudMRPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.PercentageStatDiff[(int) Stats.MagicResist] = (int) nudMRPercentage.Value;
        }

        private void nudSpdPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.PercentageStatDiff[(int) Stats.Speed] = (int) nudSpdPercentage.Value;
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

        private void nudCritMultiplier_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Combat.CritMultiplier = (double) nudCritMultiplier.Value;
        }

        private void nudOnHitDuration_ValueChanged(object sender, EventArgs e)
        {
            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.OnHit)
            {
                mEditorItem.Combat.OnHitDuration = (int) nudDuration.Value;
            }

            if (cmbTargetType.SelectedIndex == (int) SpellTargetTypes.Trap)
            {
                mEditorItem.Combat.TrapDuration = (int) nudDuration.Value;
            }
        }

        private void chkBound_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Bound = chkBound.Checked;
        }

        private void btnAddCooldownGroup_Click(object sender, EventArgs e)
        {
            var cdGroupName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.SpellEditor.CooldownGroupPrompt, Strings.SpellEditor.CooldownGroupTitle, ref cdGroupName,
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

        private void cmbCooldownGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.CooldownGroup = cmbCooldownGroup.Text;
        }

        private void chkIgnoreGlobalCooldown_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreGlobalCooldown = chkIgnoreGlobalCooldown.Checked;
        }

        private void chkIgnoreCdr_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreCooldownReduction = chkIgnoreCdr.Checked;
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in SpellBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((SpellBase) itm.Value).Folder) &&
                    !mFolders.Contains(((SpellBase) itm.Value).Folder))
                {
                    mFolders.Add(((SpellBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((SpellBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((SpellBase) itm.Value).Folder);
                    }
                }

                if (!string.IsNullOrWhiteSpace(((SpellBase)itm.Value).CooldownGroup) &&
                    !mKnownCooldownGroups.Contains(((SpellBase)itm.Value).CooldownGroup))
                {
                    mKnownCooldownGroups.Add(((SpellBase)itm.Value).CooldownGroup);
                }
            }

            // Do we add item cooldown groups as well?
            if (Options.Combat.LinkSpellAndItemCooldowns)
            {
                foreach (var itm in ItemBase.Lookup)
                {
                    if (!string.IsNullOrWhiteSpace(((ItemBase)itm.Value).CooldownGroup) &&
                    !mKnownCooldownGroups.Contains(((ItemBase)itm.Value).CooldownGroup))
                    {
                        mKnownCooldownGroups.Add(((ItemBase)itm.Value).CooldownGroup);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            mKnownCooldownGroups.Sort();
            cmbCooldownGroup.Items.Clear();
            cmbCooldownGroup.Items.Add(string.Empty);
            cmbCooldownGroup.Items.AddRange(mKnownCooldownGroups.ToArray());

            var items = SpellBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((SpellBase)pair.Value)?.Name ?? Models.DatabaseObject<SpellBase>.Deleted, ((SpellBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.SpellEditor.folderprompt, Strings.SpellEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    lstGameObjects.ExpandFolder(folderName);
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
                txtSearch.Text = Strings.SpellEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.SpellEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.SpellEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.SpellEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion
    }

}
