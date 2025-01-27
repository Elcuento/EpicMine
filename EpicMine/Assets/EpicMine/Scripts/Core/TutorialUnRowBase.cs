namespace BlackTemple.EpicMine.Core
{
    public class TutorialUnRowBase
    {
        public TutorialUnRowStepIds Id;

        public TutorialUnRowBase(Dto.TutorialUnRowBase data)
        {
            Id = data.Id;
        }

        public TutorialUnRowBase(TutorialUnRowStepIds id)
        {
            Id = id;
        }
    }
}