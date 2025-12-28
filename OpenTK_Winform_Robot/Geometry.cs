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
            mPosVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mUvVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mNormalVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mEbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEbo); 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 

            mVao = GL.GenVertexArray();
            GL.BindVertexArray(mVao); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); 
            GL.EnableVertexAttribArray(0); 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); 
            GL.EnableVertexAttribArray(1); 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); 
            GL.EnableVertexAttribArray(2); 
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindVertexArray(0); 

            mIndicesCount = indices.Count;

        }

        public Geometry(
        List<float> positions,
        List<float> colors,
        List<float> normals,
        List<float> uvs,
        List<uint> indices)
        {
            mPosVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mUvVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mNormalVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mColorVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Count * sizeof(float), colors.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            mEbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mEbo); 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 

            mVao = GL.GenVertexArray();
            GL.BindVertexArray(mVao); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mPosVbo); 
            GL.EnableVertexAttribArray(0); 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mUvVbo); 
            GL.EnableVertexAttribArray(1); 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mNormalVbo); 
            GL.EnableVertexAttribArray(2); 
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorVbo); 
            GL.EnableVertexAttribArray(3); 
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindVertexArray(0); 

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
            -halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize,
            -halfSize, -halfSize, -halfSize, -halfSize, halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, -halfSize, -halfSize,
            -halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, halfSize, -halfSize, -halfSize, halfSize, -halfSize,
            -halfSize, -halfSize, -halfSize, halfSize, -halfSize, -halfSize, halfSize, -halfSize, halfSize, -halfSize, -halfSize, halfSize,
            halfSize, -halfSize, halfSize, halfSize, -halfSize, -halfSize, halfSize, halfSize, -halfSize, halfSize, halfSize, halfSize,
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
		        0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
		        0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,

		        0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,

		        0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,

		        1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,

		        -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
            };

            uint[] indices = {
            0, 1, 2, 2, 3, 0,     
            4, 5, 6, 6, 7, 4,     
            8, 9, 10, 10, 11, 8,    
            12, 13, 14, 14, 15, 12,   
            16, 17, 18, 18, 19, 16,   
            20, 21, 22, 22, 23, 20    
            };

            geometry.mPosVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mUvVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mNormalVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mEbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 

            geometry.mVao = GL.GenVertexArray();
            GL.BindVertexArray(geometry.mVao); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.EnableVertexAttribArray(0); 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.EnableVertexAttribArray(1); 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.EnableVertexAttribArray(2); 
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindVertexArray(0); 

            geometry.mIndicesCount = indices.Length;

            return geometry;

        }

        public Geometry createSphere(float size)
        {
            Geometry geometry = new Geometry();
            int numLatLines = 60;  
            int numLongLines = 60;  
            float radius = size;

            List<float> positions = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();
            List<int> indices = new List<int>();


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

            geometry.mPosVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * sizeof(float), positions.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mUvVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Count * sizeof(float), uvs.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mNormalVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count * sizeof(float), normals.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mEbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(float), indices.ToArray(), BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 

            geometry.mVao = GL.GenVertexArray();
            GL.BindVertexArray(geometry.mVao); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.EnableVertexAttribArray(0); 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.EnableVertexAttribArray(1); 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.EnableVertexAttribArray(2); 
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindVertexArray(0); 

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

            geometry.mPosVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mUvVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mNormalVbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            geometry.mEbo = GL.GenBuffer(); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.mEbo); 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 

            geometry.mVao = GL.GenVertexArray();
            GL.BindVertexArray(geometry.mVao); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mPosVbo); 
            GL.EnableVertexAttribArray(0); 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mUvVbo); 
            GL.EnableVertexAttribArray(1); 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); 

            GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.mNormalVbo); 
            GL.EnableVertexAttribArray(2); 
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); 

            GL.BindVertexArray(0); 

            geometry.mIndicesCount = indices.Length;

            return geometry;

        }
    }
}
