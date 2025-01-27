namespace BlackTemple.EpicMine.Dto
{
    public class FieldAttackPoint
    {
        public AttackPointType PointType;

        public int Size;
        public int X;
        public int Y;

        public FieldAttackPoint()
        {

        }

        public FieldAttackPoint(Core.FieldAttackPoint point)
        {
            PointType = point.PointType;
            X = point.X;
            Y = point.Y;
            Size = point.Size;
        }
    }
}

