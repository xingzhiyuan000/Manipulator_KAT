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

        Matrix4 viewMatrix = Matrix4.Identity; 
        Matrix4 transform = Matrix4.Identity;
        Matrix4 orthoMatrix = Matrix4.Identity; 
        Matrix4 perspectiveMatrix = Matrix4.Identity; 
        Matrix4 projectionMatrix = Matrix4.Identity; 


        Point glMousePt;
        bool mLeftMouseDown = false;
        bool mRightMouseDown = false;
        bool mMiddleMouseDown = false;

        float mCurrentX = 0.0f;
        float mCurrentY = 0.0f;
        private float _mouseSensitivity = 0.2f; 
        private float _moveSensitivity = 0.005f; 
        private float _scaleSensitivity;
        private float _gameSensitivity = 0.1f; 
        private int projectionIndex = 1;  
        private int cameraIndex = 1;  
        private float deltaScale = 1;
        Camera camera = new Camera();
        Geometry geometry = new Geometry();

        float specularIntensity;
        float shiness;
        Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.2f);  

        Renderer renderer = null;
        Light light = null;

        Object zouTai; 

        Object robotModel; 
        Object mojiModel; 
        Object MJ01; 
        Object MJ02; 
        Object MJ03;
        Object MJ04; 
        Object CB01;
        Object Car01; 
        Object Car02;
        Object eeAxis; 

        List<Joint> listJoints; 
        Object joint = new Object(); 
        Vector4 endEffectorPos;
        Vector4 endEffectorPos10; 

        Object find_joint1 = new Object(); 
        Object find_joint2 = new Object(); 
        Object find_joint3 = new Object(); 
        Object find_joint4 = new Object(); 
        Object find_joint5 = new Object(); 
        Object find_joint6 = new Object(); 
        Object find_joint7 = new Object(); 
        Object find_joint8 = new Object(); 
        Object find_joint9 = new Object(); 

        Object find_EE = new Object(); 

        private Drawline[] drawLine = new Drawline[2]; 
        Vector3 point_PP;
        public int indexLine;
                                       
        private void Form1_Load(object sender, EventArgs e)
        {
            drawLine[0] = new Drawline(); 
            drawLine[1] = new Drawline();
            drawLine[0].lineColor = new Vector3(1.0f, 0.0f, 0.0f);
            drawLine[1].lineColor = new Vector3(0.0f, 0.0f, 1.0f);

            this.StartPosition = FormStartPosition.CenterScreen;
            CheckForIllegalCrossThreadCalls = false; 

            specularIntensity = trackBar1.Value / 10.0f; ;   
            shiness = 32.0f;
            _scaleSensitivity = trackBar4.Value / 10.0f;
            _moveSensitivity = trackBar5.Value / 1000.0f;

            init_Pos();     
            init_MDH();     
            init_Offset(); 
            InitChart();    
        }

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
            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY2.MajorGrid.Enabled = false;

            area.AxisX.Interval = 5;

            area.AxisY2.Enabled = AxisEnabled.True;
            area.AxisY2.Title = "Prismatic (mm)";
            area.AxisY.Title = "Rotation (°)";

            area.AxisY2.TitleFont = new Font("Times New Roman", 14F);
            area.AxisY.TitleFont = new Font("Times New Roman", 14F);


            for (int i = 0; i < 7; i++)
            {
                Series series = new Series("Joint q" + (i + 1));
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 2;

                if (i + 1 == 1 || i + 1 == 4)
                    series.YAxisType = AxisType.Secondary;  
                else
                    series.YAxisType = AxisType.Primary;    
   

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

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100;

            chart1.ChartAreas[0].AxisY.Minimum = -180;
            chart1.ChartAreas[0].AxisY.Maximum = 40;

            chart1.ChartAreas[0].AxisY2.Minimum = 0;
            chart1.ChartAreas[0].AxisY2.Maximum = 10000;
        }

        private void AddPointToSeries(int seriesIndex, double xValue, double yValue)
        {
            if (seriesIndex < 0 || seriesIndex >= chart1.Series.Count)
                return;

            Series series = chart1.Series[seriesIndex];

            series.Points.AddXY(xValue, yValue);

            if (series.Points.Count > 100)
            {
                series.Points.RemoveAt(0);
            }

            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            deltaScale = e.Delta / Math.Abs(e.Delta);
            if (projectionIndex == 1) 
            {
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
            glControl1_Paint(null, null); 
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

            this.glControl1.MouseWheel += new MouseEventHandler(glControl1_MouseWheel); 
            
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.Multisample);


            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); 

            timer1.Interval = 1000 / 60;  
            timer1.Enabled = true;

           
            prepareCamera(); 

            if (projectionIndex == 1) preparePerspective(); 
            else prepareOrtho(); 

            prepare();  

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

            
            CB01 = findJoint(robotModel, "衬板");
            Car01=findJoint(robotModel, "运输小车");
            Car02= findJoint(robotModel, "回转台");
            eeAxis= findJoint(robotModel, "末端执行器坐标系");

        }

        private void prepareCamera()
        {
            viewMatrix = camera.GetViewMatrix();
        }

        private void prepareOrtho()
        {
            projectionMatrix = camera.GetOrthoMatrix();

        }


        private void preparePerspective()
        {
            float fov = MathHelper.DegreesToRadians(60.0f); 
            float aspectRatio = (float)glControl1.Width / (float)glControl1.Height; 
            projectionMatrix = camera.GetPerspectiveMatrix(fov, aspectRatio, camera.pNear, camera.pFar);
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
            Point pt = MousePosition; 
            glMousePt = this.glControl1.PointToClient(pt);
            mCurrentX = glMousePt.X;
            mCurrentY = glMousePt.Y;
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
            Point pt = MousePosition; 
            glMousePt = this.glControl1.PointToClient(pt);

            if (mLeftMouseDown)
            {
                float deltaX = (glMousePt.X - mCurrentX) * _mouseSensitivity;
                float deltaY = (glMousePt.Y - mCurrentY) * _mouseSensitivity;
                updatePitch(deltaY);
                updateYaw(deltaX);


            }
            else if (mMiddleMouseDown) 
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
        private float mPitch = 0.0f; 
        private void updatePitch(float angle)
        {
            Matrix4 mat = Matrix4.Identity;
            if (cameraIndex == 1) 
            {
                mat = mat * Matrix4.CreateFromAxisAngle(camera._right, MathHelper.DegreesToRadians(angle));  
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                Vector4 transformedPos = mat * (new Vector4(camera._position, 1.0f));
                camera._position = new Vector3(transformedPos.X, transformedPos.Y, transformedPos.Z);
            }
            else 
            {
                mPitch += angle;
                if (mPitch > 89.0f || mPitch < -89.0f)
                {
                    mPitch -= angle;
                    return;
                }
                mat = mat * Matrix4.CreateFromAxisAngle(camera._right, MathHelper.DegreesToRadians(angle));  
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
            }


        }

        private void updateYaw(float angle)
        {
            Matrix4 mat = Matrix4.Identity;
            if (cameraIndex == 1) 
            {
                mat = mat * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angle));  
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                Vector4 transformedPos = mat * (new Vector4(camera._position, 1.0f));
                camera._position = new Vector3(transformedPos.X, transformedPos.Y, transformedPos.Z);
                Vector4 transformedRight = mat * (new Vector4(camera._right, 1.0f));
                camera._right = new Vector3(transformedRight.X, transformedRight.Y, transformedRight.Z);
            }
            else
            {
                mat = mat * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angle));  
                Vector4 transformedUp = mat * (new Vector4(camera._up, 0.0f));
                camera._up = new Vector3(transformedUp.X, transformedUp.Y, transformedUp.Z);
                Vector4 transformedRight = mat * (new Vector4(camera._right, 1.0f));
                camera._right = new Vector3(transformedRight.X, transformedRight.Y, transformedRight.Z);
            }

        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {

            if (cameraIndex != 2) return;
            Vector3 direction = Vector3.Zero; 
            Vector3 front = Vector3.Cross(camera._up, camera._right); 
            Vector3 right = camera._right;
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
            camera._position = new Vector3(0.0f, 0.0f, 2.0f); 
            camera._up = Vector3.UnitY;  
            camera._right = Vector3.UnitX; 

            orthoMatrix = Matrix4.CreateOrthographicOffCenter(-2.0f, 2.0f, -2.0f, 2.0f, 2.0f, -2.0f); 

            float fov = MathHelper.DegreesToRadians(60.0f); 
            float aspectRatio = (float)glControl1.Width / (float)glControl1.Height; 
            float near = 0.01f; 
            float far = 100.0f; 

            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);  

            if (projectionIndex == 1) projectionMatrix = perspectiveMatrix;
            else projectionMatrix = orthoMatrix;
            glControl1_Paint(null, null);

        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            GL.ClearColor(
                btnColor_White.BackColor.R / 255f,
                btnColor_White.BackColor.G / 255f,
                btnColor_White.BackColor.B / 255f,
                btnColor_White.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        private void btnColor_Green_Click(object sender, EventArgs e)
        {
            GL.ClearColor(
                btnColor_Green.BackColor.R / 255f,
                btnColor_Green.BackColor.G / 255f,
                btnColor_Green.BackColor.B / 255f,
                btnColor_Green.BackColor.A / 255f);
            glControl1_Paint(null, null);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            specularIntensity = trackBar1.Value / 10.0f;   
            glControl1_Paint(null, null);
        }

        void getTree(Object obj, TreeNode tnParent)
        {
            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].GetName() != null)
                {
                    TreeNode tn = new TreeNode(children[i].GetName());
                    tnParent.Nodes.Add(tn);     
                    getTree(obj.mChildren[i], tn);
                }

            }
        }

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
            shiness = trackBar2.Value; ;   
            glControl1_Paint(null, null);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            glControl1_Paint(null, null);
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _mouseSensitivity = trackBar3.Value / 10.0f;   
            glControl1_Paint(null, null);
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            _scaleSensitivity = trackBar4.Value / 10.0f;   
            glControl1_Paint(null, null);
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            _moveSensitivity = trackBar5.Value / 1000.0f;   
            glControl1_Paint(null, null);
        }
        int PP_Stop=1;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            renderer.Render(global.scene, camera, light, projectionMatrix, specularIntensity, shiness);

            if (PP_Stop==0)
            {
                endObject_xyz = showEndPosition();  
                point_PP = new Vector3(
                    (float)Math.Round((float)endObject_xyz[9], 4),
                    -(float)Math.Round((float)endObject_xyz[11], 4),
                    (float)Math.Round((float)endObject_xyz[10], 4)
                    );
                Debug.WriteLine(Math.Round((float)endObject_xyz[9], 4).ToString());
                drawLine[indexLine].AddPoint(new Vector3((float)endObject_xyz[9], -(float)endObject_xyz[11], (float)endObject_xyz[10]));
            }

            if (chk_traj.Checked) drawLine[0].Draw();   
            if (chk_traj.Checked) drawLine[1].Draw();   

            glControl1.SwapBuffers(); 

        }
        string exeDir;
        string projectRoot;
        void prepare()
        {
             exeDir = Application.StartupPath;
             projectRoot = Directory.GetParent(exeDir).Parent.FullName;

            renderer = new Renderer();  
            global.scene = new Scene();        

            robotModel = AssimpLoader.loadModel(projectRoot + "/Resources/FBX/Manipulator_20250326.fbx");  
           

            robotModel.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            light = new Light();
            light.setDirectionalLight(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f));


            global.scene.addChild(robotModel);



            Mesh bones = prepareBones();
            global.scene.addChild(bones);

            projectBones();


            MJ01 = AssimpLoader.loadModel(projectRoot + "/Resources/FBX/MJ01.fbx");
            MJ01.SetName("磨机_01");
            MJ01.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ01);

            MJ02 = AssimpLoader.loadModel(projectRoot + "/Resources/FBX/MJ02.fbx");
            MJ02.SetName("磨机_02");
            MJ02.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ02);
            

            MJ03 = AssimpLoader.loadModel(projectRoot + "/Resources/FBX/MJ03.fbx");
            MJ03.SetName("磨机_03");
            MJ03.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ03);
            

            MJ04 = AssimpLoader.loadModel(projectRoot + "/Resources/FBX/MJ04.fbx");
            MJ04.SetName("磨机_04");
            MJ04.SetScale(new Vector3(0.01f, 0.01f, 0.01f));
            global.scene.addChild(MJ04);




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

            joint0.constraints(0, 0, 0, 0, 0, 0, 0, 5.6f);  
            joint1.constraints(0, 0, 0, 0, 0, 0, 0, 5.9f);  
            joint2.constraints(0, 0, -180.0f, 180.0f, 0, 0, 0, 0.0f);  
            joint4.constraints(-40.0f, 35.0f, 0, 0, 0, 0, 0, 0);  
            joint6.constraints(0, 0, 0, 0, 0, 0, 0, 1.5f);  
            joint7.constraints(0, 0, 0, 0, 0, 0, 0, 1.5f);  
            joint8.constraints(0, 0, -75.0f, 75.0f, 0, 0, 0, 0);  
            joint9.constraints(-105.0f, 30.0f, 0, 0, 0, 0, 0, 0);  
            joint10.constraints(0, 0, 0, 0, -180.0f, 180.0f, 0, 0);  


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
        private Mesh prepareInstance()
        {
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            Material material = new Material();

            material.mDiffuse = new Texture(projectRoot + "/Resources/Textures/earth.png", 0);
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
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            Material material = new Material();
            material.mType = MaterialType.WhiteMaterial;

            Mesh mesh = new Mesh(geometry, material);
            mesh.SetName("目标点");
            mesh.SetPosition(global.t_position);

            return mesh;
        }

        private Mesh prepareTarget()
        {
            Geometry geometry = new Geometry();
            geometry = geometry.createSphere(0.1f);

            Material material = new Material();

            material.mDiffuse = new Texture(projectRoot + "/Resources/Textures/target.jpg", 0);
            material.mType = MaterialType.PhongMaterial;

            Mesh mesh = new Mesh(geometry, material);


            mesh.SetName("目标");

            mesh.SetPosition(global.t_position);

            return mesh;
        }

        private Mesh prepareBones()
        {
            Geometry geometry = new Geometry();
            geometry = geometry.createBox(0.001f);  

            Material material = new Material();

            material.mDiffuse = new Texture(projectRoot + "/Resources/Textures/bone.png", 0);
            material.mType = MaterialType.PhongMaterial;
            
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

            return mesh_0;
        }
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

        private Mesh prepareSkyBox()
        {
            Texture envTex = new Texture(projectRoot + "/Resources/Textures/skybox/sphericalMap.png", 0);
            Geometry cubeGeometry = new Geometry();
            cubeGeometry = cubeGeometry.createBox(1.0f);
            Material sphereMaterial = new Material();
            sphereMaterial.mType = MaterialType.SphereMaterial;
            sphereMaterial.mDiffuse = envTex;
            sphereMaterial.mDepthWrite = false;  
            sphereMaterial.mCullFace = CullFaceMode.Front;
            Mesh cubeMesh = new Mesh(cubeGeometry, sphereMaterial);
            return cubeMesh;
        }

        private double[] showEndPosition()
        {
            Matrix4 modelTransform = listJoints[11].getMesh().GetModelMatrix();
            modelTransform = Matrix4.Transpose(modelTransform);
            endEffectorPos = modelTransform * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

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
            double[] pos_xyz = new double[12];

            pos_xyz[0] = Convert.ToDouble(modelTransform.M11);  
            pos_xyz[1] = Convert.ToDouble(modelTransform.M21);  
            pos_xyz[2] = Convert.ToDouble(modelTransform.M31);  

            pos_xyz[3] = Convert.ToDouble(modelTransform.M12);  
            pos_xyz[4] = Convert.ToDouble(modelTransform.M22);  
            pos_xyz[5] = Convert.ToDouble(modelTransform.M32);  

            pos_xyz[6] = Convert.ToDouble(modelTransform.M13);  
            pos_xyz[7] = Convert.ToDouble(modelTransform.M23);  
            pos_xyz[8] = Convert.ToDouble(modelTransform.M33);  

            pos_xyz[9] = Convert.ToDouble(modelTransform.M14);   
            pos_xyz[10] = Convert.ToDouble(modelTransform.M24);  
            pos_xyz[11] = Convert.ToDouble(modelTransform.M34);  
            return pos_xyz;
        }

        private void trackBar_1_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_1.Value / 10.0);
            Txt1.Text = ((float)trackBar_1.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint1(change_val);

        }

        private void moveJoint1(float val)
        {
            Vector3 changedValue = new Vector3(0.0f, 0.0f, -val / 1000.0f);
            find_joint1.SetPosition(changedValue);
            listJoints[0].getMesh().SetPosition(changedValue);
            showEndPosition();
        }
        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar9.Value / 10.0);
            Txt2.Text = ((float)trackBar9.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint2(change_val);
        }

        private void moveJoint2(float val)
        {
            Vector3 changedValue = new Vector3(0.0f, val / 1000.0f, 0.0f);

            find_joint2.SetPosition(changedValue);

            changedValue = new Vector3(0.0f, 0.0f, -val / 1000.0f);
            Vector3 offset = global.w_position_1 - global.w_position_0;
            listJoints[1].getMesh().SetPosition(offset + changedValue);

            showEndPosition();
        }
        private void trackBar_2_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_2.Value / 10.0);
            Txt3.Text = change_val.ToString();
            moveJoint3(change_val);
        }

        private void moveJoint3(float val)
        {
            find_joint3.SetAngle2((float)val);
            listJoints[2].getMesh().setAngleY((float)val);

            showEndPosition();
        }

        private void trackBar_3_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_3.Value / 10.0);
            Txt4.Text = change_val.ToString();
            moveJoint4(-change_val);
        }
        private void moveJoint4(float val)
        {
            find_joint4.SetAngle3((float)val);
            listJoints[4].getMesh().setAngleX((float)val);

            showEndPosition();
        }
        private void trackBar_4_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar_4.Value / 10.0);
            Txt5.Text = ((float)trackBar_4.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint5(change_val);
        }
        private void moveJoint5(float val)
        {
            find_joint5.SetPosition(new Vector3(0.0f, val / 1000.0f, 0.0f));
            Vector3 changedValue = new Vector3(0.0f, 0.0f, val / 1000.0f);
            Vector3 offset = global.w_position_6 - global.w_position_5;
            listJoints[6].getMesh().SetPosition(offset - changedValue);

            showEndPosition();
        }
        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            float change_val = (float)(trackBar10.Value / 10.0);
            Txt6.Text = ((float)trackBar10.Maximum / 10.0 - change_val).ToString("F1");
            moveJoint6(change_val);
        }
        private void moveJoint6(float val)
        {
            find_joint6.SetPosition(new Vector3(0.0f, val / 1000.0f, 0.0f));
            Vector3 changedValue = new Vector3(0.0f, 0.0f, val / 1000.0f);
            Vector3 offset = global.w_position_7 - global.w_position_6;
            listJoints[7].getMesh().SetPosition(offset - changedValue);

            showEndPosition();
        }
        private void init_Pos()
        {
            trackBar10.Value = trackBar10.Maximum;
            trackBar10_Scroll(null, null);

            trackBar_4.Value = trackBar_4.Maximum;
            trackBar_4_Scroll(null, null);

            trackBar9.Value = trackBar9.Maximum;
            trackBar9_Scroll(null, null);

            trackBar_1.Value = trackBar_1.Maximum;
            trackBar_1_Scroll(null, null);

        }


        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            float opacity = trackBar8.Value / 10.0f;   

            setModelBlend(robotModel, true, opacity);
            glControl1_Paint(null, null);
        }
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
            if (!Common.check_Tex(Txt1.Text)) return;   

            if (Convert.ToDouble(Txt1.Text) > 5600) Txt1.Text = "5600.0";
            if (Convert.ToDouble(Txt1.Text) < 0) Txt1.Text = "0.0";

            trackBar_1.Value = Convert.ToInt32(trackBar_1.Maximum - Convert.ToDouble(Txt1.Text) * 10);
            moveJoint1((float)Convert.ToDouble(trackBar_1.Value / 10.0f));
        }

        private void Txt2_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt2.Text)) return;
            if (Convert.ToDouble(Txt2.Text) > 5900) Txt2.Text = "5900.0";
            if (Convert.ToDouble(Txt2.Text) < 0) Txt2.Text = "0.0";

            trackBar9.Value = Convert.ToInt32(trackBar9.Maximum - Convert.ToDouble(Txt2.Text) * 10);
            moveJoint2((float)Convert.ToDouble(trackBar9.Value / 10.0f));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt3.Text)) return;
            if (Convert.ToDouble(Txt3.Text) > 180) Txt3.Text = "180.0";
            if (Convert.ToDouble(Txt3.Text) < -180) Txt3.Text = "-180.0";

            moveJoint3((float)Convert.ToDouble(Txt3.Text));
            trackBar_2.Value = Convert.ToInt32(Convert.ToDouble(Txt3.Text) * 10);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt4.Text)) return;
            if (Convert.ToDouble(Txt4.Text) > 40) Txt4.Text = "40.0";
            if (Convert.ToDouble(Txt4.Text) < -35) Txt4.Text = "-35.0";

            moveJoint4(-(float)Convert.ToDouble(Txt4.Text));
            trackBar_3.Value = Convert.ToInt32(Convert.ToDouble(Txt4.Text) * 10);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt5.Text)) return;
            if (Convert.ToDouble(Txt5.Text) > 1500) Txt5.Text = "1500.0";
            if (Convert.ToDouble(Txt5.Text) < 0) Txt5.Text = "0.0";

            trackBar_4.Value = Convert.ToInt32(trackBar_4.Maximum - Convert.ToDouble(Txt5.Text) * 10);
            moveJoint5((float)Convert.ToDouble(trackBar_4.Value / 10.0f));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt6.Text)) return;
            if (Convert.ToDouble(Txt6.Text) > 1500) Txt6.Text = "1500.0";
            if (Convert.ToDouble(Txt6.Text) < 0) Txt6.Text = "0.0";

            trackBar10.Value = Convert.ToInt32(trackBar10.Maximum - Convert.ToDouble(Txt6.Text) * 10);
            moveJoint6((float)Convert.ToDouble(trackBar10.Value / 10.0f));

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt7.Text)) return;
            if (Convert.ToDouble(Txt7.Text) > 75) Txt7.Text = "75.0";
            if (Convert.ToDouble(Txt7.Text) < -75) Txt7.Text = "-75.0";

            moveJoint7((float)Convert.ToDouble(Txt7.Text));
            trackBar_5.Value = Convert.ToInt32(Convert.ToDouble(Txt7.Text) * 10);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt8.Text)) return;
            if (Convert.ToDouble(Txt8.Text) > 30) Txt8.Text = "30.0";
            if (Convert.ToDouble(Txt8.Text) < -105) Txt8.Text = "-105.0";

            moveJoint8(-(float)Convert.ToDouble(Txt8.Text));
            trackBar6.Value = Convert.ToInt32(Convert.ToDouble(Txt8.Text) * 10);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!Common.check_Tex(Txt9.Text)) return;
            if (Convert.ToDouble(Txt9.Text) > 180) Txt9.Text = "180.0";
            if (Convert.ToDouble(Txt9.Text) < -180) Txt9.Text = "-180.0";

            moveJoint9((float)Convert.ToDouble(Txt9.Text));
            trackBar7.Value = Convert.ToInt32(Convert.ToDouble(Txt9.Text) * 10);
        }

       
        private Dictionary<int, InstancedMesh> InstancedMeshDict = new Dictionary<int, InstancedMesh>();
        private int rowCount;   
        string showPath=@"J:\同步空间\BaiduSyncdisk\Matlab\路径规划\Data";    
        private void btn_show_workspace_Click(object sender, EventArgs e)
        {
            int instance_Lim = 100; 
            richTextBox2.Clear();

            if (btn_show_workspace.Text == "Show")
            {

                Geometry geometry = new Geometry();         
                geometry = geometry.createSphere(0.02f);       
                

                Material material = new Material();
                material.mDiffuse = new Texture(projectRoot + "/Resources/Textures/green.png", 0);
                material.mType = MaterialType.PhongInstanceMaterial;
                material.mShiness = 64.0f; 

                
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = showPath;  
                openFileDialog.Filter = "文本文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    btn_show_workspace.Text = "Clear";

                    showPath = openFileDialog.FileName;   
                    XLWorkbook inventory_rb = new XLWorkbook(showPath);
                    IXLWorksheet worksheet = inventory_rb.Worksheet(1);   

                    rowCount = worksheet.RowsUsed().Count();   

                    for (int i = 1; i < rowCount + 1; i++)
                    {

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

        double[] q_con = new double[7]; 
        double[] q = new double[7];     
        Vector<double> target_pos;      
        double w_k = 1e-6;
        double w_1 = 1;
        double w_2 = 1;

        private void btn_updateJoint_Click(object sender, EventArgs e)
        {

            Stopwatch timeWatch = new Stopwatch();
            timeWatch.Start();  

            updateJoint();

            timeWatch.Stop();  
            groupBox8.Text = "逆向运动学(" + timeWatch.ElapsedMilliseconds + "ms)";
        }
        int iter_num = 1;    
        private (double, double, double[]) updateJoint(bool showAnimation = false)
        {
            double change_val = 0; 
            Matrix<double> d_w = Matrix<double>.Build.DenseOfArray(new double[,] {
                        {Convert.ToDouble(Txt_n_x.Text), Convert.ToDouble(Txt_o_x.Text), Convert.ToDouble(Txt_a_x.Text), Convert.ToDouble(Txt_p_x.Text)},
                        {Convert.ToDouble(Txt_n_y.Text), Convert.ToDouble(Txt_o_y.Text), Convert.ToDouble(Txt_a_y.Text), Convert.ToDouble(Txt_p_y.Text)},
                        {Convert.ToDouble(Txt_n_z.Text), Convert.ToDouble(Txt_o_z.Text), Convert.ToDouble(Txt_a_z.Text), Convert.ToDouble(Txt_p_z.Text)}
                    });
            Matrix<double> T08 = null;
            for (int i = 0; i < 7; i++)
            {
                string CMD_W_J = "0" + (i + 1).ToString();  
                string CMD_J_E = (i + 1).ToString() + "8";  
                q_con = Common.convertJoint(q[0], q[1], q[2], q[3], q[4], q[5], q[6]); 
                T08 = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], "08");
                Matrix<double> T0j = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], CMD_W_J);
                Matrix<double> Tj8 = Common.Fkine_LH4500(q_con[0], q_con[1], q_con[2], q_con[3], q_con[4], q_con[5], q_con[6], CMD_J_E);
                Vector<double> pc_j = Vector<double>.Build.DenseOfArray(new double[] { Tj8[0, 3], Tj8[1, 3], Tj8[2, 3] });

                if (i == 0 || i == 3)
                {
                    Vector<double> Z_P = T0j.SubMatrix(0, 3, 0, 3).Inverse() * T0j.Column(2).SubVector(0, 3);
                    double tc = pc_j.DotProduct(Z_P) / Z_P.DotProduct(Z_P);    
                    Vector<double> pt_j = tc * Z_P;                        

                    Vector<double> pd_w_P = Vector<double>.Build.DenseOfArray(new double[] { target_pos[0], target_pos[1], target_pos[2], 1 });
                    Vector<double> pd_j_P = T0j.Inverse() * pd_w_P;
                    double td = pd_j_P.SubVector(0, 3).DotProduct(Z_P) / Z_P.DotProduct(Z_P);    
                    Vector<double> pt_j_ = td * Z_P;

                    Vector<double> deta_d = pt_j_ - pt_j;  
                    change_val = deta_d.L2Norm();   

                    if (deta_d.DotProduct(Z_P) < 0) change_val = -change_val;  
                }

                if (i == 1 || i == 2 || i == 4 || i == 5 || i == 6)
                {
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

                    double beta = Math.Atan2(B, A[0, 0]);
                    change_val = Math.PI / 2 - beta;


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
                if (q[i] + change_val < global.q_lim[i, 0]) change_val = global.q_lim[i, 0] - q[i];
                if (q[i] + change_val > global.q_lim[i, 1]) change_val = global.q_lim[i, 1] - q[i];
                q[i] = q[i] + change_val;  

                if (showAnimation)
                {
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

            double e1 = (target_pos - T08.Column(3).SubVector(0, 3)).L2Norm();
            Matrix<double> d = d_w;
            Matrix<double> c = T08.SubMatrix(0, 3, 0, 3);
            double e2 = 0.0;
            for (int u = 0; u < 3; u++)
            {
                Vector<double> d_v = d.Column(u);
                Vector<double> c_v = c.Column(u);
                e2 = e2 + Math.Pow(d_v.DotProduct(c_v) - 1, 2);
            }

            richTextBox3.AppendText(iter_num.ToString() + ":" + Math.Round(e2, 2) + System.Environment.NewLine);


            richTextBox3.SelectionStart = richTextBox3.Text.Length;  
            richTextBox3.ScrollToCaret();  

            iter_num = iter_num + 1;

            return (e1, e2, q);
        }

        static Vector<double> CrossProduct(Vector<double> a, Vector<double> b)
        {
            if (a.Count != 3 || b.Count != 3)
            {
                throw new ArgumentException("Both vectors must be three-dimensional.");
            }

            double x = a[1] * b[2] - a[2] * b[1];
            double y = a[2] * b[0] - a[0] * b[2];
            double z = a[0] * b[1] - a[1] * b[0];

            return Vector.Build.DenseOfArray(new double[] { x, y, z });
        }

        private void btn_FK_Click(object sender, EventArgs e)
        {
            double V1 = 6658.2;
            double V2 = 49.7;
            double V3 = 35.5;
            double V4 = 2798.1;
            double V5 = -20.3;
            double V6 = -6.35;
            double V7 = 111.5;
            double[] q = Common.convertJoint(V1, V2 * (Math.PI / 180), V3 * (Math.PI / 180), V4, V5 * (Math.PI / 180), V6 * (Math.PI / 180), V7 * (Math.PI / 180)); 

            Matrix<double> T = Common.Fkine_LH4500(q[0], q[1], q[2], q[3], q[4], q[5], q[6], "08");
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
        

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (num_current > num_fresh)
            {
                num_current = 0;
                move_joint += 1;
            }

            if (move_joint > 4)
            {
                Task.Delay(1000).Wait();  
                timer2.Enabled = false;

                num_current = 0;
                timer3.Interval = 50;
                timer3.Enabled = true;
            }

            switch (move_joint)
            {
                case 1:  
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

                case 2:  
                    double q_current_2 = num_current * move_step[1];
                    Txt3.Text = q_current_2.ToString();
                    break;

                case 3:  
                    double q_current_3 = num_current * move_step[2];
                    Txt4.Text = q_current_3.ToString();
                    break;

                case 4:  
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


                    double q_current_5 = num_current * move_step[4];
                    Txt7.Text = q_current_5.ToString();


                    double q_current_6 = num_current * move_step[5];
                    Txt8.Text = q_current_6.ToString();


                    double q_current_7 = num_current * move_step[6];
                    Txt9.Text = q_current_7.ToString();
                    break;

            }
            num_current += 1;
        }

        double[] endObject_xyz;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (move_joint > 4) move_joint = 4;

            if (move_joint < 1)
            {
                timer3.Enabled = false;
            }

            if (num_current > num_fresh)
            {
                num_current = 0;
                move_joint -= 1;
            }

            endObject_xyz = showEndPosition();
            global.targetPoint.SetPosition(new Vector3((float)endObject_xyz[9], (float)endObject_xyz[10], (float)endObject_xyz[11]));

            switch (move_joint)
            {
                case 1:  
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

                case 2:  
                    double q_current_2 = (num_fresh - num_current) * move_step[1];
                    Txt3.Text = q_current_2.ToString();
                    break;

                case 3:  
                    double q_current_3 = (num_fresh - num_current) * move_step[2];
                    Txt4.Text = q_current_3.ToString();
                    break;

                case 4:  
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

                    double q_current_5 = (num_fresh - num_current) * move_step[4];
                    Txt7.Text = q_current_5.ToString();


                    double q_current_6 = (num_fresh - num_current) * move_step[5];
                    Txt8.Text = q_current_6.ToString();

                    double q_current_7 = (num_fresh - num_current) * move_step[6];
                    Txt9.Text = q_current_7.ToString();
                    break;
            }
            num_current += 1;
        }

        List<double[]> q_current_list = new List<double[]>();
        List<double[]> Td_list = new List<double[]>();
        double[] endTd;
        private void timer4_Tick(object sender, EventArgs e)
        {
            double[] q_current_array = new double[7];

            if (num_current > num_fresh-1)
            {
                timer4.Enabled = false;
                
            }

            q_current_array[0] = num_current * move_step[0];  
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

            q_current_array[1] = num_current * move_step[1];  
            Txt3.Text = q_current_array[1].ToString();

            q_current_array[2] = num_current * move_step[2];  
            Txt4.Text = q_current_array[2].ToString();

            q_current_array[3] = num_current * move_step[3];  
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

            q_current_array[4] = num_current * move_step[4];  
            Txt7.Text = q_current_array[4].ToString();

            q_current_array[5] = num_current * move_step[5];  
            Txt8.Text = q_current_array[5].ToString();

            q_current_array[6] = num_current * move_step[6];  
            Txt9.Text = q_current_array[6].ToString();

            q_current_list.Add(q_current_array);

            endTd = showEndPosition();
            Td_list.Add(endObject_xyz);


            num_current += 1;
        }


        private void chk_CB01_CheckedChanged(object sender, EventArgs e)
        {
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
        private void btn_FS_Click(object sender, EventArgs e)
        {
            camera._position= new Vector3(0.0f, 15.0f, 10.0f); 
            camera._right= new Vector3(0.0f, 0.0f, -1.0f);  
            camera._up = new Vector3(-1.0f, 0.0f, 0.0f);  

            chk_MJ01.Checked = false;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = true;
            chk_MJ04.Checked = false;
        }


        private void btn_CS_Click(object sender, EventArgs e)
        {
            camera._position = new Vector3(12.0f, 1.0f, 12.0f); ; 
            camera._right = new Vector3(0.0f, 0.0f, -1.0f);  
            camera._up = Vector3.UnitY;  

            chk_MJ01.Checked = true;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = false;
            chk_MJ04.Checked = false;
        }
        private void btn_ZS_Click(object sender, EventArgs e)
        {
            camera._position = new Vector3(0.0f, 1.0f, 18.0f); ; 
            camera._right = new Vector3(1.0f, 0.0f, 0.0f);  
            camera._up = Vector3.UnitY;  

            chk_MJ01.Checked = true;
            chk_MJ02.Checked = true;
            chk_MJ03.Checked = false;
            chk_MJ04.Checked = false;
        }
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
        private void btn_random_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            double randJoint1 = 11500.0 * rand.NextDouble();
            Txt1.Text = (randJoint1/2.0).ToString("F1");
            Txt2.Text = (randJoint1 / 2.0).ToString("F1");

            Txt3.Text = (-180.0+360.0 * rand.NextDouble()).ToString("F1");  
            Txt4.Text = (-35.0+75.0 * rand.NextDouble()).ToString("F1");  
            double randJoint4 = 3000.0 * rand.NextDouble();
            Txt5.Text = (randJoint4 / 2.0).ToString("F1");
            Txt6.Text = (randJoint4 / 2.0).ToString("F1");

            Txt7.Text = (-75.0 + 150.0 * rand.NextDouble()).ToString("F1");  
            Txt8.Text = (-105.0 + 135.0 * rand.NextDouble()).ToString("F1");  
            Txt9.Text = (-180.0 + 360.0 * rand.NextDouble()).ToString("F1");  
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

        public IXLWorksheet worksheet_PP;
        private bool isPaused;       
        private void btn_pp_show_Click(object sender, EventArgs e)
        {
            string defaultPath;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"J:\同步空间\BaiduSyncdisk\Matlab\路径规划\Data";  
            openFileDialog.Filter = "文本文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                defaultPath = openFileDialog.FileName;   
                XLWorkbook inventory_rb = new XLWorkbook(defaultPath);
                worksheet_PP = inventory_rb.Worksheet(1);   

                int rowCount = worksheet_PP.RowsUsed().Count();   

                PP_Stop = 0;               

                Task.Run(async () =>
                {
                    for (int i = 1; i < rowCount + 1; i++)
                    {
                        while (isPaused) await Task.Delay(100);  

                        float q_current_1 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 1).Value.ToString());
                        float q_current_2 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 2).Value.ToString());
                        float q_current_3 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 3).Value.ToString());
                        float q_current_4 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 4).Value.ToString());
                        float q_current_5 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 5).Value.ToString());
                        float q_current_6 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 6).Value.ToString());
                        float q_current_7 = (float)Convert.ToDouble(worksheet_PP.Cell(i, 7).Value.ToString());


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
                            for (int j = 0; j < chart1.Series.Count; j++)
                            {
                                double value = Convert.ToDouble(worksheet_PP.Cell(i, j + 1).Value.ToString());

                                AddPointToSeries(j, i, value);

                                dataGridView1.Rows[index].Cells[0].Value = i;
                                dataGridView1.Rows[index].Cells[j+1].Value = value.ToString("F1");

                            }

                        }));

                        glControl1.Invoke(new Action(() => glControl1_Paint(null, null)));

                        await Task.Delay(2);
                    }
                    PP_Stop = 1;          
                });
            }
        }

    }
}
