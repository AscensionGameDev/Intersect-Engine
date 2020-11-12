using System;
using System.Collections.Generic;
using System.IO;

using Intersect.Client.Core.Sounds;
using Intersect.Client.Entities;
using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.General;
using Intersect.Logging;

namespace Intersect.Client.Core
{

    public static class Audio
    {

        private static string sCurrentSong = "";

        private static float sFadeRate;

        private static long sFadeTimer;

        private static bool sFadingOut;

        //Sounds
        private static List<Sound> sGameSounds = new List<Sound>();

        private static bool sIsInitialized;

        private static float sQueuedFade;

        private static bool sQueuedLoop;

        //Music
        private static string sQueuedMusic = "";

        private static IAudioInstance sMyMusic { get; set; }

        private static IGameContext GameContext { get; set; }

        //Init
        public static void Init(IGameContext gameContext)
        {
            if (sIsInitialized == true)
            {
                return;
            }

            GameContext = gameContext;

            // GameContext.ContentManager.LoadAudio();
            sIsInitialized = true;
        }

        public static void UpdateGlobalVolume()
        {
            if (sMyMusic != null)
            {
                sMyMusic.Volume = sMyMusic.Volume;
            }

            for (var i = 0; i < sGameSounds.Count; i++)
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
                        sMyMusic.Volume = sMyMusic.Volume - 1;
                        if (sMyMusic.Volume <= 1)
                        {
                            StopMusic();
                            PlayMusic(sQueuedMusic, 0f, sQueuedFade, sQueuedLoop);
                        }
                        else
                        {
                            sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                        }
                    }
                    else
                    {
                        sMyMusic.Volume = sMyMusic.Volume + 1;
                        if (sMyMusic.Volume < 100)
                        {
                            sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                        }
                        else
                        {
                            sFadeTimer = 0;
                        }
                    }
                }
            }

            for (var i = 0; i < sGameSounds.Count; i++)
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
            if (string.IsNullOrWhiteSpace(filename))
            {
                //Entered a map with no music selected, fade out any music that's already playing.
                StopMusic(3f);

                return;
            }

            ClearQueue();

            // filename = GameContentManager.RemoveExtension(filename);
            filename = Path.GetFileNameWithoutExtension(filename);
            if (sMyMusic != null)
            {
                if (fadeout < 0.01 ||
                    sMyMusic.State == AudioState.Stopped ||
                    sMyMusic.State == AudioState.Paused ||
                    sMyMusic.Volume == 0)
                {
                    StopMusic();
                    StartMusic(filename, fadein, loop);
                }
                else
                {
                    //Start fadeout
                    if (!string.Equals(sCurrentSong, filename, StringComparison.CurrentCultureIgnoreCase) || sFadingOut)
                    {
                        sFadeRate = sMyMusic.Volume / fadeout;
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
                StartMusic(filename, fadein, loop);
            }
        }

        private static void ClearQueue()
        {
            sQueuedMusic = null;
            sQueuedLoop = false;
            sQueuedFade = -1;
        }

        private static void StartMusic(string filename, float fadein = 0f, bool loop = false)
        {
            var trackName = Path.GetFileNameWithoutExtension(filename);
            var music = GameContext.ContentManager.Find<IAudioSource>(ContentType.Music, trackName);
            if (music == null)
            {
                return;
            }

            if (sMyMusic != null)
            {
                Log.Warn($"Trying to start '{filename}' without properly closing '{sCurrentSong}'.");
            }

            sMyMusic = music.CreateInstance();
            sCurrentSong = filename;
            sMyMusic.Play();
            sMyMusic.Volume = 0;
            sMyMusic.IsLooping = loop;
            sFadeRate = (float) 100 / fadein;
            sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000) + 1;
            sFadingOut = false;
        }

        public static void StopMusic(float fadeout = 0f)
        {
            if (sMyMusic == null)
            {
                return;
            }

            if (Math.Abs(fadeout) < 0.01 ||
                sMyMusic.State == AudioState.Stopped ||
                sMyMusic.State == AudioState.Paused ||
                sMyMusic.Volume == 0)
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
                sFadeRate = (float) sMyMusic.Volume / fadeout;
                sFadeTimer = Globals.System.GetTimeMs() + (long) (sFadeRate / 1000);
                sFadingOut = true;
            }
        }

        //Sounds
        public static MapSound AddMapSound(
            string filename,
            int x,
            int y,
            Guid mapId,
            bool loop,
            int distance,
            Entity parent = null
        )
        {
            if (sGameSounds?.Count > 128)
            {
                return null;
            }

            var sound = new MapSound(GameContext, filename, x, y, mapId, loop, distance, parent);
            sGameSounds?.Add(sound);

            return sound;
        }

        public static Sound AddGameSound(string filename, bool loop)
        {
            if (sGameSounds?.Count > 128)
            {
                return null;
            }

            var sound = new Sound(GameContext, filename, loop);
            sGameSounds?.Add(sound);

            return sound;
        }

        public static void StopSound(MapSound sound)
        {
            sound?.Stop();
        }

        public static void StopAllSounds()
        {
            for (var i = 0; i < sGameSounds.Count; i++)
            {
                if (sGameSounds[i] != null)
                {
                    sGameSounds[i].Stop();
                }
            }
        }

    }

}
