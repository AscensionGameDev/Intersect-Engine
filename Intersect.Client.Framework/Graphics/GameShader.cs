using Intersect.Client.Framework.GenericClasses;

using System;

namespace Intersect.Client.Framework.Graphics
{
    public abstract class GameShader : IShader
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Dirty { get; private set; }

        /// <inheritdoc />
        public object ShaderObject { get; protected set; }

        protected GameShader(string name, object shaderObject)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            ShaderObject = shaderObject ?? throw new ArgumentNullException(nameof(shaderObject));
        }

        /// <inheritdoc />
        public virtual void SetFloat(string key, float value) => Dirty = true;

        /// <inheritdoc />
        public virtual void SetInt(string key, int value) => Dirty = true;

        /// <inheritdoc />
        public virtual void SetColor(string key, Color value) => Dirty = true;

        /// <inheritdoc />
        public virtual void SetVector2(string key, Pointf value) => Dirty = true;

        /// <inheritdoc />
        public virtual void MarkClean() => Dirty = false;

        /// <inheritdoc />
        public TShader GetShader<TShader>() where TShader : class => ShaderObject as TShader;
    }

    public abstract class GameShader<TShader> : GameShader where TShader : class {
        public TShader Shader { get; private set; }

        protected GameShader(string name, TShader shader) : base(name, shader)
        {
            Shader = shader;
        }
    }
}
