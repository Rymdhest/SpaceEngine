using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;

namespace SpaceEngine.RenderEngine
{
    internal class PostProcessingRenderer
    {

        private ShaderProgram FXAAShader = new ShaderProgram("Simple_Vertex", "FXAA_Fragment");
        private ShaderProgram verticalBlurShader = new ShaderProgram("blur_Vertical_Vertex", "blur_Fragment");
        private ShaderProgram horizontalBlurShader = new ShaderProgram("blur_Horizontal_Vertex", "blur_Fragment");
        private ShaderProgram bloomFilterShader = new ShaderProgram("Simple_Vertex", "bloom_Filter_Fragment");
        private ShaderProgram combineShader = new ShaderProgram("Simple_Vertex", "Combine_Fragment");
        private ShaderProgram skyShader = new ShaderProgram("Simple_Vertex", "sky_Fragment");
        private FrameBuffer vBlurFBO;
        private FrameBuffer hBlurFBO;
        private FrameBuffer bloomFilterFBO;

        public PostProcessingRenderer()
        {

            FXAAShader.bind();
            FXAAShader.loadUniformInt("l_tex", 0);
            FXAAShader.unBind();

            bloomFilterShader.bind();
            bloomFilterShader.loadUniformInt("gDiffuse", 0);
            bloomFilterShader.loadUniformInt("gPosition", 1);
            bloomFilterShader.unBind();

            combineShader.bind();
            combineShader.loadUniformInt("texture0", 0);
            combineShader.loadUniformInt("texture1", 1);
            combineShader.unBind();

            FrameBufferSettings frameBufferSettings = new FrameBufferSettings(WindowHandler.resolution/4);
            frameBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment0));

            vBlurFBO = new FrameBuffer(frameBufferSettings);
            hBlurFBO = new FrameBuffer(frameBufferSettings);

            FrameBufferSettings bloomFrameBufferSettings = new FrameBufferSettings(WindowHandler.resolution);
            bloomFrameBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment0));
            bloomFilterFBO = new FrameBuffer(bloomFrameBufferSettings);
        }

        public void doPostProcessing(ScreenQuadRenderer renderer, FrameBuffer gBuffer, Entity sunEntity, Vector3 viewPosition, Matrix4 viewMatrix)
        {
            applyFXAA(renderer);
            applyBloom(renderer, gBuffer);
            applySky(renderer, gBuffer, sunEntity, viewPosition, viewMatrix);
            
        }

        private void applySky(ScreenQuadRenderer renderer, FrameBuffer gBuffer, Entity sunEntity, Vector3 viewPosition, Matrix4 viewMatrix)
        {
            //renderer.getNextFrameBuffer().blitDepthBufferFrom(gBuffer);
            //renderer.getLastFrameBuffer().blitDepthBufferFrom(gBuffer);

            Sun sun = sunEntity.getComponent<Sun>();

            skyShader.bind();
            skyShader.loadUniformVector3f("viewPositionWorld", viewPosition);
            skyShader.loadUniformMatrix4f("viewMatrix", viewMatrix);
            skyShader.loadUniformVector2f("screenResolution", WindowHandler.resolution);
            //Vector4 sunDirectionViewSpace = new Vector4(sunDirection.X, sunDirection.Y, sunDirection.Z, 1.0f) * Matrix4.Transpose(Matrix4.Invert(viewMatrix));
            skyShader.loadUniformVector3f("sunDirectionWorldSpace", sun.getDirection());

            skyShader.loadUniformVector3f("skyColorGround", sun.getSkyColorGround());
            skyShader.loadUniformVector3f("skyColorSpace", sun.getSkyColorSpace());
            skyShader.loadUniformVector3f("sunColorGlare", sun.getSunScatterColor());
            skyShader.loadUniformVector3f("sunColor", sun.getFullSunColor());
            skyShader.loadUniformFloat("sunSetFactor", sun.getSunsetFactor());


            renderer.getLastFrameBuffer().bind();

            GL.DepthFunc(DepthFunction.Lequal);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderer.getLastOutputTexture());

            renderer.render(depthTest :true, depthMask:false, blend :false, clearColor:false);
            //renderer.stepToggle();
            skyShader.unBind();

            GL.DepthFunc(DepthFunction.Less);
        } 

        private void applyFXAA(ScreenQuadRenderer renderer)
        {
            FXAAShader.bind();
            FXAAShader.loadUniformVector2f("win_size", WindowHandler.resolution);
            renderer.renderTextureToNextFrameBuffer(renderer.getLastOutputTexture());
            FXAAShader.unBind();
        }
        private void applyBloom(ScreenQuadRenderer renderer, FrameBuffer gBuffer)
        {
            bloomFilterFBO.bind();
            bloomFilterShader.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderer.getNextOutputTexture());
            //GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));
            renderer.render();
            bloomFilterShader.unBind();
            bloomFilterFBO.unbind();

            verticalBlurShader.bind();
            vBlurFBO.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bloomFilterFBO.getRenderAttachment(0));
            verticalBlurShader.loadUniformFloat("targetHeight", vBlurFBO.getResolution().Y);
            renderer.render();
            vBlurFBO.unbind();
            verticalBlurShader.unBind();

            horizontalBlurShader.bind();
            hBlurFBO.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, vBlurFBO.getRenderAttachment(0));
            horizontalBlurShader.loadUniformFloat("targetWidth", hBlurFBO.getResolution().X);
            renderer.render();
            hBlurFBO.unbind();
            horizontalBlurShader.unBind();

            combineShader.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, hBlurFBO.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, renderer.getLastOutputTexture());
            renderer.renderToNextFrameBuffer();
            combineShader.unBind();

            //MasterRenderer.simpleShader.bind();
            //renderer.renderTextureToNextFrameBuffer(hBlurFBO.getRenderAttachment(0));
            //screenQuadRenderer.renderTextureToScreen(geometryPassRenderer.gBuffer.getRenderAttachment(2));
            //MasterRenderer.simpleShader.unBind();
        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            vBlurFBO.resize(WindowHandler.resolution/4);
            hBlurFBO.resize(WindowHandler.resolution / 4);
            bloomFilterFBO.resize(WindowHandler.resolution);
        }
    }
}
