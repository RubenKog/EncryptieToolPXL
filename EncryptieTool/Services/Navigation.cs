using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using EncryptieTool.Views;

namespace EncryptieTool.Services
{
    public static class Navigation
    {
        #region Properties

        private static Frame MainFrame;

        private static Dictionary<string, Page> AppPages = new Dictionary<string, Page>
        {
            { "MainView", new MainView() },
            { "RsaView", new RsaView() },
            { "AesView", new AesView() },
            { "HashingView", new HashingView() }
        };

        #endregion

        /// <summary>
        /// Sets the MainFrame property to the specified Frame object.
        /// </summary>
        /// <param name="frame">The Frame object to set as the MainFrame.</param>
        public static void SetMainFrame(Frame frame)
        {
            MainFrame = frame;
        }

        /// <summary>
        /// Navigates to the specified page within the application by setting the MainFrame's content to the appropriate view. It checks if the MainFrame is set and throws an exception if not.
        /// </summary>
        /// <param name="page">Specifies the page to navigate to. Accepts MainView, RsaView and AesView.</param>
        /// <exception cref="Exception">Thrown when the Main Frame is not set.</exception>
        public static void Navigate(string pageName)
        {
            //! Return if Main Frame is not set
            if (MainFrame == null)
                throw new Exception("Main Frame is not set.");

            //! Cancel if we're already on that page
            if (AlreadyThere(pageName)) return;

            //> Navigate to the selected page
            MainFrame.Content = AppPages[pageName];
        }

        public static void GoBack()
        {
            //> Go back one page in the main frame
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }

        public static void ReloadPages()
        {
            //> Reload all pages
            AppPages["MainView"] = new MainView();
            AppPages["RsaView"] = new RsaView();
            AppPages["AesView"] = new AesView();
            AppPages["HashingView"] = new HashingView();
            //Refresh
            RefreshPage();
        }

        public static void RefreshPage()
        {
            MainFrame.NavigationService.Refresh();
        }

        private static bool AlreadyThere(string pageName)
        {
            //< Get Current Page
            //? Check if page title and page name match
            return MainFrame.Content is Page currentPage && currentPage.Title == pageName;
        }
    }
}