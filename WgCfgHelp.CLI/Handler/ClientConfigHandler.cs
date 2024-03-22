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
    internal class ClientConfigHandler : IHandler
    {
        public Command GetCommand()
        {
            var configArg = new Argument<string>("config", "site config file");
            var addressArg = new Argument<string>("address", "address for the client");

            var presharedKeyOption = new Option<bool>(new string[]{"-p", "--preshared" }, "generate preshared key");
            var outputToFileOption = new Option<string>(new string[] { "-o", "--output" }, "output to file instead of std");
            var forceOption = new Option<bool>("-f", "force writing files");

            var command = new Command("gen-client", "generate client access file");
            command.AddArgument(configArg);
            command.AddArgument(addressArg);
            command.AddOption(presharedKeyOption);
            command.AddOption(outputToFileOption);
            command.AddOption(forceOption);

            command.SetHandler(async (config, address, presharedKey, outputToFile, force) =>
            {
                await handle(config, address, presharedKey, outputToFile, force);
            }, configArg, addressArg, presharedKeyOption, outputToFileOption, forceOption);

            return command;

        }

        private async Task handle(string config, string address, bool presharedKey, string outputToFile, bool force)
        {
            if (!File.Exists(config))
            {
                Console.WriteLine("config file not found");
                return;
            }
            var configFile = SiteConfigFile.LoadFromFile(config);

            var clientConfig = WgConfigFactory.GenClientConfig(address,
                configFile.PublicKey!,
                configFile.Endpoint!,
                configFile.AllowedIPs!,
                configFile.Dns,
                presharedKey);

            if (!string.IsNullOrWhiteSpace(outputToFile))
            {
                if (File.Exists(outputToFile) && !force)
                {
                    Console.WriteLine($"file {outputToFile} already exists");
                    return;
                }
                await File.WriteAllTextAsync(outputToFile, clientConfig.ToConfigFileFormat());
            }
            else
            {
                Console.WriteLine(clientConfig.ToConfigFileFormat());
            }
        }
    }
}
