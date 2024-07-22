using System.Security.Cryptography;

namespace PayrollAPI.Data
{
    public class PasswordHasher
    {
        private const int _SaltSize = 128 / 8;
        private const int _KeySize = 256 / 8;
        private const int _Iterations = 15000;
        private static readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';

        public string Hash(string password, string epf)
        {
            var salt = RandomNumberGenerator.GetBytes(_SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2((password + epf), salt, _Iterations, hashAlgorithmName, _KeySize);

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool Verify(string passwordHash, string inputPassword, string epf)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2((inputPassword + epf), salt, _Iterations, hashAlgorithmName, _KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
