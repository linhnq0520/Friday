namespace Friday.Modules.Salon.Application.Security;

public interface IAdminPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
