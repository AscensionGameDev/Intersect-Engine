using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmClass : EditorForm
    {

        private List<ClassBase> mChanged = new List<ClassBase>();

        private string mCopiedItem;

        private ClassBase mEditorItem;

        private List<string> mKnownFolders = new List<string>();

        public FrmClass()
        {
            ApplyHooks();
            InitializeComponent();

            cmbWarpMap.Items.Clear();
            for (var i = 0; i < MapList.OrderedMaps.Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.OrderedMaps[i].Name);
            }

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = ClassBase.Get(id);
            UpdateEditor();
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

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
        }

        private void UpdateSpellList(bool keepIndex = true)
        {
            // Refresh List
            var n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (var i = 0; i < mEditorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(
                    Strings.ClassEditor.spellitem.ToString(
                        i + 1, SpellBase.GetName(mEditorItem.Spells[i].Id), mEditorItem.Spells[i].Level
                    )
                );
            }

            if (keepIndex)
            {
                lstSpells.SelectedIndex = n;
            }
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            var n = new ClassSpell
            {
                Id = SpellBase.IdFromList(cmbSpell.SelectedIndex),
                Level = (int) nudLevel.Value
            };

            mEditorItem.Spells.Add(n);
            UpdateSpellList(false);
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex == -1)
            {
                return;
            }

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
                nudBaseHP.Value = Math.Max(
                    Math.Min(mEditorItem.BaseVital[(int) Vitals.Health], nudBaseHP.Maximum), nudBaseHP.Minimum
                );

                nudBaseMana.Value = mEditorItem.BaseVital[(int) Vitals.Mana];
                nudPoints.Value = mEditorItem.BasePoints;
                chkLocked.Checked = Convert.ToBoolean(mEditorItem.Locked);

                cmbFolder.Text = mEditorItem.Folder;

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
                nudHPRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Health];
                nudMpRegen.Value = mEditorItem.VitalRegen[(int) Vitals.Mana];

                //Exp
                nudBaseExp.Value = mEditorItem.BaseExp;
                nudExpIncrease.Value = mEditorItem.ExpIncrease;

                //Stat Increases
                if (!mEditorItem.IncreasePercentage)
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
                    cmbSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.Spells[lstSpells.SelectedIndex].Id);
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
                    cmbSprite.SelectedIndex = cmbSprite.FindString(
                        TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Sprite)
                    );

                    if (mEditorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
                    {
                        rbMale.Checked = true;
                    }
                    else
                    {
                        rbFemale.Checked = true;
                    }
                }

                var mapIndex = -1;

                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == mEditorItem.SpawnMapId)
                    {
                        mapIndex = i;

                        break;
                    }
                }

                if (mapIndex == -1)
                {
                    cmbWarpMap.SelectedIndex = 0;
                    mEditorItem.SpawnMapId = MapList.OrderedMaps[0].MapId;
                }
                else
                {
                    cmbWarpMap.SelectedIndex = mapIndex;
                }

                nudX.Value = mEditorItem.SpawnX;
                nudY.Value = mEditorItem.SpawnY;
                cmbDirection.SelectedIndex = mEditorItem.SpawnDir;

                UpdateSpawnItemValues();
                DrawSprite();
                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
                }

                grpExpGrid.Hide();
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
            cmbSprite.Items.AddRange(
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity)
            );

            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.General.none);
            cmbFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            cmbSpawnItem.Items.Clear();
            cmbSpawnItem.Items.Add(Strings.General.none);
            cmbSpawnItem.Items.AddRange(ItemBase.Names);
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(SpellBase.Names);
            nudLevel.Maximum = Options.MaxLevel;
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(AnimationBase.Names);
            cmbScalingStat.Items.Clear();
            for (var x = 0; x < ((int)Stats.Speed) + 1; x++)
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
            lblMap.Text = Strings.Warping.map.ToString("");
            lblX.Text = Strings.Warping.x.ToString("");
            lblY.Text = Strings.Warping.y;
            lblDir.Text = Strings.Warping.direction.ToString("");
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

            grpSpawnItems.Text = Strings.ClassEditor.spawnitems;
            lblSpawnItem.Text = Strings.ClassEditor.spawnitem;
            lblSpawnItemAmount.Text = Strings.ClassEditor.spawnamount;
            btnSpawnItemAdd.Text = Strings.ClassEditor.spawnitemadd;
            btnSpawnItemRemove.Text = Strings.ClassEditor.spawnitemremove;

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
            lblCritMultiplier.Text = Strings.ClassEditor.critmultiplier;
            lblDamageType.Text = Strings.ClassEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (var i = 0; i < Strings.Combat.damagetypes.Count; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }

            lblScalingStat.Text = Strings.ClassEditor.scalingstat;
            lblScalingAmount.Text = Strings.ClassEditor.scalingamount;
            lblAttackAnimation.Text = Strings.ClassEditor.attackanimation;

            grpAttackSpeed.Text = Strings.NpcEditor.attackspeed;
            lblAttackSpeedModifier.Text = Strings.NpcEditor.attackspeedmodifier;
            lblAttackSpeedValue.Text = Strings.NpcEditor.attackspeedvalue;
            cmbAttackSpeedModifier.Items.Clear();
            foreach (var val in Strings.NpcEditor.attackspeedmodifiers.Values)
            {
                cmbAttackSpeedModifier.Items.Add(val.ToString());
            }

            grpLeveling.Text = Strings.ClassEditor.leveling;
            lblBaseExp.Text = Strings.ClassEditor.levelexp;
            lblExpIncrease.Text = Strings.ClassEditor.levelexpscale;
            grpLevelBoosts.Text = Strings.ClassEditor.levelboosts;
            rdoStaticIncrease.Text = Strings.ClassEditor.staticboost;
            rdoPercentageIncrease.Text = Strings.ClassEditor.percentageboost;
            lblHpIncrease.Text = Strings.ClassEditor.hpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMpIncrease.Text = Strings.ClassEditor.mpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblStrengthIncrease.Text = Strings.ClassEditor.attackboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblArmorIncrease.Text = Strings.ClassEditor.armorboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblSpeedIncrease.Text = Strings.ClassEditor.speedboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMagicIncrease.Text = Strings.ClassEditor.abilitypowerboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMagicResistIncrease.Text = Strings.ClassEditor.magicresistboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblPointsIncrease.Text = Strings.ClassEditor.pointsboost;

            //Exp Grid
            btnExpGrid.Text = Strings.ClassEditor.expgrid;
            grpExpGrid.Text = Strings.ClassEditor.experiencegrid;
            btnResetExpGrid.Text = Strings.ClassEditor.gridreset;
            btnCloseExpGrid.Text = Strings.ClassEditor.gridclose;
            btnExpPaste.Text = Strings.ClassEditor.gridpaste;

            //Create EXP Grid...
            var levelCol = new DataGridViewTextBoxColumn();
            levelCol.HeaderText = Strings.ClassEditor.gridlevel;
            levelCol.ReadOnly = true;
            levelCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            var tnlCol = new DataGridViewTextBoxColumn();
            tnlCol.HeaderText = Strings.ClassEditor.gridtnl;
            tnlCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            var totalCol = new DataGridViewTextBoxColumn();
            totalCol.HeaderText = Strings.ClassEditor.gridtotalexp;
            totalCol.ReadOnly = true;
            totalCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            expGrid.Columns.Clear();
            expGrid.Columns.Add(levelCol);
            expGrid.Columns.Add(tnlCol);
            expGrid.Columns.Add(totalCol);

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.ClassEditor.sortchronologically;
            txtSearch.Text = Strings.ClassEditor.searchplaceholder;
            lblFolder.Text = Strings.ClassEditor.folderlabel;

            btnSave.Text = Strings.ClassEditor.save;
            btnCancel.Text = Strings.ClassEditor.cancel;
        }

        public void InitEditor()
        {
            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in ClassBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ClassBase) itm.Value).Folder) &&
                    !mFolders.Contains(((ClassBase) itm.Value).Folder))
                {
                    mFolders.Add(((ClassBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((ClassBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((ClassBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            var items = ClassBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((ClassBase)pair.Value)?.Name ?? Models.DatabaseObject<ClassBase>.Deleted, ((ClassBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
        }

        private void lstSprites_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                cmbSprite.SelectedIndex = cmbSprite.FindString(
                    TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Sprite)
                );

                cmbFace.SelectedIndex = cmbFace.FindString(
                    TextUtils.NullToNone(mEditorItem.Sprites[lstSprites.SelectedIndex].Face)
                );

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
                mEditorItem.Sprites[lstSprites.SelectedIndex].Gender = Gender.Male;

                RefreshSpriteList();
            }
        }

        private void rbFemale_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                mEditorItem.Sprites[lstSprites.SelectedIndex].Gender = Gender.Female;

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
            for (var i = 0; i < mEditorItem.Sprites.Count; i++)
            {
                if (mEditorItem.Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(
                        Strings.ClassEditor.spriteitemmale.ToString(
                            i + 1, TextUtils.NullToNone(mEditorItem.Sprites[i].Sprite)
                        )
                    );
                }
                else
                {
                    lstSprites.Items.Add(
                        Strings.ClassEditor.spriteitemfemale.ToString(
                            i + 1, TextUtils.NullToNone(mEditorItem.Sprites[i].Sprite)
                        )
                    );
                }
            }

            if (saveSpot)
            {
                lstSprites.SelectedIndex = n;
            }
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
                lstSprites.Items.Add(
                    Strings.ClassEditor.spriteitemmale.ToString(
                        mEditorItem.Sprites.Count, TextUtils.NullToNone(n.Sprite)
                    )
                );
            }
            else
            {
                lstSprites.Items.Add(
                    Strings.ClassEditor.spriteitemfemale.ToString(
                        mEditorItem.Sprites.Count, TextUtils.NullToNone(n.Sprite)
                    )
                );
            }

            lstSprites.SelectedIndex = lstSprites.Items.Count - 1;
            lstSprites_Click(null, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex == -1)
            {
                return;
            }

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
                    gfx.DrawImage(
                        img, new Rectangle(0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions),
                        new Rectangle(0, 0, img.Width / Options.Instance.Sprites.NormalFrames, img.Height / Options.Instance.Sprites.Directions), GraphicsUnit.Pixel
                    );

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
                    gfx.DrawImage(
                        img, new Rectangle(0, 0, img.Width, img.Height), new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel
                    );

                    img.Dispose();
                }
            }

            gfx.Dispose();
            picFace.BackgroundImage = picFaceBmp;
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            var frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.SelectTile(
                MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId, (int) nudX.Value, (int) nudY.Value
            );

            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (MapList.OrderedMaps[i].MapId == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;

                        break;
                    }
                }

                nudX.Value = frmWarpSelection.GetX();
                nudY.Value = frmWarpSelection.GetY();
                mEditorItem.SpawnMapId = MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId;
                mEditorItem.SpawnX = (byte) nudX.Value;
                mEditorItem.SpawnY = (byte) nudY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mEditorItem == null)
            {
                return;
            }

            mEditorItem.SpawnMapId = MapList.OrderedMaps[cmbWarpMap.SelectedIndex].MapId;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mEditorItem == null)
            {
                return;
            }

            mEditorItem.SpawnDir = (byte) cmbDirection.SelectedIndex;
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
            mEditorItem.Locked = chkLocked.Checked;
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

            nudStrengthIncrease.Value = Math.Min(
                nudStrengthIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.Attack]
            );

            nudArmorIncrease.Value = Math.Min(nudArmorIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.Defense]);
            nudMagicIncrease.Value = Math.Min(
                nudMagicIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.AbilityPower]
            );

            nudMagicResistIncrease.Value = Math.Min(
                nudMagicResistIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.MagicResist]
            );

            nudSpeedIncrease.Value = Math.Min(nudSpeedIncrease.Maximum, mEditorItem.StatIncrease[(int) Stats.Speed]);

            lblHpIncrease.Text = Strings.ClassEditor.hpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMpIncrease.Text = Strings.ClassEditor.mpboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblStrengthIncrease.Text = Strings.ClassEditor.attackboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblArmorIncrease.Text = Strings.ClassEditor.armorboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblSpeedIncrease.Text = Strings.ClassEditor.speedboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMagicIncrease.Text = Strings.ClassEditor.abilitypowerboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            lblMagicResistIncrease.Text = Strings.ClassEditor.magicresistboost.ToString(
                rdoStaticIncrease.Checked ? "" : Strings.ClassEditor.boostpercent.ToString()
            );

            nudPointsIncrease.Value = mEditorItem.PointIncrease;
        }

        private void rdoPercentageIncrease_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IncreasePercentage = rdoPercentageIncrease.Checked;
            UpdateIncreases();
        }

        private void rdoStaticIncrease_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IncreasePercentage = rdoPercentageIncrease.Checked;
            UpdateIncreases();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Class);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ClassEditor.deleteprompt, Strings.ClassEditor.deletetitle, DarkDialogButton.YesNo,
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
                        Strings.ClassEditor.undoprompt, Strings.ClassEditor.undotitle, DarkDialogButton.YesNo,
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

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1 && cmbSpell.SelectedIndex > -1)
            {
                mEditorItem.Spells[lstSpells.SelectedIndex].Id = SpellBase.IdFromList(cmbSpell.SelectedIndex);
                UpdateSpellList();
            }
        }

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.Spells[lstSpells.SelectedIndex].Id);
                nudLevel.Value = mEditorItem.Spells[lstSpells.SelectedIndex].Level;
            }
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnX = (byte) nudX.Value;
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnY = (byte) nudY.Value;
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

        private void UpdateSpawnItemValues(bool keepIndex = false)
        {
            var index = lstSpawnItems.SelectedIndex;
            lstSpawnItems.Items.Clear();

            var spawnItems = mEditorItem.Items.ToArray();
            foreach (var spawnItem in spawnItems)
            {
                if (ItemBase.Get(spawnItem.Id) == null)
                {
                    mEditorItem.Items.Remove(spawnItem);
                }
            }

            for (var i = 0; i < mEditorItem.Items.Count; i++)
            {
                if (mEditorItem.Items[i].Id != Guid.Empty)
                {
                    lstSpawnItems.Items.Add(
                        Strings.ClassEditor.spawnitemdisplay.ToString(
                            ItemBase.GetName(mEditorItem.Items[i].Id), mEditorItem.Items[i].Quantity
                        )
                    );
                }
                else
                {
                    lstSpawnItems.Items.Add(TextUtils.None);
                }
            }

            if (keepIndex && index < lstSpawnItems.Items.Count)
            {
                lstSpawnItems.SelectedIndex = index;
            }
        }

        private void cmbSpawnItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpawnItems.SelectedIndex > -1 && lstSpawnItems.SelectedIndex < mEditorItem.Items.Count)
            {
                mEditorItem.Items[lstSpawnItems.SelectedIndex].Id = ItemBase.IdFromList(cmbSpawnItem.SelectedIndex - 1);
            }

            UpdateSpawnItemValues(true);
        }

        private void nudSpawnItemAmount_ValueChanged(object sender, EventArgs e)
        {
            // This should never be below 1. We shouldn't accept giving 0 items!
            nudSpawnItemAmount.Value = Math.Max(1, nudSpawnItemAmount.Value);

            if (lstSpawnItems.SelectedIndex < lstSpawnItems.Items.Count)
            {
                return;
            }

            mEditorItem.Items[(int) lstSpawnItems.SelectedIndex].Quantity = (int) nudSpawnItemAmount.Value;
            UpdateSpawnItemValues(true);
        }

        private void lstSpawnItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpawnItems.SelectedIndex > -1)
            {
                cmbSpawnItem.SelectedIndex = ItemBase.ListIndex(mEditorItem.Items[lstSpawnItems.SelectedIndex].Id) + 1;
                nudSpawnItemAmount.Value = mEditorItem.Items[lstSpawnItems.SelectedIndex].Quantity;
            }
        }

        private void btnSpawnItemAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Items.Add(new ClassItem());
            mEditorItem.Items[mEditorItem.Items.Count - 1].Id = ItemBase.IdFromList(cmbSpawnItem.SelectedIndex - 1);
            mEditorItem.Items[mEditorItem.Items.Count - 1].Quantity = (int) nudSpawnItemAmount.Value;

            UpdateSpawnItemValues();
        }

        private void btnSpawnItemRemove_Click(object sender, EventArgs e)
        {
            if (lstSpawnItems.SelectedIndex > -1)
            {
                var i = lstSpawnItems.SelectedIndex;
                lstSpawnItems.Items.RemoveAt(i);
                mEditorItem.Items.RemoveAt(i);
            }

            UpdateSpawnItemValues(true);
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

        #region "Exp Grid"

        private void btnExpGrid_Click(object sender, EventArgs e)
        {
            grpExpGrid.Show();
            grpExpGrid.BringToFront();

            expGrid.Rows.Clear();

            for (var i = 1; i <= Options.MaxLevel; i++)
            {
                var index = expGrid.Rows.Add(i.ToString(), "", "");
                var row = expGrid.Rows[index];
                row.Cells[0].Style.SelectionBackColor = row.Cells[0].Style.BackColor;
                row.Cells[2].Style.SelectionBackColor = row.Cells[2].Style.BackColor;
            }

            UpdateExpGridValues(1);
        }

        private void UpdateExpGridValues(int start, int end = -1)
        {
            if (end == -1)
            {
                end = Options.MaxLevel;
            }

            if (start > end)
            {
                return;
            }

            if (start < 1)
            {
                start = 1;
            }

            for (var i = start; i <= end; i++)
            {
                if (i < Options.MaxLevel)
                {
                    if (mEditorItem.ExperienceOverrides.ContainsKey(i))
                    {
                        expGrid.Rows[i - 1].Cells[1].Value = Convert.ChangeType(
                            mEditorItem.ExperienceOverrides[i], expGrid.Rows[i - 1].Cells[1].ValueType
                        );

                        var style = expGrid.Rows[i - 1].Cells[1].InheritedStyle;
                        style.Font = new Font(style.Font, FontStyle.Bold);
                        expGrid.Rows[i - 1].Cells[1].Style.ApplyStyle(style);
                    }
                    else
                    {
                        expGrid.Rows[i - 1].Cells[1].Value = Convert.ChangeType(
                            mEditorItem.ExperienceCurve.Calculate(i), expGrid.Rows[i - 1].Cells[1].ValueType
                        );

                        expGrid.Rows[i - 1].Cells[1].Style.ApplyStyle(expGrid.Rows[i - 1].Cells[0].InheritedStyle);
                    }
                }
                else
                {
                    expGrid.Rows[i - 1].Cells[1].Value = Convert.ChangeType(0, expGrid.Rows[i - 1].Cells[1].ValueType);
                    expGrid.Rows[i - 1].Cells[1].ReadOnly = true;
                }

                if (i == 1)
                {
                    expGrid.Rows[i - 1].Cells[2].Value = Convert.ChangeType(0, expGrid.Rows[i - 1].Cells[1].ValueType);
                }
                else
                {
                    expGrid.Rows[i - 1].Cells[2].Value = Convert.ChangeType(
                        long.Parse(expGrid.Rows[i - 2].Cells[2].Value.ToString()) +
                        long.Parse(expGrid.Rows[i - 2].Cells[1].Value.ToString()),
                        expGrid.Rows[i - 1].Cells[2].ValueType
                    );
                }
            }
        }

        private void btnCloseExpGrid_Click(object sender, EventArgs e)
        {
            grpExpGrid.Hide();
        }

        private void expGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right &&
                expGrid.CurrentCell != null &&
                expGrid.CurrentCell.IsInEditMode == false)
            {
                var cell = expGrid.CurrentCell;
                if (cell != null)
                {
                    var r = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                    var p = new System.Drawing.Point(r.X + r.Width, r.Y + r.Height);
                    mnuExpGrid.Show((DataGridView) sender, p);
                }
            }
        }

        private void expGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (expGrid.Rows.Count <= 0)
            {
                return;
            }

            var sel = expGrid.SelectedCells;
            if (sel.Count == 0)
            {
                expGrid.Rows[0].Cells[1].Selected = true;
            }
            else
            {
                var selection = sel[0];
                if (selection.ColumnIndex != 1)
                {
                    expGrid.Rows[selection.RowIndex].Cells[1].Selected = true;
                }
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                var s = Clipboard.GetText();
                var lines = s.Split('\n');
                int iFail = 0, iRow = expGrid.CurrentCell.RowIndex;
                var iCol = expGrid.CurrentCell.ColumnIndex;
                DataGridViewCell oCell;
                foreach (var line in lines)
                {
                    if (iRow < expGrid.RowCount && line.Length > 0)
                    {
                        var sCells = line.Split('\t');
                        for (var i = 0; i < 1; ++i)
                        {
                            if (iCol + i < this.expGrid.ColumnCount)
                            {
                                oCell = expGrid[iCol + i, iRow];
                                if (!oCell.ReadOnly)
                                {
                                    if (oCell.Value.ToString() != sCells[i])
                                    {
                                        var val = 0;
                                        if (int.TryParse(sCells[i], out val))
                                        {
                                            if (val > 0)
                                            {
                                                oCell.Value = Convert.ChangeType(val.ToString(), oCell.ValueType);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        iFail++;
                                    }

                                    //only traps a fail if the data has changed 
                                    //and you are pasting into a read only cell
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        iRow++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void expGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(expGrid_KeyPress);
            var tb = e.Control as TextBox;
            if (tb != null)
            {
                tb.KeyPress += new KeyPressEventHandler(expGrid_KeyPress);
            }
        }

        private void expGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void expGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = expGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            long val = 0;
            if (long.TryParse(cell.Value.ToString(), out val))
            {
                if (val == 0 || val == mEditorItem.ExperienceCurve.Calculate(e.RowIndex + 1))
                {
                    if (mEditorItem.ExperienceOverrides.ContainsKey(e.RowIndex + 1))
                    {
                        mEditorItem.ExperienceOverrides.Remove(e.RowIndex + 1);
                    }
                }
                else
                {
                    if (!mEditorItem.ExperienceOverrides.ContainsKey(e.RowIndex + 1))
                    {
                        mEditorItem.ExperienceOverrides.Add(e.RowIndex + 1, val);
                    }
                    else
                    {
                        mEditorItem.ExperienceOverrides[e.RowIndex + 1] = val;
                    }
                }

                UpdateExpGridValues(e.RowIndex + 1);
            }
            else
            {
                UpdateExpGridValues(e.RowIndex + 1, e.RowIndex + 2);
            }
        }

        private void btnResetExpGrid_Click(object sender, EventArgs e)
        {
            mEditorItem.ExperienceOverrides.Clear();
            UpdateExpGridValues(1);
        }

        private void expGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (expGrid.CurrentCell != null)
                {
                    if (!expGrid.CurrentCell.IsInEditMode && expGrid.CurrentCell.ReadOnly == false)
                    {
                        var level = expGrid.CurrentCell.RowIndex + 1;
                        if (mEditorItem.ExperienceOverrides.ContainsKey(level))
                        {
                            mEditorItem.ExperienceOverrides.Remove(level);
                        }

                        UpdateExpGridValues(level);
                    }
                }
            }
        }

        #endregion

        #region "Item List - Folders, Searching, Sorting, Etc"

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ClassEditor.folderprompt, Strings.ClassEditor.foldertitle, ref folderName,
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
                txtSearch.Text = Strings.ClassEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.ClassEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.ClassEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.ClassEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
