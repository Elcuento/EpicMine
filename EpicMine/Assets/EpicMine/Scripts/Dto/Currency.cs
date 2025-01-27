using CodeStage.AntiCheat.ObscuredTypes;
using CommonDLL.Static;

namespace BlackTemple.EpicMine.Dto
{
    public struct Currency
    {
        public CurrencyType Type;

        public ObscuredLong Amount;

        public Currency(CurrencyType type, long amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}