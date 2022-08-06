using System.Linq.Expressions;

using ImGuiNET;

using Intersect.Collections;
using Intersect.Editor.Localization;
using Intersect.Localization;
using Intersect.Models;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class LocalizationWindow
{
    protected bool InputTextWithHint<TObject, TProperty>(
        TObject target,
        (Type, Guid, string) compositeId,
        Expression<Func<TObject, TProperty?>> propertyExpression,
        out TProperty? currentValue,
        string? imguiLabel,
        string imguiId,
        LocalizedString? hint,
        uint maxLength = default,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None,
        LocalizedString? tooltip = default
    )
    {
        var compiledGetter = propertyExpression.Compile();
        var previousValue = _edited.TryGetValue(compositeId, out var editedObject)
            ? compiledGetter((TObject)editedObject)
            : (target != null ? compiledGetter(target) : default);

        var currentStringValue = previousValue?.ToString() ?? string.Empty;
        _ = ImGui.InputTextWithHint(
            $"{imguiLabel ?? string.Empty}###{imguiId}",
            hint,
            ref currentStringValue,
            maxLength,
            flags
        );

        if (!string.IsNullOrWhiteSpace(tooltip) && ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text(tooltip);
            ImGui.EndTooltip();
        }

        if (typeof(TProperty) != typeof(string))
        {
            throw new NotSupportedException(
                $"{nameof(InputTextWithHint)} does not support {typeof(TProperty).FullName}."
            );
        }

        currentValue = (TProperty?)(object?)(string.IsNullOrEmpty(currentStringValue) ? default : currentStringValue);
        return DetectChanges(
            compositeId,
            target,
            propertyExpression,
            currentValue,
            out _
        );
    }

    protected bool LayoutTable(FrameTime frameTime)
    {
        var tableFlags = ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInner;

        var supportedCultures = Options.Instance.Localization.SupportedCultureInfos.ToArray();
        var columnCount = 2 + supportedCultures.Length;
        if (!ImGui.BeginTable($"localization_table_strings", columnCount, tableFlags, default))
        {
            return false;
        }

        ImGui.TableSetupColumn($"{Strings.Windows.Localization.ColumnId}###localization_column_id", ImGuiTableColumnFlags.WidthFixed, 25f);
        ImGui.TableSetupColumn($"{Strings.Windows.Localization.ColumnComment}###localization_column_comment", ImGuiTableColumnFlags.WidthStretch, 0.8f / (columnCount - 1));

        foreach (var supportedCulture in supportedCultures)
        {
            var columnLabel = Strings.Windows.Localization.ColumnLocaleShort.ToString(
                supportedCulture.IetfLanguageTag,
                supportedCulture.DisplayName
            );
            ImGui.TableSetupColumn($"{columnLabel}###localization_column_{supportedCulture.IetfLanguageTag}", ImGuiTableColumnFlags.WidthStretch, 0.9f / (columnCount - 1));
        }

        ImGui.TableHeadersRow();

        var hasChanges = false;
        var contentStrings = ObjectStore<ContentString>.Instance.ValueList;
        foreach (var contentString in contentStrings)
        {
            ImGui.TableNextRow();

            var columnIndex = 0;
            _ = ImGui.TableSetColumnIndex(columnIndex++);
            ImGui.Text(contentString.Id.Guid.ToString());
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text(contentString.Id.Guid.ToString());
                ImGui.EndTooltip();
            }

            _ = ImGui.TableSetColumnIndex(columnIndex++);
            hasChanges |= InputTextWithHint(
                contentString,
                (typeof(ContentString), contentString.Id.Guid, string.Empty),
                contentString => contentString.Comment,
                out _,
                default,
                $"localization_inputtext_{contentString.Id.Guid}_comment",
                Strings.Windows.Localization.ColumnCommentHint,
                255,
                ImGuiInputTextFlags.AutoSelectAll,
                Strings.Windows.Localization.ColumnCommentHint
            );

            foreach (var supportedCulture in supportedCultures)
            {
                _ = ImGui.TableSetColumnIndex(columnIndex++);

                var locale = supportedCulture.IetfLanguageTag;
                _ = contentString.TryGetValue(
                    locale,
                    out var localization
                );

                hasChanges |= InputTextWithHint(
                    localization!,
                    (typeof(LocaleContentString), contentString.Id.Guid, locale),
                    localization => localization.Value,
                    out _,
                    default,
                    $"localization_inputtext_{contentString.Id.Guid}_value_{locale}",
                    Strings.Windows.Localization.ColumnLocaleHint,
                    255,
                    ImGuiInputTextFlags.AutoSelectAll,
                    Strings.Windows.Localization.ColumnLocaleHint
                );
            }
        }

        ImGui.EndTable();

        HasPendingChanges = hasChanges;

        return true;
    }
}
