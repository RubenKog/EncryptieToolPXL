using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace EncryptieTool.Windows
{
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
            TxtInput.Focus();
        }

        //@"\", @"/", @":", @"|", @"*", @"?", "\"", @">", @"<"

        /// <summary>
        /// This is the text that the user has inputted.
        /// You can access this after the window has been closed.
        /// </summary>
        public string InputText { get; private set; }

        /// <summary>
        /// Indicates if the user has pressed the OK button.
        /// If not ok, cancel changing input.
        /// </summary>
        public bool IsOk { get; private set; }

        private void ButtonOk(object sender, RoutedEventArgs e)
        {
            InputText = TxtInput.Text;
            IsOk = true;
            Close();
        }

        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            Close();
        }

        private void TxtInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //Replace white space with underscore.
            TxtInput.Text = TxtInput.Text.Replace(" ", "_");
        }

        private void TxtInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Replace space with underscore.
            if (e.Key == Key.Space)
            {
                int index = TxtInput.CaretIndex;
                TxtInput.Text = TxtInput.Text.Insert(index, "_");
                TxtInput.CaretIndex = index + 1;
                e.Handled = true;
                return;
            }
        }
    }
}