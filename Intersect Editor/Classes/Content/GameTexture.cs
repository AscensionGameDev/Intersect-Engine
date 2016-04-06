using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect_Editor.Classes.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Editor.Classes.Content
{
    public class GameTexture
    {
        private Texture2D _tex;
        private string _path = "";
        private bool _loadError = false;
        private int _width = -1;
        private int _height = -1;
        private long _lastAccessTime = 0;

        public GameTexture(string path)
        {
            _path = path;
            GameContentManager.AllTextures.Add(this);
        }

        public void LoadTexture()
        {
            _loadError = true;
            if (File.Exists(_path))
            {
                using (var fileStream = new FileStream(_path, FileMode.Open))
                {
                    _tex = Texture2D.FromStream(EditorGraphics.GetGraphicsDevice(), fileStream);
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
            _lastAccessTime = Environment.TickCount + 15000;
        }

        public int GetWidth()
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

        public int GetHeight()
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

        public Texture2D GetTexture()
        {
            ResetAccessTime();
            if (_tex == null) LoadTexture();
            return _tex;
        }

        public void Update()
        {
            if (_tex != null)
            {
                if (_lastAccessTime < Environment.TickCount)
                {
                    _tex.Dispose();
                }
            }
        }
    }
}
