using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace AlgoModSimpleWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (this != null)
            {
                Vars.MainWindow = this;
            }

            Loaded += LoadedEntrance;
            Closing += Exit;
        }


        /// <summary>
        /// Entrance code on load
        /// </summary>
        /// <remarks><see cref="MethodsEntrance"/> contains all the code that runs during entrance</remarks>
        private async void LoadedEntrance(object sender, RoutedEventArgs e)
        {
            await MethodsEntrance.Entrance();
        }


        /// <summary>
        /// Readys application for shutdown, writes vars to file
        /// </summary>
        private void Exit(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clears mutex
            Vars.AlgoMutex?.Dispose();

            // Dispose http client
            Vars.AlgoHttpClient.Dispose();

            // Write local log to file
            TabMore.HandleLog(3);

            // Write info vars to file on exit
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "info.txt")))
            {
                MethodsData.WriteToInfo();
            }
        }



        /*
         * UI Interaction events
         */

        /*
         * Stage Interactions
        */

        /// <summary>
        /// Handles clicking of the next/continue button during setup
        /// </summary>
        public async void Next(object sender, RoutedEventArgs e)
        {
            if (Vars.Tier != "2")
            {
                switch (Vars.SetupStage)
                {
                    // Couldn't Find Rocket League Stage
                    case 3:
                        // Checks if that path is valid, goes to stage 4 if it is
                        Vars.bAutoRLFail = true;
                        break;

                    // Couldn't Find Bakkes Stage
                    case 5:
                        // Checks if that path is valid, goes to stage 4 if it is
                        Vars.bAutoBakkesFail = true;
                        break;

                    // Couldn't Find ID Stage
                    case 7:
                        // Checks if given ID is valid
                        Vars.bAutoIDFail = true;
                        break;

                    // Car Warn Stage
                    case 10:
                        // Goes to Download/Verify stage
                        Vars.SetupStage = 11;
                        break;
                }

                await Stage.UpdateStage();
            }
            else
            {
                Vars.Tier2Stage += 1;
                Tabs.UpdateTabs(setupStage: 8);
                await Stage.HandleTier2();
            }
        }


        /// <!-- Stage 0 Patreon -->
        /// <remarks>Named PatreonClick1 because the original name was ambiguous and was causing a warning</remarks>
        public async void PatreonClick1(object sender, RoutedEventArgs e)
        {
            PatreonButton.IsEnabled = false;
            await Stage.UpdateStage();
        }


        /// <!-- Stage 2 Found two RL paths -->
        public async void EpicButtonClick(object sender, RoutedEventArgs e)
        {
            InfoData.PathType = "1";
            await Stage.UpdateStage(setupStage: 4);
        }

        public async void SteamButtonClick(object sender, RoutedEventArgs e)
        {
            InfoData.PathType = "0";
            await Stage.UpdateStage(setupStage: 4);
        }


        /// <!-- Stage 8 Choose Mod -->
        /// <summary>
        /// Sets mod elements to selected mod's info
        /// </summary>
        /// <remarks><see cref="MethodsSetup.SetModInfo"/> handles the parsing of website info to element sources</remarks>
        public void ModComboBoxChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModComboBox.SelectedIndex != 0)
                {
                    // Set mod info to selected mod
                    FinishButton.IsEnabled = true;
                    MethodsSetup.SetModInfo();
                }
                else
                {
                    // Set mod info to default values
                    ModName.Content = string.Empty;
                    ModDescription.Text = string.Empty;
                    ModImage.Source = new BitmapImage(new Uri("/Images/ChooseYourMods.png", UriKind.RelativeOrAbsolute));

                    FinishButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(0, "Fatal Error!", $"Error CheckCarWarn: {ex.Message}", "Exit");
            }
        }

        private void ModComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ModComboBox.IsDropDownOpen = !ModComboBox.IsDropDownOpen;
        }


        /// <summary>
        /// Mod selected, continue to verification
        /// If Tier 2, go to next stage
        /// </summary>
        private async void Finish(object sender, RoutedEventArgs e)
        {
            if (Vars.Tier == "2")
            {
                Vars.Tier2Stage += 1;
                await Stage.HandleTier2();
            }
            else
            {
                if (ModComboBox.SelectedIndex != 0)
                {
                    await Stage.UpdateStage(setupStage: 9);
                }
            }
        }


        /// <!-- Stage 10 Warn Body -->
        public async void Back(object sender, RoutedEventArgs e)
        {
            if (Vars.Tier == "2")
            {
                Vars.Tier2Stage -= 1;
                Tabs.UpdateTabs(setupStage: 8);
            }
            else
            {
                await Stage.UpdateStage(setupStage: 8);
            }
        }


        /// <!-- Stage 13 Choose Epic -->
        public async void EpicID_Click(object sender, RoutedEventArgs e)
        {
            await MethodsSetup.SelectEpicID();
        }

        private void EpicID_ComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EpicID_ComboBox.IsDropDownOpen = !EpicID_ComboBox.IsDropDownOpen;
        }

        private void EpicID_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EpicID_Button.IsEnabled = EpicID_ComboBox.SelectedIndex != 0;
        }

        

        /*
         * Question dots
        */

        private void ModQuestionClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(Vars.ModIDChosen))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = $"{Vars.WebsiteBase}algomod/modslist#{Vars.ModIDChosen}",
                    UseShellExecute = true
                });
            }
        }

        private void ModQuestionDot_MouseEnter(object sender, MouseEventArgs e)
        {
            ModQuestionDot.Opacity = 1;
        }

        private void ModQuestionDot_MouseLeave(object sender, MouseEventArgs e)
        {
            ModQuestionDot.Opacity = 0.50;
        }

        private void SetupQuestionClick(object sender, MouseButtonEventArgs e)
        {
            MethodsSetup.SetupQuestionDot();
        }

        private void SetupQuestionDot_MouseEnter(object sender, MouseEventArgs e)
        {
            SetupQuestionDot.Opacity = 1;
        }

        private void SetupQuestionDot_MouseLeave(object sender, MouseEventArgs e)
        {
            SetupQuestionDot.Opacity = 0.50;
        }

        

        /*
         * Tab events
        */

        /// <!-- Tab buttons -->
        private async void Tab1_Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await Stage.UpdateStage(tabStage: 1);
        }

        private void Tab2_Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(tabStage: 2);
        }

        private void Tab3_Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(tabStage: 3);
        }

        private void Tab4_Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(tabStage: 4);
        }

        private void Tab5_Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(tabStage: 5);
        }


        /*
         * Tab 1 (Injector)
        */

        /// <!-- Version selection -->
        private void Steam_Clicked(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ChangeVersion(0);
        }

        private void Epic_Clicked(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ChangeVersion(1);
        }


        /// <!-- Mod selection -->
        private void Mod_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabInjector.UpdateModSelection();
        }

        private void ModBorderMouseEnter(object sender, MouseEventArgs e)
        {
            ModBoarder.Source = new BitmapImage(new Uri("Images/ModBorderHover.png", UriKind.RelativeOrAbsolute));
        }

        private void ModBorderMouseLeave(object sender, MouseEventArgs e)
        {
            ModBoarder.Source = new BitmapImage(new Uri("Images/ModBorder.png", UriKind.RelativeOrAbsolute));
        }

        private void ModBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModBoarder.Source = new BitmapImage(new Uri("Images/ModBorderSelect.png", UriKind.RelativeOrAbsolute));
        }

        private void ModBorderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ToggleModComboBox();
        }

        private void Mod_ComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ToggleModComboBox();
        }

        private async void Mod_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            await TabInjector.ModComboBoxDelay();
        }


        /// <!-- Map selection -->
        private async void Map_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await TabInjector.UpdateMapSelection();
        }

        private void MapBorderMouseEnter(object sender, MouseEventArgs e)
        {
            MapBorder.Source = new BitmapImage(new Uri("Images/MapBoarderHover.png", UriKind.RelativeOrAbsolute));
        }

        private void MapBorderMouseLeave(object sender, MouseEventArgs e)
        {
            MapBorder.Source = new BitmapImage(new Uri("Images/MapBoarder.png", UriKind.RelativeOrAbsolute));
        }

        private void MapBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapBorder.Source = new BitmapImage(new Uri("Images/MapBoarderSelect.png", UriKind.RelativeOrAbsolute));
        }

        private void MapBorderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ToggleMapComboBox();
        }

        private void Map_ComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabInjector.ToggleMapComboBox();
        }

        private async void Map_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            await TabInjector.MapComboBoxDelay();
        }

        private async void Inject_Click(object sender, RoutedEventArgs e)
        {
            InjectButton.IsEnabled = false;
            await TabInjector.Inject();
            InjectButton.IsEnabled = true;
        }

        /// <!-- Version selection -->
        private void VersionEpic_MouseEnter(object sender, MouseEventArgs e)
        {
            if (InfoData.PathType == "0")
            {
                VersionEpic.Source = Vars.bitEpicLogoHover;
            }
        }

        private void VersionEpic_MouseLeave(object sender, MouseEventArgs e)
        {
            if (InfoData.PathType == "0")
            {
                VersionEpic.Source = Vars.bitEpicLogo;
            }
        }

        private void VersionSteam_MouseEnter(object sender, MouseEventArgs e)
        {
            if (InfoData.PathType == "1")
            {
                VersionSteam.Source = Vars.bitSteamLogoHover;
            }
        }

        private void VersionSteam_MouseLeave(object sender, MouseEventArgs e)
        {
            if (InfoData.PathType == "1")
            {
                VersionSteam.Source = Vars.bitSteamLogo;
            }
        }

        
        /*
         * Tab 2 (File)
        */

        private void Tab2_SteamPath_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(fileTab: 0);
        }

        private void Tab2_EpicPath_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(fileTab: 1);
        }

        private void Tab2_BakkesPath_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(fileTab: 2);
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string path = Path_TextBox.Text;

            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
            }
            else
            {
                MethodsPopup.Popup(0, "Attention!", "No valid path to open.", "Okay");
            }
        }

        private void AutoFind_Click(object sender, RoutedEventArgs e)
        {
            TabFile.AutoFindPath();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            TabFile.SetPath();
        }


        /*
         * Tab 3 (More)
        */

        /// <!-- Tab buttons -->
        private void More_General_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Tabs.UpdateTabs(moreStage: 0);
        }

        private async void More_AddMod_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await NetworkCURL.GetIDS();

            Tabs.UpdateTabs(moreStage: 1);
        }

        private async void More_AddID_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await NetworkCURL.GetIDS();

            Tabs.UpdateTabs(moreStage: 2);
        }

        private async void More_MyInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await NetworkCURL.GetIDS();

            Tabs.UpdateTabs(moreStage: 3);
        }


        /// <!-- General -->
        private void Log_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TabMore.HandleLog(0);
        }

        private void Log_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TabMore.HandleLog(1);
        }

        private void Admin_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TabMore.HandleAdmin(1);
        }

        private void LogOpen_Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"));
        }

        private void Admin_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TabMore.HandleAdmin(2);
        }

        private void Uninject_Button_Click(object sender, RoutedEventArgs e)
        {
            TabMore.Uninject(true);
        }
        private void Uninstall_Button_Click(object sender, RoutedEventArgs e)
        {
            TabMore.Uninstall();
        }


        /// <!-- Add mod -->
        private async void GetMod_Clicked(object sender, RoutedEventArgs e)
        {
            await TabMore.AddMod();
        }

        private void AddMod_ComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddMod_ComboBox.IsDropDownOpen = !AddMod_ComboBox.IsDropDownOpen;
        }


        /// <!-- Add ID -->
        private async void AddID_Button_Click(object sender, RoutedEventArgs e)
        {
            await TabMore.HandleClickAddID();
        }

        private void AddID_EpicComboBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddID_EpicComboBox.IsDropDownOpen = !AddID_EpicComboBox.IsDropDownOpen;
        }

        
        /*
         * Tab 4 (Links)
        */

        private void Patreon_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://www.patreon.com/Algo_RL",
                UseShellExecute = true
            });
        }

        private void Discord_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://discord.gg/HCMkFSA5jU",
                UseShellExecute = true
            });
        }

        private void Website_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://algorl.com",
                UseShellExecute = true
            });
        }



        /*
         * Window interactions
        */
        public void ExitApp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Minimize(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Popup_Clicked(object sender, RoutedEventArgs e)
        {
            MethodsPopup.PopupClick();
        }

        private void X_MouseEnter(object sender, MouseEventArgs e)
        {
            XImage.Source = new BitmapImage(new Uri("Images/AlgoModXHover.png", UriKind.RelativeOrAbsolute));
        }

        private void X_MouseLeave(object sender, MouseEventArgs e)
        {
            XImage.Source = new BitmapImage(new Uri("Images/AlgoModX.png", UriKind.RelativeOrAbsolute));
        }

        private void Dash_MouseEnter(object sender, MouseEventArgs e)
        {
            DashImage.Source = new BitmapImage(new Uri("Images/AlgoModDashHover.png", UriKind.RelativeOrAbsolute));
        }

        private void Dash_MouseLeave(object sender, MouseEventArgs e)
        {
            DashImage.Source = new BitmapImage(new Uri("Images/AlgoModDash.png", UriKind.RelativeOrAbsolute));
        }
    }
}
