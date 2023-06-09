using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Shaders;
using SpaceEngine.Util;

namespace SpaceEngine.RenderEngine
{
    internal class DeferredLightPassRenderer
    {
        private ShaderProgram globalLightShader = new ShaderProgram("Simple_Vertex", "Global_Light_Fragment");
        private ShaderProgram pointLightShader = new ShaderProgram("Point_Light_Vertex", "Point_Light_Fragment");
        private ShaderProgram ambientOcclusionShader = new ShaderProgram("Simple_Vertex", "AmbientOcclusion_Fragment");
        private ShaderProgram ambientOcclusionBlurShader = new ShaderProgram("Simple_Vertex", "AmbientOcclusion_Blur_Fragment");

        public int noiseScale = 4;
        private const int kernelSize = 32;
        private Vector3[] kernelSamples;
        private int noiseTexture;

        public DeferredLightPassRenderer() {
            globalLightShader.bind();
            globalLightShader.loadUniformInt("gAlbedo", 0);
            globalLightShader.loadUniformInt("gNormal", 1);
            globalLightShader.loadUniformInt("gPosition", 2);
            globalLightShader.unBind();

            pointLightShader.bind();
            pointLightShader.loadUniformInt("gAlbedo", 0);
            pointLightShader.loadUniformInt("gNormal", 1);
            pointLightShader.loadUniformInt("gPosition", 2);
            pointLightShader.unBind();

            ambientOcclusionBlurShader.bind();
            ambientOcclusionBlurShader.loadUniformInt("ssaoInput", 0);
            ambientOcclusionBlurShader.unBind();

            ambientOcclusionShader.bind();
            ambientOcclusionShader.loadUniformInt("texNoise", 0);
            ambientOcclusionShader.loadUniformInt("gNormal", 1);
            ambientOcclusionShader.loadUniformInt("gPosition", 2);
            ambientOcclusionShader.unBind();

            kernelSamples = new Vector3[kernelSize];
            for (int i = 0; i < kernelSize; i++)
            {
                Vector3 sample = new Vector3(MyMath.rngMinusPlus(), MyMath.rngMinusPlus(), MyMath.rand.NextSingle());
                sample.Normalize();
                sample *= MyMath.rand.NextSingle();
                float scale = (float)i / kernelSize;
                scale = 0.1f + scale*scale * (1f - 0.1f);
                sample *= scale;
                kernelSamples[i] = sample;
            }

            var noisePixels = new float[3 * noiseScale * noiseScale];
            for (int i = 0; i < noiseScale * noiseScale; i++)
            {
                noisePixels[i*3] = MyMath.rngMinusPlus();
                noisePixels[i*3+1] = MyMath.rngMinusPlus();
                noisePixels[i*3+2] = 0;
            }
            noiseTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, noiseScale, noiseScale, 0, PixelFormat.Rgb, PixelType.Float, noisePixels);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Repeat);

        }

        private void renderAmbientOcclusion(ScreenQuadRenderer renderer, FrameBuffer gBuffer, Matrix4 projectionMatrix)
        {
            ambientOcclusionShader.bind();
            ambientOcclusionShader.loadUniformVector2f("noiseScale", new Vector2(WindowHandler.resolution.X/ noiseScale, WindowHandler.resolution.Y/ noiseScale));
            ambientOcclusionShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            ambientOcclusionShader.loadUniformVector3fArray("samples", kernelSamples);

            ambientOcclusionShader.loadUniformFloat("radius", 0.8f);
            ambientOcclusionShader.loadUniformFloat("strength", 7.5f);
            ambientOcclusionShader.loadUniformFloat("bias", 0.04f);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(1));
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));

            renderer.renderToNextFrameBuffer();

            ambientOcclusionShader.unBind();


            ambientOcclusionBlurShader.bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderer.getLastOutputTexture());

            gBuffer.bind();
            GL.ColorMask(0, false, false, false, true);
            GL.ColorMask(1, false, false, false, false);
            GL.ColorMask(2, false, false, false, false);
            renderer.render();  
            GL.ColorMask(0,true, true, true, true);
            GL.ColorMask(1,true, true, true, true);
            GL.ColorMask(2,true, true, true, true);
            ambientOcclusionBlurShader.unBind();
        }

            private void renderGlobalLight(ScreenQuadRenderer renderer, FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix)
        {
            globalLightShader.bind();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(1));
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));
            Vector4 sunPosViewSpace = new Vector4(sunPosition.X, sunPosition.Y, sunPosition.Z, 1.0f) * viewMatrix;
            globalLightShader.loadUniformVector3f("sunPositionViewSpace", sunPosViewSpace.Xyz);
            renderer.renderToNextFrameBuffer();

            globalLightShader.unBind();

        }
        private void renderPointLights(FrameBuffer gBuffer, ComponentSystem pointLights, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            pointLightShader.bind();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            GL.DepthFunc(DepthFunction.Greater);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(1));
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));
            pointLightShader.loadUniformMatrix4f("viewMatrix", viewMatrix);
            pointLightShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            pointLightShader.loadUniformFloat("gScreenSizeX", WindowHandler.resolution.X);
            pointLightShader.loadUniformFloat("gScreenSizeY", WindowHandler.resolution.Y);

            foreach (PointLight pointLight in pointLights.getMembers())
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(pointLight.owner.getComponent<Transformation>());
                Model model = pointLight.lightVolumeModel;
                pointLightShader.loadUniformMatrix4f("TransformationMatrix", transformationMatrix);
                pointLightShader.loadUniformVector3f("attenuation", pointLight.attenuation);
                pointLightShader.loadUniformVector3f("lightColor", pointLight.color);

                Vector3 lightPositiobn = pointLight.owner.getComponent<Transformation>().position;
                Vector4 lightPositionViewSpace = new Vector4(lightPositiobn.X, lightPositiobn.Y, lightPositiobn.Z, 1.0f) * viewMatrix;
                pointLightShader.loadUniformVector3f("lightPositionViewSpace", lightPositionViewSpace.Xyz);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }

            GL.DepthFunc(DepthFunction.Less);
            pointLightShader.unBind();
            GL.CullFace(CullFaceMode.Back);
        }
        public void render(ScreenQuadRenderer renderer, FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix, Matrix4 projectionMatrix, ComponentSystem pointLights)
        {
            renderAmbientOcclusion(renderer, gBuffer, projectionMatrix);
            renderer.getNextFrameBuffer().blitDepthBufferFrom(gBuffer);
            renderer.getLastFrameBuffer().blitDepthBufferFrom(gBuffer);
            renderGlobalLight(renderer, gBuffer, sunPosition, viewMatrix);
            renderPointLights(gBuffer, pointLights, viewMatrix, projectionMatrix);
        }
    }


}
