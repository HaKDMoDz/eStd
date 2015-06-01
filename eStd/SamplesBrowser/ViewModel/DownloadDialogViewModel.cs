using System;
using System.Net;
using System.Windows.Input;
using System.Wpf.Mvvm;
using System.Wpf.Mvvm.Commands;

namespace SamplesBrowser.ViewModel
{
    public class DownloadDialogViewModel : ViewModelBase
    {
        private DownloadDialog downloadDialog;
        private bool done;

        public DownloadDialogViewModel(DownloadDialog downloadDialog)
        {
            this.downloadDialog = downloadDialog;

            DownloadSamples();

            OkCommand = new AsyncDelegateCommand(async (s) =>
            {
                downloadDialog.Close();
            }, (obj) => done);
        }

        public async void DownloadSamples()
        {
            var wc = new WebClient();
            var samples = await wc.DownloadStringTaskAsync("http://myvar.org/estd/get/all");

            foreach (var item in samples.Split('\r'))
            {
                await wc.DownloadStringTaskAsync(item);
            }
        }

        public ICommand OkCommand { get; set; }
    }
}