using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Geometry.Geometry2;
using Microsoft.MapPoint.CoordinateSystems;
using System.Drawing;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using swm = System.Windows.Media;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes
{
    class ProbesHelper
    {
        private Bitmap icon;
        private IPalette palette = new LinearPalette(swm.Colors.Blue, swm.Colors.Green, swm.Colors.Red);

        public ProbesHelper(string fileName, bool fromFile)
        {
            if (fromFile)
            {
                if (fileName.StartsWith("http:"))
                {
                    try
                    {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead(fileName);
                        icon = new Bitmap(stream);
                        stream.Close();
                    }
                    catch (Exception exc)
                    {
                        Trace.WriteLine(exc.Message);
                        Assembly asm = Assembly.GetCallingAssembly();
                        string resourceName = "DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes.ProbeSample.png";
                        Stream file =
                            asm.GetManifestResourceStream(resourceName);
                        icon = new Bitmap(file);
                    }
                }
                else
                    icon = new Bitmap(fileName);

            }
            else
            {
                Assembly asm = Assembly.GetCallingAssembly();

                string resourceName = "DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes." + fileName;
                Stream file =
                    asm.GetManifestResourceStream(resourceName);


                icon = new Bitmap(file);
            }
        }

        public RasterPatch2 GetTilePatch(PointSet pointSet, Box2 regionBox, double iconSize)
        {
            GeoRect tileBox = new GeoRect(
                regionBox.MinCoordinate.X,
                regionBox.MinCoordinate.Y,
                regionBox.MaxCoordinate.X - regionBox.MinCoordinate.X,
                regionBox.MaxCoordinate.Y - regionBox.MinCoordinate.Y);

            int width = 256;
            int height = 256;
            Bitmap resultBitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resultBitmap);

            double minT = 0, maxT = 0, k;
            if (!Double.IsNaN(pointSet.MaxValue))
            {
                minT = pointSet.MinValue;
                maxT = pointSet.MaxValue;
                k = 1.0 / (maxT - minT);
            }
            else
                k = Double.NaN;

            ColorMatrix colorMatrix = new ColorMatrix();
            ImageAttributes imageAttributes = new ImageAttributes();


            for (int i = 0; i < pointSet.Data.Count; i++)
            {
                GeoRect workingRect = new GeoRect(
                            pointSet.Data[i].Longitude - iconSize / 2.0,
                            pointSet.Data[i].Latitude - iconSize / 2.0,
                            iconSize,
                            iconSize);

                if (GeoRect.IntersectionExist(workingRect, tileBox))
                {
                    int x0 = (int)(Math.Min(width, Math.Max(0, pointSet.Data[i].Longitude - iconSize / 2.0 - tileBox.Left) / tileBox.Width * width));
                    int y0 = (int)(Math.Min(height, Math.Max(0, tileBox.Top - pointSet.Data[i].Latitude - iconSize / 2.0) / tileBox.Height * height));

                    int x1 = (int)(Math.Min(width, Math.Max(0, pointSet.Data[i].Longitude + iconSize / 2.0 - tileBox.Left) / tileBox.Width * width));
                    int y1 = (int)(Math.Min(height, Math.Max(0, tileBox.Top - pointSet.Data[i].Latitude + iconSize / 2.0) / tileBox.Height * height));

                    double widthX = Math.Min(tileBox.Width, iconSize);
                    double heightY = Math.Min(tileBox.Height, iconSize);



                    lock (icon)
                    {
                        System.Windows.Media.Color tempColor = System.Windows.Media.Brushes.Red.Color;
                        if (!Double.IsNaN(k))
                        {
                            double hue = 270 * ((maxT - (double)pointSet.Data[i].Value) * k);
                            tempColor = new HsbColor(hue, 1, 1, 0.8).ToArgb();
                        }

                        colorMatrix.Matrix00 = (float)tempColor.R / 255f;
                        colorMatrix.Matrix11 = (float)tempColor.G / 255f;
                        colorMatrix.Matrix22 = (float)tempColor.B / 255f;


                        imageAttributes.SetColorMatrix(colorMatrix,
                                                ColorMatrixFlag.Default,
                                                ColorAdjustType.Bitmap);

                        int x2 = 0;
                        if (pointSet.Data[i].Longitude - iconSize / 2.0 < tileBox.Left)
                        {
                            x2 = icon.Width - (int)((double)(x1 - x0) * icon.Width / (widthX / tileBox.Width * width));
                        }

                        int y2 = 0;
                        if (tileBox.Top - pointSet.Data[i].Latitude - iconSize / 2.0 < 0)
                        {
                            y2 = icon.Height - (int)((double)(y1 - y0) * icon.Height / (heightY / tileBox.Height * height));
                        }

                        graphics.DrawImage(
                            icon,
                            new Rectangle(x0, y0, x1 - x0, y1 - y0),
                            x2,
                            y2,
                            (int)((double)(x1 - x0) * icon.Width / (widthX / tileBox.Width * width)),
                            (int)((double)(y1 - y0) * icon.Height / (heightY / tileBox.Height * height)),
                            GraphicsUnit.Pixel, imageAttributes);
                    }

                }
            }
            return new RasterPatch2(
                    regionBox,
                    resultBitmap,
                    Wgs84CoordinateReferenceSystem.Instance);
        }

        public RasterPatch2 GetTilePatch(IDataSource2D<double> field, Box2 regionBox, double iconSize)
        {
            System.Windows.Point[,] grid = field.Grid;

            Coordinate2D minCoordinate = new Coordinate2D(grid[0, 0].X, grid[0, 0].Y);
            Coordinate2D maxCoordinate = new Coordinate2D(grid[field.Width - 1, field.Height - 1].X, grid[field.Width - 1, field.Height - 1].Y);


            for (int j = 0; j < field.Height; j++)
            {
                for (int i = 0; i < field.Width; i++)
                {
                    if (grid[i, j].X < minCoordinate.X)
                        minCoordinate.X = grid[i, j].X;

                    if (grid[i, j].X > maxCoordinate.X)
                        maxCoordinate.X = grid[i, j].X;

                    if (grid[i, j].Y < minCoordinate.Y)
                        minCoordinate.Y = grid[i, j].Y;

                    if (grid[i, j].Y > maxCoordinate.Y)
                        maxCoordinate.Y = grid[i, j].Y;
                }
            }

            GeoRect gridBox = new GeoRect(
                minCoordinate.X,
                minCoordinate.Y,
                maxCoordinate.X - minCoordinate.X,
                maxCoordinate.Y - minCoordinate.Y);

            GeoRect tileBox = new GeoRect(
                regionBox.MinCoordinate.X,
                regionBox.MinCoordinate.Y,
                regionBox.MaxCoordinate.X - regionBox.MinCoordinate.X,
                regionBox.MaxCoordinate.Y - regionBox.MinCoordinate.Y);

            GeoRect intersectionRect = GeoRect.Intersect(gridBox, tileBox);

            if (intersectionRect != null)
            {
                int width = 256;
                int height = 256;
                Bitmap resultBitmap = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(resultBitmap);
                for (int i = 0; i < field.Width; i++)
                {
                    for (int j = 0; j < field.Height; j++)
                    {
                        GeoRect workingRect = new GeoRect(
                            field.Grid[i, j].X - iconSize / 2.0,
                            field.Grid[i, j].Y - iconSize / 2.0,
                            iconSize,
                            iconSize);
                        if (GeoRect.IntersectionExist(workingRect, intersectionRect))
                        {
                            int x0 = (int)(Math.Min(width, Math.Max(0, field.Grid[i, j].X - iconSize / 2.0 - tileBox.Left) / tileBox.Width * width));
                            int y0 = (int)(Math.Min(height, Math.Max(0, tileBox.Top - field.Grid[i, j].Y - iconSize / 2.0) / tileBox.Height * height));

                            int x1 = (int)(Math.Min(width, Math.Max(0, field.Grid[i, j].X + iconSize / 2.0 - tileBox.Left) / tileBox.Width * width));
                            int y1 = (int)(Math.Min(height, Math.Max(0, tileBox.Top - field.Grid[i, j].Y + iconSize / 2.0) / tileBox.Height * height));

                            double widthX = Math.Min(tileBox.Width, iconSize);
                            double heightY = Math.Min(tileBox.Height, iconSize);


                            lock (icon)
                            {
                                int x2 = 0;
                                if (field.Grid[i, j].X - iconSize / 2.0 < tileBox.Left)
                                {
                                    x2 = icon.Width - (int)((double)(x1 - x0) * icon.Width / (widthX / tileBox.Width * width));
                                }

                                int y2 = 0;
                                if (tileBox.Top - field.Grid[i, j].Y - iconSize / 2.0 < 0)
                                {
                                    y2 = icon.Height - (int)((double)(y1 - y0) * icon.Height / (heightY / tileBox.Height * height));
                                }

                                graphics.DrawImage(
                                    icon,
                                    new Rectangle(x0, y0, x1 - x0, y1 - y0),
                                    x2,
                                    y2,
                                    (int)((double)(x1 - x0) * icon.Width / (widthX / tileBox.Width * width)),
                                    (int)((double)(y1 - y0) * icon.Height / (heightY / tileBox.Height * height)),
                                    GraphicsUnit.Pixel);
                            }
                        }

                    }
                }
                return new RasterPatch2(
                    regionBox,
                    resultBitmap,
                    Wgs84CoordinateReferenceSystem.Instance);
            }
            else
            {
                return null;
            }
        }

    }
}
