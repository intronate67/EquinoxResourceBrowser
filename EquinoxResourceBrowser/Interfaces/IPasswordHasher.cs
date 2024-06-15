namespace EquinoxResourceBrowser.Interfaces
{
    public interface IPasswordHasher
    {
        Task<bool> VerifyPassword(string password);
    }
}
