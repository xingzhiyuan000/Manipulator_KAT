using OpenTK.Graphics.OpenGL;

namespace OpenTK_Winform_Robot
{
    enum MaterialType
    { 
        PhongMaterial,
        WhiteMaterial,
        DepthMaterial,
        CubeMaterial,  
        PhongEnvMaterial,
        PhongInstanceMaterial,   
        SphereMaterial,
        AxisMaterial, 
    }
    class Material
    {

        public Texture mDiffuse=null;  
        public Texture mSpecularMask = null;  
        public float mShiness = 1.0f; 
        public Texture mEnv = null;
        public MaterialType mType;

        public bool mDepthTest = true; 
        public DepthFunction mDepthFunc = DepthFunction.Lequal; 
        public bool mDepthWrite=true;  

        public bool mPolygonOffset = false; 
        public EnableCap mPolygonOffsetType = EnableCap.PolygonOffsetFill;
        public float mFactor = 0.0f;
        public float mUnit = 0.0f;

        public bool mFaceCulling = true;
        public FrontFaceDirection mFrontFace = FrontFaceDirection.Ccw; 
        public CullFaceMode mCullFace = CullFaceMode.Back; 

        public bool mBlend = false; 
        public BlendingFactor mSFactor = BlendingFactor.SrcAlpha; 
        public BlendingFactor mDFactor = BlendingFactor.OneMinusSrcAlpha; 
        public float mOpacity = 1.0f; 

        public bool mStencilTest = false; 
        public StencilOp mSFail = StencilOp.Keep; 
        public StencilOp mZFail = StencilOp.Keep; 
        public StencilOp mZPass = StencilOp.Keep; 
        public int mStencilMask = 0xFF; 
        public StencilFunction mStencilFunc = StencilFunction.Always; 
        public int mStencilRef = 0;
        public int mStencilFuncMask = 0xFF;

    }
}
