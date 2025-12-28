using OpenTK;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    /// <summary>
    /// Object类型：空物体/Mesh
    /// </summary>
    public enum ObjectType
    {
        Object,
        Mesh,
        InstancedMesh,
        Scene
    }

    public class Object
    {
        private Vector3 mPosition;
        private Vector3 mScale = Vector3.One;
        private float mAngleX, mAngleY, mAngleZ;
        private string mName;

        public List<Object> mChildren = new List<Object>(); // 子对象列表
        public Object mParent = null; // 父对象
         
        public ObjectType mType ;  //Object类型记录

        private Matrix4 translationToOrigin = Matrix4.Identity;
        private Matrix4 translationBack = Matrix4.Identity;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Object()
        {
            mType = ObjectType.Object;
        }

        public ObjectType getType()
        {
            return mType;
        }

        public Vector3 getPosition()
        {
            return mPosition;
        }

        /// <summary>
        /// 【移除孩子】-只能删除直系孩子
        /// </summary>
        public void removeChild(Object obj)
        {
            // 1. 检查是否已经加入过这个孩子
            if (mChildren.Contains(obj))
            {
                //foreach (var child in mChildren.ToList()) // 避免修改集合时遍历错误
                //{
                //    parentObj.removeChild(child);
                //}
                // 2. 删除孩子
                mChildren.Remove(obj);
            }
            // 3. 告诉新加入的孩子他的父对象是谁
            //obj.mParent = null;
        }

        public void addChild(Object obj)
        {
            // 1. 检查是否已经加入过这个孩子
            if (mChildren.Contains(obj))
            {
                MessageBox.Show("该模型已经添加过！");
                return;
            }

            // 2. 加入孩子
            mChildren.Add(obj);
            // 3. 告诉新加入的孩子他的父对象是谁
            obj.mParent = this;
        }

        public List<Object> getChild()
        {
            return mChildren;
        }

        public Object getParent()
        {
            return mParent;
        }

        public void SetName(String name)
        {
            mName = name;
        }

        public string GetName()
        {
            return mName;
        }

        public void SetPosition(Vector3 pos)
        {
            mPosition = pos;
        }

        public void setAngleX(float angle)
        {
            mAngleX = angle;
        }
        public void setAngleY(float angle)
        {
            mAngleY = angle;
        }
        public void setAngleZ(float angle)
        {
            mAngleZ = angle;
            
        }

        public void RotateX(float angle)
        {
            mAngleX += angle;
        }

        public void RotateY(float angle)
        {
            mAngleY += angle;
            
        }

        public void RotateZ(float angle)
        {
            mAngleZ += angle;
           
        }
        /// ↓↓↓↓↓↓↓↓↓【控制相关】↓↓↓↓↓↓↓↓↓
        /// <summary>
        /// 【2-小臂回旋】
        /// </summary>
        public void SetAngle2(float angle)
        {
            mAngleZ = angle;
            Vector3 jointPosition = new Vector3(0.0f, -14.788f, 0.0f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
          
        }
        /// <summary>
        /// 【3-小臂俯仰】
        /// </summary>
        public void SetAngle3(float angle)
        {
            mAngleX = angle;

            Vector3 jointPosition = new Vector3(0.0f, -14.748f, 0.2305f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        /// <summary>
        /// 【腕部平摆】
        /// </summary>
        public void SetAngle4(float angle)
        {
            mAngleZ = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.029f,0.0485f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        /// <summary>
        /// 【腕部俯仰】
        /// </summary>
        public void SetAngle5(float angle)
        {
            mAngleX = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.309f, 0.0485f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        /// <summary>
        /// 【腕部滚摆】
        /// </summary>
        public void SetAngle6(float angle)
        {
            mAngleY = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.057f, 0.118614f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        /// ↑↑↑↑↑↑↑↑↑↑↑【控制相关】↑↑↑↑↑↑↑↑↑↑↑
        public void SetScale(Vector3 scale)
        {
            mScale = scale;
        }

        public Matrix4 GetModelMatrix()
        {

            Matrix4 paretModelMatrix = Matrix4.Identity;
            if (mParent!=null) paretModelMatrix = mParent.GetModelMatrix();

            //缩放→旋转→平移
            Matrix4 transform = Matrix4.Identity;

            Matrix4 scaleMat = Matrix4.CreateScale(mScale);
            // 定义旋转矩阵（围绕XYZ轴）

            //Matrix4 rotateMatX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(mAngleX));
            //Matrix4 rotateMatY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(mAngleY));
            //Matrix4 rotateMatZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(mAngleZ));

            Matrix4 rotateMatX = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f), MathHelper.DegreesToRadians(mAngleX));//【弧度】
            Matrix4 rotateMatY = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), MathHelper.DegreesToRadians(mAngleY));
            Matrix4 rotateMatZ = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), MathHelper.DegreesToRadians(mAngleZ));
            // 定义平移矩阵
            Matrix4 translateMat = Matrix4.CreateTranslation(mPosition);
            //transform = rotateMatZ * rotateMatY * rotateMatX *translateMat * scaleMat * paretModelMatrix* transform;
            transform = translationBack * rotateMatZ * rotateMatY * rotateMatX * translationToOrigin * translateMat * scaleMat * paretModelMatrix * transform;
            //transform = paretModelMatrix * scaleMat * translationBack * translateMat * rotateMatZ * rotateMatY * rotateMatX * translationToOrigin * transform;

            return transform;
        }

        public Matrix4 GetAxisModelMatrix()
        {
            Matrix4 transform = Matrix4.Identity;

            Matrix4 translateMat = Matrix4.CreateTranslation(mPosition);
            Matrix4 rotateMatX = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f), MathHelper.DegreesToRadians(mAngleX));//【弧度】
            Matrix4 rotateMatY = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), MathHelper.DegreesToRadians(mAngleY));
            Matrix4 rotateMatZ = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), MathHelper.DegreesToRadians(mAngleZ));

            transform = rotateMatZ * rotateMatY * rotateMatX * translateMat* transform;

            return transform;
        }
    }
}
