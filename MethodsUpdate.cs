using System.IO;

namespace AlgoModSimpleWPF
{
    internal class MethodsUpdate
    {
        /// <summary>
        /// Keeps plugin and mod files up to date
        /// </summary>
        public static async Task CheckForUpdates()
        {
            await CheckForModUpdates();
            await AcquireVerifiedMods();
            await CheckForPluginUpdates();
        }



        /// <summary>
        /// Upgrades any out of date mods the user may have in their mods folder
        /// </summary>
        private static async Task CheckForModUpdates()
        {
            try
            {
                string[] modinfoList = Vars.WebsiteInfo.Split("|||")[0].Split("||");
                string[] modFiles = Directory.GetFiles(Vars.ModsPath);

                // Iterates each file found in mods folder
                foreach (string modFile in modFiles)
                {
                    // Gets info for mod file
                    string modFileName = Path.GetFileName(modFile);
                    string modFileExtension = Path.GetExtension(modFile);

                    // Ensure this file is an algo mod
                    if (modFileExtension == "alg")
                    {
                        // Find mod info for specified mod
                        string? modInfo = modinfoList.FirstOrDefault(mod => mod.Contains(modFileName));

                        if (modInfo != null)
                        {
                            // Get file size of mod from mod info
                            string fileSize = modInfo.Split('|')[6];

                            if (fileSize != new FileInfo(modFile).Length.ToString())
                            {
                                TabMore.Log($"CheckForModUpdates: {modFileName} is the wrong size and will be updated");

                                // Delete and download new mod file
                                File.Delete(modFile);
                                await NetworkDownload.DownloadMod(modFileName, Vars.ModsPath);
                            }
                        }
                        else
                        {
                            TabMore.Log($"CheckForModUpdates: fileSize is null for {modFileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CheckForModUpdates: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Ensures user has all the mods they're verified for
        /// </summary>
        private static async Task AcquireVerifiedMods()
        {
            try
            {
                if (!string.IsNullOrEmpty(Vars.UserLine))
                {
                    // Get list of user's verified mods
                    string[] verifiedModsList = MethodsVerification.CleanString(Vars.UserLine.Split('|')[7], true, true, true, false).Split("_");

                    // Iterate array of verified mods
                    foreach (string rawVerifiedMod in verifiedModsList)
                    {
                        // Cleans verifiedMod
                        string verifiedMod = rawVerifiedMod.Trim();

                        // Makes sure the "all" part of tier 3 and X lines is filtered out
                        if (verifiedMod != "all")
                        {
                            if (!string.IsNullOrEmpty(verifiedMod))
                            {
                                // Build mod path
                                string modPath = Path.Combine(Vars.ModsPath, verifiedMod + ".alg");

                                // Checks to see if user has the mod downloaded
                                if (!File.Exists(modPath))
                                {
                                    TabMore.Log($"AcquireVerifiedMods: {verifiedMod} does not exist and will be downloaded");

                                    // Download verified mod
                                    await NetworkDownload.DownloadMod(verifiedMod, modPath);
                                }
                            }
                        }
                    }

                    // Repopulates mod selection box to include newly added mods
                    TabInjector.PopulateModBox();
                }
                else
                {
                    TabMore.Log("Userline is empty");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error AcquireVerifiedMods: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Ensures AlgoMod's plugin is downloaded and up to date
        /// </summary>
        private static async Task CheckForPluginUpdates()
        {
            try
            {
                // Get the plugin file size specified on the website
                string pluginFileSize = Vars.WebsiteInfo.Split("|||")[2].Split('|')[2];

                // Ensure BakkesModPath is valid and found
                if (File.Exists(InfoData.BakkesModPath))
                {
                    // Finds AlgoMod.dll in plugins folder
                    string? algoModPluginPath = Directory.GetFiles(Path.Combine(InfoData.BakkesModPath, "plugins")).FirstOrDefault(i => i.Contains("AlgoMod"));

                    if (!string.IsNullOrEmpty(algoModPluginPath))
                    {
                        if (File.Exists(algoModPluginPath))
                        {
                            // Gets the file size of the clients plugin
                            string currentPluginFileSize = new FileInfo(algoModPluginPath).Length.ToString();

                            // Compare client and website file sizes
                            if (currentPluginFileSize != pluginFileSize)
                            {
                                TabMore.Log($"CheckForPluginUpdates: {currentPluginFileSize} != {pluginFileSize}, downloading new version of plugin.");

                                // File sizes don't match, download new plugin
                                await NetworkDownload.DownloadPlugin();
                            }
                        }
                    }
                    else
                    {
                        TabMore.Log("CheckForPluginUpdates: No plugin found, downloading AlgoMod dll.");

                        // Plugin not found, download it
                        await NetworkDownload.DownloadPlugin();
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CheckForPluginUpdates: {ex.Message}", "Exit");
            }
        }
    }
}
