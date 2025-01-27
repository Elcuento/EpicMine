using System;
using System.Collections.Generic;
using System.Linq;
using AMTServer.Common;
using AMTServer.Core;
using CommonDLL.Dto;
using CommonDLL.Static;
using MongoDB.Driver;
using Player = AMTServer.Dto.Player;


namespace AMTServer.Helpers
{
    public static class PvpHelper
    {
        public static bool EndPvpMatch(DataBaseLinks links, PvpArenaMatchInfo arena, StaticData data)
        {
            if (arena != null && arena.Status != PvpArenaMatchStatusType.End && arena.Players.Count == 2)
            {
                /*if (arena.EndTime > Utils.GetUnixTime() &&
                    arena.Players.Find(x=>x.Walls == 10) == null)
                    return false;*/

                var playerOne = arena.Players[0];
                var playerTwo = arena.Players[1];


                if (playerTwo == null ||
                    playerOne == null)
                {
                    Console.WriteLine("null");
                    return true;
                }

                Player playerOneData = null;
                Player playerTwoData = null;

                if(!playerOne.IsBot)
                    playerOneData = links.UserCollection.FindSync(x => x.Data.Id == playerOne.Id).FirstOrDefault();

                if (!playerTwo.IsBot)
                    playerTwoData = links.UserCollection.FindSync(x => x.Data.Id == playerTwo.Id).FirstOrDefault();


                if (playerOneData != null)
                {
                    playerOne.Rating = playerOneData.Data.Pvp.Rating;
                }
                if (playerTwoData != null)
                {
                    playerTwo.Rating = playerTwoData.Data.Pvp.Rating;
                }

                var winner = "";


                if (arena.IsConfirmed(playerOne.Id) && arena.IsConfirmed(playerTwo.Id) || !arena.IsConfirmed(playerOne.Id) && !arena.IsConfirmed(playerTwo.Id))
                {
                    winner = playerOne.Walls > playerTwo.Walls ? playerOne.Id :
                        playerOne.Walls == playerTwo.Walls ? "" : playerTwo.Id;
                }
                if (!arena.IsConfirmed(playerOne.Id) && arena.IsConfirmed(playerTwo.Id))
                {
                    winner = playerOne.Id;
                }
                else if (!arena.IsConfirmed(playerTwo.Id) && arena.IsConfirmed(playerOne.Id))
                {
                    winner = playerTwo.Id;
                }

                var playerOneResult =
                    winner == playerOne.Id ? PvpArenaGameResoultType.Win
                    : winner == "" ? PvpArenaGameResoultType.Draw : PvpArenaGameResoultType.Loose;

                var playerTwoResult =
                    winner == playerTwo.Id ? PvpArenaGameResoultType.Win
                    : winner == "" ? PvpArenaGameResoultType.Draw : PvpArenaGameResoultType.Loose;


                arena.SetResult(new PvpArenaMatchResult(
                    new Dictionary<string, PvpArenaGameResoultType>
                    {
                        {playerOne.Id, playerOneResult},
                        {playerTwo.Id, playerTwoResult},
                    }));


                if (arena.Type == PvpArenaMatchType.RandomMatch)
                {
                    CalculateEloRating(playerOne, playerTwo, playerOneResult, data);

                    SetPlayerPvpResults(playerOneData, links, data, playerOne, playerTwo, playerOneResult);
                    SetPlayerPvpResults(playerTwoData, links, data, playerTwo, playerOne, playerTwoResult);
                }

                return true;
            }

            return false;

        }

        public static void SetPlayerPvpResults(Player userData, DataBaseLinks links, StaticData staticData, PvpArenaUserInfo user, PvpArenaUserInfo opponent, PvpArenaGameResoultType result)
        {
            if (user.IsBot)
            {
                var bot = staticData.PvpBots.Find(x => x.Id == user.Id);
                if (bot != null)
                {
                    if (bot.MaxRatingOffset < user.Rating || bot.MinRatingOffset > user.Rating)
                    {
                        user.Rating = new Random().Next(bot.MinRatingOffset, bot.MaxRatingOffset);
                    }
                }
            }
            else
            {
                if (userData != null)
                {
                    userData.Data.Pvp.Games += 1;
                    userData.Data.Pvp.Rating = user.Rating;

                    if (result == PvpArenaGameResoultType.Win)
                    {
                        userData.Data.Pvp.Win += 1;

                        userData.Data.Pvp.Chests = userData.Data.Pvp.Chests < 5
                            ? userData.Data.Pvp.Chests + 1
                            : 5;
                    }
                    else if (result == PvpArenaGameResoultType.Loose)
                    {
                        userData.Data.Pvp.Loose += 1;
                    }

                    if (userData.Data.Pvp.LastTimePlayed == null)
                    {
                        userData.Data.Pvp.LastTimePlayed = new List<LastTimePlayed>();
                    }

                    if (opponent != null && !string.IsNullOrEmpty(opponent.Name))
                    {
                        var exist = userData.Data.Pvp.LastTimePlayed.Find(x => x.PlayerName == opponent.Name);

                        userData.Data.Pvp.LastTimePlayed.Remove(exist);

                        if (userData.Data.Pvp.LastTimePlayed.Count > 4)
                        {
                            userData.Data.Pvp.LastTimePlayed.Remove(userData.Data.Pvp.LastTimePlayed[0]);
                        }
                        userData.Data.Pvp.LastTimePlayed.Add(new LastTimePlayed(opponent.Name));
                    }


                    links.UserCollection.ReplaceOne(x => x.Id == userData.Id, userData);
                }

            }

            links.UserPvpRatingCollection.DeleteOne(x => x.Rating.UserId == user.Id);
            links.UserPvpRatingCollection.InsertOne(new Dto.PlayerPvpRating(
                new CommonDLL.Dto.PlayerPvpRating
                {
                    Rating = user.Rating,
                    UserId = user.Id,
                    UserLocalate = user.Localate,
                    IsBot = user.IsBot,
                    UserNick = user.Name,
                    League =
                        EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueNumberByRating(staticData, user.Rating),
                }));
        }

