using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing;

public readonly record struct FivePatch : IAtlasDrawable
{
    private readonly IGameTexture _texture;
    private readonly float _textureWidth;
    private readonly float _textureHeight;
    private readonly UVSquare[] _uvSquares;
    private readonly Rectangle[] _debugUVSquares;
    private readonly Margin _exteriorMargin;
    private readonly Margin _interiorMargin;
    private readonly float _x;
    private readonly float _y;
    private readonly float _width;
    private readonly float _height;
    private readonly int _interiorWidth;
    private readonly int _interiorHeight;

    public FivePatch(
        IGameTexture texture,
        int x,
        int y,
        int width,
        int height,
        Margin exteriorMargin,
        Margin interiorMargin
    ) : this()
    {
        _texture = texture;
        _textureWidth = _texture.Width;
        _textureHeight = _texture.Height;
        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _exteriorMargin = exteriorMargin;
        _interiorMargin = interiorMargin;
        _interiorWidth = width - (exteriorMargin.Left + interiorMargin.Left + interiorMargin.Right + exteriorMargin.Right);
        _interiorHeight = height - (exteriorMargin.Top + interiorMargin.Top + interiorMargin.Bottom + exteriorMargin.Bottom);
        _uvSquares = new UVSquare[25];
        _debugUVSquares = new Rectangle[25];

        Init(x, y, width, height);
    }

    [SuppressMessage("ReSharper", "UselessBinaryOperation")]
    private void Init(
        int x,
        int y,
        int w,
        int h
    )
    {
        var offset = 5 * 0;
        SetUVSquare(offset + 0, x, y, _exteriorMargin.Left, _exteriorMargin.Top);
        SetUVSquare(offset + 1, x + _exteriorMargin.Left, y, _interiorMargin.Left, _exteriorMargin.Top);
        SetUVSquare(offset + 2, x + _exteriorMargin.Left + _interiorMargin.Left, y, _interiorWidth, _exteriorMargin.Top);
        SetUVSquare(offset + 3, x + w - (_exteriorMargin.Right + _interiorMargin.Right), y, _interiorMargin.Right, _exteriorMargin.Top);
        SetUVSquare(offset + 4, x + w - _exteriorMargin.Right, y, _exteriorMargin.Right, _exteriorMargin.Top);

        offset = 5 * 1;
        SetUVSquare(offset + 0, x, y + _exteriorMargin.Top, _exteriorMargin.Left, _interiorMargin.Top);
        SetUVSquare(offset + 1, x + _exteriorMargin.Left, y + _exteriorMargin.Top, _interiorMargin.Left, _interiorMargin.Top);
        SetUVSquare(offset + 2, x + _exteriorMargin.Left + _interiorMargin.Left, y + _exteriorMargin.Top, _interiorWidth, _interiorMargin.Top);
        SetUVSquare(offset + 3, x + w - (_exteriorMargin.Right + _interiorMargin.Right), y + _exteriorMargin.Top, _interiorMargin.Right, _interiorMargin.Top);
        SetUVSquare(offset + 4, x + w - _exteriorMargin.Right, y + _exteriorMargin.Top, _exteriorMargin.Right, _interiorMargin.Top);

        offset = 5 * 2;
        SetUVSquare(offset + 0, x, y + _exteriorMargin.Top + _interiorMargin.Top, _exteriorMargin.Left, _interiorHeight);
        SetUVSquare(offset + 1, x + _exteriorMargin.Left, y + _exteriorMargin.Top + _interiorMargin.Top, _interiorMargin.Left, _interiorHeight);
        SetUVSquare(offset + 2, x + _exteriorMargin.Left + _interiorMargin.Left, y + _exteriorMargin.Top + _interiorMargin.Top, _interiorWidth, _interiorHeight);
        SetUVSquare(offset + 3, x + w - (_exteriorMargin.Right + _interiorMargin.Right), y + _exteriorMargin.Top + _interiorMargin.Top, _interiorMargin.Right, _interiorHeight);
        SetUVSquare(offset + 4, x + w - _exteriorMargin.Right, y + _exteriorMargin.Top + _interiorMargin.Top, _exteriorMargin.Right, _interiorHeight);

        offset = 5 * 3;
        SetUVSquare(offset + 0, x, y + h - (_exteriorMargin.Bottom + _interiorMargin.Bottom), _exteriorMargin.Left, _interiorMargin.Bottom);
        SetUVSquare(offset + 1, x + _exteriorMargin.Left, y + h - (_exteriorMargin.Bottom + _interiorMargin.Bottom), _interiorMargin.Left, _interiorMargin.Bottom);
        SetUVSquare(offset + 2, x + _exteriorMargin.Left + _interiorMargin.Left, y + h - (_exteriorMargin.Bottom + _interiorMargin.Bottom), _interiorWidth, _interiorMargin.Bottom);
        SetUVSquare(offset + 3, x + w - (_exteriorMargin.Right + _interiorMargin.Right), y + h - (_exteriorMargin.Bottom + _interiorMargin.Bottom), _interiorMargin.Right, _interiorMargin.Bottom);
        SetUVSquare(offset + 4, x + w - _exteriorMargin.Right, y + h - (_exteriorMargin.Bottom + _interiorMargin.Bottom), _exteriorMargin.Right, _interiorMargin.Bottom);

        offset = 5 * 4;
        SetUVSquare(offset + 0, x, y + h - _exteriorMargin.Bottom, _exteriorMargin.Left, _exteriorMargin.Bottom);
        SetUVSquare(offset + 1, x + _exteriorMargin.Left, y + h - _exteriorMargin.Bottom, _interiorMargin.Left, _exteriorMargin.Bottom);
        SetUVSquare(offset + 2, x + _exteriorMargin.Left + _interiorMargin.Left, y + h - _exteriorMargin.Bottom, _interiorWidth, _exteriorMargin.Bottom);
        SetUVSquare(offset + 3, x + w - (_exteriorMargin.Right + _interiorMargin.Right), y + h - _exteriorMargin.Bottom, _interiorMargin.Right, _exteriorMargin.Bottom);
        SetUVSquare(offset + 4, x + w - _exteriorMargin.Right, y + h - _exteriorMargin.Bottom, _exteriorMargin.Right, _exteriorMargin.Bottom);
    }

    private void SetUVSquare(int num, int x, int y, int w, int h)
    {
        _uvSquares[num] = new UVSquare(
            U1: x / _textureWidth,
            V1: y / _textureHeight,
            U2: (x + w) / _textureWidth,
            V2: (y + h) / _textureHeight
        );

        _debugUVSquares[num] = new Rectangle(x, y, w, h);
    }

    private readonly Rectangle[] _debugDrawSquares = new Rectangle[25];

    private void DrawUVSquare(Renderer.Base renderer, int uvSquareIndex, Rectangle targetBounds, Color color)
    {
        if (targetBounds.Width < 1 || targetBounds.Height < 1)
        {
            return;
        }

        // renderer.DrawColor = color;
        // renderer.DrawLinedRect(targetBounds);

        _debugDrawSquares[uvSquareIndex] = targetBounds;

        var uvSquare = _uvSquares[uvSquareIndex];
        renderer.DrawTexturedRect(
            texture: _texture,
            targetBounds: targetBounds,
            color: color,
            u1: uvSquare.U1,
            v1: uvSquare.V1,
            u2: uvSquare.U2,
            v2: uvSquare.V2
        );
    }

    public void Draw(Renderer.Base renderer, Rectangle targetBounds, Color color)
    {
        renderer.DrawColor = color;

        if (targetBounds.Width < _width && targetBounds.Height < _height)
        {
            renderer.DrawTexturedRect(
                _texture,
                targetBounds,
                color,
                _uvSquares[12].U1,
                _uvSquares[12].V1,
                _uvSquares[12].U2,
                _uvSquares[12].V2
            );

            return;
        }

        var left = targetBounds.Left;
        var top = targetBounds.Top;
        var right = targetBounds.Right;
        var height = targetBounds.Bottom;

        var interiorRatioLeft = _interiorMargin.Left / Math.Max(1, (float)(_interiorMargin.Left + _interiorMargin.Right));
        var interiorRatioTop = _interiorMargin.Top / Math.Max(1, (float)(_interiorMargin.Top + _interiorMargin.Bottom));
        var exteriorDrawMargin = _exteriorMargin;
        var remainingWidth = targetBounds.Width - (exteriorDrawMargin.Left + exteriorDrawMargin.Right + _interiorWidth);
        var remainingHeight = targetBounds.Height - (exteriorDrawMargin.Top + exteriorDrawMargin.Bottom + _interiorHeight);
        var interiorDrawMargin = new Margin(
            (int)Math.Floor(interiorRatioLeft * remainingWidth),
            (int)Math.Floor(interiorRatioTop * remainingHeight),
            (int)Math.Ceiling((1 - interiorRatioLeft) * remainingWidth),
            (int)Math.Ceiling((1 - interiorRatioTop) * remainingHeight)
        );

        var offset = 5 * 0;
        DrawUVSquare(renderer, offset + 0, new Rectangle(left, top, exteriorDrawMargin.Left, exteriorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 1, new Rectangle(left + exteriorDrawMargin.Left, top, interiorDrawMargin.Left, exteriorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 2, new Rectangle(left + exteriorDrawMargin.Left + interiorDrawMargin.Left, top, _interiorWidth, exteriorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 3, new Rectangle(right - (exteriorDrawMargin.Right + interiorDrawMargin.Right), top, interiorDrawMargin.Right, exteriorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 4, new Rectangle(right - exteriorDrawMargin.Right, top, exteriorDrawMargin.Right, exteriorDrawMargin.Top), color);

        offset = 5 * 1;
        DrawUVSquare(renderer, offset + 0, new Rectangle(left, top + exteriorDrawMargin.Top, exteriorDrawMargin.Left, interiorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 1, new Rectangle(left + exteriorDrawMargin.Left, top + exteriorDrawMargin.Top, interiorDrawMargin.Left, interiorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 2, new Rectangle(left + exteriorDrawMargin.Left + interiorDrawMargin.Left, top + exteriorDrawMargin.Top, _interiorWidth, interiorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 3, new Rectangle(right - (exteriorDrawMargin.Right + interiorDrawMargin.Right), top + exteriorDrawMargin.Top, interiorDrawMargin.Right, interiorDrawMargin.Top), color);
        DrawUVSquare(renderer, offset + 4, new Rectangle(right - exteriorDrawMargin.Right, top + exteriorDrawMargin.Top, exteriorDrawMargin.Right, interiorDrawMargin.Top), color);

        offset = 5 * 2;
        DrawUVSquare(renderer, offset + 0, new Rectangle(left, top + exteriorDrawMargin.Top + interiorDrawMargin.Top, exteriorDrawMargin.Left, _interiorHeight), color);
        DrawUVSquare(renderer, offset + 1, new Rectangle(left + exteriorDrawMargin.Left, top + exteriorDrawMargin.Top + interiorDrawMargin.Top, interiorDrawMargin.Left, _interiorHeight), color);
        DrawUVSquare(renderer, offset + 2, new Rectangle(left + exteriorDrawMargin.Left + interiorDrawMargin.Left, top + exteriorDrawMargin.Top + interiorDrawMargin.Top, _interiorWidth, _interiorHeight), color);
        DrawUVSquare(renderer, offset + 3, new Rectangle(right - (exteriorDrawMargin.Right + interiorDrawMargin.Right), top + exteriorDrawMargin.Top + interiorDrawMargin.Top, interiorDrawMargin.Right, _interiorHeight), color);
        DrawUVSquare(renderer, offset + 4, new Rectangle(right - exteriorDrawMargin.Right, top + exteriorDrawMargin.Top + interiorDrawMargin.Top, exteriorDrawMargin.Right, _interiorHeight), color);

        offset = 5 * 3;
        DrawUVSquare(renderer, offset + 0, new Rectangle(left, height - (exteriorDrawMargin.Bottom + interiorDrawMargin.Bottom), exteriorDrawMargin.Left, interiorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 1, new Rectangle(left + exteriorDrawMargin.Left, height - (exteriorDrawMargin.Bottom + interiorDrawMargin.Bottom), interiorDrawMargin.Left, interiorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 2, new Rectangle(left + exteriorDrawMargin.Left + interiorDrawMargin.Left, height - (exteriorDrawMargin.Bottom + interiorDrawMargin.Bottom), _interiorWidth, interiorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 3, new Rectangle(right - (exteriorDrawMargin.Right + interiorDrawMargin.Right), height - (exteriorDrawMargin.Bottom + interiorDrawMargin.Bottom), interiorDrawMargin.Right, interiorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 4, new Rectangle(right - exteriorDrawMargin.Right, height - (exteriorDrawMargin.Bottom + interiorDrawMargin.Bottom), exteriorDrawMargin.Right, interiorDrawMargin.Bottom), color);

        offset = 5 * 4;
        DrawUVSquare(renderer, offset + 0, new Rectangle(left, height - exteriorDrawMargin.Bottom, exteriorDrawMargin.Left, exteriorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 1, new Rectangle(left + exteriorDrawMargin.Left, height - exteriorDrawMargin.Bottom, interiorDrawMargin.Left, exteriorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 2, new Rectangle(left + exteriorDrawMargin.Left + interiorDrawMargin.Left, height - exteriorDrawMargin.Bottom, _interiorWidth, exteriorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 3, new Rectangle(right - (exteriorDrawMargin.Right + interiorDrawMargin.Right), height - exteriorDrawMargin.Bottom, interiorDrawMargin.Right, exteriorDrawMargin.Bottom), color);
        DrawUVSquare(renderer, offset + 4, new Rectangle(right - exteriorDrawMargin.Right, height - exteriorDrawMargin.Bottom, exteriorDrawMargin.Right, exteriorDrawMargin.Bottom), color);
    }
}