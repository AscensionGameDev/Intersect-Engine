using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core.GameObjects.Mounts;

namespace Intersect.Client.Interface.Game.Mounts;

public partial class MountsWindow
{
    private readonly Base mParent;

    private WindowControl mWindow;

    private ListBox mMountList;

    private Button mToggleButton;

    public MountsWindow(Canvas gameCanvas)
    {
        mParent = gameCanvas;

        GenerateControls();
    }

    private void GenerateControls()
    {
        mWindow = new WindowControl(mParent, Strings.Mounts.Title, false, "MountsWindow")
        {
            IsHidden = true,
        };
        mWindow.SetSize(220, 310);
        mWindow.SetPosition(280, 180);
        mWindow.DisableResizing();

        mMountList = new ListBox(mWindow, "MountList")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
        };
        mMountList.SetBounds(8, 32, 204, 234);
        mMountList.EnableScroll(false, true);
        mMountList.RowSelected += MountList_RowSelected;

        mToggleButton = new Button(mWindow, "ToggleButton")
        {
            FontName = "sourcesansproblack",
            FontSize = 10,
        };
        mToggleButton.SetBounds(8, 272, 204, 28);
        mToggleButton.Clicked += ToggleButton_Clicked;

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show() => mWindow.Show();

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Nothing to update on a per-frame basis; the list and mounted state refresh from server packets.
        UpdateToggleButton();
    }

    public void UpdateList()
    {
        mMountList.RemoveAllRows();

        if (Globals.Me == null)
        {
            return;
        }

        foreach (var mountId in Globals.Me.UnlockedMountIds)
        {
            // Mounts are game objects: resolve the name from the lookup filled by the game data packets.
            if (MountDescriptor.Get(mountId) is not { } mount)
            {
                continue;
            }

            var row = mMountList.AddRow(mount.Name, name: null, userData: mount.Id);

            if (mount.Id == Globals.Me.MountId)
            {
                row.IsSelected = true;
            }
        }

        UpdateToggleButton();
    }

    private void UpdateToggleButton()
    {
        if (Globals.Me == null)
        {
            return;
        }

        mToggleButton.SetText(Globals.Me.IsMounted ? Strings.Mounts.Dismount : Strings.Mounts.Mount);
        mToggleButton.IsDisabled = !Globals.Me.IsMounted && Globals.Me.MountId == Guid.Empty;
    }

    private void MountList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        // ListBox raises this event without populating SelectedUserData, so read the row's UserData directly.
        if (arguments.SelectedItem?.UserData is not Guid mountId)
        {
            return;
        }

        PacketSender.SendSetActiveMount(mountId);
    }

    private void ToggleButton_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendToggleMount();
    }
}
