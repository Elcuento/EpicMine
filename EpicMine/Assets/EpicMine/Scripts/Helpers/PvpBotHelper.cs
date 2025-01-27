using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public static class PvpBotHelper
    {
     
        public static int GetRandomRating(PvpBot bot)
        {
            return Random.Range(bot.MinRatingOffset, bot.MaxRatingOffset);
        }

        public static int GetRatingAfterCameToLeagueCorner(PvpBot bot)
        {
            return GetRandomRating(bot);
        }

        public static string GetRandomBotName(PvpBot bot)
        {
            var name = bot.Names.RandomElement();
            return name;
        }

        public static int GetSectionPassSpeed(PvpBot bot, Pickaxe equipPickAxe)
        {
            return (int) (Random.Range(bot.BreakWallTimeMin, bot.BreakWallTimeMax) *
                          (equipPickAxe.Type == PickaxeType.Donate ? PvpLocalConfig.BotDonatePickaxeExtraDamage : 1));
        }
        public static int GetSectionPassSpeedAccordingStatistic(int passTime, Pickaxe equipPickAxe)
        {
            return (int)(Random.Range(passTime * 0.5f, passTime * 1.5f) *
                         (equipPickAxe.Type == PickaxeType.Donate ? PvpLocalConfig.BotDonatePickaxeExtraDamage : 1));
        }

        public static PvpBot GetPvpBotAccordingLeague()
        {
            var bots = App.Instance.StaticData.PvpBots.FindAll(x => App.Instance.Player.Pvp.Rating >= x.MinRatingOffset &&
                                                                    App.Instance.Player.Pvp.Rating <= x.MaxRatingOffset);
            if (bots.Count == 0)
            {
                App.Instance.Services.LogService.Log("not bots, taking last on");
                return App.Instance.StaticData.PvpBots.Last();
            }

            return bots.RandomElement();
        }

        public static Pickaxe GetRandomPickAxe(PvpBot bot)
        {
            var playerSettings = PvpHelper.GetSuiteSettings();

            var all = App.Instance.StaticData.Pickaxes.FindAll(x => x.Type != PickaxeType.Mythical &&
                                                                    (x.RequiredTierNumber <= playerSettings.TierMax &&
                                                                     x.RequiredTierNumber >= playerSettings.TierMin));

            if (all.Count <= 0)
            {
                App.Instance.Services.LogService.LogWarning("What's wrong pick?");

                all = App.Instance.StaticData.Pickaxes.FindAll(x =>
                    x.Type != PickaxeType.Mythical && x.Type != PickaxeType.Donate && x.Type != PickaxeType.God);

                return all.RandomElement();
            }

            return all.RandomElement();
        }

        public static Torch GetRandomTorch(PvpBot bot)
        {
            var minLeague = (App.Instance.Player.Pvp.CurrentLeagueLvl + 1) - 2;
            minLeague = minLeague < 1 ? 1 : minLeague;

            var all = App.Instance.StaticData.Torches.FindAll(x => x.LeagueId >= minLeague 
                                                                   &&  x.RequiredFortuneLevel <= (App.Instance.Player.Skills.Fortune.Number + 1));
            if (all.Count <= 0)
            {
                App.Instance.Services.LogService.LogWarning("What's wrong torch ?");

                return App.Instance.StaticData.Torches.RandomElement();
            }

            return all.RandomElement();
        }

    }
}