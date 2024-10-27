using HUBT_Social_API.src.Core.Settings;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Authentication.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;


namespace HUBT_Social_API.src.Features.Auth.Services.ChildServices
{
    public class EmailService : IEmailService
    {
        private readonly SMPTSetting _emailSetting;
        private readonly IMongoCollection<Postcode> _postcode;
        private readonly UserManager<AUser> _userManager;


        public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode, UserManager<AUser> userManager)
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

            string smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _emailSetting.Host;
            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? _emailSetting.Port);
            string smtpEmail = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? _emailSetting.Email;
            string smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? _emailSetting.Password;

            smpt.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            smpt.Authenticate(smtpEmail, smtpPassword);


            await smpt.SendAsync(email);

            smpt.Disconnect(true);
        }
        public async Task<Postcode> CreatePostcode(string reciver)
        {
            Random random = new();

            string code = random.Next(100000, 999999).ToString();
            var expireTime = DateTime.UtcNow;
            Postcode postcode = new() { Code = code, StudentCode = reciver, ExpireTime = expireTime };

            await _postcode.InsertOneAsync(postcode);
            return postcode;
        }

        public async Task<AUser?> ValidatePostcode(VLpostcodeRequest postcode)
        {
            AUser? user = await _userManager.FindByNameAsync(postcode.UserName);
            if (user == null)
            {
                return null;
            }
            Postcode userPostcode = await _postcode.Find(ps => ps.Code == postcode.Postcode && ps.StudentCode == postcode.UserName).FirstOrDefaultAsync();
            if (userPostcode == null)
            {
                return null;
            }
            return user;


        }

    }
}
