using System.IO;
using System.Linq;
using System.Reflection;
using Unbroken.LaunchBox.Plugins.Data;

namespace Lazy_Ini_Tweaker.Core
{
    public static class GameHelper
    {
        private static string PluginDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string ConfigsDir => $"{PluginDir}\\Configs";

        public static string GetGameConfigDir(IGame game)
        {
            var safePlatformName = GetNameSafeString(game.Platform);
            var safeGameName = GetNameSafeString(game.Title);
            
            var gameConfigDir = $"{ConfigsDir}\\[{safePlatformName}] {safeGameName}";
            return gameConfigDir;
        }

        private static string GetNameSafeString(string str)
        {
            var safeStr = Path.GetInvalidFileNameChars().Aggregate(str, (s, c) => s.Replace(c.ToString(), ""));
            return safeStr;
        }
    }
}
