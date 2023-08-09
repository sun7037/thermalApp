using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  class ControlSize
    {
        public int height;
        public int getHeight() { 
            return height;
        }
        public void setHeight(int x) { 
            height = x;
        }
        public int Screenwidth { get; set; }
        public int Screenheight { get; set; }
        public int ImagePannelWidth { get; set; }
        public int ImagePannelHeight { get; set; }
        public int WorkSpaceWidth { get; set; }
        public int WorkSpaceHeight { get; set;}
    }
}
