using System;
using System.Collections.Generic;
using System.ComponentModel;

using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Utilities;

namespace Intersect.Editor.Maps
{

    class CustomCategory : CategoryAttribute
    {

        public CustomCategory(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return Strings.MapProperties.categories[value];
        }

    }

    class CustomDisplayName : DisplayNameAttribute
    {

        public CustomDisplayName(string name) : base(name)
        {
        }

        public override string DisplayName => Strings.MapProperties.displaynames[DisplayNameValue];

    }

    class CustomDescription : DescriptionAttribute
    {

        public CustomDescription(string desc) : base(desc)
        {
        }

        public override string Description => Strings.MapProperties.descriptions[DescriptionValue];

    }

    class MapProperties
    {

        private MapBase mMyMap;

        public MapProperties(MapBase map)
        {
            mMyMap = map;
        }

        [Browsable(false)]
        public Guid MapId => mMyMap.Id;

        [CustomCategory("general"), CustomDescription("namedesc"), CustomDisplayName("name"), DefaultValue("New Map")]
        public string Name
        {
            get => mMyMap.Name;
            set
            {
                if (mMyMap.Name != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Name = value;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("general"), CustomDescription("zonedesc"), CustomDisplayName("zonetype"),
         DefaultValue("Normal"), TypeConverter(typeof(MapZoneProperty)), Browsable(true)]
        public string ZoneType
        {
            get => Strings.MapProperties.zones[(int) mMyMap.ZoneType];
            set
            {
                Globals.MapEditorWindow.PrepUndoState();
                for (byte i = 0; i < Enum.GetNames(typeof(MapZones)).Length; i++)
                {
                    if (Strings.MapProperties.zones[i] == value)
                    {
                        mMyMap.ZoneType = (MapZones) i;
                    }
                }

                Globals.MapEditorWindow.AddUndoState();
            }
        }

        [CustomCategory("audio"), CustomDescription("musicdesc"), CustomDisplayName("music"), DefaultValue("None"),
         TypeConverter(typeof(MapMusicProperty)), Browsable(true)]
        public string Music
        {
            get
            {
                var musicList = new List<string> {Strings.General.none};
                musicList.AddRange(GameContentManager.SmartSortedMusicNames);
                mMyMap.Music = musicList.Find(
                    item => string.Equals(item, mMyMap.Music, StringComparison.InvariantCultureIgnoreCase)
                );

                return TextUtils.NullToNone(mMyMap.Music);
            }
            set
            {
                if (mMyMap.Music != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Music = TextUtils.SanitizeNone(value);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("audio"), CustomDescription("sounddesc"), CustomDisplayName("sound"), DefaultValue("None"),
         TypeConverter(typeof(MapSoundProperty)), Browsable(true)]
        public string Sound
        {
            get
            {
                var soundList = new List<string> {Strings.General.none};
                soundList.AddRange(GameContentManager.SmartSortedSoundNames);
                mMyMap.Sound = soundList.Find(
                    item => string.Equals(item, mMyMap.Sound, StringComparison.InvariantCultureIgnoreCase)
                );

                return TextUtils.NullToNone(mMyMap.Sound);
            }
            set
            {
                if (mMyMap.Sound != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Sound = TextUtils.SanitizeNone(value);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("isindoorsdesc"), CustomDisplayName("isindoors"),
         DefaultValue(false)]
        public bool IsIndoors
        {
            get => mMyMap.IsIndoors;
            set
            {
                if (mMyMap.IsIndoors != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.IsIndoors = value;
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("brightnessdesc"), CustomDisplayName("brightness"),
         DefaultValue(100)]
        public int Brightness
        {
            get => mMyMap.Brightness;
            set
            {
                if (mMyMap.Brightness != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Brightness = Math.Max(value, 0);
                    mMyMap.Brightness = Math.Min(mMyMap.Brightness, 100);
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("playerlightsizedesc"), CustomDisplayName("playerlightsize"),
         DefaultValue(300)]
        public int PlayerLightSize
        {
            get => mMyMap.PlayerLightSize;
            set
            {
                if (mMyMap.PlayerLightSize != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.PlayerLightSize = Math.Max(value, 0);
                    mMyMap.PlayerLightSize = Math.Min(mMyMap.PlayerLightSize, 1000);
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("playerlightexpanddesc"), CustomDisplayName("playerlightexpand"),
         DefaultValue(0)]
        public float PlayerLightExpand
        {
            get => mMyMap.PlayerLightExpand;
            set
            {
                if (mMyMap.PlayerLightExpand != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.PlayerLightExpand = Math.Max(value, 0f);
                    mMyMap.PlayerLightExpand = Math.Min(mMyMap.PlayerLightExpand, 1f);
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("playerlightintensitydesc"),
         CustomDisplayName("playerlightintensity"), DefaultValue(255)]
        public float PlayerLightIntensity
        {
            get => mMyMap.PlayerLightIntensity;
            set
            {
                if (mMyMap.PlayerLightIntensity != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    var val = Math.Max(value, (byte) 0);
                    mMyMap.PlayerLightIntensity = (byte) Math.Min(val, (byte) 255);
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("lighting"), CustomDescription("playerlightcolordesc"), CustomDisplayName("playerlightcolor"),
         DefaultValue(0)]
        public System.Drawing.Color PlayerLightColor
        {
            get =>
                System.Drawing.Color.FromArgb(
                    mMyMap.PlayerLightColor.A, mMyMap.PlayerLightColor.R, mMyMap.PlayerLightColor.G,
                    mMyMap.PlayerLightColor.B
                );
            set
            {
                if (mMyMap.PlayerLightColor.A != value.A ||
                    mMyMap.PlayerLightColor.R != value.R ||
                    mMyMap.PlayerLightColor.G != value.G ||
                    mMyMap.PlayerLightColor.B != value.B)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.PlayerLightColor = Color.FromArgb(value.A, value.R, value.G, value.B);
                    Graphics.TilePreviewUpdated = true;
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("overlay"), CustomDescription("rhuedesc"), CustomDisplayName("rhue"), DefaultValue(0)]
        public int RHue
        {
            get => mMyMap.RHue;
            set
            {
                if (mMyMap.RHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.RHue = Math.Max(value, 0);
                    mMyMap.RHue = Math.Min(mMyMap.RHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("overlay"), CustomDescription("ghuedesc"), CustomDisplayName("ghue"), DefaultValue(0)]
        public int GHue
        {
            get => mMyMap.GHue;
            set
            {
                if (mMyMap.GHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.GHue = Math.Max(value, 0);
                    mMyMap.GHue = Math.Min(mMyMap.GHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("overlay"), CustomDescription("bhuedesc"), CustomDisplayName("bhue"), DefaultValue(0)]
        public int BHue
        {
            get => mMyMap.BHue;
            set
            {
                if (mMyMap.BHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.BHue = Math.Max(value, 0);
                    mMyMap.BHue = Math.Min(mMyMap.BHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("overlay"), CustomDescription("ahuedesc"), CustomDisplayName("ahue"), DefaultValue(0)]
        public int AHue
        {
            get => mMyMap.AHue;
            set
            {
                if (mMyMap.AHue != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.AHue = Math.Max(value, 0);
                    mMyMap.AHue = Math.Min(mMyMap.AHue, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("fog"), CustomDescription("fogdesc"), CustomDisplayName("fog"), DefaultValue("None"),
         TypeConverter(typeof(MapFogProperty)), Browsable(true)]
        public string Fog
        {
            get
            {
                var fogList = new List<string>
                {
                    Strings.General.none
                };

                fogList.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Fog));
                if (fogList.IndexOf(mMyMap.Fog) <= -1)
                {
                    mMyMap.Fog = null;
                }

                return TextUtils.NullToNone(mMyMap.Fog);
            }
            set
            {
                if (mMyMap.Fog != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Fog = TextUtils.SanitizeNone(value);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("fog"), CustomDescription("fogxspeeddesc"), CustomDisplayName("fogxspeed"), DefaultValue(0)]
        public int FogXSpeed
        {
            get => mMyMap.FogXSpeed;
            set
            {
                if (mMyMap.FogXSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.FogXSpeed = Math.Max(value, -5);
                    mMyMap.FogXSpeed = Math.Min(mMyMap.FogXSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("fog"), CustomDescription("fogyspeeddesc"), CustomDisplayName("fogyspeed"), DefaultValue(0)]
        public int FogYSpeed
        {
            get => mMyMap.FogYSpeed;
            set
            {
                if (mMyMap.FogYSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.FogYSpeed = Math.Max(value, -5);
                    mMyMap.FogYSpeed = Math.Min(mMyMap.FogYSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("fog"), CustomDescription("fogalphadesc"), CustomDisplayName("fogalpha"), DefaultValue(0)]
        public int FogAlpha
        {
            get => mMyMap.FogTransparency;
            set
            {
                if (mMyMap.FogTransparency != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.FogTransparency = Math.Max(value, 0);
                    mMyMap.FogTransparency = Math.Min(mMyMap.FogTransparency, 255);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        //Weather
        [CustomCategory("weather"), CustomDescription("weatherdesc"), CustomDisplayName("weather"),
         DefaultValue("None"), TypeConverter(typeof(MapWeatherProperty)), Browsable(true)]
        public string Weather
        {
            get
            {
                var WeatherList = new List<string>
                {
                    Strings.General.none
                };

                WeatherList.AddRange(AnimationBase.Names);
                var name = AnimationBase.GetName(mMyMap.WeatherAnimationId);
                if (AnimationBase.Get(mMyMap.WeatherAnimationId) == null)
                {
                    name = null;
                }

                return TextUtils.NullToNone(name);
            }
            set
            {
                var idVal = Guid.Empty;
                if (!TextUtils.IsNone(value))
                {
                    var animationNames = new List<string>(AnimationBase.Names);
                    var index = animationNames.IndexOf(value);
                    idVal = AnimationBase.IdFromList(index);
                }

                if (mMyMap.WeatherAnimationId != idVal)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.WeatherAnimation = AnimationBase.Get(idVal);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("weather"), CustomDescription("weatherxspeeddesc"), CustomDisplayName("weatherxspeed"),
         DefaultValue(0)]
        public int WeatherXSpeed
        {
            get => mMyMap.WeatherXSpeed;
            set
            {
                if (mMyMap.WeatherXSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.WeatherXSpeed = Math.Max(value, -5);
                    mMyMap.WeatherXSpeed = Math.Min(mMyMap.WeatherXSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("weather"), CustomDescription("weatheryspeeddesc"), CustomDisplayName("weatheryspeed"),
         DefaultValue(0)]
        public int WeatherYSpeed
        {
            get => mMyMap.WeatherYSpeed;
            set
            {
                if (mMyMap.WeatherYSpeed != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.WeatherYSpeed = Math.Max(value, -5);
                    mMyMap.WeatherYSpeed = Math.Min(mMyMap.WeatherYSpeed, 5);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("weather"), CustomDescription("weatherintensitydesc"), CustomDisplayName("weatherintensity"),
         DefaultValue(0)]
        public int WeatherAlpha
        {
            get => mMyMap.WeatherIntensity;
            set
            {
                if (mMyMap.WeatherIntensity != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.WeatherIntensity = Math.Max(value, 0);
                    mMyMap.WeatherIntensity = Math.Min(mMyMap.WeatherIntensity, 100);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("misc"), CustomDescription("panoramadesc"), CustomDisplayName("panorama"), DefaultValue("None"),
         TypeConverter(typeof(MapImageProperty)), Browsable(true)]
        public string Panorama
        {
            get
            {
                var imageList = new List<string>
                {
                    Strings.General.none
                };

                imageList.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Image));
                if (imageList.IndexOf(mMyMap.Panorama) <= -1)
                {
                    mMyMap.Panorama = null;
                }

                return TextUtils.NullToNone(mMyMap.Panorama);
            }
            set
            {
                if (mMyMap.Panorama != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.Panorama = TextUtils.SanitizeNone(value);
                    Globals.MapEditorWindow.AddUndoState();
                }
            }
        }

        [CustomCategory("misc"), CustomDescription("overlaygraphicdesc"), CustomDisplayName("overlaygraphic"),
         DefaultValue("None"), TypeConverter(typeof(MapImageProperty)), Browsable(true)]
        public string OverlayGraphic
        {
            get
            {
                var imageList = new List<string>
                {
                    Strings.General.none
                };

                imageList.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Image));
                if (imageList.IndexOf(mMyMap.OverlayGraphic) <= -1)
                {
                    mMyMap.OverlayGraphic = null;
                }

                return TextUtils.NullToNone(mMyMap.OverlayGraphic);
            }
            set
            {
                if (mMyMap.OverlayGraphic != value)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    mMyMap.OverlayGraphic = TextUtils.SanitizeNone(value);
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var musicList = new List<string>
            {
                Strings.General.none
            };

            musicList.AddRange(GameContentManager.SmartSortedMusicNames);

            return new StandardValuesCollection(musicList.ToArray());
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var soundList = new List<string>
            {
                Strings.General.none
            };

            soundList.AddRange(GameContentManager.SmartSortedSoundNames);

            return new StandardValuesCollection(soundList.ToArray());
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var fogList = new List<string>
            {
                Strings.General.none
            };

            fogList.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Fog));

            return new StandardValuesCollection(fogList.ToArray());
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var imageList = new List<string>
            {
                Strings.General.none
            };

            imageList.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Image));

            return new StandardValuesCollection(imageList.ToArray());
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = new List<string>();
            for (byte i = 0; i < Enum.GetNames(typeof(MapZones)).Length; i++)
            {
                values.Add(Strings.MapProperties.zones[i]);
            }

            return new StandardValuesCollection(values);
        }

    }

    public class MapWeatherProperty : StringConverter
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var WeatherList = new List<string>
            {
                Strings.General.none
            };

            WeatherList.AddRange(AnimationBase.Names);

            return new StandardValuesCollection(WeatherList.ToArray());
        }

    }

}
