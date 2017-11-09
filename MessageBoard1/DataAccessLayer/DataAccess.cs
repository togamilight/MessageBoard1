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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = user.Password;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.NVarChar).Value = user.PhoneNum;
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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = Username;
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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = user.Password;
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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.NVarChar).Value = user.PhoneNum;
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
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = user.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //保存用户写的新留言
        public int SaveMessage(Message msg) {
            var conn = GetConnection();
            conn.Open();

            //往数据库中新增用户
            string cmdText = "insert into Message(Username,Title,Content,DateTime) values (@Username, @Title, @Content, @DateTime);";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = msg.Username;
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = msg.Title;
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = msg.Content;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = DateTime.Now;   //获取当前日期时间
            int line = cmd.ExecuteNonQuery();

            conn.Close();

            return line;
        }

        //从reader中读取Message除内容外的数据组成List
        public List<Message> GetMsgTitleList(SqlDataReader reader) {
            List<Message> msgs = new List<Message>();
            //先获取每个属性的列序号
            int c1 = reader.GetOrdinal("Id");
            int c2 = reader.GetOrdinal("Username");
            int c3 = reader.GetOrdinal("Title");
            int c4 = reader.GetOrdinal("IsPublic");
            int c5 = reader.GetOrdinal("DateTime");
            int c6 = reader.GetOrdinal("ReplyNum");
            int c7 = reader.GetOrdinal("NewReply");
            //获取属性生成Message对象，加入List
            while (reader.Read()) {
                Message msg = new Message() {
                    Id = (int)reader[c1],
                    Username = (string)reader[c2],
                    Title = (string)reader[c3],
                    IsPublic = (bool)reader[c4],
                    DateTime = (DateTime)reader[c5],
                    ReplyNum = (int)reader[c6],
                    NewReply = (int)reader[c7],
                };
                msgs.Add(msg);
            }
            return msgs;
        }

        //查询所有用户信息，组成List
        public List<Message> GetUserMsgTitleList(string username) {
            var conn = GetConnection();
            conn.Open();

            //查询用户所有留言, 并排序
            string cmdText = "select Id, Username, Title, IsPublic, DateTime, ReplyNum, NewReply from Message where Username = @Username order by IsPublic, DateTime desc;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
            SqlDataReader reader = cmd.ExecuteReader();
            var users = GetMsgTitleList(reader);

            reader.Close();
            conn.Close();

            return users;
        }

        //判断管理员的用户名密码是否正确
        public bool CheckAdmin(Admin admin) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在该管理员
            string cmdText = "select Id from Admin where AdminName = @AdminName and Password = @Password;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.NVarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = admin.Password;
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
            cmd.Parameters.Add("@AdminName", SqlDbType.NVarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = admin.Password;
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
            cmd.Parameters.Add("@AdminName", SqlDbType.NVarChar).Value = admin.AdminName;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = admin.Password;
            int line = cmd.ExecuteNonQuery();

            conn.Close();

            return line;
        }

        //判断管理员用户名是否被使用
        public bool CheckAdminName(string AdminName) {
            var conn = GetConnection();
            conn.Open();

            //查询是否存在同名管理员
            string cmdText = "select Id from Admin where AdminName = @AdminName;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@AdminName", SqlDbType.NVarChar).Value = AdminName;
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

        //搜索用户
        public List<MyUser> SearchUserList(string keyWord) {
            var conn = GetConnection();
            conn.Open();

            //根据关键词查询用户查询
            string cmdText = "select * from MyUser where Username like '%" + keyWord + "%';" ;
            var cmd = new SqlCommand(cmdText, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            var users = GetUserList(reader);

            reader.Close();
            conn.Close();

            return users;
        }

        //删除用户
        public int DeleteUser(string username) {
            var conn = GetConnection();
            conn.Open();
            //开启一个事务
            SqlTransaction trans = conn.BeginTransaction();
            var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            try {
                //删除管理员对这个用户的所有回复
                cmd.CommandText = "delete from Reply where MessageId in (select Id from Message where Username = @Username);";
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                cmd.ExecuteNonQuery();
                
                //删除这个用户的所有留言 
                cmd.CommandText = "delete from Message where Username = @Username;";
                cmd.ExecuteNonQuery();

                //根据id删除用户
                cmd.CommandText = "delete from MyUser where Username = @Username;";
                int line = cmd.ExecuteNonQuery();

                //提交事务
                trans.Commit();

                return line;
            }
            catch (Exception) {
                //当遇到异常时回滚事务
                trans.Rollback();
                throw;
            }
            finally {
                conn.Close();
            }
            ////根据id删除用户
            //string cmdText = "delete from MyUser where Id=@Id;";
            //var cmd = new SqlCommand(cmdText, conn);
            //cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            //int line = cmd.ExecuteNonQuery();
        }

        //删除留言
        public int DeleteMessage(int MsgId) {
            var conn = GetConnection();
            conn.Open();
            //开启一个事务
            SqlTransaction trans = conn.BeginTransaction();
            var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            try {
                //更新用户的新回复数（减去要删除的留言的新回复数）
                cmd.CommandText = "update MyUser set NewReply = NewReply - (select NewReply from Message where Id = @MsgId) where Username in (select Username from Message where Id = @MsgId);";
                cmd.Parameters.Add("@MsgId", SqlDbType.Int).Value = MsgId;
                cmd.ExecuteNonQuery();

                //删除管理员对这个留言的所有回复
                cmd.CommandText = "delete from Reply where MessageId = @MsgId;";
                cmd.ExecuteNonQuery();

                //删除这个留言 
                cmd.CommandText = "delete from Message where Id = @MsgId;";
                int line = cmd.ExecuteNonQuery();

                //提交事务
                trans.Commit();

                return line;
            }
            catch (Exception) {
                //当遇到异常时回滚事务
                trans.Rollback();
                throw;
            }
            finally {
                conn.Close();
            }
        }

        //管理员版本的修改用户信息(可修改用户名，密码和手机号码)
        public int ChangeUserInfoByAdmin(MyUser user) {
            var conn = GetConnection();
            conn.Open();

            //修改
            string cmdText = "update MyUser set Username = @Username, Password = @Password, PhoneNum = @PhoneNum where Id = @Id;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = user.Password;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.NVarChar).Value = user.PhoneNum;
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = user.Id;
            int line = cmd.ExecuteNonQuery();

            conn.Close();
            return line;
        }

        //根据Id获取Message，包括相应的回复
        public Message GetMessage(int Id) {
            var conn = GetConnection();
            conn.Open();

            //查询留言
            string cmdText = "select * from Message where Id = @Id;";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            SqlDataReader reader = cmd.ExecuteReader();
            Message msg = new Message();
            while (reader.Read()) {
                msg.Id = (int)reader["Id"];
                msg.Username = (string)reader["Username"];
                msg.Title = (string)reader["Title"];
                msg.Content = (string)reader["Content"];
                msg.IsPublic = (bool)reader["IsPublic"];
                msg.DateTime = (DateTime)reader["DateTime"];
                msg.ReplyNum = (int)reader["ReplyNum"];
                msg.NewReply = (int)reader["NewReply"];
            }
            reader.Close();

            //查询留言对应的所有回复
            string cmdText1 = "select * from Reply where MessageId = @MessageId;";
            var cmd1 = new SqlCommand(cmdText1, conn);
            cmd1.Parameters.Add("@MessageId", SqlDbType.Int).Value = Id;
            List<Reply> replies = new List<Reply>();
            SqlDataReader reader1 = cmd1.ExecuteReader();
            int c1 = reader1.GetOrdinal("Id");
            int c2 = reader1.GetOrdinal("MessageId");
            int c3 = reader1.GetOrdinal("AdminName");
            int c4 = reader1.GetOrdinal("Content");
            int c5 = reader1.GetOrdinal("DateTime");
            while(reader1.Read()){
                Reply reply = new Reply() {
                    Id = (int)reader1[1],
                    MessageId = (int)reader1[2],
                    AdminName = (string)reader1[3],
                    Content = (string)reader1[4],
                    DateTime = (DateTime)reader1[5]
                };
                replies.Add(reply);
            }
            reader1.Close();
            conn.Close();

            msg.Replies = replies;
            return msg;
        }
    }
}