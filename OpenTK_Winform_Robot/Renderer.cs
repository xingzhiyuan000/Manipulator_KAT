using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK_Winform_Robot.Meshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    class Renderer
    {
        private Shader mPhongShader = null;
        private Shader mWhiteShader = null;
        private Shader mDepthShader = null;
        private Shader mCubeShader = null;
        private Shader mPhongEnvShader = null;
        private Shader mPhongInstanceShader = null;
        private Shader mSphereShader = null;
        private Shader mAxisShader = null;
        private Shader mLineShader = null;          // 画线用的Shader

        public List<Mesh> mOpacityObjects=new List<Mesh>();         //【不透明物体】
        public List<Mesh> mTransparentObjects = new List<Mesh>();   //【透明物体】

        public Renderer()
        {
            mPhongShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/phong.vert", Directory.GetCurrentDirectory() + "/GLSL/phong.frag");
            mWhiteShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/white.vert", Directory.GetCurrentDirectory() + "/GLSL/white.frag");
            mDepthShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/depth.vert", Directory.GetCurrentDirectory() + "/GLSL/depth.frag");
            mCubeShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/cube.vert", Directory.GetCurrentDirectory() + "/GLSL/cube.frag");
            mPhongEnvShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/phongEnv.vert", Directory.GetCurrentDirectory() + "/GLSL/phongEnv.frag");
            mPhongInstanceShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/phongInstance.vert", Directory.GetCurrentDirectory() + "/GLSL/phongInstance.frag");
            mSphereShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/sphere.vert", Directory.GetCurrentDirectory() + "/GLSL/sphere.frag");
            //mAxisShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/axis.vert", Directory.GetCurrentDirectory() + "/GLSL/axis.frag");
            mLineShader = new Shader(Directory.GetCurrentDirectory() + "/GLSL/line.vert", Directory.GetCurrentDirectory() + "/GLSL/line.frag");
        }

        void renderObject(
        Object obj,
        Camera camera,
        Light light,
        Matrix4 projectionMatrix,
        float specularIntensity,
        float shiness
        )
        {
            //1 判断是Mesh还是Object，如果是Mesh需要渲染
            if (obj!=null && (obj.getType()==ObjectType.Mesh || obj.getType()== ObjectType.InstancedMesh))
            {
                var mesh =(Mesh)obj ;
                var geometry = mesh.mGeometry;
                var material = mesh.mMaterial;

                //1.设置渲染状态-【深度检查】
                setDepthState(material);

                //2. 设置【polygonOffset】
                setPolygonOffsetState(material);

                //3. 设置【深度测试】
                setStencilState(material);

                //设置【面剔除】
                setFaceCullingState(material);

                //设置【颜色混合】
                setBlendState(material);

                //1 决定用那个Shader
                Shader shader = pickShader(material.mType);

                //2 更新shader的uniform
                shader.Begin(); //使用的Shader程序
                switch (material.mType)
                {
                    case MaterialType.PhongMaterial:
                        {
                            
                            shader.SetInt("sampler", 0); //【采样器与纹理单元挂钩】
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); //【采样器与纹理单元挂钩】
                            if (material.mSpecularMask!=null) material.mSpecularMask.Bind();
                            
                            //mvp
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix()))); // 计算法线矩阵

                            shader.SetMatrix3("normalMatrix", normalMatrix); //设置【法线矩阵】

                            shader.SetVector3("lightColor", light.mLightColor); //【平行光强度】
                            shader.SetVector3("lightDirection", light.mDirection); //【平行光方向】
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); //【光斑大小】
                            shader.SetFloat1("opacity", material.mOpacity); //【光斑大小】

                            shader.SetVector3("ambientColor", light.mAmbientColor);//【环境光强度】
                            shader.SetVector3("cameraPosition", camera._position); //【相机信息】
                            break;
                        }
                    case MaterialType.WhiteMaterial:
                        {
                            //mvp
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            break;
                        }
                    case MaterialType.DepthMaterial:
                        //mvp
                        shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                        shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                        shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                        shader.SetFloat1("near", camera.pNear);
                        shader.SetFloat1("far", camera.pFar);

                        break;
                    case MaterialType.CubeMaterial:

                        mesh.SetPosition(camera._position);

                        shader.SetInt("cubeSampler", 0); //【采样器与纹理单元挂钩】
                        
                        //mvp
                        shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                        shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                        shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】
                        
                        material.mDiffuse.Bind();

                        break;
                    case MaterialType.PhongEnvMaterial:
                        {
                            shader.SetInt("sampler", 0); //【采样器与纹理单元挂钩】
                            material.mDiffuse.Bind();

                            shader.SetInt("envSampler", 1); //【采样器与纹理单元挂钩】-填空盒环境光
                            material.mEnv.Bind();

                            shader.SetInt("specularMaskSampler", 2); //【采样器与纹理单元挂钩】-高光
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            //mvp
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix()))); // 计算法线矩阵

                            shader.SetMatrix3("normalMatrix", normalMatrix); //设置【法线矩阵】

                            shader.SetVector3("lightColor", light.mLightColor); //【平行光强度】
                            shader.SetVector3("lightDirection", light.mDirection); //【平行光方向】
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); //【光斑大小】

                            shader.SetVector3("ambientColor", light.mAmbientColor);//【环境光强度】
                            shader.SetVector3("cameraPosition", camera._position); //【相机信息】
                            break;
                        }
                    case MaterialType.PhongInstanceMaterial:
                        {
                            InstancedMesh im = (InstancedMesh)mesh;
                            shader.SetInt("sampler", 0); //【采样器与纹理单元挂钩】
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); //【采样器与纹理单元挂钩】
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            //mvp
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix()))); // 计算法线矩阵

                            shader.SetMatrix3("normalMatrix", normalMatrix); //设置【法线矩阵】

                            shader.SetVector3("lightColor", light.mLightColor); //【平行光强度】
                            shader.SetVector3("lightDirection", light.mDirection); //【平行光方向】
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); //【光斑大小】
                            shader.SetFloat1("opacity", material.mOpacity); //【光斑大小】

                            shader.SetVector3("ambientColor", light.mAmbientColor);//【环境光强度】
                            shader.SetVector3("cameraPosition", camera._position); //【相机信息】

                            shader.SetMatrix4Array("matrices",im.mInstanceMatrices,im.mInstanceCount);
                            break;
                        }
                    case MaterialType.SphereMaterial:
                        {
                            mesh.SetPosition(camera._position);

                            shader.SetInt("sphericalSampler", 0); //【采样器与纹理单元挂钩】

                            //mvp
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            material.mDiffuse.Bind();

                            break;
                        }
                    case MaterialType.AxisMaterial:
                        {
                            //Vector3 offset = new Vector3(-10.0f, 0.0f, 0.0f);
                            //mesh.SetPosition(camera._position + offset);
                            shader.SetInt("sampler", 0); //【采样器与纹理单元挂钩】
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); //【采样器与纹理单元挂钩】
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            //mvp
                            shader.SetMatrix4("transform", mesh.GetAxisModelMatrix()); //设置【模型变换矩阵】
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); //设置【摄像机变换矩阵】
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); //设置【透视变换矩阵】

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetAxisModelMatrix()))); // 计算法线矩阵

                            shader.SetMatrix3("normalMatrix", normalMatrix); //设置【法线矩阵】

                            shader.SetVector3("lightColor", light.mLightColor); //【平行光强度】
                            shader.SetVector3("lightDirection", light.mDirection); //【平行光方向】
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); //【光斑大小】
                            shader.SetFloat1("opacity", material.mOpacity); //【光斑大小】

                            shader.SetVector3("ambientColor", light.mAmbientColor);//【环境光强度】
                            shader.SetVector3("cameraPosition", camera._position); //【相机信息】
                            break;
                        }
                    default:
                        break;
                }


                //3 绑定Vao 
                GL.BindVertexArray(geometry.getVao()); //绑定VAO
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.getEbo());//绑定EBO
                if (obj.getType() == ObjectType.InstancedMesh)
                {
                    InstancedMesh im = (InstancedMesh)mesh;
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, geometry.getIndicesCount(), DrawElementsType.UnsignedInt, IntPtr.Zero, (int)im.mInstanceCount);

                }
                else GL.DrawElements(PrimitiveType.Triangles, geometry.getIndicesCount(), DrawElementsType.UnsignedInt, 0); ;


                //shader.End();
            }

            ////2 遍历object的子节点，对每个子节点都需要调用renderObject
            //var children = obj.getChild();
            //for (int i = 0; i < children.Count; i++)
            //{
            //    renderObject(children[i], camera, light, projectionMatrix, specularIntensity, shiness);
            //}
        }

        public void Render
        (
        Scene scene,
        Camera camera,
        Light light,
        Matrix4 projectionMatrix,
        float specularIntensity,
        float shiness
        )
        {
            GL.Enable(EnableCap.DepthTest);//开启【深度检测】-遮挡关系
            GL.DepthFunc(DepthFunction.Less);//【深度检测方式】-数值小的通过
            GL.DepthMask(true); //打开【深度写入】
            GL.Disable(EnableCap.PolygonOffsetFill);//关闭【PolygonOffset】
            GL.Disable(EnableCap.PolygonOffsetLine);

            GL.Disable(EnableCap.Blend); //默认关闭【颜色混合】

            GL.Enable(EnableCap.StencilTest);//开启【模版测试】-选中状态
            GL.StencilOp(StencilOp.Keep,StencilOp.Keep,StencilOp.Keep); //设置【模版测试】状态
            GL.StencilMask(0xFF);//开启【模版测试】写入-保证模版缓冲可以被清理

            //1.清理画布和深度缓存
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit| ClearBufferMask.StencilBufferBit);

            //.清空物体队列
            mOpacityObjects.Clear();
            mTransparentObjects.Clear();
            projectObject(scene); //【分离透明物体】

            // 对透明物体排序

            //mTransparentObjects.Sort((a, b) =>
            //{
            //    var viewMatrix = camera.GetViewMatrix();

            //    // 1. 计算 a 的相机系的 Z
            //    var modelMatrixA = a.GetModelMatrix();
            //    var worldPositionA = modelMatrixA * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            //    var cameraPositionA = viewMatrix * worldPositionA;

            //    // 2. 计算 b 的相机系的 Z
            //    var modelMatrixB = b.GetModelMatrix();
            //    var worldPositionB = modelMatrixB * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            //    var cameraPositionB = viewMatrix * worldPositionB;

            //    return cameraPositionA.Z.CompareTo(cameraPositionB.Z);
            //});

            //3 渲染两个队列
            for (int i = 0; i < mOpacityObjects.Count; i++)
            {
                renderObject(mOpacityObjects[i], camera, light, projectionMatrix, specularIntensity, shiness);
            }

            for (int i = 0; i < mTransparentObjects.Count; i++)
            {
                renderObject(mTransparentObjects[i], camera, light, projectionMatrix, specularIntensity, shiness);
            }

            ////2. 将scene当作根节点开始递归渲染
            //renderObject(scene,camera,light,projectionMatrix,specularIntensity,shiness);
        }

        /// <summary>
        /// 根据类型返回对应的Shader
        /// </summary>
        /// <param name="materialType"></param>
        /// <returns></returns>
        private Shader pickShader(MaterialType materialType)
        {
            Shader result = null;
            switch (materialType)
            {
                case MaterialType.PhongMaterial:
                    result = mPhongShader;
                        break;
                case MaterialType.WhiteMaterial:
                    result = mWhiteShader;
                    break;
                case MaterialType.DepthMaterial:
                    result = mDepthShader;
                    break;
                case MaterialType.CubeMaterial:
                    result = mCubeShader;
                    break;
                case MaterialType.PhongEnvMaterial:
                    result = mPhongEnvShader;
                    break;
                case MaterialType.PhongInstanceMaterial:
                    result = mPhongInstanceShader;
                    break;
                case MaterialType.SphereMaterial:
                    result = mSphereShader;
                    break;
                case MaterialType.AxisMaterial:
                    result = mAxisShader;
                    break;
                default:
                    MessageBox.Show("未找到Shader类型");
                    break;
            }
            return result;
        }
        /// <summary>
        /// 【分离透明物体和不透明物体】
        /// </summary>
        private void projectObject(Object obj)
        {
            if (obj.getType()==ObjectType.Mesh || obj.getType() == ObjectType.InstancedMesh)
            {
                Mesh mesh = (Mesh)obj;
                Material material = mesh.mMaterial;
                if (material.mBlend) mTransparentObjects.Add(mesh); // 如果是透明物体
                else mOpacityObjects.Add(mesh);
            }

            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                projectObject(children[i]);
            }
        }
        /// <summary>
        /// 【设置深度状态】
        /// </summary>
        void setDepthState(Material material)
        {
            if (material.mDepthTest)
            {
                GL.Enable(EnableCap.DepthTest); //打开【深度检测】
                GL.DepthFunc(material.mDepthFunc); //设置【深度检测方法】
            }
            else GL.Disable(EnableCap.DepthTest); //关闭【深度检测】

            if (material.mDepthWrite) GL.DepthMask(true); //打开【深度写入】
            else GL.DepthMask(false); //关闭【深度写入】
        }
        /// <summary>
        /// 【设置PolygonOffset状态】
        /// </summary>
        void setPolygonOffsetState(Material material)
        {
            if (material.mPolygonOffset)
            {
                GL.Enable(material.mPolygonOffsetType);
                GL.PolygonOffset(material.mFactor, material.mUnit);
            }
            else
            {
                GL.Disable(EnableCap.PolygonOffsetFill);//关闭【PolygonOffset】
                GL.Disable(EnableCap.PolygonOffsetLine);
            }
        }
        /// <summary>
        /// 【设置深度测试状态】
        /// </summary>
        void setStencilState(Material material)
        {
            if (material.mStencilTest)
            {
                GL.Enable(EnableCap.StencilTest);//开启【模版测试】-选中状态
                GL.StencilOp(material.mSFail, material.mZFail, material.mZPass); //设置【模版测试】状态
                GL.StencilMask(material.mStencilMask);//开启【模版测试】写入-保证模版缓冲可以被清理
                GL.StencilFunc(material.mStencilFunc,material.mStencilRef,material.mStencilFuncMask);
            }
            else GL.Disable(EnableCap.StencilTest); //关闭【模版测试】
        }
        //【设置面剔除状态】
        void setFaceCullingState(Material material)
        {
            if (material.mFaceCulling)
            {
                GL.Enable(EnableCap.CullFace);//开启【面剔除】
                GL.FrontFace(material.mFrontFace); //【逆时针朝前】
                GL.CullFace(material.mCullFace); //【剔除背面】
            }
            else GL.Disable(EnableCap.CullFace);//关闭【面剔除】

        }
        /// <summary>
        /// 【设置颜色混合状态】
        /// </summary>
        void setBlendState(Material material)
        {
            if (material.mBlend)
            {
                GL.Enable(EnableCap.Blend); //开启【颜色混合】
                GL.BlendFunc(material.mSFactor, material.mDFactor);//【颜色混合方式】
            }
            else
            {
                GL.Disable(EnableCap.Blend);//关闭【颜色混合】
            }
           
        }
    }
}
