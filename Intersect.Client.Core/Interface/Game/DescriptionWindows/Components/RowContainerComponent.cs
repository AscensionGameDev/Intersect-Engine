using Newtonsoft.Json.Linq;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Intersect.Client.Core;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components;

public partial class RowContainerComponent : ComponentBase
{
    private readonly JObject _keyValueRowLayout;

    private int _componentY = 0;

    public RowContainerComponent(Base parent, string name) : base(parent, name)
    {
        var _keyValueRow = new KeyValueRowComponent(this);
        LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        _keyValueRowLayout = _keyValueRow.GetJson() ?? throw new Exception($"Failed to load {nameof(KeyValueRowComponent)} layout for {Name}.");
        RemoveChild(_keyValueRow, true);
    }

    private void PositionComponent(ComponentBase component)
    {
        component.SetPosition(component.X, _componentY);
        _componentY += component.Height;
    }

    /// <inheritdoc/>
    public override void CorrectWidth()
    {
        base.CorrectWidth();
        var margins = Margin;
        SetSize(Width, Height + margins.Bottom);
    }

    /// <summary>
    /// Add a new <see cref="KeyValueRowComponent"/> row to the container.
    /// </summary>
    /// <param name="key">The key to display.</param>
    /// <param name="value">The value to display.</param>
    /// <returns>Returns a new instance of <see cref="KeyValueRowComponent"/> with the provided settings.</returns>
    public KeyValueRowComponent AddKeyValueRow(string key, string value)
    {
        var row = new KeyValueRowComponent(this, key, value);

        // Since we're pulling some trickery here, catch any errors doing this ourselves and log them.
        try
        {
            row.LoadJson(_keyValueRowLayout);
        }
        catch (Exception ex)
        {
            ApplicationContext.Context.Value?.Logger.LogError(ex, $"An error occured while loading {nameof(KeyValueRowComponent)} Json for {Name}");
        }

        row.SizeToChildren(true, false);
        PositionComponent(row);
        return row;
    }
}
