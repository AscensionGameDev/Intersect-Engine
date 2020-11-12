using Intersect.Client.Framework;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Diagnostics;

namespace Intersect.Client.MonoGame.Graphics
{
    public class MonoFont : GameFont
    {
        internal new MonoGameContext GameContext => base.GameContext as MonoGameContext;

        private SpriteFont Font { get; set; }

        public MonoFont(IGameContext gameContext, AssetReference assetReference) : base(gameContext, assetReference)
        {
        }

        public override TFont AsPlatformFont<TFont>()
        {
            if (Font == null)
            {
                try
                {
                    Font = GameContext.Game.Content.Load<SpriteFont>(Reference.ResolvedPathWithoutExtension);
                }
                catch (Exception exception)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Log.Trace(exception);
                }
            }

            return (TFont) (Font as object);
        }
    }
}
