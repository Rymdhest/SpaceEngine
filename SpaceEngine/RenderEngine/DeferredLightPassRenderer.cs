using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceEngine.Shaders;

namespace SpaceEngine.RenderEngine
{
    internal class DeferredLightPassRenderer
    {
        private ShaderProgram globalLightShader = new ShaderProgram("Simple_Vertex", "Global_Light_Fragment");
        public ScreenQuadRenderer quadRenderer = new ScreenQuadRenderer();
        public DeferredLightPassRenderer() {
            globalLightShader.bind();

            globalLightShader.loadUniformInt("gAlbedo", 0);
            globalLightShader.loadUniformInt("gNormal", 1);
            globalLightShader.loadUniformInt("gPosition", 2);

            globalLightShader.stop();
        }

        public void render(FrameBuffer gBuffer, Vector3 sunPosition, Matrix4 viewMatrix)
        {
            prepare(sunPosition, viewMatrix);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(0));
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(1));
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, gBuffer.getRenderAttachment(2));
            quadRenderer.renderToNextFrameBuffer();

            finish();
        }
        private void prepare(Vector3 sunPosition, Matrix4 viewMatrix)
        {
            globalLightShader.bind();

            Vector4 sunPosViewSpace = new Vector4(sunPosition.X, sunPosition.Y, sunPosition.Z, 1.0f) * viewMatrix;
            globalLightShader.loadUniformVector3f("sunPositionViewSpace", new Vector3(sunPosViewSpace.X, sunPosViewSpace.Y, sunPosViewSpace.Z));
            
        }
        private void finish()
        {
            globalLightShader.stop();
        }
    }


}
