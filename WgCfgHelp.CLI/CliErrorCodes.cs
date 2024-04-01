namespace WgCfgHelp.CLI;

public static class CliErrorCodes
{
    public const int INVALID_FORMAT = 12;
    public const int FILE_ALREADY_EXISTS = 11;
    public const int OUTPUT_FOLDER_DOES_NOT_EXIST = 10;
    public const int MISSING_PRIVATE_KEY = 9;
    public const int INVALID_LISTEN_PORT = 8;
    public const int SUCCESS = 0;
    public const int LAST_ADDRESS_IS_NETWORK_OR_BROADCAST_ADDRESS = 7;
    public const int COULD_NOT_PARSE_IP_ADDRESS = 4;
    public const int CONFIG_FILE_NOT_FOUND = 1;
    public const int MISSING_PARAMETER_ALLOWED_IPS = 2;
    public const int ADDRESS_NOT_PART_OF_NETWORK = 3;
    public const int START_ADDRESS_IS_NETWORK_OR_BROADCAST_ADDRESS = 5;
    public const int LAST_IP_IS_OUTSIDE_OF_NETWORK = 6;
}