using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseEffectsCheck : Response<SendData>
    {

        public ResponseEffectsCheck(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var dateNow = Utils.GetUnixTime();
                var nextDay = dateNow + 24 * 60 * 60 ;

                var crystalsToAdd = 0;

                foreach (var buff in Peer.Player.Data.Effect.BuffList)
                {
                    if (buff.Time < dateNow || buff.NextCheck > dateNow)
                        continue;

                    if (buff.Type == BuffType.Currency)
                    {
                        foreach (var f in buff.Values)
                        {
                            if (f.Type == BuffValueType.CrystalsByDay)
                            {
                                crystalsToAdd += (int)f.Value;
                                buff.NextCheck = nextDay;
                            }
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