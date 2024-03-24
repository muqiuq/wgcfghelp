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

            var rootCommand = new RootCommand("WireGuard Config Helper")
            {
                (new SiteConfigGenHandler()).GetCommand(),
                (new ClientConfigHandler()).GetCommand()
            };

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
