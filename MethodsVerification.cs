using System.Text;

namespace AlgoModSimpleWPF
{
    internal class MethodsVerification
    {
        /// <summary>
        /// Checks to see if a user is present in the website's IDs
        /// </summary>
        /// <param name="emailOrID">This is the user's identification, can be an email or a steam/epic game ID</param>
        /// <returns>Returns a bool based on if they're verified or not</returns>
        public static async Task<bool> IsVerified(string emailOrID)
        {
            try
            {
                // Gets IDs from website, decrypts them into plain text
                await NetworkCURL.GetIDS();

                // Ensures cURL was successful and a valid identification param was given 
                if (!string.IsNullOrEmpty(emailOrID.Trim()) && !string.IsNullOrEmpty(Vars.RawIds))
                {
                    // Split IDs into list
                    List<string> rawIDsList = Vars.RawIds.Split(',').ToList();

                    // Looks for identification in IDs list
                    string? userIDLine = rawIDsList.FirstOrDefault(line => line.Contains(emailOrID));

                    if (userIDLine == null)
                    {
                        // User not found
                        return false;
                    }
                    else
                    {
                        // User found
                        string[] userLineInfo = userIDLine.Split('|');

                        // Ensure the line was found due to an email, steam id or epic id
                        if (emailOrID == userLineInfo[1] || emailOrID == userLineInfo[2] || emailOrID == userLineInfo[3])
                        {
                            Vars.UserLine = userIDLine.Trim();
                            return true;
                        }
                        else
                        {
                            // verification given was not an email or game ID
                            return false;
                        }
                    }
                }
                else
                {
                    // cURL not successful or identification param empty/null
                    return false;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error IsVerified: {ex.Message}", "Exit");
                return false;
            }
        }


        /// <summary>
        /// Gets current version of userline from IDs and sets it's values to vars
        /// </summary>
        public static void UpdateUserLine()
        {
            try
            {
                // Makes sure IDs var has been found and Email is present
                if (!string.IsNullOrEmpty(InfoData.Email.Trim()) && !string.IsNullOrEmpty(Vars.RawIds))
                {
                    // Split IDs into list
                    List<string> rawIDsList = Vars.RawIds.Split(',').ToList();

                    // Find user line
                    string? userIDLine = rawIDsList.FirstOrDefault(line => !string.IsNullOrEmpty(line) && line.Split('|')[3] == InfoData.Email);

                    // Ensure line was found
                    if (userIDLine != null)
                    {
                        // Capture userline 
                        Vars.UserLine = userIDLine.Trim();

                        // Assign line parts to vars
                        string[] lineParts = userIDLine.Split('|');

                        InfoData.EpicID = lineParts.ElementAtOrDefault(1) ?? "???";
                        InfoData.SteamID = lineParts.ElementAtOrDefault(2) ?? "???";
                        InfoData.Email = lineParts.ElementAtOrDefault(3) ?? "???";
                        Vars.Tier = lineParts.ElementAtOrDefault(4) ?? "???";
                        Vars.ModCredits = lineParts.ElementAtOrDefault(5) ?? "???";
                        Vars.Cents = lineParts.ElementAtOrDefault(6) ?? "???";
                        Vars.VerifiedMods = lineParts.ElementAtOrDefault(7) ?? "???";

                        // Sets path type 
                        if (InfoData.EpicID == "???" && InfoData.SteamID != "???")
                        {
                            InfoData.PathType = "0";
                        }
                        else if (InfoData.EpicID != "???" && InfoData.SteamID == "???")
                        {
                            InfoData.PathType = "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error IsVerified: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Builds a userline to send to the server
        /// </summary>
        /// <remarks>If user has unlimited mods, creates and adds a list of all mods to line</remarks>
        /// <returns>Returns built user line as string</returns>
        public static string CreateUserLine()
        {
            try
            {
                // Ensures ID has been found
                if (!string.IsNullOrEmpty(InfoData.SteamID) && !string.IsNullOrEmpty(InfoData.EpicID))
                {
                    // Ensures Patreon info has been found
                    if (!string.IsNullOrEmpty(InfoData.Email) && !string.IsNullOrEmpty(Vars.Tier) && !string.IsNullOrEmpty(Vars.Cents))
                    {
                        // Adds all mods to a users line if they have unlimited access
                        if (Vars.Tier is not "1" and not "2")
                        {
                            string[] modList = Vars.WebsiteInfo.Split("|||")[0].Split("||");

                            // User string builder to make a list of mod id's
                            StringBuilder modSelectionBuilder = new();

                            foreach (string mod in modList)
                            {
                                string modID = mod.Split('|')[0].Trim();

                                // Add separator if it's not the first time
                                if (modSelectionBuilder.Length > 0)
                                {
                                    modSelectionBuilder.Append('_');
                                }

                                // Add modID to list
                                modSelectionBuilder.Append(modID);
                            }

                            Vars.ModSelection = modSelectionBuilder.ToString();
                        }

                        // Build and return line
                        return CleanString($"USER|{InfoData.EpicID}|{InfoData.SteamID}|{InfoData.Email}|{Vars.Tier}|0|{Vars.Cents}|{Vars.ModSelection},", true, true, true, false);
                    }
                    else
                    {
                        MethodsPopup.Popup(1, "Fatal Error!", $"Error CreateUserLine: Could not get your information!", "Exit");
                        return "error";
                    }
                }
                else
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error CreateUserLine: You are not verified!", "Exit");
                    return "error";
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error CreateUserLine: {ex.Message}", "Exit");
                return "error";
            }
        }


        /// <summary>
        /// String cleaner with more options than .Trim()
        /// </summary>
        public static string CleanString(string input, bool newLine, bool carriageReturn, bool spaces, bool caps)
        {
            string output = input;

            if (newLine)
            {
                output = output.Replace("\n", string.Empty);
            }

            if (carriageReturn)
            {
                output = output.Replace("\r", string.Empty);
            }

            if (spaces)
            {
                output = output.Replace(" ", string.Empty);
            }

            if (caps)
            {
                output = output.ToLower();
            }

            return output;
        }
    }
}
