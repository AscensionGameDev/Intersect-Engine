using System.Resources;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Skin.Texturing;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Single = Intersect.Client.Framework.Gwen.Skin.Texturing.Single;

namespace Intersect.Client.Framework.Gwen.Skin;


/// <summary>
///     Base textured skin.
/// </summary>
public class IntersectSkin : TexturedBase
{
    private static GameTexture LoadEmbeddedSkinTexture(GameContentManager contentManager)
    {
        const string skinTextureName = "skin-intersect.png";
        var skinResourceName = $"{typeof(IntersectSkin).Namespace}.{skinTextureName}";
        var resourceNames = typeof(IntersectSkin).Assembly.GetManifestResourceNames();
        if (!resourceNames.Contains(skinResourceName))
        {
            throw new MissingManifestResourceException("Missing embedded Intersect GWEN skin");
        }

        if (contentManager.GetTexture(TextureType.Gui, skinTextureName) is { } textureFromFile)
        {
            ApplicationContext.Context.Value?.Logger.LogDebug(
                "{SkinName} was found on disk, not using embedded version",
                skinTextureName
            );
            return textureFromFile;
        }

        ApplicationContext.Context.Value?.Logger.LogDebug(
            "{SkinName} was not found on disk, using embedded version",
            skinTextureName
        );

        return contentManager.Load<GameTexture>(
            ContentType.Interface,
            skinTextureName,
            () => typeof(IntersectSkin).Assembly.GetManifestResourceStream(skinResourceName) ??
                  throw new MissingManifestResourceException($"Missing '{skinResourceName}")
        );
    }

