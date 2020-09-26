using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.MonoGame.Extensions;

using Microsoft.Xna.Framework.Graphics;

using System;

namespace Intersect.Client.MonoGame.Graphics
{
    public class MonoShader : GameShader<Effect>
    {
        public MonoShader(string name, Effect effect) : base(name, effect)
        {
        }

        public override void SetFloat(string key, float value)
        {
            base.SetFloat(key, value);
            Shader.Parameters[key].SetValue(value);
        }

        public override void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            Shader.Parameters[key].SetValue(value);
        }

        public override void SetColor(string key, Color value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            base.SetColor(key, value);
            Shader.Parameters[key].SetValue(value.AsVector4());
        }

        public override void SetVector2(string key, Pointf value)
        {
            base.SetVector2(key, value);
            Shader.Parameters[key].SetValue(value.AsVector2());
        }
    }
}
