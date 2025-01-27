using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateCurrency : Response<RequestDataUpdateCurrency>
    {

        public ResponseUpdateCurrency(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Wallet?.Currencies == null)
                {
                    Peer.Player.Data.Wallet = new Wallet
                    {
                        Currencies = new List<Currency>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Wallet.Currencies.Find(x => x.Type == item.Type);
                    Peer.Player.Data.Wallet.Currencies.Remove(itemData);

                    Peer.Player.Data.Wallet.Currencies.Add(new Currency(item.Type, item.Amount));
                }

                Peer.SavePlayer();
            }

            return true;
        }

    }
}