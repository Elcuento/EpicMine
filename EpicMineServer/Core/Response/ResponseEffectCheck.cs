using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseEffectCheck : Response<RequestDataEffectCheck>
    {

        public ResponseEffectCheck(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var buff = Peer.Player.Data.Effect.BuffList.Find(x => x.Id == Value.Id);

                var dateNow = Utils.GetUnixTime();
                var nextDay = dateNow + 24 * 60 * 60;

                var crystalsToAdd = 0;

                if (buff.Time < dateNow || buff.NextCheck > dateNow)
                    return false;

                if (buff.Type == BuffType.Currency)
                {
                    foreach (var f in buff.Values)
                    {
                        if (f.Type == BuffValueType.CrystalsByDay)
                        {
                            crystalsToAdd += (int) f.Value;
                            buff.NextCheck = nextDay;
                        }
                    }
                }

                Peer.AddCurrency(CurrencyType.Crystals, crystalsToAdd);

                ResponseData = new ResponseDataAddCrystal(crystalsToAdd);

                Peer.SavePlayer();
            }

            return true;
        }
    }
}