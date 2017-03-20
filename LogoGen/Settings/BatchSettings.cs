using System.Drawing;

namespace LogoGen.Settings
{
    public class BatchSettings
    {
        public BatchSettings(string svgPath, float scale, Color backgroundColor, bool saveOutputFiles,
            BatchItemSettings[] itemSettings)
        {
            SvgPath = svgPath;
            Scale = scale;
            BackgroundColor = backgroundColor;
            SaveOutputFiles = saveOutputFiles;
            ItemSettings = itemSettings;
        }

        public string SvgPath { get; }
        public float Scale { get; }
        public Color BackgroundColor { get; }
        public bool SaveOutputFiles { get; }
        public BatchItemSettings[] ItemSettings { get; }
    }
}