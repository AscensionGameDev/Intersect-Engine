using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     HSV hue selector.
/// </summary>
public partial class ColorSlider : Base
{

    private bool mDepressed;

    private int mSelectedDist;

    private IGameTexture mTexture;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorSlider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public ColorSlider(Base parent) : base(parent)
    {
        SetSize(32, 128);
        MouseInputEnabled = true;
        mDepressed = false;
    }

    /// <summary>
    ///     Selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => GetColorAtHeight(mSelectedDist);
        set => SetColor(value);
    }

    /// <summary>
    ///     Invoked when the selected color has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> ColorChanged;

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.Renderer.DrawColor = Color.White;
        skin.Renderer.DrawTexturedRect(
            skin.Renderer.GetWhiteTexture(), new Rectangle(5, 0, Width - 10, Height), Color.White
        );

        var drawHeight = mSelectedDist - 3;

        //Draw our selectors
        skin.Renderer.DrawColor = Color.Black;
        skin.Renderer.DrawFilledRect(new Rectangle(0, drawHeight + 2, Width, 1));
        skin.Renderer.DrawFilledRect(new Rectangle(0, drawHeight, 5, 5));
        skin.Renderer.DrawFilledRect(new Rectangle(Width - 5, drawHeight, 5, 5));
        skin.Renderer.DrawColor = Color.White;
        skin.Renderer.DrawFilledRect(new Rectangle(1, drawHeight + 1, 3, 3));
        skin.Renderer.DrawFilledRect(new Rectangle(Width - 4, drawHeight + 1, 3, 3));

        base.Render(skin);
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        mDepressed = true;
        InputHandler.MouseFocus = this;

        OnMouseMoved(mousePosition.X, mousePosition.Y, 0, 0);
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        mDepressed = false;
        InputHandler.MouseFocus = null;

        OnMouseMoved(mousePosition.X, mousePosition.Y, 0, 0);
    }

    /// <summary>
    ///     Handler invoked on mouse moved event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="dx">X change.</param>
    /// <param name="dy">Y change.</param>
    protected override void OnMouseMoved(int x, int y, int dx, int dy)
    {
        if (mDepressed)
        {
            var cursorPos = CanvasPosToLocal(new Point(x, y));

            if (cursorPos.Y < 0)
            {
                cursorPos.Y = 0;
            }

            if (cursorPos.Y > Height)
            {
                cursorPos.Y = Height;
            }

            mSelectedDist = cursorPos.Y;
            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private Color GetColorAtHeight(int y)
    {
        var yPercent = y / (float) Height;

        return Util.HsvToColor(yPercent * 360, 1, 1);
    }

    private void SetColor(Color color)
    {
        var hsv = color.ToHsv();

        mSelectedDist = (int) (hsv.H / 360 * Height);

        if (ColorChanged != null)
        {
            ColorChanged.Invoke(this, EventArgs.Empty);
        }
    }

}
