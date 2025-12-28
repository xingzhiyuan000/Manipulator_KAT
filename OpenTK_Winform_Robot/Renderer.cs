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
        private Shader mLineShader = null;           

        public List<Mesh> mOpacityObjects=new List<Mesh>();         
        public List<Mesh> mTransparentObjects = new List<Mesh>();   

        
        public Renderer()
        {
            string exeDir = Application.StartupPath;
            string projectRoot = Directory.GetParent(exeDir).Parent.FullName;

            mPhongShader = new Shader(projectRoot + "/GLSL/phong.vert", projectRoot + "/GLSL/phong.frag");
            mWhiteShader = new Shader(projectRoot + "/GLSL/white.vert", projectRoot + "/GLSL/white.frag");
            mDepthShader = new Shader(projectRoot + "/GLSL/depth.vert", projectRoot + "/GLSL/depth.frag");
            mCubeShader = new Shader(projectRoot + "/GLSL/cube.vert", projectRoot + "/GLSL/cube.frag");
            mPhongEnvShader = new Shader(projectRoot + "/GLSL/phongEnv.vert", projectRoot + "/GLSL/phongEnv.frag");
            mPhongInstanceShader = new Shader(projectRoot + "/GLSL/phongInstance.vert", projectRoot + "/GLSL/phongInstance.frag");
            mSphereShader = new Shader(projectRoot + "/GLSL/sphere.vert", projectRoot + "/GLSL/sphere.frag");
            mLineShader = new Shader(projectRoot + "/GLSL/line.vert", projectRoot + "/GLSL/line.frag");
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
            if (obj!=null && (obj.getType()==ObjectType.Mesh || obj.getType()== ObjectType.InstancedMesh))
            {
                var mesh =(Mesh)obj ;
                var geometry = mesh.mGeometry;
                var material = mesh.mMaterial;

                setDepthState(material);

                setPolygonOffsetState(material);

                setStencilState(material);

                setFaceCullingState(material);

                setBlendState(material);

                Shader shader = pickShader(material.mType);

                shader.Begin(); 
                switch (material.mType)
                {
                    case MaterialType.PhongMaterial:
                        {
                            
                            shader.SetInt("sampler", 0); 
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); 
                            if (material.mSpecularMask!=null) material.mSpecularMask.Bind();
                            
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix())));  

                            shader.SetMatrix3("normalMatrix", normalMatrix); 

                            shader.SetVector3("lightColor", light.mLightColor); 
                            shader.SetVector3("lightDirection", light.mDirection); 
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); 
                            shader.SetFloat1("opacity", material.mOpacity); 

                            shader.SetVector3("ambientColor", light.mAmbientColor);
                            shader.SetVector3("cameraPosition", camera._position); 
                            break;
                        }
                    case MaterialType.WhiteMaterial:
                        {
                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            break;
                        }
                    case MaterialType.DepthMaterial:
                        shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                        shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                        shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                        shader.SetFloat1("near", camera.pNear);
                        shader.SetFloat1("far", camera.pFar);

                        break;
                    case MaterialType.CubeMaterial:

                        mesh.SetPosition(camera._position);

                        shader.SetInt("cubeSampler", 0); 
                        
                        shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                        shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                        shader.SetMatrix4("projectionMatrix", projectionMatrix); 
                        
                        material.mDiffuse.Bind();

                        break;
                    case MaterialType.PhongEnvMaterial:
                        {
                            shader.SetInt("sampler", 0); 
                            material.mDiffuse.Bind();

                            shader.SetInt("envSampler", 1); 
                            material.mEnv.Bind();

                            shader.SetInt("specularMaskSampler", 2); 
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix())));  

                            shader.SetMatrix3("normalMatrix", normalMatrix); 

                            shader.SetVector3("lightColor", light.mLightColor); 
                            shader.SetVector3("lightDirection", light.mDirection); 
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); 

                            shader.SetVector3("ambientColor", light.mAmbientColor);
                            shader.SetVector3("cameraPosition", camera._position); 
                            break;
                        }
                    case MaterialType.PhongInstanceMaterial:
                        {
                            InstancedMesh im = (InstancedMesh)mesh;
                            shader.SetInt("sampler", 0); 
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); 
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetModelMatrix())));  

                            shader.SetMatrix3("normalMatrix", normalMatrix); 

                            shader.SetVector3("lightColor", light.mLightColor); 
                            shader.SetVector3("lightDirection", light.mDirection); 
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); 
                            shader.SetFloat1("opacity", material.mOpacity); 

                            shader.SetVector3("ambientColor", light.mAmbientColor);
                            shader.SetVector3("cameraPosition", camera._position); 

                            shader.SetMatrix4Array("matrices",im.mInstanceMatrices,im.mInstanceCount);
                            break;
                        }
                    case MaterialType.SphereMaterial:
                        {
                            mesh.SetPosition(camera._position);

                            shader.SetInt("sphericalSampler", 0); 

                            shader.SetMatrix4("transform", mesh.GetModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            material.mDiffuse.Bind();

                            break;
                        }
                    case MaterialType.AxisMaterial:
                        {
                            shader.SetInt("sampler", 0); 
                            material.mDiffuse.Bind();

                            shader.SetInt("specularMaskSampler", 1); 
                            if (material.mSpecularMask != null) material.mSpecularMask.Bind();

                            shader.SetMatrix4("transform", mesh.GetAxisModelMatrix()); 
                            shader.SetMatrix4("viewMatrix", camera.GetViewMatrix()); 
                            shader.SetMatrix4("projectionMatrix", projectionMatrix); 

                            Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mesh.GetAxisModelMatrix())));  

                            shader.SetMatrix3("normalMatrix", normalMatrix); 

                            shader.SetVector3("lightColor", light.mLightColor); 
                            shader.SetVector3("lightDirection", light.mDirection); 
                            shader.SetFloat1("specularIntensity", specularIntensity);
                            shader.SetFloat1("shiness", shiness); 
                            shader.SetFloat1("opacity", material.mOpacity); 

                            shader.SetVector3("ambientColor", light.mAmbientColor);
                            shader.SetVector3("cameraPosition", camera._position); 
                            break;
                        }
                    default:
                        break;
                }


                GL.BindVertexArray(geometry.getVao()); 
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.getEbo());
                if (obj.getType() == ObjectType.InstancedMesh)
                {
                    InstancedMesh im = (InstancedMesh)mesh;
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, geometry.getIndicesCount(), DrawElementsType.UnsignedInt, IntPtr.Zero, (int)im.mInstanceCount);

                }
                else GL.DrawElements(PrimitiveType.Triangles, geometry.getIndicesCount(), DrawElementsType.UnsignedInt, 0); ;


            }

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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true); 
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Disable(EnableCap.PolygonOffsetLine);

            GL.Disable(EnableCap.Blend); 

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep,StencilOp.Keep,StencilOp.Keep); 
            GL.StencilMask(0xFF);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit| ClearBufferMask.StencilBufferBit);

            mOpacityObjects.Clear();
            mTransparentObjects.Clear();
            projectObject(scene); 

            for (int i = 0; i < mOpacityObjects.Count; i++)
            {
                renderObject(mOpacityObjects[i], camera, light, projectionMatrix, specularIntensity, shiness);
            }

            for (int i = 0; i < mTransparentObjects.Count; i++)
            {
                renderObject(mTransparentObjects[i], camera, light, projectionMatrix, specularIntensity, shiness);
            }

        }

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
        private void projectObject(Object obj)
        {
            if (obj.getType()==ObjectType.Mesh || obj.getType() == ObjectType.InstancedMesh)
            {
                Mesh mesh = (Mesh)obj;
                Material material = mesh.mMaterial;
                if (material.mBlend) mTransparentObjects.Add(mesh);  
                else mOpacityObjects.Add(mesh);
            }

            var children = obj.getChild();
            for (int i = 0; i < children.Count; i++)
            {
                projectObject(children[i]);
            }
        }
        void setDepthState(Material material)
        {
            if (material.mDepthTest)
            {
                GL.Enable(EnableCap.DepthTest); 
                GL.DepthFunc(material.mDepthFunc); 
            }
            else GL.Disable(EnableCap.DepthTest); 

            if (material.mDepthWrite) GL.DepthMask(true); 
            else GL.DepthMask(false); 
        }
        void setPolygonOffsetState(Material material)
        {
            if (material.mPolygonOffset)
            {
                GL.Enable(material.mPolygonOffsetType);
                GL.PolygonOffset(material.mFactor, material.mUnit);
            }
            else
            {
                GL.Disable(EnableCap.PolygonOffsetFill);
                GL.Disable(EnableCap.PolygonOffsetLine);
            }
        }
        void setStencilState(Material material)
        {
            if (material.mStencilTest)
            {
                GL.Enable(EnableCap.StencilTest);
                GL.StencilOp(material.mSFail, material.mZFail, material.mZPass); 
                GL.StencilMask(material.mStencilMask);
                GL.StencilFunc(material.mStencilFunc,material.mStencilRef,material.mStencilFuncMask);
            }
            else GL.Disable(EnableCap.StencilTest); 
        }
        void setFaceCullingState(Material material)
        {
            if (material.mFaceCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.FrontFace(material.mFrontFace); 
                GL.CullFace(material.mCullFace); 
            }
            else GL.Disable(EnableCap.CullFace);

        }
        void setBlendState(Material material)
        {
            if (material.mBlend)
            {
                GL.Enable(EnableCap.Blend); 
                GL.BlendFunc(material.mSFactor, material.mDFactor);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
            }
           
        }
    }
}
