using System;

namespace MvvmTest
{
    public class Sample : SamplesBrowser.Model.Sample
    {
        public Sample()
        {
            Name = "Mvvm";
            View = new SampleControl();
        }
    }
}