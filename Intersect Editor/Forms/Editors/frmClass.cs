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
using Intersect.Localization;

namespace Intersect.Editor.Forms
{
    public partial class frmClass : EditorForm
    {
        private List<ClassBase> _changed = new List<ClassBase>();
        private byte[] _copiedItem;
        private ClassBase _editorItem;

        public frmClass()
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
                if (_editorItem != null && !ClassBase.Lookup.Values.Contains(_editorItem))
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

        private void lstClasses_Click(object sender, EventArgs e)
        {
            _editorItem = ClassBase.Lookup.Get<ClassBase>(Database.GameObjectIdFromList(GameObjectType.Class, lstClasses.SelectedIndex));
            UpdateEditor();
        }

        private void scrlDropIndex_Scroll(object sender, ScrollValueEventArgs e)
        {
            UpdateDropValues();
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value;
            lblDropIndex.Text = Strings.Get("classeditor", "itemindex", index + 1);
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _editorItem.Items[index].ItemNum) + 1;
            nudItemAmount.Value = _editorItem.Items[index].Amount;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstClasses.Items[Database.GameObjectListIndex(GameObjectType.Class, _editorItem.Index)] = txtName.Text;
        }

        private void UpdateSpellList(bool keepIndex = true)
        {
            // Refresh List
            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < _editorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(Strings.Get("classeditor", "spellitem", i + 1,
                    SpellBase.GetName(_editorItem.Spells[i].SpellNum), _editorItem.Spells[i].Level));
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

            _editorItem.Spells.Add(n);
            UpdateSpellList(false);
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex == -1) return;
            _editorItem.Spells.RemoveAt(lstSpells.SelectedIndex);
            lstSpells.Items.RemoveAt(lstSpells.SelectedIndex);

            UpdateSpellList(false);

            if (lstSpells.Items.Count > 0)
            {
                lstSpells.SelectedIndex = 0;
            }
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();
                txtName.Text = _editorItem.Name;
                nudAttack.Value = _editorItem.BaseStat[(int) Stats.Attack];
                nudMag.Value = _editorItem.BaseStat[(int) Stats.AbilityPower];
                nudDef.Value = _editorItem.BaseStat[(int) Stats.Defense];
                nudMR.Value = _editorItem.BaseStat[(int) Stats.MagicResist];
                nudSpd.Value = _editorItem.BaseStat[(int) Stats.Speed];
                nudBaseHP.Value = Math.Max(Math.Min(_editorItem.BaseVital[(int)Vitals.Health], nudBaseHP.Maximum), nudBaseHP.Minimum);
                nudBaseMana.Value = _editorItem.BaseVital[(int) Vitals.Mana];
                nudPoints.Value = _editorItem.BasePoints;
                chkLocked.Checked = Convert.ToBoolean(_editorItem.Locked);

