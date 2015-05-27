using System;
using System.IO;
using System.Net;
using System.Text;

namespace System.Network
{
    public class PausableWebClient
    {
        private readonly System.Net.WebClient wc;
        private string _url;
        private string file;

        private bool finish;

        public PausableWebClient()
        {
            wc = new System.Net.WebClient();
        }

        public void DownloadFile(string url, string filename)
        {
            file = filename;
            _url = url;
            wc.DownloadFileAsync(new Uri(url), filename);
            finish = true;
        }

        public string DownloadString(string url)
        {
            return wc.DownloadString(new Uri(url));
        }

        public byte[] DownloadData(string url)
        {
            return Encoding.ASCII.GetBytes(DownloadString(url));
        }

        public void Cancel()
        {
            wc.CancelAsync();
        }

        public void Resume()
        {
            long lStartPos = 0;
            FileStream fs = null;
            if (File.Exists(file))
            {
                fs = File.OpenWrite(file);
                lStartPos = fs.Length;
                //current point within the stream
                fs.Seek(lStartPos, SeekOrigin.Current);
            }
            else
            {
                fs = new FileStream(file, FileMode.Create);
                lStartPos = 0;
            }

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(_url);
                if (lStartPos > 0)
                {
                    request.AddRange(Convert.ToInt32(lStartPos));
                }
                //set the value of range             
                //send request to sever, gain the response data stream
                Stream ns = request.GetResponse().GetResponseStream();
                var nbytes = new byte[512];
                int nReadSize = 0;
                nReadSize = ns.Read(nbytes, 0, 512);
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = ns.Read(nbytes, 0, 512);
                }
                fs.Close();
                ns.Close();
                Console.WriteLine("Download complete");
            }
            catch (Exception ex)
            {
                fs.Close();
                Console.WriteLine("Error occured during downloading" + ex);
            }
        }

        public void Pause()
        {
            Cancel();
        }

        public void DownloadFile(string url)
        {
            DownloadFile(url, Path.GetFileName(url));
        }
    }
}