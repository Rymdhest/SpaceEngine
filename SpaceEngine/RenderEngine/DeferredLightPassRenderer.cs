using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
        public ScreenQuadRenderer quadRenderer = new ScreenQuadRenderer();
        public DeferredLightPassRenderer() {
            globalLightShader.bind();
            globalLightShader.loadUniformInt("gAlbedo", 0);
            globalLightShader.loadUniformInt("gNormal", 1);
            globalLightShader.loadUniformInt("gPosition", 2);
            globalLightShader.stop();

            pointLightShader.bind();
            pointLightShader.loadUniformInt("gAlbedo", 0);
            pointLightShader.loadUniformInt("gNormal", 1);
            pointLightShader.loadUniformInt("gPosition", 2);
            pointLightShader.stop();
        }
        private void renderGlobalLight(FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix)
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
            quadRenderer.renderToNextFrameBuffer();

            globalLightShader.stop();

        }
        private void renderPointLights(FrameBuffer gBuffer, List<Entity> pointLights, Matrix4 viewMatrix, Matrix4 projectionMatrix)
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

            foreach (Entity entity in pointLights)
            {
                PointLight pointLight = entity.getComponent<PointLight>();
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(entity.getComponent<Transformation>());
                Model model = entity.getComponent<PointLight>().lightVolumeModel;
                pointLightShader.loadUniformMatrix4f("TransformationMatrix", transformationMatrix);
                pointLightShader.loadUniformVector3f("attenuation", pointLight.attenuation);
                pointLightShader.loadUniformVector3f("lightColor", pointLight.color);

                Vector3 lightPositiobn = entity.getComponent<Transformation>().position;
                Vector4 lightPositionViewSpace = new Vector4(lightPositiobn.X, lightPositiobn.Y, lightPositiobn.Z, 1.0f) * viewMatrix;
                pointLightShader.loadUniformVector3f("lightPositionViewSpace", lightPositionViewSpace.Xyz);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }

            GL.DepthFunc(DepthFunction.Less);
            pointLightShader.stop();
            GL.CullFace(CullFaceMode.Back);
        }
        public void render(FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix, Matrix4 projectionMatrix, List<Entity> pointLights)
        {
            quadRenderer.getNextFrameBuffer().blitDepthBufferFrom(gBuffer);
            quadRenderer.getLastFrameBuffer().blitDepthBufferFrom(gBuffer);
            renderGlobalLight(gBuffer, sunPosition, viewMatrix);
            renderPointLights(gBuffer, pointLights, viewMatrix, projectionMatrix);
        }
    }


}
