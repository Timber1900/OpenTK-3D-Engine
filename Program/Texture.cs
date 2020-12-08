using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Program
{
    /// <summary>
    ///     Class that defines a openTK texture
    /// </summary>
    public class Texture
    {
        /// <summary>
        ///     Handle of the texture
        /// </summary>
        public readonly int Handle;

        /// <summary>
        ///     Creates a openTk texture
        /// </summary>
        /// <param name="path">Path to the .png texture</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        public Texture(string path, TextureMinFilter min, TextureMagFilter mag)
        {
            Handle = GL.GenTexture();
            Use();
            using (var image = new Bitmap(path))
            {
                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) mag);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        /// <summary>
        ///     Creates a openTk texture
        /// </summary>
        /// <param name="text">Texture bitmap</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        public Texture(Image text, TextureMinFilter min, TextureMagFilter mag)
        {
            Handle = GL.GenTexture();
            Use();
            using (var image = new Bitmap(text))
            {
                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) mag);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }


        /// <summary>
        /// </summary>
        /// <param name="unit"></param>
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}