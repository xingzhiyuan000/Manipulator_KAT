using OpenTK.Graphics.OpenGL;

namespace OpenTK_Winform_Robot
{
    enum MaterialType
    { 
        PhongMaterial,
        WhiteMaterial,
        DepthMaterial,
        CubeMaterial,  //天空盒材质
        PhongEnvMaterial,
        PhongInstanceMaterial,   //实例绘制材质
        SphereMaterial,
        AxisMaterial, //固定坐标轴
    }
    class Material
    {

        public Texture mDiffuse=null;  //基础颜色
        public Texture mSpecularMask = null;  //蒙版
        public float mShiness = 1.0f; //高光自身强度
        public Texture mEnv = null;
        public MaterialType mType;

        public bool mDepthTest = true; //【深度检测】
        public DepthFunction mDepthFunc = DepthFunction.Lequal; //深度检测方式
        public bool mDepthWrite=true;  //【深度写入】

        public bool mPolygonOffset = false; //PolygonOffset-【Zfighting】
        public EnableCap mPolygonOffsetType = EnableCap.PolygonOffsetFill;
        public float mFactor = 0.0f;
        public float mUnit = 0.0f;

        public bool mFaceCulling = true;//【面剔除】
        public FrontFaceDirection mFrontFace = FrontFaceDirection.Ccw; //【逆时针朝前】
        public CullFaceMode mCullFace = CullFaceMode.Back; //【剔除背面】

        public bool mBlend = false; //【颜色混合】
        public BlendingFactor mSFactor = BlendingFactor.SrcAlpha; //【颜色混合方式】
        public BlendingFactor mDFactor = BlendingFactor.OneMinusSrcAlpha; //【颜色混合方式】
        public float mOpacity = 1.0f; //【总体透明度】

        public bool mStencilTest = false; //【模版测试】-开启状态
        public StencilOp mSFail = StencilOp.Keep; //模版测试失败了怎么办
        public StencilOp mZFail = StencilOp.Keep; //模版测试通过但是深度检测没通过怎么办
        public StencilOp mZPass = StencilOp.Keep; //模版+深度测试都通过了怎么办
        public int mStencilMask = 0xFF; //用于控制【模版写入】
        public StencilFunction mStencilFunc = StencilFunction.Always; //【如何测试】
        public int mStencilRef = 0;
        public int mStencilFuncMask = 0xFF;

    }
}
