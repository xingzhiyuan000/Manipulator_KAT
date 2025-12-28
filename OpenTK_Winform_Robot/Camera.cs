using OpenTK;
using System;


namespace OpenTK_Winform_Robot
{
    class Camera
    {
        public Vector3 _position = new Vector3(10.0f, 0.0f, 5.0f); 
        public Vector3 _up = Vector3.UnitY;  
        public Vector3 _right = new Vector3(0.0f, 0.0f, -1.0f); 

        public float oLeft = -2.0f;
        public float oRight = 2.0f;
        public float oBottom = -2.0f;
        public float oTop = 2.0f;
        public float oNear = 2.0f;
        public float oFar = -2.0f;

        public float pNear = 0.01f; 
        public float pFar = 100.0f; 


        public Matrix4 GetViewMatrix()
        {
            Vector3 _front = Vector3.Cross(_up,_right);
            Vector3 _center = _position + _front;
            return Matrix4.LookAt(_position, _center, _up);
        }

        public Matrix4 GetOrthoMatrix()
        {
            return Matrix4.CreateOrthographicOffCenter(oLeft, oRight, oBottom, oTop, oNear, oFar); 
        }

        public Matrix4 GetPerspectiveMatrix(float fov, float aspectRatio, float near, float far)
        {
           return  Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);  
        }

    }

}
