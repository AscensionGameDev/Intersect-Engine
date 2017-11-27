using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    public partial class FrmClass : EditorForm
    {
        private List<ClassBase> mChanged = new List<ClassBase>();
        private byte[] mCopiedItem;
        private ClassBase mEditorItem;


        public FrmClass()
        {
            ApplyHooks();
            InitializeComponent();
            lstClasses.LostFocus += itemList_FocusChanged;
            lstClasses.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Class)
            {
                InitEditor();
                if (mEditorItem != null && !ClassBase.Lookup.Values.Contains(mEditorItem))
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

        private void lstClasses_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                ClassBase.Lookup.Get<ClassBase>(
                    Database.GameObjectIdFromList(GameObjectType.Class, lstClasses.SelectedIndex));
            UpdateEditor();
        }

        private void scrlDropIndex_Scroll(object sender, ScrollValueEventArgs e)
        {
            UpdateDropValues();
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value;
            lblDropIndex.Text = Strings.ClassEditor.itemindex.ToString( index + 1);
            cmbItem.SelectedIndex =
                Database.GameObjectListIndex(GameObjectType.Item, mEditorItem.Items[index].ItemNum) + 1;
            nudItemAmount.Value = mEditorItem.Items[index].Amount;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstClasses.Items[Database.GameObjectListIndex(GameObjectType.Class, mEditorItem.Index)] = txtName.Text;
            mChangingName = false;
        }

        private void UpdateSpellList(bool keepIndex = true)
        {
            // Refresh List
            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(Strings.ClassEditor.spellitem.ToString(i + 1,
                    SpellBase.GetName(mEditorItem.Spells[i].SpellNum), mEditorItem.Spells[i].Level));
            }
            if (keepIndex) lstSpells.SelectedIndex = n;
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            var n = new ClassSpell
            {
                SpellNum = Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex),
                Level = (int) nudLevel.Value
            };

            mEditorItem.Spells.Add(n);
            UpdateSpellList(false);
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex == -1) return;
            mEditorItem.Spells.RemoveAt(lstSpells.SelectedIndex);
            lstSpells.Items.RemoveAt(lstSpells.SelectedIndex);

            UpdateSpellList(false);

            if (lstSpells.Items.Count > 0)
            {
                lstSpells.SelectedIndex = 0;
            }
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();
                txtName.Text = mEditorItem.Name;
                nudAttack.Value = mEditorItem.BaseStat[(int) Stats.Attack];
                nudMag.Value = mEditorItem.BaseStat[(int) Stats.AbilityPower];
                nudDef.Value = mEditorItem.BaseStat[(int) Stats.Defense];
                nudMR.Value = mEditorItem.BaseStat[(int) Stats.MagicResist];
                nudSpd.Value = mEditorItem.BaseStat[(int) Stats.Speed];
                nudBaseHP.Value = Math.Max(Math.Min(mEditorItem.BaseVital[(int) Vitals.Health], nudBaseHP.Maximum),
                    nudBaseHP.Minimum);
                nudBaseMana.Value = mEditorItem.BaseVital[(int) Vitals.Mana];
                nudPoints.Value = mEditorItem.BasePoints;
                chkLocked.Checked = Convert.ToBoolean(mEditorItem.Locked);

                //Combat
                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudScaling.Value = mEditorItem.Scaling;
                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.AttackAnimation) + 1;

                //Regen
                nudHPRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Health];
                nudMpRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Mana];

                //Exp
                nudBaseExp.Value = mEditorItem.BaseExp;
                nudExpIncrease.Value = mEditorItem.ExpIncrease;

                //Stat Increases
                if (mEditorItem.IncreasePercentage == 0)
                {
                    rdoStaticIncrease.Checked = true;
                }
                else
                {
                    rdoPercentageIncrease.Checked = true;
                }

                UpdateIncreases();

                UpdateSpellList(false);
                
                cmbSpell.SelectedIndex = -1;

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                        mEditorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                    nudLevel.Value = mEditorItem.Spells[lstSpells.SelectedIndex].Level;
                }
                else
                {
                    cmbSpell.SelectedIndex = -1;
                    nudLevel.Value = 0;
                }

                RefreshSpriteList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSprites.Items.Count > 0)
                {
                    lstSprites.SelectedIndex = 0;
                    cmbSprite.SelectedIndex =
                        cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Sprite));
                    if (mEditorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
                    {
                        rbMale.Checked = true;
                    }
                    else
                    {
                        rbFemale.Checked = true;
                    }
                }

                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == mEditorItem.SpawnMap)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                if (cmbWarpMap.SelectedIndex == -1)
                {
                    cmbWarpMap.SelectedIndex = 0;
                    mEditorItem.SpawnMap = MapList.GetOrderedMaps()[0].MapNum;
                }
                nudX.Value = mEditorItem.SpawnX;
                nudY.Value = mEditorItem.SpawnY;
                cmbDirection.SelectedIndex = mEditorItem.SpawnDir;

                UpdateDropValues();
                DrawSprite();
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

        private void frmClass_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.General.none);
            cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));
            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.General.none);
            cmbFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            cmbItem.Items.Clear();
            cmbItem.Items.Add(Strings.General.none);
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            nudLevel.Maximum = Options.MaxLevel;
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }
            nudAttack.Maximum = Options.MaxStatValue;
            nudMag.Maximum = Options.MaxStatValue;
            nudDef.Maximum = Options.MaxStatValue;
            nudMR.Maximum = Options.MaxStatValue;
            nudSpd.Maximum = Options.MaxStatValue;
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.ClassEditor.title;
            toolStripItemNew.Text = Strings.ClassEditor.New;
            toolStripItemDelete.Text = Strings.ClassEditor.delete;
            toolStripItemCopy.Text = Strings.ClassEditor.copy;
            toolStripItemPaste.Text = Strings.ClassEditor.paste;
            toolStripItemUndo.Text = Strings.ClassEditor.undo;

            grpClasses.Text = Strings.ClassEditor.classes;

            grpGeneral.Text = Strings.ClassEditor.general;
            lblName.Text = Strings.ClassEditor.name;
            chkLocked.Text = Strings.ClassEditor.locked;

            grpSpawnPoint.Text = Strings.ClassEditor.spawnpoint;
            lblMap.Text = Strings.Warping.map.ToString( "");
            lblX.Text = Strings.Warping.x.ToString( "");
            lblY.Text = Strings.Warping.y;
            lblDir.Text = Strings.Warping.direction.ToString( "");
            cmbDirection.Items.Clear();
            for (var i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Directions.dir[i]);
            }
            btnVisualMapSelector.Text = Strings.Warping.visual;

            grpSprite.Text = Strings.ClassEditor.spriteface;
            lblSpriteOptions.Text = Strings.ClassEditor.spriteoptions;
            btnAdd.Text = Strings.ClassEditor.addicon;
            btnRemove.Text = Strings.ClassEditor.removeicon;
            grpGender.Text = Strings.ClassEditor.gender;
            rbMale.Text = Strings.ClassEditor.male;
            rbFemale.Text = Strings.ClassEditor.female;
            lblSprite.Text = Strings.ClassEditor.sprite;
            lblFace.Text = Strings.ClassEditor.face;

            grpItems.Text = Strings.ClassEditor.items;
            lblDropIndex.Text = Strings.ClassEditor.itemindex.ToString( scrlDropIndex.Value + 1);
            lblDropItem.Text = Strings.ClassEditor.item;
            lblDropAmount.Text = Strings.ClassEditor.amount;

            grpBaseStats.Text = Strings.ClassEditor.basestats;
            lblHP.Text = Strings.ClassEditor.basehp;
            lblMana.Text = Strings.ClassEditor.basemp;
            lblAttack.Text = Strings.ClassEditor.baseattack;
            lblDef.Text = Strings.ClassEditor.basearmor;
            lblSpd.Text = Strings.ClassEditor.basespeed;
            lblMag.Text = Strings.ClassEditor.baseabilitypower;
            lblMR.Text = Strings.ClassEditor.basemagicresist;
            lblPoints.Text = Strings.ClassEditor.basepoints;

            grpSpells.Text = Strings.ClassEditor.learntspells;
            lblSpellNum.Text = Strings.ClassEditor.spell;
            lblLevel.Text = Strings.ClassEditor.spelllevel;
            btnAddSpell.Text = Strings.ClassEditor.addspell;
            btnRemoveSpell.Text = Strings.ClassEditor.removespell;

            grpRegen.Text = Strings.ClassEditor.regen;
            lblHpRegen.Text = Strings.ClassEditor.hpregen;
            lblManaRegen.Text = Strings.ClassEditor.mpregen;
            lblRegenHint.Text = Strings.ClassEditor.regenhint;

            grpCombat.Text = Strings.ClassEditor.combat;
            lblDamage.Text = Strings.ClassEditor.basedamage;
            lblCritChance.Text = Strings.ClassEditor.critchance;
            lblDamageType.Text = Strings.ClassEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.Combat.damagetypes.Length; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.ClassEditor.scalingstat;
            lblScalingAmount.Text = Strings.ClassEditor.scalingamount;
            lblAttackAnimation.Text = Strings.ClassEditor.attackanimation;

            grpLeveling.Text = Strings.ClassEditor.leveling;
            lblBaseExp.Text = Strings.ClassEditor.levelexp;
            lblExpIncrease.Text = Strings.ClassEditor.levelexpscale;
            grpLevelBoosts.Text = Strings.ClassEditor.levelboosts;
            rdoStaticIncrease.Text = Strings.ClassEditor.staticboost;
            rdoPercentageIncrease.Text = Strings.ClassEditor.percentageboost;
            lblHpIncrease.Text = Strings.ClassEditor.hpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMpIncrease.Text = Strings.ClassEditor.mpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblStrengthIncrease.Text = Strings.ClassEditor.attackboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblArmorIncrease.Text = Strings.ClassEditor.armorboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblSpeedIncrease.Text = Strings.ClassEditor.speedboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMagicIncrease.Text = Strings.ClassEditor.abilitypowerboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMagicResistIncrease.Text = Strings.ClassEditor.magicresistboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblPointsIncrease.Text = Strings.ClassEditor.pointsboost;

            btnSave.Text = Strings.ClassEditor.save;
            btnCancel.Text = Strings.ClassEditor.cancel;
        }

        public void InitEditor()
        {
            lstClasses.Items.Clear();
            lstClasses.Items.AddRange(Database.GetGameObjectList(GameObjectType.Class));
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
        }

        private void lstSprites_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                cmbSprite.SelectedIndex = cmbSprite.FindString(TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Sprite));
                cmbFace.SelectedIndex = cmbFace.FindString(TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Face));
                if (mEditorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }
            }
        }

        private void rbMale_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                mEditorItem.Sprites[lstSprites.SelectedIndex].Gender = 0;

                RefreshSpriteList();
            }
        }

        private void rbFemale_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                mEditorItem.Sprites[lstSprites.SelectedIndex].Gender = 1;

                RefreshSpriteList();
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex >= 0)
            {
                mEditorItem.Sprites[lstSprites.SelectedIndex].Sprite = TextUtils.SanitizeNone(cmbSprite?.Text);

                RefreshSpriteList();
            }
            DrawSprite();
        }

        private void RefreshSpriteList(bool saveSpot = true)
        {
            // Refresh List
            var n = lstSprites.SelectedIndex;
            lstSprites.Items.Clear();
            for (int i = 0; i < mEditorItem.Sprites.Count; i++)
            {
                if (mEditorItem.Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(Strings.ClassEditor.spriteitemmale.ToString( i + 1,
                        TextUtils.NullToNone(mEditorItem.Sprites[i].Sprite)));
                }
                else
                {
                    lstSprites.Items.Add(Strings.ClassEditor.spriteitemfemale.ToString( i + 1,
                        TextUtils.NullToNone(mEditorItem.Sprites[i].Sprite)));
                }
            }
            if (saveSpot) lstSprites.SelectedIndex = n;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var n = new ClassSprite
            {
                Sprite = null,
                Face = null,
                Gender = 0
            };

            mEditorItem.Sprites.Add(n);

            if (n.Gender == 0)
            {
                lstSprites.Items.Add(Strings.ClassEditor.spriteitemmale.ToString(mEditorItem.Sprites.Count, TextUtils.NullToNone(n.Sprite)));
            }
            else
            {
                lstSprites.Items.Add(
                    Strings.ClassEditor.spriteitemfemale.ToString(mEditorItem.Sprites.Count, TextUtils.NullToNone(n.Sprite)));
            }

            lstSprites.SelectedIndex = lstSprites.Items.Count - 1;
            lstSprites_Click(null, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex == -1) return;
            mEditorItem.Sprites.RemoveAt(lstSprites.SelectedIndex);
            lstSprites.Items.RemoveAt(lstSprites.SelectedIndex);

            RefreshSpriteList(false);

            if (lstSprites.Items.Count > 0)
            {
                lstSprites.SelectedIndex = 0;
            }
        }

        private void DrawSprite()
        {
            var picSpriteBmp = new Bitmap(picSprite.Width, picSprite.Height);
            var gfx = Graphics.FromImage(picSpriteBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picSprite.Width, picSprite.Height));
            if (cmbSprite.SelectedIndex > 0)
            {
                if (File.Exists("resources/entities/" + cmbSprite.Text))
                {
                    var img = Image.FromFile("resources/entities/" + cmbSprite.Text);
                    gfx.DrawImage(img, new Rectangle(0, 0, img.Width / 4, img.Height / 4),
                        new Rectangle(0, 0, img.Width / 4, img.Height / 4), GraphicsUnit.Pixel);
                    img.Dispose();
                }
            }
            gfx.Dispose();
            picSprite.BackgroundImage = picSpriteBmp;

            var picFaceBmp = new Bitmap(picFace.Width, picFace.Height);
            gfx = Graphics.FromImage(picFaceBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picSprite.Width, picSprite.Height));
            if (cmbFace.SelectedIndex > 0)
            {
                if (File.Exists("resources/faces/" + cmbFace.Text))
                {
                    var img = Image.FromFile("resources/faces/" + cmbFace.Text);
                    gfx.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
                        new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    img.Dispose();
                }
            }
            gfx.Dispose();
            picFace.BackgroundImage = picFaceBmp;
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            FrmWarpSelection frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, (int) nudX.Value,
                (int) nudY.Value);
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
                nudX.Value = frmWarpSelection.GetX();
                nudY.Value = frmWarpSelection.GetY();
                mEditorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                mEditorItem.SpawnX = (int) nudX.Value;
                mEditorItem.SpawnY = (int) nudY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mEditorItem == null) return;
            mEditorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mEditorItem == null) return;
            mEditorItem.SpawnDir = cmbDirection.SelectedIndex;
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex >= 0)
            {
                mEditorItem.Sprites[lstSprites.SelectedIndex].Face = TextUtils.SanitizeNone(cmbFace?.Text);

                RefreshSpriteList();
            }
            DrawSprite();
        }

        private void chkLocked_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Locked = Convert.ToInt32(chkLocked.Checked);
        }

        private void UpdateIncreases()
        {
            if (rdoStaticIncrease.Checked)
            {
                nudHpIncrease.Maximum = 10000;
                nudMpIncrease.Maximum = 10000;
                nudStrengthIncrease.Maximum = Options.MaxStatValue;
                nudArmorIncrease.Maximum = Options.MaxStatValue;
                nudMagicIncrease.Maximum = Options.MaxStatValue;
                nudMagicResistIncrease.Maximum = Options.MaxStatValue;
                nudSpeedIncrease.Maximum = Options.MaxStatValue;
            }
            else
            {
                nudHpIncrease.Maximum = 100;
                nudMpIncrease.Maximum = 100;
                nudStrengthIncrease.Maximum = 100;
                nudArmorIncrease.Maximum = 100;
                nudMagicIncrease.Maximum = 100;
                nudMagicResistIncrease.Maximum = 100;
                nudSpeedIncrease.Maximum = 100;
            }

            nudHpIncrease.Value = Math.Min(nudHpIncrease.Maximum, mEditorItem.VitalIncrease[(int) Vitals.Health]);
            nudMpIncrease.Value = Math.Min(nudMpIncrease.Maximum, mEditorItem.VitalIncrease[(int) Vitals.Mana]);

            nudStrengthIncrease.Value = Math.Min(nudStrengthIncrease.Maximum,
                mEditorItem.StatIncrease[(int) Stats.Attack]);
            nudArmorIncrease.Value = Math.Min(nudArmorIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.Defense]);
            nudMagicIncrease.Value = Math.Min(nudMagicIncrease.Maximum,
                mEditorItem.StatIncrease[(int) Stats.AbilityPower]);
            nudMagicResistIncrease.Value = Math.Min(nudMagicResistIncrease.Maximum,
                mEditorItem.StatIncrease[(int) Stats.MagicResist]);
            nudSpeedIncrease.Value = Math.Min(nudSpeedIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.Speed]);

            lblHpIncrease.Text = Strings.ClassEditor.hpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMpIncrease.Text = Strings.ClassEditor.mpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblStrengthIncrease.Text = Strings.ClassEditor.attackboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblArmorIncrease.Text = Strings.ClassEditor.armorboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblSpeedIncrease.Text = Strings.ClassEditor.speedboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMagicIncrease.Text = Strings.ClassEditor.abilitypowerboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());
            lblMagicResistIncrease.Text = Strings.ClassEditor.magicresistboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString());

            nudPointsIncrease.Value = mEditorItem.PointIncrease;
        }

        private void rdoPercentageIncrease_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void rdoStaticIncrease_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Class);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstClasses.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.ClassEditor.deleteprompt,
                        Strings.ClassEditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstClasses.Focused)
            {
                mCopiedItem = mEditorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstClasses.Focused)
            {
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.ClassEditor.undoprompt,
                        Strings.ClassEditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstClasses.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstClasses.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstClasses.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstClasses.Focused;
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

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1 && cmbSpell.SelectedIndex > -1)
            {
                mEditorItem.Spells[lstSpells.SelectedIndex].SpellNum = Database.GameObjectIdFromList(
                    GameObjectType.Spell,
                    cmbSpell.SelectedIndex);
                UpdateSpellList();
            }
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Items[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item,
                cmbItem.SelectedIndex - 1);
        }

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                    mEditorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                nudLevel.Value = mEditorItem.Spells[lstSpells.SelectedIndex].Level;
            }
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int) (nudScaling.Value);
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnX = (int) nudX.Value;
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnY = (int) nudY.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseStat[(int) Stats.Attack] = (int) nudAttack.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseStat[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseStat[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseStat[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudPoints_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BasePoints = (int) nudPoints.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseStat[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex >= 0)
            {
                mEditorItem.Spells[lstSpells.SelectedIndex].Level = (int) nudLevel.Value;

                UpdateSpellList();
            }
        }

        private void nudHPRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalRegen[(int) Vitals.Health] = (int) nudHPRegen.Value;
            UpdateIncreases();
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalRegen[(int) Vitals.Mana] = (int) nudMpRegen.Value;
            UpdateIncreases();
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudExpIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.ExpIncrease = (int) nudExpIncrease.Value;
        }

        private void nudHpIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalIncrease[(int) Vitals.Health] = (int) nudHpIncrease.Value;
        }

        private void nudMpIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalIncrease[(int) Vitals.Mana] = (int) nudMpIncrease.Value;
        }

        private void nudArmorIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatIncrease[(int) Stats.Defense] = (int) nudArmorIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicResistIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatIncrease[(int) Stats.MagicResist] = (int) nudMagicResistIncrease.Value;
            UpdateIncreases();
        }

        private void nudStrengthIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatIncrease[(int) Stats.Attack] = (int) nudStrengthIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatIncrease[(int) Stats.AbilityPower] = (int) nudMagicIncrease.Value;
            UpdateIncreases();
        }

        private void nudSpeedIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatIncrease[(int) Stats.Speed] = (int) nudSpeedIncrease.Value;
            UpdateIncreases();
        }

        private void nudPointsIncrease_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PointIncrease = (int) nudPointsIncrease.Value;
            UpdateIncreases();
        }

        private void nudItemAmount_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Items[scrlDropIndex.Value].Amount = (int) nudItemAmount.Value;
        }

        private void nudBaseExp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseExp = (int) nudBaseExp.Value;
        }

        private void nudBaseHP_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseVital[(int) Vitals.Health] = (int) nudBaseHP.Value;
        }

        private void nudBaseMana_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.BaseVital[(int) Vitals.Mana] = (int) nudBaseMana.Value;
        }
    }
}