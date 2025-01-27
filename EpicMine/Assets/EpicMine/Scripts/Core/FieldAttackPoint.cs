


using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class FieldAttackPoint
    {
        public AttackPointType PointType { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Size { get; private set; }

        public FieldAttackPoint(int x, int y, int size = 1, AttackPointType type = AttackPointType.Default)
        {
            X = x;
            Y = y;

            Size = size;

            SetType(type);
        }

        public void ResetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Default(int x, int y)
        {
            X = x;
            Y = y;
            PointType = AttackPointType.Empty;
        }

        public FieldAttackPoint()
        {

        }

        public FieldAttackPoint(Dto.FieldAttackPoint point)
        {
            X = point.X;
            Y = point.Y;

            Size = point.Size;

            SetType(point.PointType);
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