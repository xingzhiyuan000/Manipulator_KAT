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
        private List<Vector3> colors = new List<Vector3>();  

        private float lineWidth = 2.0f;
        public Vector3 lineColor = new Vector3(1.0f, 0.0f, 0.0f);   
        private Shader mLineShader = null;                           
        private int bufferSize=2048;                                 

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
            if (point != Vector3.Zero)    
            {
                if (points.Count == 0 || points[points.Count - 1] != point)
                {
                    points.Add(point);
                    colors.Add(lineColor);  
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
            points.Clear();  
            colors.Clear();
            UpdateBuffer();     
        }

        private void UpdateBuffer()
        {

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, points.Count * Vector3.SizeInBytes, points.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Count * Vector3.SizeInBytes, colors.ToArray(), BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0);


        }

        public void Draw()
        {

            if (points.Count < 2) return;  


            int colorLoc = GL.GetUniformLocation(mLineShader.ID, "lineColor");
            GL.Uniform3(colorLoc, lineColor);

            GL.LineWidth(lineWidth);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, points.Count);   
            GL.BindVertexArray(0);
        }

    }
}
