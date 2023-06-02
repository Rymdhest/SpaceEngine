using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceEngine.RenderEngine
{
    internal class DrawBufferSettings
    {
        public PixelInternalFormat formatInternal = PixelInternalFormat.Rgba;
        public PixelFormat formatExternal = PixelFormat.Rgba;
        public PixelType pixelType = PixelType.Float;
        public FramebufferAttachment colorAttachment;
        public DrawBufferSettings(FramebufferAttachment colorAttachment) {
            this.colorAttachment = colorAttachment;
        }
    }
}
