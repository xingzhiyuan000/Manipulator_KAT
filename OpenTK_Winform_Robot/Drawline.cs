using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Winform_Robot
{
    class Drawline
    {
        private int vbo, cbo, vao;
        private List<Vector3> points = new List<Vector3>();
        private List<Vector3> colors = new List<Vector3>(); // 存储每个点的颜色

        private float lineWidth = 2.0f;
        public Vector3 lineColor = new Vector3(1.0f, 0.0f, 0.0f);  // 默认绿色
        private Shader mLineShader = null;                          // 画线用的Shader
        private int bufferSize=2048;                                // 如果点过多则线条绘制不完

        public Drawline()
        {
            string exeDir = Application.StartupPath;
            string projectRoot = Directory.GetParent(exeDir).Parent.FullName;

            mLineShader = new Shader(projectRoot + "/GLSL/line.vert", projectRoot + "/GLSL/line.frag");
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            cbo = GL.GenBuffer();
        }

        public void AddPoint(Vector3 point)
        {
            if (point != Vector3.Zero) // 只添加非 (0,0,0) 的点
            {
                // 只有新点与上一个点不同，才添加
                if (points.Count == 0 || points[points.Count - 1] != point)
                {
                    points.Add(point);
                    colors.Add(lineColor); // 每个点都存储当前颜色
                    UpdateBuffer();
                }
                else
                {
                    Console.WriteLine("跳过重复点：" + point);
                }
            }
            else
            {
                Console.WriteLine("跳过了 (0,0,0) 点，未添加到轨迹中！");
            }
        }
        public void ClearPoints()
        {
            points.Clear(); // 清空列表
            colors.Clear();
            UpdateBuffer(); // 更新 VBO，防止 OpenGL 仍然绘制旧数据
            //Console.WriteLine("轨迹已清空！");
        }

        private void UpdateBuffer()
        {

            //GL.BindVertexArray(this.vertexArrayHandel); //绑定VAO

            //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, points.Count * sizeof(float) * 3, points.ToArray());

            //// 更新颜色数据
            //GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, colors.Count * sizeof(float) * 4, colors.ToArray());

            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(vao);

            // 绑定点坐标 VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, points.Count * Vector3.SizeInBytes, points.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            // 绑定颜色 VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Count * Vector3.SizeInBytes, colors.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0);


        }

        public void Draw()
        {

            if (points.Count < 2) return; // 至少要2个点

            //GL.UseProgram(shaderProgramID);  // 使用传入的 Shader ID
            //mLineShader.Begin(); //使用的Shader程序
            //GL.Uniform4(colorLocation, lineColor);

            //shaderLines.SetVector4("lineColor", lineColor);


            //mLineShader.Use(); // 使用着色器程序

            // 设置统一颜色 (如果你用的方法A: 整条线一个颜色)
            int colorLoc = GL.GetUniformLocation(mLineShader.ID, "lineColor");
            GL.Uniform3(colorLoc, lineColor);

            GL.LineWidth(lineWidth);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, points.Count);  // 绘制线条
            GL.BindVertexArray(0);
            //mLineShader.End();

        }

    }
}
