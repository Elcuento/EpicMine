using System;
using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class PlayerResponseArchive
    {
        public ObjectId Id;
        public string PlayerId;
        public List<ResponseOperation> Archives;
        public long LastTimeUpdate;

        public PlayerResponseArchive(string playerId)
        {
            Id = ObjectId.GenerateNewId();
            PlayerId = playerId;
            Archives = new List<ResponseOperation>();
        }

        public void Add(ResponseOperation op)
        {
            lock (Archives)
            {
                Archives.Add(op);

                if (Archives.Count > 15)
                    Archives.RemoveRange(10, 5);

                LastTimeUpdate = Utils.GetUnixTime();
            }


        }

        public bool IsExist(Package pack)
        {
            lock (Archives)
            {
                var exist = Archives.Find(x => x.Compare(pack));
                return exist != null;
            }

        }

        public ResponseOperation Get(Package package)
        {
            lock (Archives)
            {
                var exist = Archives.Find(x => x.Compare(package));
                return exist;

            }
        }
    }
}
