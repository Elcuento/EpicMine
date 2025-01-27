using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using Currency = BlackTemple.EpicMine.Dto.Currency;


namespace BlackTemple.EpicMine.Core
{
    public class AbilityLevel
    {
        public AbilityType Type { get; }

        public int Number { get; private set; }

        public CommonDLL.Static.AbilityLevel StaticLevel { get; private set; }

        public CommonDLL.Static.AbilityLevel NextStaticLevel { get; private set; }


        public bool CanUpgrade
        {
            get
            {
                if (NextStaticLevel == null)
                    return false;

                var costPack = new Pack();

                if (NextStaticLevel.CostCurrencyType != null)
                    costPack.Add(new Currency(NextStaticLevel.CostCurrencyType.Value, NextStaticLevel.CostCurrencyAmount));

                if (!string.IsNullOrEmpty(NextStaticLevel.CostItemId))
                    costPack.Add(new Item(NextStaticLevel.CostItemId, NextStaticLevel.CostItemAmount));

                return App.Instance.Player.Inventory.Has(costPack) && App.Instance.Player.Wallet.Has(costPack);
            }
        }


        public AbilityLevel(AbilityType type, int number)
        {
            Type = type;
            Number = number;

            UpdateStaticLevels();
        }


        public void Up(bool force = false)
        {
            if (NextStaticLevel == null)
                return;

            if (force)
            {
                Number++;
                UpdateStaticLevels();
                EventManager.Instance.Publish(new AbilityLevelChangeEvent(this));
                return;
            }

            var costPack = new Pack();

            if (NextStaticLevel.CostCurrencyType != null)
                costPack.Add(new Currency(NextStaticLevel.CostCurrencyType.Value, NextStaticLevel.CostCurrencyAmount));

            if (!string.IsNullOrEmpty(NextStaticLevel.CostItemId))
                costPack.Add(new Item(NextStaticLevel.CostItemId, NextStaticLevel.CostItemAmount));

            if (!App.Instance.Player.Inventory.Has(costPack) || !App.Instance.Player.Wallet.Has(costPack))
                return;

            if (costPack.Currencies.Count > 0)
            {
                var purchaseCurrency = NextStaticLevel.CostCurrencyType == null
                    ? NextStaticLevel.CostItemId
                    : NextStaticLevel.CostCurrencyType.Value.ToString();

                App.Instance
                    .Services
                    .AnalyticsService
                    .InAppPurchase(
                        Type.ToString(),
                        "Ability",
                        1,
                        NextStaticLevel.CostCurrencyAmount,
                        purchaseCurrency);
            }

            Number++;
            UpdateStaticLevels();

            App.Instance.Player.Inventory.Remove(costPack, SpendType.Using);
            App.Instance.Player.Wallet.Remove(costPack);

            var changeEvent = new AbilityLevelChangeEvent(this);
            EventManager.Instance.Publish(changeEvent);
        }


        private void UpdateStaticLevels()
        {
            StaticLevel = StaticHelper.GetAbilityLevel(Type, Number);
            NextStaticLevel = StaticHelper.GetAbilityLevel(Type, Number + 1);
        }
    }
}