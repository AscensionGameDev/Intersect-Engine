using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.ControlInternal;

#if DEBUG || DIAGNOSTIC
#endif

namespace Intersect.Client.Framework.Gwen.Skin;


/// <summary>
///     Base skin.
/// </summary>
public partial class Base : IDisposable
{

    protected readonly Renderer.Base mRenderer;

    /// <summary>
    ///     Colors of various UI elements.
    /// </summary>
    public SkinColors Colors;

    protected GameFont mDefaultFont;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Base" /> class.
    /// </summary>
    /// <param name="renderer">Renderer to use.</param>
    protected Base(Renderer.Base renderer)
    {
        mDefaultFont = null;
        mRenderer = renderer;
    }

    /// <summary>
    ///     Default font to use when rendering text if none specified.
    /// </summary>
    public GameFont DefaultFont
    {
        get => mDefaultFont;
        set => mDefaultFont = value;
    }

    /// <summary>
    ///     Renderer used.
    /// </summary>
    public Renderer.Base Renderer => mRenderer;

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

#if DIAGNOSTIC
    ~Base()
    {
        ApplicationContext.Context.Value?.Logger.LogDebug($"IDisposable object finalized: {GetType()}");
    }
#endif

    #region UI elements

    public virtual void DrawButton(Control.Base control, bool depressed, bool hovered, bool disabled, bool focused)
    {
    }

    public virtual void DrawPanel(Panel panel)
    {
    }

    public virtual void DrawTabButton(Control.Base control, bool active, Pos dir)
    {
    }

    public virtual void DrawTabControl(Control.Base control)
    {
    }

    public virtual void DrawTabTitleBar(Control.Base control)
    {
    }

    public virtual void DrawMenuItem(Control.Base control, bool submenuOpen, bool isChecked)
    {
    }

    public virtual void DrawMenuRightArrow(Control.Base control)
    {
    }

    public virtual void DrawMenuStrip(Control.Base control)
    {
    }

    public virtual void DrawMenu(Control.Base control, bool paddingDisabled)
    {
    }

    public virtual void DrawRadioButton(Control.Base control, bool selected, bool depressed)
    {
    }

    public virtual void DrawRadioButton(Control.Base control, bool selected, bool hovered, bool depressed) => DrawRadioButton(control, selected, depressed);

    public virtual void DrawCheckBox(Control.Base control, bool selected, bool depressed)
    {
    }

    public virtual void DrawCheckBox(Control.Base control, bool selected, bool hovered, bool depressed) => DrawCheckBox(control, selected, depressed);

    public virtual void DrawGroupBox(Control.Base control, int textStart, int textHeight, int textWidth)
    {
    }

    public virtual void DrawTextBox(Control.Base control)
    {
    }

    public virtual void DrawWindow(Control.Base control, int topHeight, bool inFocus)
    {
    }

    public virtual void DrawWindowCloseButton(Control.Base control, bool depressed, bool hovered, bool disabled)
    {
    }

    public virtual void DrawHighlight(Control.Base control)
    {
    }

    public virtual void DrawStatusBar(Control.Base control)
    {
    }

    public virtual void DrawShadow(Control.Base control)
    {
    }

    public virtual void DrawScrollBarBar(ScrollBarBar scrollBarBar)
    {
    }

    public virtual void DrawScrollBar(Control.Base control, bool horizontal, bool depressed)
    {
    }

    public virtual void DrawScrollButton(
        Control.Base control,
        Pos direction,
        bool depressed,
        bool hovered,
        bool disabled
    )
    {
    }

    public virtual void DrawProgressBar(Control.Base control, bool horizontal, float progress)
    {
    }

    public virtual void DrawListBox(Control.Base control)
    {
    }

    public virtual void DrawListBoxLine(Control.Base control, bool selected, bool even)
    {
    }

    public virtual void DrawSlider(Control.Base control, bool horizontal, double[]? notches, int numNotches, int barSize)
    {
    }

    public virtual void DrawSlider(
        Slider slider,
        Orientation orientation,
        double[]? notches,
        int numNotches,
        int barSize
    ) => DrawSlider(
        slider,
        orientation is Orientation.LeftToRight or Orientation.RightToLeft,
        notches,
        numNotches,
        barSize
    );

