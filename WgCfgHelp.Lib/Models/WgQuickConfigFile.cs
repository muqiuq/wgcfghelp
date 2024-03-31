using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCfgHelp.Lib.Models
{
    public class WgQuickConfigFile
    {

        public string? Address { get; set; }

        public string? DNS { get; set; }

        public string? PrivateKey { get; set; }

        public string? PublicKey { get; set; }

        public int? ListenPort { get; set; }

        public List<WgQuickPeer> Peers { get; set; } = new List<WgQuickPeer>();

        public string ToConfigFileFormat()
        {
            string output = "[Interface]\n";
            output += $"Address = {Address}\n";
            output += $"PrivateKey = {PrivateKey}\n";
            output += $"# PublicKey = {PublicKey}\n";
            if(!string.IsNullOrWhiteSpace(DNS)) output += $"DNS = {DNS}\n";
            if (ListenPort.HasValue) output += $"ListenPort = {ListenPort}\n";
            
            foreach (var peer in Peers)
            {
                output += "\n[Peer]\n";
                output += peer.ToConfigFileFormat();
            }

            return output;

        }

    }
}
