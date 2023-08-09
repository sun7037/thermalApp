using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLL
{
    public class DrawMeasureRectangle
    {
      public DrawMeasureRectangle(){}
        /// <summary>
        /// 绘制矩形命令
        /// </summary>
        public void DrawRectangle(Object contrle, PaintEventArgs e,Color color,int PenSize,int locationX,int locationY,int height,int width)
        {
            Pen pen = new Pen(color, PenSize);
            e.Graphics.DrawRectangle(pen, locationX, locationY, width, height);
        }
        public void DrawPolygon(Object contrle, PaintEventArgs e, Color color, int PenSize, List<Point> polygon) {
            Pen pen = new Pen(color, PenSize);
            e.Graphics.DrawLines(pen, polygon.ToArray());
        }
        
    }
}
