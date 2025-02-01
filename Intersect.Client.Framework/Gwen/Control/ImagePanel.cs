using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Skin.Texturing;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Image container.
/// </summary>
public partial class ImagePanel : Base
{

    private readonly float[] mUv;

    //Sound Effects
    protected string mHoverSound;

    protected string mLeftMouseClickSound;

    private Modal mModal;

    private Base mOldParent;

    protected string mRightMouseClickSound;

    private GameTexture? mTexture;
    private string? mTextureFilename;

    private Margin? _textureNinePatchMargin;
    private Bordered? _ninepatchRenderer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImagePanel" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public ImagePanel(Base parent, string name = "") : base(parent)
    {
        mUv = new float[4];
        mTexture = null;
        SetUv(0, 0, 1, 1);
        MouseInputEnabled = true;
        Name = name;
        this.Clicked += ImagePanel_Clicked;
        this.RightClicked += ImagePanel_RightClicked;
        this.HoverEnter += ImagePanel_HoverEnter;
    }

    public Margin? TextureNinePatchMargin
    {
        get => _textureNinePatchMargin;
        set
        {
            if (value == _textureNinePatchMargin)
            {
                return;
            }

            _textureNinePatchMargin = value;
            _ninepatchRenderer = null;
        }
    }

    /// <summary>
    ///     Assign Existing Texture
    /// </summary>
    public GameTexture? Texture
    {
        get => mTexture;
        set
        {
            mTexture = value;
            _ninepatchRenderer = null;
            this.InvalidateParent();
        }
    }

    public string? TextureFilename
    {
        get => mTextureFilename;
        set
        {
            var textureFilename = value?.Trim();
            if (string.Equals(textureFilename, mTextureFilename, StringComparison.Ordinal))
            {
                return;
            }

            mTextureFilename = textureFilename;
            mTexture = mTextureFilename == null
                ? null
                : GameContentManager.Current?.GetTexture(TextureType.Gui, mTextureFilename);
        }
    }

    private void ImagePanel_HoverEnter(Base sender, System.EventArgs arguments)
    {
        PlaySound(mHoverSound);
    }

    private void ImagePanel_RightClicked(Base sender, EventArguments.ClickedEventArgs arguments)
    {
        PlaySound(mRightMouseClickSound);
    }

