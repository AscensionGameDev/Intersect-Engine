using Intersect.Client.Core;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Core;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Network.Packets.Server;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities.Events;

public partial class Event : Entity
{
    private bool _drawCompletedWithoutTexture = false;

    public string Desc { get; set; } = string.Empty;

    public bool DirectionFix { get; set; }

    public bool DisablePreview { get; set; }

    public EventGraphic Graphic { get; set; } = new EventGraphic();

    public int RenderLevel { get; set; } = 1;

    public bool WalkingAnim { get; set; } = true;

    public EventTrigger Trigger { get; set; }

    protected override Pointf CenterOffset
    {
        get
        {
            switch (Graphic.Type)
            {
                case EventGraphicType.None:
                    return HasAnimations ? Pointf.UnitY * Options.Instance.Map.TileHeight / 2f : Pointf.Empty;

                case EventGraphicType.Sprite:
                    return base.CenterOffset;

                case EventGraphicType.Tileset:
                    return Pointf.UnitY * Options.Instance.Map.TileHeight * (Graphic.Height + 1) / 2f;

                default:
                    ApplicationContext.Context.Value?.Logger.LogError($"Unimplemented graphic type: {Graphic.Type}");
                    return Pointf.Empty;
            }
        }
    }

    public Event(Guid id, EventEntityPacket packet) : base(id, packet, EntityType.Event)
    {
        mRenderPriority = 1;
    }

    public override void Load(EntityPacket? packet)
    {
        if (packet is not EventEntityPacket eventEntityPacket)
        {
            ApplicationContext.Context.Value?.Logger.LogError($"Received invalid packet for {nameof(Event)}: {packet?.GetType()?.FullName}");
            return;
        }

        DirectionFix = eventEntityPacket.DirectionFix;
        WalkingAnim = eventEntityPacket.WalkingAnim;
        DisablePreview = eventEntityPacket.DisablePreview;
        Desc = eventEntityPacket.Description;
        Graphic = eventEntityPacket.Graphic;
        RenderLevel = eventEntityPacket.RenderLayer;
        Trigger = eventEntityPacket.Trigger;

        _drawCompletedWithoutTexture = Graphic.Type != EventGraphicType.Tileset;

        base.Load(packet);

        if (!string.IsNullOrEmpty(Graphic?.Filename))
        {
            Sprite = Graphic.Filename;
        }
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

    protected bool TryEnsureTexture(out GameTexture? texture)
    {
        if (_drawCompletedWithoutTexture)
        {
            texture = Texture;
            return texture != default;
        }

        LoadTextures(Sprite);
        texture = Texture;
        _drawCompletedWithoutTexture = (texture == default);
        return !_drawCompletedWithoutTexture;
    }

    public override void Draw()
    {
        WorldPos.Reset();
        if (MapInstance == default || !Globals.GridMaps.ContainsKey(MapId) || !TryEnsureTexture(out var texture))
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
                var width = (Graphic.Width + 1) * Options.Instance.Map.TileWidth;
                var height = (Graphic.Height + 1) * Options.Instance.Map.TileHeight;
                srcRectangle = new FloatRect(
                    Graphic.X * Options.Instance.Map.TileWidth,
                    Graphic.Y * Options.Instance.Map.TileHeight,
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
            X = map.X + X * Options.Instance.Map.TileWidth + OffsetX,
            Y = map.Y + Y * Options.Instance.Map.TileHeight + OffsetY,
            Width = Math.Max(Options.Instance.Map.TileWidth, srcRectangle.Width),
            Height = Math.Max(Options.Instance.Map.TileHeight, srcRectangle.Height),
        };

        if (srcRectangle.Width > Options.Instance.Map.TileWidth)
        {
            destRectangle.X -= (srcRectangle.Width - Options.Instance.Map.TileWidth) / 2;
        }

        if (srcRectangle.Height > Options.Instance.Map.TileHeight)
        {
            destRectangle.Y -= srcRectangle.Height - Options.Instance.Map.TileHeight;
        }

        destRectangle.X = (int)Math.Ceiling(destRectangle.X);
        destRectangle.Y = (int)Math.Ceiling(destRectangle.Y);

        // Set up our targetting rectangle.
        // If we're smaller than a tile, force the target size to a tile.
        WorldPos = destRectangle;

        if (texture == default)
        {
            return;
        }

        Graphics.DrawGameTexture(texture, srcRectangle, destRectangle, Color.White);
    }

    public override void LoadTextures(string textureName)
    {
        switch (Graphic.Type)
        {
            case EventGraphicType.Tileset:
                Texture = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Tileset, textureName);
                break;

            case EventGraphicType.Sprite:
                base.LoadTextures(textureName);
                break;

            default:
                break;
        }
    }

    public override HashSet<Entity>? DetermineRenderOrder(HashSet<Entity>? renderList, IMapInstance? map)
    {
        if (RenderLevel == 1)
        {
            return base.DetermineRenderOrder(renderList, map);
        }

        _ = (renderList?.Remove(this));
        if (map == null || Globals.Me == null || Globals.Me.MapInstance == null || Globals.MapGrid == default)
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
                    var renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight * maps + Y];

                    _ = (renderSet?.Add(this));
                    if(renderSet != default)
                    {
                        renderList = renderSet;
                    }

                    return renderList;
                }
            }
        }

        return renderList;
    }

    public override float GetTop(int overrideHeight = -1)
    {
        float heightScale;
        switch (Graphic.Type)
        {
            case EventGraphicType.Sprite:
                return base.GetTop(overrideHeight);

            case EventGraphicType.Tileset:
                heightScale = Graphic.Height + 1;
                break;

            case EventGraphicType.None:
                heightScale = HasAnimations ? 1 : 0.5f;
                break;

            default:
                throw new IndexOutOfRangeException($"Unsupported {nameof(EventGraphicType)} '{Graphic.Type}'");
        }

        var top = base.GetTop(0);
        var offset = heightScale * Options.Instance.Map.TileHeight;
        return top - offset;
    }

    public override void DrawName(Color? textColor, Color? borderColor, Color? backgroundColor)
    {
        base.DrawName(
            textColor: textColor ?? new Color(CustomColors.Names.Events.Name),
            borderColor: borderColor ?? new Color(CustomColors.Names.Events.Outline),
            backgroundColor: backgroundColor ?? new Color(CustomColors.Names.Events.Background)
        );
    }

    ~Event() => Dispose();
}
