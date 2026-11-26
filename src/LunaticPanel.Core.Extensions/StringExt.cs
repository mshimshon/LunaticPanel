using System.Text.RegularExpressions;

namespace LunaticPanel.Core.Extensions;

public static class StringExt
{
    public static string ClearNewLines(this string str)
        => Regex.Replace(str, @"\r?\n", string.Empty);
    public static string ClearSpaces(this string str)
        => Regex.Replace(str, @"\s+", "");

}
