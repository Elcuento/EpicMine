using System.Collections.Generic;

namespace BlackTemple.EpicMine.Dto
{
    public struct RedDots
    {
        public List<string> NewPickaxes;

        public List<string> ViewedDailyTasks;

        public List<string> NewRecipes;

        public List<string> NewShopPacks;

        public List<string> ViewedItems;

        public bool IsNewLeaderBoard;

        public bool IsPvpWindowShowed;

        public bool IsChatShowed;

        public bool IsQuestsChangeShowed;

        public bool IsTorchesWindowShowed;

        public RedDotSimple AbilitiesSkillsDot;

        public List<RedDotState> AbilitiesDot;

        public List<RedDotState> SkillsDot;

        public RedDots(List<string> newPickaxes, List<string> viewedDailyTasks, List<string> newRecipes,
            List<string> newShopPacks, List<string> viewedItems, bool isQuestsChangeShowed, bool isNewLeaderBoard, bool isPvpWindowShowed, bool isTorchesWindowShowed, bool isChatShowed, List<RedDotState> abilitiesDot,
            List<RedDotState> skillsDot, RedDotSimple abilitiesSkillsDot)
        {
            NewPickaxes = newPickaxes;
            ViewedDailyTasks = viewedDailyTasks;
            NewRecipes = newRecipes;
            NewShopPacks = newShopPacks;
            ViewedItems = viewedItems;
            IsNewLeaderBoard = isNewLeaderBoard;
            AbilitiesDot = abilitiesDot;
            SkillsDot = skillsDot;
            AbilitiesSkillsDot = abilitiesSkillsDot;
            IsPvpWindowShowed = isPvpWindowShowed;
            IsTorchesWindowShowed = isTorchesWindowShowed;
            IsChatShowed = isChatShowed;
            IsQuestsChangeShowed = isQuestsChangeShowed;
        }


    }
}