    public virtual void DrawSliderButton(SliderBar sliderBar)
    {
    }

    public virtual void DrawComboBox(Control.Base control, bool down, bool isMenuOpen)
    {
    }

    public virtual void DrawComboBoxArrow(
        Control.Base control,
        bool hovered,
        bool depressed,
        bool open,
        bool disabled
    )
    {
    }

    public virtual void DrawKeyboardHighlight(Control.Base control, Rectangle rect, int offset)
    {
    }

    public virtual void DrawToolTip(Control.Base control)
    {
    }

    public virtual void DrawNumericUpDownButton(Control.Base control, bool depressed, bool up)
    {
    }

    public virtual void DrawTreeButton(Control.Base control, bool open)
    {
    }

    public virtual void DrawTreeControl(Control.Base control)
    {
    }

    public virtual void DrawDebugOutlines(Control.Base control)
    {
        if (control.IsHidden)
        {
            return;
        }

        var padding = control.Padding;
        if (padding != default)
        {
            var paddingRect = new Rectangle(
                control.Bounds.Left + padding.Left,
                control.Bounds.Top + padding.Top,
                control.Bounds.Width - padding.Right - padding.Left,
                control.Bounds.Height - padding.Bottom - padding.Top
            );

            DrawRectStroke(paddingRect, control.PaddingOutlineColor);
        }

        var margin = control.Margin;
        if (margin != default)
        {
            var marginRect = new Rectangle(
                control.Bounds.Left - margin.Left,
                control.Bounds.Top - margin.Top,
                control.Bounds.Width + margin.Right + margin.Left,
                control.Bounds.Height + margin.Bottom + margin.Top
            );

            DrawRectStroke(marginRect, control.MarginOutlineColor);
        }

        if (control is ITextContainer textContainer)
        {
            var textPadding = textContainer.TextPadding;
            if (textPadding != default)
            {
                var color = textContainer.TextPaddingDebugColor ?? DefaultTextPaddingDebugColor;
                var textPaddingRect = new Rectangle(
                    control.Bounds.Left + textPadding.Left,
                    control.Bounds.Top + textPadding.Top,
                    control.Bounds.Width - textPadding.Right - textPadding.Left,
                    control.Bounds.Height - textPadding.Bottom - textPadding.Top
                );

                DrawRectStroke(textPaddingRect, color);
            }
        }

        DrawRectStroke(control.Bounds, control.BoundsOutlineColor);
    }

    public static Color DefaultTextPaddingDebugColor = Color.FromHex("79b5d2", Color.White);

    public virtual void DrawRectStroke(Rectangle rect, Color color) => DrawRect(rect, color, filled: false);

    public virtual void DrawRectFill(Rectangle rect, Color color) => DrawRect(rect, color, filled: true);

    public virtual void DrawRect(Rectangle rect, Color color, bool filled)
    {
        mRenderer.DrawColor = color;
        if (filled)
        {
            mRenderer.DrawFilledRect(rect);
        }
        else
        {
            mRenderer.DrawLinedRect(rect);
        }
    }

    public virtual void DrawTreeNode(
        Control.Base ctrl,
        bool open,
        bool selected,
        int treeNodeHeight,
        int labelHeight,
        int labelWidth,
        int halfWay,
        int lastBranch,
        bool isRoot
    )
    {
        Renderer.DrawColor = Colors.Tree.Lines;

        if (!isRoot)
        {
            Renderer.DrawFilledRect(new Rectangle(8, halfWay, 16 - 9, 1));
        }

        if (!open)
        {
            return;
        }

        Renderer.DrawFilledRect(new Rectangle(14 + 7, labelHeight + 1, 1, lastBranch + halfWay - labelHeight));
    }

    public virtual void DrawPropertyRow(Control.Base control, int iWidth, bool bBeingEdited, bool hovered)
    {
        var rect = control.RenderBounds;

        if (bBeingEdited)
        {
            mRenderer.DrawColor = Colors.Properties.ColumnSelected;
        }
        else if (hovered)
        {
            mRenderer.DrawColor = Colors.Properties.ColumnHover;
        }
        else
        {
            mRenderer.DrawColor = Colors.Properties.ColumnNormal;
        }

        mRenderer.DrawFilledRect(new Rectangle(0, rect.Y, iWidth, rect.Height));

        if (bBeingEdited)
        {
            mRenderer.DrawColor = Colors.Properties.LineSelected;
        }
        else if (hovered)
        {
            mRenderer.DrawColor = Colors.Properties.LineHover;
        }
        else
        {
            mRenderer.DrawColor = Colors.Properties.LineNormal;
        }

        mRenderer.DrawFilledRect(new Rectangle(iWidth, rect.Y, 1, rect.Height));

        rect.Y += rect.Height - 1;
        rect.Height = 1;

        mRenderer.DrawFilledRect(rect);
    }

