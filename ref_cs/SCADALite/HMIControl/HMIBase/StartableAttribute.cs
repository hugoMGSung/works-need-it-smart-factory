using System;

namespace HMIControl
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Serializable]
    public class StartableAttribute : Attribute
    {
    }
}
