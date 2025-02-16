using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Input;

public interface IGameInput
{
    IControlSet ControlSet { get; set; }

    Pointf MousePosition { get; }

    bool IsKeyDown(Keys key);
    bool WasKeyDown(Keys key);

    bool IsMouseButtonDown(MouseButton mb);
    bool WasMouseButtonDown(MouseButton mb);

    /// <summary>
    /// The available controls providers for this input instance.
    /// </summary>
    IReadOnlySet<IControlsProvider> ControlsProviders { get; }

    /// <summary>
    /// Adds one or more <see cref="IControlsProvider" />s to this input instance.
    /// </summary>
    /// <remarks>This method should attempt to add all providers even if one fails to be added.</remarks>
    /// <param name="controlsProviders">The <see cref="IControlsProvider" />(s) to add.</param>
    /// <returns>If all the specified providers were added or already existed in the set.</returns>
    bool AddControlsProviders(params IControlsProvider[] controlsProviders);

    /// <summary>
    /// Removes one or more <see cref="IControlsProvider" />s from this input instance.
    /// </summary>
    /// <remarks>This method should attempt to remove all providers even if one fails to be removed.</remarks>
    /// <param name="controlsProviders">The <see cref="IControlsProvider" />(s) to remove.</param>
    /// <returns>If all the specified providers were removed or already did not exist in the set.</returns>
    bool RemoveControlsProviders(params IControlsProvider[] controlsProviders);
}