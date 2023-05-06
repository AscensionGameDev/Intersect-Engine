using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core.Sounds;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.General;
using Intersect.Logging;
using Intersect.Utilities;

namespace Intersect.Client.Core
{

    public static partial class Audio
    {

        private static string sCurrentSong = "";

        private static int sFadeRate;

        private static long sFadeTimer;

        private static bool sFadingOut;

        //Sounds
        private static List<Sound> sGameSounds = new List<Sound>();

        private static bool sIsInitialized;

        private static int sQueuedFade;

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
                var currentTime = Timing.Global.MillisecondsUtc;
                if (sFadeTimer != 0 && sFadeTimer < currentTime)
                {
                    if (sFadingOut)
                    {
                        sMyMusic.SetVolume(sMyMusic.GetVolume() - 1, true);
                        if (sMyMusic.GetVolume() <= 1)
                        {
                            StopMusic();
                            PlayMusic(sQueuedMusic, 0, sQueuedFade, sQueuedLoop);
                        }
                        else
                        {
                            sFadeTimer = currentTime + sFadeRate;
                        }
                    }
                    else
                    {
                        sMyMusic.SetVolume(sMyMusic.GetVolume() + 1, true);
                        if (sMyMusic.GetVolume() < 100)
                        {
                            sFadeTimer = currentTime + sFadeRate;
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
        /// <summary>
        /// Play a music track, automatically fading out the old track with the configured values.
        /// </summary>
        /// <param name="filename">The song file name to start playing. A blank filename will only stop the currently playing music track.</param>
        /// <param name="fadein">The time (in ms) it should take to fade in the new music track.</param>
        /// <param name="fadeout">The time (in ms) it should take to fade out the current music track.</param>
        /// <param name="loop">Determines whether the song loops once it's over or not.</param>
        public static void PlayMusic(string filename, int fadeout = 0, int fadein = 0, bool loop = false)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                //Entered a map with no music selected, fade out any music that's already playing.
                StopMusic(fadeout);

                return;
            }

            ClearQueue();

            filename = GameContentManager.RemoveExtension(filename);
            if (sMyMusic != null)
            {
                if (fadeout == 0 ||
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
                        StopMusic(fadeout);
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

        /// <summary>
        /// Start playing a new music track.
        /// </summary>
        /// <param name="filename">The song file name to start playing.</param>
        /// <param name="fadein">The time (in ms) it should take to fade in the new music track.</param>
        /// <param name="loop">Determines whether the song loops once it's over or not.</param>
        private static void StartMusic(string filename, int fadein = 0, bool loop = false)
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
            sFadeRate = fadein / 100;
            sFadeTimer = Timing.Global.MillisecondsUtc + sFadeRate;
            sFadingOut = false;
        }

        /// <summary>
        /// Stops the current playing music track.
        /// </summary>
        /// <param name="fadeout">The time (in ms) it should take to fade out the current music track.</param>
        public static void StopMusic(int fadeout = 0)
        {
            if (sMyMusic == null)
            {
                return;
            }

            if (fadeout == 0 ||
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
                sFadeRate = fadeout / sMyMusic.GetVolume();
                sFadeTimer = Timing.Global.MillisecondsUtc + sFadeRate;
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
            IEntity parent = null
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

        public static void StopSound(IMapSound sound)
        {
            sound?.Stop();
        }

        public static void StopAllGameSoundsOf(string[] filenames)
        {
            var validSounds = sGameSounds.Where(s => filenames.Contains(s.Filename));
            foreach (var sound in validSounds)
            {
                sound.Stop();
            }
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
