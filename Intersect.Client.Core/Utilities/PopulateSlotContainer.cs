using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game;

namespace Intersect.Client.Utilities;

public static class PopulateSlotContainer
{
    public static void Populate(ScrollControl slotContainer, List<SlotItem> items)
    {
        float containerInnerWidth = slotContainer.InnerPanel.InnerWidth;
        for (var slotIndex = 0; slotIndex < items.Count; slotIndex++)
        {
            var slot = items[slotIndex];
            var outerSize = slot.OuterBounds.Size;
            var itemsPerRow = (int)(containerInnerWidth / outerSize.X);

            var column = slotIndex % itemsPerRow;
            var row = slotIndex / itemsPerRow;

            var xPosition = column * outerSize.X + slot.Margin.Left;
            var yPosition = row * outerSize.Y + slot.Margin.Top;

            slot.SetPosition(xPosition, yPosition);
        }
    }
}