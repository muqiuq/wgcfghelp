using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using QRCoder;
using SixLabors.ImageSharp;
using WgCfgHelp.CLI.Models;
using WgCfgHelp.Lib;
using WgCfgHelp.Lib.IPHelpers;
using WgCfgHelp.Lib.Models;

namespace WgCfgHelp.CLI.Handler
{
    public class ServerConfigArgs
    {
        public bool ToFile { get; set; }
        public bool Force { get; set; }
        public string BasePath { get; set; }
        public string Config { get; set; }

        public string Address { get; set; }
        
        public string ConfigFileName
        {
            get
            {
                return Config;
            }
        }
    }
    
    internal class ServerConfigHandler : IHandler
    {
        public Command GetCommand()
        {
            var configArg = new Argument<string>("config", "site config file");
            var addressArg = new Argument<string>("address", "address for the client");

            var outputToFileOption =
                new Option<bool>(new string[] { "-o", "--to-file" }, "output to file instead of std");
            var forceOption = new Option<bool>(new string[] { "-f", "--Force" }, "force writing files");
            var basePathOption =
                new Option<string>(new string[] { "-b", "--base-path" }, "write all files in this path");

            var command = new Command("gen-server", "generate client access file");
            command.AddArgument(configArg);
            command.AddArgument(addressArg);
            command.AddOption(basePathOption);
            command.AddOption(outputToFileOption);
            command.AddOption(forceOption);

            command.Handler = CommandHandler.Create(
                (ServerConfigArgs args) => handle(args));

            return command;
        }

        private int handle(ServerConfigArgs args)
        {
            if (!File.Exists(args.ConfigFileName))
            {
                Console.WriteLine("config file not found");
                return CliErrorCodes.CONFIG_FILE_NOT_FOUND;
            }

            var configFile = SiteConfigFile.LoadFromFile(args.ConfigFileName);

            if (!HandlerHelper.TryVerifyIpAddress(args.Address, configFile.AllowedIPs, 0,
                    out var errorCode, out var ipAddr, out var lastIpAddr, out var network))
            {
                return errorCode;
            }

            if (configFile.Port == null || !configFile.Port.HasValue || configFile.Port < 1 || configFile.Port > 65535)
            {
                Console.WriteLine($"Invalid ListenPort ({configFile.Port})");
                return CliErrorCodes.INVALID_LISTEN_PORT;
            }

            if (string.IsNullOrWhiteSpace(configFile.PrivateKey))
            {
                Console.WriteLine($"Missing privateKey in config file");
                return CliErrorCodes.MISSING_PRIVATE_KEY;
            }
            
            return GenerateServerAccessFile(configFile, $"{ipAddr}/{network!.Cidr}", args, network!);
        }

        private static int GenerateServerAccessFile(SiteConfigFile configFile,
            string address,
            ServerConfigArgs args,
            IPNetwork2 network)
        {
            var wgQuickPeers = configFile.Peers.Select(x => new WgQuickPeer()
            {
                AllowedIPs = IpHelper.ReplaceOrAddCidr(x.Address!, 32),
                PresharedKey = x.PresharedKey,
                PublicKey = x.PublicKey,
                Comment = x.Name
            }).ToList();

            var serverConfig = WgConfigFactory.GenServerConfig(address,
                configFile.Port!.Value,
                configFile.PrivateKey!,
                wgQuickPeers
            );
            
            if (!HandlerHelper.TrySaveToFileOrOutput(
                    address, args.BasePath, args.ToFile, args.Force, serverConfig.ToConfigFileFormat(), 
                    out var errorCode, out var filePathWithoutExtension))
            {
                return errorCode;
            }

            return CliErrorCodes.SUCCESS;
        }
    }
}