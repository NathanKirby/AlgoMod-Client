using System.Net.Http;
using System.IO;

namespace AlgoModSimpleWPF
{
    internal class NetworkCURL
    {
        /// <summary>
        /// Uses cURL request to gather and process ID lines from website
        /// </summary>
        /// <remarks><see cref="Cryptography.DecryptIDS"/> converts IDs to plain text</remarks>
        public static async Task GetIDS()
        {
            try
            {
                // Performs cURL request to get IDs from website
                HttpResponseMessage response = await Vars.AlgoHttpClient.GetAsync(Vars.WebsiteBase + "algomod/ids.txt");
                string content = await response.Content.ReadAsStringAsync();

                // Ensures request was successful
                if (!string.IsNullOrEmpty(content) && response.IsSuccessStatusCode)
                {
                    // Decrypt IDs into plain text
                    string decryptedContent = Cryptography.DecryptIDS(content);

                    // Capture IDs in var
                    Vars.RawIds = decryptedContent;

                    // Write IDs to file
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ids.txt"), decryptedContent);

                    // Update user info vars
                    MethodsVerification.UpdateUserLine();
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error GetIDS: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Uses cURL request to capture website info
        /// </summary>
        public static async Task GetInfo()
        {
            if (string.IsNullOrEmpty(Vars.WebsiteInfo))
            {
                try
                {
                    HttpResponseMessage response = await Vars.AlgoHttpClient.GetAsync(Vars.WebsiteBase + "algomod/info.txt");
                    Vars.WebsiteInfo = await response.Content.ReadAsStringAsync();

                }
                catch (Exception ex)
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error GetInfo: {ex.Message}", "Exit");
                }
            }
        }


        /// <summary>
        /// Connects to google to see if internet is connected
        /// </summary>
        /// <returns>Returns internet connection status as bool</returns>
        public static async Task<bool> IsInternetAvailable()
        {
            try
            {
                HttpResponseMessage httpResponse = await Vars.AlgoHttpClient.GetAsync("http://www.google.com/generate_204");
                return httpResponse.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Sends a header request to given URL to see if the link is valid
        /// </summary>
        /// <param name="url">Link to check</param>
        /// <returns>true if the link is valid, false if not</returns>
        public static async Task<bool> IsUrlValid(string url)
        {
            try
            {
                HttpResponseMessage httpResponse = await Vars.AlgoHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                return httpResponse.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
