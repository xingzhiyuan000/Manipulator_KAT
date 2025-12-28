using Assimp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    class AssimpLoader
    {

        public static Object loadModel(string path)
        {
            string rootPath = Path.GetDirectoryName(path);
            string texturesPath = rootPath.Substring(0, rootPath.LastIndexOf("FBX"));

            Object rootNode = new Object();
            var importer = new AssimpContext();
            
            //读取模型文件
            importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            Assimp.Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            // Validate if the scene was read correctly
            if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) != 0 || scene.RootNode == null)
            {
                MessageBox.Show("模型读取失败");
                return null;
            }
            
            processNode(scene.RootNode, rootNode, scene, texturesPath);

            return rootNode;
        }
        /// <summary>
        /// 【处理节点】
        /// </summary>
        /// <param name="ainode"></param>
        /// <param name="parent"></param>
        private static void processNode(Assimp.Node ainode, Object parent, Assimp.Scene scene, string texturesPath)
        {
            Object node = new Object();  //【生成对应Object】
            parent.addChild(node);


            System.Numerics.Matrix4x4 localMatrix = toMatrix4x4(ainode.Transform);
            System.Numerics.Vector3 position, eulerAngle, scale;
            Tools.Decompose(localMatrix, out position, out eulerAngle, out scale);

            //Matrix4 localMatrix = toMatrix4(ainode.Transform);
            //Vector3 position, eulerAngle;
            //Tools.Decompose(localMatrix, out position, out eulerAngle);

            node.SetName(ainode.Name.ToString());
            node.SetPosition(new OpenTK.Vector3(position.X, position.Y, position.Z));
            node.setAngleX(eulerAngle.X);
            node.setAngleY(eulerAngle.Y);
            node.setAngleZ(eulerAngle.Z);
            node.SetScale(new OpenTK.Vector3(scale.X, scale.Y, scale.Z));

            //【检查有没有Mesh，并解析】
            for (int i = 0; i < ainode.MeshCount; i++)
            {
                int meshID = ainode.MeshIndices[i];       //mesh的ID们
                Assimp.Mesh aimesh = scene.Meshes[meshID];

                var mesh = processMesh(aimesh,scene, texturesPath);
                node.addChild(mesh); //【mesh加入对应node节点】
            }

            for (int i = 0; i < ainode.ChildCount; i++)
            {
                    processNode(ainode.Children[i], node, scene, texturesPath);
            }

        }

        private static Meshes.Mesh processMesh(Assimp.Mesh aimesh, Assimp.Scene scene, string texturesPath)
        {
            List<float> positions = new List<float>();
            List<float> normals = new List<float>();
            List<float> uvs = new List<float>();
            List<uint> indices = new List<uint>();

            //1.【解析顶点、Normal、UVs】
            for (int i = 0; i < aimesh.VertexCount; i++)
            {
                // Vertex 顶点
                positions.Add(aimesh.Vertices[i].X);
                positions.Add(aimesh.Vertices[i].Y);
                positions.Add(aimesh.Vertices[i].Z);

                // Normals数据
                if (aimesh.HasNormals)
                {
                    normals.Add(aimesh.Normals[i].X);
                    normals.Add(aimesh.Normals[i].Y);
                    normals.Add(aimesh.Normals[i].Z);
                }

                // UVs数据
                if (aimesh.HasTextureCoords(0))
                {
                    uvs.Add(aimesh.TextureCoordinateChannels[0][i].X);
                    uvs.Add(aimesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    uvs.Add(0.0f);
                    uvs.Add(0.0f);
                }
            }

            //2.【解析indices】
            for (int i = 0; i < aimesh.FaceCount; i++)
            {
                Face face = aimesh.Faces[i];
                foreach (var index in face.Indices)
                {
                    indices.Add((uint)index);
                }
            }

            var geometry = new Geometry(positions, normals,uvs,indices);
            var material = new Material();


            //3.【解析material】
            
            Texture texture = null;
            Assimp.Material aiMat = scene.Materials[aimesh.MaterialIndex];
                
            // 获取纹理路径
            string texturePath = aiMat.TextureDiffuse.FilePath;
            //判断是否有嵌入FBX图片
            if (texturePath!=null)
            {
                //判断是否是嵌入FBX图片
                if (texturePath.StartsWith("*"))
                {
                    // 获取内嵌纹理的索引
                    int textureIndex = int.Parse(texturePath.Substring(1)); //去掉字符串的第一个字符，并返回剩下的部分

                    // 获取内嵌纹理
                    var embeddedTexture = scene.Textures[textureIndex];

                    // 纹理数据通常以二进制数据形式存储
                    byte[] textureData = embeddedTexture.CompressedData;
                    // 获取纹理的格式
                    string textureFormat = embeddedTexture.CompressedFormatHint;

                    // 将内嵌纹理保存为文件（例如PNG）
                    string outputFilePath = texturesPath + $"Textures\\embedded_texture_{textureIndex}.{textureFormat}";
                    File.WriteAllBytes(outputFilePath, textureData); //写入文件目录

                    texture = Texture.CreateTextureFromMemory(outputFilePath, 0, textureData); //从内存读取贴图数据

                }
                else
                {
                    //没有内嵌-【默认贴图】
                    texture = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/defaultTexture.png", 0);
                }
                material.mDiffuse = texture;
            }
            else
            {
                //【默认贴图】
                material.mDiffuse = new Texture(Directory.GetCurrentDirectory() + "/Resources/Textures/defaultTexture.png", 0);
            }

            return new Meshes.Mesh(geometry, material);

        }


        private static System.Numerics.Matrix4x4 toMatrix4x4(Assimp.Matrix4x4 aiMatrix)
        {
            // Convert Assimp Matrix4x4 to System.Numerics.Matrix4x4
            return new System.Numerics.Matrix4x4(
                aiMatrix.A1, aiMatrix.A2, aiMatrix.A3, aiMatrix.A4,
                aiMatrix.B1, aiMatrix.B2, aiMatrix.B3, aiMatrix.B4,
                aiMatrix.C1, aiMatrix.C2, aiMatrix.C3, aiMatrix.C4,
                aiMatrix.D1, aiMatrix.D2, aiMatrix.D3, aiMatrix.D4);
        }

        private static Matrix4 toMatrix4(Assimp.Matrix4x4 aiMatrix)
        {
            // Convert Assimp Matrix4x4 to System.Numerics.Matrix4x4
            return new Matrix4(
                aiMatrix.A1, aiMatrix.A2, aiMatrix.A3, aiMatrix.A4,
                aiMatrix.B1, aiMatrix.B2, aiMatrix.B3, aiMatrix.B4,
                aiMatrix.C1, aiMatrix.C2, aiMatrix.C3, aiMatrix.C4,
                aiMatrix.D1, aiMatrix.D2, aiMatrix.D3, aiMatrix.D4);
        }

    }
}
