using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Net;

namespace StarDust.Collections
{
    public static class Smtp
    {
        #region Properties

        

        #endregion

        #region Methods

        /// <summary>
        /// Sends a basic e-mail message with attachment(s)
        /// </summary>
        /// <param name="toAddress">List of simicolon delimited email address.</param>
        /// <param name="fromAddress">Return email address</param>
        /// <param name="subject">Text that will appear in the email's subject line</param>
        /// <param name="body">Text that will appear in the body of the email</param>
        /// <param name="isHTML">If true will send the email in HTML format</param>
        /// <param name="priority">high,normal,or low. If empty, will default to normal.</param>
        /// <param name="attachments">A list of attachments to include in the email</param>
        public static void Send(string toAddress, string fromAddress,
                                string subject, string body, bool isHTML, string priority, params string[] attachments)
        {
            Send(toAddress, null, null, fromAddress, null, subject, body, isHTML, priority, attachments);
        }

        /// <summary>
        /// Sends a basic e-mail message with attachment(s)
        /// </summary>
        /// <param name="toAddress">List of simicolon delimited email address.</param>
        /// <param name="ccAddress">List of simicolon delimited email address.</param>
        /// <param name="bccAddress">List of simicolon delimited email address.</param>
        /// <param name="fromAddress">Return email address</param>
        /// <param name="replyToAddress">ReplyTo email address</param>
        /// <param name="subject">Text that will appear in the email's subject line</param>
        /// <param name="body">Text that will appear in the body of the email</param>
        /// <param name="isHTML">If true will send the email in HTML format</param> 
        /// <param name="priority">high,normal,or low. If empty, will default to normal.</param>
        /// <param name="attachments">A list of attachments to include in the email</param>
        public static void Send(string toAddress, string ccAddress, string bccAddress, string fromAddress, string replyToAddress,
                                string subject, string body, bool isHTML, string priority, params string[] attachments)
        {
            Send(BuildMailAddressCollection(toAddress), BuildMailAddressCollection(ccAddress), BuildMailAddressCollection(bccAddress), fromAddress, replyToAddress, subject, body, isHTML, priority, attachments);
        }

        /// <summary>
        /// Sends a basic e-mail message with attachment(s)
        /// </summary>
        /// <param name="toAddresses">An array of email address.</param>
        /// <param> Return email address Return email address <name>fromAddress</name> </param>
        /// <param name="fromAddress"> Return email address Return email address</param>
        /// <param name="subject">Text that will appear in the email's subject line</param>
        /// <param name="body">Text that will appear in the body of the email</param>
        /// <param name="isHTML">If true will send the email in HTML format</param>
        /// <param name="priority">high,normal,or low. If empty, will default to normal.</param>
        /// <param name="attachments">A list of attachments to include in the email</param>
        public static void Send(string[] toAddresses, string fromAddress,
                                string subject, string body, bool isHTML, string priority, params string[] attachments)
        {
            Send(toAddresses, null, null, fromAddress, null, subject, body, isHTML, priority, attachments);
        }

        /// <summary>
        /// Sends a basic e-mail message with attachment(s)
        /// </summary>
        /// <param name="toAddresses">An array of email address.</param>
        /// <param name="ccAddresses">List of simicolon delimited email address.</param>
        /// <param name="bccAddresses">List of simicolon delimited email address.</param>
        /// <param name="fromAddress"> Return email address Return email address </param>
        /// <param name="replyToAddress">ReplyTo email address</param>
        /// <param name="subject">Text that will appear in the email's subject line</param>
        /// <param name="body">Text that will appear in the body of the email</param>
        /// <param name="isHTML">If true will send the email in HTML format</param>
        /// <param name="priority">high,normal,or low. If empty, will default to normal.</param>
        /// <param name="attachments">A list of attachments to include in the email</param>
        public static void Send(string[] toAddresses, string[] ccAddresses, string[] bccAddresses, string fromAddress, string replyToAddress,
                                string subject, string body, bool isHTML, string priority, params string[] attachments)
        {
            Send(BuildMailAddressCollection(toAddresses), BuildMailAddressCollection(ccAddresses), BuildMailAddressCollection(bccAddresses), fromAddress, replyToAddress, subject, body, isHTML, priority, attachments);
        }

