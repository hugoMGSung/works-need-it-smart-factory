using System;
using System.Runtime.InteropServices;

namespace DataService
{
    // PLC 아날로그 모듈 스케일범위 계산용 구조체
    // PVLAST = ((EUHi-EULo)*PVRAW/4000.0) + EULo
    [StructLayout(LayoutKind.Sequential)]
    public struct Scaling : IComparable<Scaling>
    {
        public short ID;

        public ScaleType ScaleType;

        public float EUHi;

        public float EULo;

        public float RawHi;

        public float RawLo;

        public Scaling(short id, ScaleType type, float euHi, float euLo, float rawHi, float rawLo)
        {
            ID = id;
            ScaleType = type;
            EUHi = euHi;
            EULo = euLo;
            RawHi = rawHi;
            RawLo = rawLo;
        }

        public int CompareTo(Scaling other)
        {
            return ID.CompareTo(other.ID);
        }

        public static readonly Scaling Empty = new Scaling { ScaleType = ScaleType.None };
    }

    /// <summary>
    /// 아이템 데이터 구조체
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ItemData<T>
    {
        public T Value;
        public long TimeStamp;
        public QUALITIES Quality;

        public ItemData(T value, long timeStamp, QUALITIES quality)
        {
            Value = value;
            TimeStamp = timeStamp;
            Quality = quality;
        }
    }

    public struct FileData : IComparable<FileData>
    {
        public short ID;
        public Storage Value;
        public string Text;

        public FileData(short id, Storage value, string text)
        {
            ID = id;
            Value = value;
            Text = text;
        }

        public int CompareTo(FileData other)
        {
            return this.ID.CompareTo(other.ID);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    /// <summary>
    /// 연산자 구조체
    /// </summary>
    public struct Operator
    {
        public char OperatorStack;
        public byte Level;
        public Operator(char OperatorStack, byte Level)
        {
            this.OperatorStack = OperatorStack;
            this.Level = Level;
        }
    }

    public struct PDUArea
    {
        public DeviceAddress Start;
        public int Len;
        public int StartIndex;
        public int Count;

        public PDUArea(DeviceAddress start, int len, int startIndex, int count)
        {
            Start = start;
            Len = len;
            StartIndex = startIndex;
            Count = count;
        }
    }
}
