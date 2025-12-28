using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Winform_Robot
{
    public class Geometry
    {

        private int mVao = 0;
        private int mPosVbo = 0;
        private int mUvVbo = 0;
        private int mNormalVbo = 0;
        private int mEbo = 0;
        private int mColorVbo = 0;

        private int mIndicesCount = 0;

        public Geometry()
        { 
        
        }

        public Geometry(
        List<float> positions,
        List<float> normals,
        List<float> uvs,
        List<uint> indices)
        {
            //2.VBO操作-位置
            mPosVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-uv
            mUvVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-normal
            mNormalVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.EBO操作
            mEbo = GL.GenBuffer(); //生成EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEbo); //绑定EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); //向EBO注入数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //解绑EBO

            //3.VAO操作
            mVao = GL.GenVertexArray();//创建VAO
            GL.BindVertexArray(mVao); //绑定VAO

            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); //绑定VBO-位置
            GL.EnableVertexAttribArray(0); //激活VAO中0号位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【位置】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(1); //激活VAO中1号位置
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); //向VAO中压入【uv】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(2); //激活VAO中2号位置
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【颜色】属性

            GL.BindVertexArray(0); //解绑VAO

            mIndicesCount = indices.Count;

        }

        public Geometry(
        List<float> positions,
        List<float> colors,
        List<float> normals,
        List<float> uvs,
        List<uint> indices)
        {
            //2.VBO操作-位置
            mPosVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-uv
            mUvVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-normal
            mNormalVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-color
            mColorVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Count * sizeof(float), colors.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.EBO操作
            mEbo = GL.GenBuffer(); //生成EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEbo); //绑定EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); //向EBO注入数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //解绑EBO

            //3.VAO操作
            mVao = GL.GenVertexArray();//创建VAO
            GL.BindVertexArray(mVao); //绑定VAO

            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); //绑定VBO-位置
            GL.EnableVertexAttribArray(0); //激活VAO中0号位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【位置】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(1); //激活VAO中1号位置
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); //向VAO中压入【uv】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); //绑定VBO-法线
            GL.EnableVertexAttribArray(2); //激活VAO中2号位置
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【法线】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorVbo); //绑定VBO-颜色
            GL.EnableVertexAttribArray(3); //激活VAO中3号位置
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【颜色】属性

            GL.BindVertexArray(0); //解绑VAO

            mIndicesCount = indices.Count;

        }

        public int getVao()
        {
            return mVao;
        }

        public int getEbo()
        {
            return mEbo;
        }

        public int getIndicesCount()
        {
            return mIndicesCount;
        }

        public Geometry createBox(float size)
        {
            Geometry geometry = new Geometry();
            float halfSize = size / 2.0f;
            float[] positions = {
            // Front face
            -halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize,
            // Back face
            -halfSize, -halfSize, -halfSize, -halfSize, halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, -halfSize, -halfSize,
            // Top face
            -halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, -halfSize, -halfSize, halfSize, -halfSize,
            // Bottom face
            -halfSize, -halfSize, -halfSize, halfSize, -halfSize, -halfSize, halfSize, -halfSize, halfSize, -halfSize, -halfSize, halfSize,
            // Right face
            halfSize, -halfSize, halfSize, halfSize, -halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize, halfSize,
            // Left face
            -halfSize, -halfSize, -halfSize, -halfSize, -halfSize, halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, -halfSize
            };

            float[] uvs = {
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f
            };

            float[] normals = {
		        //前面
		        0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
		        //后面
		        0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,

		        //上面
		        0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,

		        //下面
		        0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,

		        //右面
		        1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,

		        //左面
		        -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
            };

            uint[] indices = {
            0, 1, 2, 2, 3, 0,   // Front face
            4, 5, 6, 6, 7, 4,   // Back face
            8, 9, 10, 10, 11, 8,  // Top face
            12, 13, 14, 14, 15, 12, // Bottom face
            16, 17, 18, 18, 19, 16, // Right face
            20, 21, 22, 22, 23, 20  // Left face
            };

            //2.VBO操作-位置
            geometry.mPosVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-uv
            geometry.mUvVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-normal
            geometry.mNormalVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.EBO操作
            geometry.mEbo = GL.GenBuffer(); //生成EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); //绑定EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw); //向EBO注入数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //解绑EBO

            //3.VAO操作
            geometry.mVao = GL.GenVertexArray();//创建VAO
            GL.BindVertexArray(geometry.mVao); //绑定VAO

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO-位置
            GL.EnableVertexAttribArray(0); //激活VAO中0号位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【位置】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(1); //激活VAO中1号位置
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); //向VAO中压入【uv】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(2); //激活VAO中2号位置
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【颜色】属性

            GL.BindVertexArray(0); //解绑VAO

            geometry.mIndicesCount = indices.Length;

            return geometry;

        }

        public Geometry createSphere(float size)
        {
            Geometry geometry = new Geometry();
            int numLatLines = 60; // 纬线数量
            int numLongLines = 60; // 经线数量
            float radius = size;

            List<float> positions = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();
            List<int> indices = new List<int>();


            // 生成位置和UV坐标
            // 生成位置和UV坐标
            for (int i = 0; i <= numLatLines; i++)
            {
                for (int j = 0; j <= numLongLines; j++)
                {
                    float phi = i * MathHelper.Pi / numLatLines;
                    float theta = j * 2 * MathHelper.Pi / numLongLines;

                    float y = radius * (float)Math.Cos(phi);
                    float x = radius * (float)Math.Sin(phi) * (float)Math.Cos(theta);
                    float z = radius * (float)Math.Sin(phi) * (float)Math.Sin(theta);

                    positions.Add(x);
                    positions.Add(y);
                    positions.Add(z);

                    float u = 1.0f - (float)j / numLongLines;
                    float v = 1.0f - (float)i / numLatLines;

                    uvs.Add(u);
                    uvs.Add(v);

                    normals.Add(x);
                    normals.Add(y);
                    normals.Add(z);
                }
            }

            // 生成顶点索引
            for (int i = 0; i < numLatLines; i++)
            {
                for (int j = 0; j < numLongLines; j++)
                {
                    int p1 = i * (numLongLines + 1) + j;
                    int p2 = p1 + numLongLines + 1;
                    int p3 = p1 + 1;
                    int p4 = p2 + 1;

                    indices.Add(p1);
                    indices.Add(p2);
                    indices.Add(p3);

                    indices.Add(p3);
                    indices.Add(p2);
                    indices.Add(p4);
                }
            }

            //2.VBO操作-【位置】
            geometry.mPosVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-【uv】
            geometry.mUvVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-【normal】
            geometry.mNormalVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.EBO操作
            geometry.mEbo = GL.GenBuffer(); //生成EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); //绑定EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); //向EBO注入数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //解绑EBO

            //3.VAO操作
            geometry.mVao = GL.GenVertexArray();//创建VAO
            GL.BindVertexArray(geometry.mVao); //绑定VAO

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO-位置
            GL.EnableVertexAttribArray(0); //激活VAO中0号位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【位置】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(1); //激活VAO中2号位置
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); //向VAO中压入【uv】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO-normal
            GL.EnableVertexAttribArray(2); //激活VAO中2号位置
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【Normal】属性

            GL.BindVertexArray(0); //解绑VAO

            geometry.mIndicesCount = indices.Count;

            return geometry;

        }

        public Geometry createPlane(float width, float height)
        {
            Geometry geometry = new Geometry();
            float halfW = width / 2.0f;
            float halfH = height / 2.0f;

            float[] positions = {
            -halfW, -halfH, 0.0f,
            halfW, -halfH, 0.0f,
            halfW, halfH, 0.0f,
            -halfW, halfH, 0.0f
            };

            float[] uvs = {
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f
            };

            float[] normals = {
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f
            };
            uint[] indices = {
            0, 1, 2,
            2, 3, 0
            };

            //2.VBO操作-位置
            geometry.mPosVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-uv
            geometry.mUvVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.VBO操作-【normal】
            geometry.mNormalVbo = GL.GenBuffer(); //生成VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw); //向VBO注入数据
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //解绑VBO

            //2.EBO操作
            geometry.mEbo = GL.GenBuffer(); //生成EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); //绑定EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw); //向EBO注入数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //解绑EBO

            //3.VAO操作
            geometry.mVao = GL.GenVertexArray();//创建VAO
            GL.BindVertexArray(geometry.mVao); //绑定VAO

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); //绑定VBO-位置
            GL.EnableVertexAttribArray(0); //激活VAO中0号位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【位置】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); //绑定VBO-uv
            GL.EnableVertexAttribArray(1); //激活VAO中2号位置
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); //向VAO中压入【uv】属性

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); //绑定VBO-normal
            GL.EnableVertexAttribArray(2); //激活VAO中2号位置
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //向VAO中压入【Normal】属性

            GL.BindVertexArray(0); //解绑VAO

            geometry.mIndicesCount = indices.Length;

            return geometry;

        }
    }
}
