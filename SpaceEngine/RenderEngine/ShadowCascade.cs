using OpenTK.Mathematics;
using System.Net.Http.Headers;

namespace SpaceEngine.RenderEngine
{
    internal class ShadowCascade
    {
        private Matrix4 cascadeProjectionMatrix;
        private FrameBuffer cascadeFrameBuffer;
        private float projectionSize;
        public ShadowCascade(Vector2i resolution, float projectionSize)
        {
            this.projectionSize = projectionSize;
            FrameBufferSettings settings = new FrameBufferSettings(resolution);
            DepthAttachmentSettings depthAttachmentSettings = new DepthAttachmentSettings();
            depthAttachmentSettings.isTexture = true;
            depthAttachmentSettings.isShadowDepthTexture = true;
            settings.depthAttachmentSettings = depthAttachmentSettings;
            cascadeFrameBuffer = new FrameBuffer(settings);

            cascadeProjectionMatrix = Matrix4.CreateOrthographic(projectionSize, projectionSize, -projectionSize, projectionSize);
        }

        public Vector2i getResolution()
        {
            return cascadeFrameBuffer.getResolution();
        }

        public int getDepthTexture()
        {
            return cascadeFrameBuffer.getDepthAttachment();
        }

        public Matrix4 getProjectionMatrix()
        {
            return cascadeProjectionMatrix;
        }

        public void bindFrameBuffer()
        {
            cascadeFrameBuffer.bind();
        }
        public float getProjectionSize()
        {
            return projectionSize;
        }
    }




}
