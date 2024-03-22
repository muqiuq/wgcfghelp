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
            var qrCodeOption = new Option<bool>(new string[] { "-q", "--qrcode" }, "generate qrcode");
            var forceOption = new Option<bool>("-f", "force writing files");
            var basePathOption =
                new Option<string>(new string[] { "-b", "--basepath" }, "write all files in this path");

            var command = new Command("gen-client", "generate client access file");
            command.AddArgument(configArg);
            command.AddArgument(addressArg);
            command.AddOption(presharedKeyOption);
            command.AddOption(qrCodeOption);
            command.AddOption(basePathOption);
            command.AddOption(outputToFileOption);
            command.AddOption(forceOption);

            command.SetHandler(async (config, address, presharedKey, outputToFile, force, qrCode, basePath) =>
            {
                await handle(config, address, presharedKey, outputToFile, force, qrCode, basePath);
            }, configArg, addressArg, presharedKeyOption, outputToFileOption, forceOption, qrCodeOption, basePathOption);

            return command;

        }

        private async Task handle(string config, string address, bool presharedKey, bool outputToFile, bool force, bool genQrCode, string basePath)
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
