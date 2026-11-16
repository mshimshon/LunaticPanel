namespace LunaticPanel.Core.Extensions;

public static class AnsiColorExt
{
    private const string RESET = "\u001b[0m";
    private const string RED = "\u001b[31m";
    private const string GREEN = "\u001b[32m";
    private const string YELLOW = "\u001b[33m";
    private const string BLUE = "\u001b[34m";
    private const string MAGENTA = "\u001b[35m";
    private const string CYAN = "\u001b[36m";
    private const string WHITE = "\u001b[37m";
    public static string Red(this string str)
    {
        return $"{RED}{str}{RESET}";
    }
    public static string Green(this string str)
    {
        return $"{GREEN}{str}{RESET}";
    }
    public static string Yellow(this string str)
    {
        return $"{YELLOW}{str}{RESET}";
    }

    public static string Blue(this string str)
    {
        return $"{BLUE}{str}{RESET}";
    }

    public static string Magenta(this string str)
    {
        return $"{MAGENTA}{str}{RESET}";
    }

    public static string Cyan(this string str)
    {
        return $"{CYAN}{str}{RESET}";
    }

    public static string White(this string str)
    {
        return $"{WHITE}{str}{RESET}";
    }
}
