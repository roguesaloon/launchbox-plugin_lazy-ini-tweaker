using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;
using Lazy_Ini_Tweaker.Core;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace Lazy_Ini_Tweaker.Plugins
{
    internal class SystemEventPlugin : ISystemEventsPlugin
    {
        private static IGame SelectedGame => PluginHelper.StateManager.GetAllSelectedGames().FirstOrDefault();
        private static readonly FileIniDataParser IniParser = new FileIniDataParser();

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public void OnEventRaised(string eventType)
        {
            switch (eventType)
            {
                case SystemEventTypes.GameStarting:
                    OnGameStarting();
                    break;
                case SystemEventTypes.GameExited:
                    OnGameExited();
                    break;
            }
        }

        private static void OnGameStarting()
        {
            void Action(KeyData path, string gameConfigDir)
            {
                File.Copy($"{path.Value}\\{path.KeyName}.ini", $"{path.Value}\\{path.KeyName}.base.ini", true);

                var targetConfig = IniParser.ReadFile($"{path.Value}\\{path.KeyName}.ini");
                var tweakConfig = IniParser.ReadFile($"{gameConfigDir}\\{path.KeyName}.ini");

                targetConfig.Merge(tweakConfig);
                IniParser.WriteFile($"{path.Value}\\{path.KeyName}.ini", targetConfig, Encoding.UTF8);
            }

            LoopConfigPaths(Action);
        }

        private static void OnGameExited()
        {
            void Action(KeyData path, string gameConfigDir)
            {
                File.Delete($"{path.Value}\\{path.KeyName}.ini");
                File.Move($"{path.Value}\\{path.KeyName}.base.ini", $"{path.Value}\\{path.KeyName}.ini");
            }

            LoopConfigPaths(Action);
        }

        private static void LoopConfigPaths(Action<KeyData, string> action)
        {
            var gameConfigDir = GameHelper.GetGameConfigDir(SelectedGame);

            if (!Directory.Exists(gameConfigDir)) return;

            var gameConfigPaths = IniParser.ReadFile($"{gameConfigDir}\\lazy_ini_tweaker.ini")["Paths"];

            foreach (var path in gameConfigPaths)
            {
                action.Invoke(path, gameConfigDir);
            }
        }
    }
}
