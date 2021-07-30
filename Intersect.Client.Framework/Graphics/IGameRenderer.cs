using Intersect.Client.Framework.GenericClasses;
using System.Collections.Generic;
using System.IO;

namespace Intersect.Client.Framework.Graphics
{
    public interface IGameRenderer
    {
        Resolution ActiveResolution { get; }
        Resolution OverrideResolution { get; set; }
        Resolution PreferredResolution { get; set; }
        List<Stream> ScreenshotRequests { get; }

        void Clear(Color color);
        GameRenderTexture CreateRenderTexture(int width, int height);
        void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos = true, GameRenderTexture renderTexture = null, Color borderColor = null);
        void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos, GameRenderTexture renderTexture, FloatRect clipRect, Color borderColor = null);
        void DrawTexture(GameTexture tex, float sx, float sy, float sw, float sh, float tx, float ty, float tw, float th, Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.None, GameShader shader = null, float rotationDegrees = 0, bool isUi = false, bool drawImmediate = false);
        int Fps { get; }
        string ResolutionAsString { get; }
        int ScreenHeight { get; }
        int ScreenWidth { get; }
        List<string> ValidVideoModes { get; }
        FloatRect CurrentView { get; set; }
        GameTexture GetWhiteTexture();
        Pointf MeasureText(string text, GameFont gameFont, float fontScale);
        void RequestScreenshot(string screenshotDir = "screenshots");
    }
}