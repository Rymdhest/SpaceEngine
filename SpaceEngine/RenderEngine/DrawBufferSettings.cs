using OpenTK.Graphics.OpenGL;
namespace SpaceEngine.RenderEngine
{
    internal class DrawBufferSettings
    {
        public PixelInternalFormat formatInternal = PixelInternalFormat.Rgba16f;
        public PixelFormat formatExternal = PixelFormat.Rgba;
        public PixelType pixelType = PixelType.UnsignedByte;
        public FramebufferAttachment colorAttachment;
        public DrawBufferSettings(FramebufferAttachment colorAttachment) {
            this.colorAttachment = colorAttachment;
        }
    }
}
