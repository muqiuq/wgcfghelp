namespace WgCfgHelp.Lib.IPHelpers;

public class IpHelperException : Exception
{
    public IpHelperException()
    {
    }

    public IpHelperException(string? message) : base(message)
    {
    }

    public IpHelperException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}