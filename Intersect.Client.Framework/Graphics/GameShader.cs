using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

using System;

namespace Intersect.Client.Framework.Graphics
{
    public abstract class GameShader : IShader
    {
        /// <inheritdoc />
        public string Name => Reference.Name;

        /// <inheritdoc />
        public AssetReference Reference { get; }

        /// <inheritdoc />
        public bool Dirty { get; private set; }

        /// <inheritdoc />
        public object ShaderObject { get; protected set; }

        protected GameShader(AssetReference assetReference, object shaderObject)
        {
            Reference = assetReference;
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

    public abstract class GameShader<TShader> : GameShader where TShader : class
    {
        public TShader Shader { get; private set; }

        protected GameShader(AssetReference assetReference, TShader shader) : base(assetReference, shader)
        {
            Shader = shader;
        }
    }
}
