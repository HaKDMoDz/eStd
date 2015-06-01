using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Wpf.Mvvm;
using SamplesBrowser.Model;

namespace SamplesBrowser.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<string> samples;
        private Dictionary<string, UserControl> _samples = new Dictionary<string, UserControl>();

        public ObservableCollection<string> Samples
        {
            get
            {
                return this.samples;
            }
            set
            {
                this.samples = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshSamplesCommand { get; set; }

        public void SelectionChanged(ListBox sender, SelectionChangedEventArgs e, MainWindow win)
        {
            foreach (var item in _samples)
            {
                if (((string)sender.SelectedItem) == item.Key)
                {
                    AddSample(win, item.Value);
                }
            }
        }

        private void AddSample(Window parent, UserControl ctrl)
        {
            Grid.SetColumn(ctrl, 1);
            Grid.SetRow(ctrl, 1);

            ctrl.ClearValue(UserControl.WidthProperty);
            ctrl.ClearValue(UserControl.HeightProperty);

            var g = parent.FindName("content") as Grid;
            g.Children.Add(ctrl);
        }

        public MainWindowViewModel()
        {
            Samples = new ObservableCollection<string>();

            RefreshSamplesCommand = new System.Wpf.Mvvm.Commands.AsyncDelegateCommand(async (t) =>
            {
                var d = new DownloadDialog();
                d.ShowDialog();
            }, (obj) => true);

            foreach (var s in Directory.GetFiles(Environment.CurrentDirectory + "\\Samples\\", "*.dll"))
            {
                var ass = Assembly.LoadFile(s);
                foreach (var t in ass.GetTypes())
                {
                    if (t.BaseType.Name == typeof(Sample).Name)
                    {
                        var inst = ass.CreateInstance(t.FullName) as Sample;

                        _samples.Add(inst.Name, inst.View);
                    }
                }
            }

            foreach (var item in _samples)
            {
                Samples.Add(item.Key);
            }
        }
    }
}