using System.IO;
using System.Net.Http;

namespace AlgoModSimpleWPF
{
    internal class NetworkDownload
    {
        /// <summary>
        /// Downloads newest version of plugin
        /// </summary>
        public static async Task DownloadPlugin()
        {
            try
            {
                string pluginLink = Vars.WebsiteInfo.Split("|||")[2].Split('|')[1];
                string pluginPath = Path.Combine(InfoData.BakkesModPath, @"plugins\AlgoMod.dll");
                string[] pluginsList = Directory.GetFiles(Path.Combine(InfoData.BakkesModPath, @"plugins"));

                // Delete AlgoMod plugin
                foreach (string pluginFile in pluginsList)
                {
                    string pluginName = Path.GetFileName(pluginFile);

                    if (pluginName == "AlgoMod.dll")
                    {
                        File.Delete(pluginFile);
                        break;
                    }
                }

                // Download plugin
                await DownloadFile(pluginLink, pluginPath);
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DownloadPlugin: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Downloads all mods the user is verified for
        /// </summary>
        /// <remarks>Users <see cref="DownloadMod(string, string)"/> to download individual mods, and <see cref="DownloadAllMods()"/> to download all</remarks>
        public static async Task DownloadVerifiedMods()
        {
            try
            {
                // Downloads for tier 1 and 2
                if (Vars.Tier is "1" or "2")
                {
                    // Finds the user's line
                    List<string> IDsList = Vars.RawIds.Split(',').ToList();
                    string? userLine = IDsList.FirstOrDefault(line => line.Contains(InfoData.Email));

                    // Ensures line was found
                    if (userLine != null)
                    {
                        string[] lineParts = userLine.Split('|');

                        // Ensures the line is valid
                        if (lineParts.Length > 7)
                        {
                            // Gets array of users mods
                            string[] modArray = lineParts[7].Split('_');

                            foreach (string mod in modArray)
                            {
                                // Makes sure it's not "all" (shouldn't happen, just in case)
                                if (mod == "all" || string.IsNullOrEmpty(mod))
                                {
                                    continue;
                                }

                                // Builds path for this mod
                                string modPath = Path.Combine(Vars.ModsPath, $"{mod}.alg");

                                // Downloads mod
                                TabMore.Log($"DownloadVerifiedMods: Downloading '{mod}' to path '{modPath}'");
                                await DownloadMod(mod, modPath);

                                // Checks to see if download was successful
                                if (File.Exists(modPath))
                                {
                                    TabMore.Log($"DownloadVerifiedMods: Successfully downloaded '{mod}'");
                                }
                                else
                                {
                                    TabMore.Log($"DownloadVerifiedMods: Failed to download '{mod}'");
                                }
                            }
                        }
                    }
                }

                // Downloads all mods if has unlimited access
                if (Vars.Tier is "3" or "X")
                {
                    await DownloadAllMods();
                }

                TabInjector.PopulateModBox();
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DownloadVerifiedMods: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Download individual mod by it's ID/name
        /// </summary>
        /// <param name="modToDownload">Mod ID/name (first part of mod info)</param>
        public static async Task DownloadMod(string modToDownload, string modPath)
        {
            try
            {
                // Find specified mod info from website data
                List<string> modsList = Vars.WebsiteInfo.Split("|||")[0].Split("||").ToList();
                string? modInfo = modsList.FirstOrDefault(mod => mod.Contains(modToDownload));

                // Ensure mod was found
                if (modInfo != null)
                {
                    string[] modParts = modInfo.Split('|');

                    // Ensure the mod parts is the right size
                    if (modParts.Length > 5)
                    {
                        string modID = modParts[0];
                        string modLink = modParts[5];

                        // Download mod
                        TabMore.Log($"DownloadMod: Attempting to download {modID} at {modPath} using {modLink}");
                        await DownloadFile(modLink, modPath);

                        // Check to see if the mod was downloaded successfully
                        if (File.Exists(modPath))
                        {
                            TabMore.Log($"DownloadMod: Successfully downloaded {modID}");
                        }
                        else
                        {
                            TabMore.Log($"DownloadMod: Failed to download {modID}!");
                        }
                    }
                    else
                    {
                        TabMore.Log($"DownloadMod: Not downloading {modToDownload} because it's info is invalid!");
                    }
                }
                else
                {
                    TabMore.Log($"DownloadMod: Not downloading {modToDownload} because it cannot be found in website info!");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DownloadMod: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Downloads and extracts all algomod mods
        /// </summary>
        public static async Task DownloadAllMods()
        {
            try
            {
                string allModsLink = Vars.WebsiteInfo.Split("|||")[2].Split('|')[0];
                string allModsZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"mods\AllMods.zip");

                // Delete all existing files in the mods folder
                string[] existingFiles = Directory.GetFiles(Vars.ModsPath);

                foreach (string existingFile in existingFiles)
                {
                    File.Delete(existingFile);
                }

                // Download all mods zip
                TabMore.Log($"DownloadAllMods: Attempting to download AllMods.zip at {allModsZipPath} using {allModsLink}");
                await DownloadFile(allModsLink, allModsZipPath);

                // Ensure download was successful
                if (File.Exists(allModsZipPath))
                {
                    TabMore.Log($"DownloadAllMods: Successfully downloaded AllMods.zip");

                    // Get extraction folder
                    string? extractFolder = Path.GetDirectoryName(allModsZipPath);

                    if (!string.IsNullOrEmpty(extractFolder))
                    {
                        // Extract zip
                        System.IO.Compression.ZipFile.ExtractToDirectory(allModsZipPath, extractFolder);

                        // Delete zip after extraction
                        File.Delete(allModsZipPath);

                        // Check to see if extraction was successful
                        if (Directory.GetFiles(extractFolder).Length > 1 && !File.Exists(allModsZipPath))
                        {
                            TabMore.Log("DownloadAllMods: Successfully extracted AllMods.zip");
                        }
                        else
                        {
                            TabMore.Log("DownloadAllMods: Failed to extract AllMods.zip!");
                        }
                    }
                }
                else
                {
                    TabMore.Log($"DownloadAllMods: Failed to download AllMods.zip!");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DownloadAllMods: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Simple file download helper method
        /// </summary>
        public static async Task DownloadFile(string link, string path)
        {
            try
            {
                HttpResponseMessage reponse = await Vars.AlgoHttpClient.GetAsync(link);
                using Stream content = await reponse.Content.ReadAsStreamAsync();
                using FileStream file = new(path, FileMode.Create, FileAccess.Write, FileShare.None);

                await content.CopyToAsync(file);
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DownloadFile: {ex.Message}", "Exit");
            }
        }
    }
}
