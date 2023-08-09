using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  class DeliverData
    {
        private List<double> myData = new List<double>();
        private object lockObject = new object();

        public DeliverData() { }
        public string SelectItem { get; set; }
        public List<Point> PolygonPoints { get; set; }
        public string AreaNameLast { get; set; }
        public List<double> AreaMaxTempratures { 
            get
            {
                lock (lockObject)
                {
                    return myData;
                }
            }
            set
            {
                lock (lockObject)
                {
                    myData = value;
                }
            }
        }
        public double ImageMaxTemprature { get; set; }

    }
}
