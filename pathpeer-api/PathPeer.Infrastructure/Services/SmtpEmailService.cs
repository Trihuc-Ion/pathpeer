using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SmtpEmailService(IConfiguration configuration)
    {
        _host = configuration["Email:Host"] ?? throw new InvalidOperationException("Email:Host lipsește.");
        _port = int.Parse(configuration["Email:Port"] ?? "587");
        _username = configuration["Email:Username"] ?? throw new InvalidOperationException("Email:Username lipsește.");
        _password = configuration["Email:Password"] ?? throw new InvalidOperationException("Email:Password lipsește.");
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@pathpeer.com";
        _fromName = configuration["Email:FromName"] ?? "PathPeer";
    }

    public Task SendPurchaseConfirmationAsync(string toEmail, string username, string courseTitle, decimal amount) =>
        SendAsync(toEmail, "Confirmare achiziție — PathPeer", $"""
            Salut {username},

            Ai cumpărat cu succes cursul "{courseTitle}" pentru {amount} USD.

            Îl poți accesa oricând din contul tău PathPeer.

            PathPeer Team
            """);

    public Task SendTeacherApprovedAsync(string toEmail, string username) =>
        SendAsync(toEmail, "Cererea ta de profesor a fost aprobată!", $"""
            Salut {username},

            Felicitări! Cererea ta de a deveni profesor pe PathPeer a fost aprobată.
            Acum poți crea și publica cursuri.

            PathPeer Team
            """);

    public Task SendTeacherRejectedAsync(string toEmail, string username) =>
        SendAsync(toEmail, "Cererea ta de profesor a fost respinsă", $"""
            Salut {username},

            Din păcate, cererea ta de a deveni profesor pe PathPeer a fost respinsă.
            Poți trimite o nouă cerere oricând.

            PathPeer Team
            """);

    public Task SendCourseApprovedAsync(string toEmail, string username, string courseTitle) =>
        SendAsync(toEmail, $"Cursul tău \"{courseTitle}\" a fost aprobat!", $"""
            Salut {username},

            Cursul tău "{courseTitle}" a trecut review-ul comunității și este acum live pe PathPeer!

            PathPeer Team
            """);

    public Task SendCourseRejectedAsync(string toEmail, string username, string courseTitle) =>
        SendAsync(toEmail, $"Cursul tău \"{courseTitle}\" a fost respins", $"""
            Salut {username},

            Cursul tău "{courseTitle}" nu a acumulat suficiente voturi în perioada de review.
            Îl poți revizui și retrimite oricând.

            PathPeer Team
            """);

    private async Task SendAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true
        };

        var message = new MailMessage(
            from: new MailAddress(_fromEmail, _fromName),
            to: new MailAddress(toEmail))
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        await client.SendMailAsync(message);
    }
}