        /// <summary>
        /// Sends a basic e-mail message with attachment(s)
        /// </summary>
        /// <param name="toAddresses">An array of email address.</param>
        /// <param name="ccAddresses">List of simicolon delimited email address.</param>
        /// <param name="bccAddresses">List of simicolon delimited email address.</param>
        /// <param name="fromAddress">Return email address</param>
        /// <param name="replyToAddress">ReplyTo email address</param>                /// 
        /// <param name="subject">Text that will appear in the email's subject line</param>
        /// <param name="body">Text that will appear in the body of the email</param>
        /// <param name="isHTML">If true will send the email in HTML format</param>
        /// <param name="priority">high,normal,or low. If empty, will default to normal.</param>
        /// <param name="attachments">A list of attachments to include in the email</param>
        public static void Send(MailAddressCollection toAddresses, MailAddressCollection ccAddresses, MailAddressCollection bccAddresses, string fromAddress, string replyToAddress,
                                string subject, string body, bool isHTML, string priority, params string[] attachments)
        {
            using (MailMessage message = new MailMessage())
            {
                foreach (MailAddress ma in toAddresses)
                {
                    message.To.Add(ma);
                }
                foreach (MailAddress mac in ccAddresses)
                {
                    message.CC.Add(mac);
                }
                foreach (MailAddress mabc in bccAddresses)
                {
                    message.Bcc.Add(mabc);
                }

                if (!string.IsNullOrEmpty(replyToAddress))
                {
                    message.ReplyTo = new MailAddress(replyToAddress);
                }

                message.From = new MailAddress(fromAddress);
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                message.IsBodyHtml = isHTML;
                if (!string.IsNullOrEmpty(priority))
                {
                    switch (priority.ToLower())
                    {
                        case "high":
                            message.Priority = MailPriority.High;
                            break;
                        case "low":
                            message.Priority = MailPriority.Low;
                            break;
                    }
                }
                else
                {
                    message.Priority = MailPriority.Normal;
                }
                foreach (string t in attachments)
                {
                    if (string.IsNullOrEmpty(t)) continue;
                    Attachment attachedFile = new Attachment(t, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = attachedFile.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(t);
                    disposition.ModificationDate = File.GetLastWriteTime(t);
                    disposition.ReadDate = File.GetLastAccessTime(t);
                    message.Attachments.Add(attachedFile);
                }
                Send(message);
            }
        }

        /// <summary>
        /// Builds a collection of email address based upon a semicolon delimited sring 
        /// </summary>
        /// <param name="mailAddresses">System.Net.Mail.MailMessage object</param>
        /// <returns>Collection of email addresses</returns>
        public static MailAddressCollection BuildMailAddressCollection(string mailAddresses)
        {
            if (string.IsNullOrEmpty(mailAddresses))
            {
                return new MailAddressCollection();
            }
            string[] addresses = mailAddresses.Split(';');
            return BuildMailAddressCollection(addresses);
        }

        /// <summary>
        /// Builds a collection of email address based upon a semicolon delimited sring 
        /// </summary>
        /// <param name="mailAddresses">System.Net.Mail.MailMessage object</param>
        /// <returns>Collection of email addresses</returns>
        public static MailAddressCollection BuildMailAddressCollection(string[] mailAddresses)
        {
            MailAddressCollection addressCollection = new MailAddressCollection();
            if (mailAddresses != null)
            {
                foreach (string emailAddress in mailAddresses.Where(emailAddress => emailAddress.Trim() != ""))
                {
                    try
                    {
                        addressCollection.Add(emailAddress.Trim());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error adding email address to collection: " + emailAddress.Trim(), ex);
                    }
                }
            }
            return addressCollection;
        }

        /// <summary>
        /// sends the MailMessage Synchronously
        /// </summary>
        /// <param name="mailMessage">System.Net.Mail.MailMessage object</param>
        /// <returns>message sent Success/Failure</returns>
        public static void Send(MailMessage mailMessage)
        {
            //Add the Creddentials
            SmtpClient client = new SmtpClient {Port = smtpPort, Host = smtpAddress, EnableSsl = false};   
            if (smtpUseCreditaials)
                client.Credentials = new NetworkCredential(smtpLogin, smtpPassword);
            try
            {
                //you can also call client.Send(msg)
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {

            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        public static string smtpPassword { get; set; }

        public static string smtpLogin { get; set; }

        public static bool smtpUseCreditaials { get; set; }

        public static string smtpAddress { get; set; }

        public static int smtpPort { get; set; }

        #endregion


    }
}
