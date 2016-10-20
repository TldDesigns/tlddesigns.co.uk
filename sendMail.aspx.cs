using System;
using System.Web;
using System.Net.Mail;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace TLDDesigns.Home
{
    public class EmailMessage
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public MailAddress To { get; set; }
        public MailAddress From { get; set; }
        public string Subject { get; set; }
        public MessageBody Message { get; set; }
        public EmailMessage(string name, string number, string to, string from, string subject, MessageBody message)
        {
            To = new MailAddress(to);
            From = new MailAddress(from);
            Subject = subject;
            Message = message;
            Name = name;
            Number = number;
        }

        public EmailMessage()
        {
            Message = new MessageBody();
        }
        public static implicit operator MailMessage(EmailMessage message)
        {
            MailMessage returnMessage = new MailMessage(message.From, message.To);

            returnMessage.Subject = message.Subject;
            returnMessage.Body = message.Message.PlainBody;

            if (string.IsNullOrWhiteSpace(message.Message.HTMLBody) == false)
            {
                Stream HtmlStream = GenerateStreamFromString(message.Message.HTMLBody);
                AlternateView HtmlView = new AlternateView(HtmlStream, "text/html");
                returnMessage.AlternateViews.Add(HtmlView);
            }

            return returnMessage;
        }
        private static Stream GenerateStreamFromString(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }
    }

    public class ApiResponse
    {
        public string MethodName { get; set; }
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Details { get; set; }
        public bool Debug { get; set; }
        public ApiResponse(string methodName, bool success, int errorCode, string details, bool debug)
        {
            MethodName = methodName;
            Success = success;
            ErrorCode = errorCode;
            Details = details;
            Debug = debug;
        }
        public ApiResponse()
        {
            Success = true;
            ErrorCode = 0;
            Details = "";
            Debug = false;
        }
    }

    public class MessageBody
    {
        public string HTMLBody { get; set; }
        public string PlainBody { get; set; }
        public MessageBody()
        {

        }
    }

    public partial class sendMail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sendEmail();
        }
        protected void writeJSON(object output)
        {
            string outputString;

            Response.ContentType = "application/json";

            outputString = JsonConvert.SerializeObject(output);

            Response.Write(outputString);
        }
        protected void sendEmail()
        {
            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            bool debug = false;

            EmailMessage enquiryEmail = new EmailMessage();
            EmailMessage acknowledgeEmail = new EmailMessage();

            MessageBody enquiryMessageBody = new MessageBody();
            MessageBody acknowledgeMessageBody = new MessageBody();

            ApiResponse responseMessage = new ApiResponse();

            MailAddress enquiryAddress = new MailAddress("enquiries@tlddesigns.co.uk");
            MailAddress noReplyAddress = new MailAddress("noreply@tlddesigns.co.uk");

            //string responseEmailMessage;
            string acknowledgeEmailSubject = "Thank you for contacting us";

            string from = (string)Request["from"];
            string name = (string)Request["name"];

            responseMessage.MethodName = "sendMail";

            debug = GetDebugStatus();

            responseMessage.Debug = debug;

            if (string.IsNullOrEmpty(from) == false)
            {
                from = from.ToLower();
            }

            try
            {
                enquiryEmail.Name = name;
                enquiryEmail.Number = sanitiseInput(Request["number"]);
                enquiryEmail.From = new MailAddress(from, name);
                enquiryEmail.Subject = sanitiseInput(Request["subject"].ToString());
                enquiryEmail.Message.PlainBody = sanitiseInput(HttpUtility.UrlDecode(
                    Request["message"].ToString())
                    );
                enquiryEmail.To = enquiryAddress;

                acknowledgeEmail.From = noReplyAddress;
                acknowledgeEmail.To = enquiryEmail.From;
                acknowledgeEmail.Subject = acknowledgeEmailSubject;


                acknowledgeMessageBody = populateBody('r');

                enquiryMessageBody = populateBody('e', enquiryEmail.Name, enquiryEmail.Number, enquiryEmail.From.Address, enquiryEmail.Message.PlainBody);

                acknowledgeEmail.Message = acknowledgeMessageBody;
                enquiryEmail.Message = enquiryMessageBody;

                responseMessage.Details = sendEmails(enquiryEmail, acknowledgeEmail, debug, apiKey);

            }
            catch (Exception ex)
            //catch (SmtpException ex)
            {
                responseMessage.Success = false;
                responseMessage.ErrorCode = -1;
                responseMessage.Details = ex.Message;
            }
            writeJSON(responseMessage);
        }
        private bool GetDebugStatus()
        {
            bool debug = false;
            string debugString;
            debugString = (string)Request["debug"];

            if (string.IsNullOrEmpty(debugString) == false)
            {
                debugString = debugString.ToLower();

                if (debugString == "true")
                {
                    debug = true;
                }
                else
                {
                    debug = false;
                }
            }
            else
            {
                debug = false;
            }
            return debug;
        }
        protected string sanitiseInput(string input)
        {
            input = HttpUtility.HtmlEncode(input);

            return input;
        }
        private MessageBody populateBody(char type, string name = "", string number = "", string email = "", string txtBody = "")
        {
            string body = string.Empty;
            string bodyText = string.Empty;

            MessageBody messageBody = new MessageBody();

            if (type == 'r')
            {

                using (StreamReader reader = new StreamReader(Server.MapPath("~/templates/responseTemplate.html")))
                {
                    body = reader.ReadToEnd();
                }

                using (StreamReader reader = new StreamReader(Server.MapPath("~/templates/responseBody.txt")))
                {
                    bodyText = reader.ReadToEnd();
                }
                body = body.Replace("{body_text}", bodyText);

                bodyText = bodyText.Replace("<p>", "");

                bodyText = bodyText.Replace("</p>", "");
            }
            else if (type == 'e')
            {
                using (StreamReader reader = new StreamReader(Server.MapPath("~/templates/enquiryTemplate.html")))
                {
                    body = reader.ReadToEnd();
                }
                bodyText = HttpUtility.HtmlEncode(txtBody);

                body = body.Replace("{name}", name);

                body = body.Replace("{number}", number);

                body = body.Replace("{email}", email);

                body = body.Replace("{message}", bodyText);
            }

            messageBody.HTMLBody = body;
            messageBody.PlainBody = bodyText;

            return messageBody;
        }
        protected string sendEmails(EmailMessage enquiry, EmailMessage acknowledgement, bool debug, string apiKey)
        {
            string details = string.Empty;

            MailMessage enquiryMail;
            MailMessage acknowledgeMail;

            bool enquiryMailSuccess = false;
            bool acknowledgeMailSuccess = false;

            enquiryMail = enquiry;

            acknowledgeMail = acknowledgement;

            //SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.sendgrid.net";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("apikey", apiKey);
            //smtp.Timeout = 20000;


            if (debug == false)
            {
                //MailMessage asyncMail;
                //if (type == "e")
                //{
                //    asyncMail = enquiryMail;
                //}
                //else {
                //    type = "a";
                //    asyncMail = acknowledgeMail;
                //}
                sendEmailAsync(acknowledgeMail, enquiryMail, smtp);
            }
            else
            {
                acknowledgeMailSuccess = sendEmailSync(acknowledgeMail, smtp);
                enquiryMailSuccess = sendEmailSync(enquiryMail, smtp);
                var mailResponse = new { acknowledgeMailSent = acknowledgeMailSuccess, enquiryMailSent = enquiryMailSuccess };
                details = mailResponse.ToString();
                smtp.Dispose();
            }

            return details;
        }
        protected bool sendEmailSync(MailMessage message, SmtpClient smtp)
        {
            bool success = false;

            smtp.Send(message);

            message.Dispose();

            //smtp.Dispose();

            success = true;

            return success;
        }
        protected void sendEmailAsync(MailMessage acknowledgeMail, MailMessage enquiryMail, SmtpClient smtp)
        {
            smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            smtp.SendAsync(acknowledgeMail, enquiryMail);

            //message.Dispose();
        }
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            SmtpClient callbackClient = sender as SmtpClient;

            if (e.UserState is MailMessage)
            {
                MailMessage nextMessage = e.UserState as MailMessage;
                callbackClient.SendAsync(nextMessage, "completed");
            }
            else
            {
                callbackClient.Dispose();

            }

            //callbackMessage.Dispose();

        }
    }
}