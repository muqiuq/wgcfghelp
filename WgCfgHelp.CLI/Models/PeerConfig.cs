namespace WgCfgHelp.CLI.Models;

public class PeerConfig
{

    public string? Name { get; set; }
    
    public string? PublicKey { get; set; }

    public string? PresharedKey { get; set; }

    public string? AllowedIPs { get; set; }
    public string? Address { get; set; }
}