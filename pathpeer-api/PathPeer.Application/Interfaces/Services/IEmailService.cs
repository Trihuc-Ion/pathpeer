namespace PathPeer.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPurchaseConfirmationAsync(string toEmail, string username, string courseTitle, decimal amount);
    Task SendTeacherApprovedAsync(string toEmail, string username);
    Task SendTeacherRejectedAsync(string toEmail, string username);
    Task SendCourseApprovedAsync(string toEmail, string username, string courseTitle);
    Task SendCourseRejectedAsync(string toEmail, string username, string courseTitle);
}