using System.IO;

namespace AlgoModSimpleWPF
{
    internal class TabFile
    {
        /// <summary>
        /// Handles the click event of Auto Find Path
        /// </summary>
        public static void AutoFindPath()
        {
            switch (Vars.FileTab)
            {
                case 0:
                    AutoFindSteam();
                    break;

                case 1:
                    AutoFindEpic();
                    break;

                case 2:
                    AutoFindBakkes();
                    break;
            }
        }


        /// <summary>
        /// Looks for steam version of rocket league in it's default location
        /// </summary>
        public static void AutoFindSteam()
        {
            string steamexe = @"Program Files (x86)\Steam\steamapps\common\rocketleague\Binaries\Win64\RocketLeague.exe";
            string steamFolder = @"Program Files (x86)\Steam\steamapps\common\rocketleague";
            string steamPathFound = string.Empty;

            try
            {
                // Looks for rocket league in all available drives
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    string steamPath = Path.Combine(drive.RootDirectory.FullName, steamexe);

                    if (File.Exists(steamPath))
                    {
                        InfoData.SteamRLPath = Path.Combine(drive.RootDirectory.FullName, steamFolder);
                        steamPathFound = steamPath;
                        break;
                    }
                }

                // Only shows popup when user is in file tab
                if (Vars.TabStage == 2)
                {
                    if (!string.IsNullOrEmpty(steamPathFound))
                    {
                        Vars.MainWindow.Path_TextBox.Text = InfoData.SteamRLPath;

                        MethodsPopup.Popup(0, "Success!", "Successfully found Steam Rocket League path!", "Okay");
                        MethodsData.WriteToInfo();
                    }
                    else
                    {
                        MethodsPopup.Popup(0, "Attention!", "Unable to find Steam Rocket League path!", "Okay");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error AutoFindSteam: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Looks for epic version of rocket league in it's default location
        /// </summary>
        public static void AutoFindEpic()
        {
            string epicexe = @"Program Files\Epic Games\rocketleague\Binaries\Win64\RocketLeague.exe";
            string epicFolder = @"Program Files\Epic Games\rocketleague";
            string epicPathFound = string.Empty;

            try
            {
                // Looks for rocket league in all available drives
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    string epicPath = Path.Combine(drive.RootDirectory.FullName, epicexe);

                    if (File.Exists(epicPath))
                    {
                        InfoData.EpicRLPath = Path.Combine(drive.RootDirectory.FullName, epicFolder);
                        epicPathFound = epicPath;
                        break;
                    }
                }

                // Only shows popup when user is in file tab
                if (Vars.TabStage == 2)
                {
                    if (!string.IsNullOrEmpty(epicPathFound))
                    {
                        Vars.MainWindow.Path_TextBox.Text = InfoData.EpicRLPath;
                        MethodsPopup.Popup(0, "Success!", "Successfully found Epic Rocket League path!", "Okay");
                        MethodsData.WriteToInfo();
                    }
                    else
                    {
                        MethodsPopup.Popup(0, "Attention!", "Unable to find Epic Rocket League path!", "Okay");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error AutoFindEpic: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Looks for bakkesmod in it's default location
        /// </summary>
        private static void AutoFindBakkes()
        {
            try
            {
                // Builds bakkesmod folder
                string bakkesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"bakkesmod\bakkesmod");

                // Ensures the folder exists
                if (Directory.Exists(bakkesFolder))
                {
                    // Captures folder in vars
                    InfoData.BakkesModPath = bakkesFolder;
                    Vars.MainWindow.Path_TextBox.Text = bakkesFolder;

                    // Write folder to info file
                    MethodsData.WriteToInfo();

                    // Show success
                    MethodsPopup.Popup(0, "Success!", "Successfully found BakkesMod path!", "Okay");
                }
                else
                {
                    MethodsPopup.Popup(0, "Attention!", "Unable to find BakkesMod path!", "Okay");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error AutoFindBakkes: {ex.Message}", "Exit");
            }
        }

        
        /// <summary>
        /// Handles click event from Set Path
        /// Sets path vars if the given directory exists and is valid
        /// </summary>
        public static void SetPath()
        {
            string input = Vars.MainWindow.Path_TextBox.Text;

            if (!string.IsNullOrEmpty(input))
            {
                // Creates a path to test to see if it's valid
                string testPath = Path.Combine(input, "TAGame", "CookedPCConsole", "Labs_Underpass_P.upk");

                // Set steam or epic path
                if (Vars.FileTab is 0 or 1)
                {
                    // Checks to see if the test path exists
                    if (File.Exists(testPath))
                    {
                        // Sets var based on if it's steam or epic
                        switch (Vars.FileTab)
                        {
                            // Steam
                            case 0:
                                InfoData.SteamRLPath = input;
                                MethodsPopup.Popup(0, "Success!", "Successfully set Steam Rocket League path!", "Okay");
                                break;

                            // Epic
                            case 1:
                                InfoData.EpicRLPath = input;
                                MethodsPopup.Popup(0, "Success!", "Successfully set Epic Rocket League path!", "Okay");
                                break;
                        }
                    }
                    else
                    {
                        MethodsPopup.Popup(0, "Attention!", "The path given doesn't exist!", "Okay");
                    }
                }

                // If in the bakkesmod tab, set bakkesmod path
                if (Vars.FileTab == 2)
                {
                    // Ensure path given exists
                    if (Directory.Exists(input))
                    {
                        // Capture input as var
                        InfoData.BakkesModPath = input;

                        // Write set path to info file
                        MethodsData.WriteToInfo();

                        // Show success
                        MethodsPopup.Popup(0, "Success!", "Successfully set BakkesMod path!", "Okay");
                    }
                    else
                    {
                        MethodsPopup.Popup(0, "Attention!", "The path given doesn't exist!", "Okay");
                    }
                }
            }
            else
            {
                MethodsPopup.Popup(0, "Attention!", "Please enter your path in the box.", "Okay");
            }
        }
    }
}
