using OpenTK;
using System;


namespace OpenTK_Winform_Robot
{
    class Camera
    {
        //public Vector3 _position = new Vector3 (0.0f,0.0f,10.0f); //【摄像机位置】-初始位置
        public Vector3 _position = new Vector3(10.0f, 0.0f, 5.0f); //【摄像机位置】-初始位置
        public Vector3 _up = Vector3.UnitY;  //【摄像机顶部】
        //public Vector3 _right = Vector3.UnitX; //【摄像机右侧】
        public Vector3 _right = new Vector3(0.0f, 0.0f, -1.0f); //【摄像机右侧】

        public float oLeft = -2.0f;
        public float oRight = 2.0f;
        public float oBottom = -2.0f;
        public float oTop = 2.0f;
        public float oNear = 2.0f;
        public float oFar = -2.0f;

        public float pNear = 0.01f; //【近平面】-距离相机的距离
        public float pFar = 100.0f; //【远平面】-距离相机的距离


        //通过LookAt函数获得【相机变换矩阵】
        public Matrix4 GetViewMatrix()
        {
            //-eye:相机位置（使用mPosition）
            //-center：看向世界坐标的哪个点
            //-top：穹顶（使用mUp替代）
            Vector3 _front = Vector3.Cross(_up,_right);
            Vector3 _center = _position + _front;
            return Matrix4.LookAt(_position, _center, _up);
        }

        //【正交投影矩阵】
        public Matrix4 GetOrthoMatrix()
        {
            return Matrix4.CreateOrthographicOffCenter(oLeft, oRight, oBottom, oTop, oNear, oFar);// 创建正交投影矩阵
        }

        //【透视投影矩阵】
        public Matrix4 GetPerspectiveMatrix(float fov, float aspectRatio, float near, float far)
        {
           return  Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far); // 创建【透视投影矩阵】
        }

    }

}
