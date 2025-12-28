using OpenTK;
using OpenTK_Winform_Robot.Meshes;
namespace OpenTK_Winform_Robot
{
    class Joint
    {
        private Object mJoint;
        public Vector3[] constraint = new Vector3[2]; //【角度限制】
        public float[] constraintTrans = new float[2]; //【位移限制】
        private float mAngleX, mAngleY, mAngleZ, mTranslation;

        /// <summary>
        /// 【设置关节限制】
        /// </summary>
        /// <returns></returns>
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
        /**
        * Set absolute angle, constaints are check, overflow over -360/360 degrees
        * are handled.
        *【设置旋转角度】
        * @throw ConstraintException if any angle is outside its constriant
        * @see Bone::checkConstraints()
        * @return this
        */
        public void setAngle(float x, float y, float z)
        {
            float lx, ly, lz;
            //取模，将角度限制在0-360
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
        /**
       * Check if given angles are inside the contraints.
       *【检查是否位于限制角度内】
       * @throw ConstraintException if any angle is outside its constraint
       */
        public bool checkConstraints(float x, float y, float z)
        {
            if (x < constraint[0].X || x > constraint[1].X ||
                y < constraint[0].Y || y > constraint[1].Y ||
                z < constraint[0].Z || z > constraint[1].Z)
            {
                //printf("%f, %f, %f\n(%f,%f), (%f,%f), (%f,%f)\n", x, y, z, constraint[0].x, constraint[1].x, constraint[0].y, constraint[1].y, constraint[0].z, constraint[1].z);
                //throw new ConstraintException();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 【检查是否在位置区间内】
        /// </summary>
        /// <param name="tran"></param>
        public bool checkConstraintsTrans(float tran)
        {
            if (tran > constraintTrans[1] || tran < constraintTrans[0])
            {
                //printf("%f, %f, %f\n(%f,%f), (%f,%f), (%f,%f)\n", x, y, z, constraint[0].x, constraint[1].x, constraint[0].y, constraint[1].y, constraint[0].z, constraint[1].z);
                //throw new ConstraintException();
                return false;
            }
            return true;
        }

    }
}
