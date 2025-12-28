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
            
            importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            Assimp.Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) != 0 || scene.RootNode == null)
            {
                MessageBox.Show("模型读取失败");
                return null;
            }
            
            processNode(scene.RootNode, rootNode, scene, texturesPath);

            return rootNode;
        }
        private static void processNode(Assimp.Node ainode, Object parent, Assimp.Scene scene, string texturesPath)
        {
            Object node = new Object();  
            parent.addChild(node);


            System.Numerics.Matrix4x4 localMatrix = toMatrix4x4(ainode.Transform);
            System.Numerics.Vector3 position, eulerAngle, scale;
            Tools.Decompose(localMatrix, out position, out eulerAngle, out scale);

            node.SetName(ainode.Name.ToString());
            node.SetPosition(new OpenTK.Vector3(position.X, position.Y, position.Z));
            node.setAngleX(eulerAngle.X);
            node.setAngleY(eulerAngle.Y);
            node.setAngleZ(eulerAngle.Z);
            node.SetScale(new OpenTK.Vector3(scale.X, scale.Y, scale.Z));

            for (int i = 0; i < ainode.MeshCount; i++)
            {
                int meshID = ainode.MeshIndices[i];       
                Assimp.Mesh aimesh = scene.Meshes[meshID];

                var mesh = processMesh(aimesh,scene, texturesPath);
                node.addChild(mesh); 
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

            for (int i = 0; i < aimesh.VertexCount; i++)
            {
                positions.Add(aimesh.Vertices[i].X);
                positions.Add(aimesh.Vertices[i].Y);
                positions.Add(aimesh.Vertices[i].Z);

                if (aimesh.HasNormals)
                {
                    normals.Add(aimesh.Normals[i].X);
                    normals.Add(aimesh.Normals[i].Y);
                    normals.Add(aimesh.Normals[i].Z);
                }

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


            Texture texture = null;
            Assimp.Material aiMat = scene.Materials[aimesh.MaterialIndex];
                
            string texturePath = aiMat.TextureDiffuse.FilePath;

            string exeDir = Application.StartupPath;
            string projectRoot = Directory.GetParent(exeDir).Parent.FullName;
            if (texturePath!=null)
            {
                if (texturePath.StartsWith("*"))
                {
                    int textureIndex = int.Parse(texturePath.Substring(1)); 

                    var embeddedTexture = scene.Textures[textureIndex];

                    byte[] textureData = embeddedTexture.CompressedData;
                    string textureFormat = embeddedTexture.CompressedFormatHint;

                    string outputFilePath = texturesPath + $"Textures\\embedded_texture_{textureIndex}.{textureFormat}";
                    File.WriteAllBytes(outputFilePath, textureData); 

                    texture = Texture.CreateTextureFromMemory(outputFilePath, 0, textureData); 

                }
                else
                {
                    texture = new Texture(projectRoot + "/Resources/Textures/defaultTexture.png", 0);
                }
                material.mDiffuse = texture;
            }
            else
            {
                material.mDiffuse = new Texture(projectRoot + "/Resources/Textures/defaultTexture.png", 0);
            }

            return new Meshes.Mesh(geometry, material);

        }


        private static System.Numerics.Matrix4x4 toMatrix4x4(Assimp.Matrix4x4 aiMatrix)
        {
            return new System.Numerics.Matrix4x4(
                aiMatrix.A1, aiMatrix.A2, aiMatrix.A3, aiMatrix.A4,
                aiMatrix.B1, aiMatrix.B2, aiMatrix.B3, aiMatrix.B4,
                aiMatrix.C1, aiMatrix.C2, aiMatrix.C3, aiMatrix.C4,
                aiMatrix.D1, aiMatrix.D2, aiMatrix.D3, aiMatrix.D4);
        }

        private static Matrix4 toMatrix4(Assimp.Matrix4x4 aiMatrix)
        {
            return new Matrix4(
                aiMatrix.A1, aiMatrix.A2, aiMatrix.A3, aiMatrix.A4,
                aiMatrix.B1, aiMatrix.B2, aiMatrix.B3, aiMatrix.B4,
                aiMatrix.C1, aiMatrix.C2, aiMatrix.C3, aiMatrix.C4,
                aiMatrix.D1, aiMatrix.D2, aiMatrix.D3, aiMatrix.D4);
        }

    }
}
