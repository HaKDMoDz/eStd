using SamplesBrowser.ViewModel;
using System;
using System.Windows;

namespace SamplesBrowser
{
    public partial class DownloadDialog : Window
    {
        public DownloadDialog()
        {
            InitializeComponent();

            DataContext = new DownloadDialogViewModel(this);
        }
    }
}