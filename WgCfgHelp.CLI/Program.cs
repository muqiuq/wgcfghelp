using System.CommandLine;
using System.Text.RegularExpressions;
using WgCfgHelp.CLI.Handler;
using WgCfgHelp.CLI.Models;
using WgCfgHelp.Lib;

namespace WgCfgHelp.CLI
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var genClient = new ClientConfigHandler();
            
            var rootCommand = new RootCommand("WireGuard Config Helper")
            {
                genClient.GetCommand()
            };

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
