using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Wpf.Mvvm.Commands;
using MvvmTest.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Wpf.Mvvm;
using System.Threading.Tasks;
using System.Threading;

namespace MvvmTest.ViewModel
{
    public class SampleControlViewModel : ViewModelBase
    {
        private ObservableCollection<Person> persons;

        public ObservableCollection<Person> Persons
        {
            get
            {
                return this.persons;
            }
            set
            {
                this.persons = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private CancellationTokenSource cts;

        public SampleControlViewModel(SampleControl mw)
        {
            Persons = new ObservableCollection<Person>();
            cts = new CancellationTokenSource();

            CancelCommand = new DelegateCommand(async (t) =>
            {
                cts.Cancel();
            }, (obj) => !cts.Token.IsCancellationRequested);

            RefreshCommand = new DelegateCommand(async (t) =>
            {
                Persons = new ObservableCollection<Person>();
                cts = new CancellationTokenSource();

                for (int i = 1; i <= 100; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    var p = new Person();
                    p.Name = "Person " + i;
                    p.Address = "SomethingStreet " + i * 2;

                    var icon = new BitmapImage();

                    icon.BeginInit();
                    icon.UriSource = new Uri("https://cdn0.iconfinder.com/data/icons/users-android-l-lollipop-icon-pack/24/user-16.png");
                    icon.EndInit();

                    p.Icon = icon;

                    Persons.Add(p);
                   
                    mw.grid.ScrollIntoView(p);

                    await Task.Delay(250);
                }
            }, (obj) => true);

            mw.Loaded += (s, e) =>
            {
                RefreshCommand.Execute(null);
            };
        }
    }
}