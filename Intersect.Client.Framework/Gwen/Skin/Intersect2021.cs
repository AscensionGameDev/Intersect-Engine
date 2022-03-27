using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Skin.Texturing;

namespace Intersect.Client.Framework.Gwen.Skin
{

    /// <summary>
    ///     Base textured skin.
    /// </summary>
    public class Intersect2021 : TexturedBase
    {

        private readonly GameTexture mTexture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="textureName">Name of the skin texture map.</param>
        public Intersect2021(Renderer.Base renderer, GameTexture texture) : base(renderer, texture)
        {
            mTexture = texture;

            InitializeColors();
            InitializeTextures();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #region Initialization

        protected override void InitializeTextures()
        {
            base.InitializeTextures();

            mTextures.Window.Normal = new Bordered(mTexture, 0, 0, 16, 40, new Margin(4, 32, 4, 4));
            mTextures.Window.Inactive = new Bordered(mTexture, 16, 0, 16, 40, new Margin(4, 32, 4, 4));

            mTextures.Window.Close = new Single(mTexture, 60, 0, 24, 24);
            mTextures.Window.CloseDown = new Single(mTexture, 60, 24, 24, 24);
            mTextures.Window.CloseDisabled = new Single(mTexture, 60, 48, 24, 24);
            mTextures.Window.CloseHover = new Single(mTexture, 60, 72, 24, 24);

            mTextures.Panel.Normal = new Bordered(mTexture, 32, 0, 16, 16, Margin.Four);
            mTextures.Panel.Highlight = new Bordered(mTexture, 32, 16, 16, 16, Margin.Four);
            mTextures.Panel.Bright = new Bordered(mTexture, 32, 32, 16, 16, Margin.Four);
            mTextures.Panel.Dark = new Bordered(mTexture, 32, 48, 16, 16, Margin.Four);

            mTextures.Input.Button.Normal = new Bordered(mTexture, 48, 0, 12, 12, Margin.Four);
            mTextures.Input.Button.Disabled = new Bordered(mTexture, 48, 24, 12, 12, Margin.Four);
            mTextures.Input.Button.Hovered = new Bordered(mTexture, 48, 12, 12, 12, Margin.Four);
            mTextures.Input.Button.Pressed = new Bordered(mTexture, 48, 36, 12, 12, Margin.Four);
        }

        #endregion

    }

}
