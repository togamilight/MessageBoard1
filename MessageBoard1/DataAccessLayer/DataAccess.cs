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
            string cmdText = "insert into MyUser(Username,Password,PhoneNum,SignDate) values (@Username, @Password, @PhoneNum, @SignDate);";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.VarChar).Value = user.PhoneNum;
            cmd.Parameters.Add("@SignDate", SqlDbType.Date).Value = DateTime.Now.Date;   //获取当前日期
            int line = cmd.ExecuteNonQuery();

            conn.Close();  

            return line;
        }

        //判断用户名是否被使用
        public bool CheckUsername(string Username) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在同名用户
            string cmdText = "select Id from MyUser where Username = @Username;";
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

            //查询是否存在该用户名和密码
            string cmdText = "select Id from MyUser where Username = @Username and Password = @Password;";
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
            string cmdText = "select PhoneNum from MyUser where Username = @Username;";
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

            //修改（手机号码）
            string cmdText = "update MyUser set PhoneNum = @PhoneNum where Username = @Username;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.VarChar).Value = user.PhoneNum;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //修改用户密码
        public int ChangeUserPassword(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update MyUser set Password = @Password where Username = @Username;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //判断管理员的用户名密码是否正确
        public bool CheckAdmin(Admin admin) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在该管理员
            string cmdText = "select Id from Admin where AdminName = @AdminName and Password = @Password;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.VarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = admin.Password;
            SqlDataReader reader = cmd.ExecuteReader();
            bool result = reader.HasRows;

            reader.Close();
            conn.Close();

            return result;
        }

        //修改管理员密码
        public int ChangeAdminPassword(Admin admin) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update Admin set Password = @Password where AdminName = @AdminName;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.VarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = admin.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //新增管理员
        public int SaveAdmin(Admin admin) {
            var conn = GetConnection();
            conn.Open();

            //往数据库中新增管理员
            string cmdText = "insert into Admin(AdminName,Password) values (@AdminName, @Password);";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.VarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = admin.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();

            return line;
        }

        //判断用户名是否被使用
        public bool CheckAdminName(string AdminName) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在同名管理员
            string cmdText = "select Id from Admin where AdminName = @AdminName;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.VarChar).Value = AdminName;
            SqlDataReader reader = cmd.ExecuteReader();
            bool isUnique = !reader.HasRows;

            reader.Close();
            conn.Close();

            return isUnique;
        }

        //从reader中读取User数据组成List
        public List<MyUser> GetUserList(SqlDataReader reader) {
            List<MyUser> users = new List<MyUser>();
            //先获取每个属性的列序号
            int c1 = reader.GetOrdinal("Id");
            int c2 = reader.GetOrdinal("Username");
            int c3 = reader.GetOrdinal("Password");
            int c4 = reader.GetOrdinal("PhoneNum");
            int c5 = reader.GetOrdinal("SignDate");
            //获取属性生成MyUser对象，加入List
            while (reader.Read()) {
                MyUser user = new MyUser() {
                    Id = (int)reader[c1],
                    Username = (string)reader[c2],
                    Password = (string)reader[c3],
                    PhoneNum = (string)reader[c4],
                    SignDate = (DateTime)reader[c5]
                };
                users.Add(user);
            }
            return users;
        }

        //查询所有用户信息，组成List
        public List<MyUser> GetAllUserList() {
            var conn = GetConnection();
            conn.Open();

            //查询所有MyUser
            string cmdText = "select * from MyUser;";
            var cmd = new SqlCommand(cmdText, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            var users = GetUserList(reader);

            reader.Close();
            conn.Close();

            return users;
        }

        //删除用户
        public int DeleteUser(int id) {
            var conn = GetConnection();
            conn.Open();

            //根据id删除用户
            string cmdText = "delete from MyUser where Id=@Id;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int line = cmd.ExecuteNonQuery();

            conn.Close();

            return line;
        }

        //管理员版本的修改用户信息(可修改用户名，密码和手机号码)
        public int ChangeUserInfoByAdmin(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update MyUser set Username = @Username, Password = @Password, PhoneNum = @PhoneNum where Id = @Id;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.VarChar).Value = user.PhoneNum;
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = user.Id;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }
    }
}