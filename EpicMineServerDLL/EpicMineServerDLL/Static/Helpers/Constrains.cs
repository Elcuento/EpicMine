using System.Collections.Generic;
using EpicMineServerDLL.Static.Enums;

namespace EpicMineServerDLL.Static.Helpers
{
    public static class Constrains
    {
        private static readonly List<CommandType> _commandWithoutLogin = new List<CommandType>
        {
            CommandType.Login,
            CommandType.ReLogin,
            CommandType.LoginWithFaceBook,
            CommandType.LoginWithGoogle,
            CommandType.GetNews,
            CommandType.GetServerStatus,
            CommandType.GetServerAddress,
            CommandType.BugReport
        };

        public static bool IsSuccessesWithoutLogin(CommandType type)
        {
            return _commandWithoutLogin.Contains(type);
        }
    }
}
