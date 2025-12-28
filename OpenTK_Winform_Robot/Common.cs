using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using OpenTK;
using System;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    class Common
    {

        /// <summary>
        /// 随机骨骼位置和角度
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="rand"></param>
        public static void RandomSetJoint(Joint joint, Random rand)
        {
            //随机位置
            float randTrans = joint.constraintTrans[0] + (float)rand.NextDouble() * (joint.constraintTrans[1] - joint.constraintTrans[0]);
            joint.translate(randTrans); //检查是否在区间内
                                     
            float randRotX = joint.constraint[0].X + (float)rand.NextDouble() * (joint.constraint[1].X - joint.constraint[0].X);
            float randRotY = joint.constraint[0].Y + (float)rand.NextDouble() * (joint.constraint[1].Y - joint.constraint[0].Y);
            float randRotZ = joint.constraint[0].Z + (float)rand.NextDouble() * (joint.constraint[1].Z - joint.constraint[0].Z);
            
            joint.setAngle(randRotX, randRotY, randRotZ); //区间检查

        }
        /// <summary>
        /// 文本框输入检查
        /// </summary>
        public static bool check_Tex(string val)
        {
            if (val == "" || val == "-" || val.EndsWith(".") || val == "*" || val == "/") return false;
            else return true;


        }

        /// <summary>
        ///【齐次变换矩阵】 
        /// </summary>
        public static Matrix<double> HomoTransMat(double d1, double theta2, double theta3, double d4, double theta5, double theta6, double theta7, string cmd)
        {
            
            int i=0;
            //double[] theta = new double[] {0, theta2, theta3, -90 * (Math.PI / 180), theta5,theta6,theta7, -90 * (Math.PI / 180)};
            //double[] alpha = new double[] {0, -90 * (Math.PI / 180), -90 * (Math.PI / 180), 90 * (Math.PI / 180), -90 * (Math.PI / 180), 90 * (Math.PI / 180), 90 * (Math.PI / 180), 0};
            //double[] a = new double[] {0, 0,40, 559, 0,280,70.114,0};
            //double[] d = new double[] {d1, 230.5,0, d4, -741,0,0,587.431};

            double[] theta = new double[] { global.theta[0], theta2, theta3, global.theta[3], theta5, theta6, theta7, global.theta[7]};
            double[] alpha = new double[7];
            double[] a = new double[7];
            for (int k = 0; k < 8; k++)
            { 
                alpha[k] = global.alpha[k];
                a[k] = global.a[k];
            } 


            double[] d = new double[] { d1, global.d[1], global.d[2], d4, global.d[4], global.d[5], global.d[6], global.d[7]};

            switch (cmd)
            {
                case "01":
                    i = 0;
                    break;
                case "12":
                    i = 1;
                    break;
                case "23":
                    i = 2;
                    break;
                case "34":
                    i = 3;
                    break;
                case "45":
                    i = 4;
                    break;
                case "56":
                    i = 5;
                    break;
                case "67":
                    i = 6;
                    break;
                case "78":
                    i = 7;
                    break;
            }
            double nx = Math.Cos(theta[i]);
            double ny = Math.Sin(theta[i]) *Math.Cos(alpha[i]);
            double nz = Math.Sin(theta[i]) * Math.Sin(alpha[i] );
            double ox = -Math.Sin(theta[i] );
            double oy = Math.Cos(theta[i] ) * Math.Cos(alpha[i]);
            double oz = Math.Cos(theta[i] ) * Math.Sin(alpha[i]);
            double ax = 0;
            double ay = -Math.Sin(alpha[i]);
            double az = Math.Cos(alpha[i] );
            double px = a[i];
            double py = -Math.Sin(alpha[i]) *d[i];
            double pz = Math.Cos(alpha[i]) * d[i];

            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
            {nx, ox, ax, px},
            {ny, oy, ay, py},
            {nz, oz, az, pz},
            {0, 0, 0, 1}});

            return T;
        }

        /// <summary>
        /// 正运动学
        /// </summary>
        public static Matrix<double> Fkine_LH4500(double d1, double theta2, double theta3, double d4, double theta5, double theta6, double theta7, string type)
        {
            var T01 = HomoTransMat(d1,  theta2,  theta3,  d4,  theta5,  theta6, theta7, "01");
            var T12 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "12");
            var T23 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "23");
            var T34 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "34");
            var T45 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "45");
            var T56 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "56");
            var T67 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "67");
            var T78 = HomoTransMat(d1, theta2, theta3, d4, theta5, theta6, theta7, "78");

            Matrix<double> T = null;
            // 逐步计算齐次变换矩阵 T08 = T01 * T12 * T23 * T34 * T45 * T56 * T67* T78
            var T02 = T01 * T12;
            var T03 = T01 * T12 * T23 ;
            var T04 = T01 * T12 * T23 * T34;
            var T05 = T01 * T12 * T23 * T34 * T45;
            var T06 = T01 * T12 * T23 * T34 * T45 * T56;
            var T07 = T01 * T12 * T23 * T34 * T45 * T56 * T67;
            var T08 = T01 * T12 * T23 * T34 * T45 * T56 * T67 * T78;

            //T18=T12 * T23 * T34 * T45 * T56 * T67* T78
            var T18 = T12 * T23 * T34 * T45 * T56 * T67 * T78;
            var T28 = T23 * T34 * T45 * T56 * T67 * T78;
            var T38 = T34 * T45 * T56 * T67 * T78;
            var T48 = T45 * T56 * T67 * T78;
            var T58 = T56 * T67 * T78;
            var T68 = T67 * T78;


            // 根据type选择不同的矩阵
            switch (type){
                case "01":
                    T = T01;
                    break;
                case "02":
                    T = T02;
                    break;
                case "03":
                    T = T03;
                    break;
                case "04":
                    T = T04;
                    break;
                case "05":
                    T = T05;
                    break;
                case "06":
                    T = T06;
                    break;
                case "07":
                    T = T07;
                    break;
                case "08":
                    T = T08;
                    break;
                case "18":
                    T = T18;
                    break;
                case "28":
                    T = T28;
                    break;
                case "38":
                    T = T38;
                    break;
                case "48":
                    T = T48;
                    break;
                case "58":
                    T = T58;
                    break;
                case "68":
                    T = T68;
                    break;
                case "78":
                    T = T78;
                    break;
            }

            return T;
        }

        /// <summary>
        /// 角度偏置转换
        /// </summary>
        public static double[] convertJoint(double V1, double V2, double V3, double V4, double V5, double V6, double V7)
        {
            // 偏置补偿
            //double O1 = 3288;
            //double O2 = Math.PI / 2;
            //double O3 = -Math.PI / 2;
            //double O4 = 2281;
            //double O5 = -Math.PI / 2;
            //double O6 = Math.PI / 2;
            //double O7 = 0;

            double O1 = global.offset[0];
            double O2 = global.offset[1];
            double O3 = global.offset[2];
            double O4 = global.offset[3];
            double O5 = global.offset[4];
            double O6 = global.offset[5];
            double O7 = global.offset[6];

            // 计算偏置补偿后的theta
            double[] theta = new double[7];
            theta[0] = V1 + O1;
            theta[1] = V2  + O2; // 度转弧度
            theta[2] = V3  + O3;
            theta[3] = V4 +  O4;
            theta[4] = V5  + O5;
            theta[5] = V6  + O6;
            theta[6] = V7  + O7;

            return theta;
        }
}

    
    public static class global
    {
        public static Vector3 t_position = new Vector3(-0.6229f, -3.314f, 7.625f); //【目标初始位置】
        //public static Vector3 t_position = new Vector3(-4.385f, -0.903f, 10.256f); //【目标初始位置】

        public static Vector3 r_position_0 = new Vector3(0.0f,0.0f,0.0f);
        public static Vector3 r_position_1 = new Vector3(0.0f, 0.0f, 7.483f);  //【大臂移动-前段】
        public static Vector3 r_position_2 = new Vector3(0.0f, 0.0f, 7.305f); //【小臂回旋】

        public static Vector3 r_position_3 = new Vector3(0.0f, 0.2305f, 0.0f); 
        public static Vector3 r_position_4 = new Vector3(0.0f, 0.0f, -0.04f);//【小臂俯仰】
        public static Vector3 r_position_5 = new Vector3(0.0f, 0.559f, 0.0f);//【小臂俯仰-上-辅助】
        public static Vector3 r_position_6 = new Vector3(0.0f, 0.0f, 3.47f);//【】
        public static Vector3 r_position_7 = new Vector3(0.0f, 0.0f, 1.811f);//【腕部平摆】
        public static Vector3 r_position_8 = new Vector3(0.0f, -0.741f, 0.0f);
        public static Vector3 r_position_9 = new Vector3(0.0f, 0.0f, 0.28f);//【腕部俯仰】
        public static Vector3 r_position_10 = new Vector3(0.0f,0.070114f, 0.0f);//【腕部翻滚】
        public static Vector3 r_position_11 = new Vector3(0.0f, 0.0f, 0.587431f);//【末端执行器】

        public static Vector3 w_position_0 = r_position_0;
        public static Vector3 w_position_1 = r_position_0 + r_position_1;
        public static Vector3 w_position_2 = r_position_0 + r_position_1 + r_position_2;
        public static Vector3 w_position_3 = r_position_0 + r_position_1 + r_position_2 + r_position_3;
        public static Vector3 w_position_4 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4;
        public static Vector3 w_position_5 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5;
        public static Vector3 w_position_6 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6;
        public static Vector3 w_position_7 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6 + r_position_7;
        public static Vector3 w_position_8 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6 + r_position_7 + r_position_8;
        public static Vector3 w_position_9 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6 + r_position_7 + r_position_8 + r_position_9;
        public static Vector3 w_position_10 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6 + r_position_7 + r_position_8 + r_position_9 + r_position_10;
        public static Vector3 w_position_11 = r_position_0 + r_position_1 + r_position_2 + r_position_3 + r_position_4 + r_position_5 + r_position_6 + r_position_7 + r_position_8 + r_position_9 + r_position_10 + r_position_11;

        public static float[] state;  //机器人状态
        public static Object targetPoint;
        public static Scene scene = null;
        public static int num_step = 0;

        // MD-H
        public static double[] alpha;
        public static double[] theta;
        public static double[] a;
        public static double[] d;

        // 关节偏置补偿
        public static double[] offset;


        //角度限制
        public static Matrix<double> q_lim = Matrix<double>.Build.DenseOfArray(new double[,] {
            { 0, 11500},
            { -180 * (Math.PI / 180), 180 * (Math.PI / 180) },
            { -35 * (Math.PI / 180), 40 * (Math.PI / 180) },
            { 0, 3000},
            { -75 * (Math.PI / 180), 75 * (Math.PI / 180) },
            { -105 * (Math.PI / 180), 30 * (Math.PI / 180) },
            { -180 * (Math.PI / 180), 180 * (Math.PI / 180) }
        });

    }
}
