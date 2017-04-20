using System.Drawing;

namespace LogoGen.Settings
{
    public class BatchSettings
    {
        public BatchSettings(string svgPath, float scale, Color backgroundColor, string backgroundImage,
            bool saveOutputFiles, BitDepth outputBitDepth, BatchItemSettings[] itemSettings)
        {
            SvgPath = svgPath;
            Scale = scale;
            BackgroundColor = backgroundColor;
            BackgroundImage = backgroundImage;
            SaveOutputFiles = saveOutputFiles;
            OutputBitDepth = outputBitDepth;
            ItemSettings = itemSettings;
        }

        public string SvgPath { get; }
        public float Scale { get; }
        public Color BackgroundColor { get; }
        public string BackgroundImage { get; }
        public bool SaveOutputFiles { get; }
        public BitDepth OutputBitDepth { get; }
        public BatchItemSettings[] ItemSettings { get; }
    }
}