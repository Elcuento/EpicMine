using BlackTemple.Common;

namespace BlackTemple.EpicMine.Core
{
    public class Artifacts
    {
        public long Amount { get; private set; }

        public Artifacts(long amount)
        {
            Amount = amount;
        }
   

        public void Add(long amount)
        {
            if (amount <= 0)
                return;

            Amount += amount;
            var amountChangeEvent = new ArtefactsAmountChangeEvent(Amount);
            EventManager.Instance.Publish(amountChangeEvent);
        }

        public bool Remove(long amount)
        {
            if (Amount < amount)
                return false;

            Amount -= amount;

            var amountChangeEvent = new ArtefactsAmountChangeEvent(Amount);
            EventManager.Instance.Publish(amountChangeEvent);
            return true;
        }

        public void Set(int i)
        {
            Amount = i;

            if (Amount < 0)
                Amount = 0;

            var amountChangeEvent = new ArtefactsAmountChangeEvent(Amount);
            EventManager.Instance.Publish(amountChangeEvent);
        }
    }
}