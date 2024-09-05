
namespace AlgoModSimpleWPF
{
    internal class Stage
    {
        /// <summary>
        /// Navigates to different setup stages
        /// </summary>
        /// <param name="setupStage">Updates var <see cref="Vars.SetupStage"/></param>
        /// <param name="tabStage">Updates var <see cref="Vars.TabStage"/></param>
        /// <remarks>
        /// If <see cref="Vars.TabStage"/> is 0, you're in the setup stage
        /// 
        /// <!-- Setup stages -->
        /// <see cref="Vars.SetupStage"/> navigates through the setup stage
        /// 
        /// case 0: Patreon stage
        /// If Patreon verification with API is successful, navigate to stage 1
        /// 
        /// case 1: Get Rocket League path
        /// If no paths found, navigate to tab 3
        /// If found 2 paths, naviaget to stage 2
        /// if one path found, navigate to stage 4
        /// 
        /// case 3: Unable to find Rocket League path
        /// If Rocket League path given is valid, navigate to stage 4
        /// 
        /// case 4: Get Bakkesmod path
        /// If Bakkesmod path found, navigate to stage 6
        /// If path not found, navigate to stage 5
        /// 
        /// case 5: Unable to find Bakkesmod path
        /// If Bakkesmod path given is valid, navigate to stage 6
        /// 
        /// case 6: If bVerified == false, Steam/Epic ID. If bVerified == true, go to Download
        /// 
        /// case 7: Unable to find Steam/Epic ID
        /// If ID given is valid, navigate to stage 8
        /// 
        /// case 8: Choose mod
        /// If mod requires specific body, navigate to stage 9
        /// When all mods chosen, navigate to stage 11
        /// 
        /// case 9: Check body
        /// If mod uses limited body, navigate to stage 10
        /// If mod uses any, navigate to stage 11
        /// 
        /// case 10: Body warn
        /// If clicked back, navigate back to stage 8
        /// If clicked next, navigate to stage 11
        /// 
        /// case 11: Verify
        /// Verify user then navigate to stage 12
        /// 
        /// case 12: Download and configure
        /// Downloads verified mods & plugin and configures bakkesmod
        /// Shows injector and completes setup stage
        /// 
        /// case 13: Choose Epic ID
        /// Multiple Epic IDs were found, prompt use to pick one
        /// </remarks>
        public static async Task UpdateStage(int? setupStage = null, int? tabStage = null)
        {
            // Set vars if defined
            Vars.SetupStage = setupStage ?? Vars.SetupStage;
            Vars.TabStage = tabStage ?? Vars.TabStage;

            try
            {
                // Makes sure you're in the setup stage
                if (Vars.TabStage == 0)
                {
                    switch (Vars.SetupStage)
                    {
                        case 0:
                            await MethodsSetup.Patreon();
                            break;

                        case 1:
                            await MethodsSetup.RLPath();
                            break;

                        case 3:
                            await MethodsSetup.IsPathValid(MethodsSetup.CleanedRLInput());
                            break;

                        case 4:
                            await MethodsSetup.BakkesPath();
                            break;

                        case 5:
                            await MethodsSetup.CheckBakkesPath();
                            break;

                        case 6:
                            if (await MethodsVerification.IsVerified(InfoData.Email))
                            {
                                MethodsData.WriteToInfo();
                                Tabs.UpdateTabs(setupStage: 12);
                                await UpdateStage();
                            }
                            else
                            {
                                await MethodsSetup.GetGameID();
                            }
                            break;

                        case 7:
                            await MethodsSetup.CheckGivenID();
                            break;

                        case 8:
                            switch (Vars.Tier)
                            {
                                case "1":
                                    MethodsSetup.PopulateChooseModComboBox();
                                    break;

                                case "2":
                                    await HandleTier2();
                                    break;

                                default:
                                    await UpdateStage(setupStage: 11);
                                    break;
                            }
                            break;

                        case 9:
                            if (Vars.Tier != "2")
                            {
                                await MethodsSetup.CheckCarWarn();
                            }
                            break;

                        case 11:
                            if (string.IsNullOrEmpty(Vars.Line))
                            {
                                Vars.Line = MethodsVerification.CreateUserLine();
                                await NetworkServer.SendToServer(Vars.Line);
                                MethodsData.WriteToInfo();

                                await UpdateStage(setupStage:12);
                            }
                            break;

                        case 12:
                            await MethodsSetup.DownloadAndConfigure();
                            break;

                        case 13:
                            MethodsSetup.PopulateStageEpicIDsComboBox();
                            break;
                    }
                }
                else
                {
                    await NetworkCURL.GetInfo();

                    if (Vars.TabStage == 1)
                    {
                        if (InfoData.PathType == "0")
                        {
                            Vars.MainWindow.VersionSteam.Source = Vars.bitSteamLogoSelect;
                        }
                        else
                        {
                            Vars.MainWindow.VersionEpic.Source = Vars.bitEpicLogoSelect;
                        }
                    }
                }

                Tabs.UpdateTabs();
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error UpdateStage: {ex.Message}", "Exit");
            }
        }



        /*
         * Tier 2 stage
         */

        /// <summary>
        /// Handles setup stages for tier 2 specific tabs
        /// </summary>
        /// <remarks>
        /// <see cref="Vars.Tier2Stage"/> navigates through the tier 2 stages
        /// 
        /// case 0: Showing Stage 8 mod selection for first basic mod
        /// fires when mod selection becomes visible for the first time
        /// uses GetInfo to cURL request mod Vars.ModInfo, Populates mod combo box with basic mods, Renames finish button to "Get Mod (1/2)"
        /// waits for button to be pressed
        /// 
        /// case 1, 3 and 5: Code for when Get Mod is pressed
        /// If Continue is pressed, go to Tier2Stage 2. If Back, Stage 8 and update tabs
        /// 
        /// case 2: Showing Stage 8 mod selection for second basic mod
        /// Sets T2Mod1 to ModSelection, sets FinishButton content to (2/3), populates combobox (with the first selected mod not included)
        /// 
        /// case 4: Showing Stage 8 mod selection for the last mod (premium or basic)
        /// 
        /// case 6: Finish setup and go to download
        /// </remarks>
        public static async Task HandleTier2()
        {
            try
            {
                switch (Vars.Tier2Stage)
                {
                    case 0:
                        Vars.MainWindow.FinishButton.Content = "Get Mod (1/3)";
                        await NetworkCURL.GetInfo();
                        MethodsSetup.PopulateChooseModComboBox();
                        break;

                    case 1 or 3 or 5:
                        await MethodsSetup.CheckCarWarn();
                        break;

                    case 2:
                        Vars.T2Mod1 = Vars.ModSelection;
                        Vars.MainWindow.FinishButton.Content = "Get Mod (2/3)";
                        MethodsSetup.PopulateChooseModComboBox();
                        break;

                    case 4:
                        Vars.T2Mod2 = Vars.ModSelection;
                        Vars.MainWindow.FinishButton.Content = "Finish";
                        MethodsSetup.PopulateChooseModComboBox();
                        break;

                    case 6:
                        Vars.ModSelection = $"{Vars.T2Mod1}_{Vars.T2Mod2}_{Vars.ModSelection}";
                        await UpdateStage(setupStage: 11);
                        break;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleTier2: {ex.Message}", "Exit");
            }
        }
    }
}
