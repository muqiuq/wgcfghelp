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

        public int? Mtu { get; set; }
        
        public string? PreUp { get; set; }
        
        public string? PostUp { get; set; }
        
        public string? PreDown { get; set; }
        
        public string? PostDown { get; set; }
        
        public List<WgQuickPeer> Peers { get; set; } = new List<WgQuickPeer>();

        public string ToConfigFileFormat()
        {
            string output = "[Interface]\n";
            output += $"Address = {Address}\n";
            output += $"PrivateKey = {PrivateKey}\n";
            output += $"# PublicKey = {PublicKey}\n";
            if(!string.IsNullOrWhiteSpace(DNS)) output += $"DNS = {DNS}\n";
            if(Mtu is not null) output += $"MTU = {Mtu}\n";
            if(!string.IsNullOrWhiteSpace(PreUp)) output += $"PreUp = {PreUp}\n";
            if(!string.IsNullOrWhiteSpace(PreDown)) output += $"PreDown = {PreDown}\n";
            if(!string.IsNullOrWhiteSpace(PostUp)) output += $"PostUp = {PostUp}\n";
            if(!string.IsNullOrWhiteSpace(PostDown)) output += $"PostDown = {PostDown}\n";
            if (ListenPort.HasValue) output += $"ListenPort = {ListenPort}\n";
            
            foreach (var peer in Peers)
            {
                output += "\n[Peer]\n";
                output += peer.ToConfigFileFormat();
            }

            return output;
        }

        public string ToMikroTikFormat()
        {
            string output = $"/interface/wireguard/add private-key=\"{PrivateKey}\"";
            if (ListenPort.HasValue) output += $" listen-port=\"{ListenPort}\"";
            output += "\n";
            string interface_retrival_cmd = $"[/interface/wireguard/find where private-key=\"{PrivateKey}\"]";
            output += $"/ip/address add interface={interface_retrival_cmd} address={Address}\n";
            foreach (var peer in Peers)
            {
                output += peer.ToMikroTikFormat(interface_retrival_cmd) + "\n";
            }
            return output;
        }

    }
}