    public IntersectSkin(Renderer.Base renderer, GameContentManager contentManager) : base(
        renderer,
        LoadEmbeddedSkinTexture(contentManager)
    )
    {

    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
    /// </summary>
    /// <param name="renderer">Renderer to use.</param>
    /// <param name="contentManager"></param>
    /// <param name="textureName"></param>
    public IntersectSkin(Renderer.Base renderer, GameContentManager contentManager, string textureName) : base(
        renderer,
        contentManager,
        textureName
    )
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

    protected override void InitializeColors()
    {
        base.InitializeColors();

        Colors.Button.Normal = Renderer.PixelColor(_texture, 0, 480, Color.Yellow);
        Colors.Button.Disabled = Renderer.PixelColor(_texture, 8, 480, Color.Yellow);
        Colors.Button.Hover = Renderer.PixelColor(_texture, 16, 480, Color.Yellow);
        Colors.Button.Active = Renderer.PixelColor(_texture, 24, 480, Color.Yellow);

        Colors.Label.Normal = Renderer.PixelColor(_texture, 32, 480, Color.Yellow);
        Colors.Label.Disabled = Renderer.PixelColor(_texture, 40, 480, Color.Yellow);
        Colors.Label.Hover = Renderer.PixelColor(_texture, 48, 480, Color.Yellow);
        Colors.Label.Active = Renderer.PixelColor(_texture, 56, 480, Color.Yellow);

        Colors.Tab.Active.Normal = Renderer.PixelColor(_texture, 0, 480, Color.Yellow);
        Colors.Tab.Active.Disabled = Renderer.PixelColor(_texture, 8, 480, Color.Yellow);
        Colors.Tab.Active.Hover = Renderer.PixelColor(_texture, 16, 480, Color.Yellow);
        Colors.Tab.Active.Active = Renderer.PixelColor(_texture, 24, 480, Color.Yellow);
        Colors.Tab.Inactive.Normal = Renderer.PixelColor(_texture, 0, 480, Color.Yellow);
        Colors.Tab.Inactive.Disabled = Renderer.PixelColor(_texture, 8, 480, Color.Yellow);
        Colors.Tab.Inactive.Hover = Renderer.PixelColor(_texture, 16, 480, Color.Yellow);
        Colors.Tab.Inactive.Active = Renderer.PixelColor(_texture, 24, 480, Color.Yellow);
    }

    protected override void InitializeTextures()
    {
        base.InitializeTextures();

        mTextures.CheckBox.Default.Box = new Single(_texture, 84, 0, 22, 22);
        mTextures.CheckBox.Default.Fill = new Single(_texture, 106, 0, 22, 22);
        mTextures.CheckBox.Active.Box = new Single(_texture, 84, 22, 22, 22);
        mTextures.CheckBox.Active.Fill = new Single(_texture, 106, 22, 22, 22);
        mTextures.CheckBox.Hovered.Box = new Single(_texture, 84, 44, 22, 22);
        mTextures.CheckBox.Hovered.Fill = new Single(_texture, 106, 44, 22, 22);
        mTextures.CheckBox.Disabled.Box = new Single(_texture, 84, 66, 22, 22);
        mTextures.CheckBox.Disabled.Fill = new Single(_texture, 106, 66, 22, 22);

        mTextures.Input.Button.Normal = new Bordered(_texture, 48, 0, 12, 12, Margin.Four);
        mTextures.Input.Button.Disabled = new Bordered(_texture, 48, 12, 12, 12, Margin.Four);
        mTextures.Input.Button.Hovered = new Bordered(_texture, 48, 24, 12, 12, Margin.Four);
        mTextures.Input.Button.Active = new Bordered(_texture, 48, 36, 12, 12, Margin.Four);

        mTextures.Input.ComboBox.Normal = new Bordered(_texture, 385, 336, 126, 31, new Margin(8, 8, 32, 8));
        mTextures.Input.ComboBox.Hover = new Bordered(_texture, 385, 336 + 32, 126, 31, new Margin(8, 8, 32, 8));
        mTextures.Input.ComboBox.Down = new Bordered(_texture, 385, 336 + 64, 126, 31, new Margin(8, 8, 32, 8));
        mTextures.Input.ComboBox.Disabled = new Bordered(_texture, 385, 336 + 96, 126, 31, new Margin(8, 8, 32, 8));

        mTextures.Input.Slider.H.Normal = new Bordered(_texture, 176, 0, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.H.Disabled = new Bordered(_texture, 176, 8, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.H.Hover = new Bordered(_texture, 176, 16, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.H.Active = new Bordered(_texture, 176, 24, 7, 7, new Margin(3, 2, 3, 4));

        mTextures.Input.Slider.V.Normal = new Bordered(_texture, 192, 0, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.V.Disabled = new Bordered(_texture, 192, 8, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.V.Hover = new Bordered(_texture, 192, 16, 7, 7, new Margin(3, 2, 3, 4));
        mTextures.Input.Slider.V.Active = new Bordered(_texture, 192, 24, 7, 7, new Margin(3, 2, 3, 4));

        mTextures.Menu.Strip = new Bordered(_texture, 0, 128, 127, 21, Margin.One);
        mTextures.Menu.BackgroundWithMargin = new Bordered(_texture, 96, 96, 32, 16, new Margin(24, 4, 4, 4));
        mTextures.Menu.Background = new Bordered(_texture, 96, 112, 16, 16, Margin.Four);
        mTextures.Menu.Hovered = new Bordered(_texture, 320, 320, 32, 32, Margin.Six);
        mTextures.Menu.RightArrow = new Single(_texture, 464, 112, 15, 15);
        mTextures.Menu.Check = new Single(_texture, 448, 112, 15, 15);

        mTextures.Panel.Control = new Bordered(_texture, 64, 112, 16, 16, Margin.Four);
        mTextures.Panel.Normal = new Bordered(_texture, 32, 0, 16, 16, Margin.Four);
        mTextures.Panel.Highlight = new Bordered(_texture, 32, 16, 16, 16, Margin.Four);
        mTextures.Panel.Bright = new Bordered(_texture, 32, 32, 16, 16, Margin.Four);
        mTextures.Panel.Dark = new Bordered(_texture, 32, 48, 16, 16, Margin.Four);

        mTextures.RadioButton.Default.Box = new Single(_texture, 128, 0, 22, 22);
        mTextures.RadioButton.Default.Fill = new Single(_texture, 150, 0, 22, 22);
        mTextures.RadioButton.Active.Box = new Single(_texture, 128, 22, 22, 22);
        mTextures.RadioButton.Active.Fill = new Single(_texture, 150, 22, 22, 22);
        mTextures.RadioButton.Hovered.Box = new Single(_texture, 128, 44, 22, 22);
        mTextures.RadioButton.Hovered.Fill = new Single(_texture, 150, 44, 22, 22);
        mTextures.RadioButton.Disabled.Box = new Single(_texture, 128, 66, 22, 22);
        mTextures.RadioButton.Disabled.Fill = new Single(_texture, 150, 66, 22, 22);

        mTextures.Scroller.TrackV = new Bordered(_texture, 176, 32, 15, 15, Margin.Four);
        mTextures.Scroller.BarV.Normal = new Bordered(_texture, 192, 32, 15, 15, Margin.Four);
        mTextures.Scroller.BarV.Disabled = new Bordered(_texture, 208, 32, 15, 15, Margin.Four);
        mTextures.Scroller.BarV.Hovered = new Bordered(_texture, 224, 32, 15, 15, Margin.Four);
        mTextures.Scroller.BarV.Active = new Bordered(_texture, 240, 32, 15, 15, Margin.Four);
        mTextures.Scroller.TrackH = new Bordered(_texture, 176, 80, 15, 15, Margin.Four);
        mTextures.Scroller.BarH.Normal = new Bordered(_texture, 192, 80, 15, 15, Margin.Four);
        mTextures.Scroller.BarH.Disabled = new Bordered(_texture, 208, 80, 15, 15, Margin.Four);
        mTextures.Scroller.BarH.Hovered = new Bordered(_texture, 224, 80, 15, 15, Margin.Four);
        mTextures.Scroller.BarH.Active = new Bordered(_texture, 240, 80, 15, 15, Margin.Four);

        int[] scrollerArrowButtonY = [96, 48, 112, 64];
        mTextures.Scroller.Button.Normal = scrollerArrowButtonY.Select(y => new Bordered(_texture, 192, y, 15, 15, Margin.Four)).ToArray();
        mTextures.Scroller.Button.Disabled = scrollerArrowButtonY.Select(y => new Bordered(_texture, 208, y, 15, 15, Margin.Four)).ToArray();
        mTextures.Scroller.Button.Hovered = scrollerArrowButtonY.Select(y => new Bordered(_texture, 224, y, 15, 15, Margin.Four)).ToArray();
        mTextures.Scroller.Button.Active = scrollerArrowButtonY.Select(y => new Bordered(_texture, 240, y, 15, 15, Margin.Four)).ToArray();

        mTextures.Tab.Control = new Bordered(_texture, 0, 48, 16, 16, Margin.Four);
        mTextures.Tab.Top.Active = new Bordered(_texture, 0, 64, 16, 10, new Margin(4, 4, 4, 2));
        mTextures.Tab.Top.Inactive = new Bordered(_texture, 16, 64, 16, 10, new Margin(4, 4, 4, 2));
        mTextures.Tab.Bottom.Active = new Bordered(_texture, 0, 80, 16, 10, new Margin(4, 2, 4, 4));
        mTextures.Tab.Bottom.Inactive = new Bordered(_texture, 16, 80, 16, 10, new Margin(4, 2, 4, 4));
        mTextures.Tab.Right.Active = new Bordered(_texture, 0, 96, 10, 16, new Margin(2, 4, 4, 4));
        mTextures.Tab.Right.Inactive = new Bordered(_texture, 16, 96, 10, 16, new Margin(2, 4, 4, 4));
        mTextures.Tab.Left.Active = new Bordered(_texture, 0, 112, 10, 16, new Margin(4, 4, 2, 4));
        mTextures.Tab.Left.Inactive = new Bordered(_texture, 16, 112, 10, 16, new Margin(4, 4, 2, 4));

        mTextures.TextBox.Normal = new Bordered(_texture, 32, 64, 16, 16, Margin.Four);
        mTextures.TextBox.Focus = new Bordered(_texture, 32, 80, 16, 16, Margin.Four);
        mTextures.TextBox.Disabled = new Bordered(_texture, 32, 96, 16, 16, Margin.Four);

        mTextures.Tooltip = new Bordered(_texture, 64, 96, 16, 16, Margin.Four);

        mTextures.Tree.Background = new Bordered(_texture, 32, 112, 16, 16, Margin.Four);
        mTextures.Tree.Plus = new Single(_texture, 448, 96, 15, 15);
        mTextures.Tree.Minus = new Single(_texture, 464, 96, 15, 15);

        mTextures.Window.Normal = new Bordered(_texture, 0, 24, 16, 16, Margin.Four);
        mTextures.Window.ActiveTitleBar = new Bordered(_texture, 0, 0, 16, 24, Margin.Four);
        mTextures.Window.Inactive = new Bordered(_texture, 16, 24, 16, 16, Margin.Four);
        mTextures.Window.InactiveTitleBar = new Bordered(_texture, 16, 0, 16, 24, Margin.Four);

        mTextures.Window.CloseButton.Normal = new FivePatch(_texture, 60, 0, 24, 24, Margin.Two, Margin.Two);
        mTextures.Window.CloseButton.Disabled = new FivePatch(_texture, 60, 48, 24, 24, Margin.Two, Margin.Two);
        mTextures.Window.CloseButton.Hovered = new FivePatch(_texture, 60, 72, 24, 24, Margin.Two, Margin.Two);
        mTextures.Window.CloseButton.Active = new FivePatch(_texture, 60, 24, 24, 24, Margin.Two, Margin.Two);
    }

    #endregion

    #region Rendering Overrides

    public override void DrawCheckBox(Control.Base control, bool selected, bool hovered, bool depressed)
    {
        if (!(control is Checkbox checkBox))
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

        Rectangle frameBounds = windowControl.Bounds;

        var shouldDrawTitlebarBackground = titleBar != default && windowControl.Titlebar.ShouldDrawBackground;
        if (shouldDrawTitlebarBackground)
        {
            var titlebarBottom = windowControl.Titlebar.Bottom;
            frameBounds = new Rectangle(
                0,
                titlebarBottom,
                control.RenderBounds.Width,
                control.RenderBounds.Height - titlebarBottom
            );
        }

        if (frame != default && windowControl.ShouldDrawBackground)
        {
            frame.Draw(Renderer, frameBounds, windowControl.RenderColor);
        }

        if (shouldDrawTitlebarBackground)
        {
            titleBar.Draw(Renderer, windowControl.Titlebar.Bounds, windowControl.RenderColor);
        }
    }

    #endregion

}
