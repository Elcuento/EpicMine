using BlackTemple.Common;

namespace BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers
{
    public abstract class ShopTrigger
    {
        public string ShopPackId;
        public bool IsCompleted;

        protected ShopTrigger(string offerId, bool isCompleted)
        {
            ShopPackId = offerId;
            IsCompleted = isCompleted;
        }

        protected void SetCompleted()
        {
            IsCompleted = true;
            OnCompleted();

            EventManager.Instance.Publish(new ShopTriggerCompleteEvent(this));
        }

        protected void SetReset()
        {
            IsCompleted = false;
            OnReset();
            SetStart();
        }

        public void SetStart()
        {
            OnStart();
            EventManager.Instance.Publish(new ShopTriggerStartEvent(this));
        }

        public void SetCheck()
        {
            OnCheck();
        }

        public abstract void OnCompleted();
        public abstract void OnReset();
        public abstract void OnStart();
        public abstract void OnCheck();

    }
}
