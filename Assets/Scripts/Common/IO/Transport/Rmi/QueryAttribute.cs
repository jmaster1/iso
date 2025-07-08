using System;

namespace IsoNet.Core.Transport.Rmi
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class QueryAttribute : Attribute
    {
    }
}