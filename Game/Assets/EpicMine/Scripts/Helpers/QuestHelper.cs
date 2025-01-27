using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using QuestTaskGoal = CommonDLL.Static.QuestTaskGoal;


namespace BlackTemple.EpicMine
{
    public static class QuestHelper
    {

        public static List<Core.Quest> GetVillageQuests()
        {
            var questList = new List<Core.Quest>();

            for(var i = 0; i < App.Instance.Player.Quests.QuestList.Count; i ++)
            {
                if (App.Instance.Player.Quests.QuestList[i].Status == QuestStatusType.Activated)
                    continue;

                if(App.Instance.Player.Quests.QuestList[i].StaticQuest.StartTrigger.Key == QuestTriggerExecuter.Character)
                    questList.Add(App.Instance.Player.Quests.QuestList[i]);
            }

            return questList;
        }

        public static bool IsVillageHasQuests()
        {
            for (var index = 0; index < App.Instance.Player.Quests.QuestList.Count; index++)
            {
                var playerQuest = App.Instance.Player.Quests.QuestList[index];
                if (playerQuest.Status != QuestStatusType.Activated)
                    continue;

                if (playerQuest.StaticQuest.StartTrigger.Key == QuestTriggerExecuter.Character)
                {
                    return true;
                }
            }

            return false;
        }


