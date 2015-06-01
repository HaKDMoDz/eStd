using System;

namespace System.IO
{
    public struct ByteRange
    {
        public static readonly ByteRange MaxValue = new ByteRange(0, long.MaxValue);
        public static readonly ByteRange MinValue = new ByteRange(0, 0);
        private readonly long _End;
        private readonly long _Length;
        private readonly long _Start;

        public ByteRange(long start, long end)
        {
            if (start < 0 || end < 0)
                throw new FormatException("ByteRange does not support negative Start and End values.");

            if (start > end)
            {
                _Start = end;
                _End = start;
            }
            else
            {
                _Start = start;
                _End = end;
            }
            _Length = _End - _Start;
        }

        public long Start
        {
            get { return _Start; }
        }

        public long End
        {
            get { return _End; }
        }

        public long Length
        {
            get { return _Length; }
        }

        public bool Contains(long offset)
        {
            if (offset >= Start && offset <= End) return true;
            else return false;
        }

        public bool Contains(ByteRange range)
        {
            if (range.Start >= Start && range.End <= End) return true;
            return false;
        }

        public bool Contains(long start, long end)
        {
            if (start >= Start && end <= End) return true;
            else return false;
        }

        public bool Overlaps(ByteRange range)
        {
            if (Contains(range.Start) || Contains(range.End)) return true;
            else return false;
        }

        public bool Overlaps(long start, long end)
        {
            if (Contains(start) || Contains(end)) return true;
            else return false;
        }

        #region Operator Overloads

        private const long TOP_MASK = unchecked((long) 0xFFFFFFFF00000000);
        private const long BOTTOM_MASK = 0x00000000FFFFFFFF;

        public static bool operator <(ByteRange a, ByteRange b)
        {
            if (a.Start > b.Start && a.End <= b.End) return true;
            else if (a.Start >= b.Start && a.End < b.End) return true;
            else return false;
        }

        public static bool operator >(ByteRange a, ByteRange b)
        {
            if (a.Start < b.Start && a.End >= b.End) return true;
            else if (a.Start <= b.Start && a.End > b.End) return true;
            else return false;
        }

        public static bool operator ==(ByteRange a, ByteRange b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ByteRange a, ByteRange b)
        {
            return !(a == b);
        }

        public static bool operator >=(ByteRange a, ByteRange b)
        {
            if (a == b) return true;
            else if (a > b) return true;
            else return false;
        }

        public static bool operator <=(ByteRange a, ByteRange b)
        {
            if (a == b) return true;
            else if (a < b) return true;
            else return false;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && ((ByteRange) obj).Start == Start && ((ByteRange) obj).End == End;
        }

        public override int GetHashCode()
        {
            var hashcode = (int) ((Start & TOP_MASK) >> 32);
            hashcode ^= (int) (Start & BOTTOM_MASK);
            hashcode ^= (int) ((End & TOP_MASK) >> 32);
            hashcode ^= (int) (End & BOTTOM_MASK);

            return hashcode;
        }

        #endregion
    }
}