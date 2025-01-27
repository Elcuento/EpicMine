using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Dto
{
    public class FieldPointArea : Field
    {

        public FieldRulesType RulesType;

        public int MaxPoints;

        public Dictionary<string, float> Figures;


        public FieldPointArea(Core.FieldPointArea field) : base(field)
        {
            if (field == null)
                return;
           /* var fieldSizeX = field.Grid.GetLength(0);
            var fieldSizeY = field.Grid.GetLength(1);

            Grid = new AttackPoint[fieldSizeX, fieldSizeY];
            Id = field.Id;

            for (var i = 0; i < fieldSizeX; i++)
            for (var j = 0; j < fieldSizeY; j++)
            {
                Grid[i, j] = new AttackPoint(field.Grid[i, j]);
            }*/

            RulesType = field.RulesType;
            Figures = field.Figures;
            MaxPoints = field.MaxPoints;
        }
    }


}