        public static bool GetAnyOneMineQuest()
        {
            for (var index = 0; index < App.Instance.Player.Quests.QuestList.Count; index++)
            {
                var playerQuest = App.Instance.Player.Quests.QuestList[index];
                if (playerQuest.Status != QuestStatusType.Activated)
                    continue;

                for (var i = 0; i < playerQuest.TaskList.Count; i++)
                {
                    var questTask = playerQuest.TaskList[i];
                    if (questTask.IsCompleted || questTask.IsReady)
                        continue;

                    for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                    {
                        var goal = questTask.GoalsList[index1];
                        if (goal.IsCompleted)
                            continue;

                        if (goal.StaticGoal.Type == QuestTaskType.Kill ||
                            goal.StaticGoal.Type == QuestTaskType.Collect)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static List<Core.Quest> GetMineQuests(int tier, int mine)
        {
            var questList = new List<Core.Quest>();

            if (mine == 5)
            {
                return questList;
            }

            var activeQuests = App.Instance.Player.Quests.QuestList.FindAll(x => x.Status == QuestStatusType.Started);

            var monsterSpawn = App.Instance.StaticData.MineMonstersSpawn[tier]
                .SpawnChances[mine]
                .Chance;

            for (var index = 0; index < activeQuests.Count; index++)
            {
                var activeQuest = activeQuests[index];
                var exit = false;
                for (var i = 0; i < activeQuest.TaskList.Count; i++)
                {
                    var questTask = activeQuest.TaskList[i];
                    if (exit || !questTask.IsOpen)
                        break;

                    for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                    {
                        var questTaskGoal = questTask.GoalsList[index1];
                        if (exit)
                            break;

                        if (questTaskGoal.IsCompleted)
                            continue;

                        switch (questTaskGoal.StaticGoal.Type)
                        {
                            case QuestTaskType.Collect:
                                var key = questTaskGoal.StaticGoal.Goal.Key;

                                foreach (var monster in monsterSpawn)
                                {
                                    var monsterData = App.Instance.StaticData.Monsters.Find(x => x.Id == monster.Key);
                                    if (monsterData.DropItems.Find(x => x.Id == key) != null)
                                    {
                                        questList.Add(activeQuest);
                                        exit = true;
                                        break;
                                    }

                                    var extraIdDrops = App.Instance.StaticData.QuestTaskGoalsCollect.FindAll(
                                        x => (x.SourceType == QuestTaskGoalCollectSourceType.MonsterType &&
                                              x.Source == monsterData.Type.ToString() && x.Id == key) ||
                                             (x.SourceType == QuestTaskGoalCollectSourceType.MonsterId &&
                                              x.Source == monsterData.Id && x.Id == key));

                                    for (var i1 = 0; i1 < extraIdDrops.Count; i1++)
                                    {
                                        questList.Add(activeQuest);
                                        exit = true;
                                        break;
                                    }
                                }

                                if (App.Instance.StaticData.Tiers[tier].WallItem1Id == key
                                    || App.Instance.StaticData.Tiers[tier].WallItem2Id == key
                                    || App.Instance.StaticData.Tiers[tier].WallItem3Id == key)
                                {
                                    questList.Add(activeQuest);
                                    exit = true;
                                    break;
                                }

                                break;
                            case QuestTaskType.Kill:
                                var key2 = questTaskGoal.StaticGoal.Goal.Key;

                                foreach (var monster in monsterSpawn)
                                {
                                    if (monster.Key == key2)
                                    {
                                        questList.Add(activeQuest);
                                        exit = true;
                                        break;
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            return questList;
        }


        public static string GetGoalTitle(QuestTaskGoal goal)
        {
            var text = "";

            if (LocalizationHelper.IsLocaleExist(goal.Id))
            {
                return LocalizationHelper.GetLocale(goal.Id);
            }



            return text;
        }

        public static void ExtractQuestReward(CommonDLL.Static.Quest quest)
        {
            if (quest == null)
                return;

            var itemsAdd = new List<Item>();
            var currencyAdd = new List<Currency>();

            foreach (var i in quest.RewardCurrency)
            {
                var currency = new Currency(i.Key, i.Value);
                App.Instance.Player.Wallet.Add(currency, IncomeSourceType.FromQuest);
                currencyAdd.Add(currency);
            }

            foreach (var i in quest.RewardItems)
            {
                var pickaxe = App.Instance.Player.Blacksmith.Pickaxes.Find(x => x.StaticPickaxe.Id == i.Key);
                if (pickaxe != null)
                {
                    pickaxe.Create();
                    continue;
                }

                var torch = App.Instance.Player.TorchesMerchant.Torches.Find(x => x.StaticTorch.Id == i.Key);
                if (torch != null)
                {
                    torch.Create();
                    continue;
                }

                var item = new Item(i.Key, i.Value);
                App.Instance.Player.Inventory.Add(item, IncomeSourceType.FromQuest);
                itemsAdd.Add(item);
            }

            Debug.Log(quest.RewardFeatures);
            foreach (var questRewardFeature in quest.RewardFeatures)
            {
                Debug.Log(questRewardFeature);
            }

            for (var index = 0; index < quest.RewardFeatures.Count; index++)
            {
                var i = quest.RewardFeatures[index];
                App.Instance.Player.Features.Add(i);
            }

            if (itemsAdd.Count == 0 && currencyAdd.Count == 0)
                return;

            WindowManager.Instance.Show<WindowQuestReward>()
                .Initialize(itemsAdd, currencyAdd);
        }

        public static bool IsQuestAvailableForTracking()
        {
            return App.Instance.Player.Quests.QuestList.FindAll(x => x.IsTracking).Count <
                   QuestsLocalConfig.MaxTrackingQuests;
        }


        public static bool ExtractQuestNeeds(Core.QuestTaskGoal questTaskGoal)
        {
            var goal = questTaskGoal.StaticGoal.Goal;

            switch (questTaskGoal.StaticGoal.Type)
            {
                case QuestTaskType.Collect:
                    var items = new Item(goal.Key, goal.Value);

                    if (App.Instance.Player.Inventory.Has(items))
                    {
                        App.Instance.Player.Inventory.Remove(items, SpendType.Quest);
                        return true;
                    }
                    else return false;
                case QuestTaskType.CollectCurrency:

                    if (!Enum.TryParse(goal.Key, out CurrencyType cur))
                    {
                        App.Instance.Services.LogService.LogError("Quests, wrong currency to extract");
                        return false;
                    }

                    var currency = new Currency(cur, goal.Value);

                    if (App.Instance.Player.Wallet.Has(currency))
                    {
                        App.Instance.Player.Wallet.Remove(currency);
                        return true;
                    }
                    else return false;
            }

            return true;
        }

        public static List<int> GetAllMonsterAppearTiers(string monsterId)
        {
            var tiers = new List<int>();

            for (var i = 0; i < App.Instance.StaticData.MineMonstersSpawn.Count; i++)
            {
                for (var index = 0; index < App.Instance.StaticData.MineMonstersSpawn[i].SpawnChances.Count; index++)
                {
                    var mineMonsterSpawnChance = App.Instance.StaticData.MineMonstersSpawn[i].SpawnChances[index];
                    if (mineMonsterSpawnChance.Chance.ContainsKey(monsterId))
                    {
                        if (!tiers.Contains(i))
                            tiers.Add(i);
                    }
                }
            }

            return tiers;
        }

        public static List<string> GetAllMonsterByItemDrop(string itemId)
        {
            var monsterList = new List<string>();

            var extraIdDrops = App.Instance.StaticData.QuestTaskGoalsCollect.FindAll(
                x => x.Id == itemId);

            for (var index = 0; index < extraIdDrops.Count; index++)
            {
                var extraIdDrop = extraIdDrops[index];
                var monster = App.Instance.StaticData.Monsters.Find(x =>
                    (extraIdDrop.SourceType == QuestTaskGoalCollectSourceType.MonsterId &&
                     x.Id == extraIdDrop.Source) ||
                    (extraIdDrop.SourceType == QuestTaskGoalCollectSourceType.MonsterType &&
                     x.Type.ToString() == extraIdDrop.Source));

                if (monster != null)
                    monsterList.Add(monster.Id);
            }


            foreach (var staticDataMonster in App.Instance.StaticData.Monsters)
            {
                if(staticDataMonster.DropItems.Find(x=>x.Id == itemId)!=null)
                    monsterList.Add(staticDataMonster.Id);
            }

            return monsterList;
        }

    /*    public static List<int> GetItemListMonsterDropInfo(string itemId)
        {
            var dropTiers = new List<int>();

            foreach (var monster in App.Instance.StaticData.Monsters)
            {
                if (monster.DropItems.Find(x => x.Id == itemId) != null)
                {
                    for(var i =0; i < App.Instance.StaticData.MineMonstersSpawn.Count; i++)
                    {
                        foreach (var mineMonsterSpawnChance in App.Instance.StaticData.MineMonstersSpawn[i].SpawnChances)
                        {
                            if(mineMonsterSpawnChance.Chance.ContainsKey(monster.Id))
                                dropTiers.Add(i);
                        }
                    }
                }
            }

            

            var extraIdDrop = App.Instance.StaticData.QuestTaskGoalsCollect.Find(
                x => x.Id == itemId);

            if (extraIdDrop != null )
            {
                var key = extraIdDrop.SourceType == QuestTaskGoalCollectSourceType.MonsterId ?
                    extraIdDrop.Source : App.Instance.StaticData.Monsters.Find(x=>x.Type.ToString() == extraIdDrop.Source).Id;

                for (var i = 0; i < App.Instance.StaticData.MineMonstersSpawn.Count; i++)
                {
                    foreach (var mineMonsterSpawnChance in App.Instance.StaticData.MineMonstersSpawn[i].SpawnChances)
                    {
                        if (mineMonsterSpawnChance.Chance.ContainsKey(key))
                            dropTiers.Add(i);
                    }
                }
            }

            return dropTiers;
        }*/

        public static List<int> GetItemTiersDropInfo(string itemId)
        {
            var dropTiers = new List<int>();

            for(var i = 0; i < App.Instance.StaticData.Tiers.Count; i++)
            {
                var tier = App.Instance.StaticData.Tiers[i];
                if (tier.WallItem1Id == itemId
                    || tier.WallItem2Id == itemId
                    || tier.WallItem3Id == itemId)
                {
                    if (!dropTiers.Contains(i))
                        dropTiers.Add(i);
                }
            }

            return dropTiers;
        }

        public static string GetQuestGoalsInfo(Core.Quest quest)
        {
            var resultText = "";

            if (!string.IsNullOrEmpty(quest.StaticQuest.Typ))
                return LocalizationHelper.GetLocale(quest.StaticQuest.Typ);

            var source = "";

            switch (quest.StaticQuest.Filter)
            {
                case "Ore":
                    source = LocalizationHelper.GetLocale("window_daily_task_quest_info_ore");

                    for (var index = 0; index < quest.TaskList.Count; index++)
                    {
                        var questTask = quest.TaskList[index];
                        for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                        {
                            var questTaskGoal = questTask.GoalsList[index1];
                            var tiersDrop = new List<int>();
                            var extraStr = "";

                            var resource =
                                App.Instance.StaticData.Resources.Find(x => x.Id == questTaskGoal.StaticGoal.Goal.Key);

                            if (resource.Type == ResourceType.Ingot)
                            {
                                tiersDrop = GetItemTiersDropInfo(resource.Id.Replace("ingot", "ore"));
                                extraStr =
                                    $"{LocalizationHelper.GetLocale("window_daily_task_quest_info_ingot_to_ore")} ";
                            }

                            var tiersText = "";
                            for (var i = 0; i < tiersDrop.Count; i++)
                                tiersText += $"{tiersDrop[i] + 1}" + (i < tiersDrop.Count - 1 ? ", " : ". ");

                            tiersText += "\n";

                            resultText += extraStr + string.Format(source,
                                              $"{LocalizationHelper.GetLocale(questTaskGoal.StaticGoal.Goal.Key)}",
                                              tiersText);
                        }
                    }

                    break;
                case "Kill":
                    source = LocalizationHelper.GetLocale("window_daily_task_quest_info_kill");
                    for (var index = 0; index < quest.TaskList.Count; index++)
                    {
                        var questTask = quest.TaskList[index];
                        for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                        {
                            var questTaskGoal = questTask.GoalsList[index1];
                            var tiersDrop = GetAllMonsterAppearTiers(questTaskGoal.StaticGoal.Goal.Key);

                            var tierText = "";

                            for (var i = 0; i < tiersDrop.Count; i++)
                                tierText += $"{1 + tiersDrop[i]}" + (i < tiersDrop.Count - 1 ? ", " : ". ");

                            tierText += "\n";

                            resultText += string.Format(source,
                                LocalizationHelper.GetLocale(questTaskGoal.StaticGoal.Goal.Key), tierText);
                        }
                    }

                    break;
                case "Collect":
                    source = LocalizationHelper.GetLocale("window_daily_task_quest_info_collect");
                    for (var index = 0; index < quest.TaskList.Count; index++)
                    {
                        var questTask = quest.TaskList[index];
                        for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                        {
                            var questTaskGoal = questTask.GoalsList[index1];
                            var monsters = GetAllMonsterByItemDrop(questTaskGoal.StaticGoal.Goal.Key);
                            var tiers = new List<int>();

                            foreach (var monster in monsters)
                            {
                                tiers.AddRange(GetAllMonsterAppearTiers(monster));
                            }

                            var monstersText = "";
                            for (var i = 0; i < monsters.Count; i++)
                                monstersText += $"{LocalizationHelper.GetLocale(monsters[i])}" +
                                                (i < monsters.Count - 1 ? " , " : ". ");

                            var tiersText = "";
                            for (var i = 0; i < tiers.Count; i++)
                                tiersText += $"{tiers[i] + 1}" + (i < tiers.Count - 1 ? ", " : ". ");

                            tiersText += "\n";

                            if(monstersText.Length > 0)
                            resultText += string.Format(source,
                                LocalizationHelper.GetLocale(questTaskGoal.StaticGoal.Goal.Key),
                                monstersText, tiersText);
                        }
                    }

                    break;
            }

            return resultText;
        }
    }
}
