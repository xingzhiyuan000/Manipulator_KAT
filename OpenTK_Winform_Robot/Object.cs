using OpenTK;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
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

        public List<Object> mChildren = new List<Object>();  
        public Object mParent = null;  
         
        public ObjectType mType ;  

        private Matrix4 translationToOrigin = Matrix4.Identity;
        private Matrix4 translationBack = Matrix4.Identity;

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

        public void removeChild(Object obj)
        {
            if (mChildren.Contains(obj))
            {
                mChildren.Remove(obj);
            }
        }

        public void addChild(Object obj)
        {
            if (mChildren.Contains(obj))
            {
                return;
            }

            mChildren.Add(obj);
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
        public void SetAngle2(float angle)
        {
            mAngleZ = angle;
            Vector3 jointPosition = new Vector3(0.0f, -14.788f, 0.0f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
          
        }
        public void SetAngle3(float angle)
        {
            mAngleX = angle;

            Vector3 jointPosition = new Vector3(0.0f, -14.748f, 0.2305f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        public void SetAngle4(float angle)
        {
            mAngleZ = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.029f,0.0485f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        public void SetAngle5(float angle)
        {
            mAngleX = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.309f, 0.0485f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        public void SetAngle6(float angle)
        {
            mAngleY = angle;

            Vector3 jointPosition = new Vector3(0.0f, -20.057f, 0.118614f);
            translationBack = Matrix4.CreateTranslation(-jointPosition);
            translationToOrigin = Matrix4.CreateTranslation(jointPosition);
        }
        public void SetScale(Vector3 scale)
        {
            mScale = scale;
        }

        public Matrix4 GetModelMatrix()
        {

            Matrix4 paretModelMatrix = Matrix4.Identity;
            if (mParent!=null) paretModelMatrix = mParent.GetModelMatrix();

            Matrix4 transform = Matrix4.Identity;

            Matrix4 scaleMat = Matrix4.CreateScale(mScale);
            Matrix4 rotateMatX = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f), MathHelper.DegreesToRadians(mAngleX));
            Matrix4 rotateMatY = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), MathHelper.DegreesToRadians(mAngleY));
            Matrix4 rotateMatZ = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), MathHelper.DegreesToRadians(mAngleZ));
            Matrix4 translateMat = Matrix4.CreateTranslation(mPosition);
            transform = translationBack * rotateMatZ * rotateMatY * rotateMatX * translationToOrigin * translateMat * scaleMat * paretModelMatrix * transform;
            return transform;
        }

        public Matrix4 GetAxisModelMatrix()
        {
            Matrix4 transform = Matrix4.Identity;

            Matrix4 translateMat = Matrix4.CreateTranslation(mPosition);
            Matrix4 rotateMatX = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f), MathHelper.DegreesToRadians(mAngleX));
            Matrix4 rotateMatY = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), MathHelper.DegreesToRadians(mAngleY));
            Matrix4 rotateMatZ = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), MathHelper.DegreesToRadians(mAngleZ));

            transform = rotateMatZ * rotateMatY * rotateMatX * translateMat* transform;

            return transform;
        }
    }
}
