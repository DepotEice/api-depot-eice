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
        /// <summary>
        /// Send the account activation email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userToken"></param>
        /// <param name="destinationEmail"></param>
        public static bool SendActivationEmail(string userId, string userToken, string destinationEmail)
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
                //body.Append($"<a href=\"https://localhost:44332/api/auth/activation?id={userId}&token={token}\">Cliquez-ici</a>");
                body.Append($"<a href=\"https://localhost:44332/account/activation?id={userId}&token={userToken}\">Cliquez-ici</a>");
#else
                body.Append($"<a href=\"https://www.domain.com/auth/activation?token={userToken}\">Cliquez-ici</a>");
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
