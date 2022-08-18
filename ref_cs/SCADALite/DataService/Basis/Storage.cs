using System;
using System.Runtime.InteropServices;

namespace DataService
{
    // C#(관리 메모리)와 C++(비관리 메모리)의 원활한 통신(마샬링,marshal)을 위해 사용되는 코드
    // 마샬링: 한 객체의 메모리에서의 표현방식 저장,또한 전송에 적합한 다른 형식으로 변환과정
    // 데이터를 서로 다른 프로그램간에 전달할 필요가 있을 경우 사용. 직렬화와 유사
    // 멀리 떨어진 객체와 통신하기 위해서 사용함
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct Storage
    {
        // C/C++에서 공용 구조체로 선언
        [FieldOffset(0)]
        public bool Boolean;
        [FieldOffset(0)]
        public byte Byte;
        [FieldOffset(0)]
        public short Int16;
        [FieldOffset(0)]
        public int Int32;
        [FieldOffset(0)]
        public float Single;
        [FieldOffset(0)]
        public ushort Word;
        [FieldOffset(0)]
        public uint DWord;

        public static readonly Storage Empty;

        static Storage()
        {
            Empty = new Storage();
        }

        /// <summary>
        /// Equals 오버라이드 메서드
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Type type = obj.GetType();
            if (type == typeof(Storage))
                return this.Int32 == ((Storage)obj).Int32;
            else
            {
                if (type == typeof(int))
                    return this.Int32 == (int)obj;
                if (type == typeof(short))
                    return this.Int16 == (short)obj;
                if (type == typeof(byte))
                    return this.Byte == (byte)obj;
                if (type == typeof(bool))
                    return this.Boolean == (bool)obj;
                if (type == typeof(float))
                    return this.Single == (float)obj;
                if (type == typeof(ushort))
                    return this.Word == (ushort)obj;
                if (type == typeof(uint))
                    return this.DWord == (uint)obj;
                if (type == typeof(string))
                    return this.ToString() == obj.ToString();
            }
            return false;
        }

        /// <summary>
        /// Int32 해시코드 생성 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Int32.GetHashCode();
        }

        public static bool operator ==(Storage x, Storage y)
        {
            return x.Int32 == y.Int32;
        }

        public static bool operator !=(Storage x, Storage y)
        {
            return x.Int32 != y.Int32;
        }
    }
}
