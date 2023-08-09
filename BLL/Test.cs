using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace BLL
{
    public class Test
    {
        //ControlSize controlSize=new ControlSize();
        test01 test01=new test01();
        public Test() { }
        public void getValue() {
            test01.setValue();
            int value = test01.controlSize.ImagePannelHeight;
            if (value == 0) {
                MessageBox.Show("数值传输失败");
            }
            else
            {
                MessageBox.Show("数值传输成功");
            }
        }
    }
}
