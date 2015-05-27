using System.Collections.Generic;

namespace System.Net.Torrent
{
    public interface ITrackerClient
    {
        String Tracker { get; }
        Int32 Port { get; }

        BaseScraper.AnnounceInfo Announce(String url, String hash, String peerId);
        BaseScraper.AnnounceInfo Announce(String url, String hash, String peerId, Int64 bytesDownloaded, Int64 bytesLeft, Int64 bytesUploaded, 
            Int32 eventTypeFilter, Int32 ipAddress, Int32 numWant, Int32 listenPort, Int32 extensions);
        IDictionary<String, BaseScraper.AnnounceInfo> Announce(String url, String[] hashes, String peerId);
        IDictionary<String, BaseScraper.ScrapeInfo> Scrape(String url, String[] hashes);
    }
}