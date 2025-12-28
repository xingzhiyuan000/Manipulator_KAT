
namespace OpenTK_Winform_Robot.Meshes
{
    class Mesh : Object
    {
        //几何类
        public Geometry mGeometry;
        //材料类
        public Material mMaterial;

        /// <summary>
        /// 【构造函数】
        /// </summary>
        public Mesh (Geometry geometry, Material material)
        {
            mGeometry = geometry;
            mMaterial = material;
            mType = ObjectType.Mesh; //类型划分
        }

    }
}
