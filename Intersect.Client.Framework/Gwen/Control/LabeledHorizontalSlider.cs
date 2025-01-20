using Intersect.Client.Framework.GenericClasses;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Base slider.
/// </summary>
public partial class LabeledHorizontalSlider : Base
{
    private readonly Label _label;
    private readonly HorizontalSlider _slider;
    private readonly ImagePanel _sliderBackground;
    private readonly TextBoxNumeric _sliderValue;
    private double _scale = 1.0;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LabeledHorizontalSlider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public LabeledHorizontalSlider(Base parent, string name = "") : base(parent, name)
    {
        SetBounds(new Rectangle(0, 0, 32, 128));

        _label = new Label(this, "SliderLabel");

        _sliderBackground = new ImagePanel(this, "SliderBackground");
        _slider = new HorizontalSlider(_sliderBackground, "Slider");
        _sliderValue = new TextBoxNumeric(_sliderBackground, "SliderValue");

        _slider.ValueChanged += (sender, arguments) =>
        {
            if (sender == _sliderValue)
            {
                return;
            }

            _sliderValue.Value = _slider.Value * _scale;
            ValueChanged?.Invoke(sender, arguments);
        };

        _sliderValue.TextChanged += (sender, arguments) =>
        {
            if (sender == _slider)
            {
                return;
            }

            _slider.Value = _sliderValue.Value / _scale;
            ValueChanged?.Invoke(sender, arguments);
        };

        KeyboardInputEnabled = true;
        IsTabable = true;
    }

    public string Label
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    /// <summary>
    ///     Number of notches on the slider axis.
    /// </summary>
    public int NotchCount
    {
        get => _slider.NotchCount;
        set => _slider.NotchCount = value;
    }

    public double[] Notches
    {
        get => _slider.Notches;
        set => _slider.Notches = value;
    }

    /// <summary>
    ///     Determines whether the slider should snap to notches.
    /// </summary>
    public bool SnapToNotches
    {
        get => _slider.SnapToNotches;
        set => _slider.SnapToNotches = value;
    }

    /// <summary>
    ///     Minimum value.
    /// </summary>
    public double Min
    {
        get => _slider.Min;
        set => _slider.Min = value;
    }

    /// <summary>
    ///     Maximum value.
    /// </summary>
    public double Max
    {
        get => _slider.Max;
        set => _slider.Max = value;
    }

    public double Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _sliderValue.Value = _slider.Value * _scale;
        }
    }

    /// <summary>
    ///     Current value.
    /// </summary>
    public double Value
    {
        get => _slider.Value;
        set
        {
            _slider.Value = value;
            _sliderValue.Value = _slider.Value * _scale;
        }
    }

    /// <summary>
    ///     Invoked when the value has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> ValueChanged;

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
    }
}
