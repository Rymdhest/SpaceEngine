using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceEngine.RenderEngine
{
    internal class FrameBuffer
    {
        private int frameBufferID;
        private int depthAttachment = -1;
        private int[] renderAttachments;
        private Vector2i resolution;
        public FrameBuffer(FrameBufferSettings settings)
        {
            this.resolution= settings.resolution;
            frameBufferID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferID);


            DrawBuffersEnum[] buffers = new DrawBuffersEnum[settings.drawBuffers.Count];
            renderAttachments = new int[settings.drawBuffers.Count];
            for (int i = 0; i<settings.drawBuffers.Count; i++)
            {
                renderAttachments[i] = createRenderAttachment(settings.drawBuffers[i], settings.resolution);
                buffers[i] = (DrawBuffersEnum)settings.drawBuffers[i].colorAttachment;
            }
            GL.DrawBuffers(buffers.Length, buffers);

            if (settings.depthAttachmentSettings != null)
            {
                depthAttachment = createDepthAttachment(settings.depthAttachmentSettings, settings.resolution);
            }
        }
        public void resolveToScreen()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBufferID);
            GL.DrawBuffer(DrawBufferMode.Back);
            GL.BlitFramebuffer(0, 0, resolution.X, resolution.Y, 0, 0, WindowHandler.resolution.X, WindowHandler.resolution.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            unbind();
        }

        public void blitDepthBufferFrom(FrameBuffer other)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, other.frameBufferID);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, frameBufferID);
            GL.BlitFramebuffer(0, 0, other.resolution.X, other.resolution.Y, 0, 0, this.resolution.X, this.resolution.Y, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            unbind();
        }

        public void bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferID);
            GL.Viewport(0, 0, resolution.X, resolution.Y);
        }
        public int getDepthAttachment()
        {
            return depthAttachment;
        }
        public void unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, WindowHandler.resolution.X, WindowHandler.resolution.Y);
        }
        private int createRenderAttachment(DrawBufferSettings renderSettings, Vector2i resolution)
        {
            int attachment = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, attachment);
            GL.TexImage2D(TextureTarget.Texture2D, 0, renderSettings.formatInternal, resolution.X, resolution.Y, 0, renderSettings.formatExternal, renderSettings.pixelType, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);    
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, renderSettings.colorAttachment, TextureTarget.Texture2D, attachment, 0);
            return attachment;
        }
            private int createDepthAttachment(DepthAttachmentSettings depthSettings, Vector2i resolution)
        {
            int attachment;
            if (depthSettings.isTexture)
            {
                attachment = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, attachment);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, resolution.X, resolution.Y, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode , (float)TextureCompareMode.None);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, attachment, 0);

            } else
            {
                attachment = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, attachment);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, resolution.X, resolution.Y);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, attachment);

            }
            return attachment;
        }
        public int getRenderAttachment(int attachmentNumber)
        {
            return renderAttachments[attachmentNumber];
        }
    }
}
