using System.IO;
using System.Windows;
using System.Windows.Media;

namespace AlgoModSimpleWPF
{
    internal class Tabs
    {
        public static void UpdateTabs(int? tabStage = null, int? setupStage = null, int? fileTab = null, int? moreStage = null)
        {
            // Set values if defined
            Vars.TabStage = tabStage ?? Vars.TabStage;
            Vars.SetupStage = setupStage ?? Vars.SetupStage;
            Vars.FileTab = fileTab ?? Vars.FileTab;
            Vars.MoreStage = moreStage ?? Vars.MoreStage;

            #region SetupStage

            try
            {
                if (Vars.TabStage == 0)
                {
                    Vars.MainWindow.SetupGrid.Visibility = Visibility.Visible;

                    if (Vars.SetupStage == 0)
                    {
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;
                        Vars.MainWindow.NextButton.Visibility = Visibility.Hidden;

                        Vars.MainWindow.PatreonLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.PatreonButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.PatreonLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.PatreonButton.Visibility = Visibility.Hidden;
                    }

                    // Vars.SetupStage == 1
                    /// no code necessary

                    if (Vars.SetupStage == 2)
                    {
                        Vars.MainWindow.RLPathWarnLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.RLPathLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.EpicButton.Visibility = Visibility.Visible;
                        Vars.MainWindow.SteamButton.Visibility = Visibility.Visible;

                        Vars.MainWindow.NextButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Vars.MainWindow.RLPathWarnLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.RLPathLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.EpicButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.SteamButton.Visibility = Visibility.Hidden;
                    }

                    if (Vars.SetupStage == 3) // unable to find rocket league
                    {
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Visible;
                        Vars.MainWindow.NextButton.Visibility = Visibility.Visible;

                        Vars.MainWindow.RocketLeagueLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.RocketLeagueWarnLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.RocketLeagueLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.RocketLeagueWarnLabel.Visibility = Visibility.Hidden;
                    }

                    // Vars.SetupStage == 4
                    /// no code necessary

                    if (Vars.SetupStage == 5) // unable to find bakkesmod path
                    {
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Visible;
                        Vars.MainWindow.NextButton.Visibility = Visibility.Visible;

                        Vars.MainWindow.BakkesLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.BakkesWarnLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.BakkesLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.BakkesWarnLabel.Visibility = Visibility.Hidden;
                    }

                    // Vars.SetupStage == 6
                    /// no code necessary

                    if (Vars.SetupStage == 7) /// stage 7 unable to find ID
                    {
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Visible;
                        Vars.MainWindow.NextButton.Visibility = Visibility.Visible;

                        Vars.MainWindow.IDWarnLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.IDLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.IDWarnLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.IDLabel.Visibility = Visibility.Hidden;
                    }

                    if (Vars.SetupStage == 8)
                    {
                        Vars.MainWindow.NextButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;

                        Thickness adjusted = new(242, 22, 0, 0);

                        if (Vars.MainWindow.SetupQuestionDot.Margin != adjusted)
                        {
                            Vars.MainWindow.SetupQuestionDot.Margin = adjusted;
                        }

                        Vars.MainWindow.ModButtonBackground.Visibility = Visibility.Visible;
                        Vars.MainWindow.ModComboBox.Visibility = Visibility.Visible;
                        Vars.MainWindow.ModImage.Visibility = Visibility.Visible;
                        Vars.MainWindow.FinishButton.Visibility = Visibility.Visible;
                        Vars.MainWindow.ModName.Visibility = Visibility.Visible;
                        Vars.MainWindow.ModDescription.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Thickness normal = new(360, 31, 0, 0);

                        if (Vars.MainWindow.SetupQuestionDot.Margin != normal)
                        {
                            Vars.MainWindow.SetupQuestionDot.Margin = normal;
                        }

                        Vars.MainWindow.ModButtonBackground.Visibility = Visibility.Hidden;
                        Vars.MainWindow.ModComboBox.Visibility = Visibility.Hidden;
                        Vars.MainWindow.ModImage.Visibility = Visibility.Hidden;
                        Vars.MainWindow.FinishButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.ModName.Visibility = Visibility.Hidden;
                        Vars.MainWindow.ModDescription.Visibility = Visibility.Hidden;
                    }

                    if (Vars.SetupStage == 10)
                    {
                        Vars.MainWindow.NextButton.Visibility = Visibility.Visible;
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;

                        Vars.MainWindow.BackButton.Visibility = Visibility.Visible;
                        Vars.MainWindow.BodyWarnLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.BodyLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.BackButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.BodyWarnLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.BodyLabel.Visibility = Visibility.Hidden;
                    }

                    // Vars.SetupStage == 11
                    /// no code necessary

                    if (Vars.SetupStage == 12)
                    {
                        Vars.MainWindow.NextButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;

                        Vars.MainWindow.InstallStatus.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.InstallStatus.Visibility = Visibility.Hidden;
                    }

                    if (Vars.SetupStage == 13)
                    {
                        Vars.MainWindow.EpicID_WarnLabel.Visibility = Visibility.Visible;
                        Vars.MainWindow.EpicID_Text.Visibility = Visibility.Visible;
                        Vars.MainWindow.EpicID_ComboBox.Visibility = Visibility.Visible;
                        Vars.MainWindow.EpicID_Button.Visibility = Visibility.Visible;

                        Vars.MainWindow.NextButton.Visibility = Visibility.Hidden;
                        Vars.MainWindow.SetupTextBox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Vars.MainWindow.EpicID_WarnLabel.Visibility = Visibility.Hidden;
                        Vars.MainWindow.EpicID_Text.Visibility = Visibility.Hidden;
                        Vars.MainWindow.EpicID_ComboBox.Visibility = Visibility.Hidden;
                        Vars.MainWindow.EpicID_Button.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    /// hide setup grid if in tabs
                    Vars.MainWindow.SetupGrid.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(0, "Attention!", $"There was a visual error: {ex.Message}", "Okay");

                if (Vars.TabStage >= 0)
                {
                    /// sends you to help tab
                    UpdateTabs(tabStage: 5);
                }
            }

            #endregion

            #region Tabs

            try
            {
                if (Vars.TabStage != 0)
                {
                    SolidColorBrush TabColor = new((Color)ColorConverter.ConvertFromString("#FF2F2F2F"));

                    Vars.MainWindow.AllTabsGrid.Visibility = Visibility.Visible;
                    Vars.MainWindow.TabsGrid.Visibility = Visibility.Visible;

                    Vars.MainWindow.PatreonLabel.Visibility = Visibility.Hidden;
                    Vars.MainWindow.PatreonButton.Visibility = Visibility.Hidden;
                    Vars.MainWindow.InstallStatus.Visibility = Visibility.Hidden;
                    Vars.MainWindow.EpicID_WarnLabel.Visibility = Visibility.Hidden;
                    Vars.MainWindow.EpicID_Text.Visibility = Visibility.Hidden;
                    Vars.MainWindow.EpicID_ComboBox.Visibility = Visibility.Hidden;
                    Vars.MainWindow.EpicID_Button.Visibility = Visibility.Hidden;

                    if (Vars.MainWindow.HelpBrowser.Source == null)
                    {
                        Vars.MainWindow.HelpBrowser.Source = new Uri($"{Vars.WebsiteBase}algomod/how-to-installer");
                    }

                    if (Vars.TabStage == 1) /// injector
                    {
                        Vars.MainWindow.Tab1_Rectangle.Stroke = Brushes.DarkGray;

                        TabInjector.PopulateMapBox();
                        TabInjector.PopulateModBox();

                        Vars.MainWindow.InjectorGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.Tab1_Rectangle.Stroke = TabColor;

                        Vars.MainWindow.InjectorGrid.Visibility = Visibility.Collapsed;
                    }

                    if (Vars.TabStage == 2) /// file =========
                    {
                        Vars.MainWindow.Tab2_Rectangle.Stroke = Brushes.DarkGray;

                        Vars.MainWindow.FileGrid.Visibility = Visibility.Visible;

                        if (Vars.FileTab == 0) /// steam
                        {
                            Vars.MainWindow.SteamPath_Rectangle.Stroke = Brushes.DarkGray;

                            if (Directory.Exists(InfoData.SteamRLPath))
                            {
                                Vars.MainWindow.PathLabel_TextBlock.Text = "What would you like to do with your Steam Rocket League path?";
                                Vars.MainWindow.Path_TextBox.Text = InfoData.SteamRLPath;

                                Vars.MainWindow.Path_TextBox.Visibility = Visibility.Visible;
                                Vars.MainWindow.OpenFolder_Button.Visibility = Visibility.Visible;
                                Vars.MainWindow.AutoFind_Button.Visibility = Visibility.Visible;
                                Vars.MainWindow.SetPath_Button.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                Vars.MainWindow.PathLabel_TextBlock.Text = "Add your Steam Rocket League path here:";
                                Vars.MainWindow.Path_TextBox.Clear();
                            }
                        }
                        else
                        {
                            Vars.MainWindow.SteamPath_Rectangle.Stroke = TabColor;
                        }

                        if (Vars.FileTab == 1) /// epic
                        {
                            Vars.MainWindow.EpicPath_Rectangle.Stroke = Brushes.DarkGray;

                            if (Directory.Exists(InfoData.EpicRLPath))
                            {
                                Vars.MainWindow.PathLabel_TextBlock.Text = "What would you like to do with your Epic Rocket League path?";
                                Vars.MainWindow.Path_TextBox.Text = InfoData.EpicRLPath;
                            }
                            else
                            {
                                Vars.MainWindow.PathLabel_TextBlock.Text = "Add your Epic Rocket League path here:";
                                Vars.MainWindow.Path_TextBox.Clear();
                            }
                        }
                        else
                        {
                            Vars.MainWindow.EpicPath_Rectangle.Stroke = TabColor;
                        }

                        if (Vars.FileTab == 2) /// bakkes
                        {
                            Vars.MainWindow.PathLabel_TextBlock.Text = "What would you like to do with your Bakkesmod path?";

                            Vars.MainWindow.Path_TextBox.Text = InfoData.BakkesModPath;
                            Vars.MainWindow.BakkesPath_Rectangle.Stroke = Brushes.DarkGray;
                        }
                        else
                        {
                            Vars.MainWindow.BakkesPath_Rectangle.Stroke = TabColor;
                        }
                    }
                    else
                    {
                        Vars.MainWindow.Tab2_Rectangle.Stroke = TabColor;

                        Vars.MainWindow.FileGrid.Visibility = Visibility.Hidden;
                    }

                    if (Vars.TabStage == 3) /// more
                    {
                        Vars.MainWindow.Tab3_Rectangle.Stroke = Brushes.DarkGray;

                        Vars.MainWindow.MoreGrid.Visibility = Visibility.Visible;

                        if (Vars.MoreStage == 0) /// general
                        {
                            TabMore.SetInjectedModText();

                            Vars.MainWindow.General_Rectangle.Stroke = Brushes.DarkGray;

                            Vars.MainWindow.MoreGeneralTab.Visibility = Visibility.Visible;

                            TabMore.HandleLog(2); /// check checkbox if log is enabled
                            TabMore.HandleAdmin(3); /// same with admin box
                        }
                        else
                        {
                            Vars.MainWindow.General_Rectangle.Stroke = TabColor;

                            Vars.MainWindow.MoreGeneralTab.Visibility = Visibility.Collapsed;
                        }

                        if (Vars.MoreStage == 1) /// add mod
                        {
                            Vars.MainWindow.AddMod_Rectangle.Stroke = Brushes.DarkGray;

                            Vars.MainWindow.NoCredits_Text.Visibility = Visibility.Visible;

                            /// hiding my info and addid tab
                            Vars.MainWindow.MyInfo_Rectangle.Stroke = TabColor;
                            Vars.MainWindow.MyInfo_InfoText.Visibility = Visibility.Hidden;
                            Vars.MainWindow.AddID_Rectangle.Stroke = TabColor;
                            Vars.MainWindow.MoreAddIDTab.Visibility = Visibility.Collapsed;
                            Vars.MainWindow.NoAddID_Text.Visibility = Visibility.Hidden;

                            if (Vars.Tier != "1" && Vars.Tier != "2")
                            {
                                return;
                            }

                            if (!string.IsNullOrEmpty(Vars.UserLine))
                            {
                                /// check to see if user has mod credits
                                if (int.Parse(Vars.UserLine.Split('|')[5]) > 0)
                                {
                                    TabMore.PopulateAddModBox();

                                    Vars.MainWindow.Credits_Text.Text = $"You have {Vars.UserLine.Split('|')[5]} mod credits!";

                                    Vars.MainWindow.NoCredits_Text.Visibility = Visibility.Hidden;
                                    Vars.MainWindow.MoreAddModTab.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    Vars.MainWindow.NoCredits_Text.Visibility = Visibility.Visible;
                                    Vars.MainWindow.MoreAddModTab.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        else
                        {
                            Vars.MainWindow.AddMod_Rectangle.Stroke = TabColor;

                            Vars.MainWindow.NoCredits_Text.Visibility = Visibility.Hidden;

                            Vars.MainWindow.MoreAddModTab.Visibility = Visibility.Collapsed;
                        }

                        if (Vars.MoreStage == 2) /// add id
                        {
                            Vars.MainWindow.AddID_Rectangle.Stroke = Brushes.DarkGray;

                            TabMore.HandleAddID();

                            if (Vars.AddIDType == 0) /// show is false
                            {
                                /// change text for NoAddID_Text. Make it so if there is a possibility of adding a steam/epic path, it tells you
                                if (InfoData.EpicRLPath == "???")
                                {
                                    Vars.MainWindow.NoAddID_Text.Text = "Add your Epic rocketleague folder in the 'File' tab to add an Epic ID.";
                                }
                                else
                                {
                                    if (InfoData.SteamRLPath == "???")
                                    {
                                        Vars.MainWindow.NoAddID_Text.Text = "Add your Steam rocketleague folder in the 'File' tab to add a Steam ID.";
                                    }
                                    else
                                    {
                                        if (InfoData.SteamRLPath != "???" && InfoData.EpicRLPath != "???")
                                        {
                                            Vars.MainWindow.NoAddID_Text.Text = "All available IDs have been verified.";
                                        }
                                    }
                                }

                                Vars.MainWindow.NoAddID_Text.Visibility = Visibility.Visible;

                                Vars.MainWindow.MoreAddIDTab.Visibility = Visibility.Collapsed;
                            }

                            if (Vars.AddIDType == 1) /// steam id
                            {
                                Vars.MainWindow.NoAddID_Text.Visibility = Visibility.Hidden;

                                Vars.MainWindow.AddID_TextBox.Visibility = Visibility.Visible;
                                Vars.MainWindow.AddID_Button.Visibility = Visibility.Visible;
                                Vars.MainWindow.AddID_EpicComboBox.Visibility = Visibility.Hidden;
                                Vars.MainWindow.AddID_EpicText.Visibility = Visibility.Hidden;

                                Vars.MainWindow.MoreAddIDTab.Visibility = Visibility.Visible;
                            }

                            if (Vars.AddIDType == 2) /// epic id
                            {
                                Vars.MainWindow.NoAddID_Text.Visibility = Visibility.Hidden;

                                Vars.MainWindow.AddID_TextBox.Visibility = Visibility.Hidden;
                                Vars.MainWindow.AddID_Button.Visibility = Visibility.Visible;
                                Vars.MainWindow.AddID_EpicComboBox.Visibility = Visibility.Visible;
                                Vars.MainWindow.AddID_EpicText.Visibility = Visibility.Visible;

                                Vars.MainWindow.MoreAddIDTab.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            Vars.MainWindow.AddID_Rectangle.Stroke = TabColor;

                            Vars.MainWindow.MoreAddIDTab.Visibility = Visibility.Collapsed;
                            Vars.MainWindow.NoAddID_Text.Visibility = Visibility.Hidden;
                        }

                        if (Vars.MoreStage == 3) /// my info
                        {
                            Vars.MainWindow.MyInfo_Rectangle.Stroke = Brushes.DarkGray;

                            TabMore.HandleMyInfoClick();

                            Vars.MainWindow.MyInfo_InfoText.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Vars.MainWindow.MyInfo_Rectangle.Stroke = TabColor;

                            Vars.MainWindow.MyInfo_InfoText.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        Vars.MainWindow.Tab3_Rectangle.Stroke = TabColor;

                        Vars.MainWindow.MoreGrid.Visibility = Visibility.Collapsed;
                    }

                    if (Vars.TabStage == 4) /// link
                    {
                        Vars.MainWindow.Tab4_Rectangle.Stroke = Brushes.DarkGray;

                        Vars.MainWindow.PatreonBanner.Visibility = Visibility.Visible;
                        Vars.MainWindow.DiscordBanner.Visibility = Visibility.Visible;
                        Vars.MainWindow.WebsiteBanner.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Vars.MainWindow.Tab4_Rectangle.Stroke = TabColor;

                        Vars.MainWindow.PatreonBanner.Visibility = Visibility.Hidden;
                        Vars.MainWindow.DiscordBanner.Visibility = Visibility.Hidden;
                        Vars.MainWindow.WebsiteBanner.Visibility = Visibility.Hidden;
                    }

                    if (Vars.TabStage == 5) /// help
                    {
                        Vars.MainWindow.Tab5_Rectangle.Stroke = Brushes.DarkGray;
                        Vars.MainWindow.HelpBrowser.Visibility = Visibility.Visible;
                        Vars.MainWindow.Height = 400;
                    }
                    else
                    {
                        Vars.MainWindow.Tab5_Rectangle.Stroke = TabColor;

                        Vars.MainWindow.HelpBrowser.Visibility = Visibility.Hidden;

                        if (Vars.MainWindow.Height == 400)
                        {
                            Vars.MainWindow.Height = 150;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(0, "Attention!", $"There was a visual error: {ex.Message}", "Okay");

                /// sends you to help tab
                UpdateTabs(tabStage: 5);
            }

            #endregion
        }
    }
}
