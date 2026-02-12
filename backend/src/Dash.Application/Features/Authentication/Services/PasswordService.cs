using System.Security.Cryptography;
using Dash.Application.Features.Authentication.Interfaces;

namespace Dash.Application.Features.Authentication.Services;

internal sealed class PasswordService : IPasswordService
{
    private const int SaltSize = 32;        // 256 bits
    private const int HashSize = 32;        // 256 bits
    private const int Iterations = 600000;  // OWASP recommendation

    public Task<string> HashPasswordAsync(string password)
    {
        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Hash the password with PBKDF2
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSize
        );

        // combine salt and hash than convert to base64
        // Format [salt][hash]
        byte[] combined = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, combined, 0, SaltSize);
        Array.Copy(hash, 0, combined, SaltSize, HashSize);

        return Task.FromResult(Convert.ToBase64String(combined));
    }

    public Task<bool> VerifyPasswordAsync(string password, string passwordHash)
    {
        // Convert the stored hash back to bytes
        byte[] combined = Convert.FromBase64String(passwordHash);

        // get the salt (first 32 bytes)
        byte[] salt = new byte[SaltSize];
        Array.Copy(combined, 0, salt, 0, SaltSize);

        // get the stored hash (next 32 bytes)
        byte[] storedHash = new byte[HashSize];
        Array.Copy(combined, SaltSize, storedHash, 0, HashSize);

        // Hash the given password with the same salt
        byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSize
       );

        // compare the two hashes
        bool isValid = CryptographicOperations.FixedTimeEquals(storedHash, testHash);
        return Task.FromResult(isValid);
    }
}
