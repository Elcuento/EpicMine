using System;
using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class PvpArenaMatchInfo
    {
        public string Id;
        public List<PvpArenaUserInfo> Players;
        public List<string> ConfirmList;
        public List<int> Walls;
        public bool Locked;
        public int MaxPlayers;
        public PvpArenaMatchType Type;
        public int Arena;
        public long EndTime;
        public long MatchTime;
        public long TimeOutTime;

        public PvpArenaMatchFilter Filter;

        public PvpArenaMatchStatusType Status;

        public PvpArenaMatchResult Result;

        public PvpArenaMatchInfo(PvpArenaMatchFilter filter)
        {
            Id = Guid.NewGuid().ToString();
            Players = new List<PvpArenaUserInfo>();
            ConfirmList = new List<string>();
            Walls = new List<int>();
            Filter = filter;
        }

        public void SetResult(PvpArenaMatchResult result)
        {
            Result = result;
        }

        public PvpArenaMatchInfo()
        {
        }

        public void AddPlayer(string id, string nick, int rating, string pickaxe, string torch)
        {
            Players.Add(new PvpArenaUserInfo
            {
                Id = id,
                Pickaxe = pickaxe,
                Torch = torch,
                Name = nick,
                Rating = rating,
            });

        }

        public void Confirm(string playerId)
        {
            if(!ConfirmList.Contains(playerId))
            ConfirmList.Add(playerId);
        }

        public void AddPlayer(PvpArenaUserInfo info)
        {
            Players.Add(info);
        }

        public void AddPlayer(Player playerData)
        {
            Players.Add(new PvpArenaUserInfo
            {
                Id = playerData.Id,
                Pickaxe = playerData.Blacksmith?.SelectedPickaxe,
                Torch = playerData.TorchesMerchant?.SelectedTorch,
                Rating = playerData.Pvp.Rating,
                Name = playerData.Nickname,
            });

        }

        public bool IsFull()
        {
            return MaxPlayers == Players.Count;
        }

        public bool IsConfirmed(string player)
        {
            return ConfirmList.Contains(player);
        }


        public bool IsConfirmed()
        {
            foreach (var pvpArenaUserInfo in Players)
            {
                if(pvpArenaUserInfo.IsBot)
                    continue;

                if (!ConfirmList.Contains(pvpArenaUserInfo.Id))
                    return false;
            }

            return true;
        }

        public bool IsAllReady()
        {
            foreach (var pvpArenaUserInfo in Players)
            {
                if (!pvpArenaUserInfo.Ready)
                    return false;
            }

            return true;
        }

        public bool HasPlayer(string dataId)
        {
            foreach (var pvpArenaUserInfo in Players)
            {
                if (pvpArenaUserInfo.Id == dataId)
                    return true;
            }

            return false;
        }

        public PvpArenaUserInfo GetPlayer(string dataId)
        {
            foreach (var pvpArenaUserInfo in Players)
            {
                if (pvpArenaUserInfo.Id == dataId)
                    return pvpArenaUserInfo;
            }

            return null;
        }
    }
}
