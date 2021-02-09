using System;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class TimeBase
    {

        private static TimeBase sTimeBase = new TimeBase();

        [NotMapped] public Color[] DaylightHues;

        public TimeBase()
        {
            ResetColors();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        [JsonIgnore]
        [Column("DaylightHues")]
        public string DaylightHuesJson
        {
            get => JsonConvert.SerializeObject(DaylightHues, Formatting.None);
            protected set => DaylightHues = JsonConvert.DeserializeObject<Color[]>(value);
        }

        public int RangeInterval { get; set; } = 720;

        public float Rate { get; set; } = 1.0f;

        public bool SyncTime { get; set; } = true;

        public void LoadFromJson(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }

        public string GetInstanceJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static string GetTimeJson()
        {
            return JsonConvert.SerializeObject(sTimeBase);
        }

        public void ResetColors()
        {
            DaylightHues = new Color[1440 / RangeInterval];
            for (var i = 0; i < 1440 / RangeInterval; i++)
            {
                DaylightHues[i] = new Color(255, 255, 255, 255);
            }
        }

        public static int GetIntervalIndex(int minutes)
        {
            switch (minutes)
            {
                case 1440: //24 hour span
                    return 0;
                case 720: //12 hour spans
                    return 1;
                case 480: //8 hour spans
                    return 2;
                case 360: // 6 hour spans
                    return 3;
                case 240: //4 hour spans
                    return 4;
                case 180: //3 hour spans
                    return 5;
                case 120: //2 hour spans
                    return 6;
                case 60: //1 hour span
                    return 7;
                case 45: //45 minute span
                    return 8;
                case 30: //30 minute span
                    return 9;
                case 15: //15 minute span
                    return 10;
                case 10: //10 minute span
                    return 11;
            }

            return 5;
        }

        public static int GetTimeInterval(int index)
        {
            switch (index)
            {
                case 0: //24 hour span
                    return 1440;
                case 1: //12 hour spans
                    return 720;
                case 2: //8 hour spans
                    return 480;
                case 3: // 6 hour spans
                    return 360;
                case 4: //4 hour spans
                    return 240;
                case 5: //3 hour spans
                    return 180;
                case 6: //2 hour spans
                    return 120;
                case 7: //1 hour span
                    return 60;
                case 8: //45 minute span
                    return 45;
                case 9: //30 minute span
                    return 30;
                case 10: //15 minute span
                    return 15;
                case 11: //10 minute span
                    return 10;
            }

            return 5;
        }

        public static void SetStaticTime(TimeBase time)
        {
            sTimeBase = time;
        }

        public static TimeBase GetTimeBase()
        {
            return sTimeBase;
        }

    }

}
