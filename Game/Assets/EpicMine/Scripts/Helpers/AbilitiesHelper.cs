using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class AbilitiesHelper
    {
        public static Color GetTextAbilityColor(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.ExplosiveStrike:
                    return App.Instance.ReferencesTables.Colors.FireTextColor;
                case AbilityType.Freezing:
                    return App.Instance.ReferencesTables.Colors.FrostTextColor;
                case AbilityType.Acid:
                    return App.Instance.ReferencesTables.Colors.AcidTextColor;
                default:
                    return Color.yellow;
            }
        }
    }
}
