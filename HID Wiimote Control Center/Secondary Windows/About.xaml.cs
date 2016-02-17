﻿using System.Diagnostics;
using System.Windows;

namespace HID_Wiimote_Control_Center.Secondary_Windows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }
        private void OnInitialized(object sender, System.EventArgs e)
        {
            DriverPackageHeader.Content += VersionStrings.DriverPackageVersion;
            ControlCenterHeader.Content += VersionStrings.ControlCenterVersion;
        }

        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}