namespace BlackTemple.EpicMine
{
    public class Controllers
    {
        public AudioController AudioController { get; }

        public NotificationsController NotificationsController { get; }

        public RateUsController RateUsController { get; }

        public RatingsController RatingsController { get; }

        public RedDotsController RedDotsController { get; }

        public AttackPointProbabilityController AttackPointProbabilityController { get; }

        public DailyTasksController DailyTasksController { get; }

        public LocalPushNotificationsController LocalPushNotificationsController { get; }

        public TutorialController TutorialController { get; }

        public PlayerDataSynchronizationController PlayerDataSynchronizationController { get; }

        public ShopController ShopController { get; }

        public AutoMinerController AutoMinerController { get; }


        public Controllers()
        {
            AudioController = new AudioController();
            NotificationsController = new NotificationsController();
            RateUsController = new RateUsController();
            RedDotsController = new RedDotsController();
            AttackPointProbabilityController = new AttackPointProbabilityController();
            DailyTasksController = new DailyTasksController();
            LocalPushNotificationsController = new LocalPushNotificationsController();
            TutorialController = new TutorialController();
            ShopController = new ShopController();
            AutoMinerController = new AutoMinerController();
            RatingsController = new RatingsController();

        }

      
        public void Save()
        {
            RateUsController?.Save();
            RedDotsController?.Save();
            AttackPointProbabilityController?.Save();
            DailyTasksController?.Save();
            ShopController?.Save();
            RatingsController?.Save();
        }
    }
}