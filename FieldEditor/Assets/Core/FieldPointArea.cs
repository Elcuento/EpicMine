using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class FieldPointArea : Field
    {
        public FieldRulesType RulesType { get; private set; }
        public Dictionary<string, float> Figures { get; private set; } 
        public int MaxPoints { get; private set;  }

        public override BlackTemple.EpicMine.Dto.Field ExportData()
        {
            return new BlackTemple.EpicMine.Dto.FieldPointArea(this);
        }

        public FieldPointArea(BlackTemple.EpicMine.Dto.Field field)
        {
            var pointArea = field as BlackTemple.EpicMine.Dto.FieldPointArea;

            if(pointArea== null) { Debug.LogError("error");
                return;
            }

            var sizeX = field.Grid.GetLength(0);
            var sizeY = field.Grid.GetLength(1);

            Grid = new FieldAttackPoint[sizeX, sizeY];

            for (var i = 0; i < sizeX; i++)
            for (var j = 0; j < sizeY; j++)
                Grid[i, j] = new FieldAttackPoint(field.Grid[i, j]);

            Id = field.Id;
            MaxPoints = pointArea.MaxPoints;
            Figures = pointArea.Figures;
            RulesType = pointArea.RulesType;
        }

        public FieldPointArea(int sizeX, int sizeY, string id) : base(sizeX, sizeY, id)
        {
            MaxPoints = 10;
            Figures = new Dictionary<string, float>();
        }

        public void SetMaxPoints(int points)
        {
            MaxPoints = points;
        }

        public void SetRulesType(FieldRulesType type)
        {
            RulesType = type;
        }
    }
}
