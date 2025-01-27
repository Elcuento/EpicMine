using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Player
    {
        public string Id;

        public int TutorialStepId;

        public string Nickname;

        public int Artifacts;

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

        public Versions Versions ;

        public Player()
        {

        }

        public Player(string id, string nickname = "")
        {
            Id = id;
            Nickname = nickname;

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
            Versions  =new Versions();

            Location = "US";

        }

        /* public Player(Player data)
         {
             Skills = new Skills(data.Characteristics);
             Abilities = new Abilities(data.Abilities);
             Artefacts = new Artefacts(data.Artefacts);
             Burglar = new Burglar(data.Burglar);
             Inventory = new Inventory(data.Inventory);
             Features = new Features(data.Features);
             Wallet = new Wallet(data.Wallet);
             Dungeon = new Dungeon(data.Dungeon);
             Pvp = new Pvp(data.Pvp);
             Blacksmith = new Blacksmith(data.Blacksmith);
             TorchesMerchant = new TorchesMerchant(data.TorchesMerchant);
             Workshop = new Workshop(data.Workshop);
             Prestige = data.Prestige.Level;
             Gifts = new Gifts(data.Gifts);
             DailyTasks = new DailyTasks(data.DailyTasks);
             AdditionalInfo = new AdditionalInfo(data.AdditionalInfo);
             Effect = new Effect(data.Effect);
             Shop = new Shop(data.Shop);
             AutoMiner = new AutoMiner(data.AutoMiner);
             Quests = new Quests(data.Quests);
             Versions = new Versions(data.Versions);
         }*/

    }
}