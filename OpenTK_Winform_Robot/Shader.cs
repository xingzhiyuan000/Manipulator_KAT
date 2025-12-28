using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Winform_Robot
{
    class Shader
    {
        public readonly int ID;  //Shader程序索引

        /// <summary>
        /// Shader的【编译与链接】
        /// </summary>
        /// <param name="vertPath"></param>
        /// <param name="fragPath"></param>
        public Shader(string vertPath, string fragPath)
        {
            // 【vertexShader】
            var shaderSource = File.ReadAllText(vertPath); //加载Shader文件
            var vertexShader = GL.CreateShader(ShaderType.VertexShader); //创建Shader
            GL.ShaderSource(vertexShader, shaderSource); //绑定Shader程序
            CompileShader(vertexShader); //编译Shader

            // 【fragmentShader一样操作】
            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);


            ID = GL.CreateProgram(); //创建Shader程序

            // Attach both shaders...
            GL.AttachShader(ID, vertexShader); //附着vertexShader
            GL.AttachShader(ID, fragmentShader);  //附着fragmentShader

            // And then link them together.
            LinkProgram(ID);  //链接程序


            // 【Shader进行释放】
            GL.DetachShader(ID, vertexShader);
            GL.DetachShader(ID, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport); //获取视窗大小

            GL.UseProgram(ID); //进入Shader程序

            SetFloat2("ViewportSize", (float)viewport[2], (float)viewport[3]);

            //int viewportSizeUniformLoaction = GL.GetUniformLocation(ID, "ViewportSize"); //获取Uniform在Shader中的位置编号
            //GL.Uniform2(viewportSizeUniformLoaction, (float)viewport[2], (float)viewport[3]); //向Shader中Uniform注入数据


        }

        /// <summary>
        /// 【编译Shader函数】
        /// </summary>
        /// <param name="shader">Shader索引</param>
        private static void CompileShader(int shader)
        {
            //【编译Shader】
            GL.CompileShader(shader);

            //【检测错误】
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        /// <summary>
        /// 【链接程序】
        /// </summary>
        /// <param name="program"></param>
        private static void LinkProgram(int program)
        {
            // 【链接程序】
            GL.LinkProgram(program);

            // 【检测错误】
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }


        /// <summary>
        /// 向Uniform中设置Int变量
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), data);
        }
        /// <summary>
        /// 设置【1维Uniform】变量.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat1(string name, float data)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), data);
        }

        //【2维Uniform】
        public void SetFloat2(string name, float data1, float data2)
        {
            GL.Uniform2(GL.GetUniformLocation(ID, name), data1, data2);
        }

        /// <summary>
        /// 【3维Uniform】
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, name), data);
        }

        /// <summary>
        /// 【4X4矩阵Uniform】
        /// </summary>
        public void SetMatrix4(string name, Matrix4 data)
        {
            //GL.UniformMatrix4(GL.GetUniformLocation(ID, name), 1, false, Matrix4ToArray(data));
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), 1, false, ref data.Row0.X);
        }

        /// <summary>
        /// 【4X4矩阵数组Uniform】
        /// </summary>
        public void SetMatrix4Array(string name, Matrix4[] data, int count)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), count, false, ref data[0].Row0.X);
        }

        /// <summary>
        /// 【3X3矩阵Uniform】
        /// </summary>
        public void SetMatrix3(string name, Matrix3 data)
        {
            GL.UniformMatrix3(GL.GetUniformLocation(ID, name), 1, false, Matrix3ToArray(data));
        }

        private float[] Matrix4ToArray(Matrix4 matrix)
        {
            float[] data = new float[16];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    data[i * 4 + j] = matrix[i, j];

                }
            }
            return data;
        }

        private float[] Matrix3ToArray(Matrix3 matrix)
        {
            float[] data = new float[9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    data[i * 3 + j] = matrix[i, j];

                }
            }
            return data;
        }

        /// <summary>
        /// 使用Shader
        /// </summary>
        public void Begin()
        {
            GL.UseProgram(ID);
        }

        /// <summary>
        /// 结束Shader
        /// </summary>
        public void End()
        {
            GL.UseProgram(0);
        }
    }
}
