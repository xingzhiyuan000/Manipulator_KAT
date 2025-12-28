

using System;

namespace OpenTK_Winform_Robot
{
    class Solution
    {
        public static double calN11(double theta7, double nx, double ox)
        {
            return Math.Cos(theta7) * nx - Math.Sin(theta7) * ox;
        }

        public static double calN12(double ax)
        {
            return -ax;
        }

        public static double calN21(double theta7, double ny, double oy)
        {
            return Math.Cos(theta7) * ny - Math.Sin(theta7) * oy;
        }

        public static double calN22(double ay)
        {
            return -ay;
        }

        public static double calN31(double theta7, double nz, double oz)
        {
            return Math.Cos(theta7) * nz - Math.Sin(theta7) * oz;
        }

        public static double calN32(double az)
        {
            return -az;
        }

        public static double calA(double n_31, double n_11, double n_21)
        {
            return Math.Pow(n_31, 2)+ Math.Pow(n_11, 2)+ Math.Pow(n_21, 2);
        }

        public static double calB(double n_32, double n_12, double n_22)
        {
            return Math.Pow(n_32, 2) + Math.Pow(n_12, 2) + Math.Pow(n_22, 2);
        }

        public static double calC(double n_31, double n_32, double n_11, double n_12, double n_21, double n_22)
        {
            return n_31* n_32+ n_11 * n_12 + n_21 * n_22;
        }

        public static double[] calTheta6(double _a, double _b, double _c)
        {
            double x1 = (2 * _c + Math.Sqrt(4 * Math.Pow(_c, 2) - 4 * (1 - _a) * (_a - _b))) / (2 * (1 - _a));
            double x2 = (2 * _c - Math.Sqrt(4 * Math.Pow(_c, 2) - 4 * (1 - _a) * (_a - _b))) / (2 * (1 - _a));

            double[] results = new double[] { Math.Atan(x1), Math.Atan(x2) };
            return results;
        }

        //public static double[] calTheta6(double n_31, double n_32, double n_11, double n_12, double n_21, double n_22)
        //{
        //    double result1 = 0;
        //    double result2 = Math.PI/2.0;
        //    double result3 = -Math.Asin(n_31 * n_32 + n_11 * n_12 + n_21 * n_22);
        //    double[] results = new double[] {result1, result2, result3 };

        //    return results;
        //}

        public static double calTheta3(double theta6,double n_21, double n_22)
        {
            return -Math.Asin(n_21*Math.Sin(theta6)+n_22*Math.Cos(theta6));
        }


    }
}
