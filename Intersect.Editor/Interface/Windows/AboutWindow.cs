using System.Numerics;
using System.Reflection;

using ImGuiNET;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Editor.MonoGame.Content;
using Intersect.Platform;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class AboutWindow : Window
{
    private ImGuiTexture? _textureGitHubIcon;
    private ImGuiTexture? _textureLogo;

    public AboutWindow() : base()
    {
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize;
        Title = Strings.Windows.About.Title;
    }

    protected unsafe override bool LayoutBegin(FrameTime frameTime)
    {
        _textureGitHubIcon ??= ContentManager?.Find<ImGuiTexture>(ContentTypes.Interface, "github-mark-light-32px.png");
        _textureLogo ??= ContentManager?.Find<ImGuiTexture>(ContentTypes.Interface, "logo-with-text_small.png");

        var windowSize = new Vector2(640, 480);

        if (_textureLogo != default)
        {
            windowSize = new(Math.Max(windowSize.X, 64 + _textureLogo.Width), windowSize.Y);
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
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 16);
        }

        var versionText = Strings.Application.Version.ToString(Assembly.GetExecutingAssembly().GetName().Version);
        var versionTextSize = ImGui.CalcTextSize(versionText);

        ImGui.SetCursorPosX((windowSize.X - versionTextSize.X) / 2);
        ImGui.Text(versionText);
        ImGui.SameLine();
        ImGui.SetCursorPosX(32);

        var gitHubLinkText = "Join us on GitHub!";
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
        ImGui.NewLine();

        if (isHoveringLink)
        {
            ImGui.GetWindowDrawList().AddLine(imGuiCursorPos + Vector2.UnitY * gitHubLinkTextSize.Y, imGuiCursorPos + gitHubLinkTextSize, 0xffffcf7f, 1);
            ImGui.PopStyleColor();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                WebBrowser.OpenInDefaultBrowser("https://github.com/AscensionGameDev/Intersect-Engine");
            }
        }

        ImGui.TextWrapped(Strings.Licensing.GplV3LongNotice.ToString(Strings.Licensing.CopyrightYear, Strings.Licensing.CopyrightAuthor));

        return true;
    }

    protected override void StyleBegin(FrameTime frameTime)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(32));
    }

    protected override void StyleEnd(FrameTime frameTime)
    {
        ImGui.PopStyleVar(1);
    }
}
