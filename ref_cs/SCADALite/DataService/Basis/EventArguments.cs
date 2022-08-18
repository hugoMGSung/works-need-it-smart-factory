using System;
using System.Collections.Generic;

namespace DataService
{
    public class DataChangeEventArgs : EventArgs
    {
        public DataChangeEventArgs(int transactionID, IList<HistoryData> pValues)
        {
            this.TransactionID = transactionID;
            this.Values = pValues;
        }

        public int TransactionID;
        public IList<HistoryData> Values;
    }

    public class WriteCompleteEventArgs : EventArgs
    {
        public WriteCompleteEventArgs(int transactionID, short[] pIds, int[] errors)
        {
            this.TransactionID = transactionID;
            this.Values = pIds;
            this.Errors = errors;
        }

        public int TransactionID;
        public short[] Values;
        public int[] Errors;
    }

    public class IOErrorEventArgs : EventArgs
    {
        public IOErrorEventArgs(string reson)
        {
            Reason = reson;
        }
        public string Reason;
    }

    public class ValueChangingEventArgs<T> : EventArgs
    {
        public ValueChangingEventArgs(QUALITIES quality, T oldValue, T newValue, DateTime oldTime, DateTime newTime)
        {
            this.Quality = quality;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.OldTimeStamp = oldTime;
            this.NewTimeStamp = newTime;
        }

        public QUALITIES Quality;
        public T OldValue;
        public T NewValue;
        public DateTime OldTimeStamp;
        public DateTime NewTimeStamp;
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs(Storage value)
        {
            this.Value = value;
        }

        public Storage Value;
    }

    public delegate void ValueChangingEventHandler<T>(object sender, ValueChangingEventArgs<T> e);
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
    public delegate void IOErrorEventHandler(object sender, IOErrorEventArgs e);
    public delegate void DataChangeEventHandler(object sender, DataChangeEventArgs e);
    public delegate void ReadCompleteEventHandler(object sender, DataChangeEventArgs e);
    public delegate void WriteCompleteEventHandler(object sender, WriteCompleteEventArgs e);
}
