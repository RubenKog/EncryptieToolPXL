﻿using KeysLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptieTool.Views
{
    /// <summary>
    /// Interaction logic for HashingView.xaml
    /// </summary>
    public partial class HashingView : Page
    {
        string File1;
        string File2;
        public HashingView()
        {
            File1 = null;
            File2 = null;
            InitializeComponent();
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            Hashing hashing = new Hashing();
            byte[] SHA256File1 = hashing.FileToHash(File1);
            byte[] SHA256File2 = hashing.FileToHash(File2);
            string SHA256String1 = Convert.ToBase64String(SHA256File1);
            string SHA256String2 = Convert.ToBase64String(SHA256File2);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Checking integrity through SHA256 hash:");
            if (SHA256File1 == null || SHA256File2 == null)
            {
                MessageBox.Show("Please select two files.");
            }
            else if (SHA256File1 != null && SHA256File2 != null && SHA256String1 == SHA256String2)
            {
                sb.AppendLine("");
                sb.AppendLine($"File 1 hash: {SHA256String1}");
                sb.AppendLine("");
                sb.AppendLine($"File 2 hash: {SHA256String2}");
                sb.AppendLine("");
                sb.AppendLine("Integrity check complete: Success!");
                MessageBox.Show(sb.ToString(), "Ontegrity Checker");
            }            
            else
            {
                sb.AppendLine("");
                sb.AppendLine($"File 1 hash: {SHA256String1}");
                sb.AppendLine("");
                sb.AppendLine($"File 2 hash: {SHA256String2}");
                sb.AppendLine("");
                sb.AppendLine("Integrity check complete: Failure!");
                MessageBox.Show(sb.ToString(), "Integrity Checker");
            }
            sb.Clear();

        }

        private void BtnSelectFile1_Click(object sender, RoutedEventArgs e)
        {
            File1 = GetFilePath();
            TxtPlainImgName1.Text = File1;
        }
        private void BtnSelectFile2_Click(object sender, RoutedEventArgs e)
        {
            File2 = GetFilePath();
            TxtPlainImgName2.Text = File2;
        }

        private string GetFilePath()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();

            
            openFileDialog.Title = "Select a file";
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
              
                string filePath = openFileDialog.FileName;
                return filePath;
                
            }
            else
            {
                Console.WriteLine("No file selected.");
                return null;
            }
        }
    }
}