using System.IO;

namespace AlgoModSimpleWPF
{
    internal class MethodsData
    {
        /// <summary>
        /// Writes current relevant info to info.txt for later use
        /// </summary>
        public static void WriteToInfo()
        {
            try
            {
                string newInfoContent = string.Join(Vars.FileInfoSeparator, InfoData.InfoList);

                File.WriteAllText(Vars.InfoPath, newInfoContent);
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error WriteToInfo: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Sets global vars to info found in info.txt
        /// </summary>
        /// <returns>Returns a bool showing if a valid info file exists or not</returns>
        public static bool ReadInfoFile()
        {
            try
            {
                if (File.Exists(Vars.InfoPath))
                {
                    string currentContent = File.ReadAllText(Vars.InfoPath);
                    string[] currentContentList = currentContent.Split(Vars.FileInfoSeparator);

                    if (currentContentList.Length > 6)
                    {
                        InfoData.Email = currentContentList[0];
                        InfoData.SteamID = currentContentList[1];
                        InfoData.EpicID = currentContentList[2];
                        InfoData.SteamRLPath = currentContentList[3];
                        InfoData.EpicRLPath = currentContentList[4];
                        InfoData.BakkesModPath = currentContentList[5];
                        InfoData.PathType = currentContentList[6];

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error ReadInfoFile: {ex.Message}", "Exit");
            }

            return false;
        }


        /// <summary>
        /// Clears all info vars that are used for info.txt
        /// </summary>
        public static void ClearInfoVars()
        {
            Vars.UserLine = string.Empty;

            InfoData.ClearInfo();
        }
    }
}
