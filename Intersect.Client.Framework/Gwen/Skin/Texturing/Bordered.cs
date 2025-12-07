using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing;


public partial struct SubRect
{

    public float[] Uv;

}

/// <summary>
///     3x3 texture grid.
/// </summary>
public partial struct Bordered : IEquatable<Bordered>, IAtlasDrawable
{
    private readonly float mX;
    private readonly float mY;

    public static implicit operator FivePatch(Bordered bordered)
    {
        return new FivePatch(
            bordered._texture,
            (int)bordered.mX,
            (int)bordered.mY,
            (int)bordered._width,
            (int)bordered._height,
            bordered._margin,
            default
        );
    }

    private IGameTexture _texture;

    private readonly SubRect[] _subRects;

    private Margin _margin;

    private float _width;

    private float _height;

    public Margin Margin => _margin;

    public Bordered(
        IGameTexture texture,
        float x,
        float y,
        float w,
        float h,
        Margin inMargin
    ) : this()
    {
        mX = x;
        mY = y;
        _subRects = new SubRect[9];
        for (var i = 0; i < _subRects.Length; i++)
        {
            _subRects[i].Uv = new float[4];
        }

        Init(texture, x, y, w, h, inMargin);
    }

    void DrawRect(Renderer.Base render, int i, int x, int y, int w, int h, Color clr)
    {
        render.DrawTexturedRect(
            _texture, new Rectangle(x, y, w, h), clr, _subRects[i].Uv[0], _subRects[i].Uv[1], _subRects[i].Uv[2],
            _subRects[i].Uv[3]
        );
    }

    void SetRect(int num, float x, float y, float w, float h)
    {
        if (_texture == null)
        {
            return;
        }

        float texw = _texture.Width;
        float texh = _texture.Height;

        //x -= 1.0f;
        //y -= 1.0f;

        _subRects[num].Uv[0] = x / texw;
        _subRects[num].Uv[1] = y / texh;

        _subRects[num].Uv[2] = (x + w) / texw;
        _subRects[num].Uv[3] = (y + h) / texh;

        //	rects[num].uv[0] += 1.0f / m_Texture->width;
        //	rects[num].uv[1] += 1.0f / m_Texture->width;
    }

    private void Init(
        IGameTexture texture,
        float x,
        float y,
        float w,
        float h,
        Margin inMargin,
        float drawMarginScale = 1.0f
    )
    {
        _texture = texture;

        _margin = inMargin;

        SetRect(0, x, y, _margin.Left, _margin.Top);
        SetRect(1, x + _margin.Left, y, w - _margin.Left - _margin.Right, _margin.Top);
        SetRect(2, x + w - _margin.Right, y, _margin.Right, _margin.Top);

        SetRect(3, x, y + _margin.Top, _margin.Left, h - _margin.Top - _margin.Bottom);
        SetRect(
            4,
            x + _margin.Left,
            y + _margin.Top,
            w - _margin.Left - _margin.Right,
            h - _margin.Top - _margin.Bottom
        );

        SetRect(5, x + w - _margin.Right, y + _margin.Top, _margin.Right, h - _margin.Top - _margin.Bottom);

        SetRect(6, x, y + h - _margin.Bottom, _margin.Left, _margin.Bottom);
        SetRect(7, x + _margin.Left, y + h - _margin.Bottom, w - _margin.Left - _margin.Right, _margin.Bottom);
        SetRect(8, x + w - _margin.Right, y + h - _margin.Bottom, _margin.Right, _margin.Bottom);

        _width = w;
        _height = h;
    }

    // can't have this as default param
    /*public void Draw(Renderer.Base render, Rectangle r, Color col )
    {
        Draw(render, r, Color.Red);
    }*/

    public void Draw(Renderer.Base render, Rectangle r, Color col)
    {
        if (_texture == null)
        {
            return;
        }

        render.DrawColor = col;

        if (r.Width < _width && r.Height < _height)
        {
            render.DrawTexturedRect(
                _texture, r, col, _subRects[0].Uv[0], _subRects[0].Uv[1], _subRects[8].Uv[2], _subRects[8].Uv[3]
            );

            return;
        }

        DrawRect(render, 0, r.X, r.Y, _margin.Left, _margin.Top, col);
        DrawRect(render, 1, r.X + _margin.Left, r.Y, r.Width - _margin.Left - _margin.Right, _margin.Top, col);
        DrawRect(render, 2, r.X + r.Width - _margin.Right, r.Y, _margin.Right, _margin.Top, col);

        DrawRect(render, 3, r.X, r.Y + _margin.Top, _margin.Left, r.Height - _margin.Top - _margin.Bottom, col);
        DrawRect(
            render, 4, r.X + _margin.Left, r.Y + _margin.Top, r.Width - _margin.Left - _margin.Right,
            r.Height - _margin.Top - _margin.Bottom, col
        );

        DrawRect(
            render, 5, r.X + r.Width - _margin.Right, r.Y + _margin.Top, _margin.Right,
            r.Height - _margin.Top - _margin.Bottom, col
        );

        DrawRect(render, 6, r.X, r.Y + r.Height - _margin.Bottom, _margin.Left, _margin.Bottom, col);
        DrawRect(
            render, 7, r.X + _margin.Left, r.Y + r.Height - _margin.Bottom, r.Width - _margin.Left - _margin.Right,
            _margin.Bottom, col
        );

        DrawRect(
            render, 8, r.X + r.Width - _margin.Right, r.Y + r.Height - _margin.Bottom, _margin.Right,
            _margin.Bottom, col
        );
    }

    public bool Equals(Bordered other)
    {
        return _texture.Equals(other._texture) && _subRects.Equals(other._subRects) && _margin.Equals(other._margin) && _width.Equals(other._width) && _height.Equals(other._height);
    }

    public override bool Equals(object? obj)
    {
        return obj is Bordered other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_texture, _subRects, _margin, _width, _height);
    }

    public static bool operator ==(Bordered lhs, Bordered rhs) => lhs.Equals(rhs);

    public static bool operator !=(Bordered lhs, Bordered rhs) => !lhs.Equals(rhs);
}
