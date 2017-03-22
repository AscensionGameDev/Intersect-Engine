using System;
using System.ComponentModel;
using System.Collections.Generic;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.Localization;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Classes.Maps
{
    class CustomCategory : CategoryAttribute
    {
        public CustomCategory(string category) : base(category) { }
        protected override string GetLocalizedString(string value)
        {
            return Strings.Get("mapproperties",value);
        }
    }
    class CustomDisplayName : DisplayNameAttribute
    {
        public CustomDisplayName(string name) : base(name) { }

        public override string DisplayName => Strings.Get("mapproperties",DisplayNameValue);
    }

    class CustomDescription : DescriptionAttribute
    {
        public CustomDescription(string desc) : base(desc) {}

        public override string Description => Strings.Get("mapproperties", DescriptionValue);
    }

    class MapProperties
    {
        private MapBase _myMap;

        public MapProperties(MapBase map)
        {
            _myMap = map;
        }

        [CustomCategory("general"),
        CustomDescription("namedesc"),
        CustomDisplayName("name"),
        DefaultValueAttribute("New Map")]
        public string Name
        {
            get { return _myMap.Name; }
            set
            {
                if (_myMap.Name != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Name = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("general"),
        CustomDescription("zonedesc"),
        CustomDisplayName("zonetype"),
        DefaultValueAttribute("Normal"),
        TypeConverter(typeof(MapZoneProperty)),
        Browsable(true)]
        public string ZoneType
        {
            get
            {
                return Strings.Get("mapproperties","zone" + (int)_myMap.ZoneType);
            }
            set
            {
                Globals.MapEditorWindow.PrepUndoState();
                for (byte i = 0; i < Enum.GetNames(typeof(MapZones)).Length; i++)
                {
                    if (Strings.Get("mapproperties", "zone" + i) == value)
                    {
                        _myMap.ZoneType = (MapZones)i;
                    }
                }
                Globals.MapEditorWindow.AddUndoState();
            }
        }

        [CustomCategory("audio"),
        CustomDescription("musicdesc"),
        CustomDisplayName("music"),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapMusicProperty)),
        Browsable(true)]
        public string Music
        {
            get
            {
                List<string> MusicList = new List<string>
                {
                    Strings.Get("general", "none")
                };
                MusicList.AddRange(GameContentManager.GetMusicNames());
                if (MusicList.IndexOf(_myMap.Music) <= -1)
                {
                    _myMap.Music = Strings.Get("general","none");
                }
                return _myMap.Music;
            }
            set
            {
                if (_myMap.Music != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Music = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("audio"),
        CustomDescription("sounddesc"),
        CustomDisplayName("sound"),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapSoundProperty)),
        Browsable(true)]
        public string Sound
        {
            get
            {
                List<string> SoundList = new List<string>
                {
                    Strings.Get("general", "none")
                };
                SoundList.AddRange(GameContentManager.GetSoundNames());
                if (SoundList.IndexOf(_myMap.Sound) <= -1)
                {
                    _myMap.Sound = Strings.Get("general","none");
                }
                return _myMap.Sound;
            }
            set
            {
                if (_myMap.Sound != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Sound = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"),
        CustomDescription("isindoorsdesc"),
        CustomDisplayName("isindoors"),
        DefaultValueAttribute(false)]
        public bool IsIndoors
        {
            get { return _myMap.IsIndoors; }
            set
            {
                if (_myMap.IsIndoors != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.IsIndoors = value;
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("lighting"),
        CustomDescription("brightnessdesc"),
        CustomDisplayName("brightness"),
        DefaultValueAttribute(100)]
        public int Brightness
        {
            get { return _myMap.Brightness; }
            set
            {
                if (_myMap.Brightness != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Brightness = Math.Max(value, 0);
                    _myMap.Brightness = Math.Min(_myMap.Brightness, 100);
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("lighting"),
        CustomDescription("playerlightsizedesc"),
        CustomDisplayName("playerlightsize"),
        DefaultValueAttribute(300)]
        public int PlayerLightSize
        {
            get { return _myMap.PlayerLightSize; }
            set
            {
                if (_myMap.PlayerLightSize != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.PlayerLightSize = Math.Max(value, 0);
                    _myMap.PlayerLightSize = Math.Min(_myMap.PlayerLightSize, 1000);
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("lighting"),
        CustomDescription("playerlightexpanddesc"),
        CustomDisplayName("playerlightexpand"),
        DefaultValueAttribute(0)]
        public float PlayerLightExpand
        {
            get { return _myMap.PlayerLightExpand; }
            set
            {
                if (_myMap.PlayerLightExpand != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.PlayerLightExpand = Math.Max(value, 0f);
                    _myMap.PlayerLightExpand = Math.Min(_myMap.PlayerLightExpand, 1f);
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("lighting"),
        CustomDescription("playerlightintensitydesc"),
        CustomDisplayName("playerlightintensity"),
        DefaultValueAttribute(255)]
        public byte PlayerLightIntensity
        {
            get { return _myMap.PlayerLightIntensity; }
            set
            {
                if (_myMap.PlayerLightIntensity != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.PlayerLightIntensity = Math.Max(value, (byte)0);
                    _myMap.PlayerLightIntensity = Math.Min(_myMap.PlayerLightIntensity, (byte)255);
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("lighting"),
        CustomDescription("playerlightcolordesc"),
        CustomDisplayName("playerlightcolor"),
        DefaultValueAttribute(0)]
        public Color PlayerLightColor
        {
            get { return Color.FromArgb(_myMap.PlayerLightColor.A, _myMap.PlayerLightColor.R, _myMap.PlayerLightColor.G, _myMap.PlayerLightColor.B); }
            set
            {
                if (_myMap.PlayerLightColor.A != value.A || _myMap.PlayerLightColor.R != value.R || _myMap.PlayerLightColor.G != value.G || _myMap.PlayerLightColor.B != value.B)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.PlayerLightColor = Intersect_Library.Color.FromArgb(value.A, value.R, value.G, value.B);
                    EditorGraphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("overlay"),
        CustomDescription("rhuedesc"),
        CustomDisplayName("rhue"),
        DefaultValueAttribute(0)]
        public int RHue
        {
            get { return _myMap.RHue; }
            set
            {
                if (_myMap.RHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.RHue = Math.Max(value, 0);
                    _myMap.RHue = Math.Min(_myMap.RHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("overlay"),
        CustomDescription("ghuedesc"),
        CustomDisplayName("ghue"),
        DefaultValueAttribute(0)]
        public int GHue
        {
            get { return _myMap.GHue; }
            set
            {
                if (_myMap.GHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.GHue = Math.Max(value, 0);
                    _myMap.GHue = Math.Min(_myMap.GHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("overlay"),
        CustomDescription("bhuedesc"),
        CustomDisplayName("bhue"),
        DefaultValueAttribute(0)]
        public int BHue
        {
            get { return _myMap.BHue; }
            set
            {
                if (_myMap.BHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.BHue = Math.Max(value, 0);
                    _myMap.BHue = Math.Min(_myMap.BHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("overlay"),
        CustomDescription("ahuedesc"),
        CustomDisplayName("ahue"),
        DefaultValueAttribute(0)]
        public int AHue
        {
            get { return _myMap.AHue; }
            set
            {
                if (_myMap.AHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.AHue = Math.Max(value, 0);
                    _myMap.AHue = Math.Min(_myMap.AHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("fog"),
        CustomDescription("fogdesc"),
        CustomDisplayName("fog"),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapFogProperty)),
        Browsable(true)]
        public string Fog
        {
            get
            {
                List<string> FogList = new List<string>
                {
                    Strings.Get("general", "none")
                };
                FogList.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Fog));
                if (FogList.IndexOf(_myMap.Fog) <= -1)
                {
                    _myMap.Fog = Strings.Get("general","none");
                }
                return _myMap.Fog;
            }
            set
            {
                if (_myMap.Fog != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Fog = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("fog"),
        CustomDescription("fogxspeeddesc"),
        CustomDisplayName("fogxspeed"),
        DefaultValueAttribute(0)]
        public int FogXSpeed
        {
            get { return _myMap.FogXSpeed; }
            set
            {
                if (_myMap.FogXSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.FogXSpeed = Math.Max(value, -5);
                    _myMap.FogXSpeed = Math.Min(_myMap.FogXSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("fog"),
        CustomDescription("fogyspeeddesc"),
        CustomDisplayName("fogyspeed"),
        DefaultValueAttribute(0)]
        public int FogYSpeed
        {
            get { return _myMap.FogYSpeed; }
            set
            {
                if (_myMap.FogYSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.FogYSpeed = Math.Max(value, -5);
                    _myMap.FogYSpeed = Math.Min(_myMap.FogYSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CustomCategory("fog"),
        CustomDescription("fogalphadesc"),
        CustomDisplayName("fogalpha"),
        DefaultValueAttribute(0)]
        public int FogAlpha
        {
            get { return _myMap.FogTransparency; }
            set
            {
                if (_myMap.FogTransparency != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.FogTransparency = Math.Max(value, 0);
                    _myMap.FogTransparency = Math.Min(_myMap.FogTransparency, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("misc"),
        CustomDescription("panoramadesc"),
        CustomDisplayName("panorama"),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapImageProperty)),
        Browsable(true)]
        public string Panorama
        {
            get
            {
                List<string> ImageList = new List<string>
                {
                    Strings.Get("general", "none")
                };
                ImageList.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Image));
                if (ImageList.IndexOf(_myMap.Panorama) <= -1)
                {
                    _myMap.Panorama = Strings.Get("general","none");
                }
                return _myMap.Panorama;
            }
            set
            {
                if (_myMap.Panorama != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.Panorama = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("misc"),
        CustomDescription("overlaygraphicdesc"),
        CustomDisplayName("overlaygraphic"),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapImageProperty)),
        Browsable(true)]
        public string OverlayGraphic
        {
            get
            {
                List<string> ImageList = new List<string>
                {
                    Strings.Get("general", "none")
                };
                ImageList.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Image));
                if (ImageList.IndexOf(_myMap.OverlayGraphic) <= -1)
                {
                    _myMap.Panorama = Strings.Get("general","none");
                }
                return _myMap.OverlayGraphic;
            }
            set
            {
                if (_myMap.OverlayGraphic != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.OverlayGraphic = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
    }

    public class MapMusicProperty : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return false;
        }

        public override StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> MusicList = new List<string>
            {
                Strings.Get("general", "none")
            };
            MusicList.AddRange(GameContentManager.GetMusicNames());
            return new StandardValuesCollection(MusicList.ToArray());
        }
    }

    public class MapSoundProperty : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return false;
        }

        public override StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> SoundList = new List<string>
            {
                Strings.Get("general", "none")
            };
            SoundList.AddRange(GameContentManager.GetSoundNames());
            return new StandardValuesCollection(SoundList.ToArray());
        }
    }

    public class MapFogProperty : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return false;
        }

        public override StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> FogList = new List<string>
            {
                Strings.Get("general", "none")
            };
            FogList.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Fog));
            return new StandardValuesCollection(FogList.ToArray());
        }
    }

    public class MapImageProperty : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return false;
        }

        public override StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> ImageList = new List<string>
            {
                Strings.Get("general", "none")
            };
            ImageList.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Image));
            return new StandardValuesCollection(ImageList.ToArray());
        }
    }

    public class MapZoneProperty : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return false;
        }

        public override StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetNames(typeof(MapZones)));
        }
    }

}