    public virtual void DrawColorDisplay(Control.Base control, Color color)
    {
    }

    public virtual void DrawModalControl(Control.Base control)
    {
    }

    public virtual void DrawMenuDivider(Control.Base control)
    {
    }

    public virtual void DrawCategoryHolder(Control.Base control)
    {
    }

    public virtual void DrawCategoryInner(Control.Base control, bool collapsed)
    {
    }

    public virtual void DrawPropertyTreeNode(Control.Base control, int borderLeft, int borderTop)
    {
        var rect = control.RenderBounds;

        mRenderer.DrawColor = Colors.Properties.Border;

        mRenderer.DrawFilledRect(new Rectangle(rect.X, rect.Y, borderLeft, rect.Height));
        mRenderer.DrawFilledRect(new Rectangle(rect.X + borderLeft, rect.Y, rect.Width - borderLeft, borderTop));
    }

    #endregion

    #region Symbols for Simple skin

    /*
    Here we're drawing a few symbols such as the directional arrows and the checkbox check

    Texture'd skins don't generally use these - but the Simple skin does. We did originally
    use the marlett font to draw these.. but since that's a Windows font it wasn't a very
    good cross platform solution.
    */

    public virtual void DrawArrowDown(Rectangle rect)
    {
        var x = rect.Width / 5.0f;
        var y = rect.Height / 5.0f;

        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 0.0f, rect.Y + y * 1.0f, x, y * 1.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 1.0f, x, y * 2.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 2.0f, rect.Y + y * 1.0f, x, y * 3.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 3.0f, rect.Y + y * 1.0f, x, y * 2.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 4.0f, rect.Y + y * 1.0f, x, y * 1.0f));
    }

    public virtual void DrawArrowUp(Rectangle rect)
    {
        var x = rect.Width / 5.0f;
        var y = rect.Height / 5.0f;

        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 0.0f, rect.Y + y * 3.0f, x, y * 1.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 2.0f, x, y * 2.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 2.0f, rect.Y + y * 1.0f, x, y * 3.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 3.0f, rect.Y + y * 2.0f, x, y * 2.0f));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 4.0f, rect.Y + y * 3.0f, x, y * 1.0f));
    }

    public virtual void DrawArrowLeft(Rectangle rect)
    {
        var x = rect.Width / 5.0f;
        var y = rect.Height / 5.0f;

        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 3.0f, rect.Y + y * 0.0f, x * 1.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 2.0f, rect.Y + y * 1.0f, x * 2.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 2.0f, x * 3.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 2.0f, rect.Y + y * 3.0f, x * 2.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 3.0f, rect.Y + y * 4.0f, x * 1.0f, y));
    }

    public virtual void DrawArrowRight(Rectangle rect)
    {
        var x = rect.Width / 5.0f;
        var y = rect.Height / 5.0f;

        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 0.0f, x * 1.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 1.0f, x * 2.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 2.0f, x * 3.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 3.0f, x * 2.0f, y));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 4.0f, x * 1.0f, y));
    }

    public virtual void DrawCheck(Rectangle rect)
    {
        var x = rect.Width / 5.0f;
        var y = rect.Height / 5.0f;

        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 0.0f, rect.Y + y * 3.0f, x * 2, y * 2));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 1.0f, rect.Y + y * 4.0f, x * 2, y * 2));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 2.0f, rect.Y + y * 3.0f, x * 2, y * 2));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 3.0f, rect.Y + y * 1.0f, x * 2, y * 2));
        mRenderer.DrawFilledRect(Util.FloatRect(rect.X + x * 4.0f, rect.Y + y * 0.0f, x * 2, y * 2));
    }

    public virtual void DrawLabel(Control.Base control)
    {
    }

    #endregion

}
