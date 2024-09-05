using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using WpfAnimatedGif;

namespace AlgoModSimpleWPF
{
    internal class MethodsEntrance
    {
        /// <summary>
        /// Entrance method for application, initializes everything
        /// </summary>
        public static async Task Entrance()
        {
            // Create new mutex (or close if one exists already)
            CheckMutex();

            // Request admin if requested with Admin.txt
            RequestAdmin();

            // Start loading screen
            await LoadingScreen(true);
            TabMore.Log("\nNew AlgoMod window started.\n");

            // Creates version file for use in the launcher's auto-update feature
            Vtxt();

            // Don't access internet before this
            // Ensures the user has internet
            if (await NetworkCURL.IsInternetAvailable())
            {
                // Get info from server
                await NetworkServer.SendToServer("INFO");

                // Get website info
                await NetworkCURL.GetInfo();

                // If info.txt exists, read it
                if (MethodsData.ReadInfoFile())
                {
                    // Ensure paths in info.txt are still valid
                    if (CheckExistingDirectories())
                    {
                        // Ensures all required files are present
                        await VerifyFiles();

                        // Info file exists and is read
                        if (await MethodsVerification.IsVerified(InfoData.Email))
                        {
                            // Go to injector
                            await Stage.UpdateStage(tabStage: 1);
                            
                            // Update mods or plugin if needed
                            await MethodsUpdate.CheckForUpdates();
                        }
                        else
                        {
                            // Given email isn't verified, go back to setup stage
                            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "info.txt"));
                            MethodsData.ClearInfoVars();
                            Tabs.UpdateTabs(tabStage: 0);

                            Vars.MainWindow.AllTabsGrid.Visibility = Visibility.Collapsed;
                            MethodsPopup.Popup(0, "Attention!", "The Email we have for you is nolonger a Patron. Entering setup.", "Okay");
                        }

                    }
                    else
                    {
                        // User's files have changed, go through setup again
                        File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "info.txt"));
                        MethodsData.ClearInfoVars();
                        Tabs.UpdateTabs(tabStage: 0);

                        MethodsPopup.Popup(0, "Attention!", "Your paths have changed. Please verify your info again.", "Okay");
                        TabMore.Log("info.txt contains paths that nolonger exist. Running through setup.");
                    }
                }
                else
                {
                    // info.txt does not exist, meaning the user is not verified yet. Go to setup stage
                    Tabs.UpdateTabs(tabStage: 0);
                }
            }
            else
            {
                // Waits for user to connect to the internet, then tries entrance again
                await NoInternet();
            }

            // Stop loading screen
            await LoadingScreen(false);
        }



        /*
         * Helper methods for Entrance
         * Laid out in order of execution
        */

        /// <summary>
        /// Attempts to create a new mutex of algomod
        /// If the mutex already exists, close the application
        /// This ensures only one instance of the application and be open at a time
        /// </summary>
        private static void CheckMutex()
        {
            // Mutex makes sure only one instance of this window can be open at one time
            Vars.AlgoMutex = new Mutex(true, "AlgoModMutex", out bool createdNew);

            // If mutex thread is open, continue. if not, close
            if (!createdNew)
            {
                Vars.AlgoMutex.Dispose();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// If not in administrator mode and Admin.txt exists, open a new window with admin perms and close this one
        /// </summary>
        private static void RequestAdmin()
        {
            // If admin file exists (which means the admin box was checked), request admin
            string adminPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Admin.txt");

            if (File.Exists(adminPath))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(identity);

                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    try
                    {
                        ProcessStartInfo processInfo = new()
                        {
                            FileName = Process.GetCurrentProcess().MainModule?.FileName,
                            Verb = "runas",
                            UseShellExecute = true
                        };

                        Process.Start(processInfo);
                        Environment.Exit(0);
                    }
                    catch
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }


        /// <summary>
        /// User has no internet, check every second to see if it can connect to internet
        /// </summary>
        private static async Task NoInternet()
        {
            await LoadingScreen(false);
            Vars.MainWindow.NoInternet.Visibility = Visibility.Visible;

            // Waits until internet is available
            while (!await NetworkCURL.IsInternetAvailable())
            {
                await Task.Delay(1000);
            }

            Vars.MainWindow.NoInternet.Visibility = Visibility.Hidden;
            await Entrance();
        }


        /// <summary>
        /// Turn on or off loading screen
        /// </summary>
        /// <param name="show">Bool whether to hide or show</param>
        private static async Task LoadingScreen(bool show)
        {
            if (show)
            {
                // Show loading screen
                Vars.MainWindow.LoadingScreen.Visibility = Visibility.Visible;
            }
            else
            {
                // Waits for images to load before hiding loading screen
                if (Vars.TabStage != 0)
                {
                    await Task.Delay(500);
                }

                // Hide loading screen
                Vars.MainWindow.LoadingScreen.Visibility = Visibility.Hidden;

                // Changes source of the element so it doesn't continue running while hidden
                ImageBehavior.SetAnimatedSource(Vars.MainWindow.LoadingScreen, null);
            }
        }


        /// <summary>
        /// Write the version txt file the launcher uses to see if the application needs an update
        /// </summary>
        private static void Vtxt()
        {
            string vPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "v.txt");

            if (File.Exists(vPath))
            {
                // Rewrites file if the version is different
                if (File.ReadAllText(vPath) != Vars.v)
                {
                    File.WriteAllText(vPath, Vars.v);
                }
            }
            else
            {
                // Writes new version file
                File.WriteAllText(vPath, Vars.v);
            }
        }


        /// <summary>
        /// Ensures that all necessary files are present. Creates or downloads files needed
        /// </summary>
        private static async Task VerifyFiles()
        {
            string modsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
            string uninjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uninject.alg");

            // Mods folder
            if (!Directory.Exists(modsPath))
            {
                Directory.CreateDirectory(modsPath);
            }

            // Uninject.alg
            if (!File.Exists(uninjectPath))
            {
                await NetworkDownload.DownloadFile($"{Vars.WebsiteBase}algomod/download/Uninject.alg", uninjectPath);
            }
        }


        /// <summary>
        /// Checks to see if directories specified in info.txt exist. If they don't, delete info.txt and go through verification again
        /// </summary>
        /// <returns>Bool based on if info.txt directories exist</returns>
        private static bool CheckExistingDirectories()
        {
            string[] directories = { InfoData.SteamRLPath, InfoData.EpicRLPath, InfoData.BakkesModPath };

            foreach (string dir in directories)
            {
                if (!string.IsNullOrEmpty(dir) && dir != "???")
                {
                    if (!Directory.Exists(dir))
                    {
                        TabMore.Log($"{dir} doesn't exist");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
