using System;

namespace FluidCurrentModelling2.DataStructures
{

    class DoubleMatrix3D
    {
        public Matrix3D FirstMatrix { get; set; }
        public Matrix3D SecondMatrix { get; set; }
    }

    class LayerData
    {
        private Matrix3D u_var, v_var, w_var, T_var, Div_var;

        internal Matrix3D T
        {
            get { return T_var; }
            set { T_var = value; }
        }

        internal Matrix3D Div
        {
            get { return Div_var; }
            set { Div_var = value; }
        }

        internal Matrix3D W
        {
            get { return w_var; }
            set { w_var = value; }
        }

        internal Matrix3D V
        {
            get { return v_var; }
            set { v_var = value; }
        }

        internal Matrix3D U
        {
            get { return u_var; }
            set { u_var = value; }
        }
        

        public LayerData(int width, int height, int thickness)
        {
            u_var = new Matrix3D(width, height, thickness);
            v_var = new Matrix3D(width, height, thickness);
            w_var = new Matrix3D(width, height, thickness);
            T_var = new Matrix3D(width, height, thickness);
            Div_var = new Matrix3D(width, height, thickness);

            this.width = width;
            this.height = height;
            this.thickness = thickness;
        }

        public LayerData(Matrix3D u, Matrix3D v, Matrix3D w, Matrix3D T, Matrix3D Div)
        {
            if (u.Width != v.Width || u.Width != w.Width || u.Width != T.Width || u.Width != Div.Width)
                throw new ArgumentException("Invalid Layer Data, Matrices must have the same size");
            if (u.Height != v.Height || u.Height != w.Height || u.Height != T.Height || u.Height != Div.Height)
                throw new ArgumentException("Invalid Layer Data, Matrices must have the same size");
            if (u.Thickness != v.Thickness || u.Thickness != w.Thickness || u.Thickness != T.Thickness || u.Thickness != Div.Thickness)
                throw new ArgumentException("Invalid Layer Data, Matrices must have the same size");
                        
            this.width = u.Width;
            this.height = u.Height;
            this.thickness = u.Thickness;

            this.u_var = u;
            this.v_var = v;
            this.w_var = w;
            this.T_var = T;
            this.Div_var = Div;
        }

        private int width, height, thickness;

        public int Thickness
        {
            get { return thickness; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

    }
}