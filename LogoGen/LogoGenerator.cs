using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using LogoGen.Rendering;
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
            var logoImage = RenderWholeSvgToBitmap(svg, logoSize);

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
            var svgWidth = svg.ViewBox.Width;
            var svgHeight = svg.ViewBox.Height;

            if (svgWidth / settings.Width > svgHeight / settings.Height)
            {
                var newWidth = settings.Width * settings.Scale;
                var widthRatio = newWidth / svgWidth;
                var newHeight = widthRatio * svgHeight;

                return new Size((int) Math.Round(newWidth), (int) Math.Round(newHeight));
            }
            else
            {
                var newHeight = settings.Height * settings.Scale;
                var heightRatio = newHeight / svgHeight;
                var newWidth = heightRatio * svgWidth;

                return new Size((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            }
        }

        static Bitmap RenderWholeSvgToBitmap(SvgDocument svg, Size size)
        {
            var bitmap = new Bitmap(size.Width, size.Height);
            var svgRenderer = SvgRenderer.FromImage(bitmap);
            svgRenderer.SetBoundable(new GenericBoundable(0, 0, bitmap.Width, bitmap.Height));
            var svgDim = svg.GetDimensions();
            svgRenderer.ScaleTransform(bitmap.Width / svgDim.Width, bitmap.Height / svgDim.Height);

            svg.Draw(svgRenderer);

            return bitmap;
        }
    }
}