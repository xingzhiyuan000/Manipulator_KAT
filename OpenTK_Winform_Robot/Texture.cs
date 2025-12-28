using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    class Texture
    {
        private int mHandle=0;
        private int mUnit=0;
        private TextureTarget mTextureTarget=TextureTarget.Texture2D;

        // 静态字段，存储【纹理缓存】
        public static Dictionary<string, Texture> mTextureCache = new Dictionary<string, Texture>();

        /// <summary>
        /// 【纹理缓存】方法-【从硬盘】
        /// </summary>
        public static Texture CreateTexture(string path, int unit)
        {
            // 1. 检查缓存中是否已有此路径对应的纹理对象
            if (mTextureCache.TryGetValue(path, out Texture cachedTexture))
            {
                return cachedTexture;
            }

            // 2. 如果缓存中没有对应的纹理对象，则创建新的纹理对象
            Texture texture = new Texture(path, unit);
            mTextureCache[path] = texture;

            return texture;
        }

        /// <summary>
        /// 【纹理缓存】方法-【从内存】
        /// </summary>
        public static Texture CreateTextureFromMemory(
            string path,
            int unit,
            byte[] dataIn)
        {
            // 1. 检查缓存中是否已有此路径对应的纹理对象
            if (mTextureCache.TryGetValue(path, out Texture cachedTexture))
            {
                return cachedTexture;
            }

            // 2. 如果缓存中没有对应的纹理对象，则创建新的纹理对象
            Texture texture = new Texture(unit, dataIn);
            mTextureCache[path] = texture;
            
            return texture;
        }

        /// <summary>
        /// 从【硬盘】读取贴图
        /// </summary>
        public Texture (string path, int unit)
        {

            // Generate handle 【生成纹理索引】
            mHandle = GL.GenTexture();
            mUnit = unit;
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); //【激活纹理单元：0-15】
            GL.BindTexture(TextureTarget.Texture2D, mHandle); //【绑定纹理单元】

            StbImage.stbi_set_flip_vertically_on_load(1);  //像素起点翻转

            using (Stream stream = File.OpenRead(path))
            {
                //【读取图片】
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                //【向GPU中注入数据，并开辟显存】
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            //【纹理过滤】
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            //【纹理包裹】
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); //u
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); //v

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        }

        /// <summary>
        /// 从【内存】读取贴图
        /// </summary>
        public Texture(int unit, byte[] dataIn)
        {
            mUnit = unit;
            // Generate handle 【生成纹理索引】
            mHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); //【激活纹理单元：0-15】
            GL.BindTexture(TextureTarget.Texture2D, mHandle); //【绑定纹理单元】

            StbImage.stbi_set_flip_vertically_on_load(1);  //像素起点翻转

           
            // 计算图片大小
            //uint dataInSize = (heightIn == 0) ? widthIn : widthIn * heightIn * 4; // 假设RGBA

            //【读取图片】-从内存
            ImageResult image = ImageResult.FromMemory(dataIn, ColorComponents.RedGreenBlueAlpha);
 

            //【向GPU中注入数据，并开辟显存】
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            //【纹理过滤】
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //【纹理包裹】
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); //u
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); //v

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        /// <summary>
        /// 【天空盒】
        /// </summary>
        //paths:右左上下后前(+x -x +y -y +z -z)
        public Texture(List<string> paths, int unit)
        {
            mTextureTarget = TextureTarget.TextureCubeMap;
            mUnit = unit;
            StbImage.stbi_set_flip_vertically_on_load(0);  //像素起点不翻转

            //1 创建CubeMap对象
            mHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); //【激活纹理单元：0-15】
            GL.BindTexture(mTextureTarget, mHandle); //【绑定纹理单元】

            //2 循环读取六张贴图，并且放置到cubemap的六个GPU空间内
            for (int i = 0; i < paths.Count; i++)
            {
                using (Stream stream = File.OpenRead(paths[i]))
                {
                    //【读取图片】
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    if (image != null)
                    {
                        //【向GPU中注入数据，并开辟显存】
                        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX+i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                    }
                    else MessageBox.Show(paths[i].ToString() + "天空盒图片读取失败");
                }
            }

            //3 设置纹理参数
            //【纹理过滤】
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            //【纹理包裹】
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); //u
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); //v

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }

        public void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); //切换纹理单元
            GL.BindTexture(mTextureTarget, mHandle); //绑定纹理对象
        }
        public void setUnit(int unit)
        {
            mUnit = unit;
        }

        //~Texture()
        //{
        //    if (mHandle != 0)
        //    {
        //        GL.DeleteTexture(mHandle); //释放纹理
        //    }
        //}
    }
}
