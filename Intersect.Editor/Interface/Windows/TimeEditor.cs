using System.Numerics;
using ImGuiNET;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class TimeEditor : Window
{
    static int _selectedInterval;

    static int _selectedListItem;

    private bool mSelectedListItemChanged;

    private readonly string[] mIntervals =
    {
        Strings.Windows.TimeEditor.Interval24H, Strings.Windows.TimeEditor.Interval12H,
        Strings.Windows.TimeEditor.Interval8H, Strings.Windows.TimeEditor.Interval6H,
        Strings.Windows.TimeEditor.Interval4H, Strings.Windows.TimeEditor.Interval3H,
        Strings.Windows.TimeEditor.Interval2H, Strings.Windows.TimeEditor.Interval1H,
        Strings.Windows.TimeEditor.Interval45M, Strings.Windows.TimeEditor.Interval30M,
        Strings.Windows.TimeEditor.Interval15M, Strings.Windows.TimeEditor.Interval10M
    };

    private List<string> mTimes;

    private TimeBase mYTime;

    private TimeBase mBackupTime;

    private bool mSyncTime;

    private string mTimeRate;

    private Color mColor;

    private Vector4 mVect4Color = new(255, 255, 255, 255);

    public TimeEditor(TimeBase time)
    {
        Flags = ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize;
        Title = Strings.Windows.TimeEditor.Title;

        // Create a backup in case we want to revert.
        mYTime = time;
        mYTime.LoadFromJson(time.GetInstanceJson());
        mBackupTime = new TimeBase();
        mBackupTime.LoadFromJson(time.GetInstanceJson());

        // Time Sync and Rate.
        mSyncTime = mYTime.SyncTime;
        mTimeRate = mYTime.Rate.ToString();

        // Color
        _selectedListItem = -1;
        mSelectedListItemChanged = false;

        // Populate the list with an empty list... and then update it.
        mTimes = new List<string>();
        _selectedInterval = TimeBase.GetIntervalIndex(mYTime.RangeInterval);
        UpdateList(mYTime.RangeInterval);
    }

    protected unsafe override bool LayoutBegin(FrameTime frameTime)
    {
        var windowSize = new Vector2(560, 544);
        ImGui.SetNextWindowSize(windowSize);
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }
        //
        //
        //
        // ListBox
        ImGui.BeginChild("###times_of_day", new Vector2(226, 487), true);
        ImGui.BulletText(Strings.Windows.TimeEditor.Times);
        ImGui.NewLine();
        ImGui.SetNextItemWidth(210);
        ImGui.ListBox(string.Empty, ref _selectedListItem, mTimes.ToArray(), mTimes.Count, 24);
        if (ImGui.IsItemEdited())
        {
            mSelectedListItemChanged = true;
            //Core.Graphics.OverlayColor = mYTime.DaylightHues[_selectedListItem]; ??
        }
        ImGui.Text("[Debug] Selected Index: " + _selectedListItem); // Just for debugging index ; delete later !
        ImGui.EndChild();
        ImGui.SameLine();
        //
        //
        //
        // Settings
        ImGui.BeginChild("###time_settings", new Vector2(314, 210), true);
        ImGui.BulletText(Strings.Windows.TimeEditor.Settings);
        ImGui.NewLine();
        ImGui.Text(Strings.Windows.TimeEditor.Intervals);
        ImGui.SameLine();
        //
        //
        //
        // ComboBox
        ImGui.SetNextItemWidth(100);
        ImGui.Combo("###time_intervals", ref _selectedInterval, mIntervals,
            mIntervals.Length);
        if (ImGui.IsItemEdited())
        {
            mYTime.RangeInterval = TimeBase.GetTimeInterval(_selectedInterval);
            UpdateList(mYTime.RangeInterval);
            mYTime.ResetColors();
            _selectedListItem = -1;
            mSelectedListItemChanged = false;
        }
        ImGui.NewLine();
        //
        //
        //
        // Checkbox
        ImGui.Checkbox(Strings.Windows.TimeEditor.Sync, ref mSyncTime);
        //
        //
        //
        // InputText
        ImGui.Text(Strings.Windows.TimeEditor.Rate);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(30);
        if (!mSyncTime)
        {
            ImGui.InputText("###time_rate", ref mTimeRate, 4);
        }
        else // Disable the time rate input box if mSyncTime is true.
        {
            ImGui.BeginDisabled(true);
            ImGui.InputText("###time_rate", ref mTimeRate, 4);
            ImGui.EndDisabled();
        }
        ImGui.SameLine();
        ImGui.Text(Strings.Windows.TimeEditor.RateSuffix);
        ImGui.NewLine();
        ImGui.Text(Strings.Windows.TimeEditor.RateDesc);
        ImGui.EndChild();
        //
        //
        //
        // Time of Day Overlay Color Picker.
        if (_selectedListItem > -1)
        {
            if (mSelectedListItemChanged)
            {
                mColor = mYTime.DaylightHues[_selectedListItem];
                mVect4Color = new Vector4(mColor.R, mColor.G, mColor.B, mColor.A);
                mSelectedListItemChanged = false;
            }
            ImGui.SetCursorPos(new Vector2(240, 242));
            ImGui.BeginChild("###overlay_color", new Vector2(314, 270), true);
            ImGui.BulletText(Strings.Windows.TimeEditor.ColorPanelDesc);
            ImGui.SetNextItemWidth(160);
            ImGui.SetCursorPos(new Vector2(55));
            ImGui.ColorPicker4("###overlay_color_picker", ref mVect4Color,
                ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.InputRGB |
                ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreview);
            ImGui.EndChild();
        }
        //
        //
        //
        // Save & Cancel buttons
        ImGui.SetCursorPos(new Vector2(460, 516));
        if (ImGui.Button(Strings.Windows.TimeEditor.Save))
        {
            //PacketSender.SendSaveTime(mYTime.GetInstanceJson()); save here?
            this.IsOpen = false;
        }
        ImGui.SameLine();
        if (ImGui.Button(Strings.Windows.TimeEditor.Cancel))
        {
            mYTime.LoadFromJson(mBackupTime.GetInstanceJson());
            this.IsOpen = false;
        }
        //
        //
        return true;
    }

    protected override void StyleBegin(FrameTime frameTime)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6));
    }

    protected override void StyleEnd(FrameTime frameTime)
    {
        ImGui.PopStyleVar(1);
    }

    private void UpdateList(int duration)
    {
        mTimes.Clear();
        var time = new DateTime(2000, 1, 1, 0, 0, 0);
        for (var i = 0; i < 1440; i += duration)
        {
            var addRange = time.ToString("h:mm:ss tt") + " " + Strings.Windows.TimeEditor.To + " ";
            time = time.AddMinutes(duration);
            addRange += time.ToString("h:mm:ss tt");
            mTimes.Add(addRange);
        }
    }
}
