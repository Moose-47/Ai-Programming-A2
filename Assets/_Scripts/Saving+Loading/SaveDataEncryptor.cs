using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SaveDataEncryptor
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("a8F5kD2uWq9P0bXr7nVm4zCj6YtErU1Q");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("H7gT4vLxQ2mZ1pNc");

    public static string EcryptString(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using (StreamWriter sw = new(cs))
        {
            sw.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string DecryptString(string cipherText)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }
}
