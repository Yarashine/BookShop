using MailKit.Net.Smtp;
using MimeKit;
using Servises.Interfaces;
using Microsoft.Extensions.Configuration;
using Models.Abstractions;
using Models.Entities;
using Models.Exceptions;

namespace Servises.Services;

public class EmailService(IUnitOfWork _unitOfWork, IConfiguration configuration) : BaseService(_unitOfWork), IEmailService
{
    private readonly IConfiguration _config = configuration;

    public async Task SendConfirmEmail(string email, string emailBodyUrl)
    {
        var subject = "Email confirmation";
        var emailBody = $"To confirm your email <a href=\"{emailBodyUrl}\">click here </a> ";
        await SendEmail(email, subject, emailBody);
    }

    public async Task SendResetPasswordEmail(string email, string emailBodyUrl)
    {
        var subject = "Password reset";
        var emailBody = $"To reset your password <a href=\"{emailBodyUrl}\">click here </a> ";
        await SendEmail(email, subject, emailBody);
    }

    private async Task SendEmail(string email, string subject, string message)
    {
        using var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_config["Email:From:Name"], _config["Email:From:Address"]));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_config["Email:SmtpServer"], Convert.ToInt32(_config["Email:Port"]), Convert.ToBoolean(_config["Email:UseSSL"]));
            await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
            var res1 = await client.SendAsync(emailMessage);
        }
        catch
        {
            throw new Exception("Email sender trouble");
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    public async Task ConfirmUserEmailByToken(string email, string token)
    {
        AuthorizationInfo? ai = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(email)
            ?? throw new NotFoundException(nameof(User));

        if (ai.IsEmailConfirmed)
            throw new BadRequestException("Email is already confirmed.");

        if (ai.EmailConfirmToken != token)
            throw new BadRequestException("Invalid token.");

        ai.IsEmailConfirmed = true;
        await unitOfWork.SaveAllAsync();
    }
    public async Task ConfirmAdminEmailByToken(string email, string token)
    {
        AuthorizationInfo ai = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(email)
            ?? throw new NotFoundException(nameof(Administrator));

        if (ai.IsEmailConfirmed)
            throw new BadRequestException("Email is already confirmed.");

        if (ai.EmailConfirmToken != token)
            throw new BadRequestException("Invalid token.");

        ai.IsEmailConfirmed = true;
        await unitOfWork.SaveAllAsync();
    }
}
