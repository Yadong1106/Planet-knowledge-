using System.Runtime.Serialization;

namespace ChatApp.API.Untity
{
    public class User
    {
        private string? _name;
        private string? _password;
        private string? _confirmPassword;
        private string? _email;
        private string? _validateCode;

        [DataMember(Name = "name")]
        public String Name 
        { 
            get { return _name ?? ""; }
            set { _name = value; }
        }

        [DataMember(Name = "password")]
        public String PassWord
        { 
            get { return _password ?? ""; } 
            set { _password = value; }
        }

        [DataMember(Name = "confirmPassword")]
        public String ConfirmPassword
        {
            get { return _confirmPassword ?? ""; }
            set { _confirmPassword = value; }
        }

        [DataMember(Name = "email")]
        public String Email
        { get { return _email ?? ""; } 
            set {  _email = value; } 
        }

        [DataMember(Name = "validateCode")]
        public String ValidateCode
        {
            get { return _validateCode ?? ""; }
            set { _validateCode = value; }
        }
    }
}
