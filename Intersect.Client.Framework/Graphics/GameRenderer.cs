using System;
using System.Collections.Generic;
using System.IO;

using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameRenderer
    {

        public GameRenderer()
        {
            ScreenshotRequests = new List<Stream>();
        }

        public List<Stream> ScreenshotRequests { get; }

        public Resolution ActiveResolution => new Resolution(PreferredResolution, OverrideResolution);

        public bool HasOverrideResolution => OverrideResolution != Resolution.Empty;

        public Resolution OverrideResolution { get; set; }

        public Resolution PreferredResolution { get; set; }

        public abstract void Init();

        /// <summary>
        ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
        /// </summary>
        /// <returns></returns>
        public abstract bool Begin();

        public abstract bool BeginScreenshot();

        /// <summary>
        ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
        /// </summary>
        public abstract void End();

        public abstract void EndScreenshot();

        /// <summary>
        ///     Clears everything off the render target with a specified color.
        /// </summary>
        public abstract void Clear(Color color);

        public abstract void SetView(FloatRect view);

        public abstract FloatRect GetView();

        public abstract GameFont LoadFont(string filename);

        public abstract void DrawTexture(
            GameTexture tex,
            float sx,
            float sy,
            float sw,
            float sh,
            float tx,
            float ty,
            float tw,
            float th,
            Color renderColor,
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
            float rotationDegrees = 0.0f,
            bool isUi = false,
            bool drawImmediate = false
        );

        public abstract int GetFps();

        public abstract int GetScreenWidth();

        public abstract int GetScreenHeight();

        public abstract string GetResolutionString();

        public abstract bool DisplayModeChanged();

        public abstract GameRenderTexture CreateRenderTexture(int width, int height);

        public abstract GameTexture LoadTexture(string filename, string realFilename);

        public abstract GameTexture LoadTexture(
            string assetName,
            Func<Stream> createStream
        );

        public abstract GameTexture GetWhiteTexture();

        public abstract Pointf MeasureText(string text, GameFont gameFont, float fontScale);

        public abstract void DrawString(
            string text,
            GameFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos = true,
            GameRenderTexture renderTexture = null,
            Color borderColor = null
        );

        public abstract void DrawString(
            string text,
            GameFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos,
            GameRenderTexture renderTexture,
            FloatRect clipRect,
            Color borderColor = null
        );

        //Buffers
        public abstract GameTileBuffer CreateTileBuffer();

        public abstract void DrawTileBuffer(GameTileBuffer buffer);

        public abstract void Close();

        public abstract List<string> GetValidVideoModes();

        public abstract GameShader LoadShader(string shaderName);

        public void RequestScreenshot(string screenshotDir = "screenshots")
        {
            if (!Directory.Exists(screenshotDir))
            {
                Directory.CreateDirectory(screenshotDir ?? "");
            }

            var screenshotNumber = 0;
            string screenshotFile;
            do
            {
                screenshotFile = Path.Combine(
                    screenshotDir ?? "", $"{DateTime.Now:yyyyMMdd-HHmmssfff}{screenshotNumber}.png"
                );

                ++screenshotNumber;
            } while (File.Exists(screenshotFile) && screenshotNumber < 4);

            ScreenshotRequests.Add(File.OpenWrite(screenshotFile));
        }

    }

}
