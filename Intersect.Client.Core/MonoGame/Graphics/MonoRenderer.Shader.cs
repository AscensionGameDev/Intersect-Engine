using System.Numerics;
using Intersect.Client.Framework.Graphics;
using Intersect.IO.Files;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer
{
    private partial class Shader : GameShader
    {
        private readonly MonoRenderer _renderer;
        private readonly Dictionary<string, Color> Colors = new();

        private readonly Dictionary<string, float> Floats = new();

        private bool mValuesChanged;

        public Effect PlatformShader { get; }

        public override IGameTexture? Texture
        {
            get
            {
                return PlatformShader switch
                {
                    BasicEffect basicEffect => _renderer._allocatedTextures.GetValueOrDefault(basicEffect.Texture),
                    _ => throw new NotSupportedException(PlatformShader.ToString())
                };
            }
            set
            {
                switch (PlatformShader)
                {
                    case BasicEffect basicEffect:
                        basicEffect.Texture = value?.GetTexture<Texture2D>();
                        break;
                    default:
                        throw new NotSupportedException(PlatformShader.ToString());
                }
            }
        }

        internal Shader(MonoRenderer renderer, string shaderName, ContentManager contentManager) : base(shaderName)
        {
            _renderer = renderer;
            using var resourceStream = typeof(Shader).Assembly.GetManifestResourceStream(shaderName);
            var extractedPath = FileSystemHelper.WriteToTemporaryFolder(shaderName, resourceStream);
            PlatformShader = contentManager.Load<Effect>(Path.ChangeExtension(extractedPath, null));
        }

        internal Shader(MonoRenderer renderer, Effect effect) : base(effect.ToString())
        {
            _renderer = renderer;
            PlatformShader = effect;
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
                if (Colors[key].A != val.A ||
                    Colors[key].R != val.R ||
                    Colors[key].G != val.G ||
                    Colors[key].B != val.B)
                {
                    Colors[key] = val;
                    mValuesChanged = true;
                }
            }
        }

        public override void SetVector2(string key, Vector2 val)
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
                PlatformShader.Parameters[clr.Key].SetValue(vec);
            }

            foreach (var f in Floats)
            {
                PlatformShader.Parameters[f.Key].SetValue(f.Value);
            }
        }

        public override object GetShader()
        {
            return PlatformShader;
        }
    }
}