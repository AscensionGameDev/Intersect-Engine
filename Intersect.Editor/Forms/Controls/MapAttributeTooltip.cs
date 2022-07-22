using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using DarkUI.Controls;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;

namespace Intersect.Editor.Forms.Controls
{
    public partial class MapAttributeTooltip : FlowLayoutPanel
    {
        private readonly object _mapAttributeLock = new object();
        private MapAttribute _mapAttribute;

        public MapAttributeTooltip()
        {
            InitializeComponent();

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BorderStyle = BorderStyle.FixedSingle;
            FlowDirection = FlowDirection.TopDown;
        }

        public MapAttribute MapAttribute
        {
            get => _mapAttribute;
            set
            {
                lock (_mapAttributeLock)
                {
                    if (_mapAttribute == value)
                    {
                        return;
                    }

                    var oldType = _mapAttribute?.GetType();
                    _mapAttribute = value;
                    OnAttributeChanged(oldType);
                }
            }
        }

        private static Label CreateLabel(string text, AnchorStyles anchor = AnchorStyles.Left, FontStyle fontStyle = FontStyle.Regular)
        {
            return new Label
            {
                Anchor = anchor,
                AutoSize = true,
                Font = new Font(SystemFonts.DefaultFont, fontStyle),
                ForeColor = System.Drawing.Color.White,
                Text = text,
            };
        }

