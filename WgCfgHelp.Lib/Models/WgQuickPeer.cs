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
    }
}
