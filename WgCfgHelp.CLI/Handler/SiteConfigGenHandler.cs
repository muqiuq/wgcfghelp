using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgCfgHelp.CLI.Models;
using WgCfgHelp.Lib;

namespace WgCfgHelp.CLI.Handler
{
    public class SiteConfigGenHandler : IHandler
    {
        public Command GetCommand()
        {
            var configArg = new Argument<string>("name", "name for site config (will become filename with ending yaml)");
            var allowedIpsArg = new Argument<string>("allowedIps", "allowed ip networks (comma separated, format: 0.0.0.0/0)");
            var endpointArg = new Argument<string>("endpoint", "Endpoint address (format: host:port)");

            var dnsOption = new Option<string>(new string[] { "-d", "--dns" }, "dns server to use");
            var forceOption = new Option<bool>("-f", "force writing files");

            var command = new Command("gen-site", "generate site config");
            command.AddArgument(configArg);
            command.AddArgument(allowedIpsArg);
            command.AddArgument(endpointArg);
            command.AddOption(dnsOption);
            command.AddOption(forceOption);
            

            command.SetHandler(handle, configArg, allowedIpsArg, endpointArg, dnsOption, forceOption);

            return command;

        }

        private void handle(string configFilePath, string allowedIpsStr, string endpoint, string dns,
            bool forceOption)
        {
            var keyPair = WgConfigFactory.GenKeyPair();
            var port = 51820;
            var endpointParts = endpoint.Split(":");
            if (endpointParts.Length == 2 && int.TryParse(endpointParts.Last(), out var parsedPort))
            {
                port = parsedPort;
            }
            else if(endpointParts.Length == 1)
            {
                endpoint = $"{endpoint}:{port}";
            }
            var siteConfig = new SiteConfigFile()
            {
                Endpoint = endpoint,
                AllowedIPs = allowedIpsStr,
                Dns = dns,
                PublicKey = keyPair.PublicKey,
                PrivateKey = keyPair.PrivateKey,
                Port = port
            };
            configFilePath = configFilePath + ".yaml";
            if (File.Exists(configFilePath) && !forceOption)
            {
                Console.WriteLine($"config file {configFilePath} already exists. skipping");
                return;
            }
            siteConfig.SaveToFile(configFilePath);
        }
    }
}
