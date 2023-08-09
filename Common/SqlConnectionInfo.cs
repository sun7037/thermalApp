using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SqlConnectionInfo
    {
        public SqlConnectionInfo() { }   
        public string SQlSrever { get; set; }
        public string SQLUser { get; set; }
        public string SQLPassword { get; set; }
        public string SQLDatabase { get; set; }
    }
}
