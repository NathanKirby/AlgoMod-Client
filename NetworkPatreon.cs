using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.IO;

namespace AlgoModSimpleWPF
{
    internal class NetworkPatreon
    {
        /// <summary>
        /// Handles and executes methods to do the following
        /// <see cref="RequestOAuthToken"/> Open a browser with an OAuth authentication page and start a listener to get the repsonse
        /// <see cref="GetAccessToken"/> Use the OAuth token gathered to get an access token for Patreon's API
        /// <see cref="GetPatreonEmails"/> Get user's patreon info
        /// </summary>
        /// <returns>Returns list of patron's info</returns>
        public static async Task<string> Patreon()
        {
            try
            {
                Vars.OAuthToken = string.Empty;

                // Prompts user to agree to API call to get OAuth token
                RequestOAuthToken();

                // Wait for user to accept and token to be grabbed by listener
                while (string.IsNullOrEmpty(Vars.OAuthToken))
                {
                    await Task.Delay(100);
                }

                // Gets access token from patreon using OAuth token
                Vars.AccessToken = await GetAccessToken();

                if (!string.IsNullOrEmpty(Vars.AccessToken))
                {
                    // Gathers and returns patreon info using access token
                    return await GetPatreonEmails();
                }
                else
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error Patreon: Access token is null!", "Exit");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error Patreon: {ex.Message}", "Exit");

            }

            return string.Empty;
        }


        /// <summary>
        /// Start HttpListener to receive OAuth token
        /// </summary>
        private static void StartListener()
        {
            try
            {
                // Configure and start listener
                Vars.PatreonListner.Prefixes.Add("http://localhost:8080/");
                Vars.PatreonListner.Start();

                // Listen on separate thread
                Task.Run(async () =>
                {
                    while (Vars.PatreonListner.IsListening)
                    {
                        HttpListenerContext context = await Vars.PatreonListner.GetContextAsync();
                        await HandleRequest(context);
                    }
                });
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error StartListener: {ex.Message}", "Exit");
            }
        }


        /// <summary>
        /// Handle response from website backend to get OAuthToken
        /// </summary>
        private static async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                using HttpListenerResponse response = context.Response;

                // Set CORS headers
                response.AppendHeader("Access-Control-Allow-Origin", "*");
                response.AppendHeader("Access-Control-Allow-Methods", "POST");
                response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");

                // Return in case of an options request
                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }

                if (request.HttpMethod == "POST" && request?.Url?.LocalPath == "/data")
                {
                    // Read returned token
                    using StreamReader reader = new(request.InputStream, request.ContentEncoding);
                    string requestData = await reader.ReadToEndAsync();

                    // Setup response
                    string responseString = "Data received successfully! You can close this page now.";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    // Send successful response
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer);

                    // Set OAuthToken var on main thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Vars.OAuthToken = requestData.Replace("\"", "");
                    });
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error HandleRequest: {ex.Message}", "Exit");
            }
            finally
            {
                Vars.PatreonListner.Stop();
            }
        }


        /// <summary>
        /// User OAuth token to get access token from Patreon's API
        /// </summary>
        private static async Task<string> GetAccessToken()
        {
            try
            {
                // Configure POST request for access token using Patreon's API
                string tokenUrl = "https://www.patreon.com/api/oauth2/token";
                string postData = $"code={Vars.OAuthToken}&grant_type=authorization_code&client_id={SensitiveData.ClientID}&client_secret={SensitiveData.ClientSecret}&redirect_uri={Vars.RedirectURI}";

                // Commit Http request to get access token
                HttpResponseMessage response = await Vars.AlgoHttpClient.PostAsync(tokenUrl, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));

                // Ensures success
                if (response.IsSuccessStatusCode)
                {
                    // Read response json content as string async
                    string content = await response.Content.ReadAsStringAsync();

                    // Parse json to get access_token
                    JObject tokenJson = JObject.Parse(content);
                    JToken? accessToken = tokenJson["access_token"];

                    // Ensure access token was found
                    if (accessToken != null)
                    {
                        // Return token as string
                        return accessToken.ToString();
                    }
                    else
                    {
                        MethodsPopup.Popup(1, "Fatal Error!", $"Error GetAccessToken: HttpPost content null!", "Exit");
                    }
                }
                else
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error GetAccessToken: HttpPost failed!", "Exit");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error GetAccessToken: {ex.Message}", "Exit");

            }

            return string.Empty;
        }


        /// <summary>
        /// Gets user information from Patreon's API using AccessToken
        /// </summary>
        private static async Task<string> GetPatreonEmails()
        {
            try
            {
                // Define the API endpoint URL with query parameters
                string apiUrl = "https://www.patreon.com/api/oauth2/v2/identity?include=memberships.currently_entitled_tiers,memberships&fields%5Buser%5D=email&fields%5Bmember%5D=lifetime_support_cents";

                // Set the authorization header for the HTTP client using a Bearer token
                Vars.AlgoHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Vars.AccessToken);

                // Send a GET request to the API endpoint
                HttpResponseMessage response = await Vars.AlgoHttpClient.GetAsync(apiUrl);

                // Check to see if response code is successful
                if (response.IsSuccessStatusCode)
                {
                    // Return content as string
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    MethodsPopup.Popup(1, "Fatal Error!", $"Error GetPatreonEmails: Unable to access Patron info!", "Exit");
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error GetPatreonEmails: {ex.Message}", "Exit");

            }

            return string.Empty;
        }


        /// <summary>
        /// Starts HttpListener and initiates OAuth authorization in the default browser
        /// </summary>
        private static void RequestOAuthToken()
        {
            try
            {
                StartListener();
                Process.Start(new ProcessStartInfo($"https://www.patreon.com/oauth2/authorize?response_type=code&client_id={SensitiveData.ClientID}&redirect_uri={Vars.RedirectURI}&scope=identity%20identity%5Bemail%5D%20identity.memberships") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error RequestOAuthCode: {ex.Message}", "Exit");
            }
        }
    }
}