        protected virtual void OnAttributeChanged(Type oldType)
        {
            if (_mapAttribute == null)
            {
                Hide();
                return;
            }

            if (oldType != _mapAttribute.GetType())
            {
                pnlContents.Controls.Clear();
                pnlContents.Controls.Add(lblAttributeTypeLabel, 0, 0);
                pnlContents.Controls.Add(lblAttributeType, 1, 0);
            }

            switch (_mapAttribute)
            {
                case MapAnimationAttribute animationAttribute:
                    lblAttributeType.Text = Strings.Attributes.Animation;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.MapAnimation, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(AnimationBase.Get(animationAttribute.AnimationId)?.Name ?? Strings.General.None, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.MapAnimationBlock, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatBoolean(animationAttribute.IsBlock, BooleanStyle.YesNo), AnchorStyles.Left), 1, 2);
                    break;

                case MapBlockedAttribute _:
                    lblAttributeType.Text = Strings.Attributes.Blocked;
                    break;

                case MapCritterAttribute critterAttribute:
                    lblAttributeType.Text = Strings.Attributes.Critter;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterSprite, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(critterAttribute.Sprite, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterAnimation, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(AnimationBase.Get(critterAttribute.AnimationId)?.Name ?? Strings.General.None, AnchorStyles.Left), 1, 2);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterMovement, AnchorStyles.Right, FontStyle.Bold), 0, 3);
                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterMovements[critterAttribute.Movement], AnchorStyles.Left), 1, 3);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterLayer, AnchorStyles.Right, FontStyle.Bold), 0, 4);
                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterLayers[critterAttribute.Layer], AnchorStyles.Left), 1, 4);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterDirection, AnchorStyles.Right, FontStyle.Bold), 0, 5);
                    pnlContents.Controls.Add(CreateLabel(Strings.Directions.dir[critterAttribute.Direction], AnchorStyles.Left), 1, 5);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterSpeed, AnchorStyles.Right, FontStyle.Bold), 0, 6);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatTimeMilliseconds(critterAttribute.Speed), AnchorStyles.Left), 1, 6);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterFrequency, AnchorStyles.Right, FontStyle.Bold), 0, 7);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatTimeMilliseconds(critterAttribute.Frequency), AnchorStyles.Left), 1, 7);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterIgnoreNpcAvoids, AnchorStyles.Right, FontStyle.Bold), 0, 8);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatBoolean(critterAttribute.IgnoreNpcAvoids, BooleanStyle.YesNo), AnchorStyles.Left), 1, 8);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.CritterBlockPlayers, AnchorStyles.Right, FontStyle.Bold), 0, 9);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatBoolean(critterAttribute.BlockPlayers, BooleanStyle.YesNo), AnchorStyles.Left), 1, 9);
                    break;

                case MapGrappleStoneAttribute _:
                    lblAttributeType.Text = Strings.Attributes.Grapple;
                    break;

                case MapItemAttribute itemAttribute:
                    lblAttributeType.Text = Strings.Attributes.ItemSpawn;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Item, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(ItemBase.Get(itemAttribute.ItemId)?.Name ?? Strings.General.None, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Quantity, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(itemAttribute.Quantity.ToString(CultureInfo.CurrentCulture), AnchorStyles.Left), 1, 2);
                    break;

                case MapNpcAvoidAttribute _:
                    lblAttributeType.Text = Strings.Attributes.NpcAvoid;
                    break;

                case MapResourceAttribute resourceAttribute:
                    lblAttributeType.Text = Strings.Attributes.ResourceSpawn;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Resource, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(ResourceBase.Get(resourceAttribute.ResourceId)?.Name ?? Strings.General.None, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.ZDimension, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(resourceAttribute.SpawnLevel == 0 ? Strings.Attributes.ZLevel1 : Strings.Attributes.ZLevel2, AnchorStyles.Left), 1, 2);
                    break;

                case MapSlideAttribute slideAttribute:
                    lblAttributeType.Text = Strings.Attributes.Slide;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Slide, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(Strings.Directions.dir[slideAttribute.Direction], AnchorStyles.Left), 1, 1);
                    break;

                case MapSoundAttribute soundAttribute:
                    lblAttributeType.Text = Strings.Attributes.MapSound;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Sound, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(soundAttribute.File, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Distance, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(soundAttribute.Distance.ToString(CultureInfo.CurrentCulture), AnchorStyles.Left), 1, 2);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.SoundInterval, AnchorStyles.Right, FontStyle.Bold), 0, 3);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatTimeMilliseconds(soundAttribute.LoopInterval), AnchorStyles.Left), 1, 3);
                    break;

                case MapWarpAttribute warpAttribute:
                    lblAttributeType.Text = Strings.Attributes.Warp;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.Map, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(MapBase.Get(warpAttribute.MapId)?.Name ?? Strings.General.None, AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.WarpX, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(warpAttribute.X.ToString(CultureInfo.CurrentCulture), AnchorStyles.Left), 1, 2);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.WarpY, AnchorStyles.Right, FontStyle.Bold), 0, 3);
                    pnlContents.Controls.Add(CreateLabel(warpAttribute.Y.ToString(CultureInfo.CurrentCulture), AnchorStyles.Left), 1, 3);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.WarpDirection, AnchorStyles.Right, FontStyle.Bold), 0, 4);
                    pnlContents.Controls.Add(CreateLabel(Strings.Directions.dir[(int)warpAttribute.Direction - 1], AnchorStyles.Left), 1, 4);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.WarpSound, AnchorStyles.Right, FontStyle.Bold), 0, 5);
                    pnlContents.Controls.Add(CreateLabel(warpAttribute.WarpSound, AnchorStyles.Left), 1, 5);

                    pnlContents.Controls.Add(CreateLabel(Strings.Warping.ChangeInstance, AnchorStyles.Right, FontStyle.Bold), 0, 6);
                    pnlContents.Controls.Add(CreateLabel(Strings.FormatBoolean(warpAttribute.ChangeInstance, BooleanStyle.YesNo), AnchorStyles.Left), 1, 6);

                    pnlContents.Controls.Add(CreateLabel(Strings.Warping.InstanceType, AnchorStyles.Right, FontStyle.Bold), 0, 7);
                    var instanceTypeString = string.Empty;
                    if ((int)warpAttribute.InstanceType != -1)
                    {
                        instanceTypeString = Strings.Mapping.InstanceTypes[(int)warpAttribute.InstanceType];
                    }
                    pnlContents.Controls.Add(CreateLabel(instanceTypeString, AnchorStyles.Left), 1, 7);
                    break;

                case MapZDimensionAttribute zDimensionAttribute:
                    lblAttributeType.Text = Strings.Attributes.ZDimension;

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.ZGateway, AnchorStyles.Right, FontStyle.Bold), 0, 1);
                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.FormatZLevel(zDimensionAttribute.GatewayTo), AnchorStyles.Left), 1, 1);

                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.ZBlock, AnchorStyles.Right, FontStyle.Bold), 0, 2);
                    pnlContents.Controls.Add(CreateLabel(Strings.Attributes.FormatZLevel(zDimensionAttribute.BlockedLevel), AnchorStyles.Left), 1, 2);
                    break;
            }

            Invalidate();

            Show();
        }
    }
}
