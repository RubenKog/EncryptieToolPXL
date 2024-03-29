using System;
using System.Collections.Generic;
using System.Windows.Controls;
using EncryptieTool.Views;

namespace EncryptieTool.Services
{
    public static class Navigation
    {
        #region Properties

        private static Frame MainFrame;
        private static readonly Dictionary<string, Page> AppPages = new Dictionary<string, Page>
        {
            { "MainView", new MainView() },
            { "RsaView", new RsaView() },
            { "AesView", new AesView() }
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
        /// <param name="page">Specifies the page to navigate to, as defined in the PageDictionary enum.</param>
        /// <exception cref="Exception">Thrown when the Main Frame is not set.</exception>
        public static void Navigate(string pageName)
        {
            //! Return if Main Frame is not set
            if (MainFrame == null)
            {
                throw new Exception("Main Frame is not set.");
            }
            
            //! Cancel if we're already on that page
            if (AlreadyThere(pageName))
                return;
            
            //> Navigate to the selected page
            MainFrame.Content = AppPages[pageName];
        }

        private static bool AlreadyThere(string pageName)
        {
            //< Get Current Page
            Page currentPage = MainFrame.Content as Page;

            //? Check if page title and page name match
            if (currentPage != null && currentPage.Title == pageName)
                return true;
            return false;
        }
    }
}