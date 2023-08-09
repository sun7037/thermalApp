using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LagalJudge
    {
        public LagalJudge() { }
        public bool IsPolygonLagal(List<Point> point) {
            
            return false;
        }
        //判断点是否在多边形内部算法
        public bool IsPointInside(List<Point> polygon,int x,int y) {
            int numVertices = polygon.Count;
            int intersectionCount = 0;
            for (int i = 0; i < numVertices-1; i++)
            {
                Point p1 = polygon[i];
                Point p2 = polygon[(i + 1)];

                if ((p1.Y > y) != (p2.Y > y) &&
                    x < ((p2.X - p1.X) * (y - p1.Y) / (p2.Y - p1.Y) + p1.X))
                {
                    intersectionCount++;
                } 
            }
            return intersectionCount % 2 == 1;
            //int count = 0;

            //for (int i = 0, j = numVertices - 1; i < numVertices; j = i++)
            //{
            //    if (((polygon[i].Y > y) != (polygon[j].Y > y)) &&
            //        (x < (polygon[j].X - polygon[i].X) * (y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
            //    {
            //        count++;
            //    }
            //}

            //return count % 2 == 1;
        }
    }
}
