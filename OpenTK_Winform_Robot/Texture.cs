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

        public static Dictionary<string, Texture> mTextureCache = new Dictionary<string, Texture>();

        public static Texture CreateTexture(string path, int unit)
        {
            if (mTextureCache.TryGetValue(path, out Texture cachedTexture))
            {
                return cachedTexture;
            }

            Texture texture = new Texture(path, unit);
            mTextureCache[path] = texture;

            return texture;
        }

        public static Texture CreateTextureFromMemory(
            string path,
            int unit,
            byte[] dataIn)
        {
            if (mTextureCache.TryGetValue(path, out Texture cachedTexture))
            {
                return cachedTexture;
            }

            Texture texture = new Texture(unit, dataIn);
            mTextureCache[path] = texture;
            
            return texture;
        }

        public Texture (string path, int unit)
        {

            mHandle = GL.GenTexture();
            mUnit = unit;
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); 
            GL.BindTexture(TextureTarget.Texture2D, mHandle); 

            StbImage.stbi_set_flip_vertically_on_load(1);  

            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); 

        }

        public Texture(int unit, byte[] dataIn)
        {
            mUnit = unit;
            mHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); 
            GL.BindTexture(TextureTarget.Texture2D, mHandle); 

            StbImage.stbi_set_flip_vertically_on_load(1);  

           
            ImageResult image = ImageResult.FromMemory(dataIn, ColorComponents.RedGreenBlueAlpha);
 

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); 

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public Texture(List<string> paths, int unit)
        {
            mTextureTarget = TextureTarget.TextureCubeMap;
            mUnit = unit;
            StbImage.stbi_set_flip_vertically_on_load(0);  

            mHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); 
            GL.BindTexture(mTextureTarget, mHandle); 

            for (int i = 0; i < paths.Count; i++)
            {
                using (Stream stream = File.OpenRead(paths[i]))
                {
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    if (image != null)
                    {
                        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX+i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                    }
                    else MessageBox.Show(paths[i].ToString() + "Failed to load skybox image");
                }
            }

            GL.TexParameter(mTextureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(mTextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); 
            GL.TexParameter(mTextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); 

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }

        public void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + mUnit); 
            GL.BindTexture(mTextureTarget, mHandle); 
        }
        public void setUnit(int unit)
        {
            mUnit = unit;
        }
    }
}
