using System;

namespace DataService
{
    public class DriverArgumet
    {
        public short DriverID;
        public string PropertyName;
        public string PropertyValue;

        public DriverArgumet(short id, string name, string value)
        {
            DriverID = id;
            PropertyName = name;
            PropertyValue = value;
        }
    }

    public class TagMetaData : IComparable<TagMetaData>
    {
        public short ID { set; get; }
        public string Name { set; get; }
        public byte DataTypeNum
        {
            set { DataType = (DataType)value; }
            get { return (byte)DataType; }
        }
        public DataType DataType { set; get; }
        public ushort Size { set; get; }
        public string Address { set; get; }
        public short GroupID { set; get; }
        public bool Active { set; get; }
        public string Description { set; get; }
        public float Maximum { set; get; }
        public float Minimum { set; get; }
        public int Cycle { set; get; }

        public TagMetaData() { }

        public TagMetaData(short id, short grpId, string name, string address,
            DataType type, ushort size, bool archive = false, float max = 0,
            float min = 0, int cycle = 0)
        {
            ID = id;
            GroupID = grpId;
            Name = name;
            Address = address;
            DataType = type;
            Size = size;
            Active = archive;
            Maximum = max;
            Minimum = min;
            Cycle = cycle;
        }
        public int CompareTo(TagMetaData other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class GroupMeta
    {
        public string Name { set; get; }
        public short ID { set; get; }
        public short DriverID { set; get; }
        public int UpdateRate { set; get; }
        public float DeadBand { set; get; }
        public bool Active { set; get; }
    }

    public class DriverMetaData
    {
        public short ID { set; get; }
        public int DriverID { set; get; }
        public string Name { set; get; }
        public string Assembly { set; get; }
        public string ClassName { set; get; }
        public object Target { get; set; }
    }

    public class RegisterModule
    {
        public int DriverID { get; set; }
        public string AssemblyName { get; set; }
        public string Description { get; set; }
        public string ClassName { get; set; }
        public string ClassFullName { get; set; }
    }
}
