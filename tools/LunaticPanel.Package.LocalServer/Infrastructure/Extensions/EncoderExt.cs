using System.Text;

namespace LunaticPanel.Package.LocalServer.Infrastructure.Extensions;

public static class EncoderExt
{
    public static string ToBase32(this string input)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz234567";
        byte[] data = Encoding.UTF8.GetBytes(input);

        int value = 0;
        int bits = 0;

        var sb = new StringBuilder();

        foreach (var b in data)
        {
            value = (value << 8) | b;
            bits += 8;

            while (bits >= 5)
            {
                int index = (value >> (bits - 5)) & 0x1F;
                sb.Append(alphabet[index]);
                bits -= 5;
            }
        }

        return sb.ToString(); // no padding
    }

}
