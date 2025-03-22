using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Network.Packets.Server;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Resource : Entity, IResource
{
    private FloatRect _renderBoundsDest = FloatRect.Empty;
    private FloatRect _renderBoundsSrc = FloatRect.Empty;

    private bool _waitingForTilesets;

    private bool _isDead;
    private string _currentStateKey = string.Empty;
    private ResourceDescriptor? _descriptor;
    private IAnimation? _activeAnimation;

    private readonly int _tileWidth = Options.Instance.Map.TileWidth;
    private readonly int _tileHeight = Options.Instance.Map.TileHeight;
    private readonly int _tapHeight = Options.Instance.Map.MapHeight;

    public Resource(Guid id, ResourceEntityPacket packet) : base(id, packet, EntityType.Resource)
    {
        mRenderPriority = 0;
    }

    public ResourceDescriptor? Descriptor
    {
        get => _descriptor;
        set => _descriptor = value;
    }

    public ResourceStateDescriptor? CurrentGraphicState
    {
        get
        {
            if (Descriptor == default)
            {
                return default;
            }

            var graphicStates = Descriptor.StatesGraphics.ToList();
            var maxHealth = Descriptor.UseExplicitMaxHealthForResourceStates
                ? Descriptor.MaxHp
                : MaxVital[(int)Enums.Vital.Health];
            var currentHealthPercentage = Math.Floor((float)Vital[(int)Enums.Vital.Health] / maxHealth * 100);
            var currentState = graphicStates.FirstOrDefault(
                s => currentHealthPercentage >= s.Value.MinimumHealth && currentHealthPercentage <= s.Value.MaximumHealth
            );

            if (currentState.Value == default)
            {
                _sprite = string.Empty;
                Texture = default;
                return default;
            }

            if (currentState.Value.TextureType == ResourceTextureSource.Animation)
            {
                return currentState.Value;
            }

            if (currentState.Key != _currentStateKey)
            {
                _currentStateKey = currentState.Key;
                _sprite = currentState.Value.Texture;
                var textureType = currentState.Value.TextureType == ResourceTextureSource.Tileset
                    ? TextureType.Tileset
                    : TextureType.Resource;
                Texture = GameContentManager.Current.GetTexture(textureType, _sprite);
                CalculateRenderBounds();
            }

            return currentState.Value;
        }
    }

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
        }
    }

    public override void Load(EntityPacket? packet)
    {
        base.Load(packet);

        if (packet is not ResourceEntityPacket resourceEntityPacket)
        {
            return;
        }

        var wasDead = IsDead;
        IsDead = resourceEntityPacket.IsDead;

        var descriptorId = resourceEntityPacket.ResourceId;

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

    public override bool Update()
    {
        if (mDisposed)
        {
            LatestMap = null;

            return false;
        }

        if (!Maps.MapInstance.TryGet(MapId, out var map) || !map.InView())
        {
            LatestMap = map;
            Globals.EntitiesToDispose.Add(Id);

            return false;
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
        if (Descriptor == default || CurrentGraphicState is not { } graphicState || !graphicState.RenderBelowEntities)
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

        if (Graphics.RenderingEntities == default)
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
                            renderSet = Graphics.RenderingEntities[priority, _tapHeight + Y];
                        }
                        else
                        {
                            renderSet = Graphics.RenderingEntities[priority, _tapHeight * 2 + Y];
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

        if (CurrentGraphicState is not { } graphicState)
        {
            return;
        }

        _renderBoundsSrc.X = 0;
        _renderBoundsSrc.Y = 0;

        switch(graphicState.TextureType)
        {
            case ResourceTextureSource.Resource:
                _renderBoundsSrc.Width = Texture.Width;
                _renderBoundsSrc.Height = Texture.Height;
                break;

            case ResourceTextureSource.Tileset:
                if (IsDead && graphicState is { MaximumHealth: 0 } deadGraphic)
                {
                    _renderBoundsSrc = new(
                        deadGraphic.X * _tileWidth,
                        deadGraphic.Y * _tileHeight,
                        (deadGraphic.Width + 1) * _tileWidth,
                        (deadGraphic.Height + 1) * _tileHeight
                    );
                }
                else if (!IsDead && graphicState is { TextureType: ResourceTextureSource.Tileset, MinimumHealth: > 0 } aliveGraphic)
                {
                    _renderBoundsSrc = new(
                        aliveGraphic.X * _tileWidth,
                        aliveGraphic.Y * _tileHeight,
                        (aliveGraphic.Width + 1) * _tileWidth,
                        (aliveGraphic.Height + 1) * _tileHeight
                    );
                }
                else if (IsDead && graphicState is not { MaximumHealth: 0} reloadingGraphic)
                {
                    // some game dev pressed save on resource editor, tilesets are reloading, show nothing
                    _renderBoundsSrc = new();
                }
                break;

            case ResourceTextureSource.Animation:
                break;

            default:
                return;
        }

        _renderBoundsDest.Width = _renderBoundsSrc.Width;
        _renderBoundsDest.Height = _renderBoundsSrc.Height;
        _renderBoundsDest.Y = (int) (map.Y + Y * _tileHeight + OffsetY);
        _renderBoundsDest.X = (int) (map.X + X * _tileWidth + OffsetX);

        if (_renderBoundsSrc.Height > _tileHeight)
        {
            _renderBoundsDest.Y -= _renderBoundsSrc.Height - _tileHeight;
        }

        if (_renderBoundsSrc.Width > _tileWidth)
        {
            _renderBoundsDest.X -= (_renderBoundsSrc.Width - _tileWidth) / 2;
        }
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

        Graphics.DrawGameTexture(Texture, _renderBoundsSrc, _renderBoundsDest, Color.White);
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
}
