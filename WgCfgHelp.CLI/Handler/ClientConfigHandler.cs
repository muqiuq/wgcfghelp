using QRCoder;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
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
    internal class ClientConfigHandler : IHandler
    {
        public Command GetCommand()
        {
            var configArg = new Argument<string>("config", "site config file");
            var addressArg = new Argument<string>("address", "address for the client");

            var presharedKeyOption = new Option<bool>(new string[]{"-p", "--preshared" }, "generate preshared key");
            var outputToFileOption = new Option<bool>(new string[] { "-o", "--to-file" }, "output to file instead of std");
            var numOfClients =
                new Option<int>(new string[] { "-n" }, () => 1, "number of client access files to generate");
            var qrCodeOption = new Option<bool>(new string[] { "-q", "--qrcode" }, "generate qrcode");
            var forceOption = new Option<bool>("-f", "force writing files");
            var basePathOption =
                new Option<string>(new string[] { "-b", "--basepath" }, "write all files in this path");

            var command = new Command("gen-client", "generate client access file");
            command.AddArgument(configArg);
            command.AddArgument(addressArg);
            command.AddOption(numOfClients);
            command.AddOption(presharedKeyOption);
            command.AddOption(qrCodeOption);
            command.AddOption(basePathOption);
            command.AddOption(outputToFileOption);
            command.AddOption(forceOption);

            command.SetHandler(async (config, address, presharedKey, outputToFile, force, qrCode, basePath, numOfClients) =>
            {
                await handle(config, address, presharedKey, outputToFile, force, qrCode, basePath, numOfClients);
            }, configArg, addressArg, presharedKeyOption, outputToFileOption, forceOption, qrCodeOption, basePathOption, numOfClients);

            return command;

        }

        private async Task handle(string config, 
            string addressStr, 
            bool presharedKey, 
            bool outputToFile, 
            bool force, 
            bool genQrCode, 
            string basePath, 
            int numOfClients)
        {
            if (!File.Exists(config))
            {
                Console.WriteLine("config file not found");
                return;
            }
            var configFile = SiteConfigFile.LoadFromFile(config);

            if (configFile.AllowedIPs == null)
            {
                Console.WriteLine("Missing required parameter 'allowedIPs' in site config");
                return;
            }

            var allowedIpsList = configFile.AllowedIPs!.Split(",").Select(x => x.Trim()).ToList();

            if (!IPAddress.TryParse(addressStr, out var ipAddr))
            {
                Console.WriteLine($"Could not parse IP Address {addressStr}");
                return;
            }

            if (!IpHelper.IsPartOfNetwork(ipAddr, allowedIpsList, out var network))
            {
                Console.WriteLine($"{addressStr} is not part of any network in {configFile.AllowedIPs}");
                return;
            }

            var lastIpAddr = IpHelper.GetNextIpAddress(ipAddr, (uint)numOfClients-1);

            if (network.Netmask.Equals(ipAddr) || network.Broadcast.Equals(ipAddr))
            {
                Console.WriteLine($"Start address {ipAddr} cannot equal network or broadcast address.");
                return;
            }

            if (!network.Contains(lastIpAddr))
            {
                Console.WriteLine($"Last ip address ({lastIpAddr}) is outside of network {network}");
                return;
            }

            if (network.Netmask.Equals(lastIpAddr) || network.Broadcast.Equals(lastIpAddr))
            {
                Console.WriteLine($"Last address {lastIpAddr} cannot equal network or broadcast address.");
                return;
            }

            for (int a = 0; a < numOfClients; a++)
            {
                var ipAddrForClient = IpHelper.GetNextIpAddress(ipAddr, (uint)a);
                await GenerateClientAccessFile(configFile, ipAddrForClient.ToString(), presharedKey, outputToFile, force, genQrCode, basePath,
                    network);
            }

        }

        private static async Task GenerateClientAccessFile(SiteConfigFile configFile,
            string address,
            bool presharedKey,
            bool outputToFile,
            bool force,
            bool genQrCode,
            string basePath,
            IPNetwork2 network)
        {

            var clientConfig = WgConfigFactory.GenClientConfig($"{address}/{network.Cidr}",
                configFile.PublicKey!,
                configFile.Endpoint!,
                configFile.AllowedIPs!,
                configFile.Dns,
            presharedKey);

            var cleanAddress = address.Replace("/", "_");

            if (!string.IsNullOrWhiteSpace(basePath))
            {
                if (!Directory.Exists(basePath))
                {
                    Console.WriteLine("Output folder does not exist");
                    return;
                }
            }
            else
            {
                basePath = "";
            }

            var filenameConf = Path.Combine(basePath, $"{cleanAddress}.conf");
            var filenamePic = Path.Combine(basePath, $"{cleanAddress}.png");

            if (outputToFile)
            {
                if (File.Exists(filenameConf) && !force)
                {
                    Console.WriteLine($"file {filenameConf} already exists");
                    return;
                }
                await File.WriteAllTextAsync(filenameConf, clientConfig.ToConfigFileFormat());
            }
            else
            {
                Console.WriteLine(clientConfig.ToConfigFileFormat());
            }

            if (genQrCode)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(clientConfig.ToConfigFileFormat(), QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var image = qrCode.GetGraphic(20);
                if (File.Exists(filenamePic) && !force)
                {
                    Console.WriteLine($"file {filenamePic} already exists");
                    return;
                }

                await image.SaveAsync(filenamePic);
            }
        }
    }
}
