
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class FieldFigure : Field
    {

        public FieldFigure(Dto.FieldFigure field)
        {
            if (field == null)
                return;

            var fieldSizeX = field.Grid.GetLength(0);
            var fieldSizeY = field.Grid.GetLength(1);

            Grid = new FieldAttackPoint[fieldSizeX, fieldSizeY];
            Id = field.Id;

            for (var i = 0; i < fieldSizeX; i++)
            for (var j = 0; j < fieldSizeY; j++)
            {
                Grid[i, j] = new FieldAttackPoint(field.Grid[i, j]);
            }

            Grid = GetShortGrid();

            LengthX = Grid.GetLength(0);
            LengthY = Grid.GetLength(1);
            MaxXPoint = GetMaxXPoint();
            MaxYPoint = GetMaxYPoint();
        }


        public FieldFigure(FieldFigure field)
        {
            if (field == null)
                return;

            var fieldSizeX = field.Grid.GetLength(0);
            var fieldSizeY = field.Grid.GetLength(1);

            Grid = new FieldAttackPoint[fieldSizeX, fieldSizeY];
            Id = field.Id;

            LengthX = field.LengthX;
            LengthY = field.LengthY;
            MaxXPoint = field.MaxXPoint;
            MaxYPoint = field.MaxYPoint;

            for (var i = 0; i < fieldSizeX; i++)
            for (var j = 0; j < fieldSizeY; j++)
            {
                var point = field.Grid[i, j];
                Grid[i, j] = new FieldAttackPoint(point.X, point.Y, point.Size, point.PointType);
            }
        }


        public FieldAttackPoint[,] GetShortGrid()
        {

            var maxX = 0;
            var maxY = 0;

            var minX = Grid.GetLength(0) - 1;
            var minY = Grid.GetLength(1) - 1;

            foreach (var attackPoint in Grid)
            {
                if(attackPoint.PointType == AttackPointType.Empty)
                    continue;

                if (attackPoint.X  + attackPoint.Size > maxX)
                    maxX = attackPoint.X + attackPoint.Size;

                if (attackPoint.X < minX)
                    minX = attackPoint.X;

                if (attackPoint.Y + attackPoint.Size > maxY)
                    maxY = attackPoint.Y + attackPoint.Size;

                if (attackPoint.Y < minY)
                    minY = attackPoint.Y;
            }



            var grid = new FieldAttackPoint[maxX - minX, maxY - minY];

            var x = -1;
            for (var i = minX; i < maxX; i++)
            {
                x++;
                var y = -1;
                for (var j = minY; j < maxY; j++)
                {
                    y++;
                    grid[x, y] = Grid[i, j];
                    grid[x, y].ResetPosition(x,y);
                }
            }
            return grid;
        }
    }

}