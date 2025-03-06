using Intersect.Client.Core;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Resource : Entity, IResource
{
    private FloatRect _renderBoundsDest = FloatRect.Empty;
    private FloatRect _renderBoundsSrc = FloatRect.Empty;

    private bool _recalculateRenderBounds;
    private bool _waitingForTilesets;

    private bool _isDead;
    private Guid _descriptorId;
    private ResourceDescriptor? _descriptor;
    private IAnimation? _activeAnimation;

    public Resource(Guid id, ResourceEntityPacket packet) : base(id, packet, EntityType.Resource)
    {
        mRenderPriority = 0;
    }

    public ResourceDescriptor? Descriptor
    {
        get => _descriptor;
        set => _descriptor = value;
    }

    public bool IsDepleted => IsDead;

    public bool IsDead
    {
        get => _isDead;
        set
        {
            if (value == _isDead)
            {
                return;
            }

            _isDead = value;
            _recalculateRenderBounds = true;
        }
    }

    public override string Sprite
    {
        get => _sprite;
        set
        {
            if (value == _sprite)
            {
                return;
            }

            if (Descriptor == null)
            {
                return;
            }

            _sprite = value;
            ReloadSpriteTexture();
        }
    }

    private void ReloadSpriteTexture()
    {
        if (Descriptor == null)
        {
            return;
        }

        if (IsDead && Descriptor.Exhausted.GraphicFromTileset ||
            !IsDead && Descriptor.Initial.GraphicFromTileset)
        {
            if (GameContentManager.Current.TilesetsLoaded)
            {
                Texture = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Tileset, _sprite);
            }
            else
            {
                _waitingForTilesets = true;
            }
        }
        else
        {
            Texture = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Resource, _sprite);
        }

        _recalculateRenderBounds = true;
    }

    public override void Load(EntityPacket? packet)
    {
        base.Load(packet);

        _recalculateRenderBounds = true;

        if (packet is not ResourceEntityPacket resourceEntityPacket)
        {
            return;
        }

        var wasDead = IsDead;
        IsDead = resourceEntityPacket.IsDead;

        var descriptorId = resourceEntityPacket.ResourceId;
        _descriptorId = descriptorId;

        var justDied = !wasDead && resourceEntityPacket.IsDead;
        if (!ResourceDescriptor.TryGet(descriptorId, out var descriptor))
        {
            if (justDied)
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Unable to play resource exhaustion animation because resource {EntityId} ({EntityName}) is missing the descriptor ({DescriptorId})",
                    Id,
                    Name,
                    descriptorId
                );
            }

            return;
        }

        _descriptor = descriptor;
        UpdateFromDescriptor(_descriptor);

        if (!justDied)
        {
            return;
        }

        if (MapInstance is { } mapInstance)
        {
            var animation = mapInstance.AddTileAnimation(descriptor.AnimationId, X, Y, Direction.Up);
            if (animation is { IsDisposed: false })
            {
                animation.Finished += OnAnimationDisposedOrFinished;
                animation.Disposed += OnAnimationDisposedOrFinished;
            }
            _activeAnimation = animation;
        }
        else
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Unable to play resource exhaustion animation because resource {EntityId} ({EntityName}) has no reference to the map instance for map {MapId}",
                Id,
                Name,
                MapId
            );
        }
    }

    private void OnAnimationDisposedOrFinished(IAnimation animation)
    {
        if (_activeAnimation != animation)
        {
            return;
        }

        _activeAnimation = null;
        animation.Disposed -= OnAnimationDisposedOrFinished;
        animation.Finished -= OnAnimationDisposedOrFinished;
    }

    private void UpdateFromDescriptor(ResourceDescriptor? descriptor)
    {
        if (descriptor == null)
        {
            return;
        }

        var updatedSprite = IsDead ? descriptor.Exhausted.Graphic : descriptor.Initial.Graphic;
        _sprite = updatedSprite;
        ReloadSpriteTexture();
    }

    public override void Dispose()
    {
        if (RenderList != null)
        {
            _ = RenderList.Remove(this);
            RenderList = null;
        }

        ClearAnimations();
        GC.SuppressFinalize(this);
        mDisposed = true;
    }

    public override bool Update()
    {
        if (mDisposed)
        {
            LatestMap = null;

            return false;
        }

        if (Descriptor is { IsDeleted: true } deletedDescriptor)
        {
            _ = ResourceDescriptor.TryGet(deletedDescriptor.Id, out _descriptor);
            UpdateFromDescriptor(_descriptor);
        }

        if (!Maps.MapInstance.TryGet(MapId, out var map) || !map.InView())
        {
            LatestMap = map;
            Globals.EntitiesToDispose.Add(Id);

            return false;
        }

        if (_recalculateRenderBounds)
        {
            CalculateRenderBounds();
        }

        if (!Graphics.WorldViewport.IntersectsWith(_renderBoundsDest))
        {
            if (RenderList != null)
            {
                _ = RenderList.Remove(this);
            }

            return true;
        }

        var result = base.Update();
        if (!result)
        {
            if (RenderList != null)
            {
                _ = RenderList.Remove(this);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanBeAttacked => !IsDead;

    public override HashSet<Entity>? DetermineRenderOrder(HashSet<Entity>? renderList, IMapInstance? map)
    {
        if (Descriptor == default ||
            (IsDead && !Descriptor.Exhausted.RenderBelowEntities) ||
            (!IsDead && !Descriptor.Initial.RenderBelowEntities)
        )
        {
            return base.DetermineRenderOrder(renderList, map);
        }

        //Otherwise we are alive or dead and we want to render below players/npcs
        if (renderList != null)
        {
            _ = renderList.Remove(this);
        }

        if (map == null)
        {
            return null;
        }

        if (Globals.MapGrid == default)
        {
            return null;
        }

        if (Globals.Me?.MapInstance == null)
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
                    if (Globals.MapGrid[x, y] == MapId)
                    {
                        var priority = mRenderPriority;
                        if (Z != 0)
                        {
                            priority += 3;
                        }

                        HashSet<Entity> renderSet;

                        if (y == gridY - 1)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Y];
                        }
                        else if (y == gridY)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight + Y];
                        }
                        else
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight * 2 + Y];
                        }

                        _ = renderSet.Add(this);
                        renderList = renderSet;

                        return renderList;

                    }
                }
            }
        }

        return renderList;
    }

    private void CalculateRenderBounds()
    {
        if (Descriptor == default)
        {
            return;
        }

        if (MapInstance is not { } map)
        {
            return;
        }

        if (_waitingForTilesets)
        {
            if (GameContentManager.Current.TilesetsLoaded)
            {
                ReloadSpriteTexture();
                _waitingForTilesets = false;
            }
            else
            {
                // No textures yet
                return;
            }
        }

        if (Texture == null)
        {
            return;
        }

        _renderBoundsSrc.X = 0;
        _renderBoundsSrc.Y = 0;
        if (IsDead && Descriptor.Exhausted.GraphicFromTileset)
        {
            _renderBoundsSrc.X = Descriptor.Exhausted.X * Options.Instance.Map.TileWidth;
            _renderBoundsSrc.Y = Descriptor.Exhausted.Y * Options.Instance.Map.TileHeight;
            _renderBoundsSrc.Width = (Descriptor.Exhausted.Width + 1) * Options.Instance.Map.TileWidth;
            _renderBoundsSrc.Height = (Descriptor.Exhausted.Height + 1) * Options.Instance.Map.TileHeight;
        }
        else if (!IsDead && Descriptor.Initial.GraphicFromTileset)
        {
            _renderBoundsSrc.X = Descriptor.Initial.X * Options.Instance.Map.TileWidth;
            _renderBoundsSrc.Y = Descriptor.Initial.Y * Options.Instance.Map.TileHeight;
            _renderBoundsSrc.Width = (Descriptor.Initial.Width + 1) * Options.Instance.Map.TileWidth;
            _renderBoundsSrc.Height = (Descriptor.Initial.Height + 1) * Options.Instance.Map.TileHeight;
        }
        else
        {
            _renderBoundsSrc.Width = Texture.Width;
            _renderBoundsSrc.Height = Texture.Height;
        }

        _renderBoundsDest.Width = _renderBoundsSrc.Width;
        _renderBoundsDest.Height = _renderBoundsSrc.Height;
        _renderBoundsDest.Y = (int) (map.Y + Y * Options.Instance.Map.TileHeight + OffsetY);
        _renderBoundsDest.X = (int) (map.X + X * Options.Instance.Map.TileWidth + OffsetX);
        if (_renderBoundsSrc.Height > Options.Instance.Map.TileHeight)
        {
            _renderBoundsDest.Y -= _renderBoundsSrc.Height - Options.Instance.Map.TileHeight;
        }

        if (_renderBoundsSrc.Width > Options.Instance.Map.TileWidth)
        {
            _renderBoundsDest.X -= (_renderBoundsSrc.Width - Options.Instance.Map.TileWidth) / 2;
        }

        _recalculateRenderBounds = false;
    }

    //Rendering Resources
    public override void Draw()
    {
        if (MapInstance == null)
        {
            return;
        }

        if (Texture == null)
        {
            return;
        }

        // TODO: Add an option to show the exhausted sprite until the exhaustion animation is finished, but this is not necessary if the graphics line up like Blinkuz' sample provided to fix #2572
        if (_activeAnimation != null)
        {
            return;
        }

        Graphics.DrawGameTexture(Texture, _renderBoundsSrc, _renderBoundsDest, Intersect.Color.White);
    }
}
