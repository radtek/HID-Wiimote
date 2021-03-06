﻿/*

Copyright (C) 2017 Julian Löhr
All rights reserved.

Filename:
	InstallerWindow.xaml.cs

Abstract:
	Installer Window

*/
using HIDWiimote.ControlCenter.Setup.SetupAction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HIDWiimote.ControlCenter.Main_Windows
{
    /// <summary>
    /// Interaction logic for InstallerWindow.xaml
    /// </summary>
    public partial class InstallerWindow : Window
    {
        List<InstallerAction> ActionList = new List<InstallerAction>();

        public InstallerWindow()
        {
            ActionList.Add(new InstallerAction(
                HIDWiimote.ControlCenter.Properties.Installer.TestMode_Title,
                false,
                HIDWiimote.ControlCenter.Properties.Installer.TestMode_Description,
                HIDWiimote.ControlCenter.Properties.Installer.TestMode_SmallDescription,
                HIDWiimote.ControlCenter.Properties.Installer.TestMode_RedNote,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonDisable,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonEnable,
                new TestMode()
                ));

            ActionList.Add(new InstallerAction(
                HIDWiimote.ControlCenter.Properties.Installer.Certificate_Title,
                false,
                HIDWiimote.ControlCenter.Properties.Installer.Certificate_Description,
                HIDWiimote.ControlCenter.Properties.Installer.Certificate_SmallDescription,
                string.Empty,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonUninstall,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonInstall,
                new Certificate()
                ));

            ActionList.Add(new InstallerAction(
                HIDWiimote.ControlCenter.Properties.Installer.DeviceDriver_Title,
                true,
                HIDWiimote.ControlCenter.Properties.Installer.DeviceDriver_Description,
                HIDWiimote.ControlCenter.Properties.Installer.DeviceDriver_SmallDescription,
                string.Empty,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonUninstall,
                HIDWiimote.ControlCenter.Properties.Installer.ButtonInstall,
                new Setup.SetupAction.DeviceDriver()
                ));

            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            ActionPanel.ItemsSource = ActionList;
        }

        private void InstallerActionButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement Button = sender as FrameworkElement;

            if (Button != null)
            {
                InstallerAction Action = Button.DataContext as InstallerAction;
                if (Action != null)
                {
                    Action.ButtonClicked();
                }
            }

        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (AllRequirementsFulfilled())
            {
                App.ChangeMainWindow(new ControlCenterWindow(), this);
            }
            else
            {
                Close();
            }

        }

        private bool AllRequirementsFulfilled()
        {
            foreach (InstallerAction Action in ActionList)
            {
                if (Action.Required)
                {
                    if (!Action.IsGood)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class InstallerAction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private string _Title;
        private bool _Required;
        private string _Description;
        private string _SmallDescription;
        private string _RedNote;
        private bool _ShowRedNote;

        private bool _IsGood;
        private bool _TaskHasReturned;
        private string GoodButtonText;
        private string BadButtonText;

        private ISetupAction InstallerTask;

        public InstallerAction(string Title, bool Required, string Description, string SmallDescription, string RedNote, string GoodButtonText, string BadButtonText, ISetupAction InstallerTask)
        {
            this.Title = Title;
            this._Required = Required;
            this.Description = Description;
            this.SmallDescription = SmallDescription;
            this._RedNote = RedNote;
            this.GoodButtonText = GoodButtonText;
            this.BadButtonText = BadButtonText;

            this.InstallerTask = InstallerTask;

            this.ShowRedNote = false;
            this.IsGood = false;
            this.TaskHasReturned = true;

            StartTask(CheckIsGood);
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool Required
        {
            get
            {
                return _Required;
            }
        }

        public string RequiredOptional
        {
            get
            {
                if (Required)
                {
                    return "- " + HIDWiimote.ControlCenter.Properties.Installer.ActionRequired;
                }
                else
                {
                    return "- " + HIDWiimote.ControlCenter.Properties.Installer.ActionOptional;
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        public string SmallDescription
        {
            get { return _SmallDescription; }
            set
            {
                _SmallDescription = value;
                OnPropertyChanged("SmallDescription");
            }
        }

        public string RedNote
        {
            get
            {
                if (ShowRedNote)
                {
                    return _RedNote;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool ShowRedNote
        {
            get { return _ShowRedNote; }
            set
            {
                _ShowRedNote = value;
                OnPropertyChanged("ShowRedNote");
                OnPropertyChanged("RedNote");
            }
        }

        public string ButtonText
        {
            get
            {
                if (IsGood)
                {
                    return GoodButtonText;
                }
                else
                {
                    return BadButtonText;
                }
            }
        }

        public bool IsGood
        {
            get { return _IsGood; }
            set
            {
                _IsGood = value;
                OnPropertyChanged("IsGood");
                OnPropertyChanged("ButtonText");
            }
        }

        public bool TaskHasReturned
        {
            get { return _TaskHasReturned; }
            set
            {
                _TaskHasReturned = value;
                OnPropertyChanged("TaskHasReturned");
            }
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null)
            {
                Handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        private void StartTask(Action TaskAction)
        {
            if (!TaskHasReturned)
            {
                return;
            }

            TaskHasReturned = false;
            Task NewTask = new Task(TaskAction);
            NewTask.ContinueWith(TaskCompletion);
            NewTask.Start(TaskScheduler.Default);
        }

        private void TaskCompletion(Task CompletedTask)
        {
            TaskHasReturned = true;
        }

        private void CheckIsGood()
        {
            IsGood = InstallerTask.IsSetUp();
        }

        private void ButtonAction()
        {
            try
            {
                if (IsGood)
                {
                    InstallerTask.TryRevert();
                }
                else
                {
                    InstallerTask.TrySetUp();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(HIDWiimote.ControlCenter.Properties.Installer.InstallerAction_ExceptionDialogMessage + Title + "\n\n" + e.Message, HIDWiimote.ControlCenter.Properties.Installer.InstallerAction_ExceptionDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsGood = !IsGood;
            ShowRedNote = !ShowRedNote;
        }

        public void ButtonClicked()
        {
            StartTask(ButtonAction);
        }
    }

    public class BooleanToColorConverter : IValueConverter
    {
        public SolidColorBrush TrueColor { get; set; }
        public SolidColorBrush FalseColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Value = (bool)value;

            return Value ? TrueColor : FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
