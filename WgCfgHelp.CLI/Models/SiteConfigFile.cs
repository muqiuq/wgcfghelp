using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgCfgHelp.Lib.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WgCfgHelp.CLI.Models
{
    public class SiteConfigFile
    {

        public string? PrivateKey { get; set; }

        public string? PublicKey { get; set; }

        public string? Dns { get; set; }

        public string? Endpoint { get; set; }

        public int? Port { get; set; } = 51820;

        public string? AllowedIPs { get; set; }

        public List<PeerConfig> Peers { get; set; } = new List<PeerConfig>();

        public static SiteConfigFile LoadFromFile(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            return deserializer.Deserialize<SiteConfigFile>(File.ReadAllText(path));
        }

        public void SaveToFile(string path)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            File.WriteAllText(path, serializer.Serialize(this));
        }

    }
}
