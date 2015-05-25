﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represents a page adress inside a page structure - index could be byte offset position OR index in a list
    /// </summary>
    internal struct PageAddress
    {
        public const int SIZE = 6;

        public static PageAddress Empty = new PageAddress(uint.MaxValue, ushort.MaxValue);

        public uint PageID;

        public ushort Index;

        public bool IsEmpty
        { 
            get { return PageID == uint.MaxValue; }
        }

        public override bool Equals(object obj)
        {
            var other = (PageAddress)obj;
            return this.PageID == other.PageID && this.Index == other.Index;
        }

        public override int GetHashCode()
        {
            return (this.PageID + this.Index).GetHashCode();
        }

        public PageAddress(uint pageID, ushort index)
        {
            PageID = pageID;
            Index = index;
        }

        public override string ToString()
        {
            return IsEmpty ? "----" : PageID.ToString() + ":" + Index.ToString();
        }
    }
}
