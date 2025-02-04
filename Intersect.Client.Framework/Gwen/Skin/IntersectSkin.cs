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

    protected override void InitializeTextures()
    {
        base.InitializeTextures();

        mTextures.Input.Button.Normal = new Bordered(_texture, 48, 0, 12, 12, Margin.Four);
        mTextures.Input.Button.Disabled = new Bordered(_texture, 48, 24, 12, 12, Margin.Four);
        mTextures.Input.Button.Hovered = new Bordered(_texture, 48, 12, 12, 12, Margin.Four);
        mTextures.Input.Button.Pressed = new Bordered(_texture, 48, 36, 12, 12, Margin.Four);

        mTextures.CheckBox.Default.Box = new Single(_texture, 84, 0, 22, 22);
        mTextures.CheckBox.Default.Fill = new Single(_texture, 106, 0, 22, 22);
        mTextures.CheckBox.Active.Box = new Single(_texture, 84, 22, 22, 22);
        mTextures.CheckBox.Active.Fill = new Single(_texture, 106, 22, 22, 22);
        mTextures.CheckBox.Hovered.Box = new Single(_texture, 84, 44, 22, 22);
        mTextures.CheckBox.Hovered.Fill = new Single(_texture, 106, 44, 22, 22);
        mTextures.CheckBox.Disabled.Box = new Single(_texture, 84, 66, 22, 22);
        mTextures.CheckBox.Disabled.Fill = new Single(_texture, 106, 66, 22, 22);

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

        mTextures.Window.Close = new Single(_texture, 60, 0, 24, 24);
        mTextures.Window.CloseDown = new Single(_texture, 60, 24, 24, 24);
        mTextures.Window.CloseDisabled = new Single(_texture, 60, 48, 24, 24);
        mTextures.Window.CloseHover = new Single(_texture, 60, 72, 24, 24);
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
