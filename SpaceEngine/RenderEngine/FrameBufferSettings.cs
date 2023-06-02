
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;

namespace SpaceEngine.RenderEngine
{
    internal class FrameBufferSettings
    {
        public DepthAttachmentSettings? depthAttachmentSettings = null;
        public List<DrawBufferSettings> drawBuffers= new List<DrawBufferSettings>();
        public Vector2i resolution;
        public FrameBufferSettings() {
            this.resolution = new Vector2i(WindowHandler.resolution.X, WindowHandler.resolution.Y);
        }
    }
}
