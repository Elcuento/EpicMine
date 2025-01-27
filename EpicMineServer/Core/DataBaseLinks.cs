using AMTServer.Dto;
using MongoDB.Driver;

namespace AMTServer.Core
{
    public class DataBaseLinks
    {
        public MongoCollectionBase<Dto.Player> UserCollection
        {
            get
            {
             //   lock (_userCollection)
                {
                    return _userCollection;
                }
            }
        }
        public MongoCollectionBase<PlayerResponseArchive> PlayerResponseArchive
        {
            get
            {
             //   lock (_playerResponseArchive)
                {
                    return _playerResponseArchive;
                }
            }
        }

        public MongoCollectionBase<RatingInfo> RatingInfoCollection
        {
            get
            {
                //lock (_ratingInfoCollection)
                {
                    return _ratingInfoCollection;
                }
            }
        }

        public MongoCollectionBase<Dto.ServerInfo> ServerCollections
        {
            get
            {
               // lock (_serverCollection)
                {
                    return _serverCollection;
                }
            }
        }
        public MongoCollectionBase<Dto.PlayerPurchase> UserPurchaseCollection
        {
            get
            {
             //   lock (_userPurchaseCollection)
                {
                    return _userPurchaseCollection;
                }
            }
        }
        public MongoCollectionBase<Dto.PlayerPvpRating> UserPvpRatingCollection
        {
            get
            {
             //   lock (_userPvpRatingCollection)
                {
                    return _userPvpRatingCollection;
                }
            }
        }
        public MongoCollectionBase<Dto.PlayerMineRating> UserMineRatingCollection
        {
            get
            {
               // lock (_userMineRatingCollection)
                {
                    return _userMineRatingCollection;
                }
            }
        }
        private MongoCollectionBase<PlayerResponseArchive> _playerResponseArchive { get; set; }
        private MongoCollectionBase<RatingInfo> _ratingInfoCollection { get; set; }
        private MongoCollectionBase<ServerInfo> _serverCollection { get; set; }
        private MongoCollectionBase<Player> _userCollection { get; set; }
        private MongoCollectionBase<PlayerPurchase> _userPurchaseCollection { get; set; }
        private MongoCollectionBase<PlayerPvpRating> _userPvpRatingCollection { get; set; }
        private MongoCollectionBase<PlayerMineRating> _userMineRatingCollection { get; set; }



        public DataBaseLinks(string databaseName, string dbIpString)
        {
            var dbClient = new MongoClient(dbIpString);
            var db = dbClient.GetDatabase(databaseName);

            _userCollection = (MongoCollectionBase<Player>) 
                db.GetCollection<Player>("Users");

            _userPurchaseCollection =
                (MongoCollectionBase<PlayerPurchase>)
                db.GetCollection<PlayerPurchase>("UsersPurchase");

            _userPvpRatingCollection =
                (MongoCollectionBase<PlayerPvpRating>)
                db.GetCollection<PlayerPvpRating>("UsersPvpRating");

            _userMineRatingCollection =
                (MongoCollectionBase<PlayerMineRating>)
                db.GetCollection<PlayerMineRating>("UsersMineRating");

            _serverCollection =
                (MongoCollectionBase<ServerInfo>)
                db.GetCollection<ServerInfo>("ServersInfo");

            _ratingInfoCollection =
                (MongoCollectionBase<RatingInfo>)
                db.GetCollection<RatingInfo>("RatingInfo");

            _playerResponseArchive =
                (MongoCollectionBase<PlayerResponseArchive>)
                db.GetCollection<PlayerResponseArchive>("UsersResponse");
        }
    }
}
