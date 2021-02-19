using System;
using System.Collections.Generic;

using Intersect.Client.Core.Sounds;
using Intersect.Client.Entities;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.General;
using Intersect.Compression;
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

        private static GameAudioInstance sMyMusic { get; set; }

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
                        sMyMusic.SetVolume(sMyMusic.GetVolume() - 1, true);
                        if (sMyMusic.GetVolume() <= 1)
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
            }

            for (var i = 0; i < sGameSounds.Count; i++)
            {
                sGameSounds[i].Update();
                if (!sGameSounds[i].Loaded)
                {
                    sGameSounds.RemoveAt(i);
                }
            }

            // Update our pack sound cache if we have any packs loaded.
            if (Globals.ContentManager.SoundPacks != null)
            {
                Globals.ContentManager.SoundPacks.UpdateCache();
            }

            // Update our pack music cache if we have any packs loaded.
            if (Globals.ContentManager.MusicPacks != null)
            {
                Globals.ContentManager.MusicPacks.UpdateCache();
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

            filename = GameContentManager.RemoveExtension(filename);
            if (sMyMusic != null)
            {
                if (fadeout < 0.01 ||
                    sMyMusic.State == GameAudioInstance.AudioInstanceState.Stopped ||
                    sMyMusic.State == GameAudioInstance.AudioInstanceState.Paused ||
                    sMyMusic.GetVolume() == 0)
                {
                    StopMusic();
                    StartMusic(filename, fadein, loop);
                }
                else
                {
                    //Start fadeout
                    if (!string.Equals(sCurrentSong, filename, StringComparison.CurrentCultureIgnoreCase) || sFadingOut)
                    {
                        sFadeRate = sMyMusic.GetVolume() / fadeout;
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
            var music = Globals.ContentManager.GetMusic(filename);
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
            sMyMusic.SetVolume(0, true);
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
                sMyMusic.State == GameAudioInstance.AudioInstanceState.Stopped ||
                sMyMusic.State == GameAudioInstance.AudioInstanceState.Paused ||
                sMyMusic.GetVolume() == 0)
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
        public static MapSound AddMapSound(
            string filename,
            int x,
            int y,
            Guid mapId,
            bool loop,
            int loopInterval,
            int distance,
            Entity parent = null
        )
        {
            if (sGameSounds?.Count > 128)
            {
                return null;
            }

            var sound = new MapSound(filename, x, y, mapId, loop, loopInterval, distance, parent);
            sGameSounds?.Add(sound);

            return sound;
        }

        public static Sound AddGameSound(string filename, bool loop)
        {
            if (sGameSounds?.Count > 128)
            {
                return null;
            }

            var sound = new Sound(filename, loop, 0);
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
