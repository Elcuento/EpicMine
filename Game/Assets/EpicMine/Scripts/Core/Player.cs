using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine.Core
{
    public class Player
    {
        public string Id;

        public int TutorialStepId;

        public string Nickname;

        public string Language;
        public Skills Skills { get; }

        public Abilities Abilities { get; }

        public Artifacts Artefacts { get; }

        public Burglar Burglar { get; }

        public Inventory Inventory { get; }

        public Features Features { get; }

        public Wallet Wallet { get; }

        public Blacksmith Blacksmith { get; }

        public TorchesMerchant TorchesMerchant { get; }

        public Dungeon Dungeon { get; }

        public Workshop Workshop { get; }

        public int Prestige { get; private set; }

        public Gifts Gifts { get; }

        public DailyTasks DailyTasks { get; }

        public AdditionalInfo AdditionalInfo { get; }

        public Pvp Pvp { get; }

        public Shop Shop { get; }

        public Effect Effect { get; }

        public AutoMiner AutoMiner { get; }

        public Quests Quests { get; }

        public Versions Versions { get; }

        public bool UpgradePrestige { get; private set; }
        public long CreationDate { get; set; }
        public string Location { get; set; }

        public Player(CommonDLL.Dto.Player data)
        {
            if (data.UpgradePrestige)
            {
                PrestigeDrop(data);
            }

            Id = data.Id;
            Nickname = data.Nickname;
            TutorialStepId = data.TutorialStepId;
          
            Language = data.Language;
            CreationDate = data.CreationDate;
            Location = data.Location;

            Skills = new Skills(data.Skills);
            Abilities = new Abilities(data.Abilities);
            Artefacts = new Artifacts(data.Artifacts);
            Burglar = new Burglar(data.Burglar);
            Inventory = new Inventory(data.Inventory);
            Features = new Features(data.Features);
            Wallet = new Wallet(data.Wallet);
            Blacksmith = new Blacksmith(data.Blacksmith);
            TorchesMerchant = new TorchesMerchant(data.TorchesMerchant);
            Dungeon = new Dungeon(data.Dungeon);
            Workshop = new Workshop(data.Workshop);
            Prestige = data.Prestige;
            Gifts = new Gifts(data.Gifts);
            DailyTasks = new DailyTasks(data.DailyTasks);
            AdditionalInfo = new AdditionalInfo(data.AdditionalInfo);
            Pvp = new Pvp(data.Pvp);
            Shop = new Shop(data.Shop);
            Effect = new Effect(data.Effect);
            AutoMiner = new AutoMiner(data.AutoMiner);
            Quests = new Quests(data.Quests);

        }

        public void RecalculateTimeSensitive()
        {
            if (Workshop?.Slots != null)
            {
                foreach (var workshopSlot in Workshop.Slots)
                    workshopSlot.CalculateCompleted();
            }

            if (Workshop?.SlotsShard != null)
            {
                foreach (var workshopSlot in Workshop.SlotsShard)
                    workshopSlot.CalculateCompleted();
            }
        }

        public static CommonDLL.Dto.Player Load(string path)
        {
            if (File.Exists(path + "/profile.txt"))
            {
                try
                {
                    var data = File.ReadAllText(path + "/profile.txt");
                    var parseData = data.FromJson<CommonDLL.Dto.Player>();

                    Debug.Log("Parse profile completed");

                    return parseData;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                   // throw new Exception();
                     return  CreateNew();
                }
            }
            else
            {
                return CreateNew();
            }
         
        }

        public static CommonDLL.Dto.Player CreateNew()
        {
            Debug.Log("Create new profile");

            var data = new CommonDLL.Dto.Player(Guid.NewGuid().ToString())
            {
                CreationDate = TimeManager.Instance.NowUnixSeconds
            };

            Debug.Log("Create completed");

            return data;
        }
        public void SetNickName(string nickname)
        {
            Nickname = nickname;

            Save();
        }


        public void SetTutorialStep(int toString)
        {
            TutorialStepId = toString;

            Save();
        }

        public void Save()
        {
            try
            {
                var saveData = new CommonDLL.Dto.Player(this);
                File.WriteAllText(Application.persistentDataPath + "/profile.txt", saveData.ToJson());
               // Debug.Log("Save profile");
            }
            catch (Exception e)
            {
               Debug.LogError("Cant save profile" + e);
            }
    
        }

        public void SetPrestige(int userPrestigeLevel)
        {
            Prestige = userPrestigeLevel;

            Save();
        }

        public void PrestigeDrop(CommonDLL.Dto.Player data)
        {
            var itemsLeft = new List<Item>();

            foreach (var inventoryItem in data.Inventory.Items)
            {
                if (inventoryItem.Id.Contains("tnt_1") ||
                    inventoryItem.Id.Contains("shard") ||
                    inventoryItem.Id.Contains("potion") )
                {
                    itemsLeft.Add(inventoryItem);
                }
            }


            data.Dungeon.Tiers = new List<CommonDLL.Dto.Tier>() { new(0, true) };
            data.Abilities.Acid = null;
            data.Abilities.ExplosiveStrike = null;
            data.Abilities.Freezing = null;
            data.Skills.Critical = null;
            data.Skills.Damage = null;
            data.Features = new CommonDLL.Dto.Features();
            data.AutoMiner = new CommonDLL.Dto.AutoMiner();
            data.Skills.Fortune = null;
            data.Inventory = new CommonDLL.Dto.Inventory();
            data.Quests = new CommonDLL.Dto.Quests();
            data.Workshop = new CommonDLL.Dto.Workshop();
            data.Burglar = new CommonDLL.Dto.Burglar();
            data.DailyTasks = new CommonDLL.Dto.DailyTasks();
            data.Gifts = new CommonDLL.Dto.Gifts();
            data.Burglar = new CommonDLL.Dto.Burglar();
            data.Inventory.Items = itemsLeft;

            data.Blacksmith.SelectedPickaxe = data.Blacksmith.Pickaxes
                .FirstOrDefault()?.Id;

            data.TorchesMerchant.SelectedTorch = data.TorchesMerchant.Torches
                .FirstOrDefault()?.Id;

            data.UpgradePrestige = false;
        }

        public void SetUpgradePrestige(bool b)
        {
            UpgradePrestige = true;

            Save();
        }

        public void SetLanguage(string language)
        {
            Language = language;
        }
    }
}