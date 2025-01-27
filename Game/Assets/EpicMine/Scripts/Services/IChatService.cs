using System;

namespace BlackTemple.EpicMine
{
    public interface IChatService
    {
        void Initialize();

        void LogIn(string userId, string userName, Action onLogin);

        void OnLogIn(bool successes, string message = "");

        void Show();

        void Close();

        void ChangeName(string name);

    }

}