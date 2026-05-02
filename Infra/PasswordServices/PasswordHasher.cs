using Application.PasswordsServices;

namespace Infra.PasswordServices
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string senha)
        {
            return senha.ToArgon2Hash();
        }

        public bool Verify(string senha, string hash)
        {
            return senha.VerifyArgon2Hash(hash);
        }
    }
}
