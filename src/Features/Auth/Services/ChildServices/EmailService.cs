using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Auth.Services.ChildServices;

public class EmailService : IEmailService
{
    private readonly SMPTSetting _emailSetting;
    private readonly IMongoCollection<Postcode> _postcode;
    private readonly UserManager<AUser> _userManager;


    public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode,
        UserManager<AUser> userManager)
    {
        _emailSetting = setting.Value;
        _postcode = postcode;
        _userManager = userManager;
    }

    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        MimeMessage email = new();
        email.Sender = MailboxAddress.Parse(_emailSetting.Email);
        email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
        email.Subject = emailRequest.Subject;

        email.Body = new TextPart(TextFormat.Plain)
        {
            Text = emailRequest.Code
        };

        using var smpt = new SmtpClient();

        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _emailSetting.Host;
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? _emailSetting.Port);
        var smtpEmail = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? _emailSetting.Email;
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? _emailSetting.Password;

        smpt.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        smpt.Authenticate(smtpEmail, smtpPassword);


        await smpt.SendAsync(email);

        smpt.Disconnect(true);
    }

    public async Task<Postcode> CreatePostcode(string reciver)
    {
        Random random = new();

        var code = random.Next(100000, 999999).ToString();
        var expireTime = DateTime.UtcNow;
        Postcode postcode = new() { Code = code, Email = reciver, ExpireTime = expireTime };

        await _postcode.InsertOneAsync(postcode);
        return postcode;
    }

    public async Task<AUser?> ValidatePostcode(ValidatePostcodeRequest postcode)
    {
        var user = await _userManager.FindByEmailAsync(postcode.Email);
        if (user == null) return null;
        var userPostcode = await _postcode.Find(ps => ps.Code == postcode.Postcode && ps.Email == postcode.Email)
            .FirstOrDefaultAsync();
        if (userPostcode == null) return null;
        return user;
    }
}