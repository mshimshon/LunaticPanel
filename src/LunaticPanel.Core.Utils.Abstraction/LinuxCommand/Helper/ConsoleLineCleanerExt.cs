using System.Text.RegularExpressions;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper;

public static class ConsoleLineCleanerExt
{

    public static string CleanFromColorCodes(this string line)
    {
        if (string.IsNullOrEmpty(line)) return line;
        var output = line;
        // 1. Strip ANSI Color/Escape Codes (e.g., \x1B[31m)
        string noColor = Regex.Replace(output, @"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])", "");

        // 2. Strip Control Characters [\x00-\x1F\x7F]
        output = Regex.Replace(noColor, @"[\x00-\x1F\x7F]", "");
        return output;
    }
}
