namespace Servises.Interfaces;

public interface IEmailService
{
    Task SendConfirmEmail(string email, string emailBodyUrl);
    Task SendResetPasswordEmail(string email, string emailBodyUrl);
    Task ConfirmUserEmailByToken(string email, string token);
    Task ConfirmAdminEmailByToken(string email, string token);
}
