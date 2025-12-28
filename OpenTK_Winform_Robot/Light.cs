using OpenTK;

namespace OpenTK_Winform_Robot
{
    class Light
    {
        public Vector3 mLightColor = new Vector3(1.0f,1.0f,1.0f);  
        public float mSpecularIntensity = 0.6f;  

        public Vector3 mDirection = new Vector3(-1.0f, 1.0f, 1.0f);  
        public Vector3 mAmbientColor = new Vector3(0.6f, 0.6f, 0.6f);  

        public void setDirectionalLight(Vector3 lightColor,Vector3 direction)
        {
            mLightColor = lightColor;
            mDirection = direction;
        }

        public void AmbientLight()
        { 
        
        }

    }
}
