using System.Diagnostics;
using System.IO;

namespace AlgoModSimpleWPF
{
    internal class MethodsBakkesMod
    {
        /// <summary>
        /// Enables AlgoMod.dll, disabled incompatible plugins and adds F7 algo_load bind
        /// </summary>
        /// <remarks>Incompatible plugins list is stored in <see cref="Vars.WebsiteInfo"/> and has less utility now that incompatibilty issues are fixed. It's still a valuable resource though</remarks>
        public static void BakkesCFG()
        {
            try
            {
                string cfgFolder = Path.Combine(InfoData.BakkesModPath, "cfg");
                string cfgPath = Path.Combine(cfgFolder, "plugins.cfg");
                string cfgTxtPath = Path.Combine(cfgFolder, "plugins.txt");
                string bindsPath = Path.Combine(cfgFolder, "binds.cfg");
                string bindsTxtPath = Path.Combine(cfgFolder, "binds.txt");

                string bind = "bind F7 \"algo_load\"";
                string newCFG = string.Empty;
                string newBinds = string.Empty;

                // Converts txt to cfg
                File.Move(cfgPath, cfgTxtPath);
                File.Move(bindsPath, bindsTxtPath);

                // Reads content
                string cfgContent = File.ReadAllText(cfgTxtPath);
                string bindsContent = File.ReadAllText(bindsTxtPath);

                // Enable algomod.dll
                if (!cfgContent.Contains("plugin load algomod"))
                {
                    newCFG = cfgContent + "\n" + "plugin load algomod";
                    cfgContent = newCFG;
                }

                // Disables incompatible plugins 
                string[] pluginsList = Vars.WebsiteInfo.Split("|||")[1].Split('|');

                foreach (string plugin in pluginsList)
                {
                    if (cfgContent.Contains(plugin))
                    {
                        string[] lines = cfgContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        string[] filteredLines = lines.Where(line => !line.Contains($"plugin load {plugin}")).ToArray();

                        cfgContent = string.Join(Environment.NewLine, filteredLines);
                        newCFG = cfgContent;
                    }
                }

                // Adds algo_load F7 bind
                if (!bindsContent.Contains(bind))
                {
                    if (bindsContent.EndsWith("\n"))
                        newBinds = bindsContent + "\n" + bind;
                }

                // Writes new file contents if changed
                if (!string.IsNullOrEmpty(newCFG))
                {
                    File.WriteAllText(cfgTxtPath, newCFG);
                }

                if (!string.IsNullOrEmpty(newBinds))
                {
                    File.WriteAllText(bindsTxtPath, newBinds);
                }

                // Converts txt to cfg
                File.Move(cfgTxtPath, cfgPath);
                File.Move(bindsTxtPath, bindsPath);
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error BakkesCFG: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Gathers all running processes of Rocket League and closes them
        /// </summary>
        public static void CloseRocketLeague()
        {
            try
            {
                // Gather all Rocket League processes
                Process[] RLprocesses = Process.GetProcessesByName("rocketleague");

                // End process for each (if there is any)
                foreach (Process RLprocess in RLprocesses)
                {
                    RLprocess.Kill();
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CloseRocketLeague: {ex.Message}", "Exit");
            }
        }
    }
}
