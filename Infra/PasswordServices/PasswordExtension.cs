using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

public static class PasswordExtensions
{
    public static string ToArgon2Hash(this string senha)
    {
        int saltSize = 16;
        int hashSize = 32;

        byte[] salt = new byte[saltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(senha))
        {
            Salt = salt,
            DegreeOfParallelism = 2, // threads
            Iterations = 4,          // tempo
            MemorySize = 65536       // memória em KB (64MB)
        };

        byte[] hash = argon2.GetBytes(hashSize);

        // armazenar: salt + hash
        byte[] result = new byte[saltSize + hashSize];
        Buffer.BlockCopy(salt, 0, result, 0, saltSize);
        Buffer.BlockCopy(hash, 0, result, saltSize, hashSize);

        return Convert.ToBase64String(result);
    }

    public static bool VerifyArgon2Hash(this string senha, string hashArmazenado)
    {
        byte[] hashBytes = Convert.FromBase64String(hashArmazenado);

        int saltSize = 16;
        int hashSize = 32;

        byte[] salt = new byte[saltSize];
        byte[] hash = new byte[hashSize];

        Buffer.BlockCopy(hashBytes, 0, salt, 0, saltSize);
        Buffer.BlockCopy(hashBytes, saltSize, hash, 0, hashSize);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(senha))
        {
            Salt = salt,
            DegreeOfParallelism = 2,
            Iterations = 4,
            MemorySize = 65536
        };

        byte[] hashComparacao = argon2.GetBytes(hashSize);

        return CryptographicOperations.FixedTimeEquals(hash, hashComparacao);
    }
}