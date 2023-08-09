using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL {

    /// <summary>
    /// 容器用于热成像和假彩色调色板图像。
    /// </summary>
    public class ThermalPaletteImage {

        /// <summary>
        /// 容器用于热成像和假彩色调色板图像.
        /// </summary>
        /// <param name="thermalImage">The thermal image</param>
        /// <param name="paletteImage">The palette image</param>
        public ThermalPaletteImage(ushort[,] thermalImage, Bitmap paletteImage, EvoIRFrameMetadata irFrameMetadata) {
            ThermalImage = thermalImage;
            PaletteImage = paletteImage;
            IRFrameMetadata = irFrameMetadata;
        }



        /// <summary>
        ///  热成像图像的访问器。
        ///  按照以下方式执行转换为温度值:
        ///  t = ((double)data[row,column] - 1000.0) / 10.0
        /// </summary>
        /// <returns>Thermal Image as ushort[height, width]</returns>
        public ushort[,] ThermalImage { get; private set; }

        /// <summary>
        /// Accessor to false color coded palette image
        /// </summary>
        /// <returns>RGB palette image</returns>
        public Bitmap PaletteImage { get; private set; }


        /// <summary>
        /// Accessor to ir frame metadata
        /// </summary>
        /// <returns>IR frame metadata</returns>
        public EvoIRFrameMetadata IRFrameMetadata { get; private set; }
    }
}
