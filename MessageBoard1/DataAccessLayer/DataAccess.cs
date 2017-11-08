using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using MessageBoard1.Models;
using System.Data;

namespace MessageBoard1.DataAccessLayer {
    public class DataAccess {
        private string connStr;
        public DataAccess() {
            connStr = ConfigurationManager.ConnectionStrings["myConnStr"].ToString();
        }

        private SqlConnection GetConnection() {
            return new SqlConnection(connStr);
        }

        //新增用户
        public int SaveUser(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //往数据库中新增用户
            string cmdText = "insert into MyUser(Username,Password,PhoneNum,SignDate) values (@Username, @Password, @PhoneNum, @SignDate)";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.VarChar).Value = user.PhoneNum;
            cmd.Parameters.Add("@SignDate", SqlDbType.VarChar).Value = DateTime.Now.Date;   //获取当前日期
            int line = cmd.ExecuteNonQuery();

            conn.Close();  

            return line;
        }

        internal bool CheckAdmin(Admin admin) {
            throw new NotImplementedException();
        }

        //判断用户名是否被使用
        public bool CheckUsername(string Username) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在同名用户
            string cmdText = "select Id from MyUser where Username = @Username";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = Username;
            SqlDataReader reader = cmd.ExecuteReader();
            bool isUnique = !reader.HasRows;

            reader.Close();
            conn.Close();

            return isUnique;
        }

        //判断用户名密码是否正确
        public bool CheckUser(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在该用户
            string cmdText = "select Id from MyUser where Username = @Username and Password = @Password";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            SqlDataReader reader = cmd.ExecuteReader();
            bool result = reader.HasRows;

            reader.Close();
            conn.Close();

            return result;
        }

        //通过用户名获取所有用户能修改的信息，不包括密码(现在只有电话号码)
        public MyUser GetUserInfo(string username) {
            var conn = GetConnection();
            conn.Open();

            //根据用户名查询信息
            string cmdText = "select PhoneNum from MyUser where Username = @Username";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
            SqlDataReader reader = cmd.ExecuteReader();
            MyUser user = new MyUser();
            while (reader.Read()) {
                user.PhoneNum = reader.GetString(0);
            }
            user.Username = username;

            reader.Close();
            conn.Close();

            return user;
        }

        //修改用户信息
        public int ChangeUserInfo(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update MyUser set PhoneNum = @PhoneNum where Username = @Username";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.VarChar).Value = user.PhoneNum;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //修改用户密码
        public int ChangePassword(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update MyUser set Password = @Password where Username = @Username";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }
    }
}