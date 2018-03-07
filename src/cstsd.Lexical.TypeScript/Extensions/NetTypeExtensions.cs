using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cstsd.Core.Net;

namespace cstsd.TypeScript.Extensions
{
    public static class NetTypeExtensions
    {
        public static NetType UnwrapTaskType(this NetType netType)
        {
            if (netType.Name == "Task")
            {
                if (netType.GenericParameters.Count > 0)
                {
                    //Task<T> will only have one ga
                    var gp = netType.GenericParameters.First();

                    return new NetType
                    {
                        Name = gp.Name,
                        GenericParameters = gp.NetGenericParameters.ToArray(),
                        IsPublic = netType.IsPublic
                    };
                }

                //"Task" with no ga's is a void
                return new NetType
                {
                    Name = "void"
                };
            }

            return netType;
        }

    }
}
