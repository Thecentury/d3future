using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class RasterTriangle
    {
        private VertexPositionColor2D[] points;

        public VertexPositionColor2D Point1
        {
            get { return points[0]; }
            set
            {
                points[0] = value;
                sorted = false;
            }
        }

        public VertexPositionColor2D Point2
        {
            get { return points[1]; }
            set
            {
                points[1] = value;
                sorted = false;
            }
        }

        public VertexPositionColor2D Point3
        {
            get { return points[2]; }
            set
            {
                points[2] = value;
                sorted = false;
            }
        }

        private bool sorted;

        public RasterTriangle()
        {
            points = new VertexPositionColor2D[3];
            sorted = false;
        }

        public RasterTriangle(VertexPositionColor2D point1, VertexPositionColor2D point2, VertexPositionColor2D point3)
        {
            points = new VertexPositionColor2D[3];
            points[0] = point1;
            points[1] = point2;
            points[2] = point3;
            sorted = false;
        }

        public void FillGouraud(PixelArray drawingPlane)
        {
            if (Point1 == null || Point2 == null || Point3 == null)
                throw new InvalidOperationException("Some parts of triangle were not initialized");

            if (!sorted)
            {
                SortPoints();
            }

            System.Drawing.Point[] p = new System.Drawing.Point[3];
            p[0] = points[0].Position;
            p[1] = points[1].Position;
            p[2] = points[2].Position;

            Color[] c = new Color[3];
            c[0] = points[0].Color;
            c[1] = points[1].Color;
            c[2] = points[2].Color;


            double alphaX, alphaCR, alphaCG, alphaCB, betaX, betaCR, betaCG, betaCB;
            Direction direction01beta, direction01alpha, direction12beta, direction12alpha;

            if (p[0].X < p[2].X)
            {
                betaX = (double)(p[2].X - p[0].X) / (p[2].Y - p[0].Y);
                betaCR = (double)(c[2].R - c[0].R) / (p[2].Y - p[0].Y);
                betaCG = (double)(c[2].G - c[0].G) / (p[2].Y - p[0].Y);
                betaCB = (double)(c[2].B - c[0].B) / (p[2].Y - p[0].Y);
                direction01beta = Direction.LeftToRight;
            }
            else
            {
                betaX = (double)(p[0].X - p[2].X) / (p[2].Y - p[0].Y);
                betaCR = (double)(c[0].R - c[2].R) / (p[2].Y - p[0].Y);
                betaCG = (double)(c[0].G - c[2].G) / (p[2].Y - p[0].Y);
                betaCB = (double)(c[0].B - c[2].B) / (p[2].Y - p[0].Y);
                direction01beta = Direction.RightToLeft;
            }


            if (p[0].X < p[1].X)
            {
                alphaX = (double)(p[1].X - p[0].X) / (p[1].Y - p[0].Y);
                alphaCR = (double)(c[1].R - c[0].R) / (p[1].Y - p[0].Y);
                alphaCG = (double)(c[1].G - c[0].G) / (p[1].Y - p[0].Y);
                alphaCB = (double)(c[1].B - c[0].B) / (p[1].Y - p[0].Y);
                direction01alpha = Direction.LeftToRight;
            }
            else
            {
                alphaX = (double)(p[0].X - p[1].X) / (p[1].Y - p[0].Y);
                alphaCR = (double)(c[0].R - c[1].R) / (p[1].Y - p[0].Y);
                alphaCG = (double)(c[0].G - c[1].G) / (p[1].Y - p[0].Y);
                alphaCB = (double)(c[0].B - c[1].B) / (p[1].Y - p[0].Y);
                direction01alpha = Direction.RightToLeft;
            }

            int y0 = p[0].Y;
            int x0 = p[0].X;
            int c0R = c[0].R, c0G = c[0].G, c0B = c[0].B;

            int ymin = Math.Max(0, Math.Min(drawingPlane.Height - 1, y0));
            int ymax = Math.Max(0, Math.Min(drawingPlane.Height, p[1].Y));

            for (int y = ymin; y < ymax; y += 1)
            {


                int x_ = (int)((y - y0) * alphaX + p[0].X);
                int cR_ = (int)((y - y0) * alphaCR) + c0R;
                int cG_ = (int)((y - y0) * alphaCG) + c0G;
                int cB_ = (int)((y - y0) * alphaCB) + c0B;
                if (direction01alpha == Direction.RightToLeft)
                {
                    x_ = (int)(p[0].X - (y - y0) * alphaX);
                    cR_ = (int)(c0R - (y - y0) * alphaCR);
                    cG_ = (int)(c0G - (y - y0) * alphaCG);
                    cB_ = (int)(c0B - (y - y0) * alphaCB);
                }

                int x = (int)((y - y0) * betaX + p[0].X);
                int cR = (int)((y - y0) * betaCR) + c0R;
                int cG = (int)((y - y0) * betaCG) + c0G;
                int cB = (int)((y - y0) * betaCB) + c0B;

                if (direction01beta == Direction.RightToLeft)
                {
                    x = (int)(p[0].X - (y - y0) * betaX);
                    cR = (int)(c0R - (y - y0) * betaCR);
                    cG = (int)(c0G - (y - y0) * betaCG);
                    cB = (int)(c0B - (y - y0) * betaCB);
                }

                if (x < x_)
                {
                    int xmin = Math.Max(0, Math.Min(drawingPlane.Width - 1, x));
                    int xmax = Math.Max(0, Math.Min(drawingPlane.Width - 1, x_));
                    bool isXTriangleOut = (x > drawingPlane.Width - 1 && x_ > drawingPlane.Height - 1) || (x < 0 && x_ < 0);
                    if (!(isXTriangleOut && (xmax == drawingPlane.Width - 1 && xmin == drawingPlane.Width - 1 || xmax == 0 && xmin == 0)))
                    {
                        for (int i = xmin; i <= xmax; i++)
                        {
                            int red = (int)((i - x) * (cR_ - cR) / (double)(x_ - x)) + cR;
                            int green = (int)((i - x) * (cG_ - cG) / (double)(x_ - x)) + cG;
                            int blue = (int)((i - x) * (cB_ - cB) / (double)(x_ - x)) + cB;

                            drawingPlane.Pixels[i, y] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
                        }
                    }


                }
                else
                {
                    int xmin = Math.Max(0, Math.Min(drawingPlane.Width - 1, x_));
                    int xmax = Math.Max(0, Math.Min(drawingPlane.Width - 1, x));
                    bool isXTriangleOut = (x > drawingPlane.Width - 1 && x_ > drawingPlane.Height - 1) || (x < 0 && x_ < 0);
                    if (!(isXTriangleOut && (xmax == drawingPlane.Width - 1 && xmin == drawingPlane.Width - 1 || xmax == 0 && xmin == 0)))
                    {
                        for (int i = xmin; i <= xmax; i++)
                        {
                            int red = (int)((i - x_) * (cR - cR_) / (double)(x - x_)) + cR_;
                            int green = (int)((i - x_) * (cG - cG_) / (double)(x - x_)) + cG_;
                            int blue = (int)((i - x_) * (cB - cB_) / (double)(x - x_)) + cB_;
                            drawingPlane.Pixels[i, y] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
                        }
                    }
                }

            }

            Color lastColor = Color.FromArgb(
                255,
                (byte)((int)((p[1].Y - y0) * betaCR) + c0R),
                (byte)((int)((p[1].Y - y0) * betaCG) + c0G),
                (byte)((int)((p[1].Y - y0) * betaCB) + c0B));


            int x1 = (int)((p[1].Y - y0) * betaX + x0);
            if (direction01beta == Direction.RightToLeft)
            {
                x1 = (int)(x0 - (p[1].Y - y0) * betaX);
                lastColor = Color.FromArgb(
                255,
                (byte)((int)(c0R - (p[1].Y - y0) * betaCR)),
                (byte)((int)(c0G - (p[1].Y - y0) * betaCG)),
                (byte)((int)(c0B - (p[1].Y - y0) * betaCB)));
            }

            if (p[2].X < p[1].X)
            {
                alphaX = (double)(p[1].X - p[2].X) / (p[2].Y - p[1].Y);
                alphaCR = (double)(c[1].R - c[2].R) / (p[2].Y - p[1].Y);
                alphaCG = (double)(c[1].G - c[2].G) / (p[2].Y - p[1].Y);
                alphaCB = (double)(c[1].B - c[2].B) / (p[2].Y - p[1].Y);
                direction12alpha = Direction.RightToLeft;
            }
            else
            {
                alphaX = (double)(p[2].X - p[1].X) / (p[2].Y - p[1].Y);
                alphaCR = (double)(c[2].R - c[1].R) / (p[2].Y - p[1].Y);
                alphaCG = (double)(c[2].G - c[1].G) / (p[2].Y - p[1].Y);
                alphaCB = (double)(c[2].B - c[1].B) / (p[2].Y - p[1].Y);
                direction12alpha = Direction.LeftToRight;
            }

            if (p[2].X > x1)
            {
                betaX = (double)(p[2].X - x1) / (p[2].Y - p[1].Y);
                betaCR = (double)(c[2].R - lastColor.R) / (p[2].Y - p[1].Y);
                betaCG = (double)(c[2].G - lastColor.G) / (p[2].Y - p[1].Y);
                betaCB = (double)(c[2].B - lastColor.B) / (p[2].Y - p[1].Y);
                direction12beta = Direction.LeftToRight;
            }
            else
            {
                betaX = (double)(x1 - p[2].X) / (p[2].Y - p[1].Y);
                betaCR = (double)(-c[2].R + lastColor.R) / (p[2].Y - p[1].Y);
                betaCG = (double)(-c[2].G + lastColor.G) / (p[2].Y - p[1].Y);
                betaCB = (double)(-c[2].B + lastColor.B) / (p[2].Y - p[1].Y);
                direction12beta = Direction.RightToLeft;
            }

            y0 = p[1].Y;
            x0 = x1;
            c0R = lastColor.R;
            c0G = lastColor.G;
            c0B = lastColor.B;

            ymin = Math.Max(0, Math.Min(drawingPlane.Height - 1, y0));
            ymax = Math.Max(0, Math.Min(drawingPlane.Height - 1, p[2].Y));
            bool isTriangleOut = (y0 > drawingPlane.Height - 1 && p[2].Y > drawingPlane.Height - 1) || (y0 < 0 && p[2].Y < 0);
            if (!(isTriangleOut && ((ymax == 0 && ymin == 0) || ymax == drawingPlane.Height - 1 && ymin == drawingPlane.Height - 1)))
            {
                for (int y = ymin; y <= ymax; y += 1)
                {
                    int x_ = (int)(p[1].X + (y - y0) * alphaX);
                    int cR_ = (int)(c[1].R + (y - y0) * alphaCR);
                    int cG_ = (int)(c[1].G + (y - y0) * alphaCG);
                    int cB_ = (int)(c[1].B + (y - y0) * alphaCB);
                    if (direction12alpha == Direction.RightToLeft)
                    {
                        x_ = (int)(p[1].X - (y - y0) * alphaX);
                        cR_ = (int)(c[1].R - (y - y0) * alphaCR);
                        cG_ = (int)(c[1].G - (y - y0) * alphaCG);
                        cB_ = (int)(c[1].B - (y - y0) * alphaCB);
                    }

                    int x = (int)((y - y0) * betaX + x1);
                    int cR = (int)((y - y0) * betaCR) + c0R;
                    int cG = (int)((y - y0) * betaCG) + c0G;
                    int cB = (int)((y - y0) * betaCB) + c0B;
                    if (direction12beta == Direction.RightToLeft)
                    {
                        x = (int)(x1 - (y - y0) * betaX);
                        cR = (int)(c0R - (y - y0) * betaCR);
                        cG = (int)(c0G - (y - y0) * betaCG);
                        cB = (int)(c0B - (y - y0) * betaCB);
                    }

                    if (x < x_)
                    {

                        int xmin = Math.Max(0, Math.Min(drawingPlane.Width - 1, x));
                        int xmax = Math.Max(0, Math.Min(drawingPlane.Width - 1, x_));
                        bool isXTriangleOut = (x > drawingPlane.Width - 1 && x_ > drawingPlane.Height - 1) || (x < 0 && x_ < 0);
                        if (!(isXTriangleOut && (xmax == drawingPlane.Width - 1 && xmin == drawingPlane.Width - 1 || xmax == 0 && xmin == 0)))
                        {
                            for (int i = xmin; i <= xmax; i++)
                            {
                                int red = (int)((i - x) * (cR_ - cR) / (double)(x_ - x)) + cR;
                                int green = (int)((i - x) * (cG_ - cG) / (double)(x_ - x)) + cG;
                                int blue = (int)((i - x) * (cB_ - cB) / (double)(x_ - x)) + cB;

                                drawingPlane.Pixels[i, y] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
                            }
                        }
                    }
                    else
                    {
                        int xmin = Math.Max(0, Math.Min(drawingPlane.Width - 1, x_));
                        int xmax = Math.Max(0, Math.Min(drawingPlane.Width - 1, x));
                        bool isXTriangleOut = (x > drawingPlane.Width - 1 && x_ > drawingPlane.Height - 1) || (x < 0 && x_ < 0);
                        if (!(isXTriangleOut && (xmax == drawingPlane.Width - 1 && xmin == drawingPlane.Width - 1 || xmax == 0 && xmin == 0)))
                        {
                            for (int i = xmin; i <= xmax; i++)
                            {
                                int red = (int)((i - x_) * (cR - cR_) / (double)(x - x_)) + cR_;
                                int green = (int)((i - x_) * (cG - cG_) / (double)(x - x_)) + cG_;
                                int blue = (int)((i - x_) * (cB - cB_) / (double)(x - x_)) + cB_;
                                drawingPlane.Pixels[i, y] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
                            }
                        }
                    }
                }
            }


            //drawingPlane.Pixels[p[0].X, p[0].Y] = Color.FromArgb(255, 255, 255, 255);
            //drawingPlane.Pixels[p[1].X, p[1].Y] = Color.FromArgb(255, 255, 255, 255);
            //drawingPlane.Pixels[p[2].X, p[2].Y] = Color.FromArgb(255, 255, 255, 255);
        }

        private void SortPoints()
        {
            for (int i = 1; i < 3; i++)
            {
                if (points[i - 1].Position.Y > points[i].Position.Y)
                {
                    VertexPositionColor2D p = points[i - 1];
                    points[i - 1] = points[i];
                    points[i] = p;

                    if (i > 1) i -= 2;
                }
            }
            sorted = true;
        }

        enum Direction
        {
            RightToLeft,
            LeftToRight
        }

    }

    class VertexPositionColor2D
    {
        private System.Drawing.Point position;

        public System.Drawing.Point Position
        {
            get { return position; }
            set { position = value; }
        }
        private Color color;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public VertexPositionColor2D(System.Drawing.Point position, Color color)
        {
            this.position = position;
            this.color = color;
        }
    }
}

