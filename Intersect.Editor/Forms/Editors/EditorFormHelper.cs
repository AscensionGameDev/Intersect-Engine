using System.Windows.Forms;
using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors;

internal static class EditorFormHelper
{
    // Always anchors to `reference`'s OWN parent and OWN Left/Top/Bottom, never the form's.
    // Designer-generated forms frequently nest controls inside GroupBoxes/Panels, so a button's
    // Location is relative to THAT container, not the Form - adding a sibling button with
    // Controls.Add() at the form level (instead of reference.Parent.Controls.Add) silently
    // places it in the wrong coordinate space, rendering it invisible/off-screen.
    //
    // Note: "Point"/"Size" are fully qualified below because this file sits in the "Intersect"
    // namespace tree, which also declares its own unrelated "Intersect.Point" type - an unqualified
    // "Point" here resolves to THAT type instead of System.Drawing.Point, even with a using-alias.
    private static readonly System.Drawing.Size DefaultButtonSize = new(55, 23);

    private static DarkButton CreateButton(Control reference, string text, System.Drawing.Point location, System.Drawing.Size size, EventHandler onClick)
    {
        var button = new DarkButton
        {
            Text = text,
            Location = location,
            Size = size,
        };
        button.Click += onClick;
        reference.Parent.Controls.Add(button);
        return button;
    }

    public static DarkButton AddButtonAbove(Control reference, string text, EventHandler onClick, System.Drawing.Size? size = null, int gap = 4) =>
        CreateButton(reference, text, new System.Drawing.Point(reference.Left, reference.Top - (size ?? DefaultButtonSize).Height - gap), size ?? DefaultButtonSize, onClick);

    public static DarkButton AddButtonBelow(Control reference, string text, EventHandler onClick, System.Drawing.Size? size = null, int gap = 4) =>
        CreateButton(reference, text, new System.Drawing.Point(reference.Left, reference.Bottom + gap), size ?? DefaultButtonSize, onClick);

    public static DarkButton AddTranslateEnButtonAbove(Control reference, EventHandler onClick, bool matchReferenceSize = false) =>
        AddButtonAbove(reference, "EN", onClick, matchReferenceSize ? reference.Size : null);

    public static DarkButton AddTranslateEnButtonBelow(Control reference, EventHandler onClick, bool matchReferenceSize = false) =>
        AddButtonBelow(reference, "EN", onClick, matchReferenceSize ? reference.Size : null);
}
