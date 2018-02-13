using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoShader : GameShader
    {
        private Effect mShader;
        private bool mValuesChanged = false;

        public MonoShader(string shaderName, ContentManager content) : base(shaderName)
        {
            mShader = content.Load<Effect>(GameContentManager.RemoveExtension(shaderName));
        }

        public override void SetFloat(string key, float val)
        {
            mShader.Parameters[key].SetValue(val);
            mValuesChanged = true;
        }

        public override void SetInt(string key, int val)
        {
            //throw new NotImplementedException();
        }

        public override void SetColor(string key, Color val)
        {
            var vec = new Vector4(val.R / 255f, val.G / 255f, val.B / 255f, val.A / 255f);
            mShader.Parameters[key].SetValue(vec);
            mValuesChanged = true;
        }

        public override void SetVector2(string key, Pointf val)
        {
            //throw new NotImplementedException();
        }

        public override bool ValuesChanged()
        {
            return mValuesChanged;
        }

        public override void ResetChanged()
        {
            mValuesChanged = false;
        }

        public override object GetShader()
        {
            return mShader;
        }
    }
}