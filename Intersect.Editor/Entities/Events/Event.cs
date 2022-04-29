using System;
using System.Collections.Generic;
using System.Globalization;

using Intersect.Editor.Core;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Maps;
using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Logging;
using Intersect.Network.Packets.Server;

namespace Intersect.Editor.Entities.Events
{
    public partial class Event : Entity
    {
        public string Desc { get; set; } = string.Empty;

        public bool DirectionFix { get; set; }

        public bool DisablePreview { get; set; }

        public string FaceGraphic { get; set; } = string.Empty;

        public EventGraphic Graphic { get; set; } = new EventGraphic();

        public int Layer { get; set; }

        private int mOldRenderLevel { get; set; }

        private MapInstance mOldRenderMap { get; set; }

        private int mOldRenderY { get; set; }

        public int RenderLevel { get; set; } = 1;

        public bool WalkingAnim { get; set; } = true;

        public Event(Guid id, EventEntityPacket packet) : base(id, packet, EntityTypes.Event)
        {
            mRenderPriority = 1;
        }

        public override void Load(EntityPacket packet)
        {
            if (!(packet is EventEntityPacket eventEntityPacket))
            {
                Log.Error($"Received invalid packet for {nameof(Event)}: {packet?.GetType()?.FullName}");
                return;
            }
            DirectionFix = eventEntityPacket.DirectionFix;
            WalkingAnim = eventEntityPacket.WalkingAnim;
            DisablePreview = eventEntityPacket.DisablePreview;
            Desc = eventEntityPacket.Description;
            Graphic = eventEntityPacket.Graphic;
            RenderLevel = eventEntityPacket.RenderLayer;
            base.Load(packet);
        }

        public override bool Update()
        {
            var success = base.Update();
            if (!WalkingAnim)
            {
                WalkFrame = 0;
            }

            return success;
        }

        public override void Draw()
        {
            WorldPos.Reset();
            if (MapInstance == default || !Globals.GridMaps.Contains(MapId) || Texture == default)
            {
                return;
            }

            FloatRect srcRectangle;
            switch (Graphic.Type)
            {
                case EventGraphicType.Sprite: //Sprite
                    base.Draw();
                    return;

                case EventGraphicType.Tileset: //Tile
                    var width = (Graphic.Width + 1) * Options.TileWidth;
                    var height = (Graphic.Height + 1) * Options.TileHeight;
                    srcRectangle = new FloatRect(
                        Graphic.X * Options.TileWidth,
                        Graphic.Y * Options.TileHeight,
                        width,
                        height
                    );
                    break;

                default:
                    return;
            }

            var map = Maps.MapInstance.Get(MapId);

            var destRectangle = new FloatRect
            {
                X = map.GetX() + X * Options.TileWidth + OffsetX,
                Y = map.GetY() + Y * Options.TileHeight + OffsetY,
                Width = Math.Max(Options.TileWidth, srcRectangle.Width),
                Height = Math.Max(Options.TileHeight, srcRectangle.Height),
            };

            if (srcRectangle.Width > Options.TileWidth)
            {
                destRectangle.X -= (srcRectangle.Width - Options.TileWidth) / 2;
            }

            if (srcRectangle.Height > Options.TileHeight)
            {
                destRectangle.Y -= srcRectangle.Height - Options.TileHeight;
            }

            destRectangle.X = (int) Math.Ceiling(destRectangle.X);
            destRectangle.Y = (int) Math.Ceiling(destRectangle.Y);

            // Set up our targetting rectangle.
            // If we're smaller than a tile, force the target size to a tile.
            WorldPos = destRectangle;

            if (Texture != null)
            {
                Graphics.DrawGameTexture(Texture, srcRectangle, destRectangle, Color.White);
            }
        }

        public override void LoadTextures(string textureName)
        {
            switch (Graphic.Type)
            {
                case EventGraphicType.Tileset:
                    Texture = Globals.ContentManager.GetTexture(Client.Framework.Content.TextureType.Tileset, textureName);
                    break;

                case EventGraphicType.Sprite:
                    base.LoadTextures(textureName);
                    break;

                default:
                    break;
            }
        }

        public override HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, IMapInstance map)
        {
            if (RenderLevel == 1)
            {
                return base.DetermineRenderOrder(renderList, map);
            }

            renderList?.Remove(this);
            if (map == null || Globals.Me == null || Globals.Me.MapInstance == null)
            {
                return null;
            }

            var gridX = Globals.Me.MapInstance.GridX;
            var gridY = Globals.Me.MapInstance.GridY;
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        if (Globals.MapGrid[x, y] != MapId)
                        {
                            continue;
                        }

                        if (RenderLevel == 0)
                        {
                            y--;
                        }
                        else if (RenderLevel == 2)
                        {
                            y++;
                        }

                        var priority = mRenderPriority;
                        if (Z != 0)
                        {
                            priority += 3;
                        }

                        var maps = y - (gridY - 2);
                        var renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * maps + Y];

                        // If bugs arise from switching to the above, remove and uncomment this
                        //HashSet<Entity> renderSet = null;
                        //if (y == gridY - 2)
                        //{
                        //    renderSet = Graphics.RenderingEntities[priority, Y];
                        //}
                        //else if (y == gridY - 1)
                        //{
                        //    renderSet = Graphics.RenderingEntities[priority, Options.MapHeight + Y];
                        //}
                        //else if (y == gridY)
                        //{
                        //    renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 2 + Y];
                        //}
                        //else if (y == gridY + 1)
                        //{
                        //    renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 3 + Y];
                        //}
                        //else if (y == gridY + 2)
                        //{
                        //    renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 4 + Y];
                        //}

                        renderSet?.Add(this);
                        renderList = renderSet;

                        return renderList;
                    }
                }
            }

            return renderList;
        }

        public override float GetTop(int overrideHeight = -1)
        {
            if (Graphic.Type == EventGraphicType.Tileset)
            {
                var topPosition = base.GetTop(0);
                topPosition -= (Graphic.Height + 1) * Options.TileHeight;
                return topPosition;
            }

            return base.GetTop(overrideHeight);
        }

        public override void DrawName(Color textColor, Color borderColor, Color backgroundColor)
        {
            if (HideName || string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            if (!WorldPos.IntersectsWith(Graphics.Renderer.GetView()))
            {
                return;
            }

            if (LatestMap == default || !Globals.GridMaps.Contains(MapId))
            {
                return;
            }

            if (Graphic.Type == EventGraphicType.Sprite)
            {
                base.DrawName(textColor, borderColor, backgroundColor);
                return;
            }

            var x = (int)Math.Ceiling(Origin.X);
            var y = GetTop();

            var textSize = Graphics.Renderer.MeasureText(Name, Graphics.EntityNameFont, 1);

            y -= textSize.Y + 2;

            if (CustomColors.Names.Events.Background != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(),
                    new FloatRect(0, 0, 1, 1),
                    new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y),
                    CustomColors.Names.Events.Background
                );
            }

            Graphics.Renderer.DrawString(
                Name, Graphics.EntityNameFont, x - (int)Math.Ceiling(textSize.X / 2f), (int) y, 1,
                Color.FromArgb(CustomColors.Names.Events.Name.ToArgb()), true, null,
                Color.FromArgb(CustomColors.Names.Events.Outline.ToArgb())
            );
        }

        ~Event() => Dispose();
    }
}
