using ChatApp.API.Untity;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Xml.Linq;
using YamlDotNet.Serialization;

namespace ChatApp.API.Dao
{
    class Config
    {
        public string ConnectionString { get; set; }

        public Config(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }

    public class DataBaseHelper
    {
        private SqlConnection connection;
        private string yamlFilePath = "C:\\Projects\\ChatApp.API\\ChatApp.API\\config.yaml";

        public DataBaseHelper()
        {
            var connectionString = this.GetSqlServeConfig(yamlFilePath);
            //var connectionString = "Data Source=127.0.0.1,1433;Initial Catalog=ChatAppDB;User ID=sa;Pwd=19971106Zlt@;";
            this.connection = new SqlConnection(connectionString);
        }

        public DataTable ExecuteQuery(string sqlQuery)
        {
            DataTable dt = new DataTable();

            this.connection.Open();

            using (SqlCommand command = new SqlCommand(sqlQuery, this.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            this.connection.Close();

            return dt;
        }

        public void StoreVerificationCodeInDatabase(string sqlValidateCode, string email, string code)
        {
            this.connection.Open();
            using (SqlCommand command = new SqlCommand(sqlValidateCode, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@VerificationCode", code);
                command.ExecuteNonQuery();
            }
            this.connection.Close();
        }
        public void InsertQuery(string insertQuery, User user)
        {
            
            this.connection.Open();
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Password", user.PassWord);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.ExecuteNonQuery();
            }
            this.connection.Close();
        }

        public void DeleteQuery(string deleteQuery, string name) 
        {
            this.connection.Open();
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
            this.connection.Close();
        }

        public void DeleteVlidateCode()
        {
            string sql = "DELETE FROM EmailVerificationTable WHERE DATEDIFF(MINUTE, CreatedAt, GETDATE()) > 30";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public string GetVlidateCode(string sql, string email)
        {
            DataTable dt = new DataTable();
            this.connection.Open();
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            this.connection.Close();

            if (dt != null)
            {
                return dt.ToString();
            }
            else
            {
                return "";
            }
        }

        public void UpdateQuery(string updateQuery, string name, string newPassword)
        {
            this.connection.Open();
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@NewPassword", newPassword);
                command.ExecuteNonQuery();
            }
            this.connection.Close();
        }

        public string GetSqlServeConfig(string yamlFilePath) 
        {
            string yamlContent = File.ReadAllText(yamlFilePath);

            var deserializer = new DeserializerBuilder().Build(); ; // DeserializerBuilder
            var config = new Config(yamlContent);
            //var deserializedConfig = deserializer.Deserialize<Config>(yamlContent);

            return config.ConnectionString;
        }

    }

}
