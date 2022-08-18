using System;
using System.Collections.Generic;

namespace DataService
{
    public struct HistoryData : IComparable<HistoryData>
    {
        public short ID;
        public QUALITIES Quality;
        public Storage Value;
        public DateTime TimeStamp;

        public HistoryData(short id, QUALITIES qualitie, Storage value, DateTime timeStamp)
        {
            ID = id;
            Quality = qualitie;
            Value = value;
            TimeStamp = timeStamp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is HistoryData)
            {
                return this == (HistoryData)obj;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ TimeStamp.GetHashCode();
        }

        public static bool operator ==(HistoryData x, HistoryData y)
        {
            return x.ID == y.ID && x.TimeStamp == y.TimeStamp;
        }

        public static bool operator !=(HistoryData x, HistoryData y)
        {
            return x.ID != y.ID || x.TimeStamp != y.TimeStamp;
        }

        public static readonly HistoryData Empty = new HistoryData();

        public int CompareTo(HistoryData other)
        {
            int comp = this.TimeStamp.CompareTo(other.TimeStamp);
            return comp == 0 ? this.ID.CompareTo(other.ID) : comp;
        }
    }

    public class CompareHistoryData : IComparer<HistoryData>
    {
        public int Compare(HistoryData x, HistoryData y)
        {
            int c1 = x.TimeStamp.CompareTo(y.TimeStamp);
            return c1 == 0 ? x.ID.CompareTo(y.ID) : c1;
        }
    }
}
