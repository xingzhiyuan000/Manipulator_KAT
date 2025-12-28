using OpenTK;

namespace OpenTK_Winform_Robot.Meshes
{
    class InstancedMesh:Mesh
    {
        public int mInstanceCount;  //【实例数目】
        public Matrix4[] mInstanceMatrices=null;  //【实例矩阵】
        
        public InstancedMesh(
            Geometry geometry,
            Material material, 
            int instanceCount):base(geometry,material)
        {
            mType = ObjectType.InstancedMesh;
            mInstanceCount = instanceCount;

            mInstanceMatrices = new Matrix4[instanceCount];
        }

        ~InstancedMesh()
        {
        }
    }
}
