namespace BlackTemple.EpicMine.Dto
{
    public class FieldFigure : Field
    {

        public FieldFigure(Core.FieldFigure field) : base(field)
        {
            /* var fieldSizeX = field.Grid.GetLength(0);
             var fieldSizeY = field.Grid.GetLength(1);

             Grid = new AttackPoint[fieldSizeX, fieldSizeY];
             Id = field.Id;

             for (var i = 0; i < fieldSizeX; i++)
             for (var j = 0; j < fieldSizeY; j++)
             {
                 Grid[i, j] = new AttackPoint(field.Grid[i, j]);
             }*/

        }
    }
}

