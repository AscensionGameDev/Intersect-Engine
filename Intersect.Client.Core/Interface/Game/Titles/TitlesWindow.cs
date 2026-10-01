using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core.GameObjects.Titles;

namespace Intersect.Client.Interface.Game.Titles;

/// <summary>
/// Lists the titles the player has unlocked, each in its own colour. Clicking one equips it; the first row removes
/// the current title. The equipped title is marked and repeated at the top of the window.
/// </summary>
public partial class TitlesWindow
{
    private const string FontName = "sourcesansproblack";

    private const int WindowWidth = 340;

    private const int WindowHeight = 420;

    private static readonly Color DimColor = new(255, 150, 150, 150);

    private static readonly Color HeaderColor = new(255, 255, 220, 120);

    private readonly Base mParent;

    private WindowControl mWindow;

    private Label mCurrentLabel;

    private Label mHintLabel;

    private ScrollControl mScroll;

    private readonly List<Button> mRows = [];

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
        mWindow.SetSize(WindowWidth, WindowHeight);
        mWindow.SetPosition(280, 110);
        mWindow.DisableResizing();

        mCurrentLabel = new Label(mWindow, "TitlesCurrent")
        {
            FontName = FontName,
            FontSize = 12,
        };

        mHintLabel = new Label(mWindow, "TitlesHint")
        {
            FontName = FontName,
            FontSize = 9,
        };

        mScroll = new ScrollControl(mWindow, "TitlesScroll");
        mScroll.EnableScroll(false, true);
        mScroll.SizeChanged += (_, _) => LayoutRows();

        mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // The window skin (window_blue_titleswindow.png) is generated at 340x420; docking keeps everything inside the
        // frame (its borders are about 18 px on the sides and 22 px at the bottom).
        mWindow.SetSize(WindowWidth, WindowHeight);

        mCurrentLabel.Dock = Pos.Top;
        mCurrentLabel.Height = 24;
        mCurrentLabel.Margin = new Margin(18, 8, 22, 2);

        mHintLabel.Dock = Pos.Bottom;
        mHintLabel.Height = 18;
        mHintLabel.Margin = new Margin(18, 2, 22, 22);

        mScroll.Dock = Pos.Fill;
        mScroll.Margin = new Margin(18, 4, 22, 2);
    }

    public bool IsVisible => !mWindow.IsHidden;

    public void Show()
    {
        mWindow.Show();
        LayoutRows();
    }

    public void Hide() => mWindow.Hide();

    public void Update()
    {
        // Nothing to update on a per-frame basis; the list refreshes when the server pushes new data.
    }

    public void UpdateList()
    {
        foreach (var row in mRows)
        {
            mScroll.RemoveChild(row, true);
        }

        mRows.Clear();

        if (Globals.Me == null)
        {
            return;
        }

        var equippedId = Globals.Me.EquippedTitleId;

        // First row: take the current title off.
        AddRow(Strings.Titles.RemoveTitle, Guid.Empty, DimColor, equippedId == Guid.Empty);

        var titles = Globals.Me.UnlockedTitleIds
            .Select(id => TitleDescriptor.Get(id))
            .Where(title => title != null)
            .OrderBy(title => title!.Name)
            .ToList();

        foreach (var title in titles)
        {
            AddRow(title!.Name, title.Id, title.Color, title.Id == equippedId);
        }

        if (equippedId != Guid.Empty && TitleDescriptor.Get(equippedId) is { } equipped)
        {
            mCurrentLabel.SetText(Strings.Titles.Current.ToString(equipped.Name));
            mCurrentLabel.SetTextColor(equipped.Color, ComponentState.Normal);
        }
        else
        {
            mCurrentLabel.SetText(Strings.Titles.Current.ToString(Strings.Titles.NoneEquipped.ToString()));
            mCurrentLabel.SetTextColor(HeaderColor, ComponentState.Normal);
        }

        mHintLabel.SetText(
            titles.Count > 0
                ? Strings.Titles.Count.ToString(titles.Count)
                : Strings.Titles.None.ToString()
        );
        mHintLabel.SetTextColor(DimColor, ComponentState.Normal);

        LayoutRows();
    }

    private void AddRow(string text, Guid titleId, Color color, bool equipped)
    {
        var row = new Button(mScroll, $"TitleRow{titleId}")
        {
            FontName = FontName,
            FontSize = equipped ? 12 : 10,
            UserData = titleId,
        };
        row.SetText(equipped ? $"> {text}" : text);
        row.SetTextColor(color, ComponentState.Normal);
        row.SetTextColor(color, ComponentState.Hovered);
        row.SetTextColor(color, ComponentState.Active);
        row.SetTextColor(color, ComponentState.Disabled);
        row.Clicked += (_, _) => PacketSender.SendEquipTitle(titleId);
        mRows.Add(row);
    }

    /// <summary>One full-width row per title, stacked from the top.</summary>
    private void LayoutRows()
    {
        var width = mScroll.Width - (mScroll.VerticalScrollBar?.Width ?? 0) - 2;
        if (width <= 0)
        {
            return;
        }

        const int rowHeight = 30;
        const int gap = 4;
        for (var index = 0; index < mRows.Count; index++)
        {
            mRows[index].SetBounds(0, index * (rowHeight + gap), width, rowHeight);
        }
    }
}
