using Intersect.Client.Framework.UserInterface.Styling;
using ImGuiNET;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Canvas : Window
{
    protected const ImGuiWindowFlags CanvasFlags = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs;

    public Canvas(string name) : base(name)
    {
        if (name == default)
        {
            throw new ArgumentNullException(nameof(name));
        }

        DisplayMode = DisplayMode.Block;
        Flags = CanvasFlags;
    }
}
