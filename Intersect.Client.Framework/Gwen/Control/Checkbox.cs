using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Framework.Eventing;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     CheckBox control.
/// </summary>
public partial class Checkbox : Button, ICheckbox
{

    public enum ControlState
    {

        Normal = 0,

        Disabled,

        CheckedNormal,

        CheckedDisabled,

    }

    private bool mChecked;

    private IGameTexture mCheckedDisabledImage;

    private string mCheckedDisabledImageFilename;

    private IGameTexture mCheckedNormalImage;

    private string mCheckedNormalImageFilename;

    //Sound Effects
    private string mCheckSound;

    private IGameTexture mDisabledImage;

    private string mDisabledImageFilename;

    private IGameTexture mNormalImage;

    private string mNormalImageFilename;

    private string mUncheckedSound;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Checkbox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    /// <param name="disableText"></param>
    public Checkbox(Base parent, string? name = default, bool disableText = true) : base(parent, name, disableText)
    {
        MinimumSize = new Point(22, 22);
        Size = new Point(22, 22);
        IsToggle = true;
    }

    /// <summary>
    ///     Indicates whether the checkbox is checked.
    /// </summary>
    public bool IsChecked
    {
        get => mChecked;
        set
        {
            if (mChecked == value)
            {
                return;
            }

            mChecked = value;
            OnCheckChanged();
        }
    }

    /// <summary>
    ///     Determines whether unchecking is allowed.
    /// </summary>
    protected virtual bool AllowUncheck => true;

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add("NormalImage", GetImageFilename(ControlState.Normal));
        serializedProperties.Add("CheckedImage", GetImageFilename(ControlState.CheckedNormal));
        serializedProperties.Add("DisabledImage", GetImageFilename(ControlState.Disabled));
        serializedProperties.Add("CheckedDisabledImage", GetImageFilename(ControlState.CheckedDisabled));
        serializedProperties.Add("CheckedSound", mCheckSound);
        serializedProperties.Add("UncheckedSound", mUncheckedSound);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["NormalImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui, (string)obj["NormalImage"]
                ), (string)obj["NormalImage"], ControlState.Normal
            );
        }

        if (obj["CheckedImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui, (string)obj["CheckedImage"]
                ), (string)obj["CheckedImage"], ControlState.CheckedNormal
            );
        }

        if (obj["DisabledImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui, (string)obj["DisabledImage"]
                ), (string)obj["DisabledImage"], ControlState.Disabled
            );
        }

        if (obj["CheckedDisabledImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui, (string)obj["CheckedDisabledImage"]
                ), (string)obj["CheckedDisabledImage"], ControlState.CheckedDisabled
            );
        }

        if (obj["CheckedSound"] != null)
        {
            mCheckSound = (string)obj["CheckedSound"];
        }

        if (obj["UncheckedSound"] != null)
        {
            mUncheckedSound = (string)obj["UncheckedSound"];
        }
    }

    /// <summary>
    ///     Toggles the checkbox.
    /// </summary>
    public override void Toggle()
    {
        base.Toggle();
        IsChecked = !IsChecked;
        if (IsChecked)
        {
            base.PlaySound(mCheckSound);
        }
        else
        {
            base.PlaySound(mUncheckedSound);
        }
    }

    /// <summary>
    ///     Invoked when the checkbox has been checked.
    /// </summary>
    public event EventHandler<ICheckbox, EventArgs>? Checked;

    /// <summary>
    ///     Invoked when the checkbox has been unchecked.
    /// </summary>
    public event EventHandler<ICheckbox, EventArgs>? Unchecked;

    /// <summary>
    ///     Invoked when the checkbox state has been changed.
    /// </summary>
    public event EventHandler<ICheckbox, EventArgs>? CheckChanged;

    /// <summary>
    ///     Handler for CheckChanged event.
    /// </summary>
    protected virtual void OnCheckChanged()
    {
        if (IsChecked)
        {
            if (Checked != null)
            {
                Checked.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (Unchecked != null)
            {
                Unchecked.Invoke(this, EventArgs.Empty);
            }
        }

        if (CheckChanged != null)
        {
            CheckChanged.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        //base.Render(skin);
        skin.DrawCheckBox(this, mChecked, IsHovered, IsActive);
    }

    public void SetCheckSize(int w, int h)
    {
    }

    /// <summary>
    ///     Internal OnPressed implementation.
    /// </summary>
    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        if (IsChecked && !AllowUncheck)
        {
            return;
        }

        base.OnMouseClicked(mouseButton, mousePosition, userAction);
    }

    public void SetImage(IGameTexture texture, string fileName, ControlState state)
    {
        switch (state)
        {
            case ControlState.Normal:
                mNormalImageFilename = fileName;
                mNormalImage = texture;

                break;
            case ControlState.Disabled:
                mDisabledImageFilename = fileName;
                mDisabledImage = texture;

                break;
            case ControlState.CheckedNormal:
                mCheckedNormalImageFilename = fileName;
                mCheckedNormalImage = texture;

                break;
            case ControlState.CheckedDisabled:
                mCheckedDisabledImageFilename = fileName;
                mCheckedDisabledImage = texture;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public IGameTexture GetImage(ControlState state)
    {
        switch (state)
        {
            case ControlState.Normal:
                return mNormalImage;
            case ControlState.Disabled:
                return mDisabledImage;
            case ControlState.CheckedNormal:
                return mCheckedNormalImage;
            case ControlState.CheckedDisabled:
                return mCheckedDisabledImage;
            default:
                return null;
        }
    }

    public string GetImageFilename(ControlState state)
    {
        switch (state)
        {
            case ControlState.Normal:
                return mNormalImageFilename;
            case ControlState.Disabled:
                return mDisabledImageFilename;
            case ControlState.CheckedNormal:
                return mCheckedNormalImageFilename;
            case ControlState.CheckedDisabled:
                return mCheckedDisabledImageFilename;
            default:
                return null;
        }
    }

}
