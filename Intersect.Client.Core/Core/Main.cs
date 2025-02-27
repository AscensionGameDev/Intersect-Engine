using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.GameObjects;

// ReSharper disable All

namespace Intersect.Client.Core;

internal static partial class Main
{

    private static long _animationTimer;

    private static bool _loadedTilesets;

    internal static void Start(IClientContext context)
    {
        //Load Graphics
        Graphics.InitGraphics();

        //Load Sounds
        Audio.Init();
        Audio.PlayMusic(ClientConfiguration.Instance.MenuMusic, ClientConfiguration.Instance.MusicFadeTimer, ClientConfiguration.Instance.MusicFadeTimer, true);

        //Init Network
        Networking.Network.InitNetwork(context);
        Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);

        //Make Json.Net Familiar with Our Object Types
        var id = Guid.NewGuid();
        foreach (var val in Enum.GetValues(typeof(GameObjectType)))
        {
            var type = ((GameObjectType) val);
            if (type is not GameObjectType.Event and not GameObjectType.Time)
            {
                var lookup = type.GetLookup();
                var item = lookup.AddNew(type.GetObjectType(), id);
                item.Load(item.JsonData);
                _ = lookup.Delete(item);
            }
        }
    }

    public static void DestroyGame()
    {
        //Destroy Game
        //TODO - Destroy Graphics and Networking peacefully
        //Network.Close();
        Interface.Interface.DestroyGwen(true);
        Graphics.Renderer?.Close();
    }

    public static void Update(TimeSpan deltaTime)
    {
        lock (Globals.GameLock)
        {
            Networking.Network.Update();
            Fade.Update();
            Interface.Interface.SetHandleInput(Globals.GameState != GameStates.Intro);

            switch (Globals.GameState)
            {
                case GameStates.Intro:
                    ProcessIntro();

                    break;

                case GameStates.Menu:
                    ProcessMenu();

                    break;

                case GameStates.Loading:
                    ProcessLoading();

                    break;

                case GameStates.InGame:
                    ProcessGame();

                    break;

                case GameStates.Error:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(Globals.GameState), $"Value {Globals.GameState} out of range."
                    );
            }

            Globals.InputManager.Update(deltaTime);
            Audio.Update();

            Globals.OnGameUpdate(deltaTime);
        }
    }

    private static void ProcessIntro()
    {
        if (ClientConfiguration.Instance.IntroImages.Count > 0)
        {
            IGameTexture imageTex = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Image, ClientConfiguration.Instance.IntroImages[Globals.IntroIndex]
            );

            if (imageTex != null)
            {
                if (Globals.IntroStartTime == -1)
                {
                    if (Fade.DoneFading())
                    {
                        if (Globals.IntroComing)
                        {
                            Globals.IntroStartTime = Timing.Global.MillisecondsUtc;
                        }
                        else
                        {
                            Globals.IntroIndex++;
                            Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
                            Globals.IntroComing = true;
                        }
                    }
                }
                else
                {
                    if (Timing.Global.MillisecondsUtc > Globals.IntroStartTime + Globals.IntroDelay)
                    {
                        //If we have shown an image long enough, fade to black -- keep track that the image is going
                        Fade.FadeOut(ClientConfiguration.Instance.FadeDurationMs);
                        Globals.IntroStartTime = -1;
                        Globals.IntroComing = false;
                    }
                }
            }
            else
            {
                Globals.IntroIndex++;
            }

            if (Globals.IntroIndex >= ClientConfiguration.Instance.IntroImages.Count)
            {
                Globals.GameState = GameStates.Menu;
            }
        }
        else
        {
            Globals.GameState = GameStates.Menu;
        }
    }

    private static void ProcessMenu()
    {
        if (!Globals.JoiningGame)
            return;

        //if (GameGraphics.FadeAmt != 255f) return;
        //Check if maps are loaded and ready
        Globals.GameState = GameStates.Loading;
        Interface.Interface.DestroyGwen();
    }

    private static void ProcessLoading()
    {
        if (Globals.Me == null || Globals.Me.MapInstance == null)
            return;

        if (!_loadedTilesets && Globals.HasGameData)
        {
            Globals.ContentManager.LoadTilesets(TilesetDescriptor.GetNameList());
            _loadedTilesets = true;
        }

        Audio.PlayMusic(MapInstance.Get(Globals.Me.MapId).Music, ClientConfiguration.Instance.MusicFadeTimer, ClientConfiguration.Instance.MusicFadeTimer, true);
        Globals.GameState = GameStates.InGame;
        Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
    }

    private static void ProcessGame()
    {
        if (Globals.ConnectionLost)
        {
            Logout(false);
            Globals.ConnectionLost = false;
            return;
        }

        //If we are waiting on maps, lets see if we have them
        if (Globals.NeedsMaps)
        {
            Globals.NeedsMaps = false;
            //Send ping to server, so it will resync time if needed as we load in
            PacketSender.SendPing();
        }
        else
        {
            PacketSender.SendNeedMapForGrid();
        }

        if (!Globals.NeedsMaps)
        {
            //Update All Entities
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null)
                    continue;

                _ = en.Value.Update();
            }

            for (int i = 0; i < Globals.EntitiesToDispose.Count; i++)
            {
                if (Globals.Entities.TryGetValue(Globals.EntitiesToDispose[i], out var value))
                {
                    if (Globals.EntitiesToDispose[i] == Globals.Me?.Id)
                        continue;
                    value.Dispose();
                    _ = Globals.Entities.Remove(Globals.EntitiesToDispose[i]);
                }
            }

            Globals.EntitiesToDispose.Clear();

            //Update Maps
            var maps = MapInstance.Lookup.Values.ToArray();
            foreach (MapInstance map in maps.Cast<MapInstance>())
            {
                if (map == null)
                    continue;

                map.Update(map.InView());
            }
        }

        var millisecondsNow = Timing.Global.MillisecondsUtc;

        //Update Game Animations
        if (_animationTimer < millisecondsNow)
        {
            Globals.AnimationFrame = (Globals.AnimationFrame + 1) % 3;

            _animationTimer = millisecondsNow + 500;
        }

        //Remove Event Holds If Invalid
        var eventHoldIdsToRemove = new List<Guid>();
        foreach (var (holdId, mapId) in Globals.EventHolds)
        {
            // If mapId is empty its a common event, ignore. Otherwise make sure we have the map else the hold doesnt matter
            if (mapId == default || !MapInstance.TryGet(mapId, out _))
            {
                continue;
            }

            eventHoldIdsToRemove.Add(holdId);
        }

        foreach (var eventHoldId in eventHoldIdsToRemove)
        {
            _ = Globals.EventHolds.Remove(eventHoldId);
        }

        Graphics.UpdatePlayerLight();
        Time.Update();
    }

    public static void JoinGame()
    {
        Globals.LoggedIn = true;
        Audio.StopMusic(ClientConfiguration.Instance.MusicFadeTimer);
    }

    public static void Logout(bool characterSelect, bool skipFade = false)
    {
        Audio.PlayMusic(ClientConfiguration.Instance.MenuMusic, ClientConfiguration.Instance.MusicFadeTimer, ClientConfiguration.Instance.MusicFadeTimer, true);
        if (skipFade)
        {
            Fade.Cancel();
        }
        else
        {
            Fade.FadeOut(ClientConfiguration.Instance.FadeDurationMs);
        }

        if (!ClientContext.IsSinglePlayer)
        {
            Globals.SoftLogout = !characterSelect;
            PacketSender.SendLogout(characterSelect);
        }

        Globals.LoggedIn = false;
        Globals.WaitingOnServer = false;
        Globals.GameState = GameStates.Menu;
        Globals.JoiningGame = false;
        Globals.NeedsMaps = true;
        Globals.Picture = null;

        Globals.InBag = false;
        Globals.InBank = false;
        Globals.GameShop = null;
        Globals.InTrade = false;
        Globals.EventDialogs?.Clear();
        Globals.InCraft = false;

        Interface.Interface.HideUi = false;

        //Dump Game Objects
        Globals.Me = null;
        Globals.HasGameData = false;
        foreach (var map in MapInstance.Lookup)
        {
            var mp = (MapInstance) map.Value;
            mp.Dispose(false, true);
        }

        foreach (var en in Globals.Entities.ToArray())
        {
            en.Value.Dispose();
        }

        MapDescriptor.Lookup.Clear();
        MapInstance.Lookup.Clear();

        Globals.Entities.Clear();
        Globals.MapGrid = null;
        Globals.GridMaps.Clear();
        Globals.EventDialogs?.Clear();
        Globals.EventHolds.Clear();
        Globals.PendingEvents.Clear();

        Interface.Interface.InitGwen();

        if (ClientContext.IsSinglePlayer)
        {
            PacketSender.SendLogout(characterSelect);
        }

        if (skipFade)
        {
            Fade.Cancel();
        }
        else
        {
            Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
        }
    }
}
