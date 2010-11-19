using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.MapPoint.Geometry.Geometry2;
using Microsoft.MapPoint.CoordinateSystems;
using System.Drawing;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using swm = System.Windows.Media;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    partial class ColorMapHelper
    {
        WarpedDataSource2D<double> warpedField;
        RasterTriangle workingTriangle;
        Box2 regionBox;

        public ColorMapHelper(WarpedDataSource2D<double> field, Box2 regionBox, double minT, double maxT)
        {
            this.minT = minT;
            this.maxT = maxT;

            warpedField = field;

            if (regionBox != null)
            {
                this.tileBox = new GeoRect(
                    regionBox.MinCoordinate.X, regionBox.MinCoordinate.Y,
                    regionBox.MaxCoordinate.X - regionBox.MinCoordinate.X,
                    regionBox.MaxCoordinate.Y - regionBox.MinCoordinate.Y);
                this.regionBox = regionBox;
            }

            System.Windows.Point[,] grid = warpedField.Grid;

            Coordinate2D minCoordinate = new Coordinate2D(grid[0, 0].X, grid[0, 0].Y);
            Coordinate2D maxCoordinate = new Coordinate2D(grid[warpedField.Width - 1, warpedField.Height - 1].X, grid[warpedField.Width - 1, warpedField.Height - 1].Y);


            for (int j = 0; j < warpedField.Height; j++)
            {
                for (int i = 0; i < warpedField.Width; i++)
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

            gridBox = new GeoRect(
                minCoordinate.X,
                minCoordinate.Y,
                maxCoordinate.X - minCoordinate.X,
                maxCoordinate.Y - minCoordinate.Y);

            palette = new LinearPalette(swm.Colors.Blue, swm.Colors.Green, swm.Colors.Red);
            workingTriangle = new RasterTriangle();
        }

        public RasterPatch2 GetWarpedTilePatch()
        {
            int width = 256;
            int height = 256;

            GeoRect intersectionRect = GeoRect.Intersect(gridBox, tileBox);

            if (intersectionRect != null)
            {
                PixelArray plane = new PixelArray(width, height);
                GeoRect workingRect;

                // Get data and min/max.
                double[,] data = warpedField.Data;
                double minT, maxT;


                MathHelper.GetMaxMin(data, out maxT, out minT);

                this.minT = minT;
                this.maxT = maxT;
                
                double k = 1.0 / (maxT - minT);

                System.Windows.Point[,] grid = warpedField.Grid;
                for (int i = 0; i < warpedField.Width - 1; i++)
                {
                    for (int j = 0; j < warpedField.Height - 1; j++)
                    {
                        workingRect = new GeoRect(
                            new System.Windows.Point(grid[i, j].X, grid[i, j].Y),
                            new System.Windows.Point(grid[i + 1, j].X, grid[i + 1, j].Y),
                            new System.Windows.Point(grid[i, j + 1].X, grid[i, j + 1].Y));

                        if (GeoRect.IntersectionExist(workingRect, intersectionRect))
                        {
                            System.Windows.Media.Color color = palette.GetColor((data[i, j] - minT) * k);
                            workingTriangle.Point1 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i, j].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i, j].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            color = palette.GetColor((data[i + 1, j] - minT) * k);
                            workingTriangle.Point2 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i + 1, j].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i + 1, j].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            color = palette.GetColor((data[i, j + 1] - minT) * k);
                            workingTriangle.Point3 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i, j + 1].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i, j + 1].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            workingTriangle.FillGouraud(plane);

                        }

                        workingRect = new GeoRect(
                            new System.Windows.Point(grid[i + 1, j].X, grid[i + 1, j].Y),
                            new System.Windows.Point(grid[i, j + 1].X, grid[i, j + 1].Y),
                            new System.Windows.Point(grid[i + 1, j + 1].X, grid[i + 1, j + 1].Y));

                        if (GeoRect.IntersectionExist(workingRect, intersectionRect))
                        {
                            System.Windows.Media.Color color = palette.GetColor((data[i + 1, j] - minT) * k);
                            workingTriangle.Point1 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i + 1, j].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i + 1, j].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            color = palette.GetColor((data[i, j + 1] - minT) * k);
                            workingTriangle.Point2 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i, j + 1].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i, j + 1].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            color = palette.GetColor((data[i + 1, j + 1] - minT) * k);
                            workingTriangle.Point3 = new VertexPositionColor2D(
                                new System.Drawing.Point(
                                    (int)((grid[i + 1, j + 1].X - tileBox.Left) * width / tileBox.Width),
                                    (int)(height - (grid[i + 1, j + 1].Y - tileBox.Bottom) * height / tileBox.Height)),
                                    color);
                            
                            workingTriangle.FillGouraud(plane);

                        }

                    }
                }

                const int bitsPerPixel = 32;
                int stride = width * ((bitsPerPixel + 7) / 8);
                int arraySize = stride * height;
                byte[] pixels = new byte[arraySize];
                int index = 0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pixels[index++] = plane.Pixels[j, i].B;
                        pixels[index++] = plane.Pixels[j, i].G;
                        pixels[index++] = plane.Pixels[j, i].R;
                        pixels[index++] = plane.Pixels[j, i].A;

                    }
                }

                RasterPatch2 patch = new RasterPatch2(
                    regionBox,
                    pixels,
                    PatchPixelFormat.Format32bppArgb,
                    width,
                    height,
                    stride,
                    Wgs84CoordinateReferenceSystem.Instance);

                return patch;
            }
            else
            {
                return null;
            }
        }

    }
}