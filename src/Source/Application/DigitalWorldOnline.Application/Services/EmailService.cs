using DigitalWorldOnline.Commons.Interfaces;
using System.Net.Mail;
using System.Net;

namespace DigitalWorldOnline.Application.Services
{
    public class EmailService : IEmailService
    {
        public EmailService() { }

        public void Send(string destination)
        {
            string smtpServer = "dmo@gmail.com\n";
            int smtpPort = 587;
            string smtpUsername = "dmo@gmail.com\n";
            string smtpPassword = "*******";

            // Criando a mensagem de email
            MailMessage message = new MailMessage();
            message.From = new MailAddress(smtpUsername);
            message.To.Add("dmo@gmail.com\n"); // Endereço do destinatário
            message.Subject = "Digital Master Online - Account created.";
            message.Body = "Account created! Set your password here: http://127.0.0.1:2052/.";

            // Configurando o cliente SMTP
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            try
            {
                // Enviando o email
                smtpClient.Send(message);
                Console.WriteLine("Email sended !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao enviar o email: " + ex.Message);
            }
        }
    }
}
