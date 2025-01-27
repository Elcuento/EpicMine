namespace BlackTemple.EpicMine
{
    public struct WindowOpenEvent
    {
        public WindowBase Window;

        public WindowOpenEvent(WindowBase window)
        {
            Window = window;
        }
    }
}