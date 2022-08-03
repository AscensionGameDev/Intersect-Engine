using System.Numerics;
using System.Reflection;

using ImGuiNET;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Editor.MonoGame.Content;
using Intersect.Localization;
using Intersect.Platform;
using Intersect.Properties;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class AboutWindow : Window
{
    private const string TabIdAuthors = "about_tabbar_item_authors";
    private const string TabIdLicenses = "about_tabbar_item_licenses";
    private const string LicenseComponentIdIntersectEditor = "about_licenses_component_intersect_editor";

    private static readonly AuthorsMd _authorsMd = Markdown.ParseAuthorsMd();

    private ImGuiTexture? _textureGitHubIcon;
    private ImGuiTexture? _textureLogo;

    private readonly ReactiveString _labelTabAuthors;
    private readonly ReactiveString _labelTabLicenses;
    private readonly ReactiveString _labelLicenseComponentIntersectEditor;

    private bool _intersectEditorSelected;

    public AboutWindow() : base()
    {
        _labelTabAuthors = CreateLabelWithId(Strings.Windows.About.LabelAuthors, TabIdAuthors);
        _labelTabLicenses = CreateLabelWithId(Strings.Windows.About.LabelLicenses, TabIdLicenses);
        _labelLicenseComponentIntersectEditor = CreateLabelWithId(
            Strings.Windows.About.LabelLicenseComponentIntersectEditor,
            LicenseComponentIdIntersectEditor
        );

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize;
        Title = Strings.Windows.About.Title;
    }

    private static void PrintNColumnEntries(IEnumerable<AuthorsMd.Entry> entries, int columns = 1)
    {
        if (columns < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(columns));
        }

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, 4 * Vector2.One);

        var column = 0;
        foreach (var entry in entries)
        {
            if (column == 0)
            {
                ImGui.TableNextRow();
            }

            _ = ImGui.TableSetColumnIndex(column);

            var text = entry.ToString();

            var isHovered = false;

            if (!string.IsNullOrWhiteSpace(entry.Url))
            {
                var textSize = ImGui.CalcTextSize(text);
                var cursorPos = ImGui.GetCursorPos();
                var windowPos = ImGui.GetWindowPos();
                var scrollOffset = -ImGui.GetScrollY() * Vector2.UnitY;
                var offset = windowPos + scrollOffset + cursorPos;

                var tint = 0xffffffff;
                if (isHovered = ImGui.IsMouseHoveringRect(offset, offset + textSize))
                {
                    tint = 0xffe89654;
                    ImGui.PushStyleColor(ImGuiCol.Text, tint);
                }

                ImGui.GetWindowDrawList().AddLine(
                    offset - 2 * Vector2.UnitY + textSize * Vector2.UnitY,
                    offset - 2 * Vector2.UnitY + textSize,
                    tint & 0x00ffffff | 0x7f000000,
                    0.25f
                );
            }

            ImGui.Text(text);

            if (isHovered)
            {
                ImGui.PopStyleColor();

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    WebBrowser.OpenInDefaultBrowser(entry.Url);
                }
            }

            column = (column + 1) % columns;
        }

        ImGui.PopStyleVar(1);
    }

    protected unsafe override bool LayoutBegin(FrameTime frameTime)
    {
        _textureGitHubIcon ??= ContentManager?.Find<ImGuiTexture>(ContentTypes.Interface, "github_white_24.png");
        _textureLogo ??= ContentManager?.Find<ImGuiTexture>(ContentTypes.Interface, "logo-with-text_small.png");

        var windowSize = new Vector2(640, 480);

        if (_textureLogo != default)
        {
            windowSize = new(Math.Max(windowSize.X, 32 + _textureLogo.Width), windowSize.Y);
        }

        ImGui.SetNextWindowSize(windowSize);

        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        var windowPos = ImGui.GetWindowPos();

        if (_textureLogo != default)
        {
            ImGui.SetCursorPosX((windowSize.X - _textureLogo.Width) / 2);
            ImGui.Image(_textureLogo, _textureLogo.Size);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8);
        }

        ImGui.SetCursorPosX(16);

        var gitHubLinkText = Strings.Windows.About.JoinUsOnGitHub;
        var gitHubLinkTextSize = ImGui.CalcTextSize(gitHubLinkText);
        var gitHubLinkSize = new Vector2(gitHubLinkTextSize.X, gitHubLinkTextSize.Y);
        var gitHubIconSize = _textureGitHubIcon?.Size ?? default;

        if (_textureGitHubIcon != default)
        {
            gitHubLinkSize.X += gitHubIconSize.X + gitHubLinkSize.Y;
            gitHubLinkSize.Y = Math.Max(gitHubLinkSize.Y, gitHubIconSize.Y);
        }

        var imGuiCursorPos = windowPos + ImGui.GetCursorPos();
        var isHoveringLink = ImGui.IsMouseHoveringRect(imGuiCursorPos, imGuiCursorPos + gitHubLinkSize);

        var tintColor = Vector4.One;
        if (isHoveringLink)
        {
            var textColor = *ImGui.GetStyleColorVec4(ImGuiCol.Text);
            tintColor = new(0.25f, 0.8f, 1, 1);
            ImGui.PushStyleColor(ImGuiCol.Text, textColor * tintColor);
        }

        if (_textureGitHubIcon != default)
        {
            ImGui.Image(_textureGitHubIcon, gitHubIconSize, Vector2.Zero, Vector2.One, tintColor);
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (gitHubIconSize.Y - gitHubLinkTextSize.Y) / 2);
            imGuiCursorPos = windowPos + ImGui.GetCursorPos();
        }

        ImGui.Text(gitHubLinkText);

        if (isHoveringLink)
        {
            ImGui.GetWindowDrawList().AddLine(imGuiCursorPos + Vector2.UnitY * gitHubLinkTextSize.Y, imGuiCursorPos + gitHubLinkTextSize, 0xffffcf7f, 1);
            ImGui.PopStyleColor();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                WebBrowser.OpenInDefaultBrowser("https://github.com/AscensionGameDev/Intersect-Engine");
            }
        }

        ImGui.SameLine();

        var versionText = Strings.Application.Version.ToString(Assembly.GetExecutingAssembly().GetName().Version);
