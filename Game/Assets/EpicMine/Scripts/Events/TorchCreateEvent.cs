using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct TorchCreateEvent
    {
        public Torch Torch;

        public TorchCreateEvent(Torch torch)
        {
            Torch = torch;
        }
    }
}