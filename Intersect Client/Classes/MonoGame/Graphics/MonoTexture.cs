using System.IO;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoTexture : GameTexture
    {
        private GraphicsDevice _graphicsDevice;
        private int _height = -1;
        private long _lastAccessTime;
        private bool _loadError;
        private string _path = "";
        private string _name = "";
        private Texture2D _tex;
        private int _width = -1;

        public MonoTexture(GraphicsDevice graphicsDevice, string filename)
        {
            _graphicsDevice = graphicsDevice;
            _path = filename;
            _name = Path.GetFileName(filename);
        }

        public void LoadTexture()
        {
            _loadError = true;
            if (File.Exists(_path))
            {
                using (var fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    _tex = Texture2D.FromStream(_graphicsDevice, fileStream);
                    if (_path.Contains("g7"))
                    {
                        var a = true;
                    }
                    if (_tex != null)
                    {
                        _width = _tex.Width;
                        _height = _tex.Height;
                        _loadError = false;
                    }
                }
            }
        }

        public void ResetAccessTime()
        {
            _lastAccessTime = Globals.System.GetTimeMS() + 15000;
        }

        public override string GetName()
        {
            return _name;
        }

        public override int GetWidth()
        {
            ResetAccessTime();
            if (_width == -1)
            {
                if (_tex == null) LoadTexture();
                if (_loadError)
                {
                    _width = 0;
                }
            }
            return _width;
        }

        public override int GetHeight()
        {
            ResetAccessTime();
            if (_height == -1)
            {
                if (_tex == null) LoadTexture();
                if (_loadError)
                {
                    _height = 0;
                }
            }
            return _height;
        }

        public override object GetTexture()
        {
            ResetAccessTime();
            if (_tex == null) LoadTexture();
            return _tex;
        }

        public override Color GetPixel(int x1, int y1)
        {
            if (_tex == null) LoadTexture();
            if (_loadError)
            {
                return Color.White;
            }
            else
            {
                Microsoft.Xna.Framework.Color[] pixel = new Microsoft.Xna.Framework.Color[1];
                _tex.GetData(0, new Microsoft.Xna.Framework.Rectangle(x1, y1, 1, 1), pixel, 0, 1);
                return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
            }
        }

        public void Update()
        {
            if (_tex != null)
            {
                if (_lastAccessTime < Globals.System.GetTimeMS())
                {
                    _tex.Dispose();
                    _tex = null;
                }
            }
        }
    }
}