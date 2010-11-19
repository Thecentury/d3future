using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.MapPoint.Geometry.Geometry2;
using Microsoft.MapPoint.CoordinateSystems;
using swm = System.Windows.Media;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    partial class ColorMapHelper
    {
        private GeoRect gridBox;
        private GeoRect tileBox;
        private NonUniformDataSource2D<double> field;
        private IPalette palette;
        private double minT, maxT;

        public double MaxT
        {
            get { return maxT; }
        }

        public double MinT
        {
            get { return minT; }
        }

        public Box2 RegionBox
        {
            set
            {
                this.tileBox = new GeoRect(
                value.MinCoordinate.X, value.MinCoordinate.Y,
                value.MaxCoordinate.X - value.MinCoordinate.X,
                value.MaxCoordinate.Y - value.MinCoordinate.Y);

                this.regionBox = value;
            }
        }

        public ColorMapHelper(NonUniformDataSource2D<double> field, Box2 regionBox, double minT, double maxT)
        {
            this.minT = minT;
            this.maxT = maxT;

            this.gridBox = new GeoRect(
                Math.Min(field.X[0], field.X[field.Width - 1]),
                Math.Min(field.Y[0], field.Y[field.Height - 1]),
                Math.Abs(field.X[field.Width - 1] - field.X[0]),
                Math.Abs(field.Y[field.Height - 1] - field.Y[0]));

            if (regionBox != null)
            {
                this.tileBox = new GeoRect(
                    regionBox.MinCoordinate.X, regionBox.MinCoordinate.Y,
                    regionBox.MaxCoordinate.X - regionBox.MinCoordinate.X,
                    regionBox.MaxCoordinate.Y - regionBox.MinCoordinate.Y);
            }

            palette = new LinearPalette(swm.Colors.Blue, swm.Colors.Green, swm.Colors.Red);

            this.field = field;
        }


        private double Interpolate(double C00, double C01, double C10, double C11, double alpha, double beta)
        {
            return C00 * (1 - alpha) * (1 - beta) + C10 * alpha * (1 - beta) + C01 * (1 - alpha) * beta + C11 * alpha * beta;
        }

        public RasterPatch2 GetTilePatch()
        {
            int width = 256;
            int height = 256;

            GeoRect intersectionRect = GeoRect.Intersect(gridBox, tileBox);

            if (intersectionRect != null)
            {
                int startPointX = (int)(width * (intersectionRect.Left - tileBox.Left) / tileBox.Width);
                int startPointY = (int)(height * (tileBox.Top - intersectionRect.Top) / tileBox.Height);

                int endPointX = (int)(width * (intersectionRect.Right - tileBox.Left) / tileBox.Width);
                int endPointY = (int)(height * (tileBox.Top - intersectionRect.Bottom) / tileBox.Height);

                const int bitsPerPixel = 32;
                int stride = (endPointX - startPointX) * ((bitsPerPixel + 7) / 8);
                int arraySize = stride * (endPointY - startPointY);
                byte[] pixels = new byte[arraySize];
                int index = 0;


                // Get data and min/max.
                double[,] data = field.Data;
                double minT, maxT;

                MathHelper.GetMaxMin(field.Data, out maxT, out minT);

                this.minT = minT;
                this.maxT = maxT;


                double k = 1.0 / (maxT - minT);

                GeoToBitmap[] latToBitmap = new GeoToBitmap[endPointY - startPointY];
                for (int i = 0; i < endPointY - startPointY; i++)
                {
                    double y = intersectionRect.Top - (i + 0.5) * intersectionRect.Height / (endPointY - startPointY);
                    for (int j = 1; j < field.Height; j++)
                    {
                        if (y < field.Y[j])
                        {
                            latToBitmap[i] = new GeoToBitmap { Knot = j, Proportion = ((y - field.Y[j - 1]) / (field.Y[j] - field.Y[j - 1])) };
                            break;
                        }
                    }
                }

                GeoToBitmap[] lonToBitmap = new GeoToBitmap[endPointX - startPointX];
                for (int i = 0; i < endPointX - startPointX; i++)
                {
                    double x = ((i + 0.5) * intersectionRect.Width / (endPointX - startPointX)) + intersectionRect.Left;
                    for (int j = 1; j < field.Width; j++)
                    {
                        if (x < field.X[j])
                        {
                            lonToBitmap[i] = new GeoToBitmap { Knot = j, Proportion = ((x - field.X[j - 1]) / (field.X[j] - field.X[j - 1])) };
                            break;
                        }
                    }
                }

                for (int j = 0; j < endPointY - startPointY; j++)
                {
                    for (int i = 0; i < endPointX - startPointX; i++)
                    {
                        int knotY = latToBitmap[j].Knot;
                        int knotX = lonToBitmap[i].Knot;

                        double proportionY = latToBitmap[j].Proportion;
                        double proportionX = lonToBitmap[i].Proportion;

                        double colorValue = Interpolate(
                            data[knotX - 1, knotY - 1],
                            data[knotX - 1, knotY],
                            data[knotX, knotY - 1],
                            data[knotX, knotY],
                            proportionX,
                            proportionY);

                        System.Windows.Media.Color color = palette.GetColor((colorValue - minT) * k);

                        //byte b1, b2;
                        //ConvertColor(color,out b1,out b2);
                        //pixels[index++] = b2;
                        //pixels[index++] = b1;

                        pixels[index++] = color.B;
                        pixels[index++] = color.G;
                        pixels[index++] = color.R;
                        pixels[index++] = (byte)255;
                    }
                }

                index = 0;
                bool need = true;
                for (int j = 0; j < endPointY - startPointY; j++)
                {
                    for (int i = 0; i < endPointX - startPointX; i++)
                    {
                        int currentIndex = index;

                        System.Windows.Media.Color oldColor = new System.Windows.Media.Color()
                        {
                            A = pixels[currentIndex + 3],
                            R = pixels[currentIndex + 2],
                            G = pixels[currentIndex + 1],
                            B = pixels[currentIndex]
                        };
                        System.Windows.Media.Color newColor = FindClosestColor(oldColor);
                        QuantError quant_error = new QuantError
                        {
                            A = oldColor.A,
                            B = (int)oldColor.B - (int)newColor.B,
                            G = (int)oldColor.G - (int)newColor.G,
                            R = (int)oldColor.R - (int)newColor.R
                        };

                        //pixels[i,j]
                        pixels[currentIndex] = newColor.B;
                        pixels[currentIndex + 1] = newColor.G;
                        pixels[currentIndex + 2] = newColor.R;
                        pixels[currentIndex + 3] = newColor.A;

                        if (need)
                        {
                            int newValue = 0;
                            int maxValueRB = 255;

                            if (i < endPointX - startPointX - 1)
                            {

                                //pixels[i+1,j]
                                newValue = (int)pixels[currentIndex + 4] + ((quant_error.B * 7) >> 4);
                                pixels[currentIndex + 4] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));


                                newValue = (int)pixels[currentIndex + 4 + 1] + ((quant_error.G * 7) >> 4);
                                pixels[currentIndex + 4 + 1] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));


                                newValue = (int)pixels[currentIndex + 4 + 2] + ((quant_error.R * 7) >> 4);
                                pixels[currentIndex + 4 + 2] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));
                            }

                            if (j < endPointY - startPointY - 1)
                            {

                                //pixels[i,j+1]
                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX)] + ((quant_error.B * 5) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX)] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) + 1] + ((quant_error.G * 5) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) + 1] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) + 2] + ((quant_error.R * 5) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) + 2] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));
                            }

                            if (i > 0 && j < (endPointY - startPointY - 1))
                            {
                                //pixels[i-1,j+1]
                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) - 4] + ((quant_error.B * 3) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) - 4] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) - 4 + 1] + ((quant_error.G * 3) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) - 4 + 1] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) - 4 + 2] + ((quant_error.R * 3) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) - 4 + 2] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));
                            }

                            if (i < (endPointX - startPointX - 1) && j < (endPointY - startPointY - 1))
                            {
                                //pixels[i+1, j+1]
                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) + 4] + ((quant_error.B * 1) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) + 4] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) + 4 + 1] + ((quant_error.G * 1) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) + 4 + 1] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));

                                newValue = (int)pixels[currentIndex + 4 * (endPointX - startPointX) + 4 + 2] + ((quant_error.R * 1) >> 4);
                                pixels[currentIndex + 4 * (endPointX - startPointX) + 4 + 2] = (byte)Math.Max(0, Math.Min(maxValueRB, newValue));
                            }
                        }

                        index += 4;
                    }
                }

                index = 0;
                for (int j = 0; j < endPointY - startPointY; j++)
                {
                    for (int i = 0; i < endPointX - startPointX; i++)
                    {


                        System.Windows.Media.Color color = new System.Windows.Media.Color()
                        {
                            B = pixels[index],
                            G = pixels[index + 1],
                            R = pixels[index + 2]
                        };
                        color = FindClosestColor(color);
                        pixels[index] = color.B;
                        pixels[index + 1] = color.G;
                        pixels[index + 2] = color.R;
                        index += 4;
                    }
                }

                RasterPatch2 patch = new RasterPatch2(
                    new Box2(Wgs84CoordinateReferenceSystem.Instance, new Coordinate2D(intersectionRect.X, intersectionRect.Y), new Coordinate2D(intersectionRect.Right, intersectionRect.Top)),
                    pixels,
                    PatchPixelFormat.Format32bppArgb,
                    endPointX - startPointX,
                    endPointY - startPointY,
                    stride,
                    Wgs84CoordinateReferenceSystem.Instance);

                return patch;

            }
            else
            {
                return null;
            }
        }

        private bool CheckPeek(Coordinate2D peek, Box2 gridBox)
        {
            if (peek.X > gridBox.MinCoordinate.X && peek.X < gridBox.MaxCoordinate.X && peek.Y > gridBox.MinCoordinate.Y && peek.Y < gridBox.MaxCoordinate.Y)
                return true;
            else
                return false;
        }

        private void ConvertColor(System.Drawing.Color color, out byte b1, out byte b2)
        {
            b1 = 0;
            b2 = 0;
            //Packing Red component
            byte workingByte = (byte)(color.R * 32 / 256);
            workingByte <<= 3;
            b1 = (byte)(0 | workingByte);

            //Packing Green component (Left part to first byte, right to second
            workingByte = (byte)(color.G * 64 / 256);
            workingByte >>= 3;
            b1 = (byte)(b1 | workingByte);
            workingByte = (byte)(color.G * 64 / 256);
            workingByte <<= 5;
            b2 = (byte)(0 | workingByte);
            // Packing Blue component
            workingByte = (byte)(color.B * 32 / 256);
            b2 = (byte)(b2 | workingByte);
            //Setting Opacity component
            //b2 = (byte)(b2 | 1);*/
        }

        private System.Windows.Media.Color FindClosestColor(System.Windows.Media.Color color)
        {
            System.Windows.Media.Color result = new System.Windows.Media.Color();
            result.A = color.A;


            if ((int)color.B + 4 > 255)
                result.B = 255 & 0xF8;
            else
                result.B = (byte)(((color.B + 4) & 0xF8));

            if ((int)color.R + 4 > 255)
                result.R = 255 & 0xF8;
            else
                result.R = (byte)(((color.R + 4) & 0xF8));

            if ((int)color.G + 2 > 255)
                result.G = 255 & 0xFC;
            else
                result.G = (byte)(((color.G + 2) & 0xFC));

            /*
            result.B = (byte)(color.B & 0xF8);
            result.G = (byte)(color.G & 0xFC);
            result.R = (byte)(color.R & 0xF8);
            */
            return result;
        }
    }

    class QuantError
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

    }

}
