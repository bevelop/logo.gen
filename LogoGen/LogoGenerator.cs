using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using LogoGen.Results;
using LogoGen.Settings;
using Newtonsoft.Json;
using Svg;

namespace LogoGen
{
    public class LogoGenerator
    {
        public Bitmap Generate(LogoSettings settings)
        {
            var svg = SvgDocument.Open(settings.SvgPath);

            var logoSize = GetLogoSize(svg, settings);
            var logoImage = svg.Draw(logoSize.Width, logoSize.Height);

            var finalLogo = new Bitmap(settings.Width, settings.Height, PixelFormat.Format32bppArgb);
            using (var gfx = Graphics.FromImage(finalLogo))
            using (var brush = new SolidBrush(settings.BackgroundColor))
            {
                gfx.FillRectangle(brush, 0, 0, settings.Width, settings.Height);

                var midX = (finalLogo.Width - logoImage.Width) / 2;
                var midY = (finalLogo.Height - logoImage.Height) / 2;
                gfx.DrawImage(logoImage, midX, midY);
            }

            if (settings.SaveOutputFile)
            {
                finalLogo.Save(settings.OutputPath, ImageFormat.Png);
            }

            return finalLogo;
        }

        public BatchResult[] GenerateBatch(BatchSettings settings)
        {
            var results = new List<BatchResult>();

            var logoSettings = settings.ItemSettings.Select(
                i => new LogoSettings(
                    settings.SvgPath,
                    i.Width,
                    i.Height,
                    i.Scale ?? settings.Scale,
                    i.BackgroundColor ?? settings.BackgroundColor,
                    i.OutputPath,
                    settings.SaveOutputFiles));


            foreach (var s in logoSettings)
            {
                try
                {
                    var bitmap = Generate(s);
                    results.Add(new BatchResult(bitmap));
                }
                catch (Exception e)
                {
                    results.Add(new BatchResult(e));
                }
            }

            return results.ToArray();
        }

        public BatchResult[] GenerateBatch(string settingsPath)
        {
            var batchSettings = JsonConvert.DeserializeObject<BatchSettings>(File.ReadAllText(settingsPath));
            return GenerateBatch(batchSettings);
        }

        static Size GetLogoSize(SvgDocument svg, LogoSettings settings)
        {
            if (svg.Width.Value / settings.Width > svg.Height.Value / settings.Height)
            {
                var newWidth = settings.Width * settings.Scale;
                var widthRatio = newWidth / svg.Width.Value;
                var newHeight = widthRatio * svg.Height.Value;

                return new Size((int) Math.Round(newWidth), (int) Math.Round(newHeight));
            }
            else
            {
                var newHeight = settings.Height * settings.Scale;
                var heightRatio = newHeight / svg.Height.Value;
                var newWidth = heightRatio * svg.Width.Value;

                return new Size((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            }
        }
    }
}