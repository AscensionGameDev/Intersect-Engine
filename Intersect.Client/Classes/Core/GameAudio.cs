using System;
using System.Collections.Generic;

using Intersect.Client.Entities;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Maps;

namespace Intersect.Client
{
    public static class GameAudio
    {
        private static bool sIsInitialized;

        //Music
        private static string sQueuedMusic = "";

        private static string sCurrentSong = "";
        private static float sFadeRate;
        private static long sFadeTimer;
        private static GameAudioInstance sMyMusic;
        private static bool sMusicLoop;
        private static bool sFadingOut;
        private static bool sQueuedLoop;
        private static float sQueuedFade;

        //Sounds
        private static List<GameSound> sGameSounds = new List<GameSound>();

        //Init
        public static void Init()
        {
            if (sIsInitialized == true)
            {
                return;
            }
            Globals.ContentManager.LoadAudio();
            sIsInitialized = true;
        }

        public static void UpdateGlobalVolume()
        {
            if (sMyMusic != null)
            {
                sMyMusic.SetVolume(sMyMusic.GetVolume(), true);
            }
            for (int i = 0; i < sGameSounds.Count; i++)
            {
                sGameSounds[i].Update();
                if (!sGameSounds[i].Loaded)
                {
                    sGameSounds.RemoveAt(i);
                }
            }
        }

        //Update
        public static void Update()
        {
            if (sMyMusic != null)
            {
                if (sFadeTimer != 0 && sFadeTimer < Globals.System.GetTimeMs())
                {
                    if (sFadingOut)
                    {
                        sMyMusic.SetVolume(sMyMusic.GetVolume() - 1, true);
                        if (sMyMusic.GetVolume() <= 1)
                        {
                            StopMusic();
                            if (sQueuedMusic != "")
                            {
                                PlayMusic(sQueuedMusic, 0f, sQueuedFade, sQueuedLoop);
                                sQueuedMusic = "";
                            }
                        }
                        else
                        {
                            sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                        }
                    }
                    else
                    {
                        sMyMusic.SetVolume(sMyMusic.GetVolume() + 1, true);
                        if (sMyMusic.GetVolume() < 100)
                        {
                            sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                        }
                        else
                        {
                            sFadeTimer = 0;
                        }
                    }
                }
                else
                {
                    if (sMyMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped)
                    {
                        if (sMusicLoop)
                        {
                            sMyMusic.Play();
                        }
                    }
                }
            }
            for (int i = 0; i < sGameSounds.Count; i++)
            {
                sGameSounds[i].Update();
                if (!sGameSounds[i].Loaded)
                {
                    sGameSounds.RemoveAt(i);
                }
            }
        }

        //Music
        public static void PlayMusic(string filename, float fadeout = 0f, float fadein = 0f, bool loop = false)
        {
            if (filename == null) filename = "";
            filename = GameContentManager.RemoveExtension(filename);
            if (sMyMusic != null)
            {
                if (fadeout == 0 || sMyMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped ||
                    sMyMusic.GetState() == GameAudioInstance.AudioInstanceState.Paused || sMyMusic.GetVolume() == 0)
                {
                    StopMusic();
                    StartMusic(filename, fadein);
                }
                else
                {
                    //Start fadeout
                    if (sCurrentSong.ToLower() != filename.ToLower() || sFadingOut)
                    {
                        sFadeRate = (float) sMyMusic.GetVolume() / fadeout;
                        sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                        sFadingOut = true;
                        sQueuedMusic = filename;
                        sQueuedFade = fadein;
                        sQueuedLoop = loop;
                    }
                }
            }
            else
            {
                StartMusic(filename, fadein);
            }
            sMusicLoop = loop;
        }

        private static void StartMusic(string filename, float fadein = 0f)
        {
            GameAudioSource music = Globals.ContentManager.GetMusic(filename);
            if (music != null)
            {
                sMyMusic = music.CreateInstance();
                sCurrentSong = filename;
                sMyMusic.Play();
                sMyMusic.SetVolume(0, true);
                sFadeRate = (float) 100 / fadein;
                sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000) + 1;
                sFadingOut = false;
            }
        }

