using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client.TransactionalEmails.Response;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace API.DepotEice.UIL.Managers
{
    /// <summary>
    /// Manager for sending emails
    /// </summary>
    public static class MailManager
    {
#if RELEASE
        private static readonly string DOMAIN_NAME = Environment.GetEnvironmentVariable("DOMAIN_NAME") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named : " +
                $"\"DOMAIN_NAME\"");
#endif
        public static async Task<bool> SendActivationEmailAsync(string tokenId, string userToken, string destinationEmail)
        {
            MailjetClient client = new MailjetClient(
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PUBLIC") ??
                    throw new NullReferenceException("There is no Environment variable named TFE_MAILJET_API_KEY_PUBLIC"),
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PRIVATE") ??
                    throw new NullReferenceException("There is no environment variable named TFE_MAILJET_API_KEY_PRIVATE"));

            MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource,
            };

            TransactionalEmail email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("soultan.hatsijev@hainaut-promsoc.be"))
                .WithSubject("Activation de votre compte Depot EICE")
                .WithHtmlPart("<h1>Bonjour \"Nom\"</h1> " +
                    "<p>Veuillez cliquer sur le lien ci-dessous pour activer votre compte</p> " +
#if DEBUG
                    $"<a href=\"https://localhost:7245/activation/{tokenId}/{userToken}\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}auth/activation?token={userToken}\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();

            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }

        /// <summary>
        /// Send the account activation email
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="userToken"></param>
        /// <param name="destinationEmail"></param>
        public static bool SendActivationEmail(string tokenId, string userToken, string destinationEmail)
        {

            try
            {
                MailAddress fromAdresse = new("pid.depot@gmail.com", "EICE Dépôt");
                MailAddress toAdresse = new(destinationEmail, "To display name");
                const string fromPassword = "pazn gnsz llov qccp"; // PASSWORD 16 DIGITS. Se trouve sur bureau GMAIL_TOKEN_PASSWORD.png
                const string subject = "PID - Activitation du compte";

                StringBuilder body = new StringBuilder();
                body.Append("<h1>Bonjour \"Nom\"</h1>");
                body.Append("<p>Veuillez cliquer sur le lien ci-dessous pour activer votre compte</p>");

#if DEBUG
                body.Append($"<a href=\"https://localhost:7245/activation/{tokenId}/{userToken}\">Cliquez-ici</a>");
#else
                body.Append($"<a href=\"https://www.{DOMAIN_NAME}auth/activation?token={userToken}\">Cliquez-ici</a>");
#endif

                using (MailMessage mail = new())
                {
                    mail.From = fromAdresse;
                    mail.To.Add(toAdresse);
                    mail.Subject = subject;
                    mail.Body = body.ToString();
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(fromAdresse.Address, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> SendPasswordRequestEmailAsync(string userId, string userToken, string destinationEmail)
        {
            MailjetClient client = new MailjetClient(
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PUBLIC") ??
                    throw new NullReferenceException("There is no Environment variable named TFE_MAILJET_API_KEY_PUBLIC"),
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PRIVATE") ?? 
                    throw new NullReferenceException("There is no environment variable named TFE_MAILJET_API_KEY_PRIVATE"));

            MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource,
            };

            TransactionalEmail email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("soultan.hatsijev@hainaut-promsoc.be"))
                .WithSubject("Mot de passe oublié")
                .WithHtmlPart("<h1>Bonjour \"Nom\"</h1> " +
                    "<p>Veuillez cliquer sur le lien ci-dessous pour réinitialiser votre mot de passe</p> " +
#if DEBUG
                    $"<a href=\"https://localhost:7245/updatePassword/{userId}/{userToken}\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}auth/activation?token={userToken}\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();

            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }

        public static bool SendPasswordRequestEmail(string userId, string userToken, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            try
            {
                MailAddress fromAdresse = new("pid.depot@gmail.com", "EICE Dépôt");
                MailAddress toAdresse = new(email, "To display name");
                const string fromPassword = "pazn gnsz llov qccp"; // PASSWORD 16 DIGITS. Se trouve sur bureau GMAIL_TOKEN_PASSWORD.png
                const string subject = "PID - Mot de passe oublié";

                StringBuilder body = new StringBuilder();
                body.Append("<h1>Bonjour \"Nom\"</h1>");
                body.Append("<p>Veuillez cliquer sur le lien ci-dessous pour créer un nouveau mot de passe</p>");

#if DEBUG
                body.Append($"<a href=\"https://localhost:7245/updatePassword/{userId}/{userToken}\">Cliquez-ici</a>");
#else
                body.Append($"<a href=\"https://www.{DOMAIN_NAME}auth/activation?token={userToken}\">Cliquez-ici</a>");
#endif
                using (MailMessage mail = new())
                {
                    mail.From = fromAdresse;
                    mail.To.Add(toAdresse);
                    mail.Subject = subject;
                    mail.Body = body.ToString();
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(fromAdresse.Address, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
