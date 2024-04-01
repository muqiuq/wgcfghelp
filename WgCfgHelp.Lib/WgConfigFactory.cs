using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgCfgHelp.Lib.Models;

namespace WgCfgHelp.Lib
{
    public class WgConfigFactory
    {

        
        public static WgQuickConfigFile GenServerConfig(
            string address,
            int port,
            string privateKey,
            string? preUp,
            string? postUp,
            string? preDown,
            string? postDown,
            int? mtu,
            List<WgQuickPeer> peers)
        {
            var wgExeInt = WgExeInterface.Create();
            var wgConfig = new WgQuickConfigFile()
            {
                Address = address,
                PrivateKey = privateKey,
                ListenPort = port,
                PreUp = preUp,
                PostUp = postUp,
                PreDown = preDown,
                PostDown = postDown,
                Mtu = mtu,
            };
            wgConfig.PublicKey = wgExeInt.GenPublicKey(wgConfig.PrivateKey);
            wgConfig.Peers = peers;
            return wgConfig;
        }
        
        public static WgQuickConfigFile BuildNewClientConfig(
            string address,
            string publicKey, 
            string endpoint, 
            string allowedIps,
            string? dns,
            bool presharedKey = false
            )
        {
            var wgExeInt = WgExeInterface.Create();
            var wgConfig = new WgQuickConfigFile()
            {
                Address = address,
                DNS = dns,
                PrivateKey = wgExeInt.GenPrivateKey()
            };
            wgConfig.PublicKey = wgExeInt.GenPublicKey(wgConfig.PrivateKey);
            var wgQuickPeer = new WgQuickPeer()
            {
                AllowedIPs = allowedIps,
                Endpoint = endpoint,
                PublicKey = publicKey
            };
            if (presharedKey)
            {
                wgQuickPeer.PresharedKey = wgExeInt.GenPresharedKey();
            }
            wgConfig.Peers.Add(wgQuickPeer);
            return wgConfig;
        }
        

        public static WgKeyPair GenKeyPair()
        {
            var wgExeInt = WgExeInterface.Create();
            var privateKey = wgExeInt.GenPrivateKey();
            var publicKey = wgExeInt.GenPublicKey(privateKey);
            return new WgKeyPair()
            {
                PrivateKey = privateKey,
                PublicKey = publicKey
            };

        }


    }
}
