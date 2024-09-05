using System.Diagnostics;
using System.IO;
using System.Text;

namespace AlgoModSimpleWPF
{
    internal class TabMore
    {
        /*
         * General Tab Methods
        */

        /// <summary>
        /// Builds text showing user which mod (if any) they have injected
        /// </summary>
        /// <remarks>Uses <see cref="TabInjector.CurrentlyInjectedMod(int)"/> to ascertain which mod is injected</remarks>
        public static void SetInjectedModText()
        {
            string injectedModReturn = TabInjector.CurrentlyInjectedMod(1);
            Vars.MainWindow.InjectedMod_Text.Text = $"Injected mod: {(string.IsNullOrEmpty(injectedModReturn) ? "None" : injectedModReturn)}";
        }


        /// <summary>
        /// Adds a new line to log file
        /// </summary>
        /// <param name="input">New line to add to log</param>
        public static void Log(string input)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

                // Ensure log file exists
                if (File.Exists(filePath))
                {
                    // Add existing content to builder
                    StringBuilder logBuilder = new();
                    logBuilder.AppendLine(File.ReadAllText(filePath));

                    // If it's not the first line, add new line
                    if (logBuilder.Length > 0)
                    {
                        logBuilder.AppendLine();
                    }

                    // Add input into new line
                    logBuilder.Append(input);

                    // Write new log to file
                    File.WriteAllText(filePath, logBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Log: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Handles log functionality
        /// </summary>
        /// <param name="i">
        /// case 0: Create log file
        /// case 1: Delete log file
        /// case 2: Initializes log checkbox
        /// case 3: Writes local log to file
        /// </param>
        public static void HandleLog(int i)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                bool fileExists = File.Exists(filePath);

                switch (i)
                {
                    // Creates log file (enables logging)
                    case 0:
                        if (!fileExists)
                        {
                            File.WriteAllText(filePath, "AlgoModv2.0");
                        }
                        break;

                    // Deletes log file (disables logging)
                    case 1:
                        if (fileExists)
                        {
                            File.Delete(filePath);
                        }
                        break;

                    // Checks the log checkbox if logging is enabled
                    case 2:
                        if (Vars.MainWindow.Log_CheckBox.IsChecked == false)
                        {
                            if (fileExists)
                            {
                                Vars.MainWindow.Log_CheckBox.IsChecked = true;
                            }
                        }
                        break;

                    // Writes log to file
                    case 3:
                        if (fileExists)
                        {
                            File.WriteAllText(filePath, Vars.localLog);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleLog: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Handles admin functionality
        /// </summary>
        /// <param name="i">
        /// case 1: Write admin file
        /// case 2: Delete admin file
        /// case 3: Initialize admin checkbox
        /// </param>
        public static void HandleAdmin(int i)
        {
            try
            {
                string adminPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Admin.txt");

                switch (i)
                {
                    // Write admin file (enable administrator check)
                    case 1:
                        File.WriteAllText(adminPath, InfoData.Email);
                        break;

                    // Delete admin file (disable administrator check)
                    case 2:
                        if (File.Exists(adminPath))
                        {
                            File.Delete(adminPath);
                        }
                        break;

                    // Initialize admin checkbox
                    case 3:
                        if (File.Exists(adminPath))
                        {
                            Vars.MainWindow.Admin_CheckBox.IsChecked = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleAdmin: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Restores underpass to default
        /// </summary>
        /// <param name="showMessage">Bool specifying whether or not to show uninjection message when done</param>
        public static void Uninject(bool showMessage)
        {
            try
            {
                // Build path for uninject file
                string uninjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uninject.alg");

                // Ensure the file exists
                if (File.Exists(uninjectPath))
                {
                    // Get file size of uninject file (underpass)
                    long underpassSize = new FileInfo(uninjectPath).Length;
                    (bool steamUninjected, bool epicUninjected) uninjectionStatus = (false, false);

                    // Build paths for underpass
                    string steamPath = Path.Combine(InfoData.SteamRLPath, @"TAGame\CookedPCConsole\Labs_Underpass_P.upk");
                    string epicPath = Path.Combine(InfoData.EpicRLPath, @"TAGame\CookedPCConsole\Labs_Underpass_P.upk");
                    
                    // Checks to see if Steam exists
                    if (File.Exists(steamPath))
                    {
                        // Gets file size of steam underpass
                        long steamSize = new FileInfo(steamPath).Length;

                        // If file size of uninject and steam underpass is different, that means an injection has been made
                        if (underpassSize != steamSize)
                        {
                            // Restore steam underpass to default
                            File.Copy(uninjectPath, steamPath, true);
                            uninjectionStatus.steamUninjected = true;
                        }
                    }

                    // Checks to see if Epic exists
                    if (File.Exists(epicPath))
                    {
                        // Gets file size of epic underpass
                        long epicSize = new FileInfo(epicPath).Length;

                        // If file size of uninject and epic underpass is different, that means an injection has been made
                        if (underpassSize !=  epicSize)
                        {
                            // Restore epic underpass to default
                            File.Copy(uninjectPath, epicPath, true);
                            uninjectionStatus.epicUninjected = true;
                        }
                    }

                    // Show message based on what was uninjected if anything
                    if (showMessage)
                    {
                        switch (uninjectionStatus)
                        {
                            case (false, false):
                                MethodsPopup.Popup(0, "Attention!", "There is no need to uninject because no mod is injected.", "Okay");
                                break;

                            case (true, false):
                                MethodsPopup.Popup(0, "Uninjected!", "Steam Underpass has successfully been restored to default.", "Okay!");
                                break;

                            case (false, true):
                                MethodsPopup.Popup(0, "Uninjected!", "Epic Underpass has successfully been restored to default.", "Okay!");
                                break;

                            case (true, true):
                                MethodsPopup.Popup(0, "Uninjected!", "Underpass has successfully been restored to default for Steam and Epic.", "Okay!");
                                break;
                        }
                    }

                    // If changes were made, update injection text
                    if (uninjectionStatus.steamUninjected || uninjectionStatus.epicUninjected)
                    {
                        SetInjectedModText();
                    }
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "Cannot find original Underpass file!", "Okay");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Uninject: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Uninstalls algomod
        /// </summary>
        /// <remarks>Creates a file telling the launcher to uninstall, then launch</remarks>
        public static void Uninstall()
        {
            try
            {
                string uninstallPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uninstall.alg");
                string launcherPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName ?? string.Empty, "AlgoMod Launcher.exe");

                if (File.Exists(launcherPath))
                {
                    // Uninject mods before uninstalling injector
                    Uninject(false);

                    // Create Uninstall.alg which will tell the launcher to uninstall algomod
                    File.Create(uninstallPath);

                    // Launch launcher application
                    Process.Start(launcherPath);

                    // Close current application
                    Environment.Exit(0);
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "Uninstall: Launcher not found!", "Okay");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Uninstall: {ex.Message}!", "Exit");
            }
        }


        /// <summary>
        /// Returns rocket league path based on path type
        /// </summary>
        /// <returns>RL path</returns>
        public static string GetCurrentRLPath() => InfoData.PathType switch
        {
            "0" => InfoData.SteamRLPath,
            "1" => InfoData.EpicRLPath,
            _ => string.Empty
        };

        

        /*
         * Add Mod Methods
        */

        /// <summary>
        /// Add all mods available for the user to add to combobox
        /// </summary>
        public static void PopulateAddModBox()
        {
            try
            {
                Vars.MainWindow.AddMod_ComboBox.Items.Clear();

                // Ensure info has been found
                if (!string.IsNullOrEmpty(Vars.UserLine) && !string.IsNullOrEmpty(Vars.WebsiteInfo))
                {
                    // Process info
                    string[] modList = Vars.WebsiteInfo.Split("|||")[0].Split("||");
                    List<string> verifiedMods = Vars.UserLine.Split('|')[7].Split('_').ToList();

                    // Iterate all mods
                    foreach (string mod in modList)
                    {
                        string[] modInfo = mod.Split('|');

                        // Ensures user doesn't already have this mod
                        if (!verifiedMods.Contains(modInfo[0]))
                        {
                            // Filters out premium mods if you're tier 1
                            if (!(Vars.Tier == "1" && modInfo[2] == "1"))
                            {
                                // Add display name to combobox
                                Vars.MainWindow.AddMod_ComboBox.Items.Add(modInfo[1]);
                            }
                        }
                    }

                    // Select first option
                    if (Vars.MainWindow.AddMod_ComboBox.Items.Count > 0)
                    {
                        Vars.MainWindow.AddMod_ComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Uninstall: {ex.Message}", "Exit");
            }
        }
        

        /// <summary>
        /// Sends request to server to add selected mod to user line
        /// </summary>
        /// <remarks>Builds a message ex. "REQUEST|email|modID" and sends it to server</remarks>
        public static async Task AddMod()
        {
            try
            {
                // Get display name of selected mod
                string? selectedDisplayName = Vars.MainWindow.AddMod_ComboBox.SelectedItem.ToString();

                // Ensures all information has been successfully gathered
                if (!string.IsNullOrEmpty(selectedDisplayName) && Vars.WebsiteInfo != string.Empty && Vars.UserLine != string.Empty)
                {
                    // Find the selected mod in website info
                    List<string> modList = Vars.WebsiteInfo.Split("|||")[0].Split("||").ToList();
                    string? selectedMod = modList.FirstOrDefault(mod => mod.Split('|')[1] == selectedDisplayName);

                    // Ensure mod was found
                    if (selectedMod != null)
                    {
                        // Build message and send to server
                        string message = $"REQUEST|{Vars.UserLine.Split('|')[3]}|{selectedMod.Split('|')[0]}";
                        await NetworkServer.SendToServer(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Uninstall: {ex.Message}", "Exit");
            }
        }

        

        /*
         * Add ID Methods
        */

        /// <summary>
        /// Build server message for adding ID and send it to server
        /// </summary>
        public static async Task HandleClickAddID()
        {
            string inputID = string.Empty;
            string platform = string.Empty;

            // Gather info from xaml elements and define platform
            if (Vars.AddIDType == 1)
            {
                // ID type is steam, gather steam ID and set platform
                inputID = Vars.MainWindow.AddID_TextBox.Text.Replace("Steam: ", string.Empty).Replace("Epic: ", string.Empty).Trim();
                platform = "steam";

                // Ensure the size of the ID is valid
                if (inputID.Length != 17)
                {
                    MethodsPopup.Popup(0, "Attention", "Steam ID is an invalid size or format.", "Okay");
                    return;
                }
            }
            else if (Vars.AddIDType == 2)
            {
                // ID type is epic, gather epic ID and set platform
                inputID = Vars.MainWindow.AddID_EpicComboBox.Text.Trim();
                platform = "epic";

                // Ensure the ID was found
                if (inputID == string.Empty)
                {
                    MethodsPopup.Popup(0, "Attention", "Please select an Epic ID.", "Okay");
                    return;
                }
            }

            // Ensure all required info has been gathered
            if (!string.IsNullOrEmpty(inputID) && !string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(InfoData.Email))
            {
                // Build ADDID message and send it to server
                string message = $"ADDID|{InfoData.Email}|{platform}|{inputID}";
                await NetworkServer.SendToServer(message);
            }
        }

        
        /// <summary>
        /// Handles opening off the Add ID tab. Initializes everything needed to add an ID
        /// </summary>
        /// <remarks>Documentation for helpers <see cref="HandleAddEpicID"/> and <seealso cref="HandleAddSteamID"/> is available above their respective methods</remarks>
        public static void HandleAddID()
        {
            try
            {
                // User doesn't have an Epic ID verified, add Epic ID
                if (Vars.UserLine.Split('|')[1] == "???")
                {
                    // Get Epic RL path
                    TabFile.AutoFindEpic();

                    HandleAddEpicID();
                }
                else
                {
                    // User doesn't have a Steam ID, add Steam ID
                    if (Vars.UserLine.Split('|')[2] == "???") /// steam
                    {
                        // Get Steam RL path
                        TabFile.AutoFindSteam();

                        HandleAddSteamID();
                    }
                    else
                    {
                        // No ID type available to add
                        Vars.AddIDType = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleAddID: {ex.Message}", "Exit");
            }
        }

        /// <summary>
        /// Helper method for <see cref="HandleAddID"/>
        /// Adds all epic IDs found in local files to the epic combobox
        /// </summary>
        /// <remarks><see cref="Vars.AddIDType"/> is set to 2 if any epic IDs were found</remarks>
        /// <remarks>Exceptions handled at the caller level</remarks>
        private static void HandleAddEpicID()
        {
            // Build save data folder for epic
            string DBEpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\Rocket League\TAGame\SaveDataEpic\DBE_Production");

            // Ensure the folder exists
            if (Directory.Exists(DBEpath))
            {
                // Gets all files from DBE directory
                string[] dbeFiles = Directory.GetFiles(DBEpath);
                List<string> foundIDs = new();

                // Populate found IDs list with all IDs found in DBE_Production folder
                foreach (string dbeFile in dbeFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(dbeFile).Split('_')[0].Trim();
                    string fileExtention = Path.GetExtension(dbeFile);

                    // Ensure the file is a save file, the ID is the correct size and the list doesn't already contain the ID
                    if (fileExtention == "save" && fileName.Length == 32 && !foundIDs.Contains(fileName))
                    {
                        // Adds ID to list
                        foundIDs.Add(fileName);
                    }
                }

                // Ensure ID(s) were/was found
                if (foundIDs.Count > 0)
                {
                    // Populate combobox with IDs
                    foreach (string id in foundIDs)
                    {
                        Vars.MainWindow.AddID_EpicComboBox.Items.Add(id);
                    }

                    // Select first item
                    Vars.MainWindow.AddID_EpicComboBox.SelectedIndex = 0;

                    // Specify epic type
                    Vars.AddIDType = 2;
                    return;
                }
            }

            // If no epic IDs were found, make type none
            Vars.AddIDType = 0;
        }

        /// <summary>
        /// Helper method for <see cref="HandleAddID"/>
        /// Finds steam ID from local files and displays it to a textbox
        /// </summary>
        /// <remarks><see cref="Vars.AddIDType"/> is set to 1 if steam ID is found</remarks>
        /// <remarks>Exceptions handled at the caller level</remarks>
        private static void HandleAddSteamID()
        {
            // Build save data folder for steam
            string DBEpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\Rocket League\TAGame\SaveData\DBE_Production");
            
            // Ensure folder exists
            if (Directory.Exists(DBEpath))
            {
                // Gets all files from DBE directory
                string[] dbeFiles = Directory.GetFiles(DBEpath);
                string? idFound = null;

                // Find steam ID from files
                foreach (string dbeFile in dbeFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(dbeFile).Split('_')[0].Trim();
                    string fileExtention = Path.GetExtension(dbeFile);

                    // Ensure it's a save file and the name is the correct size for a steam ID
                    if (fileExtention == "save" && fileName.Length == 17)
                    {
                        idFound = fileName;
                    }
                }

                // Ensure ID was found
                if (idFound != null)
                {
                    // Set the ID to xaml textbox
                    Vars.MainWindow.AddID_TextBox.Text = "Steam: " + idFound;

                    // Set ID type to steam
                    Vars.AddIDType = 1;
                    return;
                }
            }

            // If steam ID wasn't found, set type to none
            Vars.AddIDType = 0;
        }



        /*
         * My Info Code
        */

        /// <summary>
        /// Get user's info and display it as a message
        /// </summary>
        public static void HandleMyInfoClick()
        {
            try
            {
                // Ensure userline has been found
                if (!string.IsNullOrEmpty(Vars.UserLine))
                {
                    // Get IDs from user line and create message for each
                    string[] lineParts = Vars.UserLine.Split('|');
                    string epicPart = lineParts[1] == "???" ? " You are not verified for Epic." : $" Your Epic ID is {lineParts[1]}";
                    string steamPart = lineParts[2] == "???" ? " You are not verified for Steam." : $" Your Steam ID is {lineParts[2]}";

                    // Build infotext message
                    Vars.MainWindow.MyInfo_InfoText.Text = $"Your email is {InfoData.Email}. You are a Tier {Vars.Tier} Patron.{epicPart}{steamPart} You have {Vars.ModCredits} Mod Credits.";
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleAddID: {ex.Message}", "Exit");
            }
        }
    }
}
