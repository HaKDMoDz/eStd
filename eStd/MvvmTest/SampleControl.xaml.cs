using System;
using System.Linq;
using System.Windows;
using MvvmTest.ViewModel;
using System.Windows.Controls;

namespace MvvmTest
{
    public partial class SampleControl : UserControl
    {
        public SampleControl()
        {
            DataContext = new SampleControlViewModel(this);

            InitializeComponent();
        }
    }
}