using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMTServer.Common;
using AMTServer.Core;
using AMTServer.Core.Response;
using AMTServer.Dto;
using AMTServerDLL.Core;
using AMTServerDLL.Dto;
using MongoDB.Bson;
using MongoDB.Driver;
using CommonDLL.Dto;
using CommonDLL.Static;
using EpicMineServerDLL.Dto;
using EpicMineServerDLL.Static.Enums;
using EpicMineServerDLL.Static.Helpers;
using Buff = CommonDLL.Static.Buff;
using PlayerMineRating = CommonDLL.Dto.PlayerMineRating;
using ResponseTutorialAddArtifacts = AMTServer.Core.Response.ResponseTutorialAddArtifacts;
using ServerInfo = AMTServer.Dto.ServerInfo;
using Tier = CommonDLL.Dto.Tier;


namespace AMTServer
{
    public class ClientPeer : BasePeer
    {

        public bool IsSuccessLogin { get; private set; }

        public Dto.Player Player { get; private set; }

        public AppVersionInfo Version { get; private set; }

        protected StaticData StaticData { get; private set; }

        protected FileSystem FileSystem;

        protected RatingSystem RatingSystem;

        protected DataBaseLinks DataBaseLinks;

        protected PvpMatchSystem MatchSystem;

        protected EpicMineServer Server;

        internal PlayerResponseArchive ResponsesArchive;

        public ClientPeer(EpicMineServer server, ServerHandlerClient client, FileSystem fileArchive, DataBaseLinks links,
            RatingSystem rating, PvpMatchSystem matchSystem) : base(client)
        {
            Server = server;
            Player = null;
            FileSystem = fileArchive;
            DataBaseLinks = links;
            RatingSystem = rating;
            MatchSystem = matchSystem;
        }

        public override void Log(string log, bool isError = false)
        {
            base.Log($"[{Player?.Data?.Id}]{log}", isError);
        }

        public override void Destroy(string reason)
        {
            if (IsDestroyed)
                return;

            if (Player?.Data != null)
            {
                Server.AddClientQuite(Player.Data.Id);
                SaveResponses();
            }

            base.Destroy(reason);

            try
            {
                SavePlayer();
            }
            catch (Exception e)
            {
                Log(e.ToString(), true);
            }

            try
            {
                LeavePvpArena();
            }
            catch (Exception e)
            {
                Log(e.ToString(), true);
            }

        }

        private void SaveResponses()
        {
            if (ResponsesArchive != null)
            {
                DataBaseLinks.PlayerResponseArchive.ReplaceOne(
                    x => x.Id == ResponsesArchive.Id,
                    ResponsesArchive,
                    new ReplaceOptions {IsUpsert = true});
            }
        }

        public Dto.Player GetByNickName(string nick)
        {
            return DataBaseLinks.UserCollection.FindSync(x => x.Data.Nickname.ToLower() == nick.ToLower())
                .FirstOrDefault();
        }

        public ClientPeer GetClientByNickName(string nick)
        {
            return Server.GetClientByNick(nick);
        }

        public ClientPeer GetClientById(string id)
        {
            return Server.GetClientById(id);
        }

