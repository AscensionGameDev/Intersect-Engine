using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Input;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Base slider.
/// </summary>
public partial class Slider : Base
{

    protected readonly SliderBar mSliderBar;

    private GameTexture mBackgroundImage;

    private string mBackgroundImageFilename;

    protected double mMax;

    protected double mMin;

    protected int mNotchCount;

    protected double[] _notches;

    protected bool mSnapToNotches;

    protected double mValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Slider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    protected Slider(Base parent, string name = "") : base(parent, name)
    {
        SetBounds(new Rectangle(0, 0, 32, 128));

        mSliderBar = new SliderBar(this);
        mSliderBar.Dragged += OnMoved;

        mMin = 0.0f;
        mMax = 1.0f;

        mSnapToNotches = false;
        mNotchCount = 5;
        mValue = 0.0f;

        KeyboardInputEnabled = true;
        IsTabable = true;
    }

    /// <summary>
    ///     Number of notches on the slider axis.
    /// </summary>
    public int NotchCount
    {
        get => mNotchCount;
        set => mNotchCount = value;
    }

    public double[] Notches
    {
        get => _notches;
        set => _notches = value;
    }

    /// <summary>
    ///     Determines whether the slider should snap to notches.
    /// </summary>
    public bool SnapToNotches
    {
        get => mSnapToNotches;
        set => mSnapToNotches = value;
    }

    /// <summary>
    ///     Minimum value.
    /// </summary>
    public double Min
    {
        get => mMin;
        set => SetRange(value, mMax);
    }

    /// <summary>
    ///     Maximum value.
    /// </summary>
    public double Max
    {
        get => mMax;
        set => SetRange(mMin, value);
    }

    /// <summary>
    ///     Current value.
    /// </summary>
    public double Value
    {
        get => mMin + mValue * (mMax - mMin);
        set
        {
            if (value < mMin)
            {
                value = mMin;
            }

            if (value > mMax)
            {
                value = mMax;
            }

            // Normalize Value
            value = (value - mMin) / Math.Max(1, mMax - mMin);
            SetValueInternal(value);
            Redraw();
        }
    }

    /// <summary>
    ///     Invoked when the value has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> ValueChanged;

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        obj.Add("BackgroundImage", GetImageFilename());
        obj.Add("SnapToNotches", mSnapToNotches);
        obj.Add("NotchCount", mNotchCount);
        var notches = (Notches == default || Notches.Length < 1)
            ? default
            : new JArray(Notches.Cast<object>().ToArray());
        obj.Add(nameof(Notches), notches);
        obj.Add("SliderBar", mSliderBar.GetJson());

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["BackgroundImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string)obj["BackgroundImage"]
                ), (string)obj["BackgroundImage"]
            );
        }

        if (obj["SnapToNotches"] != null)
        {
            mSnapToNotches = (bool)obj["SnapToNotches"];
        }

        if (obj["NotchCount"] != null)
        {
            mNotchCount = (int)obj["NotchCount"];
        }

        var notches = obj[nameof(Notches)];
        if (notches != null && notches.Type != JTokenType.Null)
        {
            Notches = ((JArray)notches).Select(token => (double)token).ToArray();
            if (Notches.Length < 1)
            {
                Notches = default;
            }
        }
        else
        {
            Notches = default;
        }

        if (obj["SliderBar"] != null)
        {
            mSliderBar.LoadJson(obj["SliderBar"]);
        }
    }

    /// <summary>
    ///     Handler for Right Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyRight(bool down)
    {
        if (down)
        {
            Value = Value + 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyUp(bool down)
    {
        if (down)
        {
            Value = Value + 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Left Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyLeft(bool down)
    {
        if (down)
        {
            Value = Value - 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDown(bool down)
    {
        if (down)
        {
            Value = Value - 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Home keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyHome(bool down)
    {
        if (down)
        {
            Value = mMin;
        }

        return true;
    }

    /// <summary>
    ///     Handler for End keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyEnd(bool down)
    {
        if (down)
        {
            Value = mMax;
        }

        return true;
    }

    /// <summary>
    ///     Handler invoked on mouse click (left) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="down">If set to <c>true</c> mouse button is down.</param>
    protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
    {
    }

    protected virtual void OnMoved(Base control, EventArgs args)
    {
        SetValueInternal(CalculateValue());
    }

    protected virtual float CalculateValue()
    {
        return 0;
    }

    protected virtual void UpdateBarFromValue()
    {
    }

    protected virtual void SetValueInternal(double val)
    {
        if (mSnapToNotches)
        {
            if (_notches == default || _notches.Length < 1)
            {
                val = Math.Floor(val * mNotchCount + 0.5f);
                val /= mNotchCount;
            }
            else
            {
                var notchMin = _notches.Min();
                var notchMax = _notches.Max();
                var notchRange = notchMax - notchMin;
                var sorted = _notches.OrderBy(notchValue => Math.Abs(notchMin + val * notchRange - notchValue)).ToArray();
                val = (sorted.First() - notchMin) / notchRange;
            }
        }

        if (mValue != val)
        {
            mValue = val;
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        UpdateBarFromValue();
    }

    /// <summary>
    ///     Sets the value range.
    /// </summary>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    public void SetRange(double min, double max)
    {
        mMin = min;
        mMax = max;
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderFocus(Skin.Base skin)
    {
        if (InputHandler.KeyboardFocus != this)
        {
            return;
        }

        if (!IsTabable)
        {
            return;
        }

        //skin.DrawKeyboardHighlight(this, RenderBounds, 0);
    }

    public void SetImage(GameTexture img, string fileName)
    {
        mBackgroundImage = img;
        mBackgroundImageFilename = fileName;
    }

    public GameTexture GetImage()
    {
        return mBackgroundImage;
    }

    public string GetImageFilename()
    {
        return mBackgroundImageFilename;
    }

    public void SetDraggerImage(GameTexture img, string fileName, Dragger.ControlState state)
    {
        mSliderBar.SetImage(img, fileName, state);
    }

    public GameTexture GetDraggerImage(Dragger.ControlState state)
    {
        return mSliderBar.GetImage(state);
    }

    public void SetDraggerSize(int w, int h)
    {
        mSliderBar.SetSize(w, h);
    }

}
