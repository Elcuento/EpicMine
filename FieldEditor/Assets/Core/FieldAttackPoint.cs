namespace BlackTemple.EpicMine.Core
{
    public class FieldAttackPoint
    {
        public AttackPointType PointType { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Size { get; private set; }

        public FieldAttackPoint(int x, int y, int size = 1)
        {
            X = x;
            Y = y;

            Size = size;
            SetType(AttackPointType.Empty);
        }

        public FieldAttackPoint(BlackTemple.EpicMine.Dto.FieldAttackPoint point)
        {
            X = point.X;
            Y = point.Y;

            Size = point.Size;
            SetType(point.PointType);
        }

        public void ResetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetType(AttackPointType type)
        {
            PointType = type;
        }
        public void SetSize(int size)
        {
            Size = size;
        }
        public void SetDefault()
        {
            Size = 1;
            PointType = AttackPointType.Empty;
        }

    }
}
