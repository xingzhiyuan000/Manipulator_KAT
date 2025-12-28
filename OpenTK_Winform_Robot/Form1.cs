using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK_Winform_Robot.Meshes;
using ClosedXML.Excel;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace OpenTK_Winform_Robot
{
    public partial class Form1 : Form
    {
        public static Form1 form1;
        public Form1()
        {
            InitializeComponent();
            form1 = this;

        }
        //private int indexBufferHandle; //EBO缓存索引
        //private int vertexArrayHandel; //VAO索引
        Matrix4 viewMatrix = Matrix4.Identity; //摄像机投影矩阵（由世界坐标系→摄像机坐标系）
        Matrix4 transform = Matrix4.Identity;
        Matrix4 orthoMatrix = Matrix4.Identity; //【正交投影】变换矩阵
        Matrix4 perspectiveMatrix = Matrix4.Identity; //【透视投影】变换矩阵
        Matrix4 projectionMatrix = Matrix4.Identity; //【投影】变换矩阵
        //int indicesCount;  //索引个数

        Point glMousePt;  // 鼠标位置
        bool mLeftMouseDown = false;
        bool mRightMouseDown = false;
        bool mMiddleMouseDown = false;

        float mCurrentX = 0.0f;
        float mCurrentY = 0.0f;
        private float _mouseSensitivity = 0.2f; //旋转敏感度 
        private float _moveSensitivity = 0.005f; //平移敏感度 
        private float _scaleSensitivity; //缩放敏感度 
        private float _gameSensitivity = 0.1f; //移动敏感度 
        private int projectionIndex = 1;  //投影类型：1-透视投影 2-正交投影
        private int cameraIndex = 1;  //相机类型：1-轨迹球相机 2-游戏相机
        private float deltaScale = 1;
        Camera camera = new Camera();
        Geometry geometry = new Geometry();
        //Vector3 lightDirection = new Vector3(-1.0f, -1.0f, -1.0f);  //光照方向;
        //Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);  //光照颜色;
        float specularIntensity;
        float shiness;
        Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.2f);  //环境光强度;

        Renderer renderer = null;
        //List<Mesh> meshes= null;

        Light light = null;

        Object zouTai; //走台

        Object robotModel; //导入的模型
        Object mojiModel; //导入的模型
        Object MJ01; //导入的磨机模型
        Object MJ02; //导入的模型
        Object MJ03; //导入的模型
        Object MJ04; //导入的模型
        Object CB01; //导入的模型-衬板
        Object Car01; //导入的模型-整体运输小车
        Object Car02; //导入的模型-运输小车托板
        Object eeAxis; //导入的模型-末端执行器坐标系

        List<Joint> listJoints; //骨骼列表
        Object joint = new Object(); //机器人关节
        Vector4 endEffectorPos;
        Vector4 endEffectorPos10; //末端关节位置数据

        Object find_joint1 = new Object(); //大臂移动关节-外
        Object find_joint2 = new Object(); //大臂移动关节-内
        Object find_joint3 = new Object(); //小臂回旋
        Object find_joint4 = new Object(); //小臂俯仰
        Object find_joint5 = new Object(); //小臂移动-中
        Object find_joint6 = new Object(); //小臂移动-端
        Object find_joint7 = new Object(); //腕部平摆
        Object find_joint8 = new Object(); //腕部俯仰
        Object find_joint9 = new Object(); //腕部滚摆

        Object find_EE = new Object(); //末端执行器

        private Drawline[] drawLine = new Drawline[2]; // 绘制轨迹曲线
        Vector3 point_PP;
        public int indexLine;
                                       
        private void Form1_Load(object sender, EventArgs e)
        {
            drawLine[0] = new Drawline(); //实例化轨迹绘制
            drawLine[1] = new Drawline(); //实例化轨迹绘制
            drawLine[0].lineColor = new Vector3(1.0f, 0.0f, 0.0f);
            drawLine[1].lineColor = new Vector3(0.0f, 0.0f, 1.0f);

            this.StartPosition = FormStartPosition.CenterScreen;
            CheckForIllegalCrossThreadCalls = false; //取消跨线程屏蔽

            //平行光：参数（方向，光强） uniform变量形式
            specularIntensity = trackBar1.Value / 10.0f; ;   //高光强度
            shiness = 32.0f;
            _scaleSensitivity = trackBar4.Value / 10.0f;
            _moveSensitivity = trackBar5.Value / 1000.0f;

            init_Pos();     //初始化大臂和小臂的位置
            init_MDH();     //初始化MD-H
            init_Offset(); //初始化关节偏置
            InitChart();   // 初始化表格
            //this.WindowState = FormWindowState.Maximized;   // 最大化窗口
        }

        //初始化关节偏置
        private void init_Offset()  
        {
            global.offset = new double[7];

            global.offset[0] = Convert.ToDouble(Txt_q1_offset.Text.ToString());
            global.offset[1] = Convert.ToDouble(Txt_q2_offset.Text.ToString());
            global.offset[2] = Convert.ToDouble(Txt_q3_offset.Text.ToString());
            global.offset[3] = Convert.ToDouble(Txt_q4_offset.Text.ToString());
            global.offset[4] = Convert.ToDouble(Txt_q5_offset.Text.ToString());
            global.offset[5] = Convert.ToDouble(Txt_q6_offset.Text.ToString());
            global.offset[6] = Convert.ToDouble(Txt_q7_offset.Text.ToString());

        }
        //初始化MD-H
        private void init_MDH()  
        {
            global.alpha = new double[8];
            global.a = new double[8];
            global.theta = new double[8];
            global.d = new double[8];


            global.alpha[0] = Convert.ToDouble(Txt_alpha_1.Text.ToString());
            global.alpha[1] = Convert.ToDouble(Txt_alpha_2.Text.ToString());
            global.alpha[2] = Convert.ToDouble(Txt_alpha_3.Text.ToString());
            global.alpha[3] = Convert.ToDouble(Txt_alpha_4.Text.ToString());
            global.alpha[4] = Convert.ToDouble(Txt_alpha_5.Text.ToString());
            global.alpha[5] = Convert.ToDouble(Txt_alpha_6.Text.ToString());
            global.alpha[6] = Convert.ToDouble(Txt_alpha_7.Text.ToString());

            global.a[0] = Convert.ToDouble(Txt_a_1.Text.ToString());
            global.a[1] = Convert.ToDouble(Txt_a_2.Text.ToString());
            global.a[2] = Convert.ToDouble(Txt_a_3.Text.ToString());
            global.a[3] = Convert.ToDouble(Txt_a_4.Text.ToString());
            global.a[4] = Convert.ToDouble(Txt_a_5.Text.ToString());
            global.a[5] = Convert.ToDouble(Txt_a_6.Text.ToString());
            global.a[6] = Convert.ToDouble(Txt_a_7.Text.ToString());

            global.theta[0] = Convert.ToDouble(Txt_theta_1.Text.ToString());
            global.theta[3] = Convert.ToDouble(Txt_theta_4.Text.ToString());

            global.d[1] = Convert.ToDouble(Txt_d_2.Text.ToString());
            global.d[2] = Convert.ToDouble(Txt_d_3.Text.ToString());
            global.d[4] = Convert.ToDouble(Txt_d_5.Text.ToString());
            global.d[5] = Convert.ToDouble(Txt_d_6.Text.ToString());
            global.d[6] = Convert.ToDouble(Txt_d_7.Text.ToString());

        }


        private void InitChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            //chart1.Legends.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            // 关闭主轴和副轴的网格线
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY2.MajorGrid.Enabled = false;

            // 设置X轴刻度间隔为5
            area.AxisX.Interval = 5;

            // 启用右侧的Y轴
            area.AxisY2.Enabled = AxisEnabled.True;
            area.AxisY2.Title = "Prismatic (mm)";
            area.AxisY.Title = "Rotation (°)";

            area.AxisY2.TitleFont = new Font("Times New Roman", 14F);
            area.AxisY.TitleFont = new Font("Times New Roman", 14F);

            //// 添加图例
            //Legend legend = new Legend("MainLegend");
            //legend.Docking = Docking.Top;               // 设置在顶部
            //legend.Alignment = StringAlignment.Center;  // 居中对齐
            //legend.LegendStyle = LegendStyle.Row;       // 横向排列
            //chart1.Legends.Add(legend);

            // 添加多个曲线（Series）
            for (int i = 0; i < 7; i++)
            {
                Series series = new Series("Joint q" + (i + 1));
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 2;

                // 设定 Y 轴类型：前两个使用右侧（Y2），其余默认使用左侧（Y）
                if (i + 1 == 1 || i + 1 == 4)
                    series.YAxisType = AxisType.Secondary; // 使用右边Y轴
                else
                    series.YAxisType = AxisType.Primary;   // 使用左边Y轴
   

                switch (i + 1)
                {
                    case 1:
                        series.Color = Color.Blue;
                        break;
                    case 2:
                        series.BorderDashStyle = ChartDashStyle.Dash;
                        series.Color = Color.Black;
                        break;
                    case 3:
                        series.BorderDashStyle = ChartDashStyle.Dash;
                        series.Color = Color.FromArgb(1, 255, 1);
                        break;
                    case 4:
                        series.Color = Color.Red;
                        break;
                    case 5:
                        series.BorderDashStyle = ChartDashStyle.Dash;
                        series.Color = Color.Orange;
                        break;
                    case 6:
                        series.BorderDashStyle = ChartDashStyle.Dash;
                        series.Color = Color.Purple;
                        break;
                    case 7:
                        series.BorderDashStyle = ChartDashStyle.Dash;
                        series.Color = Color.FromArgb(0, 254, 253);
                        break;
                }


                chart1.Series.Add(series);
            }

            // 设置坐标轴自动缩放
            //chart1.ChartAreas[0].AxisX.Minimum = Double.NaN;
            //chart1.ChartAreas[0].AxisX.Maximum = Double.NaN;

            // 设置坐标轴最大值和最小值
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100;

            chart1.ChartAreas[0].AxisY.Minimum = -180;
            chart1.ChartAreas[0].AxisY.Maximum = 40;

            chart1.ChartAreas[0].AxisY2.Minimum = 0;
            chart1.ChartAreas[0].AxisY2.Maximum = 10000;
        }

        /// <summary>
        /// 自动添加点
        /// </summary>
        private void AddPointToSeries(int seriesIndex, double xValue, double yValue)
        {
            if (seriesIndex < 0 || seriesIndex >= chart1.Series.Count)
                return;

            Series series = chart1.Series[seriesIndex];

            // 添加新点
            series.Points.AddXY(xValue, yValue);

            // 如果点数超过100，删除最前面的点（即老的点）
            if (series.Points.Count > 100)
            {
                series.Points.RemoveAt(0);
            }

            // 可选：自动调整 X 轴显示范围
            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        /// <summary>
        /// 摄像机【放大缩小】
        /// </summary>
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            deltaScale = e.Delta / Math.Abs(e.Delta);
            if (projectionIndex == 1) //【透视投影相机】
            {
                // e.Delta > 0 表示向上滚动

                //Debug.WriteLine(deltaScale.ToString());
                Vector3 currentFront = Vector3.Cross(camera._up, camera._right);
                camera._position += (currentFront * deltaScale * _scaleSensitivity);
            }
            else
            {
                float oScale = (float)Math.Pow(2, -deltaScale * _scaleSensitivity);
                camera.oLeft *= oScale;
                camera.oRight *= oScale;
                camera.oBottom *= oScale;
                camera.oTop *= oScale;
                projectionMatrix = camera.GetOrthoMatrix();

            }
            glControl1_Paint(null, null); //重新绘制
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

            this.glControl1.MouseWheel += new MouseEventHandler(glControl1_MouseWheel); //滚轮事件
            
            //抗锯齿
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.Multisample);


            //GL.Enable(EnableCap.PolygonOffsetFill); //开启面【深度偏移功能】
            //GL.PolygonOffset(1.0f,1.0f); //【消除ZFighting】

            //GL.ClearColor(new Color4(128 / 255f, 255 / 255f, 128 / 255f, 1.0f)); //设置清理颜色
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); //设置【白色背景】

            timer1.Interval = 1000 / 60;  //【60HZ】
            timer1.Enabled = true;

           
            prepareCamera(); //准备并初始化【相机】矩阵

            if (projectionIndex == 1) preparePerspective(); //【透视投影】相机
            else prepareOrtho(); //【正交投影】相机

            //prepareShader(); //4.【Shader操作】

            prepare();  //【各种准备】

            find_joint1 = findJoint(global.scene, "坐标1_大臂移动");
            find_joint2 = findJoint(global.scene, "坐标1_大臂移动V2");

            find_joint3 = findJoint(global.scene, "坐标2_小臂回旋");
            find_joint4 = findJoint(global.scene, "坐标3_小臂俯仰");

            find_joint5 = findJoint(global.scene, "坐标4_小臂移动");
            find_joint6 = findJoint(global.scene, "坐标4_小臂移动V2");

            find_joint7 = findJoint(global.scene, "坐标5_腕部平摆");
            find_joint8 = findJoint(global.scene, "坐标6_腕部俯仰");
            find_joint9 = findJoint(global.scene, "坐标7_腕部滚摆");

            global.targetPoint = findJoint(global.scene, "目标点");

            
            // ------分离模型-----
            CB01 = findJoint(robotModel, "衬板");
            Car01=findJoint(robotModel, "运输小车");
            Car02= findJoint(robotModel, "回转台");
            eeAxis= findJoint(robotModel, "末端执行器坐标系");

        }

        /// <summary>
        /// 【准备相机】
        /// </summary>
        private void prepareCamera()
        {
            viewMatrix = camera.GetViewMatrix();
            //viewMatrix =Matrix4.LookAt(new Vector3(5.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 0.0f), _up);
        }

        /// <summary>
        /// 【正交投影矩阵】
        /// </summary>
        private void prepareOrtho()
        {
            //orthoMatrix = Matrix4.CreateOrthographicOffCenter(-2.0f, 2.0f, -2.0f, 2.0f, 2.0f, -2.0f);// 创建正交投影矩阵
            projectionMatrix = camera.GetOrthoMatrix();

        }


        /// <summary>
        /// 【透视投影矩阵】
        /// </summary>
        private void preparePerspective()
        {
            float fov = MathHelper.DegreesToRadians(60.0f); //【视张角】
            float aspectRatio = (float)glControl1.Width / (float)glControl1.Height; //【宽高比】
            //float near = 0.01f; //【近平面】-距离相机的距离
            //float far = 10000.0f; //【远平面】-距离相机的距离
            // 创建【透视投影矩阵】
            projectionMatrix = camera.GetPerspectiveMatrix(fov, aspectRatio, camera.pNear, camera.pFar);
            //perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
        }


        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Size.Width, glControl1.Size.Height);
            glControl1.Invalidate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GL.UseProgram(0);
            timer1.Enabled = false;
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt = MousePosition; //获取鼠标位置
            glMousePt = this.glControl1.PointToClient(pt);
            mCurrentX = glMousePt.X;
            mCurrentY = glMousePt.Y;
            // 判断按下的鼠标按钮
            if (e.Button == MouseButtons.Left)
            {
                mLeftMouseDown = true;
                mRightMouseDown = false;
                mMiddleMouseDown = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                mLeftMouseDown = false;
                mRightMouseDown = true;
                mMiddleMouseDown = false;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                mLeftMouseDown = false;
                mRightMouseDown = false;
                mMiddleMouseDown = true;
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            mLeftMouseDown = false;
            mRightMouseDown = false;
            mMiddleMouseDown = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = MousePosition; //获取鼠标位置
            glMousePt = this.glControl1.PointToClient(pt);

            //【左键】-旋转操作
            if (mLeftMouseDown)
            {
                float deltaX = (glMousePt.X - mCurrentX) * _mouseSensitivity;
                float deltaY = (glMousePt.Y - mCurrentY) * _mouseSensitivity;
                //Debug.WriteLine($"Mouse Position: X = {deltaX}, Y = {deltaY}");

                //1.计算pitch-更新up和position
                updatePitch(deltaY);
                updateYaw(deltaX);


            }
            else if (mMiddleMouseDown) //【中间】-移动操作
            {
                float deltaX = (glMousePt.X - mCurrentX) * _moveSensitivity;
                float deltaY = (glMousePt.Y - mCurrentY) * _moveSensitivity;

                camera._position += camera._up * deltaY;
                camera._position -= camera._right * deltaX;
            }
            mCurrentX = glMousePt.X;
            mCurrentY = glMousePt.Y;

            glControl1_Paint(null, null);

        }
        private float mPitch = 0.0f; //累计角度
        private void updatePitch(float angle)
        {
            Matrix4 mat = Matrix4.Identity;
            if (cameraIndex == 1) //【轨迹球相机】
            {
                //绕mRight旋转:影响up和position向量

                mat = mat * Matrix4.CreateFromAxisAngle(camera._right, MathHelper.DegreesToRadians(angle)); // 创建旋转矩阵并应用
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                //更新position向量
                Vector4 transformedPos = mat * (new Vector4(camera._position, 1.0f));
                camera._position = new Vector3(transformedPos.X, transformedPos.Y, transformedPos.Z);
            }
            else //【游戏相机】
            {
                mPitch += angle;
                if (mPitch > 89.0f || mPitch < -89.0f)
                {
                    mPitch -= angle;
                    return;
                }
                //绕mRight旋转:影响up
                mat = mat * Matrix4.CreateFromAxisAngle(camera._right, MathHelper.DegreesToRadians(angle)); // 创建旋转矩阵并应用
                                                                                                            //更新up向量
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
            }


        }

        private void updateYaw(float angle)
        {
            Matrix4 mat = Matrix4.Identity;
            if (cameraIndex == 1) //【轨迹球相机】
            {
                //绕mRight旋转:影响up和position向量

                mat = mat * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angle)); // 创建旋转矩阵并应用
                                                                                                            //更新up向量
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                //更新position向量
                Vector4 transformedPos = mat * (new Vector4(camera._position, 1.0f));
                camera._position = new Vector3(transformedPos.X, transformedPos.Y, transformedPos.Z);
                //更新right向量
                Vector4 transformedRight = mat * (new Vector4(camera._right, 1.0f));
                camera._right = new Vector3(transformedRight.X, transformedRight.Y, transformedRight.Z);
            }
            else
            {
                mat = mat * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angle)); // 创建旋转矩阵并应用
                                                                                                            //更新up向量
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                //更新right向量
                Vector4 transformedRight = mat * (new Vector4(camera._right, 1.0f));
                camera._right = new Vector3(transformedRight.X, transformedRight.Y, transformedRight.Z);
            }

        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {

            if (cameraIndex != 2) return;
            Vector3 direction = Vector3.Zero; //移动方向
            Vector3 front = Vector3.Cross(camera._up, camera._right); //前向方向
            Vector3 right = camera._right;
            // 判断按下的键
            if (e.KeyCode == Keys.W)
            {
                direction += front;
                Debug.WriteLine("W");
            }

            if (e.KeyCode == Keys.A)
            {
                direction -= right;
                Debug.WriteLine("A");
            }

            if (e.KeyCode == Keys.D)
            {
                direction += right;
                Debug.WriteLine("D");
            }

            if (e.KeyCode == Keys.S)
            {
                direction -= front;
                Debug.WriteLine("S");
            }

            if (direction.Length != 0)
            {
                direction = Vector3.Normalize(direction);
                camera._position += direction * _gameSensitivity;
            }
            glControl1_Paint(null, null);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            camera._position = new Vector3(0.0f, 0.0f, 2.0f); //【摄像机位置】-初始位置
            camera._up = Vector3.UnitY;  //【摄像机顶部】
            camera._right = Vector3.UnitX; //【摄像机右侧】

            orthoMatrix = Matrix4.CreateOrthographicOffCenter(-2.0f, 2.0f, -2.0f, 2.0f, 2.0f, -2.0f);// 创建正交投影矩阵

            float fov = MathHelper.DegreesToRadians(60.0f); //【视张角】
            float aspectRatio = (float)glControl1.Width / (float)glControl1.Height; //【宽高比】
            float near = 0.01f; //【近平面】-距离相机的距离
            float far = 100.0f; //【远平面】-距离相机的距离

            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far); // 创建【透视投影矩阵】

            if (projectionIndex == 1) projectionMatrix = perspectiveMatrix;
            else projectionMatrix = orthoMatrix;
            glControl1_Paint(null, null);

        }

        /// <summary>
        /// 【白色背景】
        /// </summary>
        private void btnColor_Click(object sender, EventArgs e)
        {
            //GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); //设置背景颜色
            GL.ClearColor(
                btnColor_White.BackColor.R / 255f,
                btnColor_White.BackColor.G / 255f,
                btnColor_White.BackColor.B / 255f,
                btnColor_White.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        /// <summary>
        /// 【绿色背景】
        /// </summary>
        private void btnColor_Green_Click(object sender, EventArgs e)
        {
            //GL.ClearColor(0.5f, 1.0f, 0.5f, 1.0f); //设置背景颜色
            GL.ClearColor(
                btnColor_Green.BackColor.R / 255f,
                btnColor_Green.BackColor.G / 255f,
                btnColor_Green.BackColor.B / 255f,
                btnColor_Green.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            specularIntensity = trackBar1.Value / 10.0f;   //高光强度
            glControl1_Paint(null, null);
        }

        /// <summary>
        /// 生成TreeView
        /// </summary>
        void getTree(Object obj, TreeNode tnParent)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() != null)
                {
                    //MessageBox.Show(children[i].GetName());
                    TreeNode tn = new TreeNode(children[i].GetName());
                    tnParent.Nodes.Add(tn);//将部件子节点添加都对应的父节点中     
                    getTree(obj.mChildren[i], tn);
                }

            }
        }

        //【根据名字设置位置】
        void setPosition(Object obj, String name)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() == name)
                {
                    children[i].SetPosition(new Vector3(5.0f, 0.0f, 0.0f));
                }
                else
                {
                    setPosition(children[i], name);
                }
            }

        }

        //【根据名字设置角度】
        void setRotateY(Object obj, String name)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() == name)
                {
                    children[i].setAngleY(90.0f);
                }
                else
                {
                    setRotateY(children[i], name);
                }
            }

        }

        //【根据名字寻找关节】
        private Object findJoint(Object obj, String name)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() == name) joint = children[i];
                else findJoint(children[i], name);
            }
            return joint;
        }

        void addEmptyObject(Object obj, Object emp_obj, String childrenName, String parentName)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() == parentName)
                {
                    children[i].addChild(emp_obj);
                }

                if (children[i].GetName() == childrenName)
                {
                    emp_obj.addChild(children[i]);
                    obj.removeChild(children[i]);
                }
                else
                {
                    addEmptyObject(children[i], emp_obj, childrenName, parentName);
                }

            }

        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            shiness = trackBar2.Value; ;   //高光强度
            glControl1_Paint(null, null);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            glControl1_Paint(null, null);
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _mouseSensitivity = trackBar3.Value / 10.0f;   //【旋转】灵敏度
            glControl1_Paint(null, null);
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            _scaleSensitivity = trackBar4.Value / 10.0f;   //【缩放】灵敏度
            glControl1_Paint(null, null);
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            _moveSensitivity = trackBar5.Value / 1000.0f;   //【移动】灵敏度
            glControl1_Paint(null, null);
        }
        int PP_Stop=1;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            //findJoint(scene, "显示坐标轴").SetPosition(new Vector3(5.0f, 0.0f, glControl1.Width / 1000.0f));

            renderer.Render(global.scene, camera, light, projectionMatrix, specularIntensity, shiness);

            if (PP_Stop==0)
            {
                // 绘制轨迹
                endObject_xyz = showEndPosition(); // 末端执行器xyz坐标
                point_PP = new Vector3(
                    (float)Math.Round((float)endObject_xyz[9], 4),
                    -(float)Math.Round((float)endObject_xyz[11], 4),
                    (float)Math.Round((float)endObject_xyz[10], 4)
                    );
                Debug.WriteLine(Math.Round((float)endObject_xyz[9], 4).ToString());
                drawLine[indexLine].AddPoint(new Vector3((float)endObject_xyz[9], -(float)endObject_xyz[11], (float)endObject_xyz[10]));
            }

            if (chk_traj.Checked) drawLine[0].Draw();  // 绘制轨迹线
            if (chk_traj.Checked) drawLine[1].Draw();  // 绘制轨迹线

            glControl1.SwapBuffers(); //双缓存

        }
        void prepare()
        {
            renderer = new Renderer();  //渲染器
            global.scene = new Scene();        //场景

            //模型读取-【机械手臂】
            //robotModel = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/OnlyRobot20241216.fbx"); // 不带衬板
            robotModel = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/Manipulator_20250326.fbx"); // 带衬板带小车
            //robotModel = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/house.fbx"); 
            
            //Object robotModel = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/Cube.fbx");

            //setModelBlend(robotModel, true, 0.8f); //设置模型的【透明度】

            robotModel.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //robotModel.SetScale(new Vector3(1.0f, 1.0f, 1.0f));

            //【Light】
            light = new Light();
            light.setDirectionalLight(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f));
            //light.setDirectionalLight(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f));



            //【设置旋转角度】
            //setRotateY(robotModel, "abc"); //【设置指定Object的位置】

            global.scene.addChild(robotModel);

            ////setPosition(robotModel,"球体"); //【设置指定Object的位置】
            //setRotateY(robotModel, "坐标2_小臂回旋"); //【设置指定Object的位置】

            ////【几何-1】
            //Geometry geometry_1 = new Geometry();
            //geometry_1 = geometry_1.createBox(2.0f);

            ////【几何-1】
            //Geometry geometry_2 = new Geometry();
            //geometry_2 = geometry_2.createBox(2.0f);

            ////【Material-1】
            //Material material_1 = new Material();
            ////material_1.mShiness = 64.0f; //高光强度

            //material_1.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/box.png", 0);
            //material_1.mSpecularMask = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/sp_mask.png", 1);
            //material_1.mType = MaterialType.PhongMaterial;
            ////【Material-2】
            //Material material_2 = new Material();
            ////material_2.mShiness = 64.0f; //高光强度

            //material_2.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/earth.png", 0);
            //material_2.mType = MaterialType.PhongMaterial;  //Shader类型
            //material_2.mSpecularMask = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/sp_mask.png", 1);

            ////【Mesh-1】
            //Mesh mesh_1 = new Mesh(geometry_1, material_1);


            ////【Mesh-2】
            //mesh_2 = new Mesh(geometry_2, material_2);
            //mesh_2.SetPosition(new Vector3(5.0f, 0.0f, 0.0f));
            //mesh_2.setAngleZ(10.0f);


            ////【创建父子关系】
            //mesh_1.addChild(mesh_2);
            //scene.addChild(mesh_1);

            //【创建骨骼】--------------------
            Mesh bones = prepareBones();
            global.scene.addChild(bones);

            ////【创建目标】--------------------
            //Mesh target = prepareTarget();
            //scene.addChild(target);

            //【创建点光源】--------------------
            //Mesh pointLight = preparePointLight();
            //global.scene.addChild(pointLight);

            //setModelBlend(pointLight, true, 0.8f); //设置模型的【透明度】

            ////【创建地面】-------------------
            //Mesh groundMesh = prepareGround();
            //scene.addChild(groundMesh);

            //【创建多实例】--------------------
            //Mesh instanceMesh = prepareInstance();
            //global.scene.addChild(instanceMesh);

            //【创建天空盒】-------------------
            //Mesh cubeMesh = prepareSkyBox();
            //scene.addChild(cubeMesh);

            //【骨骼映射】
            projectBones();

            //模型读取-【衬板】

            //CB01 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/Chenban.fbx");
            //CB01.SetName("衬板_01");
            //CB01.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //CB01.SetPosition(new Vector3(0.0f, 0.0f, 5.0f));
            //global.scene.addChild(CB01);
            //find_EE = findJoint(global.scene, "坐标7_腕部滚摆");
            //find_EE.addChild(CB01);


            //模型读取-【走台】
            //zouTai = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/ZouTai_20250912.fbx");
            //zouTai.SetName("走台");
            //zouTai.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //global.scene.addChild(zouTai);

            //模型读取-【球磨机】
            //mojiModel = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/QiuMoJi.fbx");
            ////setModelBlend(mojiModel, true, 0.8f); //设置模型的【透明度】
            //mojiModel.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //scene.addChild(mojiModel);

            //模型读取-【球磨机-MJ01】

            MJ01 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/MJ01.fbx");
            MJ01.SetName("磨机_01");
            MJ01.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //setModelBlend(MJ01, true, 0.5f);                // 设置透明度
            global.scene.addChild(MJ01);

            MJ02 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/MJ02.fbx");
            MJ02.SetName("磨机_02");
            MJ02.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ02);
            

            MJ03 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/MJ03.fbx");
            MJ03.SetName("磨机_03");
            MJ03.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ03);
            

            MJ04 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/MJ04.fbx");
            MJ04.SetName("磨机_04");
            MJ04.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ04);
            //setModelBlend(MJ04, true, 0.5f);                // 设置透明度



            //模型读取-【球磨机-高亮】
            //Material materialBound = new Material();
            //materialBound.mType = MaterialType.WhiteMaterial;

            //mojiModel.SetScale(new Vector3(0.015f, 0.015f, 0.015f));

            //scene.addChild(mojiModel);


            //----【运输小车】-----【用分离法】
            //Car01 = AssimpLoader.loadModel(Directory.GetCurrentDirectory() + "/Resources/FBX/transportCar.fbx");
            //Car01.SetName("运输小车_01");
            //Car01.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            //Car01.setAngleX(90.0f);
            //Car01.SetPosition(new Vector3(0.0f, -550.0f, 0.0f));

            //global.scene.addChild(Car01)


            //----【坐标轴】-----
            ////【几何-1】
            //Geometry axis_Geometry = new Geometry();
            //axis_Geometry = axis_Geometry.createBox(1.0f);

            ////【Material-1】
            //Material axis_Mat = new Material();

            //axis_Mat.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/box.png", 0);
            //axis_Mat.mSpecularMask = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/sp_mask.png", 1);
            //axis_Mat.mType = MaterialType.AxisMaterial;

            ////【Mesh-1】
            //Mesh axis_Mesh = new Mesh(axis_Geometry, axis_Mat);
            //axis_Mesh.SetName("显示坐标轴");

            ////axis_Mesh.SetPosition(new Vector3(5.0f, 0.0f, glControl1.Width / 1000.0f));
            //Debug.WriteLine(glControl1.Width);

            //scene.addChild(axis_Mesh);

        }


        private void projectBones()
        {
            listJoints = new List<Joint>();

            Joint joint0 = new Joint(global.scene.mChildren[1]);
            Joint joint1 = new Joint(global.scene.mChildren[1].mChildren[0]);
            Joint joint2 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0]);
            Joint joint3 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint4 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint5 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint6 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint7 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint8 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint9 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint10 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);
            Joint joint11 = new Joint(global.scene.mChildren[1].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0].mChildren[0]);

            //【设置限制条件】 //nx,ny,nz,mx,my,mz,ntrans,mtrans
            joint0.constraints(0, 0, 0, 0, 0, 0, 0, 5.6f);  //【大臂外段】
            joint1.constraints(0, 0, 0, 0, 0, 0, 0, 5.9f);  //【大臂内段】
            joint2.constraints(0, 0, -180.0f, 180.0f, 0, 0, 0, 0.0f);  //【小臂回旋】
            joint4.constraints(-40.0f, 35.0f, 0, 0, 0, 0, 0, 0);  //【小臂俯仰】
            joint6.constraints(0, 0, 0, 0, 0, 0, 0, 1.5f);  //【小臂中段】
            joint7.constraints(0, 0, 0, 0, 0, 0, 0, 1.5f);  //【小臂前段】
            joint8.constraints(0, 0, -75.0f, 75.0f, 0, 0, 0, 0);  //【腕部平摆】
            joint9.constraints(-105.0f, 30.0f, 0, 0, 0, 0, 0, 0);  //【腕部俯仰】
            joint10.constraints(0, 0, 0, 0, -180.0f, 180.0f, 0, 0);  //【腕部滚摆】


            listJoints.Add(joint0);
            listJoints.Add(joint1);
            listJoints.Add(joint2);
            listJoints.Add(joint3);
            listJoints.Add(joint4);
            listJoints.Add(joint5);
            listJoints.Add(joint6);
            listJoints.Add(joint7);
            listJoints.Add(joint8);
            listJoints.Add(joint9);
            listJoints.Add(joint10);
            listJoints.Add(joint11);
        }
        /// <summary>
        /// 【创建多实例】
        /// </summary>
        private Mesh prepareInstance()
        {
            //【几何-目标】
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            //【Material】
            Material material = new Material();

            material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/earth.png", 0);
            material.mType = MaterialType.PhongInstanceMaterial;

            InstancedMesh sphereMesh = new InstancedMesh(geometry, material, 2);
            Matrix4 tranform0 = Matrix4.Identity;
            Matrix4 tranform1 = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 10.0f)) * tranform0;
            sphereMesh.mInstanceMatrices[0] = tranform0;
            sphereMesh.mInstanceMatrices[1] = tranform1;

            return sphereMesh;
        }
        private Mesh preparePointLight()
        {
            //【几何-目标】
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            //【Material】
            Material material = new Material();
            material.mType = MaterialType.WhiteMaterial;

            //【Mesh】
            Mesh mesh = new Mesh(geometry, material);
            mesh.SetName("目标点");
            mesh.SetPosition(global.t_position);

            return mesh;
        }

        private Mesh prepareTarget()
        {
            //【几何-目标】
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            //【Material】
            Material material = new Material();

            material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/target.jpg", 0);
            material.mType = MaterialType.PhongMaterial;

            //【Meshe】
            Mesh mesh = new Mesh(geometry, material);


            mesh.SetName("目标");

            mesh.SetPosition(global.t_position);

            return mesh;
        }

        /// <summary>
        /// 【骨骼】
        /// </summary>
        private Mesh prepareBones()
        {
            //【几何-1】
            Geometry geometry = new Geometry();
            geometry = geometry.createBox(0.001f);  //0.1

            //【Material】
            Material material = new Material();

            material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/bone.png", 0);
            material.mType = MaterialType.PhongMaterial;
            
            //material.mSpecularMask = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/sp_mask.png", 1);

            //【Meshes】
            Mesh mesh_0 = new Mesh(geometry, material);
            Mesh mesh_1 = new Mesh(geometry, material);
            Mesh mesh_2 = new Mesh(geometry, material);
            Mesh mesh_3 = new Mesh(geometry, material);
            Mesh mesh_4 = new Mesh(geometry, material);
            Mesh mesh_5 = new Mesh(geometry, material);
            Mesh mesh_6 = new Mesh(geometry, material);
            Mesh mesh_7 = new Mesh(geometry, material);
            Mesh mesh_8 = new Mesh(geometry, material);
            Mesh mesh_9 = new Mesh(geometry, material);
            Mesh mesh_10 = new Mesh(geometry, material);
            Mesh mesh_11 = new Mesh(geometry, material);

            //setModelBlend(mesh_11, true, 0.0f);  // 设置骨骼透明度

            mesh_0.SetName("关节0");
            mesh_1.SetName("关节1");
            mesh_2.SetName("关节2");
            mesh_3.SetName("关节3");
            mesh_4.SetName("关节4");
            mesh_5.SetName("关节5");
            mesh_6.SetName("关节6");
            mesh_7.SetName("关节7");
            mesh_8.SetName("关节8");
            mesh_9.SetName("关节9");
            mesh_10.SetName("关节10");
            mesh_11.SetName("关节11");

            mesh_0.SetPosition(global.r_position_0);
            mesh_1.SetPosition(global.r_position_1);
            mesh_2.SetPosition(global.r_position_2);
            mesh_3.SetPosition(global.r_position_3);
            mesh_4.SetPosition(global.r_position_4);
            mesh_5.SetPosition(global.r_position_5);
            mesh_6.SetPosition(global.r_position_6);
            mesh_7.SetPosition(global.r_position_7);
            mesh_8.SetPosition(global.r_position_8);
            mesh_9.SetPosition(global.r_position_9);
            mesh_10.SetPosition(global.r_position_10);
            mesh_11.SetPosition(global.r_position_11);

            mesh_0.addChild(mesh_1);
            mesh_1.addChild(mesh_2);
            mesh_2.addChild(mesh_3);
            mesh_3.addChild(mesh_4);
            mesh_4.addChild(mesh_5);
            mesh_5.addChild(mesh_6);
            mesh_6.addChild(mesh_7);
            mesh_7.addChild(mesh_8);
            mesh_8.addChild(mesh_9);
            mesh_9.addChild(mesh_10);
            mesh_10.addChild(mesh_11);

            //mesh_1.setAngleY(30.0f);
            //【创建父子关系】
            return mesh_0;
        }
        /// <summary>
        /// 【设置模型的透明度】
        /// </summary>
        private void setModelBlend(Object obj, bool blend, float opacity)
        {

            if (obj.mType == ObjectType.Mesh)
            {

                if (opacity < 1.0)
                {
                    Mesh mesh = (Mesh)obj;
                    Material material = mesh.mMaterial;
                    material.mBlend = blend;
                    material.mOpacity = opacity;
                    material.mDepthWrite = false;
                }
                else
                {
                    Mesh mesh = (Mesh)obj;
                    Material material = mesh.mMaterial;
                    material.mBlend = false;
                    material.mOpacity = opacity;
                    material.mDepthWrite = true;
                }
            }

            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                setModelBlend(children[i], blend, opacity);
            }
        }

        /// <summary>
        /// 【准备天空盒】
        /// </summary>
        private Mesh prepareSkyBox()
        {
            /////------------【天空盒相关-Cube】-------------
            //List<string> paths = new List<string>{
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/right.jpg",
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/left.jpg",
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/top.jpg",
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/bottom.jpg",
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/back.jpg",
            //    Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/front.jpg",
            //};
            //Texture envTex = new Texture(paths, 0);
            //Geometry cubeGeometry = new Geometry();
            //cubeGeometry = cubeGeometry.createBox(1.0f);
            //Material cubeMaterial = new Material();
            //cubeMaterial.mType = MaterialType.CubeMaterial;
            //cubeMaterial.mDiffuse = envTex;
            //cubeMaterial.mDepthWrite = false;  //关闭【深度写入】
            //Mesh cubeMesh = new Mesh(cubeGeometry, cubeMaterial);
            //scene.addChild(cubeMesh);
            /////------------【天空盒相关-Cube】-------------

            ///------------【天空盒相关-球形】-------------
            Texture envTex = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/skybox/sphericalMap.png", 0);
            Geometry cubeGeometry = new Geometry();
            cubeGeometry = cubeGeometry.createBox(1.0f);
            Material sphereMaterial = new Material();
            sphereMaterial.mType = MaterialType.SphereMaterial;
            sphereMaterial.mDiffuse = envTex;
            sphereMaterial.mDepthWrite = false;  //关闭【深度写入】
            sphereMaterial.mCullFace = CullFaceMode.Front;
            Mesh cubeMesh = new Mesh(cubeGeometry, sphereMaterial);
            return cubeMesh;
            ///------------【天空盒相关-球形】-------------
        }

        /// <summary>
        ///  【末端执行器】位置信息
        /// </summary>
        private double[] showEndPosition()
        {
            Matrix4 modelTransform = listJoints[11].getMesh().GetModelMatrix();
            modelTransform = Matrix4.Transpose(modelTransform);
            endEffectorPos = modelTransform * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            //Matrix4 transform = scene.mChildren[1].GetModelMatrix();  //【父系】变换矩阵
            //transform = Matrix4.Transpose(transform);
            //Vector4 worldPosition = transform *new Vector4(global.w_position_12, 1.0f);
            //lab_px.Text = "X坐标：" + (endEffectorPos.X*1000).ToString("F2") + "\r\nY坐标：" + (endEffectorPos.Y * 1000).ToString("F2") + "\r\nZ坐标：" + (endEffectorPos.Z * 1000).ToString("F2");

            lab_px.Text = (modelTransform.M14 * 1000).ToString("F2");
            lab_py.Text = (modelTransform.M24 * 1000).ToString("F2");
            lab_pz.Text = (modelTransform.M34 * 1000).ToString("F2");
            lab_nx.Text = (modelTransform.M11).ToString("F2");
            lab_ny.Text = (modelTransform.M21).ToString("F2");
            lab_nz.Text = (modelTransform.M31).ToString("F2");
            lab_ox.Text = (modelTransform.M12).ToString("F2");
            lab_oy.Text = (modelTransform.M22).ToString("F2");
            lab_oz.Text = (modelTransform.M32).ToString("F2");
            lab_ax.Text = (modelTransform.M13).ToString("F2");
            lab_ay.Text = (modelTransform.M23).ToString("F2");
            lab_az.Text = (modelTransform.M33).ToString("F2");

            Matrix4 modelTransform10 = listJoints[10].getMesh().GetModelMatrix();
            modelTransform10 = Matrix4.Transpose(modelTransform10);
            endEffectorPos10 = modelTransform10 * (new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            //label14.Text = "X坐标：" + (endEffectorPos10.X * 1000).ToString("F2") + "\r\nY坐标：" + (endEffectorPos10.Y * 1000).ToString("F2") + "\r\nZ坐标：" + (endEffectorPos10.Z * 1000).ToString("F2");
            //glControl1_Paint(null, null);
            double[] pos_xyz = new double[12];

            pos_xyz[0] = Convert.ToDouble(modelTransform.M11); // nx
            pos_xyz[1] = Convert.ToDouble(modelTransform.M21); // ny
            pos_xyz[2] = Convert.ToDouble(modelTransform.M31); // nz

            pos_xyz[3] = Convert.ToDouble(modelTransform.M12); // ox
            pos_xyz[4] = Convert.ToDouble(modelTransform.M22); // oy
            pos_xyz[5] = Convert.ToDouble(modelTransform.M32); // oz

            pos_xyz[6] = Convert.ToDouble(modelTransform.M13); // ax
            pos_xyz[7] = Convert.ToDouble(modelTransform.M23); // ay
            pos_xyz[8] = Convert.ToDouble(modelTransform.M33); // az

            pos_xyz[9] = Convert.ToDouble(modelTransform.M14);  // px
            pos_xyz[10] = Convert.ToDouble(modelTransform.M24); // py
            pos_xyz[11] = Convert.ToDouble(modelTransform.M34); // pz
            return pos_xyz;
        }

        /// <summary>
        /// 【1-大臂移动-外段】
        /// </summary>
        private void trackBar_1_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_1.Value / 10.0);
            Txt1.Text = ((float)trackBar_1.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint1(change_val);

            //Txt1.Text = trackBar_1.Value.ToString();
            //moveJoint1(trackBar_1.Value);
        }

        private void moveJoint1(float val)
        {
            //【后臂】移动
            Vector3 changedValue = new Vector3(0.0f, 0.0f, -val / 1000.0f);
            find_joint1.SetPosition(changedValue);
            //scene.mChildren[0].mChildren[0].mChildren[2].SetPosition(newPosition);

            listJoints[0].getMesh().SetPosition(changedValue);
            showEndPosition();
        }
        /// <summary>
        /// 【1-大臂移动-内段】
        /// </summary>
        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar9.Value / 10.0);
            Txt2.Text = ((float)trackBar9.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint2(change_val);
        }

        private void moveJoint2(float val)
        {
            //【前臂】移动
            Vector3 changedValue = new Vector3(0.0f, val / 1000.0f, 0.0f);

            find_joint2.SetPosition(changedValue);

            changedValue = new Vector3(0.0f, 0.0f, -val / 1000.0f);
            Vector3 offset = global.w_position_1 - global.w_position_0;
            listJoints[1].getMesh().SetPosition(offset + changedValue);

            showEndPosition();
        }
        /// <summary>
        /// 【2-小臂回旋】
        /// </summary>
        private void trackBar_2_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_2.Value / 10.0);
            Txt3.Text = change_val.ToString();
            moveJoint3(change_val);
        }

        private void moveJoint3(float val)
        {
            find_joint3.SetAngle2((float)val);
            //scene.mChildren[0].mChildren[0].mChildren[2].mChildren[1].SetAngle2((float)trackBar_2.Value);

            listJoints[2].getMesh().setAngleY((float)val);

            showEndPosition();
        }

        /// <summary>
        /// 【3-小臂俯仰】
        /// </summary>
        private void trackBar_3_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_3.Value / 10.0);
            Txt4.Text = change_val.ToString();
            moveJoint4(-change_val);
        }
        private void moveJoint4(float val)
        {
            find_joint4.SetAngle3((float)val);
            //scene.mChildren[0].mChildren[0].mChildren[2].mChildren[1].mChildren[0].SetAngle3((float)trackBar_3.Value);

            listJoints[4].getMesh().setAngleX((float)val);

            showEndPosition();
        }
        /// <summary>
        /// 【4-小臂移动-中段】
        /// </summary>
        private void trackBar_4_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_4.Value / 10.0);
            Txt5.Text = ((float)trackBar_4.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint5(change_val);
        }
        private void moveJoint5(float val)
        {
            find_joint5.SetPosition(new Vector3(0.0f, val / 1000.0f, 0.0f));
            //scene.mChildren[0].mChildren[0].mChildren[2].mChildren[1].mChildren[0].mChildren[1].SetPosition(newPosition);

            Vector3 changedValue = new Vector3(0.0f, 0.0f, val / 1000.0f);
            Vector3 offset = global.w_position_6 - global.w_position_5;
            listJoints[6].getMesh().SetPosition(offset - changedValue);

            showEndPosition();
        }
        /// <summary>
        /// 【4-小臂移动-前段】
        /// </summary>
        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar10.Value / 10.0);
            Txt6.Text = ((float)trackBar10.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint6(change_val);
        }
        private void moveJoint6(float val)
        {
            find_joint6.SetPosition(new Vector3(0.0f, val / 1000.0f, 0.0f));
            //scene.mChildren[0].mChildren[0].mChildren[2].mChildren[1].mChildren[0].mChildren[1].SetPosition(newPosition);

            Vector3 changedValue = new Vector3(0.0f, 0.0f, val / 1000.0f);
            Vector3 offset = global.w_position_7 - global.w_position_6;
            listJoints[7].getMesh().SetPosition(offset - changedValue);

            showEndPosition();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init_Pos()
        {
            //小臂前段
            trackBar10.Value = trackBar10.Maximum;
            trackBar10_Scroll(null, null);

            //小臂中段
            trackBar_4.Value = trackBar_4.Maximum;
            trackBar_4_Scroll(null, null);

            //大臂内段
            trackBar9.Value = trackBar9.Maximum;
            trackBar9_Scroll(null, null);

            //大臂外段
            trackBar_1.Value = trackBar_1.Maximum;
            trackBar_1_Scroll(null, null);

        }


        /// <summary>
        /// 透明度滑轮
        /// </summary>
        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            float opacity = trackBar8.Value / 10.0f;   //【缩放】灵敏度

            setModelBlend(robotModel, true, opacity);
            glControl1_Paint(null, null);
        }
        /// <summary>
        /// 【腕部平摆】
        /// </summary>
        private void trackBar_5_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_5.Value / 10.0);
            Txt7.Text = change_val.ToString();
            moveJoint7(change_val);
        }
        private void moveJoint7(float val)
        {
            find_joint7.SetAngle4(val);
            listJoints[8].getMesh().setAngleY(val);
            showEndPosition();
        }
        /// <summary>
        /// 【腕部俯仰】
        /// </summary>
        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar6.Value / 10.0);
            Txt8.Text = change_val.ToString();
            moveJoint8(-change_val);
        }
        private void moveJoint8(float val)
        {
            find_joint8.SetAngle5(val);
            listJoints[9].getMesh().setAngleX(val);
            showEndPosition();
        }
        /// <summary>
        /// 【腕部滚摆】
        /// </summary>
        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar7.Value / 10.0);
            Txt9.Text = change_val.ToString();
            moveJoint9(change_val);
        }
        private void moveJoint9(float val)
        {
            find_joint9.SetAngle6(-(float)val);
            listJoints[10].getMesh().setAngleZ((float)val);
            showEndPosition();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt1.Text)) return;  // 检查内容是否合法

            // 检查上下界
            if (Convert.ToDouble(Txt1.Text) > 5600) Txt1.Text = "5600.0";
            if (Convert.ToDouble(Txt1.Text) < 0) Txt1.Text = "0.0";

            trackBar_1.Value = Convert.ToInt32(trackBar_1.Maximum - Convert.ToDouble(Txt1.Text) * 10);
            moveJoint1((float)Convert.ToDouble(trackBar_1.Value / 10.0f));
        }

        private void Txt2_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt2.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt2.Text) > 5900) Txt2.Text = "5900.0";
            if (Convert.ToDouble(Txt2.Text) < 0) Txt2.Text = "0.0";

            trackBar9.Value = Convert.ToInt32(trackBar9.Maximum - Convert.ToDouble(Txt2.Text) * 10);
            moveJoint2((float)Convert.ToDouble(trackBar9.Value / 10.0f));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt3.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt3.Text) > 180) Txt3.Text = "180.0";
            if (Convert.ToDouble(Txt3.Text) < -180) Txt3.Text = "-180.0";

            moveJoint3((float)Convert.ToDouble(Txt3.Text));
            trackBar_2.Value = Convert.ToInt32(Convert.ToDouble(Txt3.Text) * 10);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt4.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt4.Text) > 40) Txt4.Text = "40.0";
            if (Convert.ToDouble(Txt4.Text) < -35) Txt4.Text = "-35.0";

            moveJoint4(-(float)Convert.ToDouble(Txt4.Text));
            trackBar_3.Value = Convert.ToInt32(Convert.ToDouble(Txt4.Text) * 10);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt5.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt5.Text) > 1500) Txt5.Text = "1500.0";
            if (Convert.ToDouble(Txt5.Text) < 0) Txt5.Text = "0.0";

            trackBar_4.Value = Convert.ToInt32(trackBar_4.Maximum - Convert.ToDouble(Txt5.Text) * 10);
            moveJoint5((float)Convert.ToDouble(trackBar_4.Value / 10.0f));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt6.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt6.Text) > 1500) Txt6.Text = "1500.0";
            if (Convert.ToDouble(Txt6.Text) < 0) Txt6.Text = "0.0";

            trackBar10.Value = Convert.ToInt32(trackBar10.Maximum - Convert.ToDouble(Txt6.Text) * 10);
            moveJoint6((float)Convert.ToDouble(trackBar10.Value / 10.0f));

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt7.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt7.Text) > 75) Txt7.Text = "75.0";
            if (Convert.ToDouble(Txt7.Text) < -75) Txt7.Text = "-75.0";

            moveJoint7((float)Convert.ToDouble(Txt7.Text));
            trackBar_5.Value = Convert.ToInt32(Convert.ToDouble(Txt7.Text) * 10);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt8.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt8.Text) > 30) Txt8.Text = "30.0";
            if (Convert.ToDouble(Txt8.Text) < -105) Txt8.Text = "-105.0";

            moveJoint8(-(float)Convert.ToDouble(Txt8.Text));
            trackBar6.Value = Convert.ToInt32(Convert.ToDouble(Txt8.Text) * 10);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt9.Text)) return;
            // 检查上下界
            if (Convert.ToDouble(Txt9.Text) > 180) Txt9.Text = "180.0";
            if (Convert.ToDouble(Txt9.Text) < -180) Txt9.Text = "-180.0";

            moveJoint9((float)Convert.ToDouble(Txt9.Text));
            trackBar7.Value = Convert.ToInt32(Convert.ToDouble(Txt9.Text) * 10);
        }

       
        /// <summary>
        /// 导入文件，并显示工作空间
        /// </summary>
        private Dictionary<int, InstancedMesh> InstancedMeshDict = new Dictionary<int, InstancedMesh>();
        private int rowCount;  // 工作空间点数
        string showPath=@"J:\同步空间\BaiduSyncdisk\Matlab\路径规划\Data";    //
        private void btn_show_workspace_Click(object sender, EventArgs e)
        {
            int instance_Lim = 100; //单个实例数目限制
            richTextBox2.Clear();

            if (btn_show_workspace.Text == "Show")
            {

                //【创建多实例】
                Geometry geometry = new Geometry();         //【几何-目标】
                geometry = geometry.createSphere(0.02f);    // 空间点大小  0.05f
                

                //【Material】
                Material material = new Material();
                //material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/yellow.png", 0);
                //material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/bone.png", 0);
                material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/green.png", 0);
                material.mType = MaterialType.PhongInstanceMaterial;
                material.mShiness = 64.0f; //高光强度

                
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.InitialDirectory = @"J:\同步空间\BaiduSyncdisk\Matlab\机器人学\Data";  //默认打开目录
                openFileDialog.InitialDirectory = showPath;  //默认打开目录
                openFileDialog.Filter = "文本文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    btn_show_workspace.Text = "Clear";

                    showPath = openFileDialog.FileName; //记录选中的目录  
                    XLWorkbook inventory_rb = new XLWorkbook(showPath);
                    IXLWorksheet worksheet = inventory_rb.Worksheet(1);  // 获取第一个工作表

                    rowCount = worksheet.RowsUsed().Count();  // 获取工作表的总行数

                    for (int i = 1; i < rowCount + 1; i++)
                    {

                        // 读取单元格内容
                        float x = (float)Convert.ToDouble(worksheet.Cell(i, 1).Value.ToString()) / 1000.0f;
                        float y = (float)Convert.ToDouble(worksheet.Cell(i, 2).Value.ToString()) / 1000.0f;
                        float z = (float)Convert.ToDouble(worksheet.Cell(i, 3).Value.ToString()) / 1000.0f;


                        string line = $"{x:F3}\t{y:F3}\t{z:F3}";
                        richTextBox2.AppendText(line + Environment.NewLine);


                        if (i == 1 || (i - 1) % instance_Lim == 0)
                        {
                            InstancedMesh sphereMesh = new InstancedMesh(geometry, material, instance_Lim);
                            sphereMesh.SetName("轨迹点");
                            InstancedMeshDict[(i - 1) / instance_Lim] = sphereMesh;
                        }
                        InstancedMeshDict[(i - 1) / instance_Lim].mInstanceMatrices[(i - 1) % instance_Lim] = Matrix4.CreateTranslation(new Vector3(x, y, z));

                    }

                    for (int i = 1; i < rowCount; i++)
                    {
                        if (i == 1 || (i - 1) % instance_Lim == 0)
                        {
                            global.scene.addChild(InstancedMeshDict[i / instance_Lim]);
                        }
                    }

                }
            }
            else
            {
                btn_show_workspace.Text = "显示工作空间";


                for (int i = 1; i < rowCount; i++)
                {
                    if (i == 1 || (i - 1) % instance_Lim == 0)
                    {
                        global.scene.removeChild(InstancedMeshDict[i / instance_Lim]);
                    }

                }
                InstancedMeshDict.Clear();


            }
            
            glControl1_Paint(null, null);
        }

        double[] q_con = new double[7]; //【初始】关节量
        double[] q = new double[7];     //【动态】关节量
        Vector<double> target_pos;      //【目标点】
        double w_k = 1e-6;
        double w_1 = 1;
        double w_2 = 1;

        private void btn_updateJoint_Click(object sender, EventArgs e)
        {

            // 创建 Stopwatch 对象
            Stopwatch timeWatch = new Stopwatch();
            timeWatch.Start(); // 开始计时

            updateJoint();

            timeWatch.Stop(); // 停止计时
            groupBox8.Text = "逆向运动学(" + timeWatch.ElapsedMilliseconds + "ms)";
        }
        /// <summary>
        /// 更新机器人姿态
        /// </summary>
        int iter_num = 1;    //单次求解迭代次数
        private (double, double, double[]) updateJoint(bool showAnimation = false)
        {
            double change_val = 0; //驱动变化量
            //【期望位姿】
            Matrix<double> d_w = Matrix<double>.Build.DenseOfArray(new double[,] {
                        {Convert.ToDouble(Txt_n_x.Text), Convert.ToDouble(Txt_o_x.Text), Convert.ToDouble(Txt_a_x.Text), Convert.ToDouble(Txt_p_x.Text)},
                        {Convert.ToDouble(Txt_n_y.Text), Convert.ToDouble(Txt_o_y.Text), Convert.ToDouble(Txt_a_y.Text), Convert.ToDouble(Txt_p_y.Text)},
                        {Convert.ToDouble(Txt_n_z.Text), Convert.ToDouble(Txt_o_z.Text), Convert.ToDouble(Txt_a_z.Text), Convert.ToDouble(Txt_p_z.Text)}
                    });
            Matrix<double> T08 = null;
            for (int i = 0; i < 7; i++)
            {
                string CMD_W_J = "0" + (i + 1).ToString();  //基坐标-关节坐标
                string CMD_J_E = (i + 1).ToString() + "8";  //关节坐标-末端执行器
                q_con = Common.convertJoint(q[0], q[1], q[2], q[3], q[4], q[5], q[6]); //初始角度-转换后
                T08 = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], "08");
                Matrix<double> T0j = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], CMD_W_J);
                Matrix<double> Tj8 = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], CMD_J_E);
                Vector<double> pc_j = Vector<double>.Build.DenseOfArray(new double[] { Tj8[0, 3], Tj8[1, 3], Tj8[2, 3] });

                if (i == 0 || i == 3)
                {
                    //【移动部分】-----------------
                    //Vector<double> Z_P = T0j.Column(2).SubVector(0, 3);
                    Vector<double> Z_P = T0j.SubMatrix(0, 3, 0, 3).Inverse() * T0j.Column(2).SubVector(0, 3);
                    double tc = pc_j.DotProduct(Z_P) / Z_P.DotProduct(Z_P);    //关节到初始垂足系数
                    Vector<double> pt_j = tc * Z_P;                        //初始垂足向量

                    Vector<double> pd_w_P = Vector<double>.Build.DenseOfArray(new double[] { target_pos[0], target_pos[1], target_pos[2], 1 });
                    Vector<double> pd_j_P = T0j.Inverse() * pd_w_P;
                    double td = pd_j_P.SubVector(0, 3).DotProduct(Z_P) / Z_P.DotProduct(Z_P);    //关节到初始垂足系数
                    Vector<double> pt_j_ = td * Z_P;

                    Vector<double> deta_d = pt_j_ - pt_j; // 计算垂足间向量
                    change_val = deta_d.L2Norm(); //计算移动关节变动量使用 L2Norm 计算范数

                    if (deta_d.DotProduct(Z_P) < 0) change_val = -change_val; // 如果条件满足，取负值
                }

                if (i == 1 || i == 2 || i == 4 || i == 5 || i == 6)
                {
                    //【旋转部分】-------------

                    Vector<double> pd_w_R = target_pos - T0j.Column(3).SubVector(0, 3);
                    Vector<double> pd_j_R = T0j.SubMatrix(0, 3, 0, 3).Inverse() * pd_w_R;

                    Matrix<double> c_j = Tj8.SubMatrix(0, 3, 0, 3);

                    Matrix<double> d_j = T0j.SubMatrix(0, 3, 0, 3).Inverse() * d_w;
                    double w_1 = w_k * ((1 + Math.Min(pc_j.L2Norm(), pd_j_R.L2Norm())) / Math.Max(pc_j.L2Norm(), pd_j_R.L2Norm()));

                    Vector<double> Z_R = T0j.SubMatrix(0, 3, 0, 3).Inverse() * T0j.Column(2).SubVector(0, 3);

                    Vector<double> a_temp = Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0, 0.0 });
                    for (int m = 0; m < 3; m++)
                    {
                        Vector<double> c_j_v = c_j.Column(m);
                        Vector<double> d_j_v = d_j.Column(m);
                        a_temp = a_temp + CrossProduct(c_j_v, d_j_v);
                    }

                    Matrix<double> A = Z_R.ToRowMatrix() * (w_1 * CrossProduct(pc_j, pd_j_R).ToColumnMatrix() + w_2 * a_temp.ToColumnMatrix());

                    double b_temp = 0.0;
                    for (int n = 0; n < 3; n++)
                    {
                        Vector<double> c_j_v = c_j.Column(n);
                        Vector<double> d_j_v = d_j.Column(n);
                        b_temp = b_temp + d_j_v.DotProduct(c_j_v) - d_j_v.DotProduct(Z_R) * Z_R.DotProduct(c_j_v);
                    }
                    double B = w_1 * pd_j_R.DotProduct(pc_j) - w_1 * pd_j_R.DotProduct(Z_R) + w_2 * b_temp;

                    //Vector<double> a = CrossProduct(Z_R, pc_j); // 计算叉乘
                    //double A = pd_j_R.DotProduct(a);
                    //double B = pd_j_R.DotProduct(pc_j) - pd_j_R.DotProduct(Z_R) * Z_R.DotProduct(pc_j);

                    double beta = Math.Atan2(B, A[0, 0]);
                    change_val = Math.PI / 2 - beta;


                    // 循环查找合适的 change_val
                    double temp;
                    for (int j = -2; j <= 2; j++)
                    {
                        temp = 2 * j * Math.PI + Math.PI / 2 - beta;
                        if (q[i] + temp > global.q_lim[i, 0] && q[i] + temp < global.q_lim[i, 1])
                        {
                            change_val = temp;
                            break;
                        }
                    }
                }
                //限制对比
                if (q[i] + change_val < global.q_lim[i, 0]) change_val = global.q_lim[i, 0] - q[i];
                if (q[i] + change_val > global.q_lim[i, 1]) change_val = global.q_lim[i, 1] - q[i];
                q[i] = q[i] + change_val; // 更新驱动关节

                //将新关节更新到界面上
                if (showAnimation)
                {
                    //【关节1】
                    if (q[0] > 5600)
                    {
                        Txt1.Text = "5600.0";
                        Txt2.Text = (q[0] - 5600).ToString();
                    }
                    else
                    {
                        Txt1.Text = q[0].ToString();
                        Txt2.Text = "0.0";
                    }

                    //【关节4】
                    if (q[3] > 1500)
                    {
                        Txt5.Text = "1500.0";
                        Txt6.Text = (q[3] - 1500).ToString();
                    }
                    else
                    {
                        Txt5.Text = q[3].ToString();
                        Txt6.Text = "0.0";
                    }
                    Txt3.Text = (q[1] * (180 / Math.PI)).ToString();
                    Txt4.Text = (q[2] * (180 / Math.PI)).ToString();

                    Txt7.Text = (q[4] * (180 / Math.PI)).ToString();
                    Txt8.Text = (q[5] * (180 / Math.PI)).ToString();
                    Txt9.Text = (q[6] * (180 / Math.PI)).ToString();
                }
            }

            //【位置误差】
            double e1 = (target_pos - T08.Column(3).SubVector(0, 3)).L2Norm();
            //【姿态误差】
            Matrix<double> d = d_w;
            Matrix<double> c = T08.SubMatrix(0, 3, 0, 3);
            double e2 = 0.0;
            for (int u = 0; u < 3; u++)
            {
                Vector<double> d_v = d.Column(u);
                Vector<double> c_v = c.Column(u);
                e2 = e2 + Math.Pow(d_v.DotProduct(c_v) - 1, 2);
            }
            //Thread.Sleep(2000); //暂停


            richTextBox3.AppendText(iter_num.ToString() + ":" + Math.Round(e2, 2) + System.Environment.NewLine);


            richTextBox3.SelectionStart = richTextBox3.Text.Length; // 将光标移动到最后一行
            richTextBox3.ScrollToCaret(); // 滚动到光标处

            iter_num = iter_num + 1;

            return (e1, e2, q);
        }

        /// <summary>
        /// 向量外积
        /// </summary>
        static Vector<double> CrossProduct(Vector<double> a, Vector<double> b)
        {
            if (a.Count != 3 || b.Count != 3)
            {
                throw new ArgumentException("Both vectors must be three-dimensional.");
            }

            // 计算叉乘
            double x = a[1] * b[2] - a[2] * b[1];
            double y = a[2] * b[0] - a[0] * b[2];
            double z = a[0] * b[1] - a[1] * b[0];

            return Vector.Build.DenseOfArray(new double[] { x, y, z });
        }

        /// <summary>
        /// 正向运动学
        /// </summary>
        private void btn_FK_Click(object sender, EventArgs e)
        {
            double V1 = 6658.2;
            double V2 = 49.7;
            double V3 = 35.5;
            double V4 = 2798.1;
            double V5 = -20.3;
            double V6 = -6.35;
            double V7 = 111.5;
            double[] q = Common.convertJoint(V1, V2 * (Math.PI / 180), V3 * (Math.PI / 180), V4, V5 * (Math.PI / 180), V6 * (Math.PI / 180), V7 * (Math.PI / 180)); //初始角度

            // 计算齐次变换矩阵
            Matrix<double> T = Common.Fkine_LH4500(q[0], q[1], q[2], q[3], q[4], q[5], q[6], "08");
            // 输出结果到 richTextBox1
            richTextBox1.Clear();

            for (int i = 0; i < 4; i++)
            {
                string line = "";
                for (int j = 0; j < 4; j++)
                {
                    line += $"{T[i, j].ToString("F1")}\t";
                }
                richTextBox1.AppendText(line + System.Environment.NewLine);
            }
        }


        /// <summary>
        /// 磨机显示控制
        /// </summary>
        private void chk_MJ01_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_MJ01.Checked) global.scene.removeChild(MJ01);
            else global.scene.addChild(MJ01);
        }

        private void chk_MJ02_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_MJ02.Checked) global.scene.removeChild(MJ02);
            else global.scene.addChild(MJ02);
        }

        private void chk_MJ03_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_MJ03.Checked) global.scene.removeChild(MJ03);
            else global.scene.addChild(MJ03);
        }

        private void chk_MJ04_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_MJ04.Checked) global.scene.removeChild(MJ04);
            else global.scene.addChild(MJ04);
        }




        double num_fresh;
        double num_current;
        double[] move_step;
        int move_joint;
        double[] q_star;
        

        /// <summary>
        /// 寻找目标
        /// </summary>
        //double q_current;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (num_current > num_fresh)
            {
                num_current = 0;
                move_joint += 1;
            }

            if (move_joint > 4)
            {
                Task.Delay(1000).Wait(); // 异步方式，但阻塞调用者
                timer2.Enabled = false;

                num_current = 0;
                timer3.Interval = 50;
                timer3.Enabled = true;
            }

            switch (move_joint)
            {
                case 1: // 【关节1动画】
                    double q_current_1 = num_current * move_step[0];

                    if (q_current_1 > 5600)
                    {
                        Txt1.Text = "5600.0";
                        Txt2.Text = (q_current_1 - 5600).ToString();
                    }
                    else
                    {
                        Txt1.Text = q_current_1.ToString();
                        Txt2.Text = "0.0";
                    }
                    break;

                case 2: // 【关节2动画】
                    double q_current_2 = num_current * move_step[1];
                    Txt3.Text = q_current_2.ToString();
                    break;

                case 3: // 【关节3动画】
                    double q_current_3 = num_current * move_step[2];
                    Txt4.Text = q_current_3.ToString();
                    break;

                case 4: // 【关节4动画】
                    double q_current_4 = num_current * move_step[3];
                    if (q_current_4 > 1500)
                    {
                        Txt5.Text = "1500.0";
                        Txt6.Text = (q_current_4 - 1500).ToString();
                    }
                    else
                    {
                        Txt5.Text = q_current_4.ToString();
                        Txt6.Text = "0.0";
                    }


                    // 【关节5动画】
                    double q_current_5 = num_current * move_step[4];
                    Txt7.Text = q_current_5.ToString();


                    // 【关节6动画】
                    double q_current_6 = num_current * move_step[5];
                    Txt8.Text = q_current_6.ToString();


                    // 【关节7动画】
                    double q_current_7 = num_current * move_step[6];
                    Txt9.Text = q_current_7.ToString();
                    break;

            }
            num_current += 1;
        }

        /// <summary>
        /// 【返回】
        /// </summary>
        double[] endObject_xyz;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (move_joint > 4) move_joint = 4;

            if (move_joint < 1)
            {
                //btn_IK_Click(null, null);
                timer3.Enabled = false;
            }

            if (num_current > num_fresh)
            {
                num_current = 0;
                move_joint -= 1;
            }

            // 目标点跟随
            //Object endObject = findJoint(global.scene, "目标点");
            endObject_xyz = showEndPosition();
            global.targetPoint.SetPosition(new Vector3((float)endObject_xyz[9], (float)endObject_xyz[10], (float)endObject_xyz[11]));

            switch (move_joint)
            {
                case 1: // 【关节1动画】
                    double q_current_1 = (num_fresh - num_current) * move_step[0];
                    if (q_current_1 > 5600)
                    {
                        Txt1.Text = "5600.0";
                        Txt2.Text = (q_current_1 - 5600).ToString();
                    }
                    else
                    {
                        Txt1.Text = q_current_1.ToString();
                        Txt2.Text = "0.0";
                    }
                    break;

                case 2: // 【关节2动画】
                    double q_current_2 = (num_fresh - num_current) * move_step[1];
                    Txt3.Text = q_current_2.ToString();
                    break;

                case 3: // 【关节3动画】
                    double q_current_3 = (num_fresh - num_current) * move_step[2];
                    Txt4.Text = q_current_3.ToString();
                    break;

                case 4: // 【关节4动画】
                    double q_current_4 = (num_fresh - num_current) * move_step[3];
                    if (q_current_4 > 1500)
                    {
                        Txt5.Text = "1500.0";
                        Txt6.Text = (q_current_4 - 1500).ToString();
                    }
                    else
                    {
                        Txt5.Text = q_current_4.ToString();
                        Txt6.Text = "0.0";
                    }

                    // 【关节5动画】
                    double q_current_5 = (num_fresh - num_current) * move_step[4];
                    Txt7.Text = q_current_5.ToString();


                    // 【关节6动画】
                    double q_current_6 = (num_fresh - num_current) * move_step[5];
                    Txt8.Text = q_current_6.ToString();

                    // 【关节7动画】
                    double q_current_7 = (num_fresh - num_current) * move_step[6];
                    Txt9.Text = q_current_7.ToString();
                    break;
            }
            num_current += 1;
        }

        /// <summary>
        /// 【生成测试轨迹样本】
        /// </summary>
        // 定义存储结果的列表
        List<double[]> q_current_list = new List<double[]>();
        List<double[]> Td_list = new List<double[]>();
        double[] endTd;
        private void timer4_Tick(object sender, EventArgs e)
        {
            // 定义一个数组来存储当前循环的 q_current_1 到 q_current_7
            double[] q_current_array = new double[7];

            if (num_current > num_fresh-1)
            {
                //num_current = 0;
                timer4.Enabled = false;
                
            }

            // 【关节1动画】
            q_current_array[0] = num_current * move_step[0]; // q_current_1
            if (q_current_array[0] > 5600)
            {
                Txt1.Text = "5600.0";
                Txt2.Text = (q_current_array[0] - 5600).ToString();
            }
            else
            {
                Txt1.Text = q_current_array[0].ToString();
                Txt2.Text = "0.0";
            }

            // 【关节2动画】
            q_current_array[1] = num_current * move_step[1]; // q_current_2
            Txt3.Text = q_current_array[1].ToString();

            // 【关节3动画】
            q_current_array[2] = num_current * move_step[2]; // q_current_3
            Txt4.Text = q_current_array[2].ToString();

            // 【关节4动画】
            q_current_array[3] = num_current * move_step[3]; // q_current_4
            if (q_current_array[3] > 1500)
            {
                Txt5.Text = "1500.0";
                Txt6.Text = (q_current_array[3] - 1500).ToString();
            }
            else
            {
                Txt5.Text = q_current_array[3].ToString();
                Txt6.Text = "0.0";
            }

            // 【关节5动画】
            q_current_array[4] = num_current * move_step[4]; // q_current_5
            Txt7.Text = q_current_array[4].ToString();

            // 【关节6动画】
            q_current_array[5] = num_current * move_step[5]; // q_current_6
            Txt8.Text = q_current_array[5].ToString();

            // 【关节7动画】
            q_current_array[6] = num_current * move_step[6]; // q_current_7
            Txt9.Text = q_current_array[6].ToString();

            // 将当前数组添加到列表中-【关节变化数据】
            q_current_list.Add(q_current_array);

            // 【期望齐次矩阵变化数据】
            endTd = showEndPosition();
            Td_list.Add(endObject_xyz);


            // 打印齐次矩阵信息
            //Console.WriteLine($"Iteration {num_current + 1}: {string.Join(", ", endTd)}");
            // 打印关节信息
            Console.WriteLine($"Iteration {num_current + 1}: {string.Join(", ", q_current_array)}");
            num_current += 1;
        }


        private void chk_CB01_CheckedChanged(object sender, EventArgs e)
        {
            // removeChild 只能删除直系的对象
            if (!chk_CB01.Checked) findJoint(robotModel, "坐标7_腕部滚摆").removeChild(CB01);
            else findJoint(robotModel, "坐标7_腕部滚摆").addChild(CB01);
        }

        private void chk_EE_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_EE.Checked) findJoint(robotModel, "坐标7_腕部滚摆").removeChild(eeAxis);
            else findJoint(robotModel, "坐标7_腕部滚摆").addChild(eeAxis);
        }
        private void chk_manipulator_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_manipulator.Checked) global.scene.removeChild(robotModel);
            else global.scene.addChild(robotModel);
        }
        /// <summary>
        /// 相机【俯视图】
        /// </summary>
        private void btn_FS_Click(object sender, EventArgs e)
        {
            camera._position= new Vector3(0.0f, 15.0f, 10.0f); //【摄像机位置】
            camera._right= new Vector3(0.0f, 0.0f, -1.0f);  //【摄像机右侧】
            camera._up = new Vector3(-1.0f, 0.0f, 0.0f);  //【摄像机顶部】

            chk_MJ01.Checked = false;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = true;
            chk_MJ04.Checked = false;
        }


        /// <summary>
        /// 相机【侧视图】
        /// </summary>
        private void btn_CS_Click(object sender, EventArgs e)
        {
            camera._position = new Vector3(12.0f, 1.0f, 12.0f); ; //【摄像机位置】
            camera._right = new Vector3(0.0f, 0.0f, -1.0f);  //【摄像机右侧】
            camera._up = Vector3.UnitY;  //【摄像机顶部】

            chk_MJ01.Checked = true;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = false;
            chk_MJ04.Checked = false;
        }
        /// <summary>
        /// 相机【正视图】
        /// </summary>
        private void btn_ZS_Click(object sender, EventArgs e)
        {
            camera._position = new Vector3(0.0f, 1.0f, 18.0f); ; //【摄像机位置】
            camera._right = new Vector3(1.0f, 0.0f, 0.0f);  //【摄像机右侧】
            camera._up = Vector3.UnitY;  //【摄像机顶部】

            chk_MJ01.Checked = true;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = false;
            chk_MJ04.Checked = false;
        }
        /// <summary>
        /// 重置构型
        /// </summary>
        private void btn_home_Click(object sender, EventArgs e)
        {
            Txt1.Text = "0.0";
            Txt2.Text = "0.0";
            Txt3.Text = "0.0";
            Txt4.Text = "0.0";
            Txt5.Text = "0.0";
            Txt6.Text = "0.0";
            Txt7.Text = "0.0";
            Txt8.Text = "0.0";
            Txt9.Text = "0.0";
        }
        /// <summary>
        /// 随机构型
        /// </summary>
        private void btn_random_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            // 关节1
            double randJoint1 = 11500.0 * rand.NextDouble();
            Txt1.Text = (randJoint1/2.0).ToString("F1");
            Txt2.Text = (randJoint1 / 2.0).ToString("F1");

            Txt3.Text = (-180.0+360.0 * rand.NextDouble()).ToString("F1"); // 关节2
            Txt4.Text = (-35.0+75.0 * rand.NextDouble()).ToString("F1"); // 关节3
            // 关节4
            double randJoint4 = 3000.0 * rand.NextDouble();
            Txt5.Text = (randJoint4 / 2.0).ToString("F1");
            Txt6.Text = (randJoint4 / 2.0).ToString("F1");

            Txt7.Text = (-75.0 + 150.0 * rand.NextDouble()).ToString("F1"); // 关节5
            Txt8.Text = (-105.0 + 135.0 * rand.NextDouble()).ToString("F1"); // 关节5
            Txt9.Text = (-180.0 + 360.0 * rand.NextDouble()).ToString("F1"); // 关节5
        }


        private void btnColor_Yellow_Click(object sender, EventArgs e)
        {
            GL.ClearColor(
            btnColor_Yellow.BackColor.R / 255f,
            btnColor_Yellow.BackColor.G / 255f,
            btnColor_Yellow.BackColor.B / 255f,
            btnColor_Yellow.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        private void btnColor_Red_Click(object sender, EventArgs e)
        {
            GL.ClearColor(
            btnColor_Red.BackColor.R / 255f,
            btnColor_Red.BackColor.G / 255f,
            btnColor_Red.BackColor.B / 255f,
            btnColor_Red.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        private void btnColor_Blue_Click(object sender, EventArgs e)
        {
            GL.ClearColor(
                btnColor_Blue.BackColor.R / 255f,
                btnColor_Blue.BackColor.G / 255f,
                btnColor_Blue.BackColor.B / 255f,
                btnColor_Blue.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        /// <summary>
        /// 路径规划关节变化演示
        /// </summary>
        public IXLWorksheet worksheet_PP;
        private bool isPaused;      // 控制暂停/继续
        private void btn_pp_show_Click(object sender, EventArgs e)
        {
            string defaultPath;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"J:\同步空间\BaiduSyncdisk\Matlab\路径规划\Data";  //默认打开目录
            openFileDialog.Filter = "文本文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                defaultPath = openFileDialog.FileName; //记录选中的目录  
                XLWorkbook inventory_rb = new XLWorkbook(defaultPath);
                worksheet_PP = inventory_rb.Worksheet(1);  // 获取第一个工作表

                int rowCount = worksheet_PP.RowsUsed().Count();  // 获取工作表的总行数

                //drawLine.ClearPoints(); //运动曲线绘制数组
                PP_Stop = 0;            // 路径规划运动结束  1-结束

                Task.Run(async () =>
                {
                    for (int i = 1; i < rowCount + 1; i++)
                    {
                        while (isPaused) await Task.Delay(100); // 等待恢复

                        // 读取单元格内容
                        float q_current_1 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 1).Value.ToString());
                        float q_current_2 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 2).Value.ToString());
                        float q_current_3 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 3).Value.ToString());
                        float q_current_4 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 4).Value.ToString());
                        float q_current_5 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 5).Value.ToString());
                        float q_current_6 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 6).Value.ToString());
                        float q_current_7 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 7).Value.ToString());


                        // 确保 UI 更新在主线程进行
                        Txt1.Invoke(new Action(() =>
                        {
                            Txt1.Text = (q_current_1 / 2.0).ToString();
                            Txt2.Text = (q_current_1 / 2.0).ToString();
                            Txt3.Text = q_current_2.ToString();
                            Txt4.Text = q_current_3.ToString();
                            Txt5.Text = (q_current_4 / 2.0).ToString();
                            Txt6.Text = (q_current_4 / 2.0).ToString();
                            Txt7.Text = q_current_5.ToString();
                            Txt8.Text = q_current_6.ToString();
                            Txt9.Text = q_current_7.ToString();

                            int index = this.dataGridView1.Rows.Add();
                            // 绘制关节变化曲线
                            for (int j = 0; j < chart1.Series.Count; j++)
                            {
                                //AddPointToSeries(j, i, Convert.ToDouble(worksheet_PP.Cell(i, j + 1).Value.ToString()));

                                double value = Convert.ToDouble(worksheet_PP.Cell(i, j + 1).Value.ToString());

                                // 1️⃣ 绘制曲线
                                AddPointToSeries(j, i, value);

                                // 2️⃣ 写入 DataGridView（第 j 列对应 q(j+1)）
                                
                                dataGridView1.Rows[index].Cells[0].Value = i;
                                dataGridView1.Rows[index].Cells[j+1].Value = value.ToString("F1");

                            }

                        }));

                        // 确保 OpenGL 绘制在主线程进行
                        glControl1.Invoke(new Action(() => glControl1_Paint(null, null)));

                        await Task.Delay(2);


                        // 第一帧暂停
                        //if (i == 1) isPaused = true; btn_pause.Text = "Continue sim";
                        //// 第30帧暂停
                        //if (i == 35) isPaused = true; btn_pause.Text = "继续演示";
                        //if (i == 50) isPaused = true; btn_pause.Text = "继续演示";
                        //if (i == 75) isPaused = true; btn_pause.Text = "继续演示";
                    }
                    PP_Stop = 1;           // 路径规划运动结束
                });
            }
        }

    }
}
