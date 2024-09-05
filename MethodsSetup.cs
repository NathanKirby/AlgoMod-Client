using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AlgoModSimpleWPF
{
    internal class MethodsSetup
    {
        /*
         * Stage 0
         * Get Patreon Info
        */

        /// <summary>
        /// Attempts to gather your Patreon info and sets the obtained info to vars if successful
        /// </summary>
        public static async Task Patreon()
        {
            // Get patreonInfo with API
            string patreonInfo = await NetworkPatreon.Patreon();

            try
            {
                if (!string.IsNullOrEmpty(patreonInfo))
                {
                    // Parse info
                    JObject patreonInfojson = JObject.Parse(patreonInfo);
                    
                    // Gets email from patreonInfo
                    string email = (string?)patreonInfojson["data"]?["attributes"]?["email"] ?? "Unknown";

                    // Gets Tier from patreonInfo
                    string tier = patreonInfo switch
                    {
                        string info when info.Contains("9324388") => "1",
                        string info when info.Contains("9324395") => "2",
                        string info when info.Contains("9324409") => "3",
                        string info when info.Contains("9457163") => "X",
                        _ => "0"
                    };

                    // Gets lifetime_support_cents from patreonInfo
                    int lifetimeSupportCents = (int?)patreonInfojson["included"]?[0]?["attributes"]?["lifetime_support_cents"] ?? 0;

                    // Set info to vars
                    InfoData.Email = email;
                    Vars.Tier = tier;
                    Vars.Cents = lifetimeSupportCents.ToString();
                    Vars.bPatron = true;

                    // Navigate to Auto-find RL path
                    await Stage.UpdateStage(setupStage: 1);
                }
                else
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error Patreon: Verification unsuccessful!", "Exit");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Patreon: {ex.Message}", "Exit");
            }
        }



        /*
         * Stage 1
         * Auto-find RL path
        */

        /// <summary>
        /// Attempts to find rocket league's folder from default download locations
        /// </summary>
        public static async Task RLPath()
        {
            string steamexe = @"Program Files (x86)\Steam\steamapps\common\rocketleague\Binaries\Win64\RocketLeague.exe";
            string steamFolder = @"Program Files (x86)\Steam\steamapps\common\rocketleague";

            string epicexe = @"Program Files\Epic Games\rocketleague\Binaries\Win64\RocketLeague.exe";
            string epicFolder = @"Program Files\Epic Games\rocketleague";

            try
            {
                // Checks for paths defined above in all available drives
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    string steamPath = Path.Combine(drive.RootDirectory.FullName, steamexe);
                    string epicPath = Path.Combine(drive.RootDirectory.FullName, epicexe);

                    if (File.Exists(steamPath))
                    {
                        string driveSteamFolder = Path.Combine(drive.RootDirectory.FullName, steamFolder);

                        if (Directory.Exists(driveSteamFolder))
                        {
                            InfoData.SteamRLPath = driveSteamFolder;
                        }
                    }

                    if (File.Exists(epicPath))
                    {
                        string driveEpicFolder = Path.Combine(drive.RootDirectory.FullName, epicFolder);

                        if (Directory.Exists(driveEpicFolder))
                        {
                            InfoData.EpicRLPath = driveEpicFolder;
                        }
                    }
                }

                TabMore.Log($"Steam location found: {InfoData.SteamRLPath}, Epic location found: {InfoData.EpicRLPath}");

                // Check what/which paths (if any) were found
                if (InfoData.SteamRLPath == "???" && InfoData.EpicRLPath == "???")
                {
                    // Nothing found, Navigate to Give RL Path
                    Tabs.UpdateTabs(setupStage: 3);
                }
                else if (InfoData.SteamRLPath != "???" && InfoData.EpicRLPath != "???")
                {
                    // Both Epic and Steam Rocket League paths found, prompt user to choose which to use
                    await Stage.UpdateStage(setupStage: 2);
                }
                else if (InfoData.SteamRLPath != "???")
                {
                    // Steam path found, without Epic
                    InfoData.PathType = "0";

                    // Navigate to Auto-find BakkesMod Path
                    await Stage.UpdateStage(setupStage: 4);
                }
                else if (InfoData.EpicRLPath != "???")
                {
                    // Epic path found, without Steam
                    InfoData.PathType = "1";

                    // Navigate to Auto-find BakkesMod Path
                    await Stage.UpdateStage(setupStage: 4);
                }
                else
                {
                    // This should never happen, but is just in case truly nothing is found
                    // Naviages to Give RL Path
                    Tabs.UpdateTabs(setupStage: 3);
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error RLPath: {ex.Message}", "Exit");
            }
        }



        /*
         * Stage 3
         * Give RL Path
        */

        /// <summary>
        /// Cleans inputed rocket league path so it ends with '/rocketleague'
        /// </summary>
        /// <remarks>Parts to remove are stored in <see cref="Vars.partsToRemoveRL"/></remarks>
        /// <returns>Returns cleaned RL directory that ends with '/rocketleague'</returns>
        public static string CleanedRLInput()
        {
            // Get RL input
            string rlInput = Vars.MainWindow.SetupTextBox.Text;

            // Remove parts of directory that may have been included by user
            foreach (string part in Vars.partsToRemoveRL)
            {
                rlInput.Replace(part, string.Empty);
            }

            // Remove "\" at the end of input if it exists and return it
            return rlInput.EndsWith(@"\") ? rlInput[..^1] : rlInput;
        }


        /// <summary>
        /// Checks to see if cleaned given rocket league path is valid. Also deduces path type
        /// </summary>
        /// <param name="cleanedInput">Cleaned input from <see cref="Vars.MainWindow.SetupTextBox"/> cleaned by <see cref="CleanedRLInput"/></param>
        /// <remarks><see cref="InfoData.PathType"/> is set to "0" if steam's API file was found in the directory, or "1" is it wasn't</remarks>
        public static async Task IsPathValid(string cleanedInput)
        {
            string pathToCheck = $@"{cleanedInput}\Binaries\Win64\RocketLeague.exe";

            if (File.Exists(pathToCheck))
            {
                if (File.Exists($@"{cleanedInput}\Binaries\Win64\steam_api64.dll"))
                {
                    // Steam API found, meaning it's a steam path
                    InfoData.PathType = "0";
                    InfoData.SteamRLPath = cleanedInput;
                }
                else
                {
                    // Steam API not found, meaning it's an epic path
                    InfoData.PathType = "1";
                    InfoData.EpicRLPath = cleanedInput;
                }

                // Navigate to Auto-find BakkesMod Path
                await Stage.UpdateStage(setupStage: 4);
            }
            else
            {
                if (Vars.bAutoRLFail)
                {
                    MethodsPopup.Popup(0, "Attention!", $"Invaild path. Make sure your path ends with /rocketleague.", "Okay");
                }
            }
        }



        /*
         * Stage 4
         * Auto-find BakkesMod Path
        */

        /// <summary>
        /// Attempts to find BakkesMod's files in their default location
        /// </summary>
        public static async Task BakkesPath()
        {
            // Gets default BakkesMod path
            string bakkesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"bakkesmod\bakkesmod");

            if (Directory.Exists(bakkesFolder))
            {
                InfoData.BakkesModPath = bakkesFolder;

                // Navigate to Auto-find ID
                await Stage.UpdateStage(setupStage: 6);
            }
            else
            {
                // Navigate to Give BakkesMod Path
                Tabs.UpdateTabs(setupStage: 5);
            }
        }



        /*
         * Stage 5
         * Give BakkesMod Path 
        */

        /// <summary>
        /// Checks to see if given BakkesMod path is valid
        /// </summary>
        public static async Task CheckBakkesPath()
        {
            string givenPath = Vars.MainWindow.SetupTextBox.Text;

            // Checks to see if the path given is valid by attempting to locate plugininstaller.exe
            if (File.Exists(Path.Combine(givenPath, "plugininstaller.exe")))
            {
                InfoData.BakkesModPath = givenPath;

                // Navigate to Auto-find ID
                await Stage.UpdateStage(setupStage: 6);
            }
            else
            {
                if (Vars.bAutoBakkesFail)
                {
                    MethodsPopup.Popup(0, "Attention!", $"Invaild path. Make sure your path ends with /bakkesmod/bakkesmod.", "Okay");
                }
            }
        }



        /*
         * Stage 6
         * Auto-find ID
        */

        /// <summary>
        /// Attempts to get game IDs using SaveData files
        /// </summary>
        public static async Task GetGameID()
        {
            try
            {
                // Clears values
                Vars.EpicIDsList.Clear();
                InfoData.SteamID = string.Empty;
                InfoData.EpicID = string.Empty;

                // Sets paths
                string taGamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"My Games\Rocket League\TAGame");
                string steamDataPath = Path.Combine(taGamePath, @"SaveData\DBE_Production");
                string epicDataPath = Path.Combine(taGamePath, @"SaveDataEpic\DBE_Production");

                // Get steam ID
                if (Directory.Exists(steamDataPath))
                {
                    // Gathers all files from steamDataPath
                    string[] steamFiles = Directory.GetFiles(steamDataPath);

                    // Gets the first file found
                    string firstfile = Path.GetFileNameWithoutExtension(steamFiles[0]);

                    // Gets just the ID from the file name
                    InfoData.SteamID = firstfile.Split("_")[0].Trim();
                }

                // Get epic ID
                if (Directory.Exists(epicDataPath))
                {
                    // Gathers all files from epicDataEpic
                    string[] epicFiles = Directory.GetFiles(epicDataPath);

                    foreach (string file in epicFiles)
                    {
                        // Ensure it's a save file
                        if (Path.GetExtension(file) == "save")
                        {
                            // Get the epic ID from the name
                            string idFound = Path.GetFileNameWithoutExtension(file).Split("_")[0].Trim();

                            // Adds idFound to list if it's not null or already present
                            if (!Vars.EpicIDsList.Contains(idFound) && !string.IsNullOrEmpty(idFound))
                            {
                                Vars.EpicIDsList.Add(idFound);
                            }
                        }
                    }

                    // If only one ID was found, set variable
                    if (Vars.EpicIDsList.Count == 1)
                    {
                        InfoData.EpicID = Vars.EpicIDsList[0];
                        TabMore.Log($"GetGameID: {InfoData.EpicID}");
                    }
                }

                // Handles IDs found if any
                if (Vars.EpicIDsList.Count > 1)
                {
                    // Multiple epic IDs found, navigate to Choose Epic ID
                    await Stage.UpdateStage(setupStage: 13);
                }
                else
                {
                    if (string.IsNullOrEmpty(InfoData.SteamID) && string.IsNullOrEmpty(InfoData.EpicID))
                    {
                        // No IDs were found, navigate to Give ID
                        Tabs.UpdateTabs(setupStage: 7);
                    }
                    else
                    {
                        // An id was found, navigate to Choose Mod
                        await Stage.UpdateStage(setupStage: 8);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error GetGameID: {ex.Message}", "Exit");
            }
        }



        /*
         * Stage 7
         * Give ID
        */

        /// <summary>
        /// Checks to see if ID given by user is in a valid format
        /// </summary>
        public static async Task CheckGivenID()
        {
            string ID = Vars.MainWindow.SetupTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(ID))
            {
                // 17 is the length of a steam ID, and 32 is the length of an epic ID
                if (ID.Length == 17 || ID.Length == 32)
                {
                    // ID given is valid, navigate to Choose Mod
                    await Stage.UpdateStage(setupStage: 8);
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "ID given is invalid.", "Okay");
                }
            }
            else
            {
                if (Vars.bAutoIDFail)
                {
                    MethodsPopup.Popup(0, "Attention!", "Please enter an ID.", "Okay");
                }
            }
        }



        /*
         * Stage 8
         * Choose Mod
        */

        /// <summary>
        /// Sets selected mod's info to preview elements
        /// </summary>
        public static void SetModInfo()
        {
            try
            {
                object selectionVAR = Vars.MainWindow.ModComboBox.SelectedItem;

                if (selectionVAR != null)
                {
                    Vars.ModComboBoxSelection = selectionVAR?.ToString() ?? "null";
                    string[] modList = Vars.WebsiteInfo.Split("|||")[0].Split("||");

                    foreach (string mod in modList)
                    {
                        if (mod.Contains(Vars.ModComboBoxSelection))
                        {
                            string[] modInfo = mod.Split('|');

                            Vars.chooseModIDSelection = modInfo[0];
                            Vars.MainWindow.ModName.Content = modInfo[1];
                            Vars.MainWindow.ModDescription.Text = modInfo[3];

                            BitmapImage modImageBitmap = new(new Uri($"{Vars.WebsiteBase}images/{modInfo[0]}.png"));
                            Vars.MainWindow.ModImage.Source = modImageBitmap;

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error SetModInfo: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Add mod selection options to combobox for user to select
        /// </summary>
        public static void PopulateChooseModComboBox()
        {
            try
            {
                // Clears Mod Combobox
                Vars.MainWindow.ModComboBox.Items.Clear();

                // Create a select a default item to prompt the user to select a mod
                Vars.MainWindow.ModComboBox.Items.Add(new ComboBoxItem() { Content = "Click here to select mod", Foreground = Brushes.Gray });
                Vars.MainWindow.ModComboBox.SelectedIndex = 0;

                // Gets list of mods with it's info
                string[] modList = Vars.WebsiteInfo.Split("|||")[0].Split("||");

                // Ensures you're in the correct window
                if (Vars.SetupStage == 8)
                {
                    // Iterates through each mode found on the website
                    foreach (string mod in modList)
                    {
                        // Ensures the part isn't an empty space
                        if (!string.IsNullOrEmpty(mod.Trim()))
                        {
                            // Breaks up mod info into array
                            string[] modInfo = mod.Split('|');

                            // Ensures the mod is a valid size
                            if (modInfo.Length > 2)
                            {
                                if (Vars.Tier == "1")
                                {
                                    // If tier 1, only add basic mods
                                    if (modInfo[2] == "0")
                                    {
                                        Vars.MainWindow.ModComboBox.Items.Add(modInfo[1]);
                                    }
                                }
                                else if (Vars.Tier == "2")
                                {
                                    // Filters out mods you've already added
                                    if (!mod.Contains(Vars.T2Mod1) && !mod.Contains(Vars.T2Mod2))
                                    {
                                        if (Vars.Tier2Stage is 0 or 2)
                                        {
                                            // First two mod selections are basic only
                                            if (modInfo[2] == "0")
                                            {
                                                Vars.MainWindow.ModComboBox.Items.Add(modInfo[1]);
                                            }
                                        }
                                        else if (Vars.Tier2Stage == 4)
                                        {
                                            // Last mod selection can be either basic or premium
                                            Vars.MainWindow.ModComboBox.Items.Add(modInfo[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error PopulateChooseModComboBox: {ex.Message}", "Exit");
            }
        }



        /*
         * Stage 9
         * Check Car Warn
        */

        /// <summary>
        /// Checks to see if a selected car requires a specific body
        /// </summary>
        public static async Task CheckCarWarn()
        {
            try
            {
                // Gets list of mods with it's info
                string[] modList = Vars.WebsiteInfo.Split("|||")[0].Split("||");

                // Iterates through each mode found on the website
                foreach (string mod in modList)
                {
                    // Makes sure check only happens for selected mod
                    if (mod.Contains(Vars.ModComboBoxSelection))
                    {
                        // Breaks up info into an array
                        string[] modInfo = mod.Split('|');

                        // Ensure the info is a valid size
                        if (modInfo.Length > 4)
                        {
                            Vars.ModSelection = modInfo[0];

                            if (modInfo[4] != "Any")
                            {
                                // If the body type isn't "Any" that means it requires a specific car or cars to work. Warn user
                                Vars.MainWindow.BodyLabel.Text = $"This mod only works if you're using {modInfo[4]}. Make sure you have at least one of the cars needed.";

                                // Navigate to Car Warn page
                                Tabs.UpdateTabs(setupStage: 10);
                            }
                            else
                            { 
                                if (Vars.Tier == "2")
                                {
                                    Vars.Tier2Stage += Vars.Tier2Stage + 1;

                                    // Nagivate back to Choose Mod
                                    Tabs.UpdateTabs(setupStage: 8);

                                    await Stage.HandleTier2();
                                }
                                else
                                {
                                    // Navigate to verification stage
                                    await Stage.UpdateStage(setupStage: 11);
                                }
                            }
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CheckCarWarn: {ex.Message}", "Exit");
            }
        }



        /* 
         * Stage 12
         * Download
        */

        /// <summary>
        /// Downloads mods and plugins and configures bakkesmod
        /// </summary>
        public static async Task DownloadAndConfigure()
        {
            Vars.MainWindow.InstallStatus.Text = "Verifying you in our servers...";
            await Task.Delay(2000);

            if (await MethodsVerification.IsVerified(InfoData.Email))
            {
                await NetworkCURL.GetInfo();

                if (!string.IsNullOrEmpty(Vars.WebsiteInfo))
                {
                    // Close rocket league and download the mods you're verified
                    Vars.MainWindow.InstallStatus.Text = "Downloading your mod(s)...";
                    MethodsBakkesMod.CloseRocketLeague();
                    await NetworkDownload.DownloadVerifiedMods();

                    // Download algomod plugin
                    Vars.MainWindow.InstallStatus.Text = "Downloading AlgoMod plugin...";
                    await NetworkDownload.DownloadPlugin();
                    
                    // Setup bakkesmod cfg
                    Vars.MainWindow.InstallStatus.Text = "Setting up AlgoMod...";
                    MethodsBakkesMod.BakkesCFG();

                    // Finish and go to injector
                    Vars.MainWindow.InstallStatus.Text = "Finished!";
                    await Task.Delay(100);
                    await Stage.UpdateStage(setupStage: 999, tabStage: 1);
                }
            }
        }



        /*
         * Stage 13
         * Choose Epic ID
        */

        /// <summary>
        /// Populate combobox for epic ID selection
        /// </summary>
        public static void PopulateStageEpicIDsComboBox()
        {
            // Clear combobox
            Vars.MainWindow.EpicID_ComboBox.Items.Clear();

            // Create a select a default item to prompt the user to select an ID
            Vars.MainWindow.EpicID_ComboBox.Items.Add(new ComboBoxItem() { Content = "Click here to select ID", Foreground = Brushes.Gray });
            Vars.MainWindow.EpicID_ComboBox.SelectedIndex = 0;

            // Add each found epic ID to combobox
            foreach (string epicID in Vars.EpicIDsList)
            {
                Vars.MainWindow.EpicID_ComboBox.Items.Add(epicID);
            }
        }


        /// <summary>
        /// Handles choosing an epic ID
        /// </summary>
        public static async Task SelectEpicID()
        {
            // Get the ID you selected
            string? selectedContent = Vars.MainWindow.EpicID_ComboBox.SelectedValue as string;

            if (!string.IsNullOrEmpty(selectedContent))
            {
                // Ensures you have something other than the default value selected
                if (Vars.MainWindow.EpicID_ComboBox.SelectedIndex != 0)
                {
                    // Sets your EpicID variable
                    InfoData.EpicID = selectedContent;
                    TabMore.Log($"SelectEpicID: {InfoData.EpicID}");

                    // Navigates to Choose Mod
                    await Stage.UpdateStage(setupStage: 8);
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "Please select an ID at the bottom left.", "Okay");
                }
            }
            else
            {
                MethodsPopup.Popup(0, "Attention!", "Please select an ID at the bottom left.", "Okay");
            }
        }

        

        /*
         * Setup Help
        */

        /// <summary>
        /// Handles click of the setup question dot
        /// </summary>
        public static void SetupQuestionDot()
        {
            // Build base link
            string link = Vars.WebsiteBase;

            // Add parts to link based on current environment
            if (Vars.SetupStage is 8 or 10)
            {
                switch (Vars.SetupStage)
                {
                    case 8:
                        link += $"algomod/modslist#{Vars.chooseModIDSelection}";
                        break;
                    case 10:
                        link += "algomod/modslist";
                        break;
                };
            }
            else
            {
                string linkParam = Vars.SetupStage switch
                {
                    0 => "#get",
                    2 => "#2-installs",
                    3 => "#no-rl-folder",
                    5 => "#no-bakkes",
                    7 => "#no-id",
                    13 => "#multiple-epic",
                    _ => string.Empty
                };

                link += $"algomod/how-to{linkParam}";
            }

            // If current environment has no specifications, go to help page
            if (string.IsNullOrEmpty(link))
            {
                link += "algomod/help";
            }

            // Open link in default browser
            ProcessStartInfo psi = new() { FileName = link, UseShellExecute = true };
            Process.Start(psi);
        }
    }
}
