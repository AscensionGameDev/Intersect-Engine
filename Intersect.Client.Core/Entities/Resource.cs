using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Network.Packets.Server;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Resource : Entity, IResource
{
    private FloatRect _renderBoundsDest = FloatRect.Empty;
    private FloatRect _renderBoundsSrc = FloatRect.Empty;

    private bool _waitingForTilesets;
    private bool _recalculateRenderBounds;

    private bool _isDead;
    private int _maximumHealthForStates;
    private ResourceStateDescriptor? _currentState;
    private ResourceDescriptor? _descriptor;
    private AnimationDescriptor? _animationDescriptor;
    private IAnimation? _activeAnimation;
    private IAnimation? _stateAnimation;

    private readonly int _tileWidth = Options.Instance.Map.TileWidth;
    private readonly int _tileHeight = Options.Instance.Map.TileHeight;
    private readonly int _mapHeight = Options.Instance.Map.MapHeight;

    /// <inheritdoc />
    public override bool CanBeAttacked => !IsDead;

    public ResourceStateDescriptor? CurrentState => _currentState;

    public Resource(Guid id, ResourceEntityPacket packet) : base(id, packet, EntityType.Resource)
    {
        mRenderPriority = 0;
    }

    public ResourceDescriptor? Descriptor
    {
        get => _descriptor;
        set
        {
            _descriptor = value;
            if (value is { } descriptor)
            {
                _maximumHealthForStates = (int)(descriptor.UseExplicitMaxHealthForResourceStates
                    ? descriptor.MaxHp
                    : MaxVital[(int)Enums.Vital.Health]);
            }
            else
            {
                _maximumHealthForStates = 0;
            }
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
            ReloadSpriteTexture();
        }
    }

    private void ReloadSpriteTexture()
    {
        if (Descriptor == null)
        {
            return;
        }

        switch (_currentState?.TextureType)
        {
            case ResourceTextureSource.Tileset:
                if (GameContentManager.Current.TilesetsLoaded)
                {
                    Texture = GameContentManager.Current.GetTexture(TextureType.Tileset, _sprite);
                }
                else
                {
                    _waitingForTilesets = true;
                }
                break;

            case ResourceTextureSource.Resource:
                Texture = GameContentManager.Current.GetTexture(TextureType.Resource, _sprite);
                break;

            case ResourceTextureSource.Animation:
                if (_stateAnimation?.Descriptor?.Id == _currentState.AnimationId)
                {
                    return;
                }

                _stateAnimation?.Dispose();
                _stateAnimation = null;

                if (MapInstance is not { } mapInstance)
                {
                    return;
                }

                if (_animationDescriptor is not { } animationDescriptor)
                {
                    return;
                }

                var animation = mapInstance.AddTileAnimation(animationDescriptor, X, Y, Direction.Up);
                if (animation is { IsDisposed: false })
                {
                    animation.InfiniteLoop = true;
                }
                _stateAnimation = animation;
                break;
        }

        if (Texture == default)
        {
            Texture = Graphics.Renderer.WhitePixel;
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

        Descriptor = descriptor;
        UpdateCurrentState();

        if (!justDied)
        {
            return;
        }

        if (MapInstance is { } mapInstance)
        {
            var animation = mapInstance.AddTileAnimation(descriptor.DeathAnimationId, X, Y, Direction.Up);
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

    public void UpdateCurrentState()
    {
        if (Descriptor == default)
        {
            return;
        }

        var graphicStates = Descriptor.States;
        var currentHealthPercentage = Math.Floor((float)Vital[(int)Enums.Vital.Health] / _maximumHealthForStates * 100);

        if (_currentState is { } currentState &&
            currentHealthPercentage >= _currentState?.MinimumHealth &&
            currentHealthPercentage <= _currentState?.MaximumHealth
        )
        {
            return;
        }

        currentState = graphicStates.Values.FirstOrDefault(
            s => currentHealthPercentage >= s.MinimumHealth && currentHealthPercentage <= s.MaximumHealth
        );

        _currentState = currentState;
        _sprite = _currentState?.TextureName ?? string.Empty;

        if (currentState is { TextureType: ResourceTextureSource.Animation } && currentState.AnimationId != Guid.Empty)
        {
            _ = AnimationDescriptor.TryGet(currentState.AnimationId, out _animationDescriptor);
        }

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
            _ = ResourceDescriptor.TryGet(deletedDescriptor.Id, out var descriptor);
            Descriptor = descriptor;
            UpdateCurrentState();
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

    public override HashSet<Entity>? DetermineRenderOrder(HashSet<Entity>? renderList, IMapInstance? map)
    {
        if (Descriptor == default || CurrentState is not { } graphicState || !graphicState.RenderBelowEntities)
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
                            renderSet = Graphics.RenderingEntities[priority, _mapHeight + Y];
                        }
                        else
                        {
                            renderSet = Graphics.RenderingEntities[priority, _mapHeight * 2 + Y];
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

        if (Texture is not { } texture)
        {
            return;
        }

        if (CurrentState is not { } graphicState)
        {
            return;
        }

        _renderBoundsSrc.X = 0;
        _renderBoundsSrc.Y = 0;

        switch(graphicState.TextureType)
        {
            case ResourceTextureSource.Resource:
                _renderBoundsSrc.Width = texture.Width;
                _renderBoundsSrc.Height = texture.Height;
                break;

            case ResourceTextureSource.Tileset:
                if (IsDead)
                {
                    if (graphicState is { MaximumHealth: 0 } deadGraphic)
                    {
                        _renderBoundsSrc = new(
                            deadGraphic.X * _tileWidth,
                            deadGraphic.Y * _tileHeight,
                            (deadGraphic.Width + 1) * _tileWidth,
                            (deadGraphic.Height + 1) * _tileHeight
                        );
                    }
                    else
                    {
                        _renderBoundsSrc = new();
                    }
                }
                else if (graphicState is { MinimumHealth: > 0 } aliveGraphic)
                {
                    _renderBoundsSrc = new(
                        aliveGraphic.X * _tileWidth,
                        aliveGraphic.Y * _tileHeight,
                        (aliveGraphic.Width + 1) * _tileWidth,
                        (aliveGraphic.Height + 1) * _tileHeight
                    );
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

        Graphics.DrawGameTexture(Texture, _renderBoundsSrc, _renderBoundsDest, Color.White);
    }
}
