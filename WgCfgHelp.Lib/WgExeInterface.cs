using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCfgHelp.Lib
{
    public class WgExeInterface
    {
        public readonly string[] DEFAULT_LOCATIONS = new string[]
        {
            "wg.exe",
            "wg",
            "C:\\Program Files\\WireGuard\\wg.exe",
            "/opt/homebrew/bin/wg",
            "/usr/bin/wg"
        };


        public string? ExecutablePath { get; set; }

        public bool DetermineExeLocation()
        {
            foreach(var location in DEFAULT_LOCATIONS)
            {
                if (File.Exists(location))
                {
                    ExecutablePath = location;
                    return true;
                }
            }
            return false;
        }

        private WgExeInterface()
        {

        }

        public static WgExeInterface Create()
        {
            var wgExeInterface = new WgExeInterface();
            if (!wgExeInterface.DetermineExeLocation())
            {
                throw new WgExeInterfaceException("Count not determine executable location");
            }

            return wgExeInterface;
        }

        public static bool TryCreate(out WgExeInterface wgExeInterface)
        {
            wgExeInterface = new WgExeInterface();
            if (!wgExeInterface.DetermineExeLocation())
            {
                return false;
            }
            return true;
        }

        private string RunProcessGetOutput(string arguments, string? stdInput = null)
        {
            using (var proc = new Process())
            {
                proc.StartInfo.FileName = ExecutablePath;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = true;

                proc.Start();
                if (stdInput != null)
                {
                    proc.StandardInput.WriteLine(stdInput);
                    proc.StandardInput.Close();
                }
                while (!proc.WaitForExit(1000)) ;
                if (proc.ExitCode != 0)
                {
                    throw new WgExeInterfaceException($"{ExecutablePath} exited with code {proc.ExitCode}");
                }
                return proc.StandardOutput.ReadToEnd().Trim();
            }
        }

        public string GenPrivateKey()
        {
            return RunProcessGetOutput("genkey");
        }

        public string GenPublicKey(string privateKey)
        {
            return RunProcessGetOutput($"pubkey", privateKey);
        }

        public string GenPresharedKey()
        {
            return RunProcessGetOutput($"genpsk");
        }
    }
}
