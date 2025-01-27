namespace BlackTemple.EpicMine
{
    public class MineSceneEmptySection : MineSceneSection
    {
        public override void SetReady()
        {
            base.SetReady();
            SetPassed(0);
        }
    }
}