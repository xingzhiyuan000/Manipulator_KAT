using OpenTK;
using OpenTK_Winform_Robot.Meshes;
namespace OpenTK_Winform_Robot
{
    class Joint
    {
        private Object mJoint;
        public Vector3[] constraint = new Vector3[2]; 
        public float[] constraintTrans = new float[2]; 
        private float mAngleX, mAngleY, mAngleZ, mTranslation;

        public Joint constraints(float nx, float mx, float ny, float my, float nz, float mz, float ntrans, float mtrans)
        {
            constraint[0] = new Vector3(nx, ny, nz);
            constraint[1] = new Vector3(mx, my, mz);
            constraintTrans[0] = ntrans;
            constraintTrans[1] = mtrans;

            return this;
        }

        public Joint(Object mesh)
        {
            mJoint = mesh;
        }
        public Object getMesh()
        {
            return mJoint;
        }
        public float getTranslation()
        {
            return mTranslation;
        }
        public float getAngleX()
        {
            return mAngleX;
        }
        public float getAngleY()
        {
            return mAngleY;
        }
        public float getAngleZ()
        {
            return mAngleZ;
        }
        public void setAngle(float x, float y, float z)
        {
            float lx, ly, lz;
            bool success = checkConstraints(lx = x % 360.0f, ly = y % 360.0f, lz = z % 360.0f);

            if (success)
            {
                mAngleX = lx;
                mAngleY = ly;
                mAngleZ = lz;
            }
        }
        public void translate(float v)
        {
            bool success = checkConstraintsTrans(v);

            if (success)
                mTranslation = v;
        }
        public bool checkConstraints(float x, float y, float z)
        {
            if (x < constraint[0].X || x > constraint[1].X ||
                y < constraint[0].Y || y > constraint[1].Y ||
                z < constraint[0].Z || z > constraint[1].Z)
            {
                return false;
            }
            return true;
        }
        public bool checkConstraintsTrans(float tran)
        {
            if (tran > constraintTrans[1] || tran < constraintTrans[0])
            {
                return false;
            }
            return true;
        }

    }
}
