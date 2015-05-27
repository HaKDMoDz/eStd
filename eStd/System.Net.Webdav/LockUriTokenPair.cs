using System;

namespace System.Net.Webdav
{
    public class LockUriTokenPair
    {
            public readonly Uri Href;
            public readonly string lockToken;

            public LockUriTokenPair(Uri href, string lockToken) {
                
            }
        }
    }