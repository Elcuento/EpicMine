using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "Sounds")]
    public class SoundsReferencesTable : ScriptableObject
    {
        public AudioClip Birds;

        public AudioClip Flame;
        public AudioClip Wind;
        public AudioClip[] Steps;

        public AudioClip Click;
        public AudioClip OpenWindow;
        public AudioClip CloseWindow;
        public AudioClip Notification;

        public AudioClip Pay;
        public AudioClip Sell;
        public AudioClip CrystalsIncome;
        public AudioClip CharacteristicUpgrade;

        public AudioClip ChestFound;
        public AudioClip OpenChest;
        public AudioClip StartBreakingChest;
        public AudioClip GiftReady;
        public AudioClip OpenGift;

        public AudioClip BlacksmithCharacterHammer;

        public AudioClip WorkshopSlotSetRecipe;

        public AudioClip[] Tnts;
        public AudioClip Potion;

        public AudioClip PickaxeCreate;
        public AudioClip PickaxeDestroyed;
        public AudioClip PickaxeDestroyedWindow;
        public AudioClip[] PickaxeOuterHits;
        public AudioClip PickaxeInnerHit;
        public AudioClip[] PickaxeMisses;
        public AudioClip PickaxeCrit;

        public AudioClip HealthAttackPointHit;
        public AudioClip EnergyAttackPointHit;

        public AudioClip WallDestroyed;

        public AudioClip Door;
        public AudioClip LastDoor;
        public AudioClip MineComplete;

        public AudioClip MineBossHealing;

        public AudioClip[] ShopSounds;
        public AudioClip[] BlacksmithSounds;

        public AudioClip[] InstantDamages;
        public AudioClip[] IncreaseDamages;
        public AudioClip[] TickingDamages;

        [Header("Abilities")]
        public AudioClip DragonLightStart;
        public AudioClip DragonLightProgress;

        [Header("Quests")]
        public AudioClip QuestTaskGoalChanged;
        public AudioClip QuestTaskCompleted;
    }
}