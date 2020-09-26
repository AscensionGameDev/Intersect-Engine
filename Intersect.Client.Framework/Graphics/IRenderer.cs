using System;
using System.Collections.Generic;
using System.IO;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

using JetBrains.Annotations;

namespace Intersect.Client.Framework.Graphics
{
    public interface IRenderer
    {
        List<Stream> ScreenshotRequests { get; }

        Resolution ActiveResolution { get; }

        bool HasOverrideResolution { get; }

        Resolution OverrideResolution { get; set; }

        Resolution PreferredResolution { get; set; }

        void Init();

        /// <summary>
        ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
        /// </summary>
        /// <returns></returns>
        bool Begin();

        bool BeginScreenshot();

        /// <summary>
        ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
        /// </summary>
        void End(bool frame = true);

        void EndScreenshot();

        /// <summary>
        ///     Clears everything off the render target with a specified color.
        /// </summary>
        void Clear(Color color);

        void SetRenderTexture(IRenderTexture renderTexture);

        void SetView(FloatRect view);

        FloatRect GetView();

        IFont LoadFont(string filename);

        void DrawTexture(
            ITexture tex,
            float sx,
            float sy,
            float sw,
            float sh,
            float tx,
            float ty,
            float tw,
            float th,
            Color renderColor,
            IRenderTexture gameRenderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            IShader shader = null,
            float rotationDegrees = 0.0f,
            bool isUi = false,
            bool drawImmediate = false
        );

        int Fps { get; }

        int ScreenWidth { get; }

        int ScreenHeight { get; }

        bool DisplayModeChanged();

        IRenderTexture CreateRenderTexture(int width, int height);

        ITexture LoadTexture(TextureType textureType, string assetName);

        ITexture LoadTexture(string filename, ITexturePackFrame texturePackFrame = null);

        ITexture LoadTexture([NotNull] string assetName, [NotNull] Func<Stream> createStream);

        ITexture GetWhiteTexture();

        Pointf MeasureText(string text, IFont gameFont, float fontScale);

        void DrawString(
            string text,
            IFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos = true,
            IRenderTexture gameRenderTexture = null,
            Color borderColor = null
        );

        void DrawString(
            string text,
            IFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos,
            IRenderTexture gameRenderTexture,
            FloatRect clipRect,
            Color borderColor = null
        );

        GameTileBuffer CreateTileBuffer();

        void DrawTileBuffer(GameTileBuffer buffer);

        void Close();

        List<string> GetValidVideoModes();

        IShader LoadShader(string name);

        void RequestScreenshot(string screenshotDir = "screenshots");
    }
}
