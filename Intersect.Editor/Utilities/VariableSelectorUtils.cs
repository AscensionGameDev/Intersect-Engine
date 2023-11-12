using Intersect.Editor.Forms;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Extensions;

namespace Intersect.Editor.Utilities;
public static class VariableSelectorUtils
{
    public static void OpenVariableSelector(Action<VariableSelection> onSelectionComplete, 
        Guid selectedVariableId, 
        VariableType selectedVariableType,
        VariableDataType dataTypeFilter = 0)
    {
        var variableSelection = new FrmVariableSelector(selectedVariableType, selectedVariableId, dataTypeFilter);
        variableSelection.ShowDialog();

        if (variableSelection.GetResult())
        {
            var selection = variableSelection.GetSelection();

            onSelectionComplete.Invoke(selection);
        }
    }

    public static string GetSelectedVarText(VariableType variableType, Guid selectedVariableId)
    {
        var type = variableType.GetDescription();
        var varName = variableType.GetRelatedTable().GetLookup().Get(selectedVariableId)?.Name;

        if (varName == default || selectedVariableId == Guid.Empty)
        {
            return Strings.VariableSelector.ValueNoneSelected.ToString();
        }

        return Strings.VariableSelector.ValueCurrentSelection.ToString(varName, type);
    }
}
