using OpenTK;

namespace OpenTK_Winform_Robot
{
    class Light
    {
        public Vector3 mLightColor = new Vector3(1.0f,1.0f,1.0f);  //平行光光照强度-外部可设置
        public float mSpecularIntensity = 0.6f;  //高光强度

        public Vector3 mDirection = new Vector3(-1.0f, 1.0f, 1.0f);  //光照方向;-外部可设置
        public Vector3 mAmbientColor = new Vector3(0.6f, 0.6f, 0.6f);  //环境光强度;

        /// <summary>
        /// 平行光
        /// </summary>
        public void setDirectionalLight(Vector3 lightColor,Vector3 direction)
        {
            mLightColor = lightColor;
            mDirection = direction;
        }

        /// <summary>
        /// 环境光  
        /// </summary>
        public void AmbientLight()
        { 
        
        }

    }
}
