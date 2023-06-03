﻿using OpenTK.Graphics.OpenGL;

namespace SpaceEngine.RenderEngine
{
    internal class ScreenQuadRenderer
    {
        private FrameBuffer buffer1;
        private FrameBuffer buffer2;
        private bool toggle;
        Model quadModel;
        public ScreenQuadRenderer() {
            float[] positions = { -1, 1, -1, -1, 1, -1, 1, 1 };
            int[] indices = { 0, 1, 2, 3, 0, 2 };
            quadModel = glLoader.loadToVAO(positions, indices);

            FrameBufferSettings frameBufferSettings= new FrameBufferSettings();
            frameBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment0));
            buffer1 = new FrameBuffer(frameBufferSettings);
            buffer2 = new FrameBuffer(frameBufferSettings);
            toggle = true;
        }
        private void renderQuad(int texture)
        {
            GL.BindVertexArray(quadModel.getVAOID());
            GL.EnableVertexAttribArray(0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.DrawElements(PrimitiveType.Triangles, quadModel.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            stepToggle();
        }
        public void renderTextureToScreen(int texture)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            renderQuad(texture);
        }
        public void renderTextureToNextFrameBuffer(int texture)
        {
            getNextFrameBuffer().bind();
            //GL.Viewport(0, 0, resolution.X, resolution.Y);
            renderQuad(texture);
        }
        public int getLastOutputTexture()
        {
            return getLastFrameBuffer().getRenderAttachment(0);
        }
        public FrameBuffer getNextFrameBuffer()
        {
            if (toggle) return buffer1;
            else return buffer2;
        }
        public FrameBuffer getLastFrameBuffer()
        {
            if (toggle) return buffer2;
            else return buffer1;
        }
        private void stepToggle()
        {
            if (toggle == true) toggle = false;
            else toggle = true;
        }
    }
}
