using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Skin.Texturing;
using Single = Intersect.Client.Framework.Gwen.Skin.Texturing.Single;

namespace Intersect.Client.Framework.Gwen.Skin;


/// <summary>
///     Base textured skin.
/// </summary>
public class Intersect2021 : TexturedBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
    /// </summary>
    /// <param name="renderer">Renderer to use.</param>
    /// <param name="textureName">Name of the skin texture map.</param>
    public Intersect2021(Renderer.Base renderer, GameContentManager contentManager) : base(renderer, contentManager, "intersect-2021.png")
    {
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
    }

    #region Initialization

    protected override void InitializeTextures()
    {
        base.InitializeTextures();

        mTextures.Window.Normal = new Bordered(mTexture, 0, 24, 16, 16, new Margin(4, 4, 4, 4));
        mTextures.Window.ActiveTitleBar = new Bordered(mTexture, 0, 0, 16, 24, new Margin(4, 4, 4, 4));
        mTextures.Window.Inactive = new Bordered(mTexture, 16, 24, 16, 16, new Margin(4, 4, 4, 4));
        mTextures.Window.InactiveTitleBar = new Bordered(mTexture, 16, 0, 16, 24, new Margin(4, 4, 4, 4));

        mTextures.Window.Close = new Single(mTexture, 60, 0, 24, 24);
        mTextures.Window.CloseDown = new Single(mTexture, 60, 24, 24, 24);
        mTextures.Window.CloseDisabled = new Single(mTexture, 60, 48, 24, 24);
        mTextures.Window.CloseHover = new Single(mTexture, 60, 72, 24, 24);

        mTextures.Panel.Normal = new Bordered(mTexture, 32, 0, 16, 16, Margin.Four);
        mTextures.Panel.Highlight = new Bordered(mTexture, 32, 16, 16, 16, Margin.Four);
        mTextures.Panel.Bright = new Bordered(mTexture, 32, 32, 16, 16, Margin.Four);
        mTextures.Panel.Dark = new Bordered(mTexture, 32, 48, 16, 16, Margin.Four);

        mTextures.Input.Button.Normal = new Bordered(mTexture, 48, 0, 12, 12, Margin.Four);
        mTextures.Input.Button.Disabled = new Bordered(mTexture, 48, 24, 12, 12, Margin.Four);
        mTextures.Input.Button.Hovered = new Bordered(mTexture, 48, 12, 12, 12, Margin.Four);
        mTextures.Input.Button.Pressed = new Bordered(mTexture, 48, 36, 12, 12, Margin.Four);

        mTextures.CheckBox.Default.Box = new Single(mTexture, 84, 0, 22, 22);
        mTextures.CheckBox.Default.Fill = new Single(mTexture, 106, 0, 22, 22);
        mTextures.CheckBox.Active.Box = new Single(mTexture, 84, 22, 22, 22);
        mTextures.CheckBox.Active.Fill = new Single(mTexture, 106, 22, 22, 22);
        mTextures.CheckBox.Hovered.Box = new Single(mTexture, 84, 44, 22, 22);
        mTextures.CheckBox.Hovered.Fill = new Single(mTexture, 106, 44, 22, 22);
        mTextures.CheckBox.Disabled.Box = new Single(mTexture, 84, 66, 22, 22);
        mTextures.CheckBox.Disabled.Fill = new Single(mTexture, 106, 66, 22, 22);

        mTextures.RadioButton.Default.Box = new Single(mTexture, 128, 0, 22, 22);
        mTextures.RadioButton.Default.Fill = new Single(mTexture, 150, 0, 22, 22);
        mTextures.RadioButton.Active.Box = new Single(mTexture, 128, 22, 22, 22);
        mTextures.RadioButton.Active.Fill = new Single(mTexture, 150, 22, 22, 22);
        mTextures.RadioButton.Hovered.Box = new Single(mTexture, 128, 44, 22, 22);
        mTextures.RadioButton.Hovered.Fill = new Single(mTexture, 150, 44, 22, 22);
        mTextures.RadioButton.Disabled.Box = new Single(mTexture, 128, 66, 22, 22);
        mTextures.RadioButton.Disabled.Fill = new Single(mTexture, 150, 66, 22, 22);

    }

    #endregion

    #region Rendering Overrides

    public override void DrawCheckBox(Control.Base control, bool selected, bool hovered, bool depressed)
    {
        if (!(control is CheckBox checkBox))
        {
            return;
        }

        if (TryGetOverrideTexture(checkBox, selected, depressed, out var overrideTexture))
        {
            Renderer.DrawColor = checkBox.RenderColor;
            Renderer.DrawTexturedRect(overrideTexture, checkBox.RenderBounds, checkBox.RenderColor);
            return;
        }

        SkinTextures._FillableButton buttonState = mTextures.CheckBox.Default;
        if (checkBox.IsDisabled)
        {
            buttonState = mTextures.CheckBox.Disabled;
        }
        else if (depressed)
        {
            buttonState = mTextures.CheckBox.Active;
        }
        else if (hovered)
        {
            buttonState = mTextures.CheckBox.Hovered;
        }

        buttonState.Box.Draw(Renderer, checkBox.RenderBounds, checkBox.RenderColor);
        if (checkBox.IsChecked)
        {
            buttonState.Fill.Draw(Renderer, checkBox.RenderBounds, checkBox.RenderColor);
        }
    }

    public override void DrawRadioButton(Control.Base control, bool selected, bool hovered, bool depressed)
    {
        if (!(control is RadioButton radioButton))
        {
            return;
        }

        if (TryGetOverrideTexture(radioButton, selected, depressed, out var overrideTexture))
        {
            Renderer.DrawColor = radioButton.RenderColor;
            Renderer.DrawTexturedRect(overrideTexture, radioButton.RenderBounds, radioButton.RenderColor);
            return;
        }

        SkinTextures._FillableButton buttonState = mTextures.RadioButton.Default;
        if (radioButton.IsDisabled)
        {
            buttonState = mTextures.RadioButton.Disabled;
        }
        else if (depressed)
        {
            buttonState = mTextures.RadioButton.Active;
        }
        else if (hovered)
        {
            buttonState = mTextures.RadioButton.Hovered;
        }

        buttonState.Box.Draw(Renderer, radioButton.RenderBounds, radioButton.RenderColor);
        if (radioButton.IsChecked)
        {
            buttonState.Fill.Draw(Renderer, radioButton.RenderBounds, radioButton.RenderColor);
        }
    }


    public override void DrawWindow(Control.Base control, int topHeight, bool inFocus)
    {
        if (control is not WindowControl windowControl)
        {
            return;
        }

        GameTexture? renderTexture = null;
        if (windowControl.TryGetTexture(WindowControl.ControlState.Active, out var activeTexture))
        {
            renderTexture = activeTexture;
        }

        if (windowControl.TryGetTexture(WindowControl.ControlState.Inactive, out var inactiveTexture))
        {
            renderTexture = inactiveTexture;
        }

        if (renderTexture != null)
        {
            Renderer.DrawColor = control.RenderColor;
            Renderer.DrawTexturedRect(renderTexture, control.RenderBounds, control.RenderColor);
            return;
        }

        Bordered titleBar;
        Bordered frame;

        if (inFocus)
        {
            titleBar = mTextures.Window.ActiveTitleBar;
            frame = mTextures.Window.Normal;
        }
        else
        {
            titleBar = mTextures.Window.InactiveTitleBar;
            frame = mTextures.Window.Inactive;
        }

        Rectangle frameBounds = windowControl.RenderBounds;

        var shouldDrawTitlebarBackground = titleBar != default && windowControl.TitleBar.ShouldDrawBackground;
        if (shouldDrawTitlebarBackground)
        {
            frameBounds = new Rectangle(
                0,
                windowControl.TitleBar.Bottom,
                control.RenderBounds.Width,
                control.RenderBounds.Height
            );
        }

        if (frame != default && windowControl.ShouldDrawBackground)
        {
            frame.Draw(Renderer, frameBounds, windowControl.RenderColor);
        }

        if (shouldDrawTitlebarBackground)
        {
            titleBar.Draw(Renderer, windowControl.TitleBar.Bounds, windowControl.RenderColor);
        }
    }

    #endregion

}
