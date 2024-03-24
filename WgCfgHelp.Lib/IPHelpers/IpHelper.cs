using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WgCfgHelp.Lib.IPHelpers
{
    public static class IpHelper
    {

        public static bool IsPartOfNetwork(IPAddress ipAddr, List<string> listOfSubnets, out IPNetwork2 network)
        {
            network = null;
            foreach (var subnetStr in listOfSubnets)
            {
                if (!IPNetwork2.TryParse(subnetStr, out var subnet))
                {
                    return false;
                }

                if (subnet.Contains(ipAddr))
                {
                    network = subnet;
                    return true;
                }
            }

            return false;
        }


        public static IPAddress GetNextIpAddress(IPAddress ipAddress, uint increment)
        {
            byte[] addressBytes = ipAddress.GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            var nextAddress = BitConverter.GetBytes(ipAsUint + increment);
            return new IPAddress(nextAddress.Reverse().ToArray());
        }

    }
}
