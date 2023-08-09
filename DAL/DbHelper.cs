using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DbHelper
    {
        private string connectionString; // 数据库连接字符串

        public DbHelper(string server, string database, string username, string password)
        {
            // 构建连接字符串
            connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
        }

        public MySqlConnection GetConnection()
        {

            // 创建数据库连接对象
            return new MySqlConnection(connectionString);
        }
        //返回受影响的行数
        public void ExecuteNonQuery(string query)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }
        //执行查询，并返回第一行第一列的值
        public object ExecuteScalar(string query)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                return command.ExecuteScalar();
            }
        }
        //读取选中的内容
        public MySqlDataReader ExecuteReader(string query)
        {
            MySqlConnection connection = GetConnection();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            return command.ExecuteReader();
        }

    }
}