                //Combat
                nudDamage.Value = _editorItem.Damage;
                nudCritChance.Value = _editorItem.CritChance;
                nudScaling.Value = _editorItem.Scaling / 100;
                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.AttackAnimation) + 1;

                //Regen
                nudHPRegen.Value = _editorItem.VitalRegen[(int) Vitals.Health];
                nudMpRegen.Value = _editorItem.VitalRegen[(int) Vitals.Mana];

                //Exp
                nudBaseExp.Value = _editorItem.BaseExp;
                nudExpIncrease.Value = _editorItem.ExpIncrease;

                //Stat Increases
                if (_editorItem.IncreasePercentage == 0)
                {
                    rdoStaticIncrease.Checked = true;
                }
                else
                {
                    rdoPercentageIncrease.Checked = true;
                }

                UpdateIncreases();

                UpdateSpellList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                        _editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                    nudLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
                }
                else
                {
                    cmbSpell.SelectedIndex = -1;
                    nudLevel.Value = 0;
                }

                cmbSpell.SelectedIndex = -1;

                RefreshSpriteList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSprites.Items.Count > 0)
                {
                    lstSprites.SelectedIndex = 0;
                    cmbSprite.SelectedIndex =
                        cmbSprite.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Sprite);
                    if (_editorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
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
                    if (MapList.GetOrderedMaps()[i].MapNum == _editorItem.SpawnMap)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                if (cmbWarpMap.SelectedIndex == -1)
                {
                    cmbWarpMap.SelectedIndex = 0;
                    _editorItem.SpawnMap = MapList.GetOrderedMaps()[0].MapNum;
                }
                nudX.Value = _editorItem.SpawnX;
                nudY.Value = _editorItem.SpawnY;
                cmbDirection.SelectedIndex = _editorItem.SpawnDir;

                UpdateDropValues();
                DrawSprite();
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

        private void frmClass_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add(Strings.Get("general", "none"));
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.Get("general", "none"));
            cmbFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            cmbItem.Items.Clear();
            cmbItem.Items.Add(Strings.Get("general", "none"));
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            nudLevel.Maximum = Options.MaxLevel;
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
            Text = Strings.Get("classeditor", "title");
            toolStripItemNew.Text = Strings.Get("classeditor", "new");
            toolStripItemDelete.Text = Strings.Get("classeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("classeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("classeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("classeditor", "undo");

            grpClasses.Text = Strings.Get("classeditor", "classes");

            grpGeneral.Text = Strings.Get("classeditor", "general");
            lblName.Text = Strings.Get("classeditor", "name");
            chkLocked.Text = Strings.Get("classeditor", "locked");

            grpSpawnPoint.Text = Strings.Get("classeditor", "spawnpoint");
            lblMap.Text = Strings.Get("warping", "map", "");
            lblX.Text = Strings.Get("warping", "x", "");
            lblY.Text = Strings.Get("warping", "y");
            lblDir.Text = Strings.Get("warping", "direction", "");
            cmbDirection.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbDirection.Items.Add(Strings.Get("directions", i.ToString()));
            }
            btnVisualMapSelector.Text = Strings.Get("warping", "visual");

            grpSprite.Text = Strings.Get("classeditor", "spriteface");
            lblSpriteOptions.Text = Strings.Get("classeditor", "spriteoptions");
            btnAdd.Text = Strings.Get("classeditor", "addicon");
            btnRemove.Text = Strings.Get("classeditor", "removeicon");
            grpGender.Text = Strings.Get("classeditor", "gender");
            rbMale.Text = Strings.Get("classeditor", "male");
            rbFemale.Text = Strings.Get("classeditor", "female");
            lblSprite.Text = Strings.Get("classeditor", "sprite");
            lblFace.Text = Strings.Get("classeditor", "face");

            grpItems.Text = Strings.Get("classeditor", "items");
            lblDropIndex.Text = Strings.Get("classeditor", "itemindex", scrlDropIndex.Value + 1);
            lblDropItem.Text = Strings.Get("classeditor", "item");
            lblDropAmount.Text = Strings.Get("classeditor", "amount");

            grpBaseStats.Text = Strings.Get("classeditor", "basestats");
            lblHP.Text = Strings.Get("classeditor", "basehp");
            lblMana.Text = Strings.Get("classeditor", "basemp");
            lblAttack.Text = Strings.Get("classeditor", "baseattack");
            lblDef.Text = Strings.Get("classeditor", "basearmor");
            lblSpd.Text = Strings.Get("classeditor", "basespeed");
            lblMag.Text = Strings.Get("classeditor", "baseabilitypower");
            lblMR.Text = Strings.Get("classeditor", "basemagicresist");
            lblPoints.Text = Strings.Get("classeditor", "basepoints");

            grpSpells.Text = Strings.Get("classeditor", "learntspells");
            lblSpellNum.Text = Strings.Get("classeditor", "spell");
            lblLevel.Text = Strings.Get("classeditor", "spelllevel");
            btnAddSpell.Text = Strings.Get("classeditor", "addspell");
            btnRemoveSpell.Text = Strings.Get("classeditor", "removespell");

            grpRegen.Text = Strings.Get("classeditor", "regen");
            lblHpRegen.Text = Strings.Get("classeditor", "hpregen");
            lblManaRegen.Text = Strings.Get("classeditor", "mpregen");
            lblRegenHint.Text = Strings.Get("classeditor", "regenhint");

            grpCombat.Text = Strings.Get("classeditor", "combat");
            lblDamage.Text = Strings.Get("classeditor", "basedamage");
            lblCritChance.Text = Strings.Get("classeditor", "critchance");
            lblDamageType.Text = Strings.Get("classeditor", "damagetype");
            cmbDamageType.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbDamageType.Items.Add(Strings.Get("classeditor", "damagetype" + i));
            }
            lblScalingStat.Text = Strings.Get("classeditor", "scalingstat");
            lblScalingAmount.Text = Strings.Get("classeditor", "scalingamount");
            lblAttackAnimation.Text = Strings.Get("classeditor", "attackanimation");

            grpLeveling.Text = Strings.Get("classeditor", "leveling");
            lblBaseExp.Text = Strings.Get("classeditor", "levelexp");
            lblExpIncrease.Text = Strings.Get("classeditor", "levelexpscale");
            grpLevelBoosts.Text = Strings.Get("classeditor", "levelboosts");
            rdoStaticIncrease.Text = Strings.Get("classeditor", "staticboost");
            rdoPercentageIncrease.Text = Strings.Get("classeditor", "percentageboost");
            lblHpIncrease.Text = Strings.Get("classeditor", "hpboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMpIncrease.Text = Strings.Get("classeditor", "mpboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblStrengthIncrease.Text = Strings.Get("classeditor", "attackboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblArmorIncrease.Text = Strings.Get("classeditor", "armorboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblSpeedIncrease.Text = Strings.Get("classeditor", "speedboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMagicIncrease.Text = Strings.Get("classeditor", "abilitypowerboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMagicResistIncrease.Text = Strings.Get("classeditor", "magicresistboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblPointsIncrease.Text = Strings.Get("classeditor", "pointsboost");

            btnSave.Text = Strings.Get("classeditor", "save");
            btnCancel.Text = Strings.Get("classeditor", "cancel");
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
                cmbSprite.SelectedIndex = cmbSprite.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Sprite);
                cmbFace.SelectedIndex = cmbFace.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Face);
                if (_editorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
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
                _editorItem.Sprites[lstSprites.SelectedIndex].Gender = 0;

                RefreshSpriteList();
            }
        }

        private void rbFemale_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                _editorItem.Sprites[lstSprites.SelectedIndex].Gender = 1;

                RefreshSpriteList();
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex >= 0)
            {
                _editorItem.Sprites[lstSprites.SelectedIndex].Sprite = cmbSprite.Text;

                RefreshSpriteList();
            }
            DrawSprite();
        }

        private void RefreshSpriteList(bool saveSpot = true)
        {
            // Refresh List
            var n = lstSprites.SelectedIndex;
            lstSprites.Items.Clear();
            for (int i = 0; i < _editorItem.Sprites.Count; i++)
            {
                if (_editorItem.Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(Strings.Get("classeditor", "spriteitemmale", i + 1,
                        _editorItem.Sprites[i].Sprite));
                }
                else
                {
                    lstSprites.Items.Add(Strings.Get("classeditor", "spriteitemfemale", i + 1,
                        _editorItem.Sprites[i].Sprite));
                }
            }
            if (saveSpot) lstSprites.SelectedIndex = n;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var n = new ClassSprite
            {
                Sprite = Strings.Get("general", "none"),
                Face = Strings.Get("general", "none"),
                Gender = 0
            };

            _editorItem.Sprites.Add(n);

            if (n.Gender == 0)
            {
                lstSprites.Items.Add(Strings.Get("classeditor", "spriteitemmale", _editorItem.Sprites.Count, n.Sprite));
            }
            else
            {
                lstSprites.Items.Add(Strings.Get("classeditor", "spriteitemfemale", _editorItem.Sprites.Count, n.Sprite));
            }

            lstSprites.SelectedIndex = lstSprites.Items.Count - 1;
            lstSprites_Click(null, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex == -1) return;
            _editorItem.Sprites.RemoveAt(lstSprites.SelectedIndex);
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
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
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
                _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                _editorItem.SpawnX = (int) nudX.Value;
                _editorItem.SpawnY = (int) nudY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnDir = cmbDirection.SelectedIndex;
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex >= 0)
            {
                _editorItem.Sprites[lstSprites.SelectedIndex].Face = cmbFace.Text;

                RefreshSpriteList();
            }
            DrawSprite();
        }

        private void chkLocked_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Locked = Convert.ToInt32(chkLocked.Checked);
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

            nudHpIncrease.Value = Math.Min(nudHpIncrease.Maximum, _editorItem.VitalIncrease[(int) Vitals.Health]);
            nudMpIncrease.Value = Math.Min(nudMpIncrease.Maximum, _editorItem.VitalIncrease[(int) Vitals.Mana]);

            nudStrengthIncrease.Value = Math.Min(nudStrengthIncrease.Maximum,
                _editorItem.StatIncrease[(int) Stats.Attack]);
            nudArmorIncrease.Value = Math.Min(nudArmorIncrease.Maximum, _editorItem.StatIncrease[(int) Stats.Defense]);
            nudMagicIncrease.Value = Math.Min(nudMagicIncrease.Maximum,
                _editorItem.StatIncrease[(int) Stats.AbilityPower]);
            nudMagicResistIncrease.Value = Math.Min(nudMagicResistIncrease.Maximum,
                _editorItem.StatIncrease[(int) Stats.MagicResist]);
            nudSpeedIncrease.Value = Math.Min(nudSpeedIncrease.Maximum, _editorItem.StatIncrease[(int) Stats.Speed]);

            lblHpIncrease.Text = Strings.Get("classeditor", "hpboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMpIncrease.Text = Strings.Get("classeditor", "mpboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblStrengthIncrease.Text = Strings.Get("classeditor", "attackboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblArmorIncrease.Text = Strings.Get("classeditor", "armorboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblSpeedIncrease.Text = Strings.Get("classeditor", "speedboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMagicIncrease.Text = Strings.Get("classeditor", "abilitypowerboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));
            lblMagicResistIncrease.Text = Strings.Get("classeditor", "magicresistboost",
                rdoStaticIncrease.Checked ? "" : Strings.Get("classeditor", "boostpercent"));

            nudPointsIncrease.Value = _editorItem.PointIncrease;
        }

        private void rdoPercentageIncrease_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void rdoStaticIncrease_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Class);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstClasses.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("classeditor", "deleteprompt"),
                        Strings.Get("classeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstClasses.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstClasses.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("classeditor", "undoprompt"),
                        Strings.Get("classeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            toolStripItemCopy.Enabled = _editorItem != null && lstClasses.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstClasses.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstClasses.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstClasses.Focused;
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

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                _editorItem.Spells[lstSpells.SelectedIndex].SpellNum = Database.GameObjectIdFromList(GameObjectType.Spell,
                    cmbSpell.SelectedIndex);
                UpdateSpellList();
            }
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Items[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item,
                cmbItem.SelectedIndex - 1);
        }

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell,
                    _editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                nudLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
            }
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Scaling = (int) (nudScaling.Value * 100);
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnX = (int) nudX.Value;
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnY = (int) nudY.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int) Stats.Attack] = (int) nudAttack.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int) Stats.AbilityPower] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int) Stats.Defense] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int) Stats.MagicResist] = (int) nudMR.Value;
        }

        private void nudPoints_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BasePoints = (int) nudPoints.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int) Stats.Speed] = (int) nudSpd.Value;
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex >= 0)
            {
                _editorItem.Spells[lstSpells.SelectedIndex].Level = (int) nudLevel.Value;

                UpdateSpellList();
            }
        }

        private void nudHPRegen_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalRegen[(int) Vitals.Health] = (int) nudHPRegen.Value;
            UpdateIncreases();
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalRegen[(int) Vitals.Mana] = (int) nudMpRegen.Value;
            UpdateIncreases();
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudExpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.ExpIncrease = (int) nudExpIncrease.Value;
        }

        private void nudHpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalIncrease[(int) Vitals.Health] = (int) nudHpIncrease.Value;
        }

        private void nudMpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalIncrease[(int) Vitals.Mana] = (int) nudMpIncrease.Value;
        }

        private void nudArmorIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int) Stats.Defense] = (int) nudArmorIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicResistIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int) Stats.MagicResist] = (int) nudMagicResistIncrease.Value;
            UpdateIncreases();
        }

        private void nudStrengthIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int) Stats.Attack] = (int) nudStrengthIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int) Stats.AbilityPower] = (int) nudMagicIncrease.Value;
            UpdateIncreases();
        }

        private void nudSpeedIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int) Stats.Speed] = (int) nudSpeedIncrease.Value;
            UpdateIncreases();
        }

        private void nudPointsIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.PointIncrease = (int) nudPointsIncrease.Value;
            UpdateIncreases();
        }

        private void nudItemAmount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Items[scrlDropIndex.Value].Amount = (int) nudItemAmount.Value;
        }

        private void nudBaseExp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseExp = (int) nudBaseExp.Value;
        }

        private void nudBaseHP_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseVital[(int) Vitals.Health] = (int) nudBaseHP.Value;
        }

        private void nudBaseMana_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseVital[(int) Vitals.Mana] = (int) nudBaseMana.Value;
        }
    }
}