        public Dto.Player GetByMongoId(string id)
        {
            try
            {
                var filter = Builders<Dto.Player>.Filter.Eq("_id", new ObjectId(id));
                var pl = DataBaseLinks.UserCollection.FindSync(filter).FirstOrDefault();
                return pl;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void Login()
        {
            IsSuccessLogin = true;

            var match = MatchSystem.GetMyMatch(Player.Data.Id);
            if (match != null)
            {
                MatchSystem.BroadCastPvpArenaUpdate(match);
                UpdatePvpStats();
            }

            Server.RemoveClientQuite(Player.Data.Id);

            ResponsesArchive = DataBaseLinks.PlayerResponseArchive.Find(x => x.PlayerId == Player.Data.Id).FirstOrDefault() 
                               ?? new PlayerResponseArchive(Player.Data.Id);

            Log("LoginSuccesses");
        }

        public void SavePlayer()
        {
            if (Player == null)
                return;

            Player.SetLastOnlineDate(Utils.GetUnixTime());

            if (GetByMongoId(Player.Id.ToString()) == null)
            {
                DataBaseLinks.UserCollection.InsertOne(Player);

            }
            else
            {
                DataBaseLinks.UserCollection.ReplaceOne(x => x.Id == Player.Id, Player);
            }
        }

        public Dto.Player GetPlayerByFaceBookId(string faceBookId)
        {
            if (string.IsNullOrEmpty(faceBookId))
                return null;

            return DataBaseLinks.UserCollection.FindSync(x => x.GoogleId == faceBookId).FirstOrDefault();
        }

        public Dto.Player GetPlayerByGoogleId(string googleId)
        {
            if (string.IsNullOrEmpty(googleId))
                return null;

            return DataBaseLinks.UserCollection.FindSync(x => x.GoogleId == googleId).FirstOrDefault();
        }

        public Dto.Player GetCreatePlayerByGoogleId(string deviceId, string googleId)
        {
            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(deviceId))
                return Player;

            Player = DataBaseLinks.UserCollection.FindSync(x => x.GoogleId == googleId).FirstOrDefault();

            if (Player == null)
            {
                var id = ObjectId.GenerateNewId();

                Player = new Dto.Player(id.ToString(), deviceId, FileSystem.StaticData);
                Player.SetGoogleId(googleId);


                Player.Data.CreationDate = new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeSeconds();

                SavePlayer();
            }

            return Player;
        }

        public Dto.Player GetCreatePlayerByFaceBookId(string deviceId, string faceBookId)
        {
            if (string.IsNullOrEmpty(faceBookId) || string.IsNullOrEmpty(deviceId))
                return Player;

            Player = DataBaseLinks.UserCollection.FindSync(x => x.FaceBookId == faceBookId).FirstOrDefault();

            if (Player == null)
            {
                var id = ObjectId.GenerateNewId();

                Player = new Dto.Player(id.ToString(), deviceId, FileSystem.StaticData);
                Player.SetDeviceId(deviceId);
                Player.SetFaceBookId(faceBookId);

                Player.Data.CreationDate = new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeSeconds();

                SavePlayer();
            }

            return Player;
        }

        public StaticData GetStaticData()
        {
            if (StaticData == null)
            {
                StaticData = FileSystem.GetStaticDataCopy();

            }

            return StaticData;
        }

        public List<AppVersionInfo> GetVersionsInfo()
        {
            //lock (FileSystem)
            {
                return FileSystem.Versions;
            }
        }


        public AppVersionInfo GetAndSetVersionInfo(PlatformType type, string appVersion)
        {
            if (Version == null)
            {
              //  lock (FileSystem)
                {
                    Version = FileSystem.GetVersion(type, appVersion);
                }
            }

            return Version;
        }


        public List<News> GetNews()
        {
          //  lock (FileSystem)
            {
                return FileSystem.News;
            }
        }

        public LocalizationData GetLanguageDictionary(string languageCode)
        {
          // lock (FileSystem)
            {
                return FileSystem.GetLanguage(Version, languageCode);
            }
        }

        public Dto.Player GetCreatePlayerByDeviceId(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                return Player;

            Player = DataBaseLinks.UserCollection.FindSync(x => x.DeviceId == deviceId).FirstOrDefault();

            if (Player == null)
            {
                var id = ObjectId.GenerateNewId();


                Player = new Dto.Player(id.ToString(), deviceId, FileSystem.StaticData)
                {
                    Data = {CreationDate = new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeSeconds()}
                };

                SavePlayer();
            }


            return Player;
        }

        /* public Player GetPlayer(string userId,string deviceId)
         {
             if (Player != null && Player.Id != ObjectId.Empty && Player.Id.ToString() == userId)
                 return Player;
 
             Player = UserCollection.FindSync(x => x.Data != null && x.Id.ToString() == userId).FirstOrDefault();
 
             if (Player == null)
             {
                 lock (FileArchive)
                 {
                     Player = new Player(ObjectId.GenerateNewId().ToString(), FileArchive.StaticData)
                     {
                         Data = new Player(userId)
                     };
                 }
             
             }
             SavePlayer();
 
             return Player;
         }*/

        public Dto.Player GetAndSetPlayerWithNull(string userId)
        {
            if (Player != null && Player.Id != ObjectId.Empty && Player.Id.ToString() == userId)
                return Player;

            Player = DataBaseLinks.UserCollection.FindSync(x => x.Data.Id == userId).FirstOrDefault();

            return Player;
        }

        // ReSharper disable once SuggestBaseTypeForParameter

        protected override void OnGetData(Package package)
        {
            var command = (CommandType)package.Command;

            Log($"[ReceivedMessage][{command}]");

            var response = ResponsesArchive?.Get(package);
            if (response != null)
            {
                var opData = Encoding.UTF8.GetString(AMTServerDLL.Utils.Decompress(response.OperationDataArchive)).FromJson<SendData>();

                if (opData != null)
                {
                    Log($"[ReSendDuplicateAgain]");

                    SendResponseLessNetworkMessage(opData, (CommandType) response.OperationLocalCommand,
                        response.OperationId);
                }

                return;
            }


            if (Server.IsLoginServer) // Login server only
            {
                switch (command) 
                {
                    case CommandType.GetServerStatus:
                        ResponseGetServerStatus.StartResponse(new ResponseGetServerStatus(this, package));
                        break;
                    case CommandType.BugReport:
                        ResponseBugReport.StartResponse(new ResponseBugReport(this, package));
                        break;
                    case CommandType.GetServerAddress:
                        var servers = Server.GetServersInfo();
                        ResponseGetServerAddress.StartResponse(new ResponseGetServerAddress(this, package, servers));
                        break;
                    default:
                        var errorResponse = new ResponseError(this, package, ErrorType.NeedLogin);
                        errorResponse.DisableLog(true);
                        ResponseError.StartResponse(errorResponse);
                        break;
                }

                return;
            }

            if (!IsSuccessLogin && !Constrains.IsSuccessesWithoutLogin(command))
            {
                var errorResponse = new ResponseError(this, package, ErrorType.NeedLogin);
                errorResponse.DisableLog(true);
                errorResponse.DisableArchive(true);
                ResponseError.StartResponse(errorResponse);
                return;
            }

            //Console.WriteLine("Get data " + package.Type);
            switch (command)
            {
                case CommandType.Login:
                    ResponseLogin.StartResponse(new ResponseLogin(this, package));
                    break;

                case CommandType.ReLogin:
                    ResponseReLogin.StartResponse(new ResponseReLogin(this, package));
                    break;

                case CommandType.GetServerStatus:
                    ResponseGetServerStatus.StartResponse(new ResponseGetServerStatus(this, package));
                    break;
                case CommandType.GetVersionInfo:
                    ResponseGetVersionInfo.StartResponse(new ResponseGetVersionInfo(this, package));
                    break;
                case CommandType.GetGamesEventsInfo:
                    ResponseGetGameEventsInfo.StartResponse(new ResponseGetGameEventsInfo(this, package));
                    break;
                case CommandType.LoginWithGoogle:
                    ResponseLoginWithGoogle.StartResponse(new ResponseLoginWithGoogle(this, package));
                    break;

                case CommandType.LoginWithFaceBook:
                    ResponseLoginWithFaceBook.StartResponse(new ResponseLoginWithFaceBook(this, package));
                    break;
                case CommandType.LinkFaceBook:
                    ResponseLinkWithFaceBook.StartResponse(new ResponseLinkWithFaceBook(this, package));
                    break;
                case CommandType.LinkGoogle:
                    ResponseLinkWithGoogle.StartResponse(new ResponseLinkWithGoogle(this, package));
                    break;
                case CommandType.GetProfile:
                    ResponseGetProfile.StartResponse(new ResponseGetProfile(this, package));
                    break;
                case CommandType.SetNickName:
                    ResponseSetNickName.StartResponse(new ResponseSetNickName(this, package));
                    break;
                case CommandType.UpdatePickaxe:
                    ResponseUpdatePickaxe.StartResponse(new ResponseUpdatePickaxe(this, package));
                    break;
                case CommandType.UpdateInventory:
                    ResponseUpdateInventory.StartResponse(new ResponseUpdateInventory(this, package));
                    break;
                case CommandType.UpdateCurrency:
                    ResponseUpdateCurrency.StartResponse(new ResponseUpdateCurrency(this, package));
                    break;
                case CommandType.BuyPickaxe:
                    ResponseBuyPickaxe.StartResponse(new ResponseBuyPickaxe(this, package));
                    break;
                case CommandType.UpdateTorches:
                    ResponseUpdateTorches.StartResponse(new ResponseUpdateTorches(this, package));
                    break;
                case CommandType.UpdateAdPickaxe:
                    ResponseUpdateAdPickaxe.StartResponse(new ResponseUpdateAdPickaxe(this, package));
                    break;
                case CommandType.UpdateAdTorch:
                    ResponseUpdateAdTorches.StartResponse(new ResponseUpdateAdTorches(this, package));
                    break;
                case CommandType.UpdatePvpInvite:
                    ResponseUpdatePvpInvite.StartResponse(new ResponseUpdatePvpInvite(this, package));
                    break;
                case CommandType.UpdateAbilities:
                    ResponseUpdateAbilities.StartResponse(new ResponseUpdateAbilities(this, package));
                    break;
                case CommandType.UpdateSkills:
                    ResponseUpdateSkills.StartResponse(new ResponseUpdateSkills(this, package));
                    break;
                case CommandType.UpdateSelectedPickaxe:
                    ResponseUpdateSelectedPickaxe.StartResponse(new ResponseUpdateSelectedPickaxe(this, package));
                    break;
                case CommandType.UpdateSelectedTorch:
                    ResponseUpdateSelectedTorch.StartResponse(new ResponseUpdateSelectedTorch(this, package));
                    break;
                case CommandType.UpdateTutorialStepId:
                    ResponseUpdateTutorialStepId.StartResponse(new ResponseUpdateTutorialStepId(this, package));
                    break;
                case CommandType.UpdateRecipes:
                    ResponseUpdateRecipes.StartResponse(new ResponseUpdateRecipes(this, package));
                    break;
                case CommandType.UpdateQuests:
                    ResponseUpdateQuests.StartResponse(new ResponseUpdateQuests(this, package));
                    break;
                case CommandType.UpdateTiers:
                    ResponseUpdateTiers.StartResponse(new ResponseUpdateTiers(this, package));
                    break;
                case CommandType.UpdateAutoMinerLevels:
                    ResponseUpdateMinerLevels.StartResponse(new ResponseUpdateMinerLevels(this, package));
                    break;
                case CommandType.WorkShopBuySlot:
                    ResponseWorkShopSlotBuy.StartResponse(new ResponseWorkShopSlotBuy(this, package));
                    break;
                case CommandType.WorkShopSetReceipt:
                    ResponseWorkShopSetSlot.StartResponse(new ResponseWorkShopSetSlot(this, package));
                    break;
                case CommandType.WorkShopCollectSlot:
                    ResponseWorkShopSlotCollect.StartResponse(new ResponseWorkShopSlotCollect(this, package));
                    break;
                case CommandType.WorkShopCompleteSlot:
                    ResponseWorkShopComplete.StartResponse(new ResponseWorkShopComplete(this, package));
                    break;
                case CommandType.WorkShopSlotCollectByCrystal:
                    ResponseWorkShopSlotCollectCrystalPay.StartResponse(
                        new ResponseWorkShopSlotCollectCrystalPay(this, package));
                    break;
                case CommandType.WorkShopTutorialCompleteSlot:
                    ResponseWorkShopSlotTutorialForceComplete.StartResponse(
                        new ResponseWorkShopSlotTutorialForceComplete(this, package));
                    break;
                case CommandType.TransferFireBaseProfile:
                    ResponseTransferFireBaseProfile.StartResponse(new ResponseTransferFireBaseProfile(this, package));
                    break;

                case CommandType.GetStaticData:
                    ResponseGetStaticData.StartResponse(new ResponseGetStaticData(this, package));
                    break;
                case CommandType.UpdateAdditionalInfo:
                    ResponseUpdateAdditionalInfo.StartResponse(new ResponseUpdateAdditionalInfo(this, package));
                    break;
                case CommandType.ShopGetPurchaseList:
                    ResponseShopGetPurchaseList.StartResponse(new ResponseShopGetPurchaseList(this, package));
                    break;
                case CommandType.ShopAddOffer:
                    ResponseAddShopOffer.StartResponse(new ResponseAddShopOffer(this, package));
                    break;
                case CommandType.ShopBuyShopPack:
                    ResponseBuyShopPack.StartResponse(new ResponseBuyShopPack(this, package));
                    break;
                case CommandType.ShopBuyShopSale:
                    ResponseAddShopSaleCharge.StartResponse(new ResponseAddShopSaleCharge(this, package));
                    break;
                case CommandType.ShopRestoreSubscription:
                    ResponseShopSubscriptionRestore.StartResponse(new ResponseShopSubscriptionRestore(this, package));
                    break;
                case CommandType.ShopBuySubscription:
                    ResponseBuyShopPackSubscription.StartResponse(new ResponseBuyShopPackSubscription(this, package));
                    break;
                case CommandType.ShopBuyResource:
                    ResponseBuyShopResources.StartResponse(new ResponseBuyShopResources(this, package));
                    break;
                case CommandType.ShopBuyTimePurchase:
                    ResponseAddShopTimePurchase.StartResponse(new ResponseAddShopTimePurchase(this, package));
                    break;
                case CommandType.GiftOpen:
                    ResponseGiftOpen.StartResponse(new ResponseGiftOpen(this, package));
                    break;
                case CommandType.CheckEffect:
                    ResponseEffectCheck.StartResponse(new ResponseEffectCheck(this, package));
                    break;
                case CommandType.CheckEffects:
                    ResponseEffectsCheck.StartResponse(new ResponseEffectsCheck(this, package));
                    break;
                case CommandType.FirceTakeFirstTradeAffairsDailyTaskCompleteGift:
                    ResponseDailyTaskTakeFirstTradeAffairsCompleteGift.StartResponse(
                        new ResponseDailyTaskTakeFirstTradeAffairsCompleteGift(this, package));
                    break;
                case CommandType.TakeDailyTaskReward:
                    ResponseDailyTaskTakeReward.StartResponse(new ResponseDailyTaskTakeReward(this, package));
                    break;
                case CommandType.TutorialGetArtifact:
                    ResponseTutorialAddArtifacts.StartResponse(new ResponseTutorialAddArtifacts(this, package));
                    break;
                case CommandType.ChestPvpDouble:
                    ResponsePvpChestGetDouble.StartResponse(new ResponsePvpChestGetDouble(this, package));
                    break;
                case CommandType.GetNews:
                    ResponseGetNews.StartResponse(new ResponseGetNews(this, package));
                    break;
                case CommandType.RestorePickaxe:
                    ResponseRestorePickaxeByCrystals.StartResponse(new ResponseRestorePickaxeByCrystals(this, package));
                    break;
                case CommandType.Prestige:
                    ResponsePrestige.StartResponse(new ResponsePrestige(this, package));
                    break;
                case CommandType.PrestigeReward:
                    ResponsePrestigeReward.StartResponse(new ResponsePrestigeReward(this, package));
                    break;
                case CommandType.ChestPvpOpen:
                    ResponsePvpChestOpen.StartResponse(new ResponsePvpChestOpen(this, package));
                    break;
                case CommandType.ChestEnchantedOpenByAd:
                    ResponseChestEnchantedOpenByAd.StartResponse(new ResponseChestEnchantedOpenByAd(this, package));
                    break;
                case CommandType.QuestComplete:
                    ResponseQuestComplete.StartResponse(new ResponseQuestComplete(this, package));
                    break;
                case CommandType.QuestRemove:
                    ResponseQuestRemove.StartResponse(new ResponseQuestRemove(this, package));
                    break;
                case CommandType.QuestSetTracking:
                    ResponseQuestSetTracking.StartResponse(new ResponseQuestSetTracking(this, package));
                    break;
                case CommandType.GiftTakeUnlockSecondaryGift:
                    ResponseGiftTakeUnlockSecondaryGift.StartResponse(
                        new ResponseGiftTakeUnlockSecondaryGift(this, package));
                    break;
                case CommandType.TierOpen:
                    ResponseTierOpen.StartResponse(new ResponseTierOpen(this, package));
                    break;
                case CommandType.AutoMinerCollect:
                    ResponseAutoMinerCollect.StartResponse(new ResponseAutoMinerCollect(this, package));
                    break;
                case CommandType.AutoMinerCapacityLevelUp:
                    ResponseAutoMinerCapacityLevelUp.StartResponse(new ResponseAutoMinerCapacityLevelUp(this, package));
                    break;
                case CommandType.AutoMinerGetDoubleEarningByCrystals:
                    ResponseAutoMinerGetDoubleEarningByCrystals.StartResponse(
                        new ResponseAutoMinerGetDoubleEarningByCrystals(this, package));
                    break;
                case CommandType.ChestEnchantedOpenByCrystals:
                    ResponseChestEnchantedOpenByCrystals.StartResponse(
                        new ResponseChestEnchantedOpenByCrystals(this, package));
                    break;
                case CommandType.GetLanguageDictionary:
                    ResponseGetLanguageDictionary.StartResponse(new ResponseGetLanguageDictionary(this, package));
                    break;
                case CommandType.ChestAdd:
                    ResponseChestAdd.StartResponse(new ResponseChestAdd(this, package));
                    break;
                case CommandType.ChestOpenTutorial:
                    ResponseChestOpenTutorial.StartResponse(new ResponseChestOpenTutorial(this, package));
                    break;
                case CommandType.ChestOpen:
                    ResponseChestOpen.StartResponse(new ResponseChestOpen(this, package));
                    break;
                case CommandType.ChestForceOpen:
                    ResponseChestOpenForce.StartResponse(new ResponseChestOpenForce(this, package));
                    break;
                case CommandType.ChestStartBreaking:
                    ResponseChestStartBreaking.StartResponse(new ResponseChestStartBreaking(this, package));
                    break;
                case CommandType.GetLeaderBoard:
                    ResponseGetLeaderBoard.StartResponse(new ResponseGetLeaderBoard(this, package));
                    break;
                case CommandType.GetLeaderBoardNewBie:
                    ResponseGetLeaderBoardNewBie.StartResponse(new ResponseGetLeaderBoardNewBie(this, package));
                    break;
                case CommandType.GetLeaderBoardPvp:
                    ResponseGetLeaderBoardPvp.StartResponse(new ResponseGetLeaderBoardPvp(this, package));
                    break;
                case CommandType.PvpUpdatePlayerInfo:
                    ResponsePvpUpdatePlayerInfo.StartResponse(new ResponsePvpUpdatePlayerInfo(this, package));
                    break;
                case CommandType.PvpUpdateMatchInfo:
                    ResponsePvpUpdateMatchInfo.StartResponse(new ResponsePvpUpdateMatchInfo(this, package));
                    break;
                case CommandType.PvpJoinCreate:
                ResponsePvpJoinCreate.StartResponse(new ResponsePvpJoinCreate(this, package));
                    break;
                case CommandType.PvpCreate:
                    ResponsePvpCreate.StartResponse(new ResponsePvpCreate(this, package));
                    break;
                case CommandType.PvpLeaveArena:
                    ResponsePvpLeaveArena.StartResponse(new ResponsePvpLeaveArena(this, package));
                    break;
                case CommandType.PvpSetBot:
                    ResponsePvpSetBot.StartResponse(new ResponsePvpSetBot(this, package));
                    break;
                case CommandType.PvpInvite:
                    ResponsePvpInvite.StartResponse(new ResponsePvpInvite(this, package));
                    break;
                case CommandType.PvpInviteAccepted:
                    ResponsePvpInviteAccepted.StartResponse(new ResponsePvpInviteAccepted(this, package));
                    break;
                case CommandType.PvpInviteCancel:
                    ResponsePvpInviteCancel.StartResponse(new ResponsePvpInviteCancel(this, package));
                    break;
                case CommandType.PvpInviteDenied:
                    ResponsePvpInviteDenied.StartResponse(new ResponsePvpInviteDenied(this, package));
                    break;
                case CommandType.PvpFindUserByName:
                    ResponsePvpFindUserByName.StartResponse(new ResponsePvpFindUserByName(this, package));
                    break;
                case CommandType.PvpConfirmResult:
                    ResponsePvpGetConfirmResult.StartResponse(new ResponsePvpGetConfirmResult(this, package));
                    break;
                case CommandType.GetServerAddress:
                    ResponseGetServerAddress.StartResponse(new ResponseGetServerAddress(this, package, new List<ServerInfo>()));
                    break;
                case CommandType.ChestSpeedUp:
                    ResponseChestSpeedUp.StartResponse(new ResponseChestSpeedUp(this, package));
                    break;
                case CommandType.DeveloperSetTutorialStep:
                    ResponseDeveloperSetTutorialStep.StartResponse(new ResponseDeveloperSetTutorialStep(this, package));
                    break;
                case CommandType.DeveloperDropSkillsAbilities:
                    ResponseDeveloperDropSkillsAbilities.StartResponse(new ResponseDeveloperDropSkillsAbilities(this, package));
                    break;
                case CommandType.DeveloperSetArtifacts:
                    ResponseDeveloperSetArtifacts.StartResponse(new ResponseDeveloperSetArtifacts(this, package));
                    break;
                case CommandType.DeveloperSetCrystals:
                    ResponseDeveloperSetCrystals.StartResponse(new ResponseDeveloperSetCrystals(this, package));
                    break;
                case CommandType.DeveloperSetRating:
                    ResponseDeveloperSetRating.StartResponse(new ResponseDeveloperSetRating(this, package));
                    break;
                case CommandType.DeveloperResetProgress:
                    ResponseDeveloperResetProgress.StartResponse(new ResponseDeveloperResetProgress(this, package));
                    break;
                case CommandType.DeveloperOpenTier:
                    ResponseDeveloperOpenTier.StartResponse(new ResponseDeveloperOpenTier(this, package));
                    break;
                case CommandType.PvpSendEmoji:
                    ResponsePvpSendEmoji.StartResponse(new ResponsePvpSendEmoji(this, package));
                    break;
                case CommandType.BugReport:
                    ResponseBugReport.StartResponse(new ResponseBugReport(this, package));
                    break;
                case CommandType.ReloadWorkshop:
                    ResponseReloadWorkShop.StartResponse(new ResponseReloadWorkShop(this, package));
                    break;
            }
        }


        public bool IsCurrencyExist(CurrencyType currencyType, long cost)
        {
            return (Player.Data.Wallet.Currencies.Find(x => x.Type == currencyType)?.Amount ?? 0) >= cost;
        }

        public bool SubsTractCurrency(CurrencyType currencyType, long cost)
        {
            if (cost == 0)
                return true;

            var current = Player.Data.Wallet.Currencies.Find(x => x.Type == currencyType);

            if (current == null || current.Amount < cost)
                return false;

            current.Amount -= cost;
            current.Amount = current.Amount < 0 ? 0 : current.Amount;
            SavePlayer();
            return true;
        }

        public void AddCurrency(CurrencyType type, long val)
        {
            var current = Player.Data.Wallet.Currencies.Find(x => x.Type == type);

            if (current != null)
                current.Amount += val;
            else Player.Data.Wallet.Currencies.Add(current = new Currency(type, val));

            current.Amount = current.Amount < 0 ? 0 : current.Amount;

            SavePlayer();

        }

        public Dto.PlayerPurchase GetPurchases()
        {
            return DataBaseLinks.UserPurchaseCollection.FindSync(x => x.PlayerId == Player.Id.ToString())
                ?.FirstOrDefault();
        }

        public void CalculatePvpRating()
        {

        }

        public void CalculateMineRating()
        {
            try
            {
                var playerData = Player.Data;

                if (string.IsNullOrEmpty(playerData.Nickname))
                    return;

                var tierNumber = playerData.Dungeon.Tiers.Count;
                var mineNumber = 0;
                var rating = 0;
                var hardcoreRating = 0;
                var prestige = playerData.Prestige;

                if (tierNumber > 0)
                    tierNumber -= 1;

                foreach (var mine in playerData.Dungeon.Tiers[tierNumber].Mines)
                {
                    if (mine.IsComplete)
                        mineNumber = mine.Number;
                }

                foreach (var tier in playerData.Dungeon.Tiers)
                {
                    foreach (var mine in tier.Mines)
                    {
                        if (mine.IsComplete)
                        {
                            rating += mine.Rating;
                            hardcoreRating += mine.HardcoreRating;
                        }
                    }
                }

                var userMongoRating = new Dto.PlayerMineRating(
                    new PlayerMineRating
                    {
                        UserNick = playerData.Nickname,
                        UserId = playerData.Id,
                        Tier = tierNumber + 1,
                        Mine = mineNumber + 1,
                        Prestige = prestige,
                        Rating = rating,
                        HardCoreRating = hardcoreRating,
                        UserLocalate = playerData.Location
                    });

                var rate = DataBaseLinks.UserMineRatingCollection.FindSync(x => x.Rating.UserId == playerData.Id)
                    .FirstOrDefault();

                if (rate != null)
                {
                    DataBaseLinks.UserMineRatingCollection.DeleteOne(x => x.Id == rate.Id);
                    DataBaseLinks.UserMineRatingCollection.InsertOne(userMongoRating);
                }
                else
                {
                    DataBaseLinks.UserMineRatingCollection.InsertOne(userMongoRating);
                }
            }
            catch (Exception e)
            {
                LogSystem.Log(e,true);
            }

        }

        public void AddPurchase(string shopPackId, string valueReceipt, ShopPurchaseStatus status)
        {
            var purchases = GetPurchases();
            var purchase = new CommonDLL.Dto.Purchase(shopPackId, valueReceipt, status, DateTime.UtcNow);
            if (purchases == null)
            {
                DataBaseLinks.UserPurchaseCollection.InsertOne(new PlayerPurchase(Player.Id.ToString(),
                    new List<CommonDLL.Dto.Purchase>() {purchase}));
            }
            else
            {
                if (purchases.Purchases == null)
                {
                    purchases.Purchases = new List<CommonDLL.Dto.Purchase>() {purchase};
                }
                else
                {
                    purchases.Purchases.Add(purchase);
                }

                DataBaseLinks.UserPurchaseCollection.ReplaceOne(x => x.Id == purchases.Id, purchases);
            }
        }

        public void AddBuffUntilTime(List<Buff> splitBuffs, long expireDate)
        {
            if (splitBuffs.Count == 0)
                return;

            var dateNow = Utils.GetUnixTime();

            foreach (var bufStatic in StaticData.Buffs)
            {
                foreach (var buffData in splitBuffs)
                {
                    if (bufStatic.Id == buffData.Id)
                    {
           var buffSetData = new CommonDLL.Dto.Buff(bufStatic.Value)
                        {
                            Id = bufStatic.Id,
                            Type = bufStatic.Type,
                            Time = expireDate,
                        };

                        var buffOn = Player.Data.Effect.BuffList.Find(x => x.Id == buffSetData.Id);

                        if (buffOn != null)
                            Player.Data.Effect.BuffList.Remove(buffOn);

                        Player.Data.Effect.BuffList.Add(buffSetData);

                        if (buffData.Type == BuffType.Currency)
                        {
                            buffSetData.NextCheck = dateNow + (24 * 60 * 60);
                        }
                    }
                }
            }

            SavePlayer();

        }

        public void AddBuffs(List<Buff> splitBuffs, float timeCoefficient = 1)
        {
            if (splitBuffs.Count == 0)
                return;

            var dateNow = Utils.GetUnixTime();

            foreach (var bufStatic in StaticData.Buffs)
            {
                var extraTime = (long) ((bufStatic.Time * 60 * 60) * timeCoefficient);

                foreach (var buffData in splitBuffs)
                {
                    if (bufStatic.Id == buffData.Id)
                    {
                        long sameBuffTime = 0;
                        var userEffect = Player.Data.Effect;

                        if (userEffect != null)
                        {
                            foreach (var element in userEffect.BuffList)
                            {
                                var elementStatic = StaticData.Buffs.Find(x => x.Id == element.Id);

                                if (element.Id != bufStatic.Id)
                                {
                                    if (elementStatic != null && elementStatic.Filter == bufStatic.Filter &&
                                        bufStatic.Priority >=
                                        elementStatic.Priority)
                                    {
                                        element.Time += extraTime;
                                    }
                                }
                            }
                        }

                        var buffOn = Player.Data.Effect.BuffList.Find(x => x.Id == bufStatic.Id);
                        if (buffOn != null)
                        {
                            if (buffOn.Time > dateNow)
                            {
                                sameBuffTime = buffOn.Time - dateNow;
                            }
                        }


                        var buffSetData = new CommonDLL.Dto.Buff(bufStatic.Value)
                        {
                            Id = bufStatic.Id,
                            Type = bufStatic.Type,
                            Time = dateNow + sameBuffTime + extraTime,
                        };

                        Player.Data.Effect.BuffList.Add(buffSetData);

                        if (buffSetData.Type == BuffType.Currency)
                        {
                            buffSetData.NextCheck = dateNow + (24 * 60 * 60);
                        }
                    }
                }
            }

            SavePlayer();
        }

        public List<PlayerMineRating> GetLeaderBoard()
        {
            return RatingSystem.GetMineRating();
        }

        public List<CommonDLL.Dto.PlayerPvpRating> GetPvpBoard()
        {
            return RatingSystem.GetPvpRating();
        }

        public List<PlayerMineRating> GetNewBieLeaderBoard()
        {
            return RatingSystem.GetNewBieMineRating();
        }

        public void LeavePvpArena()
        {
            if (Player?.Data == null)
                return;

            var arena = MatchSystem.GetMyMatch(Player.Data.Id);
            MatchSystem.Leave(arena, Player.Data.Id);
        }

        public PvpArenaUserInfo SetBotToPvpArena()
        {
            var arena = MatchSystem.GetMyMatch(Player.Data.Id);
            return MatchSystem.SetBot(arena, Player);
        }

        public PvpArenaMatchInfo CreatePvpArena(int arena, bool lockRoom = true)
        {
            return MatchSystem.CreatePvpArena(Player, arena);
        }

        public PvpArenaMatchInfo JoinCreatePvpArena(int arena)
        {
           return MatchSystem.JoinCreateArena(Player,arena);
        }

        public bool PvpUpdateMatchInfo(PvpArenaMatchInfo info)
        {
          return  MatchSystem.UpdateMatch(info);
        }

        public bool PvpUpdateUserInfo(PvpArenaUserInfo info)
        {
            return MatchSystem.UpdateMatchUser(info);
        }
        
        public bool SendInvite(string userId)
        {
            var matchData = MatchSystem.GetMyMatch(Player.Data.Id);
            if (matchData == null)
                return false;

            if (Server.GetClientById(userId) != null)
            {
                var thisUser = matchData.Players.Find(x => x.Id == Player.Data.Id);

                Log("send to client " + userId);
                Server.SendResponseLessMessageToOther(
                    userId, new ResponseDataPvpInvite(matchData, thisUser), CommandType.PvpGetInvite);

                return true;
            }
            else
            {
                return false;
            }

   
        }

        public bool SendInviteAccepted(string userId, string matchId, out PvpArenaMatchInfo matchInfo, out PvpArenaUserInfo userInfo)
        {
            var match = MatchSystem.GetMatch(matchId);

            matchInfo = match;
            userInfo = null;

            if (match == null)
                return false;

            userInfo = MatchSystem.AddPlayer(match, Player?.Data?.Id);

            if (userInfo != null)
            {
                Server.SendResponseLessMessageToOther(
                    userId, new ResponseDataPvpInviteAccepted(new PvpArenaUserInfo(Player.Data), match),
                    CommandType.PvpGetInviteAccepted);

                return true;
            }

            return false;
        }

        public void SendInviteDenied(string id)
        {
            Server.SendResponseLessMessageToOther(
                id, new ResponseDataPvpInviteCancel(new PvpArenaUserInfo(Player.Data)), CommandType.PvpGetInviteDenied);
        }

        public void SendInviteCancel(string id)
        {
            Server.SendResponseLessMessageToOther(
                id, new ResponseDataPvpInviteCancel(new PvpArenaUserInfo(Player.Data)), CommandType.PvpGetInviteCancelled);
        }

        public void SendUpdateWalletCrystals()
        {
            if (Player?.Data?.Wallet == null)
                return;

            var crystals = Player.Data.Wallet.Currencies.Find(x => x.Type == CurrencyType.Crystals)?.Amount ?? 0;

            ClientHandler.SendRequestNetworkMessage<SendData>(
                new ResponseDataWalletCrystalsUpdate(crystals), (int)CommandType.GetWalletCrystals);
        }

        public void UpdatePvpStats()
        {
            var data = GetByMongoId(Player.Id.ToString());

            Player.Data.Pvp = data.Data.Pvp;

            ClientHandler.SendRequestNetworkMessage<SendData>(
                new ResponseDataPvpInfoUpdate(Player.Data.Pvp), (int)CommandType.PvpGetInfoUpdate);
        }

        public void PvpConfirmResult()
        {
            var arena = MatchSystem.GetMyMatch(Player.Data.Id);

            if (arena != null)
            {
                MatchSystem.Confirm(arena, Player.Data.Id);
            }
        }

        public ServerAddress GetServerAddress(PlatformType platform, string version)
        {
            return FileSystem.Addresses.FirstOrDefault();
        }

        public ServerAddress GetServerAddress()
        {
            return FileSystem.Addresses.FirstOrDefault();
        }

        public void SendEmoji(string valueMatchId, int valueId)
        {
            MatchSystem.SendEmoji(valueMatchId, valueId);
        }

        public void SendResponseLessNetworkMessage(SendData data, CommandType command)
        {
            if(!IsDestroyed)
            {

              Log($"[SendMessage][{command}]");
               ClientHandler.SendResponseLessNetworkMessage<SendData>(data, (int)command);


            }
        }

        public void SendResponseLessNetworkMessage(SendData data, int command, string id)
        {
            if (!IsDestroyed)
            {

                Log($"[SendMessage][{(CommandType)command}]");
                ClientHandler.SendResponseLessNetworkMessage<SendData>(data, (int)command, id);


            }
        }

        public void SendResponseLessNetworkMessage(SendData data, CommandType command, string id)
        {
            if (!IsDestroyed)
            {
               Log($"[SendMessage][{command}]");
               ClientHandler.SendResponseLessNetworkMessage<SendData>(data, (int)command, id);
            }
        }

        public void FixProfile()
        {
            try
            {
                if (Player.Data.Dungeon.Tiers == null)
                {
                    Player.Data.Dungeon.Tiers = new List<Tier>();
                }

                if (Player.Data.Dungeon.Tiers.Count == 0)
                {
                    Player.Data.Dungeon.Tiers.Add(new Tier(0, true));
                }

                var isOpen = true;
                for (var index = 0; index < Player.Data.Dungeon.Tiers.Count; index++)
                {
                    var dungeonTier = Player.Data.Dungeon.Tiers[index];

                    if (!dungeonTier.IsOpen && index != 0)
                        isOpen = false;

                    dungeonTier.IsOpen = isOpen;
                }

                SavePlayer();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void BugReport(string deviceId, string valueStr)
        {
            FileSystem.AddBugReport(deviceId, Player?.Data?.Id ?? "", valueStr);
        }

        public void AddSendPackage(SendData responseData, int packCommand, string packId)
        {
            try
            {
                ResponsesArchive?.Add(new ResponseOperation(packId, packCommand, responseData));
            }
            catch (Exception e)
            {
                Log(e.ToString(), true);
            }

        }
    }
}