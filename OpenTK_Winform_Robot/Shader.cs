using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Winform_Robot
{
    class Shader
    {
        public readonly int ID;  

        public Shader(string vertPath, string fragPath)
        {
            var shaderSource = File.ReadAllText(vertPath); 
            var vertexShader = GL.CreateShader(ShaderType.VertexShader); 
            GL.ShaderSource(vertexShader, shaderSource); 
            CompileShader(vertexShader); 

            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);


            ID = GL.CreateProgram(); 

            GL.AttachShader(ID, vertexShader); 
            GL.AttachShader(ID, fragmentShader);  

            LinkProgram(ID);  


            GL.DetachShader(ID, vertexShader);
            GL.DetachShader(ID, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport); 

            GL.UseProgram(ID); 

            SetFloat2("ViewportSize", (float)viewport[2], (float)viewport[3]);


        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }


        public void SetInt(string name, int data)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), data);
        }
        public void SetFloat1(string name, float data)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), data);
        }

        public void SetFloat2(string name, float data1, float data2)
        {
            GL.Uniform2(GL.GetUniformLocation(ID, name), data1, data2);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, name), data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), 1, false, ref data.Row0.X);
        }

        public void SetMatrix4Array(string name, Matrix4[] data, int count)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), count, false, ref data[0].Row0.X);
        }

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

        public void Begin()
        {
            GL.UseProgram(ID);
        }

        public void End()
        {
            GL.UseProgram(0);
        }
    }
}
