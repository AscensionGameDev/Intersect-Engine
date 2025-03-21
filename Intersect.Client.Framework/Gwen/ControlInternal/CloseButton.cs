﻿using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Window close button.
/// </summary>
public partial class CloseButton : Button
{

    private readonly WindowControl mWindow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CloseButton" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="owner">Window that owns this button.</param>
    /// <param name="name"></param>
    public CloseButton(Base parent, WindowControl owner, string? name = null) : base(parent, name, disableText: true)
    {
        mWindow = owner;
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawWindowCloseButton(this);
    }

}
