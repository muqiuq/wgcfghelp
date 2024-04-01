using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCfgHelp.Lib.Models
{
    public class WgQuickPeer
    {

        public string? PublicKey { get; set; }

        public string? PresharedKey { get; set; }

        public string? AllowedIPs { get; set; }

        public string? Endpoint { get; set; }
        
        public string? Comment { get; set; }

        public string ToConfigFileFormat()
        {
            string output = "";
            if (!string.IsNullOrWhiteSpace(Comment)) output += $"# {Comment.Trim()}\n";
            output += $"PublicKey = {PublicKey}\n";
            output += $"AllowedIPs = {AllowedIPs}\n";
            if(!string.IsNullOrWhiteSpace(PresharedKey)) output += $"PresharedKey = {PresharedKey}\n";
            if(!string.IsNullOrWhiteSpace(Endpoint)) output += $"Endpoint = {Endpoint}\n";
            return output;
        }

        public string ToMikroTikFormat(string mikrotikInterfaceName)
        {
            string output = "/interface/wireguard/peers/add";
            output += $" interface={mikrotikInterfaceName}" +
                      $" public-key=\"{PublicKey}\" allowed-address=\"{AllowedIPs}\"";
            if(!string.IsNullOrWhiteSpace(PresharedKey)) output += $" preshared-key=\"{PresharedKey}\"";
            if(!string.IsNullOrWhiteSpace(Comment)) output += $" comment=\"{Comment.Replace("\"", " ")}\"";
            if (!string.IsNullOrWhiteSpace(Endpoint))
            {
                var endpointParts = Endpoint.Split(":");
                output += $" endpoint-address=\"{endpointParts[0]}\"";
                if (endpointParts.Length == 2)
                {
                    output += $" endpoint-port=\"{endpointParts[1]}\"";
                }
            }

            return output;
        }
    }
}
