using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

namespace Intersect_Editor.Classes.Maps
{
    class MapProperties
    {
        private MapStruct _myMap;

        public MapProperties(MapStruct map)
        {
            _myMap = map;
        }

        [CategoryAttribute("General"),
        Description("The name of this map."), 
        DefaultValueAttribute("New Map")]
        public string Name
        {
            get { return _myMap.MyName; }
            set {
                if (_myMap.MyName != value) {
                    Globals.MapEditorWindow.PrepUndoState();
                    _myMap.MyName = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CategoryAttribute("Audio"),
        Description("Looping background music for this map."), 
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapMusicProperty)),
        Browsable(true)]
        public string Music
        {
            get {
                List<string> MusicList = new List<string>();
                MusicList.Add("None");
                MusicList.AddRange(Audio.MusicFileNames);
                if (MusicList.IndexOf(_myMap.Music) <= -1)
                {
                    _myMap.Music = "None";
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

        [CategoryAttribute("Audio"),
        Description("Looping sound effect for this map."),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapSoundProperty)),
        Browsable(true)]
        public string Sound
        {
            get
            {
                List<string> SoundList = new List<string>();
                SoundList.Add("None");
                SoundList.AddRange(Audio.SoundFileNames);
                if (SoundList.IndexOf(_myMap.Sound) <= -1)
                {
                    _myMap.Sound = "None";
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

        [CategoryAttribute("Lighting"),
        Description("Is the map indoors?"), 
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
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }
        [CategoryAttribute("Lighting"),
        Description("How bright is this map? (Range: 0 to 100)."), 
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
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CategoryAttribute("Overlay"),
        Description("The amount of red in the overlay. (Range: 0 to 255)"), 
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
        [CategoryAttribute("Overlay"),
        Description("The amount of green in the overlay. (Range: 0 to 255)"), 
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
        [CategoryAttribute("Overlay"),
        Description("The amount of blue in the overlay. (Range: 0 to 255)"), 
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
        [CategoryAttribute("Overlay"),
        Description("The how strong the overlay appears. (Range: 0 [transparent/invisible] to 255 [solid/can't see map])"), 
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

        [CategoryAttribute("Fog"),
        Description("The overlayed image on the map. Generally used for fogs or sun beam effects."),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapFogProperty)),
        Browsable(true)]
        public string Fog
        {
            get
            {
                List<string> FogList = new List<string>();
                FogList.Add("None");
                FogList.AddRange(Graphics.FogFileNames);
                if (FogList.IndexOf(_myMap.Fog) <= -1)
                {
                    _myMap.Fog = "None";
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
        [CategoryAttribute("Fog"),
        Description("Fog Horizontal Speed (Range: -5 to 5)"),
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
        [CategoryAttribute("Fog"),
        Description("Fog Vertical Speed (Range: -5 to 5)"),
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
        [CategoryAttribute("Fog"),
        Description("The how strong the fog overlay appears. (Range: 0 [transparent/invisible] to 255 [solid/can't see map])"),
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

        [CategoryAttribute("Misc"),
        Description("This is an image that appears behind the map. It can be seen where no tiles are placed."),
        DefaultValueAttribute("None"),
        TypeConverter(typeof(MapImageProperty)),
        Browsable(true)]
        public string Panorama
        {
            get
            {
                List<string> ImageList = new List<string>();
                ImageList.Add("None");
                ImageList.AddRange(Graphics.ImageFileNames);
                if (ImageList.IndexOf(_myMap.Panorama) <= -1)
                {
                    _myMap.Panorama = "None";
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

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> MusicList = new List<string>();
            MusicList.Add("None");
            MusicList.AddRange(Audio.MusicFileNames);
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

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> SoundList = new List<string>();
            SoundList.Add("None");
            SoundList.AddRange(Audio.SoundFileNames);
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

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> FogList = new List<string>();
            FogList.Add("None");
            FogList.AddRange(Graphics.FogFileNames);
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

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> ImageList = new List<string>();
            ImageList.Add("None");
            ImageList.AddRange(Graphics.ImageFileNames);
            return new StandardValuesCollection(ImageList.ToArray());
        }
    }

}
