using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace CRM.Common.Extentions;

public static class Generators
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string RandomString(int lenght)
    {
        if (lenght <= 0)
            throw new ArgumentOutOfRangeException(nameof(lenght));

        var rnd = new Random();

        var builder = new StringBuilder();

        for (int i = 1; i <= lenght; i++)
        {
            var charater = Characters[rnd.Next(Characters.Length - 1)];

            builder.Append(charater);
        }

        return builder.ToString();
    }

    public static string HashPassword(string password, string salt)
    {
        var hasher = Encoding.UTF8.GetBytes(salt + password);

        var hash = SHA256.Create().ComputeHash(hasher);

        return Convert.ToBase64String(hash);

    }
}
