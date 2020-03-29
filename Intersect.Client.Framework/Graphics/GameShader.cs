using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameShader
    {

        public GameShader(string shaderName)
        {
        }

        public abstract void SetFloat(string key, float val);

        public abstract void SetInt(string key, int val);

        public abstract void SetColor(string key, Color val);

        public abstract void SetVector2(string key, Pointf val);

        public abstract bool ValuesChanged();

        public abstract void ResetChanged();

        public abstract object GetShader();

    }

}
