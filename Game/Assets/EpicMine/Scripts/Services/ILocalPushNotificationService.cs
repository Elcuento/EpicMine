using System;

namespace BlackTemple.EpicMine
{
    public interface ILocalPushNotificationService
    {
        void Initialize();
        string Push(string title, string text, DateTime time);
        string Push(string title, string text, TimeSpan time);
        void Cancel(string notId);
        void CancelAll();
    }

}