        public static void CalculateEloRating(PvpArenaUserInfo playerOneRating, PvpArenaUserInfo playerTwoRating, PvpArenaGameResoultType result, StaticData data)
        {
            var eloK = 32;
            var outcomeValue = result == PvpArenaGameResoultType.Win ? 1 : result == PvpArenaGameResoultType.Loose ? 0 : 0.5;

            var delta = (int)((eloK * (outcomeValue - ExpectationToWin(playerOneRating.Rating, playerTwoRating.Rating))));

            if (result == PvpArenaGameResoultType.Draw)
            {
                playerOneRating.Rating -= (int)Math.Round(delta * EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, (int)playerOneRating.Rating).WinCoefficient);
                playerTwoRating.Rating -= (int)Math.Round(delta * EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, (int)playerTwoRating.Rating).WinCoefficient);
            }
            else
            {
                playerOneRating.Rating += (int)Math.Round(delta * (result == PvpArenaGameResoultType.Win ?
                                                              EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).WinCoefficient
                                                       : EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).LooseCoefficient));


                playerTwoRating.Rating -= (int)Math.Round(delta * (result == PvpArenaGameResoultType.Loose ?
                                                              EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).WinCoefficient
                                                      : EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).LooseCoefficient));

            }

           // LogSystem.Log(playerOneRating.Name + "+:" + playerOneRating.Rating.ToString());
           // LogSystem.Log(playerTwoRating.Name + "+:" + playerTwoRating.Rating.ToString());

            playerOneRating.Rating = playerOneRating.Rating > 0 ? playerOneRating.Rating : 0;
            playerTwoRating.Rating = playerTwoRating.Rating > 0 ? playerTwoRating.Rating : 0;

            playerOneRating.Rating = playerOneRating.Rating < 11 ? 11 : playerOneRating.Rating;
            playerTwoRating.Rating = playerTwoRating.Rating < 11 ? 11 : playerTwoRating.Rating;
        //    LogSystem.Log(playerOneRating.Rating.ToString());

            // LogSystem.Log(playerOneRating.Name +":" + playerOneRating.Rating.ToString());
            //  LogSystem.Log(playerTwoRating.Name +":" + playerTwoRating.Rating.ToString());
        }

        public static float ExpectationToWin(float playerOneRating, float playerTwoRating)
        {
            return (float)(1 / (1 + Math.Pow(10, (playerTwoRating - playerOneRating) / 400)));
        }



        public static PvpBot GetPvpBotAccordingLeague(Player player, StaticData data)
        {
            var bots = data.PvpBots.FindAll(x => player.Data?.Pvp?.Rating >= x.MinRatingOffset &&
                                                 player.Data?.Pvp?.Rating <= x.MaxRatingOffset);
            if (bots.Count == 0)
            {
                return data.PvpBots.Last();
            }

            return bots.RandomElement();
        }

        public static CommonDLL.Static.Pickaxe GetBotRandomPickAxe(Player player, StaticData data, PvpBot bot)
        {
            var playerSettings =
                EpicMineServerDLL.Static.Helpers.PvpHelper.GetSuiteSettings(data,
                    player.Data?.Dungeon?.LastOpenedTier?.Number ?? 0);

            var all = data.Pickaxes.FindAll(x => x.Type != PickaxeType.Mythical &&
                                                                    (x.RequiredTierNumber <= playerSettings.TierMax &&
                                                                     x.RequiredTierNumber >= playerSettings.TierMin));

            if (all.Count <= 0)
            {
               all = data.Pickaxes.FindAll(x =>
                    x.Type != PickaxeType.Mythical && x.Type != PickaxeType.Donate && x.Type != PickaxeType.God);

                return all.RandomElement();
            }

            return all.RandomElement();
        }

        public static CommonDLL.Static.Torch GetRandomTorch(Player player, StaticData data, PvpBot bot)
        {
            
            var minLeague = (EpicMineServerDLL.Static.Helpers.PvpHelper.GetLeagueNumberByRating(data, player.Data?.Pvp?.Rating ?? 0) + 1) - 2;
            minLeague = minLeague < 1 ? 1 : minLeague;

            var all = data.Torches.FindAll(x => x.LeagueId >= minLeague
                                                                   && x.RequiredFortuneLevel <= (player.Data?.Skills?.Fortune?.Number + 1));
            if (all.Count <= 0)
            {
                return data.Torches.RandomElement();
            }

            return all.RandomElement();
        }
    }
}
