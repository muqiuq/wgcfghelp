using System.Net;
using WgCfgHelp.Lib.IPHelpers;

namespace WgCfgHelp.CLI.Handler;

public static class HandlerHelper
{

    public static bool TrySaveToFileOrOutput(string fileName, 
        string basePath, bool outputToFile, bool force, string content,
        out int errorCode,
        out string? filePathWithoutExtension)
    {
        var cleanFileName = fileName.Replace("/", "_");
        
        filePathWithoutExtension = null;
        errorCode = CliErrorCodes.SUCCESS;
        
        if (!string.IsNullOrWhiteSpace(basePath))
        {
            if (!Directory.Exists(basePath))
            {
                Console.WriteLine("Output folder does not exist");
                errorCode = CliErrorCodes.OUTPUT_FOLDER_DOES_NOT_EXIST;
                return false;
            }
        }
        else
        {
            basePath = "";
        }
        filePathWithoutExtension = Path.Combine(basePath, $"{cleanFileName}");

        var filenameConf = $"{filePathWithoutExtension}.conf";

        if (outputToFile)
        {
            if (File.Exists(filenameConf) && !force)
            {
                Console.WriteLine($"file {filenameConf} already exists");
                errorCode = CliErrorCodes.FILE_ALREADY_EXISTS;
                return false;
            }
            File.WriteAllText(filenameConf, content);
        }
        else
        {
            Console.WriteLine(content);
        }

        return true;
    }
    
    public static bool TryVerifyIpAddress(
        string ipAddrStr, 
        string? allowedIpsStr, 
        int numOfClients, 
        out int errorCode,
        out IPAddress? firstIpAddr,
        out IPAddress? lastIpAddr,
        out IPNetwork2? network
        )
    {
        network = null;
        firstIpAddr = null;
        lastIpAddr = null;
        errorCode = CliErrorCodes.SUCCESS;
        
        if (allowedIpsStr == null)
        {
            Console.WriteLine("Missing required parameter 'allowedIPs' in site config");
            errorCode =  CliErrorCodes.MISSING_PARAMETER_ALLOWED_IPS;
            return false;
        }
        
        var allowedIpsList = allowedIpsStr.Split(",").Select(x => x.Trim()).ToList();
        
        if (!IPAddress.TryParse(ipAddrStr, out firstIpAddr))
        {
            Console.WriteLine($"Could not parse IP Address {firstIpAddr}");
            errorCode = CliErrorCodes.COULD_NOT_PARSE_IP_ADDRESS;
            return false;
        }

        if (!IpHelper.IsPartOfNetwork(firstIpAddr, allowedIpsList, out network))
        {
            Console.WriteLine($"{firstIpAddr} is not part of any network in {allowedIpsStr}");
            errorCode = CliErrorCodes.ADDRESS_NOT_PART_OF_NETWORK;
            return false;
        }

        if (network.Netmask.Equals(firstIpAddr) || network.Broadcast.Equals(firstIpAddr))
        {
            Console.WriteLine($"Start address {firstIpAddr} cannot equal network or broadcast address.");
            errorCode = CliErrorCodes.START_ADDRESS_IS_NETWORK_OR_BROADCAST_ADDRESS;
            return false;
        }
        
        lastIpAddr = IpHelper.GetNextIpAddress(firstIpAddr, (uint)numOfClients-1);
        
        if (!network.Contains(lastIpAddr))
        {
            Console.WriteLine($"Last ip address ({lastIpAddr}) is outside of network {network}");
            errorCode = CliErrorCodes.LAST_IP_IS_OUTSIDE_OF_NETWORK;
            return false;
        }

        if (network.Netmask.Equals(lastIpAddr) || network.Broadcast.Equals(lastIpAddr))
        {
            Console.WriteLine($"Last address {lastIpAddr} cannot equal network or broadcast address.");
            errorCode = CliErrorCodes.LAST_ADDRESS_IS_NETWORK_OR_BROADCAST_ADDRESS;
            return false;
        }

        return true;
    }
}