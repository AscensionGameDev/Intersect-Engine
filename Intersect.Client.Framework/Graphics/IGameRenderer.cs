using Intersect.Client.Framework.GenericClasses;
using System.Collections.Generic;
using System.IO;

namespace Intersect.Client.Framework.Graphics
{
    public interface IGameRenderer
    {
        /// <summary>
        /// The current active resolution of the client.
        /// </summary>
        Resolution ActiveResolution { get; }

        /// <summary>
        /// The current active resolution of the client as a string.
        /// </summary>
        string ResolutionAsString { get; }

        /// <summary>
        /// The current height of the client screen.
        /// </summary>
        int ScreenHeight { get; }

        /// <summary>
        /// The current width of the client screen.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// The current override resolution of the client.
        /// </summary>
        Resolution OverrideResolution { get; set; }

        /// <summary>
        /// The preferred resolution of the client.
        /// </summary>
        Resolution PreferredResolution { get; set; }

        /// <summary>
        /// The current viewport of the client.
        /// </summary>
        FloatRect CurrentView { get; set; }

        /// <summary>
        /// All currently standing request for screenshots of the client.
        /// </summary>
        List<Stream> ScreenshotRequests { get; }

        /// <summary>
        /// The current framerate at which the client is drawing frames.
        /// </summary>
        int Fps { get; }

        /// <summary>
        /// All valid video modes that the client can render at.
        /// </summary>
        List<string> ValidVideoModes { get; }
        
        /// <summary>
        /// Clear the screen.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to clear the screen with.</param>
        void Clear(Color color);

        /// <summary>
        /// Create a new empty render texture in memory.
        /// </summary>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <returns>Returns a new <see cref="GameRenderTexture"/> with the configured height and width.</returns>
        GameRenderTexture CreateRenderTexture(int width, int height);

        /// <summary>
        /// Create a new render texture in memory that's white.
        /// </summary>
        /// <returns>Returns a new white <see cref="GameRenderTexture"/>.</returns>
        GameRenderTexture CreateWhiteTexture();

        /// <summary>
        /// Draw a texture to the client display.
        /// </summary>
        /// <param name="texture">The <see cref="GameTexture"/> to render to the screen.</param>
        /// <param name="sourceX">The X position to use on the texture to draw from.</param>
        /// <param name="sourceY">The Y position to use on the texture to draw from.</param>
        /// <param name="sourceWidth">The width to use on the texture to draw from.</param>
        /// <param name="sourceHeight">The height to use on the texture to draw from.</param>
        /// <param name="targetX">The destination X position on screen.</param>
        /// <param name="targetY">The destination Y position on screen.</param>
        /// <param name="targetWidth">The destination width on screen.</param>
        /// <param name="targetHeight">The destination height on screen.</param>
        /// <param name="renderColor">The <see cref="Color"/> to render this texture as. Use <see cref="Color.White"/> to retain original colors.</param>
        /// <param name="renderTarget">Overrides this method to draw to the specified <see cref="GameRenderTexture"/> instead of the screen.</param>
        /// <param name="blendMode">The <see cref="GameBlendModes"/> to use to render this texture to the screen with.</param>
        /// <param name="shader">The <see cref="GameShader"/> to use to render this to the screen with.</param>
        /// <param name="rotationDegrees">The angle to render this texture at.</param>
        void DrawTexture(GameTexture texture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, float targetX, float targetY, float targetWidth, float targetHeight, Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.None, GameShader shader = null, float rotationDegrees = 0);

        /// <summary>
        /// Draw text to the client display.
        /// </summary>
        /// <param name="text">The text to render to the screen.</param>
        /// <param name="gameFont">The <see cref="GameFont"/> to use to render the text.</param>
        /// <param name="x">The X location on the screen to render the text to.</param>
        /// <param name="y">The Y location on the screen to render the text to.</param>
        /// <param name="fontScale">The scale of the font to render the text with.</param>
        /// <param name="fontColor">The <see cref="Color"/> to use to render the text with.</param>
        /// <param name="worldPos">Determines if this text is rendered in a world relative location. When false it will render screen relative.</param>
        /// <param name="renderTexture">Overrides this method to draw to the specified <see cref="GameRenderTexture"/> instead of the screen.</param>
        /// <param name="borderColor">The <see cref="Color"/> to use to render the border of this text with.</param>
        void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos = true, GameRenderTexture renderTexture = null, Color borderColor = null);

        /// <summary>
        /// Draw text to the client display.
        /// </summary>
        /// <param name="text">The text to render to the screen.</param>
        /// <param name="gameFont">The <see cref="GameFont"/> to use to render the text.</param>
        /// <param name="x">The X location on the screen to render the text to.</param>
        /// <param name="y">The Y location on the screen to render the text to.</param>
        /// <param name="fontScale">The scale of the font to render the text with.</param>
        /// <param name="fontColor">The <see cref="Color"/> to use to render the text with.</param>
        /// <param name="worldPos">Determines if this text is rendered in a world relative location. When false it will render screen relative.</param>
        /// <param name="renderTexture">Overrides this method to draw to the specified <see cref="GameRenderTexture"/> instead of the screen.</param>
        /// <param name="borderColor">The <see cref="Color"/> to use to render the border of this text with.</param>
        /// <param name="clipRect">The <see cref="FloatRect"/> containing locations that this text can not be rendered outside of, cutting off anything outside of it.</param>
        void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos, GameRenderTexture renderTexture, FloatRect clipRect, Color borderColor = null);
        
        /// <summary>
        /// Measures a string of text.
        /// </summary>
        /// <param name="text">The text to measure the size of.</param>
        /// <param name="gameFont">The <see cref="GameFont"/> to use to measure the text with.</param>
        /// <param name="fontScale">The scale of the font to measure the text with.</param>
        /// <returns>Returns a <see cref="Pointf"/> containing the width and height of the measured text.</returns>
        Pointf MeasureText(string text, GameFont gameFont, float fontScale);

        /// <summary>
        /// Send a request for the client to take a screenshot the next draw cycle.
        /// </summary>
        /// <param name="screenshotDir">The directory (relative to the client directory) to store the screenshot in.</param>
        void RequestScreenshot(string screenshotDir = "screenshots");
    }
}