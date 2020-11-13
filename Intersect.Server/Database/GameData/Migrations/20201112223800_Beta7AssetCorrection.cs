using Intersect.GameObjects.Events.Commands;
using Intersect.Server.Database.Migration;
using Intersect.Server.Database.PlayerData;

using System.Linq;

namespace Intersect.Server.Database.GameData.Migrations
{
    public class Beta7AssetCorrection<TContext> : DataMigration<TContext> where TContext : IntersectDbContext<TContext>
    {
        protected string CleanMusicName(string textureName) => textureName?.Replace(".ogg", "");

        protected string CleanSoundName(string textureName) => textureName?.Replace(".wav", "");

        protected string CleanTextureName(string textureName) => textureName?.Replace(".png", "");
    }

    [DataMigration("20201112223800_Beta7AssetCorrection")]
    internal class Beta7AssetCorrectionGameContext : Beta7AssetCorrection<GameContext>
    {
        public override bool Up(GameContext context)
        {
            var result = context.Animations.ToList()
                .All(
                    animation =>
                    {
                        animation.Sound = CleanSoundName(animation.Sound);
                        animation.Lower.Sprite = CleanTextureName(animation.Lower.Sprite);
                        animation.Upper.Sprite = CleanTextureName(animation.Upper.Sprite);
                        return true;
                    }
                );

            result = result &&
                     context.Classes.ToList()
                         .All(
                             _class =>
                             {
                                 _class.Sprites.ForEach(
                                     sprite =>
                                     {
                                         sprite.Face = CleanTextureName(sprite.Face);
                                         sprite.Sprite = CleanTextureName(sprite.Sprite);
                                     }
                                 );

                                 return true;
                             }
                         );

            result = result &&
                     context.Events.ToList()
                         .All(
                             _event =>
                             {
                                 _event.Pages.ForEach(
                                     page =>
                                     {
                                         page.FaceGraphic = CleanTextureName(page.FaceGraphic);
                                         if (page.Graphic != null)
                                         {
                                             page.Graphic.Filename = CleanTextureName(page.Graphic?.Filename);
                                         }

                                         foreach (var commandList in page.CommandLists.Values)
                                         {
                                             foreach (var command in commandList)
                                             {
                                                 switch (command)
                                                 {
                                                     case ChangeFaceCommand changeFaceCommand:
                                                         changeFaceCommand.Face =
                                                             CleanTextureName(changeFaceCommand.Face);

                                                         break;

                                                     case ChangeSpriteCommand changeSpriteCommand:
                                                         changeSpriteCommand.Sprite =
                                                             CleanTextureName(changeSpriteCommand.Sprite);

                                                         break;

                                                     case PlayBgmCommand playBgmCommand:
                                                         playBgmCommand.File = CleanMusicName(playBgmCommand.File);
                                                         break;

                                                     case PlaySoundCommand playSoundCommand:
                                                         playSoundCommand.File = CleanSoundName(playSoundCommand.File);
                                                         break;

                                                     case SetMoveRouteCommand setMoveRouteCommand:
                                                         if ((setMoveRouteCommand.Route?.Actions?.Count ?? 0) > 0)
                                                         {
                                                             foreach (var moveRouteAction in setMoveRouteCommand.Route
                                                                 .Actions)
                                                             {
                                                                 if (moveRouteAction.Graphic != null)
                                                                 {
                                                                     moveRouteAction.Graphic.Filename =
                                                                         CleanTextureName(
                                                                             moveRouteAction.Graphic.Filename
                                                                         );
                                                                 }
                                                             }
                                                         }

                                                         break;

                                                     case ShowOptionsCommand showOptionsCommand:
                                                         showOptionsCommand.Face =
                                                             CleanTextureName(showOptionsCommand.Face);

                                                         break;

                                                     case ShowPictureCommand showPictureCommand:
                                                         showPictureCommand.File =
                                                             CleanTextureName(showPictureCommand.File);

                                                         break;

                                                     case ShowTextCommand showTextCommand:
                                                         showTextCommand.Face = CleanTextureName(showTextCommand.Face);
                                                         break;
                                                 }
                                             }
                                         }
                                     }
                                 );

                                 return true;
                             }
                         );

            result = result &&
                     context.Items.ToList()
                         .All(
                             item =>
                             {
                                 item.FemalePaperdoll = CleanTextureName(item.FemalePaperdoll);
                                 item.Icon = CleanTextureName(item.Icon);
                                 item.MalePaperdoll = CleanTextureName(item.MalePaperdoll);
                                 return true;
                             }
                         );

            result = result &&
                     context.Maps.ToList()
                         .All(
                             map =>
                             {
                                 map.Fog = CleanTextureName(map.Fog);
                                 map.Music = CleanMusicName(map.Music);
                                 map.Sound = CleanSoundName(map.Sound);
                                 map.OverlayGraphic = CleanTextureName(map.OverlayGraphic);
                                 map.Panorama = CleanTextureName(map.Panorama);
                                 return true;
                             }
                         );

            result = result &&
                     context.Npcs.ToList()
                         .All(
                             npc =>
                             {
                                 npc.Sprite = CleanTextureName(npc.Sprite);
                                 return true;
                             }
                         );

            result = result &&
                     context.Resources.ToList()
                         .All(
                             resource =>
                             {
                                 if (resource.Initial != null)
                                 {
                                     resource.Initial.Graphic = CleanTextureName(resource.Initial.Graphic);
                                 }

                                 if (resource.Exhausted != null)
                                 {
                                     resource.Exhausted.Graphic = CleanTextureName(resource.Exhausted.Graphic);
                                 }

                                 return true;
                             }
                         );

            result = result &&
                     context.Spells.ToList()
                         .All(
                             spell =>
                             {
                                 spell.Icon = CleanTextureName(spell.Icon);
                                 return true;
                             }
                         );

            result = result &&
                     context.Tilesets.ToList()
                         .All(
                             tileset =>
                             {
                                 tileset.Name = CleanTextureName(tileset.Name);
                                 return true;
                             }
                         );

            return result && context.SaveChanges() > 0;
        }
    }

    [DataMigration("20201112223800_Beta7AssetCorrection")]
    internal class Beta7AssetCorrectionPlayerContext : Beta7AssetCorrection<PlayerContext>
    {
        public override bool Up(PlayerContext context) =>
            context.Players.ToList()
                .All(
                    player =>
                    {
                        player.Face = CleanTextureName(player.Face);
                        player.Sprite = CleanTextureName(player.Sprite);
                        return true;
                    }
                );
    }
}
