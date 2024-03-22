using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WgCfgHelp.CLI.Models
{
    public class SiteConfigFile
    {

        public string? PublicKey { get; set; }

        public string? Dns { get; set; }

        public string? Endpoint { get; set; }

        public string? AllowedIPs { get; set; }

        public static SiteConfigFile LoadFromFile(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            return deserializer.Deserialize<SiteConfigFile>(File.ReadAllText(path));
        }

    }
}
