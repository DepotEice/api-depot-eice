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

        /// <summary>
        /// Send an activation email to the user
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="userToken"></param>
        /// <param name="destinationEmail"></param>
        /// <returns>
        /// <c>true</c> If the email was correctly sent. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="NullReferenceException"></exception>
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
                .WithHtmlPart("<h1>Bonjour</h1> " +
                    "<p>Veuillez cliquer sur le lien ci-dessous pour activer votre compte</p> " +
#if DEBUG
                    $"<a href=\"https://localhost:7245/activation?tokenId={tokenId}&tokenValue{userToken}\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}/activation?tokenId={tokenId}&tokenValue{userToken}\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();

            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }

        /// <summary>
        /// Send a password reset email to the user
        /// </summary>
        /// <param name="userId">the ID of the user</param>
        /// <param name="userToken">The token</param>
        /// <param name="destinationEmail">The destination email</param>
        /// <returns>
        /// <c>true</c> If the email was successfully sent. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="NullReferenceException"></exception>
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
                    $"<a href=\"https://localhost:7245/reset-password?userId={userId}&token={userToken}\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}/reset-password?userId={userId}&token={userToken}\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();

            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }

        /// <summary>
        /// Send an email to confirm the appointment
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="appointmentId">The id of the appointment</param>
        /// <param name="destinationEmail"></param>
        /// <returns>
        /// <c>true</c> If the email was successfully sent. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="NullReferenceException"></exception>
        public static async Task<bool> SendAppointmentConfirmedEmail(string userName, int appointmentId, string destinationEmail)
        {
            MailjetClient client = new MailjetClient(
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PUBLIC") ??
                    throw new NullReferenceException("There is no Environment variable named TFE_MAILJET_API_KEY_PUBLIC"),
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PRIVATE") ??
                    throw new NullReferenceException("There is no environment variable named TFE_MAILJET_API_KEY_PRIVATE"));

            MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource
            };

            TransactionalEmail email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("soultan.hatsijev@hainaut-promsoc.be"))
                .WithSubject("EICE - Rendez-vous confirmé")
                .WithHtmlPart(
                    $"<h1>Bonjour \"{userName}\"</h1> " +
                   "<p>Votre rendez-vous a bien été confirmé</p> " +
#if DEBUG
                    $"<a href=\"https://localhost:7245/profile/appointments/{appointmentId}\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}/profile/appointments/{appointmentId}\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();
            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }

        /// <summary>
        /// Send an email to confirm the deletion
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="appoitmentDate"></param>
        /// <param name="destinationEmail"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static async Task<bool> SendAppointmentDeletedMail(string userName, DateTime appoitmentDate, string destinationEmail)
        {
            MailjetClient client = new MailjetClient(
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PUBLIC") ??
                    throw new NullReferenceException("There is no Environment variable named TFE_MAILJET_API_KEY_PUBLIC"),
                Environment.GetEnvironmentVariable("TFE_MAILJET_API_KEY_PRIVATE") ??
                    throw new NullReferenceException("There is no environment variable named TFE_MAILJET_API_KEY_PRIVATE"));

            MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource
            };

            TransactionalEmail email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("soultan.hatsijev@hainaut-promsoc.be"))
                .WithSubject("EICE - Rendez-vous supprimé")
                .WithHtmlPart(
                    $"<h1>Bonjour \"{userName}\"</h1> " +
                    $"<p>Votre rendez-vous du {appoitmentDate.ToString("F")} a été annulé/supprimé</p> " +
                    $"<p>Pour reprogrammer un nouveau rendez-vous, suivant le lien suivant</p>" +
#if DEBUG
                    $"<a href=\"https://localhost:7245/appointments\">Cliquez-ici</a>")
#else
                    $"<a href=\"https://www.{DOMAIN_NAME}/appointments\">Cliquez-ici</a>")
#endif
                .WithTo(new SendContact(destinationEmail))
                .Build();
            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);

            return response.Messages.Length == 1;
        }
    }
}
