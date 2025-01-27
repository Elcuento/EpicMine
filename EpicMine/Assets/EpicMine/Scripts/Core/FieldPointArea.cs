using System.Collections.Generic;
using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class FieldPointArea : Field
    {
        public FieldRulesType RulesType;

        public int MaxPoints;

        public Dictionary<string, float> Figures;

        public void Default(List<FieldFigure> figures)
        {
            Figures = new Dictionary<string, float>();
            var chance = 100 / figures.Count;

            foreach (var fieldFigure in figures)
            {
                Figures.Add(fieldFigure.Id, chance);
            }

            Grid = new FieldAttackPoint[19, 17];

            LengthX = Grid.GetLength(0);
            LengthY = Grid.GetLength(1);

            for (var i = 0; i < LengthX; i++)
            for (var j = 0; j < LengthY; j++)
            {
                Grid[i, j] = new FieldAttackPoint();
                Grid[i, j].Default(i, j);
            }

            MaxPoints = 8;
            RulesType = FieldRulesType.Free;

            MaxXPoint = GetMaxXPoint();
            MaxYPoint = GetMaxYPoint();
        }
        public FieldPointArea() { }
        public FieldPointArea(Dto.FieldPointArea field)
        {
            if (field == null)
                return;

            LengthX = field.Grid.GetLength(0);
            LengthY = field.Grid.GetLength(1);

            Grid = new FieldAttackPoint[LengthX, LengthY];
            Id = field.Id;

            for (var i = 0; i < LengthX; i++)
            for (var j = 0; j < LengthY; j++)
            {
                Grid[i, j] = new FieldAttackPoint(field.Grid[i, j]);
            }

            RulesType = field.RulesType;
            Figures = field.Figures;
            MaxPoints = field.MaxPoints;
        }
    }

}