using System.Drawing;

namespace LogoGen.Settings
{
    public class LogoSettings
    {
        public LogoSettings(string svgPath, int width, int height, float scale = 1.0f, Color? backgroundColor = null,
            string backgroundImage = null, string outputPath = null, BitDepth outputBitDepth = BitDepth.Rgb24, bool saveOutputFile = false)
        {
            SvgPath = svgPath;
            Width = width;
            Height = height;
            Scale = scale;
            BackgroundImage = backgroundImage;
            BackgroundColor = backgroundColor ?? Color.White;
            OutputPath = outputPath;
            OutputBitDepth = outputBitDepth;
            SaveOutputFile = saveOutputFile;
        }

        public string SvgPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; }
        public Color BackgroundColor { get; set; }
        public string BackgroundImage{ get; set; }
        public string OutputPath { get; set; }
        public BitDepth OutputBitDepth { get; set; }
        public bool SaveOutputFile { get; set; }
    }
}