        public static void StopMusic(float fadeout = 0f)
        {
            if (sMyMusic == null) return;

            if (Math.Abs(fadeout) < 0.01 || sMyMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped ||
                sMyMusic.GetState() == GameAudioInstance.AudioInstanceState.Paused || sMyMusic.GetVolume() == 0)
            {
                sCurrentSong = "";
                sMyMusic.Stop();
                sMyMusic.Dispose();
                sMyMusic = null;
                sFadeTimer = 0;
            }
            else
            {
                //Start fadeout
                sFadeRate = (float) sMyMusic.GetVolume() / fadeout;
                sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                sFadingOut = true;
            }
        }

        //Sounds
        public static MapSound AddMapSound(string filename, int x, int y, Guid mapId, bool loop, int distance, Entity parent = null)
        {
            if (sGameSounds?.Count > 128) return null;
            var sound = new MapSound(filename, x, y, mapId, loop, distance, parent);
            sGameSounds?.Add(sound);
            return sound;
        }

        public static GameSound AddGameSound(string filename, bool loop)
        {
            if (sGameSounds?.Count > 128) return null;
            var sound = new GameSound(filename, loop);
            sGameSounds?.Add(sound);
            return sound;
        }

        public static void StopSound(MapSound sound)
        {
            sound?.Stop();
        }

