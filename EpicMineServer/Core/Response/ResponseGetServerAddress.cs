using System.Collections.Generic;
using AMTServer.Dto;
using AMTServerDLL.Dto;
using EpicMineServerDLL.Static.Enums;


namespace AMTServer.Core.Response
{
    public class ResponseGetServerAddress : Response<RequestDataGetAddress>
    {

        private readonly List<ServerInfo> _servers;

        public ResponseGetServerAddress(ClientPeer peer, Package pack, List<ServerInfo> servers) : base(peer, pack)
        {
            _servers = servers;
        }

        protected override bool OnProcess()
        {
            if (_servers.Count > 0)
            {
                ResponseData = new ResponseDataGetServerAddress(_servers[0].Address.Ip, _servers[0].Address.Port, ServerAddressType.Main);
            }
            else
            {
                var address = Value == null ? Peer.GetServerAddress() : Peer.GetServerAddress(Value.Platform, Value.Version);
                ResponseData = new ResponseDataGetServerAddress(address.Ip, address.Port, address.Type);
            }
          
            return true;
        }
    }
}