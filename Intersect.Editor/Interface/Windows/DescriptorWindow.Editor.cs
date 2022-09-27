using ImGuiNET;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Models.Annotations;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow
{
    protected virtual bool LayoutEditor(FrameTime frameTime)
    {
        var descriptorLookup = DescriptorType.GetLookup();

        var contentRegionMax = ImGui.GetWindowContentRegionMax();
        var editorCursorPosition = ImGui.GetCursorPos();
        var editorContentArea = contentRegionMax - editorCursorPosition;
        ImGui.BeginChild(
            "###descriptor_editor",
            editorContentArea,
            true
        );

        if (_selectedObjectId != default)
        {
            var selectedDescriptor = descriptorLookup.Get<Descriptor>(_selectedObjectId);

            foreach (var groupInteropInfo in ObjectInteropModel.Groups)
            {
                var editorGroupId = $"descriptor_editor_group_{groupInteropInfo.Name?.Name}";

                if (!ImGui.CollapsingHeader($"{groupInteropInfo.Name?.Get(Strings.Root) ?? Strings.General.Miscellaneous}###{editorGroupId}"))
                {
                    continue;
                }

                // ImGui.BeginChild(
                //     $"descriptor_editor_group_{groupInteropInfo.Name?.Get(Strings.Root)}",
                //     new(),
                //     true,
                //     ImGuiWindowFlags.AlwaysAutoResize
                // );

                ImGui.BeginGroup();

                if (!ImGui.BeginTable($"{editorGroupId}_property_table", 2, ImGuiTableFlags.SizingFixedFit, default))
                {
                    return false;
                }

                ImGui.TableSetupColumn(
                    string.Empty,
                    ImGuiTableColumnFlags.None//ImGuiTableColumnFlags.WidthStretch,
                    // 0.15f
                );

                ImGui.TableSetupColumn(
                    string.Empty,
                    ImGuiTableColumnFlags.WidthStretch
                );

                var descriptorName = DescriptorType.GetLocalizedName(Strings.Descriptors);

                foreach (var propertyInteropInfo in groupInteropInfo.Properties)
                {
                    ImGui.TableNextRow();
                    try
                    {
                        var propertyName = propertyInteropInfo.PropertyInfo.GetFullName();
                        var propertyValue = propertyInteropInfo.DelegateGet.DynamicInvoke(selectedDescriptor);
                        var propertyId = $"descriptor_editor_{selectedDescriptor.Id}_{propertyName}";

                        var inputAttribute = propertyInteropInfo.Input;
                        var label = propertyInteropInfo.Label?.Get(Strings.Root);
                        if (label != default)
                        {
                            ImGui.TableSetColumnIndex(0);
                            var labelText = label.ToString(descriptorName);
                            var labelSize = ImGui.CalcTextSize(labelText);
                            var labelColumnWidth = ImGui.GetColumnWidth();
                            if (labelColumnWidth > labelSize.X)
                            {
                                var labelCursorPositionX = ImGui.GetCursorPos().X + labelColumnWidth - labelSize.X;
                                ImGui.SetCursorPosX(labelCursorPositionX);
                            }
                            ImGui.Text(labelText);
                            ImGui.SameLine();
                        }

                        ImGui.TableSetColumnIndex(1);
                        switch (inputAttribute)
                        {
                            case null:
                                ImGui.Text($"{propertyValue}");
                                break;

                            case InputLookupAttribute inputLookupAttribute:
                            {
                                // https://gist.github.com/harold-b/7dcc02557c2b15d76c61fde1186e31d0
                                var selectedCombo = 0;
                                // ImGui.BeginCombo(
                                //     $"{inputLookupAttribute.ForeignKeyName}###{propertyId}",
                                //     propertyValue?.ToString()
                                // );
                                ImGui.Combo(
                                    $"###{propertyId}",
                                    ref selectedCombo,
                                    new[] { propertyValue?.ToString() ?? "Test" },
                                    1
                                );
                                break;
                            }

                            case InputTextAttribute inputTextAttribute:
                            {
                                var hint = inputTextAttribute.GetHint(Strings.Root);
                                var inputTextValue = propertyValue as string ?? propertyValue?.ToString();
                                var textInputFlags = ImGuiInputTextFlags.AutoSelectAll;
                                var maxLength = inputTextAttribute.MaximumLength;
                                if (propertyInteropInfo.IsReadOnly || inputTextAttribute.ReadOnly)
                                {
                                    textInputFlags |= ImGuiInputTextFlags.ReadOnly;
                                }

                                ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                                var result = hint == default
                                    ? ImGui.InputText(
                                        $"###{propertyId}",
                                        ref inputTextValue,
                                        maxLength,
                                        textInputFlags
                                    )
                                    : ImGui.InputTextWithHint(
                                        $"###{propertyId}",
                                        hint,
                                        ref inputTextValue,
                                        maxLength,
                                        textInputFlags
                                    );

                                propertyInteropInfo.DelegateSet?.DynamicInvoke(selectedDescriptor, inputTextValue);
                                break;
                            }
                        }

                        var tooltip = propertyInteropInfo.Tooltip?.Get(Strings.Root);
                        if (tooltip != default && ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(tooltip.ToString(descriptorName));
                            ImGui.EndTooltip();
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        throw;
                    }
                }

                ImGui.EndTable();
                ImGui.EndGroup();
                // ImGui.EndChild();
            }
            // var propertyInfos = DescriptorType.GetObjectType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //
            // foreach (var propertyInfo in propertyInfos)
            // {
            //     if (propertyInfo.PropertyType == typeof(string))
            //     {
            //         var inputValue = propertyInfo.GetValue(selectedDescriptor) as string;
            //         var textInputFlags = ImGuiInputTextFlags.AutoSelectAll;
            //         if (propertyInfo.SetMethod == default)
            //         {
            //             textInputFlags |= ImGuiInputTextFlags.ReadOnly;
            //         }
            //         ImGui.Text(propertyInfo.Name);
            //         _ = ImGui.InputTextWithHint(
            //             $"###{propertyInfo.GetFullName()}+{selectedDescriptor.Id}",
            //             "hint",
            //             ref inputValue,
            //             255,
            //             textInputFlags
            //         );
            //         if (propertyInfo.SetMethod != default)
            //         {
            //             propertyInfo.SetValue(selectedDescriptor, inputValue);
            //         }
            //     }
            // }
        }
        else
        {
            // TODO: Multiline centering + CalcTextSize() isn't always returning the correct value
            var wrapSize = editorContentArea.X * 0.8f;
            var textSize = ImGui.CalcTextSize(Strings.Windows.Descriptor.NoDescriptorSelected, wrapSize);
            ImGui.PushTextWrapPos(wrapSize);
            ImGui.SetCursorPos((editorContentArea - textSize) / 2f);
            // ImGui.GetWindowDrawList().AddRect(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + textSize, 0xffff00ff);
            ImGui.TextWrapped(Strings.Windows.Descriptor.NoDescriptorSelected);
            ImGui.PopTextWrapPos();
        }

        ImGui.EndChild();

        return true;
    }
}
