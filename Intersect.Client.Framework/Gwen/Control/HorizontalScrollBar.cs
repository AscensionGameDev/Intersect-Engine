using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Horizontal scrollbar.
/// </summary>
public partial class HorizontalScrollBar : ScrollBar
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="HorizontalScrollBar" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public HorizontalScrollBar(Base parent) : base(parent)
    {
        mBar.IsHorizontal = true;

        mScrollButton[0].SetDirectionLeft();
        mScrollButton[0].Clicked += NudgeLeft;

        mScrollButton[1].SetDirectionRight();
        mScrollButton[1].Clicked += NudgeRight;

        mBar.Dragged += OnBarMoved;
    }

    /// <summary>
    ///     Bar size (in pixels).
    /// </summary>
    public override int BarSize
    {
        get => mBar.Width;
        set => mBar.Width = value;
    }

    /// <summary>
    ///     Bar position (in pixels).
    /// </summary>
    public override int BarPos => mBar.X - Height;

    /// <summary>
    ///     Indicates whether the bar is horizontal.
    /// </summary>
    public override bool IsHorizontal => true;

    /// <summary>
    ///     Button size (in pixels).
    /// </summary>
    public override int ButtonSize => Height;

    public override float NudgeAmount
    {
        get => mDepressed ? mViewableContentSize / mContentSize : base.NudgeAmount;
        set => base.NudgeAmount = value;
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);

        mScrollButton[0].Width = Height;
        mScrollButton[0].Dock = Pos.Left;

        mScrollButton[1].Width = Height;
        mScrollButton[1].Dock = Pos.Right;

        mBar.Height = ButtonSize;
        mBar.Margin = new Margin(ButtonSize, 0, ButtonSize, 0);

        var barWidth = mViewableContentSize / mContentSize * (Width - ButtonSize * 2);

        if (barWidth < ButtonSize * 0.5f)
        {
            barWidth = (int) (ButtonSize * 0.5f);
        }

        mBar.Width = (int) barWidth;
        mBar.IsHidden = Width - ButtonSize * 2 <= barWidth;

        //Based on our last scroll amount, produce a position for the bar
        if (!mBar.IsHeld)
        {
            SetScrollAmount(ScrollAmount, true);
        }
    }

    public void NudgeLeft(Base control, EventArgs args)
    {
        if (!IsDisabled)
        {
            SetScrollAmount(ScrollAmount - NudgeAmount, true);
            base.PlaySound(mBar.GetMouseUpSound());
        }
    }

    public void NudgeRight(Base control, EventArgs args)
    {
        if (!IsDisabled)
        {
            SetScrollAmount(ScrollAmount + NudgeAmount, true);
            base.PlaySound(mBar.GetMouseUpSound());
        }
    }

    public override void ScrollToLeft()
    {
        SetScrollAmount(0, true);
    }

    public override void ScrollToRight()
    {
        SetScrollAmount(1, true);
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
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        var localCoordinates = CanvasPosToLocal(mousePosition);
        if (localCoordinates.X < mBar.X)
        {
            NudgeLeft(this, EventArgs.Empty);
        }
        else if (localCoordinates.X > mBar.X + mBar.Width)
        {
            NudgeRight(this, EventArgs.Empty);
        }

        mDepressed = false;
        InputHandler.MouseFocus = null;
    }

    protected override float CalculateScrolledAmount()
    {
        return (float) (mBar.X - ButtonSize) / (Width - mBar.Width - ButtonSize * 2);
    }

    /// <summary>
    ///     Sets the scroll amount (0-1).
    /// </summary>
    /// <param name="value">Scroll amount.</param>
    /// <param name="forceUpdate">Determines whether the control should be updated.</param>
    /// <returns>
    ///     True if control state changed.
    /// </returns>
    public override bool SetScrollAmount(float value, bool forceUpdate = false)
    {
        value = Util.Clamp(value, 0, 1);

        if (!base.SetScrollAmount(value, forceUpdate))
        {
            return false;
        }

        if (forceUpdate)
        {
            var newX = (int) (ButtonSize + value * (Width - mBar.Width - ButtonSize * 2));
            mBar.MoveTo(newX, mBar.Y);
        }

        return true;
    }

    /// <summary>
    ///     Handler for the BarMoved event.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected override void OnBarMoved(Base control, EventArgs args)
    {
        if (mBar.IsHeld)
        {
            SetScrollAmount(CalculateScrolledAmount(), false);
            base.OnBarMoved(control, args);
        }
        else
        {
            InvalidateParent();
        }
    }

}
