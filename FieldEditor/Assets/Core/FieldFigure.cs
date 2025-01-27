using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class FieldFigure : Field
    {
        public FieldFigure(int sizeX, int sizeY, string id) : base (sizeX,sizeY,id)
        {

        }

        public FieldFigure(BlackTemple.EpicMine.Dto.Field field)
        {
            var pointArea = field as BlackTemple.EpicMine.Dto.FieldFigure;

            if (pointArea == null)
            {
                Debug.LogError("error");
                return;
            }

            var sizeX = field.Grid.GetLength(0);
            var sizeY = field.Grid.GetLength(1);

            Grid = new FieldAttackPoint[sizeX, sizeY];

            for (var i = 0; i < sizeX; i++)
            for (var j = 0; j < sizeY; j++)
                Grid[i, j] = new FieldAttackPoint(field.Grid[i, j]);

            Id = field.Id;
        }

     

        public override BlackTemple.EpicMine.Dto.Field ExportData()
        {
            return new BlackTemple.EpicMine.Dto.FieldFigure(this);
        }
    }
}
