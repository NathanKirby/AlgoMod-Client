using System.Windows;
using System.IO;

namespace AlgoModSimpleWPF
{
    internal class MethodsPopup
    {
        /// <summary>
        /// Popup window to show messages like success and errors
        /// </summary>
        /// <param name="type">0 = closing the popup allows you to continue with the app, 1 = closing the popup closes the app (fatal error)</param>
        /// <param name="title">The title for the popup window</param>
        /// <param name="description">Message for the window</param>
        /// <param name="button">Text on the button to close it</param>
        public static void Popup(int type, string title, string description, string button)
        {
            if (!Vars.bFatalError)
            {
                if (type == 1)
                {
                    Vars.bFatalError = true;
                }

                Vars.PopupType = type;
                Vars.MainWindow.Popup_Title.Text = title;
                Vars.MainWindow.Popup_Description.Text = description;
                Vars.MainWindow.Popup_Button.Content = button;

                Vars.MainWindow.PopupGrid.Visibility = Visibility.Visible;

                TabMore.Log($"{title} (type={type}): {description}");
            }
        }


        /// <summary>
        /// Popup button click code
        /// </summary>
        public static void PopupClick()
        {
            // If popup type is fatal error delete info.txt and close app
            if (Vars.PopupType == 1)
            {
                string infoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "info.txt");

                if (File.Exists(infoPath))
                {
                    // Exception not handled because it's shutting down anyway and no difference would be made
                    File.Delete(infoPath);
                }

                Application.Current.Shutdown();
            }

            // Hide xaml popup
            Vars.MainWindow.PopupGrid.Visibility = Visibility.Collapsed;
        }
    }
}
