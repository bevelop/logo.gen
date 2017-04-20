﻿using System;
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
        static readonly IDictionary<BitDepth, PixelFormat> BitDepthToPixelFormat = new Dictionary<BitDepth, PixelFormat>
        {
            {BitDepth.Rgb16, PixelFormat.Format16bppRgb555},
            {BitDepth.Rgb24, PixelFormat.Format24bppRgb},
            {BitDepth.Rgba32, PixelFormat.Format32bppArgb}
        };

        public Bitmap Generate(LogoSettings settings)
        {
            var svg = SvgDocument.Open(settings.SvgPath);

            var logoSize = GetLogoSize(new SizeF(svg.ViewBox.Width, svg.ViewBox.Height), settings);
            using (var logoImage = RenderWholeSvgToBitmap(svg, logoSize))
            using (var finalLogo = RenderBackground(settings))
            {
                using (var gfx = Graphics.FromImage(finalLogo))
                {
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
                    settings.BackgroundImage,
                    i.OutputPath,
                    settings.OutputBitDepth,
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

        static Bitmap RenderBackground(LogoSettings settings)
        {
            var background = new Bitmap(settings.Width, settings.Height, BitDepthToPixelFormat[settings.OutputBitDepth]);

            using (var gfx = Graphics.FromImage(background))
            using (var brush = new SolidBrush(settings.BackgroundColor))
            {
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.FillRectangle(brush, 0, 0, settings.Width, settings.Height);

                if (settings.BackgroundImage != null)
                {
                    var backg = Image.FromFile(settings.BackgroundImage);
                    var src = GetBackgroundRect(backg.Size, settings);

                    gfx.DrawImage(backg,
                        new Rectangle(0, 0, settings.Width, settings.Height),
                        src,
                        GraphicsUnit.Pixel);
                }
            }

            return background;
        }

        static Size GetLogoSize(SizeF svgSize, LogoSettings settings)
        {
            var svgWidth = svgSize.Width;
            var svgHeight = svgSize.Height;

            if (svgWidth / settings.Width > svgHeight / settings.Height)
            {
                var newWidth = settings.Width * settings.Scale;
                var widthRatio = newWidth / svgWidth;
                var newHeight = widthRatio * svgHeight;

                return new Size((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            }
            else
            {
                var newHeight = settings.Height * settings.Scale;
                var heightRatio = newHeight / svgHeight;
                var newWidth = heightRatio * svgWidth;

                return new Size((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            }
        }

        static Rectangle GetBackgroundRect(Size backgroundSrcSize, LogoSettings settings)
        {
            if (backgroundSrcSize.Width / (float)settings.Width < backgroundSrcSize.Height / (float)settings.Height)
            {
                var ratio = settings.Height / (float)settings.Width;
                var srcWidth = backgroundSrcSize.Width;
                var srcHeight = (int)Math.Round(srcWidth * ratio);
                var srcY = (int)Math.Round((backgroundSrcSize.Height - srcHeight) / 2.0f);

                return new Rectangle(0, srcY, srcWidth, srcHeight);
            }
            else
            {
                var ratio = settings.Width / (float)settings.Height;
                var srcHeight = backgroundSrcSize.Height;
                var srcWidth = (int)Math.Round(srcHeight * ratio);
                var srcX = (int)Math.Round((backgroundSrcSize.Width - srcWidth) / 2.0f);

                return new Rectangle(srcX, 0, srcWidth, srcHeight);
            }
        }
    }
}