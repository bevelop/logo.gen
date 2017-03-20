using System.Drawing;

namespace LogoGen.Settings
{
    public class BatchItemSettings
    {
        public BatchItemSettings(int width, int height, string outputPath, float? scale = null, Color? backgroundColor = null)
        {
            Width = width;
            Height = height;
            OutputPath = outputPath;
            Scale = scale;
            BackgroundColor = backgroundColor;
        }

        public int Width { get; set;  }
        public int Height { get; set; }
        public string OutputPath { get; set; }
        public float? Scale { get; set; }
        public Color? BackgroundColor { get; set; }
    }
}