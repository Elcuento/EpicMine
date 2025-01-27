namespace BlackTemple.EpicMine
{
    public struct WindowCloseEvent
    {
        public WindowBase Window;

        public WindowCloseEvent(WindowBase window)
        {
            Window = window;
        }
    }
}