    private void ImagePanel_Clicked(Base sender, EventArguments.ClickedEventArgs arguments)
    {
        PlaySound(mLeftMouseClickSound);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
    }

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);

        obj.Add(nameof(Texture), TextureFilename);
        obj.Add(nameof(TextureNinePatchMargin), TextureNinePatchMargin?.ToString());
        obj.Add("HoverSound", mHoverSound);
        obj.Add("LeftMouseClickSound", mLeftMouseClickSound);
        obj.Add("RightMouseClickSound", mRightMouseClickSound);

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token);

        if (token is not JObject obj)
        {
            return;
        }

        if (obj["Texture"] != null)
        {
            Texture = GameContentManager.Current.GetTexture(
                Framework.Content.TextureType.Gui, (string) obj["Texture"]
            );

            TextureFilename = (string) obj["Texture"];
        }

        if (obj.TryGetValue(nameof(TextureNinePatchMargin), out var textureNinePatchMarginToken))
        {
            if (textureNinePatchMarginToken is JValue { Type: JTokenType.String } textureNinePatchMarginValue)
            {
                if (textureNinePatchMarginValue.Value<string>() is { } textureNinePatchMarginString &&
                    !string.IsNullOrWhiteSpace(textureNinePatchMarginString))
                {
                    TextureNinePatchMargin = Margin.FromString(textureNinePatchMarginString);
                }
                else
                {
                    TextureNinePatchMargin = null;
                }
            }
            else
            {
                TextureNinePatchMargin = null;
            }
        }

        if (obj["HoverSound"] != null)
        {
            mHoverSound = (string) obj["HoverSound"];
        }

        if (obj["LeftMouseClickSound"] != null)
        {
            mLeftMouseClickSound = (string) obj["LeftMouseClickSound"];
        }

        if (obj["RightMouseClickSound"] != null)
        {
            mRightMouseClickSound = (string) obj["RightMouseClickSound"];
        }
    }

    /// <summary>
    ///     Sets the texture coordinates of the image.
    /// </summary>
    public virtual void SetUv(float u1, float v1, float u2, float v2)
    {
        mUv[0] = u1;
        mUv[1] = v1;
        mUv[2] = u2;
        mUv[3] = v2;

        _ninepatchRenderer = null;
    }

    /// <summary>
    ///     Sets the texture coordinates of the image.
    /// </summary>
    public virtual void SetTextureRect(int x, int y, int w, int h)
    {
        if (mTexture == null)
        {
            return;
        }

        if (x < 0)
        {
            x = 0;
        }

        if (y < 0)
        {
            y = 0;
        }

        if (w <= 0)
        {
            w = mTexture.Width;
        }

        if (h <= 0)
        {
            h = mTexture.Height;
        }

        if (x + w > mTexture.Width || y + h > mTexture.Height)
        {
            return;
        }

        SetUv(
            x / (float)mTexture.Width,
            y / (float)mTexture.Height,
            (x + w) / (float)mTexture.Width,
            (y + h) / (float)mTexture.Height
        );
    }

    public virtual Rectangle GetTextureRect()
    {
        if (Texture == null)
        {
            return new Rectangle(0, 0, 0, 0);
        }

        return new Rectangle(
            (int) (mUv[0] * mTexture.Width), (int) (mUv[1] * mTexture.Height),
            (int) ((mUv[2] - mUv[0]) * mTexture.Width), (int) ((mUv[3] - mUv[1]) * mTexture.Width)
        );
    }

    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);

        EnsureTextureLoaded();
    }

    private void EnsureTextureLoaded()
    {
        if (mTexture != null)
        {
            return;
        }

        var textureFilename = mTextureFilename;
        if (string.IsNullOrWhiteSpace(textureFilename))
        {
            return;
        }

        mTexture = GameContentManager.Current?.GetTexture(TextureType.Gui, textureFilename);
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        base.Render(skin);

        if (mTexture is not { } texture)
        {
            return;
        }

        skin.Renderer.DrawColor = base.RenderColor;

        var renderBounds = RenderBounds;

        if (TextureNinePatchMargin is { } textureNinePatchMargin)
        {
            _ninepatchRenderer ??= new Bordered(
                texture,
                mUv[0] * texture.Width,
                mUv[1] * texture.Height,
                mUv[2] * texture.Width,
                mUv[3] * texture.Height,
                textureNinePatchMargin
            );

            if (_ninepatchRenderer is { } ninepatchRenderer)
            {
                ninepatchRenderer.Draw(skin.Renderer, renderBounds, Color.White);
            }
        }
        else
        {
            skin.Renderer.DrawTexturedRect(
                texture,
                renderBounds,
                base.RenderColor,
                mUv[0],
                mUv[1],
                mUv[2],
                mUv[3]
            );
        }
    }

    /// <summary>
    ///     Sizes the control to its contents.
    /// </summary>
    public virtual void SizeToContents()
    {
        if (mTexture == null)
        {
            return;
        }

        SetSize((int) (mTexture.Width * (mUv[2] - mUv[0])), (int) (mTexture.Height * (mUv[3] - mUv[1])));
    }

    /// <summary>
    ///     Control has been clicked - invoked by input system. Windows use it to propagate activation.
    /// </summary>
    public override void Touch()
    {
        base.Touch();
    }

    /// <summary>
    ///     Handler for Space keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeySpace(bool down)
    {
        if (down)
        {
            base.OnMouseClickedLeft(0, 0, true);
        }

        return true;
    }

    /// <summary>
    ///     Makes the window modal: covers the whole canvas and gets all input.
    /// </summary>
    /// <param name="dim">Determines whether all the background should be dimmed.</param>
    public void MakeModal(bool dim = false)
    {
        if (mModal != null)
        {
            return;
        }

        mModal = new Modal(GetCanvas())
        {
            ShouldDrawBackground = dim
        };

        mOldParent = Parent;
        Parent = mModal;
    }

    public void RemoveModal()
    {
        if (mModal == null)
        {
            return;
        }

        Parent = mOldParent;
        GetCanvas()?.RemoveChild(mModal, false);
        mModal = null;
    }

}
