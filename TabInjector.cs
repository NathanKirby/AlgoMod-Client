using System.IO;
using System.Windows.Media.Imaging;

namespace AlgoModSimpleWPF
{
    internal class TabInjector
    {
        /*
         * Choosing Mod Methods
        */

        /// <summary>
        /// Populate and manage selection of Choose Mod combobox
        /// </summary>
        public static void PopulateModBox()
        {
            try
            {
                Vars.MainWindow.Mod_ComboBox.Items.Clear();

                // Get all files in mods folder
                string[] mods = Directory.GetFiles(Vars.ModsPath);

                // Iterate each file found in mods path
                foreach (string mod in mods)
                {
                    string fileExtension = Path.GetExtension(mod);
                    string fileName = Path.GetFileNameWithoutExtension(mod);

                    // Ensure it's an alg file
                    if (fileExtension == "alg")
                    {
                        // Find mod info from website info
                        List<string> modInfoList = Vars.WebsiteInfo.Split("|||")[0].Split("||").ToList();
                        string? modInfo = modInfoList.FirstOrDefault(mod => mod.Split('|')[0] == fileName);

                        // Ensure mod info was found
                        if (modInfo != null)
                        {
                            // Adds mod display name to combobox
                            Vars.MainWindow.Mod_ComboBox.Items.Add(modInfo.Split('|')[1]);
                        }
                    }
                }

                // Select injected mod after mod box is populated
                if (!SelectModByName(CurrentlyInjectedMod(1)))
                {
                    // If no selection has been made, select the first index
                    if (Vars.MainWindow.Mod_ComboBox.SelectedItem == null)
                    {
                        Vars.MainWindow.Mod_ComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error PopulateModBox: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Gets the file size of Labs_Underpass_P.upk and cross-references it with the file size of mods specified on the website
        /// </summary>
        /// <param name="type">type 0 = return mod ID, type 1 = return display name</param>
        /// <returns>Returns mod ID or mod name depending on type</returns>
        public static string CurrentlyInjectedMod(int type)
        {
            try
            {
                if (!string.IsNullOrEmpty(Vars.WebsiteInfo))
                {
                    // Build underpass path depending on path type
                    string underpassPath = Path.Combine(TabMore.GetCurrentRLPath(), @"TAGame\CookedPCConsole\Labs_Underpass_P.upk");

                    // Ensure underpass path exists
                    if (File.Exists(underpassPath))
                    {
                        // Get file size of underpass
                        string underpassFileSize = new FileInfo(underpassPath).Length.ToString();

                        // Find the injected mod based on it's file size
                        List<string> modList = Vars.WebsiteInfo.Split("|||")[0].Split("||").ToList();
                        string? modInfo = modList.FirstOrDefault(mod => !string.IsNullOrEmpty(mod) && mod.Split('|')[6] == underpassFileSize);

                        // Ensure mod was found
                        if (modInfo != null)
                        {
                            string[] modParts = modInfo.Split('|');

                            // Return mod ID
                            if (type == 0 && !string.IsNullOrEmpty(modParts.ElementAtOrDefault(0)))
                            {
                                return modParts[0];
                            }

                            // Return mod name
                            if (type == 1 && !string.IsNullOrEmpty(modParts.ElementAtOrDefault(1)))
                            {
                                return modParts[1];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CurrentlyInjectedMod: {ex.Message}", "Exit");
            }

            return string.Empty;
        }


        /// <summary>
        /// Selects a mod in choose mod injector combobox based off given displayName
        /// </summary>
        /// <returns>Returns bool which indicates if a selection has been made</returns>
        public static bool SelectModByName(string displayName)
        {
            try
            {
                // Find the index of the item based on displayName given by casting Mod_ComboBox.Items to an object and finding the item that matches the displayName
                int index = Vars.MainWindow.Mod_ComboBox.Items.Cast<object>().Select((item, i) => new { Item = item, Index = i }).FirstOrDefault(x => x.Item.ToString() == displayName)?.Index ?? -1;

                if (index == -1)
                {
                    Vars.MainWindow.Mod_ComboBox.SelectedIndex = index;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error SelectModByName: {ex.Message}", "Exit");
            }

            return false;
        }


        /// <summary>
        /// Updates vars to include new selection info and sets thumbnail for the mod
        /// </summary>
        /// <remarks>Thumbnail comes from website and URL for it is built in this method</remarks>
        public static void UpdateModSelection()
        {
            try
            {
                // Get selected mod
                object? selectedobject = Vars.MainWindow.Mod_ComboBox.SelectedItem;

                // Ensure it's not null
                if (selectedobject != null)
                {
                    // It's not possible for this to be null, added check to get rid of the green squiggly line of death
                    string selectedItem = selectedobject?.ToString() ?? string.Empty;

                    // Catch selected item in var
                    Vars.CurrentModComboBoxSelection = selectedItem;

                    // Find selected mod's info in website info
                    List<string> modInfoList = Vars.WebsiteInfo.Split("|||")[0].Split("||").ToList();
                    string? selectedModInfo = modInfoList.FirstOrDefault(mod => mod.Split('|')[0] == selectedItem);

                    // Ensure mod info was found
                    if (selectedModInfo != null)
                    {
                        // Get ID and catch in variable
                        string modID = selectedModInfo.Split('|')[0];
                        Vars.ModIDChosen = modID;

                        // Build thumbnail URL for mod and use it as image source
                        Vars.MainWindow.ModInjectorImage.Source = new BitmapImage(new Uri($"{Vars.WebsiteBase}images/{modID}.png"));
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error UpdateModSelection: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Toggles open and close the mod combobox when you click the thumbnail
        /// </summary>
        public static void ToggleModComboBox()
        {
            if (Vars.bModBoxOpen == false)
            {
                if (Vars.MainWindow.Mod_ComboBox.IsDropDownOpen == false)
                {
                    Vars.bModBoxOpen = true;
                    Vars.MainWindow.Mod_ComboBox.IsDropDownOpen = true;
                }
            }
            else
            {
                Vars.bModBoxOpen = false;
                Vars.MainWindow.Mod_ComboBox.IsDropDownOpen = false;
            }
        }


        /// <summary>
        /// Ensures clicking on the thumbnail doesn't double input
        /// </summary>
        public static async Task ModComboBoxDelay()
        {
            await Task.Delay(250);
            Vars.bModBoxOpen = false;
        }

        

        /*
         * Choose Map Methods
        */

        /// <summary>
        /// Populates combobox with maps algomod supports if they're found in the users files
        /// </summary>
        public static void PopulateMapBox()
        {
            try
            {
                Vars.MainWindow.Map_ComboBox.Items.Clear();
                Vars.OmittedMapAmount = 0;

                // Gets map names from variables and put them in a string array
                string[] mapList = Vars.MapNames.Split(',');

                // Sets var rlPath to empty for later use
                string rlPath = TabMore.GetCurrentRLPath();

                // Ensure the path was found
                if (!string.IsNullOrEmpty(rlPath))
                {
                    // Get each map from Vars.MapNames()
                    foreach (string map in mapList)
                    {
                        // Gets the file name for the map
                        string[] mapInfo = map.Split('|');

                        // Builds a path with map info gathered
                        string MapPath = $@"{rlPath}\TAGame\CookedPCConsole\{mapInfo[1]}.upk";

                        // Checks to see if map path built exists
                        if (File.Exists(MapPath))
                        {
                            // Adds an item in the combobox with the map name
                            Vars.MainWindow.Map_ComboBox.Items.Add(mapInfo[0]);
                        }
                        else
                        {
                            // Skipping this map because it does not exist right now. Adds 1 to ommitted maps
                            Vars.OmittedMapAmount += 1;
                        }
                    }

                    SelectMapByIndex(rlPath);
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error PopulateMapBox: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Selects map specified by algomap.txt
        /// </summary>
        /// <param name="rlPath">Specifies so mod doesn't have to compare path types a second time</param>
        public static void SelectMapByIndex(string rlPath)
        {
            try
            {
                string mapNumPath = Path.Combine(rlPath, @"Binaries\Win64\algomap.txt");

                if (File.Exists(mapNumPath))
                {
                    int mapNum = int.Parse(File.ReadAllText(mapNumPath));
                    string selectedMapName = Vars.MapNames.Split(",")[mapNum].Split('|')[0];

                    for (int i = 0; i < Vars.MapNames.Length; i++)
                    {
                        if (selectedMapName == Vars.MainWindow.Map_ComboBox.Items[i].ToString())
                        {
                            Vars.MainWindow.Map_ComboBox.SelectedItem = Vars.MainWindow.Map_ComboBox.Items[i];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error SelectMapByIndex: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Toggles open and close the map combobox when you click the thumbnail
        /// </summary>
        public static void ToggleMapComboBox()
        {
            if (Vars.bMapBoxOpen == false)
            {
                if (Vars.MainWindow.Map_ComboBox.IsDropDownOpen == false)
                {
                    Vars.bMapBoxOpen = true;
                    Vars.MainWindow.Map_ComboBox.IsDropDownOpen = true;
                }
            }
            else
            {
                Vars.bMapBoxOpen = false;
                Vars.MainWindow.Map_ComboBox.IsDropDownOpen = false;
            }
        }


        /// <summary>
        /// Ensures clicking on the thumbnail doesn't double input
        /// </summary>
        public static async Task MapComboBoxDelay()
        {
            await Task.Delay(250);
            Vars.bMapBoxOpen = false;
        }


        /// <summary>
        /// Writes index of map selection to a file the plugin can read
        /// </summary>
        public static void WriteMapNum()
        {
            string[] mapList = Vars.MapNames.Split(",");

            // Build path to algomap.txt
            string rlPath = TabMore.GetCurrentRLPath();

            if (!string.IsNullOrEmpty(rlPath))
            {
                string mapNumPath = Path.Combine(rlPath, @"Binaries\Win64\algomap.txt");

                try
                {
                    // Get selected item
                    object selectedObject = Vars.MainWindow.Map_ComboBox.SelectedItem;

                    // Get map list index for selected map and write it to file
                    for (int i = 0; mapList.Length > 0; i++)
                    {
                        if (mapList[i].Split('|')[0] == selectedObject.ToString())
                        {
                            File.WriteAllText(mapNumPath, i.ToString());
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error SelectMapByIndex: {ex.Message}", "Exit");
                }
            }
        }


        /// <summary>
        /// Writes selection index to file and sets map thumbnail
        /// </summary>
        public static async Task UpdateMapSelection()
        {
            try
            {
                // Writes selection num to file
                WriteMapNum();

                // Get selected item as string
                string? selectedItem = Vars.MainWindow.Map_ComboBox.SelectedItem.ToString();

                // Ensure selected item was found
                if (selectedItem != null)
                {
                    foreach (string map in Vars.MapNames.Split(','))
                    {
                        string[] mapInfo = map.Split('|');

                        if (mapInfo[0] == selectedItem)
                        {
                            // Removes last 2 characters, or the "_P" part of the name
                            string mapid = mapInfo[1][..^2];

                            string mapThumbLink = $"{Vars.WebsiteBase}images/MapThumb_{mapid}.png";
                            string failedThumbLink = $"{Vars.WebsiteBase}images/MapThumb_null.png";

                            // Set map thumbnail to selection image
                            BitmapImage mapImage;

                            // Check to see if the image URL exists
                            if (await NetworkCURL.IsUrlValid(mapThumbLink))
                            {
                                mapImage = new BitmapImage(new Uri(mapThumbLink));
                            }
                            else
                            {
                                mapImage = new BitmapImage(new Uri(failedThumbLink));
                            }

                            Vars.MainWindow.MapImage.Source = mapImage;

                            break;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error UpdateMapSelection: {ex.Message}", "Exit");
            }
        }



        /*
         * Choosing Version Methods
        */

        /// <summary>
        /// Switches the version of Rocket League you're injecting to
        /// </summary>
        /// <param name="version">0 for steam, 1 for epic</param>
        /// <remarks><see cref="CheckPathTypeValidity"/> is used to determin if you can change version or not</remarks>
        public static void ChangeVersion(int version)
        {
            // Save current type for later
            string previousType = InfoData.PathType;

            // Switch to type to check
            InfoData.PathType = version.ToString();

            // Gets valid response
            (bool valid, string message) = CheckPathTypeValidity();

            if (valid)
            {
                // Version change allowed, update injector
                if (version == 0)
                {
                    Vars.MainWindow.VersionSteam.Source = Vars.bitSteamLogoSelect;
                    Vars.MainWindow.VersionEpic.Source = Vars.bitEpicLogo;
                }

                if (version == 1)
                {
                    Vars.MainWindow.VersionEpic.Source = Vars.bitEpicLogoSelect;
                    Vars.MainWindow.VersionSteam.Source = Vars.bitSteamLogo;
                }

                PopulateMapBox();
                PopulateModBox();
                TabMore.Log($"ChangeVersion: {message}");
                MethodsData.WriteToInfo();
            }
            else
            {
                // Version change not allowed, switch type back and display message
                InfoData.PathType = previousType;
                MethodsPopup.Popup(0, "Attention!", message, "Okay");
            }
        }


        /// <summary>
        /// Ensures your have the proper verification and information to switch your version
        /// </summary>
        /// <returns>
        /// valid: if true, go through with version changed. If false, display message
        /// message: tells the user what went wrong, if anything
        /// </returns>
        public static (bool valid, string message) CheckPathTypeValidity()
        {
            bool valid = false;
            string message = string.Empty;

            try
            {
                // Ensure user is verified and info can be gathered
                if (!string.IsNullOrEmpty(Vars.UserLine))
                {
                    string[] userLineSplit = Vars.UserLine.Split('|');

                    // Check steam
                    if (InfoData.PathType == "0")
                    {
                        // Ensure the steam ID part is set and is valid size
                        if (userLineSplit[2].Length == 17)
                        {
                            // Ensure steam path is given and valid
                            if (Directory.Exists(InfoData.SteamRLPath))
                            {
                                // Build path to verify it's valid
                                string mapFile = Path.Combine(InfoData.SteamRLPath, @"Binaries\Win64\RocketLeague.exe");

                                // Ensure rocket league exists in this path
                                if (File.Exists(mapFile))
                                {
                                    valid = true;
                                    message = "Successfully switched to Steam.";
                                }
                                else
                                {
                                    message = "No Rocket League files were found for Steam! You can set your path manualled in the 'File' tab.";
                                }
                            }
                            else
                            {
                                message = "You have not given your Steam Rocket League file path! You can set that in the 'File' tab.";
                            }
                        }
                        else
                        {
                            message = "You are not verified for Steam! You can verify your Steam ID in the 'More' tab.";
                        }
                    }

                    // Check epic
                    if (InfoData.PathType == "1")
                    {
                        // Ensure the epic ID part is set and is valid size
                        if (userLineSplit[1].Length == 32)
                        {
                            // Ensure epic path is given and valid
                            if (Directory.Exists(InfoData.EpicRLPath))
                            {
                                // Build path to verify it's valid
                                string mapFile = Path.Combine(InfoData.EpicRLPath, @"Binaries\Win64\RocketLeague.exe");

                                // Ensure rocket league exists in this path
                                if (File.Exists(mapFile))
                                {
                                    valid = true;
                                    message = "Successfully switched to Epic.";
                                }
                                else
                                {
                                    message = "No Rocket League files were found for Epic! You can set your path manualled in the 'File' tab.";
                                }
                            }
                            else
                            {
                                message = "You have not given your Epic Rocket League file path! You can set that in the 'File' tab.";
                            }
                        }
                        else
                        {
                            message = "You are not verified for Epic! You can verify your Epic ID in the 'More' tab.";
                        }
                    }
                }
                else
                {
                    message = "User line not set!";
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CheckPathTypeValidity: {ex.Message}", "Exit");
            }

            return (valid, message);
        }

        

        /*
         * Injection Methods
        */
        
        /// <summary>
        /// Injects your selected mod into Rocket League
        /// </summary>
        /// <remarks>
        /// Before injection goes through, it does the following:
        /// Constructs a path for the mod using <see cref="Vars.ModIDChosen"/>
        /// Ensures you have the file for the mod you're trying to inject
        /// If you don't have the mod's file, download it using <see cref="NetworkDownload.DownloadMod(string, string)"/> and prompt user to try injection after download
        /// Ensures your email <see cref="InfoData.Email"/> is present in <see cref="Vars.RawIds"/> meaning you're verified
        /// Ensures the mod you're attempting to inject has it's ID present in your user line
        /// Ensures you have a valid rocket league path and underpass exists in the version you're using
        /// Ensures you have the uninject file (which is just the default underpass file) before you inject.
        /// If you don't have the uninject file, download it using <see cref="NetworkDownload.DownloadFile(string, string)"/> and repopulate combobox <seealso cref="PopulateModBox"/>
        /// </remarks>
        public static async Task Inject()
        {
            try
            {
                // Ensure a selection has been made
                if (!string.IsNullOrEmpty(Vars.ModIDChosen))
                {
                    // Constructs file location for 
                    string modPath = Path.Combine(Vars.ModsPath, $"{Vars.ModIDChosen}.alg");

                    // Ensure mod file exists
                    if (File.Exists(modPath))
                    {
                        // Find user's ID line
                        List<string> idsList = Vars.RawIds.Split(',').ToList();
                        string? idLine = idsList.FirstOrDefault(line => line.Split('|')[3] == InfoData.Email);

                        // Ensure line was found
                        if (idLine != null)
                        {
                            // Ensure you're not trying to inject a mod which is already injected
                            if (CurrentlyInjectedMod(0) != Vars.ModIDChosen)
                            {
                                // Find the mod you're trying to inject in your line to make sure you're verified for it
                                List<string> verifiedMods = idLine.Split('|')[7].Split('_').ToList();
                                string? mod = verifiedMods.FirstOrDefault(modID => modID == Vars.ModIDChosen);

                                // Ensure mod was found in your line
                                if (mod != null)
                                {
                                    // Get current RL path
                                    string rlPath = TabMore.GetCurrentRLPath();

                                    // Ensure path was found
                                    if (!string.IsNullOrEmpty(rlPath))
                                    {
                                        // Build underpass path
                                        string underpassPath = Path.Combine(rlPath, @"TAGame\CookedPCConsole\Labs_Underpass_P.upk");

                                        // Build path for the uninjected underpass
                                        string uninjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uninject.alg");

                                        // Ensure user has the uninjected underpass file before they inject
                                        if (!File.Exists(uninjectPath))
                                        {
                                            // Download it if it's not found
                                            await NetworkDownload.DownloadFile($"{Vars.WebsiteBase}algomod/download/Uninject.alg", uninjectPath);
                                        }

                                        // Commit injection 
                                        File.Copy(modPath, underpassPath);

                                        // Ensure the injection was successful
                                        if (CurrentlyInjectedMod(0) == Vars.ModIDChosen)
                                        {
                                            // If it was, show success message
                                            ShowInjectionSuccess();
                                        }
                                        else
                                        {
                                            MethodsPopup.Popup(0, "Attention!", "There was a problem injecting. Try enabling 'Administrator' in the 'More' tab.", "Okay");
                                        }
                                    }
                                    else
                                    {
                                        MethodsPopup.Popup(0, "Attention!", "Unable to find Rocket League folder!", "Okay");
                                    }
                                }
                                else
                                {
                                    // If user isn't verified for the mod they're attempting to inject, delete it
                                    File.Delete(modPath);
                                    PopulateModBox();
                                    MethodsPopup.Popup(0, "Attention!", "Cannot inject mods you aren't verified for! File deleted.", "Okay");
                                }
                            }
                            else
                            {
                                MethodsPopup.Popup(0, "Attention!", "This mod is already injected!", "Okay");
                            }
                        }
                        else
                        {
                            MethodsPopup.Popup(0, "Attention!", "You're not verified!", "Okay");
                        }
                    }
                    else
                    {
                        // If file doesn't exist for mod selection, download it
                        MethodsPopup.Popup(0, "Attention!", "No file for selected mod found! Your mod will be downloaded.", "Okay");
                        await NetworkDownload.DownloadMod(Vars.ModIDChosen, modPath);
                        PopulateModBox();
                    }
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "You must select a mod before injecting!", "Okay");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Inject: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Helper method for <see cref="Inject"/> shows success message along with the car(s) they need if any
        /// </summary>
        private static void ShowInjectionSuccess()
        {
            try
            {
                // Ensure website info has been found
                if (!string.IsNullOrEmpty(Vars.WebsiteInfo))
                { 
                    // Get the info for chosen mod
                    string? injectedInfo = Vars.WebsiteInfo.Split("|||")[0].Split("||").Where(info => info.Contains(Vars.ModIDChosen)).ToArray()[0];

                    // Ensure the info was found
                    if (!string.IsNullOrEmpty(injectedInfo))
                    {
                        string popupDescription = string.Empty;

                        // Gets required car info
                        string requiredCar = injectedInfo.Split('|')[4];

                        // Build popup description based 
                        if (requiredCar.Contains("Any"))
                        {
                            popupDescription = $"Press 'F7' in-game to play with your mod.";
                        }
                        else
                        {
                            popupDescription = $"Equip the {requiredCar} then press 'F7'.";
                        }

                        // Show success message
                        MethodsPopup.Popup(0, "Injected!", popupDescription, "Okay!");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error ShowInjectionSuccess: {ex.Message}", "Exit");
            }
        }
    }
}
