
namespace AMTServerDLL.Dto
{
    public class ResponseDataPrestigeReward : SendData
    {
        public string PickaxeId;

        public ResponseDataPrestigeReward(string pickaxeId)
        {
            PickaxeId = pickaxeId;
        }
    }
}