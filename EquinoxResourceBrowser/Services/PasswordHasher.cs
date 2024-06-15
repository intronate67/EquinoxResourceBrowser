using EquinoxResourceBrowser.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace EquinoxResourceBrowser.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly string _salt;
        private readonly string _originalHash;

        public PasswordHasher(IConfiguration configuration)
        {
            _salt = configuration["Password:Salt"] ?? throw new InvalidOperationException("Failed to find salt in user secrets");
            _originalHash = configuration["Password:OriginalHash"] ?? throw new InvalidOperationException("Failed to find original hash in user secrets");
        }

        public async Task<bool> VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (string.IsNullOrEmpty(_originalHash)) return false;

            var saltedPassword = _salt + password;
            var hashedPassword = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword)));
            return await Task.FromResult(hashedPassword == _originalHash);
        }
    }
}
