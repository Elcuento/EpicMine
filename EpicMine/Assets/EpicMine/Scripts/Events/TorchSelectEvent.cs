using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public class TorchSelectEvent
    {
        public Torch Torch;

        public TorchSelectEvent(Torch torch)
        {
            Torch = torch;
        }
    }
}