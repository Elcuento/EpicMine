using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Tier
    {
        public int Number { get; }

        public CommonDLL.Static.Tier StaticTier { get; }

        public bool IsOpen { get; private set; }

        public List<Mine> Mines { get; }

        public List<string> UnlockedDropItems { get; }

        public int GhostActionsCount => Mines.Count(x=>x.IsGhostAppear);

        public bool IsLast => Number == App.Instance.StaticData.Tiers.Count - 1;

        public bool IsComplete => IsOpen && Mines.All(m => m.IsComplete);

        public Mine LastOpenedMine
        {
            get { return Mines.FindLast(x => x.IsOpen); }
        }


        public Tier(int number)
        {
            Number = number;
            StaticTier = App.Instance.StaticData.Tiers[Number];

            IsOpen = number == 0;
            UnlockedDropItems = new List<string>();
            Mines = new List<Mine>();

            for (var i = 0; i < LocalConfigs.TierMinesCount; i++)
            {
                var mine = new Mine(this, i);
                Mines.Add(mine);
            }
        }
        public Tier(int number, bool isOpen)
        {
            Number = number;
            StaticTier = App.Instance.StaticData.Tiers[Number];

            IsOpen = isOpen;
            UnlockedDropItems = new List<string>();
            Mines = new List<Mine>();

            for (var i = 0; i < LocalConfigs.TierMinesCount; i++)
            {
                var mine = new Mine(this, i);

                Mines.Add(mine);
            }
        }

        public Tier(int number, Tier tierData)
        {
            Number = number;
            StaticTier = App.Instance.StaticData.Tiers[Number];

            IsOpen = tierData.IsOpen;
            UnlockedDropItems = tierData.UnlockedDropItems ?? new List<string>();
            Mines = new List<Mine>();

            for (var i = 0; i < LocalConfigs.TierMinesCount; i++)
            {
                Mine mine;

                if (tierData.Mines != null && tierData.Mines.Count > i)
                {
                    var mineData = tierData.Mines[i];
                    mine = new Mine(this, i, mineData);
                }
                else
                    mine = new Mine(this, i);

                Mines.Add(mine);
            }
        }
        public Tier(int number,  CommonDLL.Dto.Tier tierData)
        {
            Number = number;
            StaticTier = App.Instance.StaticData.Tiers[Number];

            IsOpen = tierData.IsOpen;

            UnlockedDropItems = tierData.UnlockedDropItems ?? new List<string>();
            Mines = new List<Mine>();

            for (var i = 0; i < LocalConfigs.TierMinesCount; i++)
            {
                Mine mine;

                if (tierData.Mines != null && tierData.Mines.Count > i)
                {
                    var mineData = tierData.Mines[i];
                    mine = new Mine(this, i, mineData);
                }
                else
                    mine = new Mine(this, i);

                Mines.Add(mine);
            }
        }

        public void Open(Action<bool> onComplete = null, bool dev = false)
        {
            if (IsOpen)
                return;


            var staticData = App.Instance.StaticData; //.GetStaticData();

            var staticTiersCount = staticData.Tiers.Count;
            var lastOpenTier = 0;

            for (var index = 0; index < App.Instance.Player.Dungeon.Tiers.Count; index++)
            {
                if (App.Instance.Player.Dungeon.Tiers[index].IsOpen || index == 0)
                {
                    lastOpenTier = index;
                    continue;
                }

                if (!App.Instance.Player.Dungeon.Tiers[index].IsOpen)
                    break;

            }

            var nextClosedTierNumber = lastOpenTier + 1;

            if (staticTiersCount <= nextClosedTierNumber)
            {
                Debug.LogError("Tier is to much, return true any way" + nextClosedTierNumber);
                return;
            }

            var tierOpenCost = staticData.Tiers[nextClosedTierNumber - 1].RequireArtefacts;


            if (App.Instance.Player.Artefacts.Amount < tierOpenCost && !dev)
            {
                Debug.LogError("No artifacts");
                return;
            }

            while (App.Instance.Player.Dungeon.Tiers.Count < lastOpenTier)
            {
                App.Instance.Player.Dungeon.Tiers.Add(new Tier(App.Instance.Player.Dungeon.Tiers.Count, true));
            }


            if (!dev)
            {
                App.Instance.Player.Artefacts.Remove(tierOpenCost);
            }

            IsOpen = true;

            EventManager.Instance.Publish(new TierOpenEvent(this));

            onComplete?.Invoke(true);

        }

        public void UnlockDropItem(string itemStaticId)
        {
            if (!IsDropItemUnblocked(itemStaticId))
            {
                UnlockedDropItems.Add(itemStaticId);
                EventManager.Instance.Publish(new TierUnlockDropItemEvent(this, itemStaticId));
            }
        }

        public bool IsDropItemUnblocked(string itemStaticId)
        {
            return UnlockedDropItems.Contains(itemStaticId);
        }
    }
}