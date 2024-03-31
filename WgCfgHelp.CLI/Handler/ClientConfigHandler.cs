using QRCoder;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats;
using WgCfgHelp.CLI.Models;
using WgCfgHelp.Lib;
using WgCfgHelp.Lib.IPHelpers;
using System.Net;

namespace WgCfgHelp.CLI.Handler
{
    public class ClientConfigArgs
    {
        public string Config { get; set; }

        public string ConfigFileName
        {
            get
            {
                return Config;
            }
        }

        public string Address { get; set; }

        public bool Preshared { get; set; }
        public bool ToFile { get; set; }
        public int NumOfClients { get; set; }
        public bool QrCode { get; set; }
        public bool Force { get; set; }
        public bool DoNotUpdateSiteConfig { get; set; }
        public string BasePath { get; set; }
        
    }
    
    internal class ClientConfigHandler : IHandler
    {
        public Command GetCommand()
        {
            var configArg = new Argument<string>("config", "site config file");
            var addressArg = new Argument<string>("address", "address for the client");

            var presharedKeyOption = new Option<bool>(new string[]{"-p", "--preshared" }, "generate preshared key");
            var outputToFileOption = new Option<bool>(new string[] { "-o", "--to-file" }, "output to file instead of std");
            var numOfClients =
                new Option<int>(new string[] { "-n" , "--num-of-clients"}, () => 1, "number of client access files to generate");
            var qrCodeOption = new Option<bool>(new string[] { "-q", "--qrcode" }, "generate qrcode");
            var forceOption = new Option<bool>(new string[] { "-f", "--Force" }, "force writing files");
            var doNotUpdateSiteConfigOption = new Option<bool>(new string[] { "-d", "--do-not-update-site-config" }, () => false, "do not update site config");
            var basePathOption =
                new Option<string>(new string[] { "-b", "--base-path" }, "write all files in this path");

            var command = new Command("gen-client", "generate client access file");
            command.AddArgument(configArg);
            command.AddArgument(addressArg);
            command.AddOption(numOfClients);
            command.AddOption(presharedKeyOption);
            command.AddOption(qrCodeOption);
            command.AddOption(basePathOption);
            command.AddOption(outputToFileOption);
            command.AddOption(forceOption);
            command.AddOption(doNotUpdateSiteConfigOption);

            command.Handler = CommandHandler.Create(
                async (ClientConfigArgs args) =>
                {
                    await handle(args);
                });

            return command;

        }
        
        

        private async Task<int> handle(ClientConfigArgs args)
        {
            if (!File.Exists(args.ConfigFileName))
            {
                Console.WriteLine("config file not found");
                return CliErrorCodes.CONFIG_FILE_NOT_FOUND;
            }
            var configFile = SiteConfigFile.LoadFromFile(args.ConfigFileName);

            if (!HandlerHelper.TryVerifyIpAddress(args.Address, configFile.AllowedIPs, args.NumOfClients,
                    out var errorCode, out var ipAddr, out var lastIpAddr, out var network))
            {
                return errorCode;
            }

            for (int a = 0; a < args.NumOfClients; a++)
            {
                var ipAddrForClient = IpHelper.GetNextIpAddress(ipAddr!, (uint)a);
                var errorCodeSub = await GenerateClientAccessFile(configFile, ipAddrForClient.ToString(), 
                    args.Preshared, args.ToFile, args.Force, args.QrCode, args.BasePath,
                    network!);
                if (errorCodeSub != CliErrorCodes.SUCCESS)
                {
                    return errorCodeSub;
                }
            }

            if (!args.DoNotUpdateSiteConfig)
            {
                configFile.SaveToFile(args.ConfigFileName);
            }

            return CliErrorCodes.SUCCESS;
        }

        private static async Task<int> GenerateClientAccessFile(SiteConfigFile configFile,
            string address,
            bool presharedKey,
            bool outputToFile,
            bool force,
            bool genQrCode,
            string basePath,
            IPNetwork2 network)
        {
            var clientAddr = $"{address}/{network.Cidr}";

            var clientConfig = WgConfigFactory.GenClientConfig(clientAddr,
                configFile.PublicKey!,
                configFile.Endpoint!,
                configFile.AllowedIPs!,
                configFile.Dns,
            presharedKey);
            
            configFile.Peers.Add(new PeerConfig()
            {
                AllowedIPs = clientConfig.Peers.First()?.AllowedIPs,
                Name = clientAddr,
                Address = clientAddr,
                PresharedKey = clientConfig.Peers.First()?.PresharedKey,
                PublicKey = clientConfig.PublicKey,
            });

            if (!HandlerHelper.TrySaveToFileOrOutput(
                    address, basePath, outputToFile, force, clientConfig.ToConfigFileFormat(), 
                    out var errorCode, out var filePathWithoutExtension))
            {
                return errorCode;
            }

            var filenamePic = $"{filePathWithoutExtension!}.png";
            
            if (genQrCode)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(clientConfig.ToConfigFileFormat(), QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var image = qrCode.GetGraphic(20);
                if (File.Exists(filenamePic) && !force)
                {
                    Console.WriteLine($"file {filenamePic} already exists");
                    return CliErrorCodes.FILE_ALREADY_EXISTS;
                }

                await image.SaveAsync(filenamePic);
            }
            
            return CliErrorCodes.SUCCESS;
        }
    }
}
