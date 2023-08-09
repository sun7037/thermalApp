using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using PreviewDemo;
using Common;
using System.Drawing;
using System.Windows.Forms;
using System.Data.Common;

namespace DAL
{
    public class getAreaValue
    {
        bool isCapible;
        public CHCNetSDK.NET_DVR_XML_CONFIG_INPUT stdConfigInput;
        public CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT stdConfigOutput;
        public CHCNetSDK.NET_DVR_ACS_EVENT_CFG a;
        LagalJudge lagalJudge=new LagalJudge();
        public void getOptris_RectangeAreavalue(ThermalPaletteImage images,int row_start, int column_start, int rows, int columns, double amplify_x,double amplify_y,out double Area_max, out double Area_min, out int max_x, out int max_y, out int min_x, out int min_y)
        {
            Area_max = 0;
            Area_min = 50000;
            min_x = 0;
            min_y = 0;
            max_x = 0;
            max_y = 0;
            row_start = (int)(row_start / amplify_y);
            column_start = (int)(column_start / amplify_x);
            rows = (int)(rows / amplify_y)-5;
            columns = (int)(columns / amplify_x)-5;
            for (int row = row_start; row < row_start + rows; row++)
            {
                for (int column = column_start; column < column_start + columns; column++)
                {
                    ushort value = images.ThermalImage[row, column];
                    if (value > Area_max)
                    {
                        Area_max = value;
                        max_x = column;
                        max_y = row;
                    }

                    if (value > 10000)
                    {
                        if (value < Area_min)
                        {
                            Area_min = value;
                            min_x = column;
                            min_y = row;
                        }
                    }
                }

            }
            Area_max = (Area_max - 1000.0) / 10.0;
            Area_min = (Area_min - 1000.0) / 10.0;
            max_x = (int)(max_x * amplify_x);
            max_y = (int)(max_y * amplify_y);
            min_x= (int)(min_x * amplify_x);
            min_y= (int)(min_y * amplify_y);

        }
        //获取手动温度范围的内部信息
        public void getOptris_HandleAreaValue(List<Point> polygon, ThermalPaletteImage images, double amplify_x, double amplify_y, out double Area_max, out double Area_min, out int max_x, out int max_y, out int min_x, out int min_y) { 
            int up=5000,down=0,left=5000,right=0;
            int startX, startY, height, width;
            Area_max = 0;
            Area_min = 50000;
            min_x = 0;
            min_y = 0;
            max_x = 0;
            max_y = 0;

            foreach (Point p in polygon)
            {
                if(p.Y<up) up = p.Y;
                if(p.Y>down) down = p.Y;
                if (p.X<left)left = p.X;
                if(p.X>right) right = p.X;
            }
            
            startX = (int)(left / amplify_x); 
            startY= (int)(up / amplify_y);
            height = (int)((down - up) / amplify_y);
            width = (int)((right - left) / amplify_x);
            for (int row = startY; row < startY + height-5; row++)
            {
                for (int column = startX; column < startX+ width-5; column++)
                {
                    ushort value = images.ThermalImage[row, column];
                    if (value > Area_max)
                    {
                        if (lagalJudge.IsPointInside(polygon, (int)(column*amplify_x),(int)(row*amplify_y)))
                        {
                            Area_max = value;
                            max_x = column;
                            max_y = row;
                        }
                    }

                    if (value > 10000&& lagalJudge.IsPointInside(polygon, (int)(column * amplify_x), (int)(row * amplify_y)))
                    {
                        if (value < Area_min)
                        {
                            Area_min = value;
                            min_x = column;
                            min_y = row;
                        }
                    }
                }
            }
            Area_max = (Area_max - 1000.0) / 10.0;
            Area_min = (Area_min - 1000.0) / 10.0;
            max_x = (int)(max_x * amplify_x);
            max_y = (int)(max_y * amplify_y);
            min_x = (int)(min_x * amplify_x);
            min_y = (int)(min_y * amplify_y);
        }
        public int getMouseValue(ThermalPaletteImage images,int x,int y) {
            ushort value = images.ThermalImage[y, x];
            return value;
        }
        public void getHikmicro_AreaValue() {
            stdConfigInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
            stdConfigOutput=new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();

            HiLoginInfo himicroLoginData = new HiLoginInfo();
            CHCNetSDK.NET_DVR_STDXMLConfig(himicroLoginData.userId, ref stdConfigInput,ref stdConfigOutput);

            //CHCNetSDK.NET_DVR_GetSTDConfig(himicroLoginData.userId,ref)
            
        }
    }
}
