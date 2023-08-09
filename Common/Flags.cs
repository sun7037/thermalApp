using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Flags
    {
        public Flags() { }
        public bool IsGrabeImages { get; set; }//是否在获取最新图片
        public bool IsDrawingPolygon { get; set;}//是否正在画框
        public bool IsRestartDrawing { get; set; }//是否重新开始了一次画框
        public bool IsMeasuring { get; set; }//是否正在测量某个线框区域的值
        public bool IsFirstScan { get; set; }//判断是否为初始运行
        public bool IsAreasChange { get; set; }//判断区域是否发生了改变
        public bool IsThreadOpen { get; set; }//判断线程是否开启

    }
}
