using System;

namespace Common.IO.Transport.Rmi
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class CallAttribute : Attribute
    {
    }
}