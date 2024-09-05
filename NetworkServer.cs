using System.Net.Sockets;
using System.Net;
using System.Text;

namespace AlgoModSimpleWPF
{
    internal class NetworkServer
    {
        /// <summary>
        /// Sends a message to the server and handles it's response
        /// </summary>
        /// <param name="message">Message from the client</param>
        /// <remarks>
        /// <see cref="HandleResponse"/>Processes the reponse received from the server
        /// <see cref="GetDomainIP(string)"/>Gets the IP of the server from <seealso cref="Vars.WebsiteDomain"/>
        /// </remarks>
        public static async Task SendToServer(string message)
        {
            try
            {
                int port = 56208;

                // Get IP from website domain
                string ip = GetDomainIP(Vars.WebsiteDomain);
                if (string.IsNullOrEmpty(ip) || ip == Vars.WebsiteDomain)
                {
                    throw new AlgoException("SendToServer", $"IP for domain '{Vars.WebsiteDomain}' is inaccessible!");
                }

                // Create new UDP client
                using UdpClient udpClient = new();

                // Send message
                byte[] data = Encoding.ASCII.GetBytes(Cryptography.EncryptMessage(message));
                udpClient.Send(data, data.Length, ip, port);

                // Receive response
                IPEndPoint serverEndPoint = new(IPAddress.Parse(ip), port);
                byte[] receivedBytes = udpClient.Receive(ref serverEndPoint);

                // Handle the response received
                await HandleResponse(Encoding.ASCII.GetString(receivedBytes));
            }
            catch (AlgoException ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error {ex.Method}: {ex.ErrorMessage}", "Exit");
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error SendToServer: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Processes response received from the server
        /// </summary>
        private static async Task HandleResponse(string serverResponse)
        {
            try
            {
                // Ensures the response isn't empty or null
                if (!string.IsNullOrEmpty(serverResponse))
                {
                    // In the case of receiving sensitive into, set vars
                    if (serverResponse.Contains("SENSITIVE|"))
                    {
                        // ClientID|ClientSecret|FirstKey|FirstIV
                        string[] sensitiveInfo = serverResponse.Split('|');

                        SensitiveData.ClientID = sensitiveInfo[1];
                        SensitiveData.ClientSecret = sensitiveInfo[2];
                        SensitiveData.MessageKey = sensitiveInfo[3];
                        SensitiveData.MessageIV = sensitiveInfo[4];
                        SensitiveData.IDsKey = sensitiveInfo[5];

                        return;
                    }

                    // Shows server response with popup
                    if (Vars.serverMessageDic.TryGetValue(serverResponse, out var codeDetails))
                    {
                        MethodsPopup.Popup(codeDetails.code, codeDetails.title, codeDetails.message, codeDetails.button);
                    }
                    else
                    {
                        MethodsPopup.Popup(1, "Server Fatal Error!", "Server sent back an impossible code!", "Exit");
                    }

                    // Update IDs server responses that indicated a change was made to the list
                    if (serverResponse is "200" or "202")
                    {
                        await NetworkCURL.GetIDS();
                        MethodsData.WriteToInfo();
                        Tabs.UpdateTabs();
                    }
                }
                else
                {
                    MethodsPopup.Popup(1, "Server Fatal Error!", "Server sent back an empty response!", "Exit");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleResponse: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Helper method for <see cref="SendToServer(string)"/> that gets the server's IP from <seealso cref="Vars.WebsiteDomain"/>
        /// </summary>
        /// <returns>Returns server IP</returns>
        private static string GetDomainIP(string url)
        {
            IPAddress[] domainAddresses = Dns.GetHostAddresses(url);
            return domainAddresses.FirstOrDefault()?.ToString() ?? url;
        }
    }
}
