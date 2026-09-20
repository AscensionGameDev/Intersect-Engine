using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core.GameObjects.Titles;

namespace Intersect.Client.Interface.Game.Titles;


public partial class TitlesWindow
{
    private readonly Base mParent;

    private WindowControl mWindow;

    private ListBox mTitleList;

    public TitlesWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Titles.Title, false, "TitlesWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(220, 280);
        mWindow.SetPosition(280, 180);
        mWindow.DisableResizing();

        mTitleList = new ListBox(mWindow, "TitleList")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
        };
        mTitleList.SetBounds(8, 32, 204, 240);
        mTitleList.EnableScroll(false, true);
        mTitleList.RowSelected += TitleList_RowSelected;

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show() => mWindow.Show();

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Nothing to update on a per-frame basis; the list refreshes when the server pushes new data.
    }

    public void UpdateList()
    {
        mTitleList.RemoveAllRows();

        var noneRow = mTitleList.AddRow(Strings.Titles.RemoveTitle, name: null, userData: Guid.Empty);
        if (Globals.Me?.EquippedTitleId == Guid.Empty)
        {
            noneRow.IsSelected = true;
        }

        if (Globals.Me == null)
        {
            return;
        }

        foreach (var titleId in Globals.Me.UnlockedTitleIds)
        {
            // Titles are game objects: resolve name/color from the lookup filled by the game data packets.
            if (TitleDescriptor.Get(titleId) is not { } title)
            {
                continue;
            }

            var row = mTitleList.AddRow(title.Name, name: null, userData: title.Id);
            row.SetTextColor(title.Color);

            if (title.Id == Globals.Me.EquippedTitleId)
            {
                row.IsSelected = true;
            }
        }
    }

    private void TitleList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        // ListBox raises this event without populating SelectedUserData, so read the row's UserData directly.
        if (arguments.SelectedItem?.UserData is not Guid titleId)
        {
            return;
        }

        PacketSender.SendEquipTitle(titleId);
    }
}