#if DEBUG
        versionText = Strings.Application.VersionSuffixDebug.ToString(versionText);
#endif
        var versionTextSize = ImGui.CalcTextSize(versionText);
        ImGui.SetCursorPosX(windowSize.X - versionTextSize.X - 16);
        ImGui.Text(versionText);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.BeginChild("about_tabs", default, true, ImGuiWindowFlags.AlwaysAutoResize);

        if (ImGui.BeginTabBar("about_tab_bar"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4));

            if (ImGui.BeginTabItem(_labelTabAuthors))
            {
                _ = ImGui.BeginChild($"{TabIdAuthors}_child", default, true, ImGuiWindowFlags.AlwaysAutoResize);

                var tableFlags = ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInner | ImGuiTableFlags.SizingStretchSame;

                if (ImGui.BeginTable($"{TabIdAuthors}_table_authors", 1, tableFlags))
                {
                    ImGui.TableSetupColumn(Strings.Windows.About.LabelAuthors);
                    ImGui.TableHeadersRow();

                    PrintNColumnEntries(_authorsMd.Authors);

                    ImGui.EndTable();
                }

                if (ImGui.BeginTable($"{TabIdAuthors}_table_maintainers", 1, tableFlags))
                {
                    ImGui.TableSetupColumn(Strings.Windows.About.LabelMaintainers);
                    ImGui.TableHeadersRow();

                    PrintNColumnEntries(_authorsMd.Maintainers);

                    ImGui.EndTable();
                }

                if (ImGui.BeginTable($"{TabIdAuthors}_table_developers", 1, tableFlags))
                {
                    ImGui.TableSetupColumn(Strings.Windows.About.LabelDevelopers);
                    ImGui.TableHeadersRow();

                    PrintNColumnEntries(_authorsMd.Developers);

                    ImGui.EndTable();
                }

                if (ImGui.BeginTable($"{TabIdAuthors}_table_contributors", 2, tableFlags))
                {
                    ImGui.TableSetupColumn($"{TabIdAuthors}_table_contributors_col0", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.NoHeaderLabel, 0.5f);
                    ImGui.TableSetupColumn($"{TabIdAuthors}_table_contributors_col1", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.NoHeaderLabel, 0.5f);

                    ImGui.TableHeadersRow();

                    _ = ImGui.TableSetColumnIndex(0);
                    _ = ImGui.Selectable(Strings.Windows.About.LabelContributors, false, ImGuiSelectableFlags.SpanAllColumns);

                    PrintNColumnEntries(_authorsMd.Contributors, 2);

                    ImGui.EndTable();
                }

                ImGui.EndChild();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(_labelTabLicenses))
            {
                _ = ImGui.BeginChild($"{TabIdLicenses}_child", default, true, ImGuiWindowFlags.AlwaysAutoResize);

                _ = ImGui.BeginChild("about_licenses_selector", new Vector2(200, ImGui.GetWindowSize().Y - 8), true);

                ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, ImGui.GetStyle().IndentSpacing / 2);
                if (ImGui.TreeNode("about_licenses_intersect", Strings.Windows.About.LabelLicenseGroupIntersect))
                {
                    ImGui.TreePush(LicenseComponentIdIntersectEditor);
                    ImGui.Selectable(_labelLicenseComponentIntersectEditor, ref _intersectEditorSelected, ImGuiSelectableFlags.DontClosePopups);
                    ImGui.TreePop();
                }
                ImGui.EndChild();
                ImGui.PopStyleVar(1);

                ImGui.SameLine();

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 4);

                _ = ImGui.BeginChild("about_licenses_display", default, true, ImGuiWindowFlags.AlwaysAutoResize);
                
                if (_intersectEditorSelected)
                {
                    ImGui.TextWrapped(Strings.Licensing.GplV3LongNotice.ToString(Strings.Licensing.CopyrightYear, Strings.Licensing.CopyrightAuthor));
                }

                ImGui.EndChild();

                ImGui.EndChild();

                ImGui.EndTabItem();
            }

            ImGui.PopStyleVar(2);
            ImGui.EndTabBar();
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(1);

        return true;
    }

    protected override void StyleBegin(FrameTime frameTime)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(16));
    }

    protected override void StyleEnd(FrameTime frameTime)
    {
        ImGui.PopStyleVar(1);
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        base.LayoutEnd(frameTime);
    }
}
