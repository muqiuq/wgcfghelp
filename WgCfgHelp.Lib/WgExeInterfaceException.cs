using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WgCfgHelp.Lib
{
    public class WgExeInterfaceException : Exception
    {
        public WgExeInterfaceException()
        {
        }

        public WgExeInterfaceException(string? message) : base(message)
        {
        }

        public WgExeInterfaceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
