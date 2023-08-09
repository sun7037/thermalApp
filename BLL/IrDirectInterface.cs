using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BLL
{

    /// <summary>
    /// OptrisColoringPalettes
    /// </summary>
    public enum OptrisColoringPalette : int
    {
        AlarmBlue = 1,
        AlarmBlueHi = 2,
        GrayBW = 3,
        GrayWB = 4,
        AlarmGreen = 5,
        Iron = 6,
        IronHi = 7,
        Medical = 8,
        Rainbow = 9,
        RainbowHi = 10,
        AlarmRed = 11
    };

    /// <summary>
    /// OptrisPaletteScalingMethodes
    /// </summary>
    public enum OptrisPaletteScalingMethod
    {
        Manual = 1,
        MinMax = 2,
        Sigma1 = 3,
        Sigma3 = 4
    };

    /// <summary>
    /// IRImager Interface for USB or TCP-Deamon connection
    /// </summary>
    public class IrDirectInterface
    {
        #region fields

        static private IrDirectInterface _instance;
        private bool _isConnected;
        private bool _isAutomaticShutterActive;
        private int _paletteWidth, _paletteHeight;
        private int _thermalWidth, _thermalHeight;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public IrDirectInterface()
        {
        }


        #endregion

        #region properties

        /// <summary>
        /// 用于访问的单例实例
        /// </summary>
        static public IrDirectInterface Instance
        {
            get { return _instance ?? (_instance = new IrDirectInterface()); }
        }

        /// <summary>
        /// 激活或禁用自动快门
        /// </summary>
        /// <exception cref="System.Exception">Thrown on error</exception>
        public bool IsAutomaticShutterActive
        {
            get { return _isAutomaticShutterActive; }
            set
            {
                if (_isAutomaticShutterActive != value)
                {
                    _isAutomaticShutterActive = value;

                    CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_shutter_mode(InstanceId, value ? 1 : 0));

                }
            }
        }

        /// <summary>
        /// 返回当前连接状态
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

        /// <summary>
        /// Returns unique instance id
        /// </summary>
        public uint InstanceId { get; private set; }

        #endregion

        #region public methodes

        /// <summary>
        /// 通过 USB 连接到此计算机
        /// </summary>
        /// <param name="xmlConfigPath">Path to xml config</param>
        /// <param name="formatsDefPath">含 formants.def 文件的文件夹路径（如果要使用默认路径，请使用 "</param>
        /// <param name="logFilePath">可以翻译为 "日志文件的路径（如果要使用默认路径，请使用 </param>
        /// <exception cref="System.Exception">在出现错误时抛出</exception>
        public void Connect(string xmlConfigPath, string formatsDefPath = "", string logFilePath = "")
        {
            if (!File.Exists(xmlConfigPath))
            {
                throw new ArgumentException("XML Config file doesn't exist: " + xmlConfigPath, nameof(xmlConfigPath));
            }

            if (formatsDefPath.Length > 0 && !File.Exists(Path.Combine(formatsDefPath, "Formats.def")))
            {
                throw new ArgumentException("该目录中不存在格式定义文件: " + formatsDefPath, nameof(formatsDefPath));
            }

            int error;
            uint camId;
            
            if ((error = IrDirectInterfaceInvoke.evo_irimager_multi_usb_init(out camId, xmlConfigPath, formatsDefPath, logFilePath ?? "")) < 0)
            {
                throw new Exception($"Error at camera init: {error}");
                
            }

            //获取热成像图像和调色板图像的尺寸
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_palette_image_size(camId, out _paletteWidth, out _paletteHeight));
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_thermal_image_size(camId, out _thermalWidth, out _thermalHeight));

            InstanceId = camId;
            _isConnected = true;
            IsAutomaticShutterActive = true;
        }


        /// <summary>
        /// 初始化到守护进程的 TCP 连接（非阻塞）
        /// </summary>
        /// <param name="hostname">守护进程所在机器的主机名或 IP 地址（'localhost' 可以被解析）</param>
        /// <param name="port">Port of daemon, default 1337</param>
        /// <exception cref="System.Exception">Thrown on error</exception>
        public void Connect(string hostname, int port)
        {
            int error;
            uint camId;

            if ((error = IrDirectInterfaceInvoke.evo_irimager_multi_tcp_init(out camId, hostname, port)) < 0)
            {
                throw new Exception($"Error at camera init: {error}");
            }

            InstanceId = camId;
            _isConnected = true;
            IsAutomaticShutterActive = true;
        }


        /// <summary>
        /// Disconnects the camera, either connected via USB or TCP
        /// </summary>
        public void Disconnect()
        {
            if (_isConnected)
            {
                CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_terminate(InstanceId));

                _isConnected = false;
            }
        }

        /// <summary>
        /// 热成像图像的访问器.
        ///  温度值的转换应按以下方式执行
        ///  t = ((double)data[row,column] - 1000.0) / 10.0
        /// </summary>
        /// <returns>Thermal Image as ushort[height, width]</returns>
        /// <exception cref="System.Exception">Thrown on error</exception>
        public Tuple<ushort[,], EvoIRFrameMetadata> GetThermalImage()
        {
            CheckConnectionState();
            ushort[,] buffer = new ushort[_thermalHeight, _thermalWidth];
            EvoIRFrameMetadata metadata;
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_thermal_image_metadata(InstanceId, out _thermalWidth, out _thermalHeight, buffer, out metadata));
            return new Tuple<ushort[,], EvoIRFrameMetadata>(buffer, metadata);
        }

        /// <summary>
        /// Accessor to false color coded palette image  访问伪彩色调色板图像的接口
        /// </summary>
        /// <returns>RGB palette image</returns>
        /// /// <exception cref="System.Exception">Thrown on error</exception>
        public Tuple<Bitmap, EvoIRFrameMetadata> GetPaletteImage()
        {
            CheckConnectionState();

            Bitmap image = new Bitmap(_paletteWidth, _paletteHeight, PixelFormat.Format24bppRgb);

            BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

            EvoIRFrameMetadata metadata;

            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_palette_image_metadata(InstanceId, out _paletteWidth, out _paletteHeight, bmpData.Scan0, out metadata));
            image.UnlockBits(bmpData);
            return new Tuple<Bitmap, EvoIRFrameMetadata>(image, metadata);
        }


        /// <summary>
        /// Accessor to false color coded palette image and thermal image from same frame  从同一帧中获取彩色图像和热图的访问器。
        /// </summary>
        /// <returns cref="ThermalPaletteImage">False color coded palette and thermal image</returns>
        /// <exception cref="System.Exception">Thrown on error</exception>
        public ThermalPaletteImage GetThermalPaletteImage()
        {
            CheckConnectionState();

            Bitmap paletteImage = new Bitmap(_paletteWidth, _paletteHeight, PixelFormat.Format24bppRgb);
            //Bitmap paletteImage = new Bitmap(400, 400, PixelFormat.Format24bppRgb);
            ushort[,] thermalImage = new ushort[_thermalHeight, _thermalWidth];

            BitmapData bmpData = paletteImage.LockBits(new Rectangle(0,0, paletteImage.Width, paletteImage.Height), ImageLockMode.WriteOnly, paletteImage.PixelFormat);

            EvoIRFrameMetadata metadata;

            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_thermal_palette_image_metadata(
                InstanceId,
                _thermalWidth, _thermalHeight, thermalImage,
                paletteImage.Width, paletteImage.Height, bmpData.Scan0,
                out metadata));

            paletteImage.UnlockBits(bmpData);

            return new ThermalPaletteImage(thermalImage, paletteImage, metadata);
        }

        /// <summary>
        /// Sets palette format and scaling method.
        /// </summary>
        /// <param name="format">Palette format</param>
        /// <param name="scale">Scaling method</param>
        public void SetPaletteFormat(OptrisColoringPalette format, OptrisPaletteScalingMethod scale)
        {
            CheckConnectionState();

            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_palette(InstanceId, (int)format));
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_palette_scale(InstanceId, (int)scale));
        }

        /// <summary>
        /// Only available in OptrisPaletteScalingMethod.Manual palette scale mode. Sets palette manual scaling temperature range.
        /// </summary>
        /// <param name="minTemp">Minimum temperature</param>
        /// <param name="maxTemp">Maximum temperature</param>
        public void SetPaletteManualTemperatureRange(float minTemp, float maxTemp)
        {
            CheckConnectionState();
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_palette_manual_temp_range(InstanceId, minTemp, maxTemp));
        }

        /// <summary>
        /// Sets the minimum and maximum temperature range to the camera (also configurable in xml-config)
        /// </summary>
        /// <param name="tMin"></param>
        /// <param name="tMax"></param>
        public void SetTemperatureRange(int tMin, int tMax)
        {
            CheckConnectionState();
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_temperature_range(InstanceId, tMin, tMax));
        }

        /// <summary>
        /// Triggers a shutter flag cycle
        /// </summary>
        public void TriggerShutterFlag()
        {
            CheckConnectionState();
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_trigger_shutter_flag(InstanceId));
        }

        /// <summary>
        /// sets radiation properties, i.e. emissivity and transmissivity parameters (not implemented for TCP connection, usb mode only)
        /// </summary>
        /// <param name="emissivity">emissivity emissivity of observed object [0;1]</param>
        /// <param name="transmissivity">transmissivity transmissivity of observed object [0;1]</param>
        /// <param name="tAmbient">tAmbient ambient temperature, setting invalid values (below -273,15 degrees) forces the library to take its own measurement values.</param>
        public void SetRadiationParameters(float emissivity, float transmissivity, float tAmbient = -999.0f)
        {
            if (emissivity < 0 || emissivity > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(emissivity), "Valid range is 0..1");
            }
            if (transmissivity < 0 || transmissivity > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(transmissivity), "Valid range is 0..1");
            }

            CheckConnectionState();
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_radiation_parameters(InstanceId, emissivity, transmissivity, tAmbient));
        }

        /// <summary>
        /// 设置裁剪格式的位置。仅对 PI1M，具有 x=0 的位置有效.
        /// </summary>
        /// <param name="point">裁剪区域的左上角 x 和 y 坐标位置</param>
        public void SetClippedFormaPosition(Point point)
        {
            CheckConnectionState();

            if (point.X < 0 || point.X > _thermalWidth || point.X < 0 || point.X > _thermalHeight)
            {
                throw new ArgumentOutOfRangeException(nameof(point), $"X and Y values must be within the thermal resolution of {_thermalWidth}x{_thermalHeight}");
            }

            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_set_clipped_format_position(InstanceId, (ushort)point.X, (ushort)point.Y));
        }

        /// <summary>
        /// 获取裁剪格式的位置。如果之前没有设置，则返回 uint max（即 uint 类型的最大值）.
        /// </summary>
        /// <returns>裁剪区域的左上角 x 和 y 坐标位置。如果之前没有设置，则返回 uint max（即 uint 类型的最大值）.</returns>
        public Point GetClippedFormatPosition()
        {
            CheckConnectionState();
            CheckResult(IrDirectInterfaceInvoke.evo_irimager_multi_get_clipped_format_position(InstanceId, out ushort x, out ushort y));
            return new Point(x, y);
        }

        #endregion

        #region private methodes

        private void CheckResult(int result)
        {
            if (result < 0)
            {
                throw new Exception($"内部摄像头出现错误: {result}");
            }
        }
        //检查摄像头连接状态
        private void CheckConnectionState()
        {
            if (!_isConnected)
            {
                    throw new Exception($"摄像头未连接，请先连接。");
            }
        }

        #endregion

        ~IrDirectInterface()
        {
            if (_isConnected)
            {
                Disconnect();
            }
        }
    }
}
