namespace BlackTemple.EpicMine.Core
{
    public class Field
    {
        public string Id { get; protected set; }
        public FieldAttackPoint[,] Grid { get; protected set; }

        public Field()
        {

        }

        public virtual BlackTemple.EpicMine.Dto.Field ExportData()
        {
            return new BlackTemple.EpicMine.Dto.Field(this);
        }

        public Field(int sizeX, int sizeY, string id)
        {
            Grid = new FieldAttackPoint[sizeX, sizeY];
            Id = id;

            for(var i =0; i < sizeX; i++)
            for (var j = 0; j < sizeY; j++)
                Grid[i, j] = new FieldAttackPoint(i,j);
        }

        public FieldAttackPoint[,] GetShortGrid()
        {
            if (Grid.Length == 1)
                return Grid;

            var maxX = 0;
            var maxY = 0;

            var minX = Grid.GetLength(0) - 1;
            var minY = Grid.GetLength(1) - 1;

            foreach (var attackPoint in Grid)
            {
                if (attackPoint.PointType == AttackPointType.Empty)
                    continue;

                if (attackPoint.X > maxX)
                    maxX = attackPoint.X;

                if (attackPoint.X < minX)
                    minX = attackPoint.X;

                if (attackPoint.Y > maxY)
                    maxY = attackPoint.Y;

                if (attackPoint.Y < minY)
                    minY = attackPoint.Y;
            }

            var grid = new FieldAttackPoint[maxX - minX + 1, maxY - minY + 1];

            var x = -1;
            for (var i = minX; i < maxX + 1; i++)
            {
                x++;
                var y = -1;
                for (var j = minY; j < maxY + 1; j++)
                {
                    y++;
                    grid[x, y] = Grid[i, j];
                   // grid[x, y].ResetPosition(x, y);
                }
            }
            return grid;
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
                var occupied = (fieldAttackPoint.Y + 1) * fieldAttackPoint.Size;
                if (y < occupied)
                    y = occupied;
            }

            return y;
        }


        public void SetId(string id)
        {
            Id = id;
        }
    }
}
