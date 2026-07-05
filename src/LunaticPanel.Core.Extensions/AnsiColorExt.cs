namespace LunaticPanel.Core.Extensions;

public static class AnsiColorExt
{
    private const string Reset = "\u001b[0m";
    private const string _red = "\u001b[31m";
    private const string _green = "\u001b[32m";
    private const string _yellow = "\u001b[33m";
    private const string _blue = "\u001b[34m";
    private const string _magenta = "\u001b[35m";
    private const string _cyan = "\u001b[36m";
    private const string _white = "\u001b[37m";
    public static string Red(this string str)
    {
        return $"{_red}{str}{Reset}";
    }
    public static string Green(this string str)
    {
        return $"{_green}{str}{Reset}";
    }
    public static string Yellow(this string str)
    {
        return $"{_yellow}{str}{Reset}";
    }

    public static string Blue(this string str)
    {
        return $"{_blue}{str}{Reset}";
    }

    public static string Magenta(this string str)
    {
        return $"{_magenta}{str}{Reset}";
    }

    public static string Cyan(this string str)
    {
        return $"{_cyan}{str}{Reset}";
    }

    public static string White(this string str)
    {
        return $"{_white}{str}{Reset}";
    }
}
