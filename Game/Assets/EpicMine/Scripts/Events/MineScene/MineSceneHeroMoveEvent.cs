namespace BlackTemple.EpicMine
{
    public struct MineSceneHeroMoveEvent
    {
        public bool IsMoving;

        public MineSceneHeroMoveEvent(bool isMoving)
        {
            IsMoving = isMoving;
        }
    }
}