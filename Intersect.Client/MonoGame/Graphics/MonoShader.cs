using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.IO.Files;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Intersect.Client.MonoGame.Graphics
{

    public class MonoShader : GameShader
    {

        private Effect mShader;

        private bool mValuesChanged = false;

        private Dictionary<string, Color> Colors = new Dictionary<string, Color>();

        private Dictionary<string, float> Floats = new Dictionary<string, float>();

        public MonoShader(string shaderName, ContentManager contentManager) : base(shaderName)
        {
            using (var resourceStream = typeof(MonoShader).Assembly.GetManifestResourceStream(shaderName))
            {
                var extractedPath = FileSystemHelper.WriteToTemporaryFolder(shaderName, resourceStream);
                mShader = contentManager.Load<Effect>(Path.ChangeExtension(extractedPath, null));
            }
        }

        public override void SetFloat(string key, float val)
        {
            if (!Floats.ContainsKey(key))
            {
                Floats.Add(key, val);
                mValuesChanged = true;
            }
            else
            {
                if (Floats[key] != val)
                {
                    Floats[key] = val;
                    mValuesChanged = true;
                }
            }
        }

        public override void SetInt(string key, int val)
        {
            //throw new NotImplementedException();
        }

        public override void SetColor(string key, Color val)
        {
            if (!Colors.ContainsKey(key))
            {
                Colors.Add(key, val);
                mValuesChanged = true;
            }
            else
            {
                if (Colors[key].A != val.A || Colors[key].R != val.R || Colors[key].G != val.G || Colors[key].B != val.B)
                {
                    Colors[key] = val;
                    mValuesChanged = true;
                }
            }
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

            //Set Pending
            foreach (var clr in Colors)
            {
                var vec = new Vector4(clr.Value.R / 255f, clr.Value.G / 255f, clr.Value.B / 255f, clr.Value.A / 255f);
                mShader.Parameters[clr.Key].SetValue(vec);
            }

            foreach (var f in Floats)
            {
                mShader.Parameters[f.Key].SetValue(f.Value);
            }
        }

        public override object GetShader()
        {
            return mShader;
        }

    }

}
