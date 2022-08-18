using System;

namespace DataService
{
    // 사용자 정의 객체를 정렬 또는 비교하는 기능을 제공하려는 경우
    // IComparable 인터페이스 상속받은 후 기본으로 비교하려는 값으로 CompareTo메소드를 정의
    public struct DeviceAddress : IComparable<DeviceAddress>
    {
        public int Area;
        public int Start;
        public ushort DBNumber;
        public ushort DataSize;
        public ushort CacheIndex;
        public byte Bit;
        public DataType VarType;
        public ByteOrder ByteOrder;

        // Contructor
        public DeviceAddress(int area, ushort dbnumber, ushort cIndex, int start, 
                             ushort size, byte bit, DataType type, 
                             ByteOrder order = ByteOrder.None)
        {
            Area = area;
            DBNumber = dbnumber;
            CacheIndex = cIndex;
            Start = start;
            DataSize = size;
            Bit = bit;
            VarType = type;
            ByteOrder = order;
        }

        /// <summary>
        /// 빈값 인스턴스
        /// </summary>
        public static readonly DeviceAddress Empty = new DeviceAddress(0, 0, 0, 0, 0, 0, DataType.NONE);

        /// <summary>
        /// IComparable를 구현했을 때 필수 구현 메서드
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(DeviceAddress other)
        {
            return this.Area > other.Area ? 1 :
                this.Area < other.Area ? -1 :
                this.DBNumber > other.DBNumber ? 1 :
                this.DBNumber < other.DBNumber ? -1 :
                this.Start > other.Start ? 1 :
                this.Start < other.Start ? -1 :
                this.Bit > other.Bit ? 1 :
                this.Bit < other.Bit ? -1 : 0;
        }
    }
}
