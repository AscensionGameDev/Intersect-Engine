using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Declares the API for graphical shader assets.
    /// </summary>
    public interface IShader : IAsset
    {
        object ShaderObject { get; }

        bool Dirty { get; }

        void SetFloat(string key, float value);

        void SetInt(string key, int value);

        void SetColor(string key, Color value);

        void SetVector2(string key, Pointf value);

        void MarkClean();

        TShader GetShader<TShader>() where TShader : class;
    }
}
