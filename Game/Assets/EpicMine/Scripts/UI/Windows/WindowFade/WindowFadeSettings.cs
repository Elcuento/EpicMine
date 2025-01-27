using UnityEngine;

namespace BlackTemple.EpicMine
{
    public struct WindowFadeSettings
    {
        public float Time;
        public Color Color;

        public WindowFadeSettings(float time, Color color)
        {
            Time = time;
            Color = color;
        }
    }
}