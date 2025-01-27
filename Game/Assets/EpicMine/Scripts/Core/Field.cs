using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class Field
    {
        public string Id;

        public FieldAttackPoint[,] Grid;

        public int LengthX;
        public int LengthY;

        public int MaxXPoint;
        public int MaxYPoint;


        public int GetPointsCount()
        {
            var count = 0;

            foreach (var i in Grid)
            {
                if (i.PointType != AttackPointType.Empty)
                    count++;
            }

            return count;
        }


        public int GetMaxXPoint()
        {
            var x = 0;
            foreach (var fieldAttackPoint in Grid)
            {
                var occupied = (fieldAttackPoint.X + 1) * fieldAttackPoint.Size;
                if (x < occupied)
                    x = occupied;
            }

            return x;
        }
        public int GetMaxYPoint()
        {
            var y = 0;
            foreach (var fieldAttackPoint in Grid)
            {
                var occupied = (fieldAttackPoint.Y + 1)* fieldAttackPoint.Size;
                if (y < occupied)
                    y = occupied;
            }

            return y;
        }

        public void Rotate()
        {
            var dem1 = Grid.GetLength(0);
            var dem2 = Grid.GetLength(1);

            var newArray = new FieldAttackPoint[dem2, dem1];

            for (var i = 0; i < dem1; i++)
            {
                for (var j = 0; j < dem2; j++)
                {
                    newArray[j, i] = Grid[i, j];
                }
            }

            Grid = newArray;

            LengthX = Grid.GetLength(0);
            LengthY = Grid.GetLength(1);

            MaxXPoint = GetMaxXPoint();
            MaxYPoint = GetMaxYPoint();
        }

        public void Reverse()
        {
            var dem1 = Grid.GetLength(0);
            var dem2 = Grid.GetLength(1);

            var newArray = new FieldAttackPoint[dem1, dem2];

            for (var i = 0; i < dem1; i++)
            {
                for (var j = 0; j < dem2; j++)
                {
                    newArray[i, j] = Grid[dem1 - (i + 1), dem2 - (j + 1)];
                }
            }

            Grid = newArray;

            LengthX = Grid.GetLength(0);
            LengthY = Grid.GetLength(1);

            MaxXPoint = GetMaxXPoint();
            MaxYPoint = GetMaxYPoint();
        }


    }

}