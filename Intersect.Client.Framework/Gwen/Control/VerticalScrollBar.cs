using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Vertical scrollbar.
/// </summary>
public partial class VerticalScrollBar : ScrollBar
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="VerticalScrollBar" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public VerticalScrollBar(Base parent) : base(parent)
    {
        mBar.IsVertical = true;

        mScrollButton[0].SetDirectionUp();
        mScrollButton[0].Clicked += NudgeUp;

        mScrollButton[1].SetDirectionDown();
        mScrollButton[1].Clicked += NudgeDown;

        mBar.Dragged += OnBarMoved;
    }

    /// <summary>
    ///     Bar size (in pixels).
    /// </summary>
    public override int BarSize
    {
        get => mBar.Height;
        set => mBar.Height = value;
    }

    /// <summary>
    ///     Bar position (in pixels).
    /// </summary>
    public override int BarPos => mBar.Y - Width;

    /// <summary>
    ///     Button size (in pixels).
    /// </summary>
    public override int ButtonSize => Width;

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
        mScrollButton[0].Height = Width;
        mScrollButton[0].Dock = Pos.Top;

        mScrollButton[1].Height = Width;
        mScrollButton[1].Dock = Pos.Bottom;

        mBar.Width = ButtonSize;
        mBar.Margin = new Margin(0, ButtonSize, 0, ButtonSize);

        var barHeight = 0.0f;
        if (mContentSize > 0.0f)
        {
            barHeight = mViewableContentSize / mContentSize * (Height - ButtonSize * 2);
        }

        if (barHeight < ButtonSize * 0.5f)
        {
            barHeight = (int) (ButtonSize * 0.5f);
        }

        mBar.Height = (int) barHeight;
        mBar.IsHidden = Height - ButtonSize * 2 <= barHeight;

        //Based on our last scroll amount, produce a position for the bar
        var isHeld = mBar.IsActive;
        if (!isHeld)
        {
            SetScrollAmount(ScrollAmount, _wasHeld != isHeld);
        }

        _wasHeld = isHeld;

        base.Layout(skin);
    }

    private bool _wasHeld;

    public virtual void NudgeUp(Base control, EventArgs args)
    {
        if (!IsDisabled)
        {
            SetScrollAmount(ScrollAmount - NudgeAmount, true);
            base.PlaySound(mBar.GetMouseUpSound());
        }
    }

    public virtual void NudgeDown(Base control, EventArgs args)
    {
        if (!IsDisabled)
        {
            SetScrollAmount(ScrollAmount + NudgeAmount, true);
            base.PlaySound(mBar.GetMouseUpSound());
        }
    }

    public override void ScrollToTop()
    {
        SetScrollAmount(0, true);
    }

    public override void ScrollToBottom()
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
        if (localCoordinates.Y < mBar.Y)
        {
            NudgeUp(this, EventArgs.Empty);
        }
        else if (localCoordinates.Y > mBar.Y + mBar.Height)
        {
            NudgeDown(this, EventArgs.Empty);
        }

        mDepressed = false;
        InputHandler.MouseFocus = null;
    }

    protected override float CalculateScrolledAmount()
    {
        return (float) (mBar.Y - ButtonSize) / (Height - mBar.Height - ButtonSize * 2);
    }

    /// <summary>
    ///     Sets the scroll amount (0-1).
    /// </summary>
    /// <param name="value">Scroll amount.</param>
    /// <param name="forceUpdate">Determines whether the control should be updated.</param>
    /// <returns>True if control state changed.</returns>
    public override bool SetScrollAmount(float value, bool forceUpdate = false)
    {
        value = Util.Clamp(value, 0, 1);

        if (!base.SetScrollAmount(value, forceUpdate))
        {
            return false;
        }

        if (forceUpdate)
        {
            var newY = (int) (ButtonSize + value * (Height - mBar.Height - ButtonSize * 2));
            mBar.MoveTo(mBar.X, newY);
        }

        return true;
    }

    /// <summary>
    ///     Handler for the BarMoved event.
    /// </summary>
    /// <param name="control">The control.</param>
    protected override void OnBarMoved(Base control, EventArgs args)
    {
        if (mBar.IsActive)
        {
            SetScrollAmount(CalculateScrolledAmount(), false);
            base.OnBarMoved(control, EventArgs.Empty);
        }
        else
        {
            InvalidateParent();
        }
    }

}
