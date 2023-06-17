using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceEngine.RenderEngine
{
    internal class BloomRenderer
    {
        private ShaderProgram downsamplingShader = new ShaderProgram("Simple_Vertex", "Downsampling_Fragment");
        private ShaderProgram upsamplingShader = new ShaderProgram("Simple_Vertex", "Upsampling_Fragment");
        private ShaderProgram bloomFilterShader = new ShaderProgram("Simple_Vertex", "bloom_Filter_Fragment");
        private static readonly int downSamples = 7;
        private FrameBuffer[] sampleFramebuffers = new FrameBuffer[downSamples];

        public BloomRenderer()
        {
            bloomFilterShader.bind();
            bloomFilterShader.loadUniformInt("gDiffuse", 0);
            bloomFilterShader.loadUniformInt("gPosition", 1);
            bloomFilterShader.unBind();

            downsamplingShader.bind();
            downsamplingShader.loadUniformInt("srcTexture", 0);
            downsamplingShader.unBind();

            upsamplingShader.bind();
            upsamplingShader.loadUniformInt("srcTexture", 0);
            upsamplingShader.unBind();

            Vector2i resolution = new Vector2i( WindowHandler.resolution.X, WindowHandler.resolution.Y);

            for (int i = 0; i<downSamples; i++)
            {


                FrameBufferSettings settings = new FrameBufferSettings(resolution);
                DrawBufferSettings drawSettings = new DrawBufferSettings(FramebufferAttachment.ColorAttachment0);
                drawSettings.formatInternal = PixelInternalFormat.Rgba16f;
                //drawSettings.formatExternal = PixelFormat.Rgb;
                drawSettings.pixelType = PixelType.Float;
                drawSettings.wrapMode = TextureWrapMode.ClampToEdge;
                drawSettings.minFilterType = TextureMinFilter.Linear;
                drawSettings.magFilterType = TextureMagFilter.Linear;
                settings.drawBuffers.Add(drawSettings);
                sampleFramebuffers[i] = new FrameBuffer(settings);

                resolution /= 2;
            }
        }

        public void applyBloom(ScreenQuadRenderer renderer, FrameBuffer gBuffer)
        {
            sampleFramebuffers[0].bind();
            bloomFilterShader.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderer.getLastOutputTexture());
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));
            renderer.render();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            bloomFilterShader.unBind();
            downsamplingShader.bind();
            for (int i = 0; i< downSamples-1; i++)
            {
                sampleFramebuffers[i+1].bind();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, sampleFramebuffers[i].getRenderAttachment(0));
                downsamplingShader.loadUniformVector2f("srcResolution", sampleFramebuffers[i].getResolution());
                downsamplingShader.loadUniformInt("mipLevel", i);
                renderer.render(clearColor: true);

            }


            upsamplingShader.bind();
            for (int i = downSamples-1; i > 0; i--)
            {
                upsamplingShader.loadUniformFloat("filterRadius", 0.0f);
                sampleFramebuffers[i-1].bind();
                if (i == 1 )
                {
                    renderer.getLastFrameBuffer().bind();
                    //renderer.stepToggle();
                }
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, sampleFramebuffers[i].getRenderAttachment(0));
                //upsamplingShader.loadUniformVector2f("srcResolution", sampleFramebuffers[i-1].getResolution());
                renderer.render(blend: true, clearColor: false);
            }
            //sampleFramebuffers[1].resolveToScreen();
        }
    }
}
