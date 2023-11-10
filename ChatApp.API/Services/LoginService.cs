using ChatApp.API.Dao;
using ChatApp.API.Untity;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
//using System.Net.Mail;
using System.Text.Json.Nodes;
using System.Runtime;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;

namespace ChatApp.API.Services
{

    class NotificationMetadata
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    class EmailMessage
    {
        public MailboxAddress Sender { get; set; }
        public MailboxAddress Reciever { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }


    public class LoginService: ILoginService
    {
        DataBaseHelper dataBaseHelper;

        MemoryCache validateCodeCache;

        public LoginService() {
            this.dataBaseHelper = new DataBaseHelper();
            this.validateCodeCache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool ValidateLoginInfo(User userInfo)
        {

            string queryAllUserInfo = "select * from UserLogin";

            var dt = this.dataBaseHelper.ExecuteQuery(queryAllUserInfo);

            List<User> userList = new List<User>();

            if (dt != null) 
            {
                foreach (DataRow row in dt.Rows)
                {
                    // 创建一个新的 User 对象
                    User user = new User();

                    // 将数据表中的列映射到 User 对象的属性
                    user.Name = row["name"]?.ToString() ?? "";
                    user.PassWord = row["password"]?.ToString() ?? "";
                    user.Email = row["email"]?.ToString() ?? "";

                    if ((user.Name.Equals(userInfo.Name) && user.PassWord.Equals(userInfo.PassWord)) ||
                        (user.Email.Equals(userInfo.Email) && user.PassWord.Equals(userInfo.PassWord)))
                    {
                        
                        return true;
                    }

                    // 将对象添加到列表中
                    userList.Add(user);
                }
            }
            // firstly need to query the database for user
            // validate the password is or not right
            //return JsonConvert.SerializeObject(dt, Formatting.Indented);
            return false;
        }

        public void SendValidateCode(string emailAddress)
        {
            Random random = new Random();
            string validateCode = random.Next(100000, 999999).ToString(); // 生成一个6位数的随机整数

            this.validateCodeCache.Set(emailAddress, validateCode, DateTimeOffset.Now.AddMinutes(10));

            //string sqlValidateCode = "INSERT INTO EmailVerificationTable (Email, VerificationCode) VALUES (@Email, @VerificationCode)";

            //this.dataBaseHelper.StoreVerificationCodeInDatabase(sqlValidateCode, emailAddress, validateCode);
            // send mail

            EmailMessage message = new EmailMessage();
            message.Sender = new MailboxAddress("Self", "zhaiyadong2022@163.com");
            message.Reciever = new MailboxAddress("Self", emailAddress);
            message.Subject = "注册验证码";
            message.Content = $"尊敬的用户，您的验证码是：{validateCode}。请在半小时内完成注册。";
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.163.com", 465, true);
                smtpClient.Authenticate("zhaiyadong2022@163.com", "QSMNXRWOHVXFOWWY");
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
        }

        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(message.Sender);
            mimeMessage.To.Add(message.Reciever);
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            { Text = message.Content };
            return mimeMessage;
        }

        public void RegisterUser(User user)
        {
            string toEmail = user.Email;
            string code = user.ValidateCode;

            if (ValidateUser(toEmail, code))
            {
                try
                {
                    string insertQuery = "INSERT INTO UserLogin (Name, Password, Email) VALUES (@Name, @Password, @Email)";

                    this.dataBaseHelper.InsertQuery(insertQuery, user);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            else
            {
                throw new Exception("validate fail");
            }
        }

        private bool ValidateUser(string email, string code)  
        {
            //string sql = "SELECT VerificationCode FROM UserVerification WHERE Email = @Email";

            //string newCode = this.dataBaseHelper.GetVlidateCode(sql, email);
            string newCode = "";

            if (this.validateCodeCache.TryGetValue(email, out string cachedCode))
            {
                newCode = cachedCode;
            }

            if (newCode.Equals(code))
            {
                return true;
            }
            return false;
        }

        public void DeleteUser(string name)
        {
            string deleteQuery = "DELETE FROM UserLogin WHERE Name = @Name";
            this.dataBaseHelper.DeleteQuery(deleteQuery, name);

        }

        public void UpdateUser(string name, string newPassword)
        {
            string updateQuery = "UPDATE UserLogin SET password = @NewPassword WHERE name = @Name";
            this.dataBaseHelper.UpdateQuery(updateQuery, name, newPassword);

        }
    }
}