        public static void StopAllSounds()
        {
            for (int i = 0; i < sGameSounds.Count; i++)
            {
                if (sGameSounds[i] != null)
                {
                    sGameSounds[i].Stop();
                }
            }
        }
    }

    public class GameSound
    {
        protected GameAudioInstance mSound;
        protected string mFilename;
        protected bool mLoop;
        protected float mVolume;
        public bool Loaded;

        public GameSound(string filename, bool loop)
        {
            if (String.IsNullOrEmpty(filename)) return;
            mFilename = GameContentManager.RemoveExtension(filename).ToLower();
            mLoop = loop;
            GameAudioSource sound = Globals.ContentManager.GetSound(mFilename);
            if (sound != null)
            {
                mSound = sound.CreateInstance();
                mSound.SetLoop(mLoop);
                mSound.SetVolume(Globals.Database.SoundVolume);
                mSound.Play();
                Loaded = true;
            }
        }

        public virtual bool Update()
        {
            if (Loaded)
            {
                if (!mLoop && mSound.GetState() == GameAudioInstance.AudioInstanceState.Stopped)
                {
                    Stop();
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Stop()
        {
            if (Loaded)
            {
                mSound.Dispose();
                Loaded = false;
            }
        }

        public bool Loop
        {
            get => mLoop;
            set
            {
                mLoop = value;
                mSound?.SetLoop(mLoop);
            }
        }
    }

    public class MapSound : GameSound
    {
        private int mDistance;
        private Guid mMapId;
        private int mX;
        private int mY;
        private Entity mEntity;

        public MapSound(string filename, int x, int y, Guid mapId, bool loop, int distance, Entity parent = null) : base(filename, loop)
        {
            if (string.IsNullOrEmpty(filename) || mSound == null) return;
            mDistance = distance;
            mX = x;
            mY = y;
            mMapId = mapId;
            mEntity = parent;
            mSound.SetVolume(0);
        }

        public void UpdatePosition(int x, int y, Guid mapId)
        {
            mX = x;
            mY = y;
            mMapId = mapId;
        }

        public override bool Update()
        {
            if (base.Update())
            {
                UpdateSoundVolume();
                return true;
            }
            return false;
        }

        private void UpdateSoundVolume()
        {
            if (mMapId == Guid.Empty)
            {
                mSound.SetVolume(0);
                return;
            }
            var map = MapInstance.Get(mMapId);
            if ((map == null && mEntity != Globals.Me) || Globals.Me == null)
            {
                Stop();
                return;
            }

            var sameMap = mMapId == Globals.Me.CurrentMap;
            var inGrid = sameMap;
            if (!inGrid && Globals.Me.MapInstance != null)
            {
                var gridX = Globals.Me.MapInstance.MapGridX;
                var gridY = Globals.Me.MapInstance.MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != Guid.Empty)
                        {
                            if (Globals.MapGrid[x, y] == mMapId)
                            {
                                inGrid = true;
                                break;
                            }
                        }
                    }
                }
            }

            if ((mX == -1 || mY == -1 || mDistance <= 0) && sameMap)
            {
                mSound.SetVolume(100);
            }
            else
            {
                if (mDistance > 0 && Globals.GridMaps.Contains(mMapId))
                {
                    float volume = 100 - ((100 / (mDistance + 1)) * CalculateSoundDistance());
                    if (volume < 0)
                    {
                        volume = 0f;
                    }
                    mSound.SetVolume((int) volume);
                }
                else
                {
                    mSound.SetVolume(0);
                }
            }
        }

        private float CalculateSoundDistance()
        {
            float distance = 0f;
            float playerx = 0f;
            float playery = 0f;
            float soundx = 0;
            float soundy = 0;
            var map = MapInstance.Get(mMapId);
            var pMap = MapInstance.Get(Globals.Me.CurrentMap);
            if (map != null && pMap != null)
            {
                playerx = pMap.GetX() + Globals.Me.X * Options.TileWidth + 16;
                playery = pMap.GetY() + Globals.Me.Y * Options.TileHeight + 16;
                if (mX == -1 || mY == -1 || mDistance == -1)
                {
                    Point player = new Point()
                    {
                        X = (int) playerx,
                        Y = (int) playery
                    };
                    Rectangle mapRect = new Rectangle((int) map.GetX(), (int) map.GetY(),
                        Options.MapWidth * Options.TileWidth, Options.MapHeight * Options.TileHeight);
                    distance = (float) DistancePointToRectangle(player, mapRect) / 32f;
                }
                else
                {
                    soundx = map.GetX() + mX * Options.TileWidth + 16;
                    soundy = map.GetY() + mY * Options.TileHeight + 16;
                    distance = (float) Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) / 32f;
                }
            }
            return distance;
        }

        //Code Courtesy of  Philip Peterson. -- Released under MIT license.
        //Obtained, 06/27/2015 from http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle
        public static float DistancePointToRectangle(Point point, Rectangle rect)
        {
            //  Calculate a distance between a point and a rectangle.
            //  The area around/in the rectangle is defined in terms of
            //  several regions:
            //
            //  O--x
            //  |
            //  y
            //
            //
            //        I   |    II    |  III
            //      ======+==========+======   --yMin
            //       VIII |  IX (in) |  IV
            //      ======+==========+======   --yMax
            //       VII  |    VI    |   V
            //
            //
            //  Note that the +y direction is down because of Unity's GUI coordinates.

            if (point.X < rect.X)
            {
                // Region I, VIII, or VII
                if (point.Y < rect.Y)
                {
                    // I
                    point.X = point.X - rect.X;
                    point.Y = point.Y - rect.Y;
                    return (float) Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else if (point.Y > rect.Y + rect.Height)
                {
                    // VII
                    point.X = point.X - rect.X;
                    point.Y = point.Y - (rect.Y + rect.Height);
                    return (float) Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else
                {
                    // VIII
                    return rect.X - point.X;
                }
            }
            else if (point.X > rect.X + rect.Width)
            {
                // Region III, IV, or V
                if (point.Y < rect.Y)
                {
                    // III
                    point.X = point.X - (rect.X + rect.Width);
                    point.Y = point.Y - rect.Y;
                    return (float) Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else if (point.Y > rect.Y + rect.Height)
                {
                    // V
                    point.X = point.X - (rect.X + rect.Width);
                    point.Y = point.Y - (rect.Y + rect.Height);
                    return (float) Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else
                {
                    // IV
                    return point.X - (rect.X + rect.Width);
                }
            }
            else
            {
                // Region II, IX, or VI
                if (point.Y < rect.Y)
                {
                    // II
                    return rect.Y - point.Y;
                }
                else if (point.Y > rect.Y + rect.Height)
                {
                    // VI
                    return point.Y - (rect.Y + rect.Height);
                }
                else
                {
                    // IX
                    return 0f;
                }
            }
        }
    }
}