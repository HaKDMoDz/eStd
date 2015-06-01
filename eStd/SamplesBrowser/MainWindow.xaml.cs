using SamplesBrowser.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SamplesBrowser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).SelectionChanged(sender as ListBox, e, this);
        }
    }
}