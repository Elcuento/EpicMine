using CommonDLL.Static;
using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class Player
    {
        public ObjectId Id { get; private set; }
        public CommonDLL.Dto.Player Data { get; private set; }

        public string DeviceId { get; private set; }

        public string FireBaseId { get; private set; }
        public string FaceBookId { get; private set; }
        public string GoogleId { get; private set; }

        public long LastOnlineDate { get; private set; }

        public Player(string id, string deviceId, StaticData staticData)
        {
            Data = new CommonDLL.Dto.Player(id)
            {
                Blacksmith =
                {
                    SelectedPickaxe = staticData.Pickaxes[0].Id
                },
                TorchesMerchant =
                {
                    SelectedTorch = staticData.Torches[0].Id
                }
            };

            DeviceId = deviceId;

            /*  var firstTier = staticData.Tiers.FirstOrDefault();
              if (firstTier != null)
              {
                  Data.Dungeon.Tiers = new List<Tier>();
  
                  Data.Dungeon.Tiers.Add(new Tier(0, true, new List<Mine>
                  {
                      new Mine(0, false, 0, 0, 0, false, false),
                      new Mine(1, false, 0, 0, 0, false, false),
                      new Mine(2, false, 0, 0, 0, false, false),
                      new Mine(3, false, 0, 0, 0, false, false),
                      new Mine(4, false, 0, 0, 0, false, false),
  
                  }, new List<string>()));
              }*/
        }


        public void SetData(CommonDLL.Dto.Player data)
        {
            Data = data;
        }

        public void SetDeviceId(string id)
        {
            DeviceId = id;
        }
        public void SetFaceBookId(string id)
        {
            FaceBookId = id;
        }
        public void SetFireBaseId(string id)
        {
            FireBaseId = id;
        }

        public void SetGoogleId(string id)
        {
            GoogleId = id;
        }

        public void SetLastOnlineDate(long date)
        {
            LastOnlineDate = date;
        }
    }
}
