using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Shaders;

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
            quadRenderer.renderToNextFrameBuffer();
            Vector4 sunPosViewSpace = new Vector4(sunPosition.X, sunPosition.Y, sunPosition.Z, 1.0f) * viewMatrix;
            globalLightShader.loadUniformVector3f("sunPositionViewSpace", new Vector3(sunPosViewSpace.X, sunPosViewSpace.Y, sunPosViewSpace.Z));

            globalLightShader.stop();

        }
        private void renderPointLights(FrameBuffer gBuffer, List<Entity> pointLights, Matrix4 viewMatrix)
        {
            pointLightShader.bind();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.Blend);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(1));
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));

            foreach (Entity entity in pointLights)
            {

            }

            pointLightShader.stop();
        }
        public void render(FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix, List<Entity> pointLights)
        {
            renderGlobalLight(gBuffer, sunPosition, viewMatrix);
            renderPointLights(gBuffer, pointLights, viewMatrix);
        }
    }


}
