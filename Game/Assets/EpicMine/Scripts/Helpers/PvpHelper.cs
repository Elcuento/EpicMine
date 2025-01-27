using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public static class PvpHelper
    {
        public static bool EndPvpMatch(PvpArenaMatchInfo arena, StaticData data)
        {
            if (arena != null && arena.Status != PvpArenaMatchStatusType.End && arena.Players.Count == 2)
            {
                var playerOne = arena.Players[0];
                var playerTwo = arena.Players[1];


                if (playerTwo == null ||
                    playerOne == null)
                {
                    Debug.Log("null");
                    return true;
                }

                Core.Player playerOneData = null;
                Core.Player playerTwoData = null;

                if (!playerOne.IsBot)
                    playerOneData = App.Instance.Player;

                if (!playerTwo.IsBot)
                    playerTwoData = App.Instance.Player;


                if (playerOneData != null)
                {
                    playerOne.Rating = playerOneData.Pvp.Rating;
                }
                if (playerTwoData != null)
                {
                    playerTwo.Rating = playerTwoData.Pvp.Rating;
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

              //  Debug.Log(playerOne.Leaved +":" + playerTwo.Leaved);
                if (playerOne.Leaved)
                {
                    if (winner == playerOne.Id || winner=="")
                    {
                        winner = playerTwo.Id;
                    }
                }

                if (playerTwo.Leaved)
                {
                    if (winner == playerTwo.Id || winner == "")
                    {
                        winner = playerOne.Id;
                    }
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

                    SetPlayerPvpResults(playerOneData, data, playerOne, playerTwo, playerOneResult);
                    SetPlayerPvpResults(playerTwoData, data, playerTwo, playerOne, playerTwoResult);
                }

                return true;
            }

            return false;

        }

        public static void SetPlayerPvpResults(Core.Player userData, StaticData staticData, PvpArenaUserInfo user, PvpArenaUserInfo opponent, PvpArenaGameResoultType result)
        {
            if (user.IsBot)
            {
                var bot = staticData.PvpBots.Find(x => x.Id == user.Id);
                if (bot != null)
                {
                    if (bot.MaxRatingOffset < user.Rating || bot.MinRatingOffset > user.Rating)
                    {
                        user.Rating = Random.Range(bot.MinRatingOffset, bot.MaxRatingOffset);
                    }
                }
            }
            else
            {
                if (userData != null)
                {
                    userData.Pvp.Games += 1;
                    userData.Pvp._rating = user.Rating;

                    if (result == PvpArenaGameResoultType.Win)
                    {
                        userData.Pvp.Win += 1;

                        userData.Pvp._chests = userData.Pvp.Chests < 5
                            ? userData.Pvp.Chests + 1
                            : 5;
                    }
                    else if (result == PvpArenaGameResoultType.Loose)
                    {
                        userData.Pvp.Loose += 1;
                    }

                    if (userData.Pvp.LastTimePlayed == null)
                    {
                        userData.Pvp.LastTimePlayed = new List<LastTimePlayed>();
                    }

                  /*  if (opponent != null && !string.IsNullOrEmpty(opponent.Name))
                    {
                        var exist = userData.Pvp.LastTimePlayed.Find(x => x.PlayerName == opponent.Name);

                        userData.Pvp.LastTimePlayed.Remove(exist);

                        if (userData.Pvp.LastTimePlayed.Count > 4)
                        {
                            userData.Pvp.LastTimePlayed.Remove(userData.Pvp.LastTimePlayed[0]);
                        }
                        userData.Pvp.LastTimePlayed.Add(new LastTimePlayed(opponent.Name));
                    }*/

                }

            }

            if (!user.IsBot)
            {
                App.Instance.Controllers.RatingsController.UpdateSelf();
            }
        
        }
        public static float ExpectationToWin(float playerOneRating, float playerTwoRating)
        {
            return (float)(1 / (1 + Math.Pow(10, (playerTwoRating - playerOneRating) / 400)));
        }
        public static void CalculateEloRating(PvpArenaUserInfo playerOneRating, PvpArenaUserInfo playerTwoRating, PvpArenaGameResoultType result, StaticData data)
        {
            var eloK = 32;
            var outcomeValue = result == PvpArenaGameResoultType.Win ? 1 : result == PvpArenaGameResoultType.Loose ? 0 : 0.5;

            var delta = (int)((eloK * (outcomeValue - ExpectationToWin(playerOneRating.Rating, playerTwoRating.Rating))));

            if (result == PvpArenaGameResoultType.Draw)
            {
                playerOneRating.Rating -= (int)Math.Round(delta * PvpHelper.GetLeagueByRating(data, (int)playerOneRating.Rating).WinCoefficient);
                playerTwoRating.Rating -= (int)Math.Round(delta * PvpHelper.GetLeagueByRating(data, (int)playerTwoRating.Rating).WinCoefficient);
            }
            else
            {

                Debug.Log(result);
                playerOneRating.Rating += (int)Math.Round(delta * (result == PvpArenaGameResoultType.Win ?
                                                              PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).WinCoefficient
                                                       : PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).LooseCoefficient));


                playerTwoRating.Rating -= (int)Math.Round(delta * (result == PvpArenaGameResoultType.Loose ?
                                                              PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).WinCoefficient
                                                      : PvpHelper.GetLeagueByRating(data, playerOneRating.Rating).LooseCoefficient));

            }

          //  Debug.Log(playerOneRating.Name + "+:" + playerOneRating.Rating.ToString());
           //  Debug.Log(playerTwoRating.Name + "+:" + playerTwoRating.Rating.ToString());

            playerOneRating.Rating = playerOneRating.Rating > 0 ? playerOneRating.Rating : 0;
            playerTwoRating.Rating = playerTwoRating.Rating > 0 ? playerTwoRating.Rating : 0;

            playerOneRating.Rating = playerOneRating.Rating < 11 ? 11 : playerOneRating.Rating;
            playerTwoRating.Rating = playerTwoRating.Rating < 11 ? 11 : playerTwoRating.Rating;
             //   Debug.Log(playerOneRating.Rating.ToString());

              //  Debug.Log(playerOneRating.Name +":" + playerOneRating.Rating.ToString());
              //  Debug.Log(playerTwoRating.Name +":" + playerTwoRating.Rating.ToString());
        }

        public static GameObject GetHeaderAttributePrefab(int league)
        {
            return Resources.Load<GameObject>($"{Paths.ResourcesPrefabsPvpAttributesPath}League{league + 1}");
        }

        public static int GetLeagueByRating(int rating)
        {
            var count = App.Instance.StaticData.Leagues.Count;
            var league = 0;
            for (var i = 0; i < count; i++)
            {
                if (App.Instance.StaticData.Leagues[i].Rating <= rating)
                    league = i;
            }

            return league;
        }

        public static string GetSqlLobby()
        {
            var ratingMax = App.Instance.Player.Pvp.Rating + 200;
            var ratingMin = App.Instance.Player.Pvp.Rating <= 200
                ? 0 : App.Instance.Player.Pvp.Rating - 200;

            var leagueSettingsNumber = GetSuiteSettingsNumber();
            var photonVersion = App.Instance.VersionInfo.PhotonVersion.ToString();

            var sql = $"C0 = \"{leagueSettingsNumber}\" AND C1 BETWEEN  \"{ratingMin}\" AND  \"{ratingMax}\" AND C2 = \"{photonVersion}\" ";
            return sql;
        }

        public static string[] GetLobbyProps()
        {
           return new[]
            {
                "C0",
                "C1",
                "C2"
            };

        }

        public static CommonDLL.Static.Pickaxe GetBotRandomPickAxe(Core.Player player, StaticData data, PvpBot bot)
        {
            var playerSettings =
                GetSuiteSettings(data,
                    player?.Dungeon?.LastOpenedTier?.Number ?? 0);

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

        public static CommonDLL.Static.Torch GetRandomTorch(Core.Player player, StaticData data, PvpBot bot)
        {

            var minLeague = (GetLeagueNumberByRating(data, player?.Pvp?.Rating ?? 0) + 1) - 2;
            minLeague = minLeague < 1 ? 1 : minLeague;

            var all = data.Torches.FindAll(x => x.LeagueId >= minLeague
                                                && x.RequiredFortuneLevel <= (player?.Skills?.Fortune?.Number + 1));
            if (all.Count <= 0)
            {
                return data.Torches.RandomElement();
            }

            return all.RandomElement();
        }

        public static PvpBot GetPvpBotAccordingLeague(Core.Player player, StaticData data)
        {
            var bots = data.PvpBots.FindAll(x => player?.Pvp?.Rating >= x.MinRatingOffset &&
                                                 player?.Pvp?.Rating <= x.MaxRatingOffset);
            if (bots.Count == 0)
            {
                return data.PvpBots.Last();
            }

            return bots.RandomElement();
        }

        public static int GetSuiteSettingsNumber()
        {
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

            if (tier <= 1)
                tier = 2;

            var leagueSettings =
                App.Instance.StaticData.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                App.Instance.StaticData.LeaguesSettings.Last();

            return App.Instance.StaticData.LeaguesSettings.IndexOf(leagueSettings);
        }

        public static LeaguesSettings GetSuiteSettings()
        {
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

            if (tier <= 1)
                tier = 2;

            var leagueSettings =
                App.Instance.StaticData.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                App.Instance.StaticData.LeaguesSettings.Last();

            return leagueSettings;
        }

        public static int[] GetWallsHealths(int leagueNumber, int count)
        {
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

            if (tier <= 1)
                tier = 2;

            var wallHealths = new int[count];

            var leagueSettings =
                App.Instance.StaticData.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                App.Instance.StaticData.LeaguesSettings.Last();

            var league = App.Instance.StaticData.Leagues[leagueNumber];

            var averageHealth = leagueSettings.AverageWallHealth[leagueNumber];

            for (var i = 0; i < wallHealths.Length; ++i)
            {
                var wallSettings = App.Instance.StaticData.MineWalls[i];
                wallHealths[i] = (int) (averageHealth * wallSettings.HealthCoefficient * league.Coefficient);
            }

            return wallHealths;
        }

        public static Color GetLeagueTextColor(int league)
        {
            return App.Instance.ReferencesTables.Colors.LeagueTextColors[league];
        }

        public static Color GetLeagueFlagColor(int league)
        {
            return App.Instance.ReferencesTables.Colors.LeagueHeaderColors[league];
        }

        public static Color GetLeagueWallNumberColor(int league)
        {
            return App.Instance.ReferencesTables.Colors.LeagueWallNumberColors[league];
        }

        public static int GetLeagueNumberByRating(StaticData data, int rating)
        {
            var count = data.Leagues.Count;
            var league = 0;
            for (var i = 0; i < count; i++)
            {
                if (data.Leagues[i].Rating <= rating)
                    league = i;
            }

            return league;
        }

        public static League GetLeagueByRating(StaticData data, int rating)
        {
            var count = data.Leagues.Count;
            var league = data.Leagues[0];
            for (var i = 0; i < count; i++)
            {
                if (data.Leagues[i].Rating <= rating)
                    league = data.Leagues[i];
            }

            return league;
        }

        public static List<int> GetWallsHealths(StaticData data, int lastOpenTier, int leagueNumber, int count)
        {
            var tier = lastOpenTier + 1;

            if (tier <= 1)
                tier = 2;

            var wallHealths = new int[count];

            var leagueSettings =
                data.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                data.LeaguesSettings.Last();

            var league = data.Leagues[leagueNumber];

            var averageHealth = leagueSettings.AverageWallHealth[leagueNumber];

            for (var i = 0; i < wallHealths.Length; ++i)
            {
                var wallSettings = data.MineWalls[i];
                wallHealths[i] = (int)(averageHealth * wallSettings.HealthCoefficient * league.Coefficient);
            }

            return wallHealths.ToList();
        }

        public static LeaguesSettings GetSuiteSettings(StaticData data, int lastOpenTier)
        {
            var tier = lastOpenTier + 1;

            if (tier <= 1)
                tier = 2;

            var leagueSettings =
                data.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                data.LeaguesSettings.Last();

            return leagueSettings;
        }



        public static bool SuiteRating(PvpArenaMatchInfo pvpArenaMatch, Player playerData)
        {
            if (playerData.Pvp.Rating >= pvpArenaMatch.Filter.MinRating &&
                playerData.Pvp.Rating <= pvpArenaMatch.Filter.MaxRating &&
                playerData.Prestige == pvpArenaMatch.Filter.Prestige)
                return true;

            return false;
        }

        public static bool SuiteFilter(PvpArenaMatchFilter filter, PvpArenaMatchFilter arenaDataFilter)
        {

            return Math.Abs(Math.Abs(filter.MinRating) - Math.Abs(arenaDataFilter.MinRating)) < 200 &&
                   Math.Abs(Math.Abs(filter.MaxRating) - Math.Abs(arenaDataFilter.MaxRating)) < 200 &&
                   filter.LeagueSettingsNumber == arenaDataFilter.LeagueSettingsNumber &&
                   filter.Prestige == arenaDataFilter.Prestige;
        }
    }
}