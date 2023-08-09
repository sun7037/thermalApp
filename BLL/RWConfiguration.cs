using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace BLL
{
    public class RWConfiguration
    {
        string configFilePath = Path.Combine(Application.StartupPath, "record.config");
        public RWConfiguration() {
        }
        public void SaveAreaName(string name) {
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement grand=child.Element("AreaName");
            XElement newone = new XElement("name",name);
            grand.Add(newone);
            root.Save(configFilePath);
        }
        /// <summary>
        /// 向配置文件写入设置区域数据
        public void WriteAreaConfiguration(string name,string database,string X,string Y,string width,string height,string emission) {
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement TextBoxValue = new XElement(name);
            TextBoxValue.SetElementValue("名称", name);
            TextBoxValue.SetElementValue("数据库表名", database);
            TextBoxValue.SetElementValue("X轴", X);
            TextBoxValue.SetElementValue("Y轴", Y);
            TextBoxValue.SetElementValue("宽", width);
            TextBoxValue.SetElementValue("高", height);
            TextBoxValue.SetElementValue("发射率", emission);
            child.Add(TextBoxValue);
            root.Save(configFilePath);
        }
        //写入手动区域配置
        public void WriteHandleAreaConfiguration(string name,string database,string color,string size,string LabelColor,List<Point> polygon) {
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement AreaName = new XElement(name);
            AreaName.SetElementValue("名称", name);
            AreaName.SetElementValue("数据库表名", database);
            AreaName.SetElementValue("框颜色",color);
            AreaName.SetElementValue("框线尺寸", size);
            AreaName.SetElementValue("标签颜色", LabelColor);
            foreach (Point p in polygon)
            {
                XElement position = new XElement("point");
                position.SetElementValue("X", p.X);
                position.SetElementValue("Y", p.Y);
                AreaName.Add(position);
            }
            child.Add(AreaName);
            root.Save(configFilePath);
        }
        //读取手动区域配置文件
        public void ReadHandleAreaConfiguration(string name, out string database,out Color color,out int size,out Color labelColor, out List<Point> polygon)
        {
            int X, Y;
            database = null;
            polygon = new List<Point>();
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement grand = child.Element(name);
            database= grand.Element("数据库表名").Value;
            color=Color.FromName( grand.Element("框颜色").Value);
            int.TryParse(grand.Element("框线尺寸").Value, out size);
            labelColor=Color.FromName(grand.Element("标签颜色").Value);
            var list = grand.Elements();
            foreach (XElement xElement in list) {
                string Localname = xElement.Name.LocalName;
                if (Localname == "point"){
                    int.TryParse(xElement.Element("X").Value,out X);
                    int.TryParse(xElement.Element("Y").Value, out Y);
                    Point point = new Point(X,Y);
                    polygon.Add(point);
                }
                
            }
            
        }
        public void ReadNames(out List<string> names) {
            names = new List<string>();
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement grand = child.Element("AreaName");
            var list= grand.Elements();
            foreach (XElement xElement in list) {
                names.Add(xElement.Value);
            }
        }
        /// <summary>
        /// 读取配置文件的测量区域数据
        /// </summary>
        public void ReadAreaConfiguration(string name,out string areaname, out string database, out int X, out int Y, out int width, out int height, out int emission) {
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement grand = child.Element(name);
            if (grand != null)
            {
                areaname = grand.Element("名称").Value;
                int.TryParse(grand.Element("发射率").Value, out emission);
                int.TryParse(grand.Element("X轴").Value, out X);
                int.TryParse(grand.Element("Y轴").Value, out Y);
                int.TryParse(grand.Element("宽").Value, out width);
                int.TryParse(grand.Element("高").Value, out height);
                database = grand.Element("数据库表名").Value;
            }
            else {
                areaname = "";
                X = 0;
                Y= 0;
                width = 0;
                height = 0;
                emission = 0;
                database = "";
            }
        }
        /// <summary>
        /// 移除区域的配置信息
        /// </summary>
        /// <param name="remove"></param>
        public void RemoveAreaConfigration(string remove) {
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            XElement grand = child.Element(remove);
            if(grand!=null)
            grand.Remove();

        }
        /// <summary>
        /// 判断测量区域信息是否出现重复
        /// </summary>
        /// <param name="rootName"></param>
        /// <param name="reserch"></param>
        /// <returns></returns>
        public bool isRepeat(string rootName,string reserch) {
            bool flag=false;
            XDocument document = XDocument.Load(configFilePath);
            XElement root = document.Root;
            XElement child = root.Element("areas");
            var list = child.Elements();
            foreach (XElement element in list)
            {
                string localName = element.Name.LocalName;
                XElement grand = child.Element(localName);
                string rootValue = grand.Element(rootName).Value;
                if (rootValue == reserch)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

    }
}
