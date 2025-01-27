using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public class Field
    {
        public string Id;

        public FieldAttackPoint[,] Grid;


        public Field()
        {

        }

       
        public Field(Core.Field field)
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
        }
    }
}

