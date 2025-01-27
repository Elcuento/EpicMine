using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Player
    {
        public string Id;

        public int TutorialStepId;

        public string Language;

        public string Nickname;

        public long Artifacts;

        public int Prestige;

        public long CreationDate;

        public string Location ;

        public Skills Skills ;

        public Abilities Abilities ;

        public Burglar Burglar ;

        public Inventory Inventory ;

        public Features Features ;

        public Wallet Wallet ;

        public Blacksmith Blacksmith ;

        public TorchesMerchant TorchesMerchant ;

        public DailyTasks DailyTasks;

        public Tutorial Tutorial;

        public Dungeon Dungeon ;

        public Workshop Workshop ;

        public Gifts Gifts ;

        public AdditionalInfo AdditionalInfo ;

        public Pvp Pvp ;

        public Shop Shop ;

        public Effect Effect ;

        public AutoMiner AutoMiner ;

        public Quests Quests ;

        public bool UpgradePrestige;

        public Player()
        {

        }

        public Player(string id, string nickname = "")
        {
            Id = id;
            Nickname = nickname;
            Language = "English";
            Location = "English";

            Skills = new Skills();
            Abilities = new Abilities();
            Burglar = new Burglar
            {
                Chests = new List<Chest>()
            };
            Inventory = new Inventory
            {
                Items = new List<Item>()
            };
            Features = new Features
            {
                FeaturesList = new List<FeaturesType>()
            };
            Wallet = new Wallet
            {
                Currencies = new List<Currency>
                {
                    new Currency(CurrencyType.Crystals,20),
                    new Currency(CurrencyType.Gold,3500),
                }
            };

            Blacksmith = new Blacksmith
            {
                AdPickaxes = new Dictionary<string, int>(),
                Pickaxes = new List<Pickaxe>(),
                SelectedPickaxe = ""
            };
            TorchesMerchant = new TorchesMerchant
            {
                Torches = new List<Torch>(),
                AdTorches = new Dictionary<string, int>(),
                SelectedTorch = "",
            };
     
            DailyTasks = new DailyTasks
            {
                TodayTaken = new List<DailyTask>()
            };

            Tutorial = new Tutorial
            {
                LastCompleteStep = TutorialStepIds.None,
            };

            Dungeon = new Dungeon
            {
                Tiers = new List<Tier>
                {
                    new Tier(
                        0,
                        true,
                        new List<Mine>(),
                        new List<string>()
                    )
                }
            };

            Workshop = new Workshop
            {
                Recipes = new List<Recipe>(),
                Slots = new List<WorkshopSlot>(),
                SlotsShard = new List<WorkshopSlot>()
            };
            Gifts = new Gifts { };
            AdditionalInfo = new AdditionalInfo();
            Pvp =new Pvp() {Rating = 11};
            Shop = new Shop
            {
                ShopOffer = new List<ShopOffer>(),
                ShopSale = new List<ShopSale>(),
                ShopSubscription = new List<ShopSubscription>(),
                TimePurchase = new List<ShopTimerPurchase>()
            };
            Effect = new Effect
            {
                BuffList = new List<Buff>()
            };
            AutoMiner = new AutoMiner();
            Quests = new Quests
            {
                QuestList = new List<Quest>()
            };

            Location = "US";

        }

         public Player(BlackTemple.EpicMine.Core.Player data)
         {
             Id = data.Id;
             TutorialStepId = data.TutorialStepId;
             Language = data.Language;
             Nickname = data.Nickname;
             Artifacts = data.Artefacts.Amount;
             Prestige = data.Prestige;
             CreationDate = data.CreationDate;
             Location = data.Location;
             Abilities = new Abilities(data.Abilities);
             Skills = new Skills(data.Skills);
             Burglar = new Burglar(data.Burglar);
             Inventory = new Inventory(data.Inventory);
             Features = new Features(data.Features);
             Wallet = new Wallet(data.Wallet);
             Workshop = new Workshop(data.Workshop);
             Effect = new Effect(data.Effect);
             UpgradePrestige = data.UpgradePrestige;
             Tutorial = new Tutorial(data.TutorialStepId);
             Gifts = new Gifts(data.Gifts);
             Dungeon = new Dungeon(data.Dungeon);
             DailyTasks = new DailyTasks(data.DailyTasks);
             AdditionalInfo = new AdditionalInfo(data.AdditionalInfo);
             AutoMiner = new AutoMiner(data.AutoMiner);
             Blacksmith = new Blacksmith(data.Blacksmith);
             Pvp = new Pvp(data.Pvp);
             Quests = new Quests(data.Quests);
             Shop = new Shop(data.Shop);
             TorchesMerchant = new TorchesMerchant(data.TorchesMerchant);

         }

    }
}