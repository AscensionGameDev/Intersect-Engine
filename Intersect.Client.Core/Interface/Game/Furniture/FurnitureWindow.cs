using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Furniture;

namespace Intersect.Client.Interface.Game.Furniture;

/// <summary>
/// Lists the furniture catalog and lets a guild officer "arm" a piece to place in the guild hall.
/// Opened via the <c>/decorer</c> chat command (<see cref="Chat.Chatbox"/>) - there's no menu button for it,
/// this is a rarely-used tool rather than something that deserves permanent screen real estate.
/// </summary>
public partial class FurnitureWindow
{
    private readonly Base mParent;

    private WindowControl mWindow;

    private ListBox mFurnitureList;

    private Label mHintLabel;

    public FurnitureWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Furniture.Title, false, "FurnitureWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(220, 310);
        mWindow.SetPosition(280, 180);
        mWindow.DisableResizing();

        mFurnitureList = new ListBox(mWindow, "FurnitureList")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
        };
        mFurnitureList.SetBounds(8, 32, 204, 234);
        mFurnitureList.EnableScroll(false, true);
        mFurnitureList.RowSelected += FurnitureList_RowSelected;

        mHintLabel = new Label(mWindow, "HintLabel")
        {
            FontName = "sourcesansproblack",
            FontSize = 8,
        };
        mHintLabel.SetBounds(8, 272, 204, 28);
        mHintLabel.SetText(Strings.Furniture.PlacementHint);

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show()
    {
        UpdateList();
        mWindow.Show();
    }

    public void Hide()
    {
        FurniturePlacementState.Clear();
        mWindow.Hide();
    }

    public void Update()
    {
        mHintLabel.SetText(
            FurniturePlacementState.IsArmed ? Strings.Furniture.PlacementArmed : Strings.Furniture.PlacementHint
        );
    }

    private void UpdateList()
    {
        mFurnitureList.RemoveAllRows();

        foreach (var descriptor in FurnitureDescriptor.Lookup.Values)
        {
            if (descriptor is not FurnitureDescriptor furniture)
            {
                continue;
            }

            mFurnitureList.AddRow(furniture.Name, name: null, userData: furniture.Id);
        }
    }

    private void FurnitureList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        if (arguments.SelectedItem?.UserData is not Guid furnitureId)
        {
            return;
        }

        FurniturePlacementState.Arm(furnitureId);
    }
}
